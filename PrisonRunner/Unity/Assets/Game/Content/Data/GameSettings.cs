using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Content
{
    [CreateAssetMenu(menuName = "Muhanok/Game Settings")]
    public sealed class GameSettings : ScriptableObject
    {
        public RunConfig Run = new RunConfig();
        public int UdpPort = 5055;
        public float Confidence = .65f, PoseTimeout = 2f, CommandCooldown = .35f;
    }
}
