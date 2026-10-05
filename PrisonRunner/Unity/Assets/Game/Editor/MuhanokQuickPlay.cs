using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Muhanok.Editor
{
    public static class MuhanokQuickPlay
    {
        private const string ScenePath = "Assets/Game/Scenes/GameScene.unity";
        private static double nextRequestCheck;

        [InitializeOnLoadMethod]
        private static void WatchLaunchRequest()
        {
            if (UnityEngine.Application.isBatchMode) return;
            EditorApplication.update -= CheckLaunchRequest;
            EditorApplication.update += CheckLaunchRequest;
            EditorApplication.playModeStateChanged -= FocusGame;
            EditorApplication.playModeStateChanged += FocusGame;
        }

        private static void CheckLaunchRequest()
        {
            if (EditorApplication.timeSinceStartup < nextRequestCheck || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
            nextRequestCheck = EditorApplication.timeSinceStartup + .5;
            string request = Path.GetFullPath(Path.Combine(UnityEngine.Application.dataPath, "../Library/MuhanokQuickPlay.request"));
            if (!File.Exists(request)) return;
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.isPlaying = false;
                return;
            }
            File.Delete(request);
            PlayGameScene();
        }

        [MenuItem("Muhanok/Play GameScene", false, 0)]
        public static void PlayGameScene()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            EditorSceneManager.OpenScene(ScenePath);
            EditorApplication.playModeStateChanged -= FocusGame;
            EditorApplication.playModeStateChanged += FocusGame;
            EditorApplication.isPlaying = true;
        }

        [MenuItem("Muhanok/Play GameScene", true)]
        private static bool CanPlay() => !EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling;

        private static void FocusGame(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredPlayMode) return;
            EditorApplication.playModeStateChanged -= FocusGame;
            Type gameView = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
            if (gameView != null) EditorWindow.GetWindow(gameView).Focus();
            EditorApplication.delayCall+=MuhanokGameViewFit.Fit;
            UnityEngine.Debug.Log("MUHANOK_EDITOR_READY: Click the Game view to start. A/D, Space, S, W.");
        }
    }
}
