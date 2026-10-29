using System;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation
{
    // Corner counters and one contextual instruction. Detailed metrics live in pause/results.
    internal sealed class RunHud
    {
        private readonly HudSkin skin;
        private readonly ProgressHud progress;
        public readonly RunHudModel Model = new RunHudModel();
        private static readonly Rect NoticeArea = new Rect(400, 588, 480, 48);
        public RunHud(HudSkin hudSkin, ProgressHud progressHud) { skin = hudSkin; progress = progressHud; }
        public void Draw(RunSession session, float fade, IHudData data, Action pause)
        {
            GUI.color = new Color(1, 1, 1, fade);
            // Only the pause control handles input events; all decoration draws on Repaint.
            if (skin.FlatButton(new Rect(1190, 24, 64, 56), "Ⅱ", false)) pause?.Invoke();
            if (Event.current.type != EventType.Repaint) { GUI.color = Color.white; return; }
            progress.Draw(session, data?.BestDistance ?? 0);
            skin.Card(new Rect(24, 24, 128, 48), Palette.A(Palette.Ground, .82f));
            GUI.DrawTexture(new Rect(35, 33, 29, 29), skin.CoinIcon);
            GUI.Label(new Rect(75, 30, 75, 36), Model.Coins, skin.Style(26, FontStyle.Bold));
            if (Model.ShowWorkout)
            {
                skin.Card(new Rect(870, 120, 300, 48), Palette.A(Palette.Ground, .82f));
                GUI.Label(new Rect(886, 126, 268, 32), Model.Workout, skin.Style(20, FontStyle.Normal));
                GUI.color = session.Workout.GoalReached ? new Color(.4f, 1, .6f) : Palette.Gold;
                GUI.DrawTexture(new Rect(886, 161, 268 * Model.GoalProgress, 3), Texture2D.whiteTexture);
                GUI.color = Color.white;
            }
            int row = 0;
            for (int i = 0; i < Model.ItemLabels.Length; i++)
            {
                if (Model.ItemLabels[i] == null) continue;
                var rect = new Rect(24, 86 + row++ * 34, 168, 29);
                skin.Card(rect, Palette.A(Palette.Ground, .82f));
                GUI.Label(rect, Model.ItemLabels[i], skin.Centered(16, FontStyle.Bold));
            }
            if (session.Flow.State == GameFlowState.Running)
            {
                if (Model.ShowPoseRecovery && data?.CameraTexture != null)
                    PosePreview.Draw(skin, new Rect(1020, 490, 236, 174), data, true);
                if (data == null || data.StartBanner <= 0) DrawNotice();
            }
            if (data != null) DrawBanners(data, session.Flow.State);
            if (session.Flow.State == GameFlowState.Caught)
            {
                skin.Card(NoticeArea, Palette.A(Palette.Ground, .9f));
                GUI.Label(NoticeArea, session.Reason == CaptureReason.BrokenRail ? "철로에서 추락!" : "경찰에게 잡혔어요!", skin.Centered(25, FontStyle.Bold));
            }
            GUI.color = Color.white;
        }
        private void DrawNotice()
        {
            if (Model.Notice == null) return;
            skin.Card(NoticeArea, Palette.A(Palette.Ground, .9f));
            GUI.color = Model.NoticeKind == HudNoticeKind.Tracking || Model.NoticeKind == HudNoticeKind.Police ? new Color(1, .7f, .3f) : Palette.Cream;
            GUI.Label(NoticeArea, Model.Notice, skin.Centered(23, FontStyle.Bold));
            GUI.color = Color.white;
        }
        private void DrawBanners(IHudData data, GameFlowState state)
        {
            if (data.StartBanner > 0 && state == GameFlowState.Running)
            {
                float opacity = Mathf.Clamp01(data.StartBanner * 2);
                bool calibrating = data.PoseStatus == PoseStatus.Calibrating || data.PoseStatus == PoseStatus.Searching;
                GUI.color = new Color(1, .82f, .3f, opacity);
                GUI.Label(new Rect(340, 265, 600, 90), calibrating ? "바르게 서 주세요" : data.StartBanner > 1 ? "준비!" : "출발!", skin.Centered(48, FontStyle.Bold));
            }
            if (data.CaughtFlash > 0)
            {
                GUI.color = new Color(.9f, .05f, .05f, data.CaughtFlash * .45f);
                GUI.DrawTexture(new Rect(0, 0, 1280, 720), Texture2D.whiteTexture);
            }
            GUI.color = Color.white;
        }
    }
}
