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
    public sealed class GameBootstrap : MonoBehaviour
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
        public PoseInputReceiver Pose { get; private set; }
        public RunnerView Hero { get; private set; }
        private RunnerView guard;
        private KeyboardGameInput keyboard;
        private PoseGameInput poseInput;
        private InputRouter router;
        private SaveRepository save;
        private HUDPresenter hud;
        private float caughtTime, runFade;
        private ParticleSystem dust, hit;
        private bool previousStumble;
        private void Awake()
        {
            Session = new RunSession(Settings != null ? Settings.Run : new RunConfig());
            if (ChunkPrefabs == null || ChunkPrefabs.Length < 5 || CameraRig == null || OutputCamera == null)
            { Debug.LogError("GameScene requires content references. Run Muhanok/Build GameScene."); enabled = false; return; }
            keyboard = new KeyboardGameInput(); poseInput = new PoseGameInput(); router = new InputRouter(Session, keyboard, poseInput);
            Pose = new PoseInputReceiver(poseInput, Settings != null ? Settings.Confidence : .65f,
                Settings != null ? Settings.PoseTimeout : 2f, Settings != null ? Settings.CommandCooldown : .35f);
            Pose.Start(Settings != null ? Settings.UdpPort : 5055);
            save = new SaveRepository();
            Hero = CreateActor("CHR_Monkey_Prisoner", MonkeyPrefab);
            guard = CreateActor("CHR_Police_Officer", PolicePrefab);
            gameObject.AddComponent<CartRideView>().Configure(Session,Hero,CartRidePrefab);
            OutputCamera.GetComponent<CinematicFocus>()?.Configure(Session,Hero);
            var mapRoot = new GameObject("ChunkPool"); mapRoot.transform.SetParent(transform, false);
            Map = new ChunkSpawner(ChunkPrefabs, mapRoot.transform, Session);
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
            var audio = cart.GetComponent<AudioSource>();
            Intro = gameObject.AddComponent<IntroSequenceDirector>();
            Intro.Configure(Session, Hero, guard, CameraRig, cart.transform, tool.transform, bulb, audio, IntroTimeline);
            hud = gameObject.AddComponent<HUDPresenter>();
            hud.Map=Map;
            hud.Configure(Session, () => save.BestDistance, () => save.BestScore, () => Pose.Status(Time.realtimeSinceStartupAsDouble), Retry);
            hud.TitleLogo = TitleLogo;
            hud.LoadingImage=LoadingImage; hud.BeginRun=BeginRun; hud.PauseRun=TogglePause;
            Session.Flow.Changed += StateChanged;
            Session.Flow.Transition(GameFlowState.TitleIdle);
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
        public void OpenStartSetup(){if(Session.Flow.State==GameFlowState.TitleIdle&&!hud.Loading) hud.OpenSetup();}
        public void BeginRun(string weight)
        {
            if(Session.Flow.State!=GameFlowState.TitleIdle||hud.Loading||!Session.Workout.SetWeight(weight)) return;
            StartCoroutine(PrepareRun());
        }
        private IEnumerator PrepareRun()
        {
            Session.Flow.Transition(GameFlowState.LoadingIntro);
            hud.BeginLoading(); hud.LoadingProgress=.12f; yield return new WaitForSecondsRealtime(.25f);
            Session.Reset(); keyboard.Clear(); poseInput.Clear(); hud.LoadingProgress=.42f;
            yield return new WaitForSecondsRealtime(.25f);
            Map.Reset(); Map.AnimateGates(Session.Runner.Z); hud.LoadingProgress=.78f;
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
        private void Update()
        {
            Pose.Dispatch(Time.realtimeSinceStartupAsDouble);
            TickInput();
            if(Session.Paused||hud.Loading||hud.Revealing) return;
            float dt = Mathf.Min(Time.deltaTime, .1f);
            Intro.Tick(dt);
            if (Session.Flow.State == GameFlowState.IntroTimeline) hud.Fade = Mathf.Clamp01(1 - Intro.Elapsed * 3f);
            if (Session.Flow.State == GameFlowState.Running)
            {
                float previousZ = Session.Runner.Z;
                Session.Tick(dt); Hero.Apply(Session.Runner); Map.Tick(Session.Runner.Z); Map.Evaluate(previousZ);
                Map.AnimateGates(Session.Runner.Z);
                Session.Workout.Tick(dt,Session.Flow.State==GameFlowState.Running,Pose.Status(Time.realtimeSinceStartupAsDouble)=="Connected",Pose.RecognizedMotion);
                runFade += dt; hud.Fade = Mathf.Clamp01(runFade * 2f);
                if (dust != null && !dust.isPlaying) dust.Play();
                if (Session.Runner.Stumbling && !previousStumble && hit != null) hit.Play();
                previousStumble = Session.Runner.Stumbling;
                if (Session.Flow.State == GameFlowState.Running)
                {
                    guard.Pose(new Vector3(Session.Runner.X, 0, Session.Runner.Z - Mathf.Lerp(15, 1.8f, Session.Chase.Pressure)), !Session.Runner.Riding);
                    guard.Play("AN_Guard_ChaseRun");
                }
            }
            if (Session.Flow.State == GameFlowState.Caught)
            {
                caughtTime += dt;
                guard.PoseWorld(Vector3.Lerp(guard.transform.position,Hero.transform.position+Hero.transform.forward*2.2f+Hero.transform.right*.4f,dt*4),Session.Reason!=CaptureReason.BrokenRail);
                guard.transform.rotation=Quaternion.LookRotation(-Hero.transform.forward,Vector3.up);
                if (caughtTime >= .65f) Session.Flow.Transition(GameFlowState.Result);
            }
        }
        public void TickInput()
        {
            keyboard.Poll();
            if(hud.Loading||hud.Revealing){keyboard.Clear();poseInput.Clear();return;}
            router.Tick();
            if (Session.Flow.State == GameFlowState.TitleIdle && !hud.BlocksStart && KeyboardGameInput.StartPressed) OpenStartSetup();
        }
        private void LateUpdate()
        {
            if (Session != null && (Session.Flow.State == GameFlowState.Running || Session.Flow.State == GameFlowState.Caught))
            {
                if(Session.Runner.Riding)CameraRig.FollowCart(Hero.Head,Time.deltaTime,-Session.Runner.CartLean*12);
                else CameraRig.Follow(Hero.Head,Session.Runner.Crouching,Time.deltaTime,Session.Runner.Slowing?Mathf.Sin(Time.time*12)*2:0);
            }
        }
        private void StateChanged(GameFlowState state)
        {
            if (state == GameFlowState.Running) { runFade = 0; poseInput.Clear(); }
            if (state == GameFlowState.Caught)
            {
                caughtTime = 0; guard.Play("AN_Guard_Catch"); Hero.Play("AN_Monkey_Caught");
                if(Session.Reason!=CaptureReason.BrokenRail)
                {
                    guard.PoseWorld(Hero.transform.position+Hero.transform.forward*2.5f+Hero.transform.right*.8f,true);
                    guard.transform.rotation=Quaternion.LookRotation(-Hero.transform.forward,Vector3.up);
                }
                if (dust != null) dust.Stop();
            }
            if (state == GameFlowState.Result)
            {
                var args=System.Environment.GetCommandLineArgs();
                if(System.Array.IndexOf(args,"--muhanok-capture")<0&&System.Array.IndexOf(args,"--muhanok-smoke")<0)save.Record(Session.Score.Distance,Session.Score.Score);
            }
        }
        public void Retry()
        {
            if (!Session.Flow.Transition(GameFlowState.Retry)) return;
            Session.Reset(); keyboard.Clear(); poseInput.Clear(); Map.Reset(); Intro.ResetSequence();
            Time.timeScale=1; hud.CloseSetup();
            if (dust != null) dust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            if (hit != null) hit.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            previousStumble = false; runFade = caughtTime = 0; hud.Fade = 1;
            Session.Flow.Transition(GameFlowState.TitleIdle);
        }
        private void OnDestroy() {Time.timeScale=1; Pose?.Dispose(); }
    }
}
