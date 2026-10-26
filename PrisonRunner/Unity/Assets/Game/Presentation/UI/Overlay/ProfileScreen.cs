using System;
using System.Globalization;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation
{
    // The PC's player profile (name, weight, default play style) and lifetime records.
    // On the first start it asks for a name before the run setup.
    internal sealed class ProfileScreen
    {
        static readonly CultureInfo Num = CultureInfo.InvariantCulture;
        readonly OverlaySkin skin;
        readonly WorkoutEstimate probe = new WorkoutEstimate();
        string name = "", weight = "", validation = "";
        PlayStyle style = PlayStyle.FullBody;
        bool firstRun;

        public ProfileScreen(OverlaySkin overlaySkin) { skin = overlaySkin; }

        public void Open(IHudData data, bool first)
        {
            firstRun = first; validation = "";
            var profile = data?.SavedProfile ?? new PlayerProfile();
            name = profile.Name; weight = profile.Weight; style = profile.Style;
        }

        // saved: called after a valid save (first run continues to the run setup); back: leave without saving.
        public void Draw(IHudData data, Action saved, Action back)
        {
            var oldMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.identity;
            skin.Fill(new Rect(0, 0, Screen.width, Screen.height), Palette.A(Palette.Ground, 0.9f));
            GUI.matrix = oldMatrix;

            float x = 230, w = 820;
            skin.Text(new Rect(x, 36, 600, 48), firstRun ? "처음 오셨네요!" : "프로필", 44, Palette.Cream, TextAnchor.MiddleLeft);
            if (firstRun) skin.Text(new Rect(x, 88, w, 24), "랭킹에 표시될 이름을 정해 주세요. 나중에 프로필에서 바꿀 수 있습니다.", 16, Palette.Sand, TextAnchor.MiddleLeft, false, true);

            skin.Text(new Rect(x, 130, 300, 28), "이름", 18, Palette.Sand, TextAnchor.MiddleLeft, false, true);
            GUI.SetNextControlName("MuhanokProfileName");
            bool faceName=data is IIdentityData identity && identity.SignedIn;
            bool enabled=GUI.enabled;GUI.enabled=enabled&&!faceName;
            name = skin.Input(new Rect(x,162,400,52),name,10);GUI.enabled=enabled;
            if(faceName)skin.Text(new Rect(x+420,162,370,52),"얼굴 이름은 서버 프로필을 사용합니다.",13,Palette.Sand,TextAnchor.MiddleLeft,false,true);
            skin.Text(new Rect(x, 232, 300, 28), "몸무게 (선택, 칼로리 계산용)", 18, Palette.Sand, TextAnchor.MiddleLeft, false, true);
            GUI.SetNextControlName("MuhanokProfileWeight");
            weight = skin.Input(new Rect(x, 264, 300, 52), weight, 7);
            skin.Text(new Rect(x + 312, 264, 60, 52), "kg", 20, Palette.Sand, TextAnchor.MiddleLeft);
            skin.Text(new Rect(x, 334, 300, 28), "기본 플레이 방식", 18, Palette.Sand, TextAnchor.MiddleLeft, false, true);
            for (int i = 0; i < 3; i++)
            {
                var option = (PlayStyle)i;
                var r = new Rect(x + i * 136, 366, 128, 48);
                bool on = option == style;
                if (skin.Button(r, PlayStyleInfo.Label(option), on ? Palette.Gold : Palette.A(Palette.Ground, 0.9f), on ? Color.clear : Palette.Wood, on ? Palette.Outline : Palette.Cream, 20))
                    style = option;
            }
            skin.Text(new Rect(x, 430, 420, 26), validation, 16, new Color(1, .55f, .35f), TextAnchor.MiddleLeft, false, true);

            if (!firstRun) DrawStats(data, new Rect(x + 450, 130, w - 450, 300));

            string primary = firstRun ? "다음" : "저장";
            if (skin.Button(new Rect(434, 606, 200, 56), primary, Palette.Gold, Color.clear, Palette.Outline, 22) && Validate())
            {
                data?.SaveProfile(new PlayerProfile { Name = name.Trim(), Weight = weight.Trim(), Style = style,CalorieGoal=data?.SavedProfile.CalorieGoal??50 });
                saved?.Invoke();
            }
            if (skin.Button(new Rect(646, 606, 200, 56), "뒤로", Palette.A(Palette.Ground, 0.9f), Palette.Wood, Palette.Cream, 22)) back?.Invoke();
        }

        bool Validate()
        {
            if (string.IsNullOrWhiteSpace(name)) { validation = "이름을 입력해 주세요."; return false; }
            if (!probe.SetWeight(weight)) { validation = "몸무게는 10~350kg 숫자로 입력해 주세요."; return false; }
            validation = ""; return true;
        }

        void DrawStats(IHudData data, Rect r)
        {
            skin.BorderBox(r, Palette.A(Palette.Panel, 0.95f), Palette.Wood, 2);
            skin.Text(new Rect(r.x + 24, r.y + 16, r.width - 48, 34), "내 기록", 24, Palette.Gold, TextAnchor.MiddleLeft);
            var stats = data?.Stats ?? new PlayerStats();
            string[] labels = { "최고 기록", "플레이 횟수", "총 달린 거리", "총 소모 칼로리" };
            string[] values =
            {
                Mathf.FloorToInt(stats.BestDistance).ToString("N0", Num) + "m",
                stats.Runs.ToString("N0", Num) + "회",
                (stats.TotalDistance / 1000f).ToString("N1", Num) + "km",
                stats.TotalKcal.ToString("N0", Num) + "kcal",
            };
            for (int i = 0; i < labels.Length; i++)
            {
                float y = r.y + 70 + i * 54;
                skin.Text(new Rect(r.x + 24, y, 200, 40), labels[i], 18, Palette.Sand, TextAnchor.MiddleLeft, false, true);
                skin.Text(new Rect(r.xMax - 224, y, 200, 40), values[i], 24, Palette.Cream, TextAnchor.MiddleRight);
            }
        }
    }
}
