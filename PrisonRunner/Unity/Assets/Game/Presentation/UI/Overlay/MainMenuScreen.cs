using System;
using System.Globalization;
using UnityEngine;

namespace Muhanok.Presentation
{
    public enum MenuAction { None, Play, Ranking, Shop, Profile, Identity, Settings, Quit }

    // Main menu over the live title scene: logo on the left, actions on the right, coins and player name on top.
    internal sealed class MainMenuScreen
    {
        static readonly CultureInfo Num = CultureInfo.InvariantCulture;
        static readonly string[] Labels = { "게임 시작", "랭킹", "상점", "프로필", "얼굴 로그인", "설정", "게임 종료" };
        static readonly MenuAction[] Actions = { MenuAction.Play, MenuAction.Ranking, MenuAction.Shop, MenuAction.Profile, MenuAction.Identity, MenuAction.Settings, MenuAction.Quit };

        readonly OverlaySkin skin;
        public MainMenuScreen(OverlaySkin overlaySkin) { skin = overlaySkin; }

        public MenuAction Draw(IHudData data, Texture2D logo)
        {
            var oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.identity;
            skin.Fill(new Rect(0, 0, Screen.width, Screen.height), Palette.A(Palette.Ground, 0.55f));
            GUI.matrix = oldMatrix;

            if (logo != null) GUI.DrawTexture(new Rect(40, 65, 485, 310), logo, ScaleMode.ScaleToFit, true);
            else skin.OutlinedText(new Rect(76, 170, 650, 115), "Mongscape", "Mongscape", 74, Palette.Cream, Palette.Outline, 2.5f, TextAnchor.UpperLeft);

            DrawTopBar(data);

            var action = MenuAction.None;
            for (int i = 0; i < Labels.Length; i++)
            {
                var r = new Rect(860, 142 + i * 68, 340, 58);
                bool primary = i == 0;
                bool pressed = primary
                    ? skin.Button(r, Labels[i], Palette.Gold, Color.clear, Palette.Outline, 26)
                    : skin.Button(r, Labels[i], Palette.A(Palette.Ground, 0.9f), Palette.Wood, Palette.Cream, 22);
                if (pressed) action = Actions[i];
            }

            if (data != null && data.DeveloperMode)
            {
                var badge = new Rect(40, 640, 330, 40);
                skin.BorderBox(badge, Palette.A(new Color(.45f, .08f, .05f), 0.92f), Palette.Gold, 2);
                skin.Text(badge, "개발자 모드 · 모든 아이템 해금", 16, Palette.Cream, TextAnchor.MiddleCenter, false, true);
            }
            return action;
        }

        void DrawTopBar(IHudData data)
        {
            if (data == null) return;
            var coins = new Rect(1030, 28, 170, 44);
            skin.BorderBox(coins, Palette.A(Palette.Ground, 0.9f), Palette.Wood, 2);
            skin.Circle(new Rect(coins.x + 10, coins.y + 9, 26, 26), Palette.Gold);
            skin.Text(new Rect(coins.x + 44, coins.y, coins.width - 54, coins.height), data.Wallet.Coins.ToString("N0", Num), 20, Palette.Cream, TextAnchor.MiddleRight);
            if (data.DeveloperMode) return;
            var player = new Rect(800, 28, 220, 44);
            skin.BorderBox(player, Palette.A(Palette.Ground, 0.9f), Palette.Wood, 2);
            skin.Text(player, data.SavedProfile.DisplayName, 18, Palette.Cream, TextAnchor.MiddleCenter, false, true);
        }
    }
}
