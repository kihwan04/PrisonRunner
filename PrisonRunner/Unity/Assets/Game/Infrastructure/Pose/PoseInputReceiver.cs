using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Muhanok.Application;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Infrastructure
{
    [Serializable]
    public sealed class MotionMessage { public string type, command; public float confidence; public double timestamp; }
    public sealed class MotionCommandBuffer
    {
        private readonly Queue<MotionCommand> commands = new Queue<MotionCommand>(32);
        public void Add(MotionCommand command) { if (commands.Count < 32) commands.Enqueue(command); }
        public bool Read(out MotionCommand command) { command = MotionCommand.Center; if (commands.Count == 0) return false; command = commands.Dequeue(); return true; }
        public void Clear() { commands.Clear(); }
    }
    public sealed class PoseGameInput : IGameInput
    {
        public readonly MotionCommandBuffer Buffer = new MotionCommandBuffer();
        public bool TryRead(out MotionCommand command) => Buffer.Read(out command);
        public void Clear() => Buffer.Clear();
    }
    public sealed class PoseInputReceiver : IDisposable
    {
        private readonly Queue<string> packets = new Queue<string>(64);
        private readonly object packetLock = new object();
        private readonly PoseGameInput input;
        private readonly float threshold, timeout, cooldown;
        private readonly double[] lastCommand = { -100, -100, -100, -100, -100, -100 };
        private UdpClient socket;
        private Thread thread;
        private volatile bool running;
        private double lastPacket = -100, lastAcceptedTimestamp = -1;
        private MotionCommand lastMotion = MotionCommand.Center;
        private bool hasPose, hasPacket, everConnected;
        public string Error { get; private set; }
        public string Status(double now) => Error != null || now - lastPacket > timeout ? "Lost" : hasPose ? "Connected" : everConnected ? "Lost" : "Calibrating";
        public double LastPacketTime => lastPacket;
        public MotionCommand RecognizedMotion => hasPose?lastMotion:MotionCommand.Center;
        public PoseInputReceiver(PoseGameInput pose, float confidence, float timeoutSeconds, float commandCooldown)
        { input = pose; threshold = confidence; timeout = timeoutSeconds; cooldown = commandCooldown; }
        public void Start(int port)
        {
            try
            {
                socket = new UdpClient(new IPEndPoint(IPAddress.Loopback, port));
                socket.Client.ReceiveTimeout = 250; running = true;
                thread = new Thread(Receive) { IsBackground = true, Name = "Muhanok Pose UDP" }; thread.Start();
            }
            catch (Exception error) when (error is SocketException || error is ArgumentException)
            { Error = error.Message; socket?.Close(); socket = null; }
        }
        private void Receive()
        {
            while (running)
            {
                try
                {
                    IPEndPoint remote = new IPEndPoint(IPAddress.Loopback, 0);
                    byte[] bytes = socket.Receive(ref remote);
                    if (bytes.Length > 2048) continue;
                    string json = Encoding.UTF8.GetString(bytes);
                    lock (packetLock) { if (packets.Count < 64) packets.Enqueue(json); }
                }
                catch (SocketException) { if (!running) break; }
                catch (ObjectDisposedException) { break; }
            }
        }
        public void Dispatch(double now)
        {
            for (int i = 0; i < 64; i++)
            {
                string json;
                lock (packetLock) { if (packets.Count == 0) break; json = packets.Dequeue(); }
                AcceptJson(json, now);
            }
            if (now - lastPacket > timeout)
            {
                hasPose = false; input.Clear(); lastAcceptedTimestamp = -1; lastMotion = MotionCommand.Center;
            }
        }
        public bool AcceptJson(string json, double now)
        {
            MotionMessage message;
            try { message = JsonUtility.FromJson<MotionMessage>(json); }
            catch (ArgumentException) { return false; }
            if (message == null || message.type != "motion" || string.IsNullOrEmpty(message.command)
                || float.IsNaN(message.confidence) || float.IsInfinity(message.confidence)
                || message.confidence < 0 || message.confidence > 1 || double.IsNaN(message.timestamp)
                || double.IsInfinity(message.timestamp) || message.timestamp < 0) return false;
            MotionCommand command;
            switch (message.command)
            {
                case "CENTER": case "IDLE": command = MotionCommand.Center; break;
                case "LEFT": command = MotionCommand.Left; break;
                case "RIGHT": command = MotionCommand.Right; break;
                case "JUMP": command = MotionCommand.Jump; break;
                case "CROUCH": command = MotionCommand.Crouch; break;
                case "HIGH_KNEE": command = MotionCommand.HighKnee; break;
                default: return false;
            }
            bool reconnect = !hasPacket || now - lastPacket > timeout;
            if (reconnect) { lastAcceptedTimestamp = -1; lastMotion = MotionCommand.Center; input.Clear(); }
            // Positive timestamps are monotonic. Zero is allowed for the supplied debug protocol.
            if (message.timestamp > 0 && message.timestamp <= lastAcceptedTimestamp) return false;
            if (message.timestamp > 0) lastAcceptedTimestamp = message.timestamp;
            lastPacket = now; hasPacket = true; hasPose = message.confidence >= threshold;
            if (!hasPose) { input.Clear(); lastMotion = MotionCommand.Center; return false; }
            everConnected = true;
            bool edgeCommand = command == MotionCommand.Left || command == MotionCommand.Right || command == MotionCommand.Jump;
            bool duplicate = edgeCommand && command == lastMotion;
            lastMotion = command;
            int index = (int)command;
            if (duplicate || now - lastCommand[index] < cooldown) return false;
            lastCommand[index] = now; input.Buffer.Add(command); return true;
        }
        public void Dispose()
        {
            running = false; socket?.Close(); thread?.Join(750); socket = null;
            lock (packetLock) packets.Clear(); input.Clear();
        }
    }
}
