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
        public string Workout { get; private set; } = "";
        public bool ShowWorkout { get; private set; }
        public string GoalLabel { get; private set; } = "";
        public string BurnLabel { get; private set; } = "";
        public string WorkoutHint { get; private set; } = "";
        public bool ShowPoseRecovery { get; private set; }
        public bool ShowSiren { get; private set; }
        public float GoalProgress { get; private set; }
        public string Notice { get; private set; }
        public HudNoticeKind NoticeKind { get; private set; }
        public readonly string[] ItemLabels = new string[ItemCatalog.All.Length];
        public int CueScans { get; private set; }
        public bool ShowFork {get;private set;}
        public bool ForkCommitted {get;private set;}
        public int ForkLane {get;private set;}
        public float ForkProgress {get;private set;}
        public string ForkCaption {get;private set;}
        private int forkDestination=99;
        private static readonly string[] ChoiceCaptions={"왼쪽 선택","가운데 선택","오른쪽 선택"};
        private readonly int[] itemSeconds = new int[ItemCatalog.All.Length];
        private int coins = -1, kcal = -1, streak;
        private double goal = -1;
        private int workoutMode = -1;
        private bool goalReached, previousPolice, previousRiding;
        private float nextScan, lastZ = -1, lastTime = -1, lostSince = -1, comboUntil, policeUntil, sectionUntil;
        private string actionCue, comboText;
        private int previousLane;

        public void Tick(RunSession session, ChunkSpawner map, PlayStyle style, PoseStatus pose, bool police, float now)
        {
            bool running = session.Flow.State == GameFlowState.Running && !session.Paused;
            // The beacon must remain visible even when another instruction has higher priority.
            ShowSiren = running && police;
            var plan=RouteSurface.ActivePlan;
            float lap=plan?.LapDistance(session.Runner.Z)??0;
            ShowFork=running && (plan?.Extended??false) && lap>=CartFork.Start && lap<CartFork.End;
            ForkCommitted=ShowFork && plan.ForkLocked(session.Runner.Z);
            ForkLane=ForkCommitted?plan.ChoiceAt(RouteSurface.SequenceAt(session.Runner.Z)):session.Runner.Lane;
            ForkProgress=Mathf.InverseLerp(CartFork.Start,CartFork.Decision,lap);
            if(ForkCommitted)
            {
                int destination=plan.DestinationAt(RouteSurface.SequenceAt(session.Runner.Z));
                if(destination!=forkDestination){forkDestination=destination;ForkCaption=RoutePlan.DestinationName(destination)+" 진입";}
            }
            else {forkDestination=99;ForkCaption=ChoiceCaptions[Mathf.Clamp(ForkLane+1,0,2)];}
            if (now < lastTime || session.Runner.Z < lastZ - .01f)
            {
                nextScan = comboUntil = policeUntil = sectionUntil = 0;
                actionCue = null; streak = 0; lostSince = -1; previousPolice = previousRiding = false;
            }
            lastZ = session.Runner.Z; lastTime = now;
            if (session.Score.Coins != coins) { coins = session.Score.Coins; Coins = coins.ToString(); }
            ShowWorkout = true;
            int mode = !PlayStyleInfo.UsesCamera(style) ? 0 : session.Workout.WeightKg.HasValue ? 2 : 1;
            GoalProgress = mode == 2 ? (float)session.Workout.GoalProgress : 0;
            if (ShowWorkout)
            {
                int nextKcal = mode == 2 ? (int)Math.Round(session.Workout.EstimatedKcal * 10) : 0;
                double nextGoal = session.Workout.GoalKcal;
                bool reached = mode == 2 && session.Workout.GoalReached;
                if (kcal != nextKcal || goal != nextGoal || reached != goalReached || workoutMode != mode)
                {
                    kcal = nextKcal; goal = nextGoal; goalReached = reached; workoutMode = mode;
                    GoalLabel = "목표  " + goal.ToString("0.##") + " kcal";
                    BurnLabel = "소모  " + (mode == 1 ? "—" : (kcal / 10.0).ToString("F1")) + " kcal" + (reached ? "  ✓" : "");
                    WorkoutHint = mode == 0 ? "키보드 · 운동량 미집계" : mode == 1 ? "몸무게 입력 후 계산" : reached ? "목표 달성!" : "동작 기반 추정치";
                    Workout = BurnLabel + " / " + GoalLabel;
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
                nextScan = 0; actionCue = null; ShowPoseRecovery = false; lostSince = -1;
                return;
            }
            bool lost = PlayStyleInfo.UsesCamera(style) && pose != PoseStatus.Tracking;
            if (!lost) lostSince = -1;
            else if (lostSince < 0) lostSince = now;
            ShowPoseRecovery = lost && now - lostSince >= .75f;
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
