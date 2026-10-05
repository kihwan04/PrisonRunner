using System;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation
{
    public sealed partial class HUDPresenter : MonoBehaviour
    {
        private RunSession session;
        public Muhanok.Presentation.Map.ChunkSpawner Map;
        private Func<float> bestDistance;
        private Func<int> bestScore;
        private Func<string> poseStatus;
        private Action retry;
        private GUIStyle large, small, label, button, titlePrompt;
        private Font font;
        public float Fade { get; set; } = 1f;
        public Texture2D TitleLogo;
        public void Configure(RunSession run, Func<float> distance, Func<int> score, Func<string> pose, Action again)
        {
            session = run; bestDistance = distance; bestScore = score; poseStatus = pose; retry = again;
            font = Font.CreateDynamicFontFromOSFont(new[] { "Malgun Gothic", "Arial" }, 28);
        }
        private void OnGUI()
        {
            if (session == null) return;
            if (large == null)
            {
                large = Style(74, FontStyle.Bold); small = Style(20, FontStyle.Normal);
                label = Style(25, FontStyle.Bold); button = new GUIStyle(GUI.skin.button) { font = font, fontSize = 22 };
                titlePrompt=new GUIStyle(label) { alignment=TextAnchor.MiddleCenter };
            }
            Matrix4x4 old = GUI.matrix;
            GUI.matrix = ViewportLayout.GuiMatrix;
            var state = session.Flow.State;
            if(Loading||SetupOpen||Revealing)
            {
                DrawStartOverlay(); GUI.color=Color.white; GUI.matrix=old; return;
            }
            if (state == GameFlowState.TitleIdle)
            {
                GUI.color = new Color(1f, 1f, 1f, Fade);
                if (TitleLogo != null) GUI.DrawTexture(new Rect(40, 65, 485, 310), TitleLogo, ScaleMode.ScaleToFit, true);
                else GUI.Label(new Rect(76, 170, 650, 115), "무한옥", large);
                GUI.color = new Color(0,0,0,.65f*Fade); GUI.DrawTexture(new Rect(350,632,580,57),Texture2D.whiteTexture);
                GUI.color = new Color(1f, .88f, .61f, Fade);
                GUI.Label(new Rect(400, 641, 480, 42), "게임 시작", titlePrompt);
            }
            else if (state == GameFlowState.Running || state == GameFlowState.Caught)
            {
                GUI.color = new Color(1, 1, 1, Fade);
                DrawRunCards();
                GUI.color = Color.white;
                GUI.Label(new Rect(960, 641, 300, 42), "POSE  " + poseStatus(), small);
                GUI.Label(new Rect(30, 667, 860, 32),session.Runner.Riding?"A / D  좌우 기울이기 · 무너진 철로 반대쪽으로":"A / D  레인     SPACE  점프     S / ↓  슬라이드     W  하이니",small);
                if (state == GameFlowState.Caught) GUI.Label(new Rect(350, 294, 900, 100),session.Reason==CaptureReason.BananaPursuit?"경찰에게 잡혔어요!":session.Reason==CaptureReason.BrokenRail?"철로에서 추락!":"충돌!",Style(48,FontStyle.Bold));
                if(session.Paused) DrawPause();
            }
            else if (state == GameFlowState.Result)
            {
                Card(new Rect(355,112,570,520),new Color(.05f,.07f,.1f,.93f));
                GUI.color = new Color(1f, .2f, .1f); GUI.Label(new Rect(417, 151, 510, 90), "GAME OVER", StyleCachedResult());
                GUI.color = Color.white;
                GUI.Label(new Rect(408, 280, 510, 40), "Distance          " + session.Score.Distance.ToString("N0") + " m", label);
                GUI.Label(new Rect(408, 332, 510, 40), "Score               " + session.Score.Score.ToString("N0"), label);
                GUI.Label(new Rect(408, 388, 510, 40), "Best Distance   " + bestDistance().ToString("N0") + " m", small);
                GUI.Label(new Rect(408, 434, 510, 40), "Best Score        " + bestScore().ToString("N0"), small);
                if(session.Workout.WeightKg.HasValue) GUI.Label(new Rect(408,477,510,35),"예상 소모  "+session.Workout.EstimatedKcal.ToString("F1")+" kcal",small);
                if (GUI.Button(new Rect(423, 528, 435, 61), "다시 하기", button)) retry();
            }
            GUI.color = Color.white; GUI.matrix = old;
        }
        private GUIStyle resultStyle;
        private GUIStyle StyleCachedResult() { if (resultStyle == null) resultStyle = Style(48, FontStyle.Bold); return resultStyle; }
        private GUIStyle Style(int size, FontStyle weight) => new GUIStyle(GUI.skin.label) { font = font, fontSize = size, fontStyle = weight, normal = { textColor = Color.white } };
        private void OnDestroy() { if (font != null) Destroy(font);if(panelTexture!=null)Destroy(panelTexture);if(coinIcon!=null)Destroy(coinIcon);if(distanceIcon!=null)Destroy(distanceIcon); }
    }
}
