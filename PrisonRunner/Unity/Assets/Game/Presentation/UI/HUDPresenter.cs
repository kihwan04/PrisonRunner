using System;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation
{
    public enum MenuPage { None, Main, Setup, Profile, Shop, Ranking, Identity }

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
        public Action OpenIdentity;
        public Func<bool> PoliceAlert;
        private MenuPage identityBack=MenuPage.Main;
        private IdentityScreen identity;
        public bool IdentityOpen=>Page==MenuPage.Identity;
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
        private bool resultOpen, firstProfile;
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
            title = new TitleScreen(skin); setup = new SetupScreen(skin); loading = new LoadingScreen(skin); identity=new IdentityScreen(skin);
            menu = new MainMenuScreen(overlaySkin); profile = new ProfileScreen(overlaySkin); shop = new ShopScreen(overlaySkin); board = new LeaderboardScreen(overlaySkin);
            runHud = new RunHud(skin, new ProgressHud(overlaySkin)); pause = new PauseScreen(skin);
        }
        public void ShowPage(MenuPage page, bool first = false)
        {
            Page = page; firstProfile = page == MenuPage.Profile && first;
            if (page == MenuPage.Setup) setup.Open(Data);
            else if (page == MenuPage.Profile) profile.Open(Data, first);
            else if (page == MenuPage.Ranking) board.OpenBoard(Data);
            TextInput?.Invoke(page == MenuPage.Setup || page == MenuPage.Profile);
        }
        public void OpenMainMenu() => ShowPage(MenuPage.Main);
        public void ShowIdentity(){identityBack=Page==MenuPage.Setup?MenuPage.Setup:MenuPage.Main;ShowPage(MenuPage.Identity);}
        public void OpenSetup() => ShowPage(MenuPage.Setup);
        // "게임 시작": developer mode names each run on the setup screen; otherwise a new player names the profile first.
        private void RequestPlay()
        {
            if (Data != null && !Data.DeveloperMode && !Data.SavedProfile.HasName) ShowPage(MenuPage.Profile, true);
            else ShowPage(MenuPage.Setup);
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
                case MenuPage.Setup: setup.Draw(Data, BeginRun, OpenMainMenu, OpenSettings,OpenIdentity); break;
                case MenuPage.Profile: profile.Draw(Data, AfterProfileSaved, OpenMainMenu); break;
                case MenuPage.Shop: shop.Draw(Data, OpenMainMenu); break;
                case MenuPage.Ranking: board.Draw(Data, session, OpenMainMenu, "닫기", null, null); break;
                case MenuPage.Identity: identity.Draw(Data,Data as IIdentityData,()=>ShowPage(identityBack));break;
                case MenuPage.Main:
                    var action = menu.Draw(Data, TitleLogo);
                    if (action == MenuAction.Play) RequestPlay();
                    else if (action == MenuAction.Ranking) ShowPage(MenuPage.Ranking);
                    else if (action == MenuAction.Shop) ShowPage(MenuPage.Shop);
                    else if (action == MenuAction.Profile) ShowPage(MenuPage.Profile);
                    else if(action==MenuAction.Identity)OpenIdentity?.Invoke();
                    else if (action == MenuAction.Settings) OpenSettings?.Invoke();
                    else if (action == MenuAction.Quit) Quit?.Invoke();
                    break;
            }
        }
        // The first-run profile continues to the run setup; a later edit returns to the menu.
        private void AfterProfileSaved()
        {
            if (firstProfile) ShowPage(MenuPage.Setup);
            else OpenMainMenu();
        }
        private void OnDestroy() { skin?.Dispose(); overlaySkin?.Dispose(); }
    }
}
