using System;
using System.Linq;
using PrisonRunner.Application;
using PrisonRunner.Presentation.Map;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PrisonRunner.Editor
{
    [InitializeOnLoad]
    public static class Phase2SmokeCheck
    {
        private const string RunningKey = "PrisonRunner.Phase2Smoke.Running";
        private const string StepKey = "PrisonRunner.Phase2Smoke.Step";
        private const string ExitKey = "PrisonRunner.Phase2Smoke.Exit";
        private const string ExitPendingKey = "PrisonRunner.Phase2Smoke.ExitPending";
        private static int lastFrame = -1;

        static Phase2SmokeCheck()
        {
            EditorApplication.update += Check;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        [MenuItem("PrisonRunner/Run Phase 2 Smoke Check")]
        public static void Run()
        {
            SessionState.SetBool(RunningKey, true);
            SessionState.SetInt(StepKey, 0);
            SessionState.SetInt(ExitKey, 1);
            SessionState.SetBool(ExitPendingKey, false);
            EditorSceneManager.OpenScene("Assets/Game/Scenes/RunnerMVP.unity");
            EditorApplication.isPlaying = true;
        }

        private static void Check()
        {
            if (!SessionState.GetBool(RunningKey, false) || !EditorApplication.isPlaying || Time.frameCount < 3 || Time.frameCount == lastFrame)
            {
                return;
            }

            lastFrame = Time.frameCount;
            try
            {
                MapGenerator generator = UnityEngine.Object.FindFirstObjectByType<MapGenerator>();
                PlayerMovement player = UnityEngine.Object.FindFirstObjectByType<PlayerMovement>();
                if (generator == null || player == null)
                {
                    throw new Exception("MapGenerator or player is missing in Play mode.");
                }

                MapChunk[] chunks = UnityEngine.Object.FindObjectsByType<MapChunk>(FindObjectsSortMode.None)
                    .OrderBy(chunk => chunk.EntrySocket.position.z).ToArray();
                if (chunks.Length < 5)
                {
                    throw new Exception("Not enough active chunks ahead of the player.");
                }

                for (int i = 0; i < chunks.Length; i++)
                {
                    MapChunk chunk = chunks[i];
                    if (chunk.VisualRoot == null || chunk.GameplayRoot == null || chunk.VisualRoot == chunk.GameplayRoot)
                    {
                        throw new Exception("Visual and Gameplay roots are not separate.");
                    }

                    if (chunk.GameplayRoot.Find("GroundCollider")?.GetComponent<BoxCollider>() == null)
                    {
                        throw new Exception("Ground collider is missing.");
                    }

                    if (Mathf.Abs(chunk.ExitSocket.position.z - chunk.EntrySocket.position.z - MapChunk.Length) > 0.01f)
                    {
                        throw new Exception("Chunk length does not match its sockets.");
                    }

                    if (i > 0 && Mathf.Abs(chunks[i - 1].ExitSocket.position.z - chunk.EntrySocket.position.z) > 0.01f)
                    {
                        throw new Exception("Gap between chunks.");
                    }

                    for (int row = 0; row < MapChunk.RowCount; row++)
                    {
                        int occupied = 0;
                        for (int lane = -1; lane <= 1; lane++)
                        {
                            ObstacleSocket socket = chunk.GetObstacleSocket(row, lane);
                            if (socket == null || Mathf.Abs(socket.transform.position.x - lane * MapChunk.LaneSpacing) > 0.01f)
                            {
                                throw new Exception("Three-lane socket layout is invalid.");
                            }

                            occupied += socket.GetComponentsInChildren<RunnerObstacle>().Length;
                        }

                        if (occupied > 1)
                        {
                            throw new Exception("Unavoidable obstacle row detected.");
                        }
                    }
                }

                int step = SessionState.GetInt(StepKey, 0);
                if (step == 0 && Enum.GetValues(typeof(MapChunkKind)).Cast<MapChunkKind>().Any(kind => !chunks.Any(chunk => chunk.Kind == kind)))
                {
                    throw new Exception("One of the three placeholder chunk types is missing.");
                }

                if (step == 0)
                {
                    VisualSliceSmokeCheck.Validate(player, chunks);
                }

                if (step < 12)
                {
                    player.transform.position += Vector3.forward * MapChunk.Length;
                    SessionState.SetInt(StepKey, step + 1);
                    return;
                }

                int totalChunks = UnityEngine.Object.FindObjectsByType<MapChunk>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
                int totalObstacles = UnityEngine.Object.FindObjectsByType<RunnerObstacle>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
                if (totalChunks > 24 || totalObstacles > 24)
                {
                    throw new Exception("Chunk or obstacle instance count grew beyond the pooling bound.");
                }

                Debug.Log("PHASE2_SMOKE_PASS: " + chunks.Length + " active chunks, " + totalChunks + " total chunks, " + totalObstacles + " total obstacles.");
                Finish(0);
            }
            catch (Exception error)
            {
                Debug.LogError("PHASE2_SMOKE_FAIL: " + error);
                Finish(1);
            }
        }

        private static void Finish(int exitCode)
        {
            SessionState.SetBool(RunningKey, false);
            SessionState.SetInt(ExitKey, exitCode);
            SessionState.SetBool(ExitPendingKey, true);
            EditorApplication.isPlaying = false;
        }

        private static void OnPlayModeChanged(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.EnteredEditMode && SessionState.GetBool(ExitPendingKey, false) && UnityEngine.Application.isBatchMode)
            {
                SessionState.SetBool(ExitPendingKey, false);
                EditorApplication.Exit(SessionState.GetInt(ExitKey, 1));
            }
        }
    }
}
