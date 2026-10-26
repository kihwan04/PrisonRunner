using System;
using Muhanok.Domain;
using Muhanok.Presentation.Map;
using UnityEngine;

namespace Muhanok.Presentation
{
    internal sealed class PauseScreen
    {
        private readonly HudSkin skin;
        public PauseScreen(HudSkin hudSkin) { skin = hudSkin; }
        // Leaving to the menu ends the run without a record.
        public void Draw(RunSession session, IHudData data, Action resume, Action settings, Action toMenu)
        {
            skin.Card(new Rect(0, 0, 1280, 720), Palette.A(Palette.Ground, .75f));
            skin.Panel(new Rect(285, 140, 710, 440), .97f);
            GUI.color = Color.white; GUI.Label(new Rect(325, 165, 620, 60), "잠깐 쉬어가기", skin.Style(34, FontStyle.Bold));
            GUI.Label(new Rect(325, 240, 340, 35), "거리  " + session.Score.Distance.ToString("N0") + " m", skin.Label);
            GUI.Label(new Rect(325, 283, 340, 30), "코인  " + session.Score.Coins + "   ·   최고 " + (data?.BestDistance ?? 0).ToString("N0") + " m", skin.Small);
            GUI.Label(new Rect(325, 325, 350, 30), "기준 속도 " + session.TargetSpeed.ToString("F1") + " m/s · " + RouteSurface.AreaAt(session.Runner.Z), skin.Style(18, FontStyle.Normal));
            bool workout = PlayStyleInfo.UsesCamera(data?.Player?.Style ?? PlayStyle.Keyboard) && session.Workout.WeightKg.HasValue;
            GUI.Label(new Rect(325, 367, 355, 30), workout ? "예상 " + session.Workout.EstimatedKcal.ToString("F1") + " / " + session.Workout.GoalKcal.ToString("F0") + " kcal" : "키보드/몸무게 미입력 · 칼로리 계산 제외", skin.Style(17, FontStyle.Normal));
            GUI.Label(new Rect(325, 409, 350, 30), session.Config.IntervalMode ? "구간 전환까지 " + session.PhaseRemainingSeconds.ToString("F0") + "초" : "최고 콤보 " + session.BestDodgeStreak, skin.Small);
            if (skin.PrimaryButton(new Rect(710, 247, 240, 58), "계속하기", 26)) resume?.Invoke();
            if (skin.FlatButton(new Rect(710, 325, 240, 48), "설정", false)) settings?.Invoke();
            if (skin.FlatButton(new Rect(710, 391, 240, 48), "메인으로", false)) toMenu?.Invoke();
            GUI.Label(new Rect(325, 500, 625, 30), "메인으로 나가면 이번 플레이 기록은 저장하지 않습니다.", skin.Style(17, FontStyle.Normal));
        }
    }
}
