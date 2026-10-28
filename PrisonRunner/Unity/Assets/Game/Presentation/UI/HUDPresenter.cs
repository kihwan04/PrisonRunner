using System;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation
{
    public enum MenuPage { None, Main, Setup, Profile, Shop, Ranking, Identity, Login, SignUp }

    // UI root: owns the menu page state and draws exactly one screen for the current game state.
    public sealed class HUDPresenter : MonoBehaviour
    {
        private RunSession session;
        public Muhanok.Presentation.Map.ChunkSpawner Map;
        private Action retry;
        public float Fade { get; set; } = 1f;
        public Texture2D TitleLogo;
        public Texture2D LoadingImage;
        public IHudData Data;
        public Action<PlayerProfile> BeginRun;
        public Action PauseRun;
        // Result and pause: back to the main menu; developer mode result: a fresh setup for the next player.
        public Action ToMenu;
        public Action NextPlayer;
        public Action OpenSettings;
        public Action Quit;
        public Func<bool> PoliceAlert;
        private IdentityScreen identity;
        private LoginGateScreen gate;
        private SignUpScreen signUp;
        // Sign-up step state; tests and tools can drive it without clicking.
        public SignUpFlow SignUp { get; } = new SignUpFlow();
        public bool IdentityOpen=>Page==MenuPage.Identity;
        // Login gate, sign-up and face login keep the upper-body camera on for hand pointing and face capture.
        public bool AccountPageOpen=>Page==MenuPage.Login||Page==MenuPage.SignUp||Page==MenuPage.Identity;
        private IIdentityData Account=>Data as IIdentityData;
        // Text fields need the IME; the bootstrap switches it.
        public Action<bool> TextInput;
        // The UI Toolkit settings window draws over the game; the IMGUI HUD hides meanwhile.
        public bool Hidden { get; set; }
        public MenuPage Page { get; private set; }
        public bool MenuOpen => Page != MenuPage.None;
        public bool SetupOpen => Page == MenuPage.Setup;
        public bool Loading { get; private set; }
        public bool Revealing => revealRemaining > 0;
        public float RevealOpacity => Mathf.SmoothStep(0, 1, revealRemaining / .85f);
        public bool BlocksStart => MenuOpen || Loading || Revealing;
        public float LoadingProgress { get; set; }
        private float revealRemaining;
        private bool resultOpen;
        private HudSkin skin;
        private OverlaySkin overlaySkin;
        private TitleScreen title;
        private MainMenuScreen menu;
        private SetupScreen setup;
        private ProfileScreen profile;
        private ShopScreen shop;
        private LeaderboardScreen board;
        private LoadingScreen loading;
        private RunHud runHud;
        private PauseScreen pause;
        public void Configure(RunSession run, Action again)
        {
            session = run; retry = again;
            skin = new HudSkin();
            overlaySkin = new OverlaySkin(Resources.Load<Font>("MuhanokUI/BlackHanSans-Regular"), null);
            title = new TitleScreen(skin); setup = new SetupScreen(skin); loading = new LoadingScreen(skin); identity=new IdentityScreen(skin); signUp=new SignUpScreen(skin,identity);
            menu = new MainMenuScreen(overlaySkin); gate = new LoginGateScreen(overlaySkin); profile = new ProfileScreen(overlaySkin); shop = new ShopScreen(overlaySkin); board = new LeaderboardScreen(overlaySkin);
            runHud = new RunHud(skin, new ProgressHud(overlaySkin)); pause = new PauseScreen(skin);
        }
        public void ShowPage(MenuPage page)
        {
            Page = page;
            if (page == MenuPage.Setup) setup.Open(Data);
            else if (page == MenuPage.Profile) profile.Open(Data, false);
            else if (page == MenuPage.Ranking) board.OpenBoard(Data);
            else if (page == MenuPage.Login) Account?.CheckServer();
            else if (page == MenuPage.SignUp) { SignUp.Open(Account); signUp.Open(); }
            TextInput?.Invoke(page == MenuPage.Setup || page == MenuPage.Profile || page == MenuPage.SignUp);
        }
        public void OpenMainMenu() => ShowPage(MenuPage.Main);
        public void OpenLogin() => ShowPage(MenuPage.Login);
        // Face login capture screen (from the login gate).
        public void ShowIdentity() => ShowPage(MenuPage.Identity);
        public void OpenSetup() => ShowPage(MenuPage.Setup);
        public void ChooseGuest() => OpenMainMenu();
        // A recognized face goes to the main menu; a failure stays for another try.
        public void LoginWithFace() => Account?.FaceLogin(ok => { if (ok && Page == MenuPage.Identity) OpenMainMenu(); });
        // Sign-up capture: the new member is signed in and goes to the main menu.
        public void EnrollFace() => SignUp.Capture(ok => { if (ok && Page == MenuPage.SignUp) OpenMainMenu(); });
        public void ChooseLogin(LoginAction action)
        {
            if (action == LoginAction.Face) ShowIdentity();
            else if (action == LoginAction.SignUp) ShowPage(MenuPage.SignUp);
            else if (action == LoginAction.Guest) ChooseGuest();
            else if (action == LoginAction.Settings) OpenSettings?.Invoke();
        }
        // Main menu actions. "게임 시작" goes straight to the run setup; the login entry signs a member out first.
        public void SelectMenu(MenuAction action)
        {
            if (action == MenuAction.Play) ShowPage(MenuPage.Setup);
            else if (action == MenuAction.Ranking) ShowPage(MenuPage.Ranking);
            else if (action == MenuAction.Shop) ShowPage(MenuPage.Shop);
            else if (action == MenuAction.Profile) ShowPage(MenuPage.Profile);
            else if (action == MenuAction.Identity) { if (Account != null && Account.SignedIn) Account.FaceLogout(false); OpenLogin(); }
            else if (action == MenuAction.Settings) OpenSettings?.Invoke();
            else if (action == MenuAction.Quit) Quit?.Invoke();
        }
        public void CloseSetup() { ShowPage(MenuPage.None); Loading = false; revealRemaining = 0; }
        public void BeginLoading() { ShowPage(MenuPage.None); Loading = true; }
        public void EndLoading() { Loading = false; revealRemaining = .85f; }
        public RunHudModel RunModel => runHud?.Model;
        private void Update()
        {
            revealRemaining = Mathf.Max(0, revealRemaining - Time.unscaledDeltaTime);
            if (session == null || runHud == null || Hidden || Loading || Revealing) return;
            if (session.Flow.State == GameFlowState.Running || session.Flow.State == GameFlowState.Caught)
                runHud.Model.Tick(session, Map, Data?.Player?.Style ?? PlayStyle.Keyboard, Data?.PoseStatus ?? PoseStatus.Off, PoliceAlert?.Invoke() ?? false, Time.unscaledTime);
        }
        private void OnGUI()
        {
            if (session == null || Hidden) return;
            Matrix4x4 old = GUI.matrix;
            GUI.matrix = ViewportLayout.GuiMatrix;
            var state = session.Flow.State;
            if (state != GameFlowState.Result) resultOpen = false;
            if (Loading || Revealing) loading.Draw(LoadingImage, LoadingProgress, Loading, RevealOpacity);
            else if (state == GameFlowState.TitleIdle) DrawMenu();
            else if (state == GameFlowState.Running || state == GameFlowState.Caught)
            {
                if (session.Paused) pause.Draw(session, Data, PauseRun, OpenSettings, ToMenu);
                else runHud.Draw(session, Fade, Data, PauseRun);
            }
            else if (state == GameFlowState.Result)
            {
                if (!resultOpen) { resultOpen = true; board.OpenResult(Data, session); }
                bool perRun = Data != null && Data.DeveloperMode;
                board.Draw(Data, session, retry, "다시 하기", perRun ? NextPlayer : ToMenu, perRun ? "다음 플레이어" : "메인으로");
            }
            GesturePointer.Draw();
            GUI.color = Color.white; GUI.matrix = old;
        }
        private void DrawMenu()
        {
            switch (Page)
            {
                case MenuPage.None: title.Draw(TitleLogo, Fade); break;
                case MenuPage.Setup: setup.Draw(Data, BeginRun, OpenMainMenu, OpenSettings); break;
                case MenuPage.Profile: profile.Draw(Data, OpenMainMenu, OpenMainMenu); break;
                case MenuPage.Shop: shop.Draw(Data, OpenMainMenu); break;
                case MenuPage.Ranking: board.Draw(Data, session, OpenMainMenu, "닫기", null, null); break;
                case MenuPage.Login: ChooseLogin(gate.Draw(Account, TitleLogo)); break;
                case MenuPage.SignUp: if (Account != null) signUp.Draw(Data, Account, SignUp, EnrollFace, OpenLogin); break;
                case MenuPage.Identity:
                    if (Account != null) identity.Draw(Data, Account, "얼굴 로그인", "정면을 보고 틀 안에 얼굴을 맞춘 뒤 [얼굴로 로그인]을 누르세요.\n가입한 얼굴이면 이름 입력 없이 로그인됩니다.", "얼굴로 로그인", LoginWithFace, OpenLogin);
                    break;
                case MenuPage.Main: SelectMenu(menu.Draw(Data, TitleLogo)); break;
            }
        }
        private void OnDestroy() { skin?.Dispose(); overlaySkin?.Dispose(); }
    }
}
