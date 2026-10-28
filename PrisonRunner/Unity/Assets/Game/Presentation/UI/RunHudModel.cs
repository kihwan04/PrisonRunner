using System;
using Muhanok.Domain;
using Muhanok.Presentation.Map;
using UnityEngine;

namespace Muhanok.Presentation
{
    public enum HudNoticeKind { None, Tracking, Obstacle, Fork, Police, Combo, Section }

    // Sample in Update, then render cached values for all of Unity's GUI events.
    public sealed class RunHudModel
    {
        public const float CueInterval = .1f;
        public string Coins { get; private set; } = "0";
        // The double-coin item marks the coin counter with "×2" while it lasts.
        public bool CoinBoost { get; private set; }
        public string Workout { get; private set; } = "";
        public bool ShowWorkout { get; private set; }
        public bool ShowPoseRecovery { get; private set; }
        // Camera styles keep the webcam corner on screen unless the player turned it off in settings.
        public bool CameraEnabled { get; set; } = true;
        public bool ShowCamera { get; private set; }
        public float GoalProgress { get; private set; }
        public string Notice { get; private set; }
        public HudNoticeKind NoticeKind { get; private set; }
        public readonly string[] ItemLabels = new string[ItemCatalog.All.Length];
        public int CueScans { get; private set; }
        private readonly int[] itemSeconds = new int[ItemCatalog.All.Length];
        private int coins = -1, kcal = -1, goal = -1, streak;
        private bool goalReached, previousPolice, previousRiding;
        private float nextScan, lastZ = -1, lastTime = -1, lostSince = -1, comboUntil, policeUntil, sectionUntil;
        private string actionCue, comboText;
        private int previousLane;

        // Play time without pauses as "플레이 3:12"; display only, not part of the score.
        public static string PlayTime(float seconds)
        {
            int total = float.IsNaN(seconds) || float.IsInfinity(seconds) ? 0 : (int)Math.Max(0, Math.Floor(seconds));
            return "플레이 " + total / 60 + ":" + (total % 60).ToString("00");
        }
        public static string Points(int score) => score.ToString("N0", System.Globalization.CultureInfo.InvariantCulture) + "점";

        public void Tick(RunSession session, ChunkSpawner map, PlayStyle style, PoseStatus pose, bool police, float now)
        {
            bool running = session.Flow.State == GameFlowState.Running && !session.Paused;
            if (now < lastTime || session.Runner.Z < lastZ - .01f)
            {
                nextScan = comboUntil = policeUntil = sectionUntil = 0;
                actionCue = null; streak = 0; lostSince = -1; previousPolice = previousRiding = false;
            }
            lastZ = session.Runner.Z; lastTime = now;
            if (session.Score.Coins != coins) { coins = session.Score.Coins; Coins = coins.ToString(); }
            CoinBoost = session.Items.IsActive(ItemKind.DoubleScore);
            // Camera styles always estimate calories (an empty weight uses the default); keyboard play does not.
            ShowWorkout = PlayStyleInfo.UsesCamera(style);
            GoalProgress = (float)session.Workout.GoalProgress;
            if (ShowWorkout)
            {
                int nextKcal = (int)Math.Round(session.Workout.EstimatedKcal * 10), nextGoal = (int)session.Workout.GoalKcal;
                bool reached = session.Workout.GoalReached;
                if (kcal != nextKcal || goal != nextGoal || reached != goalReached)
                {
                    kcal = nextKcal; goal = nextGoal; goalReached = reached;
                    Workout = (kcal / 10.0).ToString("F1") + " / " + goal + " kcal" + (reached ? "  ✓" : "");
                }
            }
            for (int i = 0; i < ItemLabels.Length; i++)
            {
                var item = ItemCatalog.All[i];
                int seconds = session.Items.IsActive(item.Kind) ? Mathf.CeilToInt(session.Items.Remaining(item.Kind)) : 0;
                if (seconds == itemSeconds[i]) continue;
                itemSeconds[i] = seconds;
                ItemLabels[i] = seconds > 0 ? item.Name + " " + seconds + "초" : null;
            }
            Notice = null; NoticeKind = HudNoticeKind.None;
            if (!running)
            {
                nextScan = 0; actionCue = null; ShowPoseRecovery = ShowCamera = false; lostSince = -1;
                return;
            }
            bool lost = PlayStyleInfo.UsesCamera(style) && pose != PoseStatus.Tracking;
            if (!lost) lostSince = -1;
            else if (lostSince < 0) lostSince = now;
            ShowPoseRecovery = lost && now - lostSince >= .75f;
            ShowCamera = CameraEnabled && PlayStyleInfo.UsesCamera(style);
            if (session.DodgeStreak > streak && session.DodgeStreak > 1)
            {
                comboText = session.DodgeStreak + " COMBO"; comboUntil = now + 1.1f;
            }
            streak = session.DodgeStreak;
            if (police && !previousPolice) policeUntil = now + 2.2f;
            previousPolice = police;
            if (session.Runner.Riding != previousRiding)
            {
                sectionUntil = now + 1.8f; nextScan = 0;
                previousRiding = session.Runner.Riding;
            }
            // Lane changes refresh immediately; otherwise search at most ten times a second.
            if (previousLane != session.Runner.Lane) { previousLane = session.Runner.Lane; nextScan = 0; }
            if (now >= nextScan)
            {
                nextScan = now + CueInterval; CueScans++;
                actionCue = ActionCuePresenter.Find(session, map);
            }
            bool fork = RouteSurface.ActivePlan?.ForkWindow(session.Runner.Z) ?? false;
            NoticeKind = SelectNotice(ShowPoseRecovery, actionCue != null, fork, now < policeUntil, now < comboUntil, now < sectionUntil);
            switch (NoticeKind)
            {
                case HudNoticeKind.Tracking:
                    Notice = pose == PoseStatus.NoCamera || pose == PoseStatus.NoModel ? "카메라 연결 확인 · F1 설정" : "몸이 보이게 서 주세요 · R 재보정"; break;
                case HudNoticeKind.Obstacle: Notice = actionCue; break;
                case HudNoticeKind.Fork: Notice = style == PlayStyle.Keyboard ? "← A · C · D →  경로 선택" : "← 몸을 옮겨 경로 선택 →"; break;
                case HudNoticeKind.Police: Notice = "경찰 접근!"; break;
                case HudNoticeKind.Combo: Notice = comboText; break;
                case HudNoticeKind.Section: Notice = session.Runner.Riding ? "수레 구간" : "달리기 구간"; break;
            }
        }

        public static HudNoticeKind SelectNotice(bool trackingLost, bool obstacle, bool fork, bool police, bool combo, bool section)
            => trackingLost ? HudNoticeKind.Tracking : obstacle ? HudNoticeKind.Obstacle : fork ? HudNoticeKind.Fork
            : police ? HudNoticeKind.Police : combo ? HudNoticeKind.Combo : section ? HudNoticeKind.Section : HudNoticeKind.None;
    }
}
