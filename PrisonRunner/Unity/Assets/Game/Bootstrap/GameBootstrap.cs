using Muhanok.Application;
using System.Collections;
using Muhanok.Content;
using Muhanok.Domain;
using Muhanok.Infrastructure;
using Muhanok.Presentation;
using Muhanok.Presentation.Map;
using UnityEngine;
using UnityEngine.Timeline;

namespace Muhanok.Bootstrap
{
    public sealed class GameBootstrap : MonoBehaviour, IHudData, ISettingsHost, IIdentityData
    {
        public GameSettings Settings;
        public MuhanokMapChunk[] ChunkPrefabs;
        public GameObject MonkeyPrefab, PolicePrefab, CartPrefab, PickaxePrefab, IdeaPrefab, DustPrefab, HitPrefab;
        public FirstPersonCameraRig CameraRig;
        public Camera OutputCamera;
        public TimelineAsset IntroTimeline;
        public GameObject HandsPrefab;
        public Texture2D TitleLogo;
        public Texture2D LoadingImage;
        public GameObject CartRidePrefab;
        public bool StartSetupOpen=>hud!=null&&hud.SetupOpen;
        public bool IsLoading=>hud!=null&&hud.Loading;
        public bool IsRevealing=>hud!=null&&hud.Revealing;
        public RunSession Session { get; private set; }
        public ChunkSpawner Map { get; private set; }
        public IntroSequenceDirector Intro { get; private set; }
        public PoseTracker Pose { get; private set; }
        public PlayerProgressStore Store { get; private set; }
        public RunnerView Hero { get; private set; }
        private RunnerView guard;
        private KeyboardGameInput keyboard;
        private PoseGameInput poseInput;
        private InputRouter router;
        private HUDPresenter hud;
        private GameAudio audioPlayer;
        private float caughtTime, runFade, runStartedAt = -10, caughtAt = -10;
        private ParticleSystem dust, hit;
        private bool previousStumble;
        // Who is playing (or about to). Runs started from the setup screen are recorded on the leaderboard.
        private PlayerProfile player = new PlayerProfile();
        private bool playerRun, bannerSounded, goSounded, lastNewBest;
        private RankingEntry lastEntry;
        private int lastTodayRank;
        private UserSettings userSettings, settingsDefaults;
        private GraphicsSettingsApplier graphics;
        private SettingsWindow settingsWindow;
        private PerformanceOverlay overlay;
        private int lastRank;
        private LocalProfileClient identity;
        private FaceAvatarView faceAvatar;
        public PoliceCaptureSequence Capture { get; private set; }
        public PolicePursuitCue PursuitCue { get; private set; }
        private float guardDistance=15;
        private bool automated;
        private readonly AdaptivePerformance adaptive = new AdaptivePerformance();
        private UserSettings effectiveGraphics;
        private bool menuCamera;
        private PlayStyle cameraBeforeMenu;
        private void Awake()
        {
            var flags=System.Environment.GetCommandLineArgs();
            automated=System.Array.IndexOf(flags,"--muhanok-smoke")>=0||System.Array.IndexOf(flags,"--muhanok-uicapture")>=0
                ||System.Array.IndexOf(flags,"--mongscape-review")>=0||System.Array.IndexOf(flags,"--mongscape-tests")>=0
                ||System.Array.IndexOf(flags,"--muhanok-capture")>=0||System.Array.IndexOf(flags,"--mongscape-recording")>=0;
            Session = new RunSession(Settings != null ? JsonUtility.FromJson<RunConfig>(JsonUtility.ToJson(Settings.Run)) : new RunConfig());
            if (ChunkPrefabs == null || ChunkPrefabs.Length < 5 || CameraRig == null || OutputCamera == null)
            { Debug.LogError("GameScene requires content references. Run Muhanok/Build GameScene."); enabled = false; return; }
            keyboard = new KeyboardGameInput(); poseInput = new PoseGameInput(); router = new InputRouter(Session, keyboard, poseInput);
            Pose = new PoseTracker(poseInput, Settings != null ? Settings.Camera : null, Settings != null ? Settings.Motion : null, () => Session.Runner.Lane, () => Session.Runner.Riding&&!(RouteSurface.ActivePlan?.ForkWindow(Session.Runner.Z)??false));
            Store = new PlayerProgressStore(DataRoot());
            player = Store.Profile.Clone();
            Hero = CreateActor("CHR_Monkey_Prisoner", MonkeyPrefab);
            guard = CreateActor("CHR_Police_Officer", PolicePrefab);
            Capture=gameObject.AddComponent<PoliceCaptureSequence>();Capture.Configure(Hero,guard);
            PursuitCue=gameObject.AddComponent<PolicePursuitCue>();PursuitCue.Configure(Session);
            identity=gameObject.AddComponent<LocalProfileClient>(); faceAvatar=Hero.gameObject.AddComponent<FaceAvatarView>();
            identity.Changed+=IdentityChanged;
            gameObject.AddComponent<CartRideView>().Configure(Session,Hero,CartRidePrefab);
            OutputCamera.GetComponent<CinematicFocus>()?.Configure(Session,Hero);
            var mapRoot = new GameObject("ChunkPool"); mapRoot.transform.SetParent(transform, false);
            Map = new ChunkSpawner(ChunkPrefabs, mapRoot.transform, Session, Settings != null ? Settings.Render : new RenderBudget());
            Map.Decorator = new RuntimeContent(mapRoot.transform, () => Store.Wallet.Available(), Settings != null ? Settings.ItemChancePerChunk : .35f);
            CameraRig.Configure(OutputCamera);
            if(OutputCamera.GetComponent<GameViewport>()==null) OutputCamera.gameObject.AddComponent<GameViewport>();
            if (HandsPrefab != null)
            {
                var hands = Instantiate(HandsPrefab, OutputCamera.transform);
                hands.GetComponent<FirstPersonHands>().Configure(Session);
            }
            var cart = CreateProp("PROP_MineCart", CartPrefab, transform);
            var tool = CreateProp("PROP_Pickaxe", PickaxePrefab, Hero.ToolAnchor);
            tool.transform.localPosition = Hero.Humanoid ? Vector3.zero : new Vector3(.05f, -.35f, .35f);
            if(Hero.Humanoid) { tool.transform.localRotation=Quaternion.Euler(0,0,-90); tool.transform.localScale=Vector3.one*.75f; }
            var bulb = CreateProp("VFX_LightBulb_Idea", IdeaPrefab, Hero.transform); bulb.transform.localPosition = new Vector3(0, 2.4f, 0);
            if(Hero.Humanoid) bulb.transform.localScale=Vector3.one*.65f;
            dust = CreateProp("VFX_Dust_Run", DustPrefab, Hero.transform).GetComponentInChildren<ParticleSystem>();
            hit = CreateProp("VFX_Hit_Stumble", HitPrefab, Hero.transform).GetComponentInChildren<ParticleSystem>();
            var cartAudio = cart.GetComponent<AudioSource>();
            Intro = gameObject.AddComponent<IntroSequenceDirector>();
            Intro.Configure(Session, Hero, guard, CameraRig, cart.transform, tool.transform, bulb, cartAudio, IntroTimeline);
            hud = gameObject.AddComponent<HUDPresenter>();
            hud.Map=Map; hud.Data=this; hud.PoliceAlert=()=>PursuitCue.Visible;
            hud.Configure(Session, RetrySamePlayer);
            hud.TitleLogo = TitleLogo;
            hud.LoadingImage=LoadingImage; hud.BeginRun=StartPlayerRun; hud.PauseRun=TogglePause; hud.NextPlayer=NextPlayer; hud.ToMenu=ExitToMenu;
            hud.TextInput=KeyboardGameInput.SetTextInput; hud.Quit=QuitGame;
            audioPlayer = gameObject.AddComponent<GameAudio>();
            audioPlayer.Configure(Session, Settings != null ? Settings.MusicVolume : .35f, Settings != null ? Settings.EffectsVolume : .7f);
            SetupSettings();
            if (userSettings.DeveloperMode) player = new PlayerProfile { Style = Store.Profile.Style };
            if(automated)player.Style=PlayStyle.Keyboard;
            Session.Flow.Changed += StateChanged;
            Session.Flow.Transition(GameFlowState.TitleIdle);
        }
        private void SetupSettings()
        {
            settingsDefaults = new UserSettings
            {
                MusicVolume = Settings != null ? Settings.MusicVolume : .35f, EffectsVolume = Settings != null ? Settings.EffectsVolume : .7f,
                LiteLandmarkModel = Settings != null && !Settings.Camera.FullLandmarkModel, CameraDevice = Settings != null ? Settings.Camera.DeviceName : "",
            };
            settingsDefaults.ApplyPreset(QualityPreset.Balanced);
            userSettings = UserSettingsStore.Load(settingsDefaults);
            graphics = new GraphicsSettingsApplier();
            overlay = gameObject.AddComponent<PerformanceOverlay>();
            overlay.PoseLine = () => Pose.UsesCamera ? Pose.Diagnostics : "";
            overlay.QualityLine = () => "화질 " + userSettings.Preset + " · 렌더 " + Mathf.RoundToInt((effectiveGraphics?.RenderScale ?? userSettings.RenderScale) * 100) + "% · 그림자 " + (effectiveGraphics?.Shadows ?? userSettings.Shadows) + (userSettings.AutoPerformance ? " · 자동 " + adaptive.Level : "") + (userSettings.FrameCap > 0 ? " · 상한 " + userSettings.FrameCap : "");
            var window = new GameObject("SettingsWindow"); window.transform.SetParent(transform, false);
            settingsWindow = window.AddComponent<SettingsWindow>();
            settingsWindow.Configure(this);
            settingsWindow.Closed += () => hud.Hidden = false;
            hud.OpenSettings = OpenSettings;
            hud.OpenIdentity=()=>{menuCamera=false;Pose.SetStyle(PlayStyle.UpperBody);hud.ShowIdentity();};
            ApplySettings(false);
        }
        public void OpenSettings()
        {
            if (settingsWindow == null || settingsWindow.IsOpen || hud.Loading) return;
            if (Session.Flow.State == GameFlowState.Running && !Session.Paused) TogglePause();
            hud.Hidden = true; settingsWindow.Open();
        }
        public void ApplySettings(bool save)
        {
            adaptive.Reset();
            effectiveGraphics=userSettings.Clone();
            graphics?.Apply(effectiveGraphics);
            if (Map != null) Map.DetailScale = userSettings.DetailDistance;
            audioPlayer?.SetVolumes(userSettings.MusicVolume, userSettings.EffectsVolume);
            Pose.Configure(userSettings.CameraDevice, userSettings.LiteLandmarkModel, userSettings.LaneSensitivity);
            Pose.SetInferenceRate(30);
            if (overlay != null) overlay.Visible = userSettings.PerformanceOverlay;
            Store.Wallet.UnlockAll = userSettings.DeveloperMode;
            if (save) UserSettingsStore.Save(userSettings);
        }
        private RunnerView CreateActor(string id, GameObject prefab)
        {
            var root = new GameObject(id); root.transform.SetParent(transform, false);
            var visual = CreateProp("VisualRoot", prefab, root.transform);
            var view = root.AddComponent<RunnerView>(); view.Configure(visual); return view;
        }
        private static GameObject CreateProp(string id, GameObject prefab, Transform parent)
        {
            GameObject result = prefab != null ? Instantiate(prefab, parent) : GameObject.CreatePrimitive(PrimitiveType.Capsule);
            result.name = id; result.transform.SetParent(parent, false);
            if (prefab == null) { var collider = result.GetComponent<Collider>(); if (collider != null) collider.enabled = false; }
            return result;
        }
        public void StartIntro()
        {
            if (Session.Flow.Transition(GameFlowState.IntroTimeline)) Intro.Begin();
        }
        public bool MenuOpen=>hud!=null&&hud.MenuOpen;
        public void OpenMainMenu()
        {
            if(Session.Flow.State!=GameFlowState.TitleIdle||hud.Loading) return;
            hud.OpenMainMenu();
        }
        // Straight to the run setup (tests, "다음 플레이어").
        public void OpenStartSetup()
        {
            if(Session.Flow.State!=GameFlowState.TitleIdle||hud.Loading) return;
            hud.OpenSetup();
        }
        // Direct start (tests, tools): keeps the current play style and does not enter the leaderboard.
        public void BeginRun(string weight)
        {
            if(Session.Flow.State!=GameFlowState.TitleIdle||hud.Loading||!Session.Workout.SetWeight(weight)) return;
            playerRun=false; Session.Workout.Met=PlayStyleInfo.Met(player.Style);
            StartCoroutine(PrepareRun(723));
        }
        // Start from the setup screen: the run is recorded on the leaderboard. Outside developer mode
        // the chosen play style becomes the profile's default.
        public void StartPlayerRun(PlayerProfile profile)
        {
            if(profile==null||Session.Flow.State!=GameFlowState.TitleIdle||hud.Loading||!Session.Workout.SetWeight(profile.Weight)) return;
            if(!userSettings.DeveloperMode&&profile.Style!=Store.Profile.Style){var saved=Store.Profile.Clone();saved.Style=profile.Style;Store.SaveProfile(saved);}
            player=profile; playerRun=true; KeyboardGameInput.SetTextInput(false);
            Session.Workout.Met=PlayStyleInfo.Met(profile.Style);
            Session.Workout.SetGoal(profile.CalorieGoal);
            menuCamera=false; Pose.SetStyle(profile.Style);
            StartCoroutine(PrepareRun(Settings != null && Settings.Run.RandomizeLayout ? Random.Range(1, int.MaxValue) : 723));
        }
        private IEnumerator PrepareRun(int seed)
        {
            Session.Flow.Transition(GameFlowState.LoadingIntro);
            hud.BeginLoading(); hud.LoadingProgress=.12f;
            // The player stands still during loading; that is the calibration pose.
            Pose.Recalibrate();
            yield return new WaitForSecondsRealtime(.25f);
            RouteSurface.UsePlan(new RoutePlan(Session.Config, seed));
            Session.Reset(); keyboard.Clear(); poseInput.Clear(); hud.LoadingProgress=.42f;
            yield return new WaitForSecondsRealtime(.25f);
            Map.Reset(seed); Map.AnimateGates(Session.Runner.Z); hud.LoadingProgress=.78f;
            yield return new WaitForSecondsRealtime(.25f);
            Intro.PrepareGameplay(); hud.LoadingProgress=1;
            yield return new WaitForSecondsRealtime(.25f);
            Session.Flow.Transition(GameFlowState.Running); hud.EndLoading();
        }
        public void TogglePause()
        {
            if(Session.Flow.State!=GameFlowState.Running) return;
            Session.Paused=!Session.Paused; Time.timeScale=Session.Paused?0:1;
            keyboard.Clear(); poseInput.Clear();
        }
        private void UpdateAdaptive(bool live)
        {
            if(automated || !userSettings.AutoPerformance)return;
            if(!adaptive.Tick(Time.unscaledDeltaTime,live && !hud.Revealing && UnityEngine.Application.isFocused,userSettings.FrameCap))return;
            adaptive.Apply(userSettings,effectiveGraphics);graphics.Apply(effectiveGraphics);
            Map.DetailScale=effectiveGraphics.DetailDistance;
            // Rate changes reuse the loaded model and do not restart the camera or invalidate calibration.
            Pose.SetInferenceRate(adaptive.Level>=2?20:30);
        }
        private void UpdateMenuCamera(bool menu)
        {
            bool needsCamera=!automated && userSettings.HandMenus && menu;
            if(needsCamera && !Pose.UsesCamera){cameraBeforeMenu=Pose.Style;menuCamera=true;Pose.SetStyle(PlayStyle.UpperBody);}
            else if(menuCamera && !needsCamera){menuCamera=false;Pose.SetStyle(cameraBeforeMenu);}
        }
        private void Update()
        {
            double now = Time.realtimeSinceStartupAsDouble;
            bool live = Session.Flow.State == GameFlowState.Running && !Session.Paused && !hud.Loading;
            bool menu=(Session.Flow.State==GameFlowState.TitleIdle || Session.Flow.State==GameFlowState.Result || Session.Paused || settingsWindow.IsOpen) && !hud.Loading && !hud.Revealing;
            UpdateMenuCamera(menu);
            UpdateAdaptive(live);
            Pose.Tick(now, live);
            GesturePointer.Feed(Pose.LastFrame,menu && userSettings.HandMenus && Pose.Tracking,now);
            OperatorKeys();
            TickInput();
            if (Session.Flow.State == GameFlowState.Caught && Time.timeScale < 1 && Time.realtimeSinceStartup - caughtAt > .7f) Time.timeScale = 1;
            if(Session.Paused||hud.Loading||hud.Revealing) return;
            float dt = Mathf.Min(Time.deltaTime, .1f);
            Intro.Tick(dt);
            if (Session.Flow.State == GameFlowState.IntroTimeline) hud.Fade = Mathf.Clamp01(1 - Intro.Elapsed * 3f);
            if (Session.Flow.State == GameFlowState.Running)
            {
                StartBannerSounds();
                float previousZ = Session.Runner.Z;
                Session.Tick(dt); Map.Tick(Session.Runner.Z); Hero.Apply(Session.Runner); Map.Evaluate(previousZ);
                Map.AnimateGates(Session.Runner.Z);
                bool exercising = Pose.UsesCamera ? Pose.Tracking && Pose.Reading.Active : true;
                int activity=Time.realtimeSinceStartupAsDouble-Pose.LastJumpTime<.65?1:Pose.Reading.Crouch?2:Pose.Reading.HighKnee?3:Pose.Reading.Lane!=0?4:0;
                Session.Workout.TickActivity(dt,Session.Flow.State==GameFlowState.Running,Pose.UsesCamera&&Pose.Tracking,exercising,activity);
                runFade += dt; hud.Fade = Mathf.Clamp01(runFade * 2f);
                if (dust != null && !dust.isPlaying) dust.Play();
                if (Session.Runner.Stumbling && !previousStumble && hit != null) hit.Play();
                previousStumble = Session.Runner.Stumbling;
                if (Session.Flow.State == GameFlowState.Running)
                {
                    float distance=Session.Runner.Slowing&&Session.BananaHits>0||Session.BananaPursuit?1.8f:Mathf.Lerp(15,1.8f,Session.Chase.Pressure);
                    guardDistance=Mathf.MoveTowards(guardDistance,distance,dt*18);
                    guard.Pose(new Vector3(Session.Runner.X, 0, Session.Runner.Z-guardDistance), !Session.Runner.Riding);
                    guard.Play("AN_Guard_ChaseRun");
                }
            }
            if (Session.Flow.State == GameFlowState.Caught)
            {
                caughtTime += Time.unscaledDeltaTime;
                if(Session.Reason==CaptureReason.BrokenRail)
                {
                    guard.SetVisible(false);
                    if(caughtTime>=1.6f)Session.Flow.Transition(GameFlowState.Result);
                }
                else
                {
                    Capture.Tick(Time.unscaledDeltaTime);
                    if(Capture.Complete)Session.Flow.Transition(GameFlowState.Result);
                }
            }
        }
        private void StartBannerSounds()
        {
            float banner = StartBanner;
            if (banner <= 0) return;
            if (!bannerSounded) { bannerSounded = true; audioPlayer.Countdown(false); }
            if (!goSounded && banner <= 1f && Pose.Status != PoseStatus.Calibrating) { goSounded = true; audioPlayer.Countdown(true); }
        }
        private void OperatorKeys()
        {
            if (KeyboardGameInput.SettingsPressed) { if (settingsWindow != null && settingsWindow.IsOpen) settingsWindow.Close(true); else OpenSettings(); }
            if (KeyboardGameInput.OverlayPressed) { userSettings.PerformanceOverlay = !userSettings.PerformanceOverlay; ApplySettings(true); }
            if (settingsWindow != null && settingsWindow.IsOpen) return;
            if (KeyboardGameInput.RecalibratePressed && !hud.MenuOpen) Pose.Recalibrate();
            if (KeyboardGameInput.DeveloperTogglePressed && Session.Flow.State == GameFlowState.TitleIdle) SetDeveloperMode(!userSettings.DeveloperMode);
            if (KeyboardGameInput.ClearRankingPressed && userSettings.DeveloperMode) { Store.ClearToday(); Debug.Log("Today's ranking cleared."); }
        }
        internal HUDPresenter Hud => hud;
        // Development builds can keep player data in another folder (--muhanok-data <dir>) for captures and tests.
        private static string DataRoot()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            var args = System.Environment.GetCommandLineArgs(); int at = System.Array.IndexOf(args, "--muhanok-data");
            if (at >= 0 && at + 1 < args.Length) return args[at + 1];
#endif
            return null;
        }
        // Developer (exhibition) mode: per-run names and every item unlocked. Switching it starts from a clean player.
        internal void SetDeveloperMode(bool on, bool save = true)
        {
            userSettings.DeveloperMode = on; Store.Wallet.UnlockAll = on;
            if (save) UserSettingsStore.Save(userSettings);
            player = on ? new PlayerProfile { Style = Store.Profile.Style } : Store.Profile.Clone();
            if(identity.Profile!=null)player.Name=identity.Profile.name;
            if (hud.MenuOpen) hud.OpenMainMenu();
            Debug.Log("Developer mode " + (on ? "on" : "off"));
        }
        public void TickInput()
        {
            keyboard.Poll();
            if(hud.Loading||hud.Revealing||(settingsWindow!=null&&settingsWindow.IsOpen)){keyboard.Clear();poseInput.Clear();return;}
            router.Tick();
            if (Session.Flow.State == GameFlowState.TitleIdle && !hud.BlocksStart && KeyboardGameInput.StartPressed) OpenMainMenu();
        }
        private void LateUpdate()
        {
            if (Session != null && (Session.Flow.State == GameFlowState.Running || Session.Flow.State == GameFlowState.Caught))
            {
                if(Session.Flow.State==GameFlowState.Caught&&Session.Reason!=CaptureReason.BrokenRail)CameraRig.FollowCapture(Hero.transform.position,Time.unscaledDeltaTime,Capture.ProneBlend);
                else if(Session.Runner.Riding)CameraRig.FollowCart(Hero.Head,Time.deltaTime,-Session.Runner.CartLean*12);
                else CameraRig.Follow(Hero.Head,Session.Runner.Crouching,Time.deltaTime,Session.Runner.Slowing?Mathf.Sin(Time.time*12)*2:0);
            }
        }
        private void StateChanged(GameFlowState state)
        {
            if (state == GameFlowState.Running) { runFade = 0; poseInput.Clear(); Pose.ResetCommands(); runStartedAt = Time.realtimeSinceStartup; bannerSounded = goSounded = false; }
            if (state == GameFlowState.Caught)
            {
                caughtTime = 0; guard.Play("AN_Guard_Catch"); Hero.Play("AN_Monkey_Caught");
                if(Session.Reason!=CaptureReason.BrokenRail)
                {
                    Capture.Begin();
                }
                if (dust != null) dust.Stop();
                // A short slow motion and red flash make the capture readable.
                caughtAt = Time.realtimeSinceStartup; Time.timeScale = .35f;
            }
            if (state == GameFlowState.Result)
            {
                Time.timeScale = 1;
                var args=System.Environment.GetCommandLineArgs();
                bool tooling=System.Array.IndexOf(args,"--muhanok-capture")>=0||System.Array.IndexOf(args,"--muhanok-smoke")>=0
                    ||System.Array.IndexOf(args,"--mongscape-review")>=0||System.Array.IndexOf(args,"--mongscape-tests")>=0
                    ||System.Array.IndexOf(args,"--muhanok-uicapture")>=0;
                lastEntry=null; lastRank=lastTodayRank=0; lastNewBest=false;
                if(!tooling&&playerRun)
                {
                    identity.SaveRun(Session);
                    var now=System.DateTime.Now;
                    string name=identity.Profile!=null?identity.Profile.name:!userSettings.DeveloperMode?Store.Profile.DisplayName
                        :player.HasName?player.Name.Trim():"플레이어 "+(Store.Board.Day(Leaderboard.DayKey(now)).Count+1);
                    lastEntry=new RankingEntry{Name=name,Score=Session.Score.Score,Distance=Session.Score.Distance,Coins=Session.Score.Coins,
                        Style=PlayStyleInfo.Label(player.Style),Ticks=now.Ticks};
                    lastNewBest=Session.Score.Distance>Store.Stats.BestDistance;
                    lastRank=Store.Record(lastEntry,Session.Workout.EstimatedKcal);
                    lastTodayRank=Store.Board.RankOf(lastEntry,true);
                    // Exhibition runs do not fill this PC's wallet.
                    if(!userSettings.DeveloperMode) Store.DepositCoins(Session.Score.Coins);
                }
            }
        }
        public void Retry()
        {
            if (!Session.Flow.Transition(GameFlowState.Retry)) return;
            Capture.ResetSequence();
            guardDistance=15;
            Session.Reset(); keyboard.Clear(); poseInput.Clear(); Map.Reset(); Intro.ResetSequence();
            Time.timeScale=1; hud.CloseSetup();
            if (dust != null) dust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (hit != null) hit.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            previousStumble = false; runFade = caughtTime = 0; hud.Fade = 1;
            Session.Flow.Transition(GameFlowState.TitleIdle);
        }
        // "다시 하기": the same player runs again with the same settings.
        public void RetrySamePlayer()
        {
            bool again = playerRun; var profile = player;
            Retry();
            if (again) StartPlayerRun(profile);
        }
        // "다음 플레이어" (developer mode): back to an empty setup screen for the next person.
        public void NextPlayer()
        {
            var style = player.Style;
            Retry();
            identity.SignOut(); faceAvatar.Apply(null);
            player = new PlayerProfile { Style = style == PlayStyle.Keyboard ? PlayStyle.FullBody : style };
            playerRun = false; OpenStartSetup();
        }
        // "메인으로" from the result or pause screen; a paused run ends without a record.
        public void ExitToMenu()
        {
            if (Session.Paused) { Session.Paused = false; Time.timeScale = 1; }
            playerRun = false;
            Retry();
            if (!userSettings.DeveloperMode) player = Store.Profile.Clone();
            OpenMainMenu();
        }
        private void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
        private void OnDestroy() { GesturePointer.Reset(); Time.timeScale=1; graphics?.Dispose(); Pose?.Dispose(); RouteSurface.UsePlan(null); KeyboardGameInput.SetTextInput(false); }

