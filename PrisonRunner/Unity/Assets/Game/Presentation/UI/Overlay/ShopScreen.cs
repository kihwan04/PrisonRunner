using System;
using System.Globalization;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation
{
    // Item shop: coins earned in runs unlock items that then appear on the map.
    internal sealed class ShopScreen
    {
        static readonly CultureInfo Num = CultureInfo.InvariantCulture;
        readonly OverlaySkin skin;
        public ShopScreen(OverlaySkin overlaySkin) { skin = overlaySkin; }

        public void Draw(IHudData data, Action back)
        {
            var oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.identity;
            skin.Fill(new Rect(0, 0, Screen.width, Screen.height), Palette.A(Palette.Ground, 0.9f));
            GUI.matrix = oldMatrix;

            float x = 230, w = 820;
            skin.Text(new Rect(x, 36, 300, 48), "상점", 44, Palette.Cream, TextAnchor.MiddleLeft);
            var wallet = data?.Wallet;
            var coins = new Rect(x + w - 190, 38, 190, 44);
            skin.BorderBox(coins, Palette.A(Palette.Ground, 0.9f), Palette.Wood, 2);
            skin.Circle(new Rect(coins.x + 10, coins.y + 9, 26, 26), Palette.Gold);
            skin.Text(new Rect(coins.x + 44, coins.y, coins.width - 58, coins.height), (wallet != null ? wallet.Coins : 0).ToString("N0", Num), 20, Palette.Cream, TextAnchor.MiddleRight);

            skin.Text(new Rect(x, 100, w, 26), wallet != null && wallet.UnlockAll
                ? "개발자 모드에서는 모든 아이템이 해금되어 있습니다."
                : "달리면서 모은 코인으로 아이템을 해금하면 맵에 등장합니다.", 16, Palette.Sand, TextAnchor.MiddleLeft, false, true);

            float cardW = (w - 32) / 3f;
            for (int i = 0; i < ItemCatalog.All.Length; i++)
            {
                var item = ItemCatalog.All[i];
                var r = new Rect(x + i * (cardW + 16), 150, cardW, 400);
                bool owned = wallet != null && wallet.IsUnlocked(item.Kind);
                skin.BorderBox(r, Palette.A(Palette.Panel, 0.95f), owned ? Palette.Gold : Palette.Wood, 2);
                skin.Circle(new Rect(r.center.x - 48, r.y + 34, 96, 96), Palette.A(Accent(item.Kind), 0.9f));
                skin.Text(new Rect(r.x, r.y + 150, r.width, 36), item.Name, 28, Palette.Cream, TextAnchor.MiddleCenter);
                skin.Text(new Rect(r.x + 20, r.y + 196, r.width - 40, 24), item.Description, 16, Palette.Sand, TextAnchor.MiddleCenter, false, true);
                skin.Text(new Rect(r.x, r.y + 226, r.width, 24), item.Seconds.ToString("0", Num) + "초 지속", 16, Palette.Sand, TextAnchor.MiddleCenter, false, true);
                var button = new Rect(r.x + 24, r.yMax - 82, r.width - 48, 56);
                if (owned)
                {
                    skin.BorderBox(button, Palette.A(Palette.Ground, 0.9f), Palette.Gold, 2);
                    skin.Text(button, "보유 중", 20, Palette.Gold, TextAnchor.MiddleCenter);
                }
                else
                {
                    bool affordable = wallet != null && wallet.Coins >= item.UnlockCost;
                    string label = "해금  " + item.UnlockCost.ToString("N0", Num) + "코인";
                    if (affordable ? skin.Button(button, label, Palette.Gold, Color.clear, Palette.Outline, 20)
                                   : skin.Button(button, label, Palette.A(Palette.Ground, 0.9f), Palette.WoodDim, Palette.Muted, 20))
                        if (affordable) data.Unlock(item.Kind);
                }
            }

            if (skin.Button(new Rect(540, 606, 200, 56), "뒤로", Palette.A(Palette.Ground, 0.9f), Palette.Wood, Palette.Cream, 22)) back?.Invoke();
        }

        static Color Accent(ItemKind kind) =>
            kind == ItemKind.Magnet ? new Color(.75f, .2f, .18f) : kind == ItemKind.DoubleScore ? new Color(.9f, .68f, .12f) : new Color(.2f, .5f, .85f);
    }
}
