using System;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation
{
    // Run setup: play style with a live camera check. Developer (exhibition) mode also asks each run's name and weight;
    // otherwise they come from the saved profile.
    internal sealed class SetupScreen
    {
        private readonly HudSkin skin;
        private readonly WorkoutEstimate probe = new WorkoutEstimate();
        private string name = "", weightText = "", validation = "";
        private PlayStyle style = PlayStyle.FullBody;
        private string goalText="50";
        private GUIStyle guideStyle;
        private static readonly string[] KeyboardLines = { "A / ←   왼쪽", "D / →   오른쪽", "Space   점프", "S / ↓   숙이기 (누르고 있기)", "W / ↑   무릎 들기 (계단)" };
        public SetupScreen(HudSkin hudSkin) { skin = hudSkin; }
        public void Open(IHudData data)
        {
            validation = "";
            var player = data?.Player;
            if (player != null) { name = player.Name; weightText = player.Weight; style = player.Style; goalText=player.CalorieGoal.ToString("0.##",System.Globalization.CultureInfo.InvariantCulture); }
            data?.PreviewStyle(style);
        }
        public void Draw(IHudData data, Action<PlayerProfile> beginRun, Action back, Action openSettings = null)
        {
            bool perRun = data != null && data.DeveloperMode;
            skin.Card(new Rect(0, 0, 1280, 720), Palette.A(Palette.Ground, .82f));
            skin.Panel(new Rect(150, 70, 980, 590), .97f);
            GUI.color = new Color(1, .78f, .32f); GUI.Label(new Rect(195, 92, 500, 50), "탈출 준비", skin.Style(36, FontStyle.Bold));
            if (openSettings != null && skin.FlatButton(new Rect(950, 92, 140, 44), "설정", false)) openSettings();
            if (back != null && skin.FlatButton(new Rect(800, 92, 140, 44), "뒤로", false)) back();
            GUI.color = Color.white;
            if (perRun)
            {
                GUI.Label(new Rect(195, 150, 200, 34), "이름 (순위표)", skin.Label);
                skin.Panel(new Rect(195, 186, 400, 50), 1f); GUI.SetNextControlName("MuhanokName"); name = GUI.TextField(new Rect(195, 186, 400, 50), name, 10, skin.Input);
                GUI.Label(new Rect(195, 246, 400, 34), "몸무게 (선택, 미입력 시 65kg)", skin.Label);
                skin.Panel(new Rect(195, 282, 300, 50), 1f); GUI.SetNextControlName("MuhanokWeight"); weightText = GUI.TextField(new Rect(195, 282, 300, 50), weightText, 7, skin.Input);
                GUI.Label(new Rect(510, 289, 70, 40), "kg", skin.Label);
            }
            else
            {
                skin.Panel(new Rect(195, 150, 400, 182), .95f);
                GUI.Label(new Rect(215, 166, 360, 40), data?.SavedProfile.DisplayName ?? "플레이어", skin.Style(30, FontStyle.Bold));
                GUI.Label(new Rect(215, 214, 360, 34), string.IsNullOrWhiteSpace(weightText) ? "몸무게 미입력 · 65kg으로 계산" : "몸무게 " + weightText + " kg", skin.Small);
                GUI.color = new Color(1, 1, 1, .7f); GUI.Label(new Rect(215, 280, 360, 34), data is IIdentityData account && account.SignedIn ? "닉네임과 몸무게는 메인 메뉴 > 내 정보에서 바꿉니다." : "회원가입하면 닉네임과 설정이 계정에 저장됩니다.", skin.Style(15, FontStyle.Normal)); GUI.color = Color.white;
            }
            GUI.Label(new Rect(195, 344, 300, 34), "플레이 방식", skin.Label);
            for (int i = 0; i < 3; i++)
            {
                var option = (PlayStyle)i;
                if (Choice(new Rect(195 + i * 136, 380, 128, 48), PlayStyleInfo.Label(option), option == style) && option != style)
                {
                    style = option; data?.PreviewStyle(style);
                }
            }
            guideStyle??=new GUIStyle(skin.Style(16,FontStyle.Normal)){wordWrap=true};
            GUI.Label(new Rect(195,434,420,60),Guide(style),guideStyle);
            bool exercise = PlayStyleInfo.UsesCamera(style);
            GUI.Label(new Rect(195,494,400,30),exercise?"칼로리 목표 직접 입력":"키보드 플레이",skin.Small);
            bool wasEnabled=GUI.enabled; GUI.enabled=wasEnabled&&exercise;
            GUI.SetNextControlName("MongscapeCalorieGoal");
            goalText=GUI.TextField(new Rect(195,532,300,44),goalText,7,skin.Input);
            GUI.Label(new Rect(510,536,90,36),"kcal",skin.Label);
            GUI.enabled=wasEnabled;
            GUI.Label(new Rect(195,588,420,48),exercise?"목표를 넘어도 계속 플레이 · 운동 추정치":"키보드에서는 운동 칼로리를 계산하지 않습니다.",skin.Style(16,FontStyle.Normal));
            GUI.color = new Color(1, .55f, .35f); GUI.Label(new Rect(640, 548, 440, 28), validation, skin.Small); GUI.color = Color.white;
            if (data != null && PlayStyleInfo.UsesCamera(style)) PosePreview.Draw(skin, new Rect(640, 150, 450, 340), data, true);
            else KeyboardGuide(new Rect(640, 150, 450, 340));
            if (skin.PrimaryButton(new Rect(640, 584, 450, 56), "시작하기"))
            {
                if (!probe.SetWeight(weightText)) { validation = "몸무게는 10~350kg 숫자로 입력해 주세요."; return; }
                if(!probe.SetGoal(goalText)&&exercise){validation="목표는 1~5000 kcal 숫자로 입력해 주세요.";return;}
                beginRun?.Invoke(new PlayerProfile { Name = name.Trim(), Weight = weightText.Trim(), Style = style,CalorieGoal=probe.GoalKcal });
            }
            if (data != null && PlayStyleInfo.UsesCamera(style) && skin.FlatButton(new Rect(640, 500, 450, 40), "자세 다시 맞추기 (R)", false)) data.Recalibrate();
        }
        private bool Choice(Rect rect, string text, bool selected)
        {
            skin.Card(rect, selected ? Palette.Gold : Palette.Panel);
            GUI.color = selected ? Palette.Outline : Palette.Cream;
            bool pressed = GUI.Button(rect, GUIContent.none, GUIStyle.none) || GesturePointer.Activate(rect,text);
            GUI.Label(rect, text, skin.Centered(22, FontStyle.Bold)); GUI.color = Color.white;
            return pressed;
        }
        private static string Guide(PlayStyle style) =>
            style == PlayStyle.FullBody ? "몸 전체가 보이게 서세요. 좌우로 이동, 점프, 숙이기, 무릎 높이 들기."
            : style == PlayStyle.UpperBody ? "허리 위가 보이게 서거나 앉으세요. 상체 기울이기, 두 손 들기 = 점프, 숙이기, 팔 크게 흔들기 = 무릎 들기."
            : "A/D 이동, Space 점프, S 숙이기, W 무릎 들기.";
        private void KeyboardGuide(Rect area)
        {
            skin.Panel(area, .9f);
            for (int i = 0; i < KeyboardLines.Length; i++) GUI.Label(new Rect(area.x + 40, area.y + 40 + i * 52, area.width - 80, 40), KeyboardLines[i], skin.Style(25, FontStyle.Normal));
        }
    }
}