        // IHudData
        PlayerProfile IHudData.Player => player;
        PlayerProfile IHudData.SavedProfile => SignedIn?player:Store.Profile;
        bool IHudData.DeveloperMode => userSettings != null && userSettings.DeveloperMode;
        Texture IHudData.CameraTexture => Pose.CameraTexture;
        PoseFrame IHudData.PoseFrame => Pose.LastFrame;
        PoseStatus IHudData.PoseStatus => Pose.Status;
        string IHudData.PoseMessage => Pose.Message;
        string IHudData.PoseDiagnostics => userSettings != null && userSettings.PoseDiagnostics ? Pose.Diagnostics : "";
        MotionReading IHudData.Motion => Pose.Reading;
        float IHudData.SecondsSinceJump => (float)(Time.realtimeSinceStartupAsDouble - Pose.LastJumpTime);
        float IHudData.CalibrationProgress => Pose.CalibrationProgress;
        float IHudData.StepsPerMinute => Pose.StepsPerMinute;
        bool IHudData.BodyActive => Pose.Tracking && Pose.Reading.Active;
        PlayerWallet IHudData.Wallet => Store.Wallet;
        Leaderboard IHudData.Board => Store.Board;
        PlayerStats IHudData.Stats => Store.Stats;
        RankingEntry IHudData.LastEntry => lastEntry;
        int IHudData.LastRank => lastRank;
        int IHudData.LastTodayRank => lastTodayRank;
        bool IHudData.LastNewBest => lastNewBest;
        float IHudData.BestDistance => Store.Stats.BestDistance;
        public float StartBanner => Session.Flow.State == GameFlowState.Running ? Mathf.Max(0, 2.2f - (Time.realtimeSinceStartup - runStartedAt)) : 0;
        float IHudData.CaughtFlash => Session.Flow.State == GameFlowState.Caught ? Mathf.Clamp01(1 - (Time.realtimeSinceStartup - caughtAt) / .6f) : 0;
        void IHudData.PreviewStyle(PlayStyle style) { menuCamera=false; player.Style = style; Pose.SetStyle(style); }
        void IHudData.Unlock(ItemKind kind) => Store.Unlock(kind);
        void IHudData.SaveProfile(PlayerProfile profile)
        {
            var saved=profile.Clone();
            if(SignedIn)saved.Name=Store.Profile.Name;
            Store.SaveProfile(saved);
            if(!userSettings.DeveloperMode){player=profile.Clone();if(SignedIn)player.Name=identity.Profile.name;}
        }
        void IHudData.Recalibrate() => Pose.Recalibrate();

        private void IdentityChanged()
        {
            faceAvatar.Apply(identity.Avatar);
            if(identity.Profile!=null)player.Name=identity.Profile.name;
            else if(userSettings!=null&&!userSettings.DeveloperMode)player=Store.Profile.Clone();
        }

        // Local or central voluntary identity UI
        public bool IdentityBusy=>identity.Busy;
        public bool SignedIn=>identity.Profile!=null;
        public string IdentityMessage=>identity.Message;
        public Texture Portrait=>identity.Avatar;
        public void FaceLogin(bool enroll)=>identity.Authenticate(Pose.CameraTexture,enroll,player.Name);
        public void FaceLogout(bool delete)=>identity.SignOut(delete);

        // ISettingsHost
        UserSettings ISettingsHost.Settings => userSettings;
        UserSettings ISettingsHost.Defaults => settingsDefaults;
        System.Collections.Generic.IReadOnlyList<string> ISettingsHost.CameraDevices => PoseTracker.CameraDevices();
        IHudData ISettingsHost.Hud => this;
        void ISettingsHost.RefreshCamera() => Pose.RefreshCamera();
    }
}
