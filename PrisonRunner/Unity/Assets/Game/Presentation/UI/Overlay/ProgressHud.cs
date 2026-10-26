using System.Globalization;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Presentation
{
    // Reuse one content object instead of retaining every distance string in the skin's dictionary.
    internal sealed class ProgressHud
    {
        private readonly OverlaySkin skin;
        private readonly GUIContent content = new GUIContent();
        private readonly GUIContent bestContent = new GUIContent();
        private int shownMeters = -1, shownBest = -1;
        public ProgressHud(OverlaySkin overlaySkin) { skin = overlaySkin; }
        public void Draw(RunSession session, float savedBest)
        {
            int meters = Mathf.FloorToInt(session.Score.Distance);
            if (meters != shownMeters)
            {
                shownMeters = meters;
                content.text = meters.ToString("N0", CultureInfo.InvariantCulture) + " m";
            }
            int best = Mathf.FloorToInt(Mathf.Max(session.Score.Distance, savedBest));
            if (best != shownBest)
            {
                shownBest = best;
                bestContent.text = "최고 기록 " + best.ToString("N0", CultureInfo.InvariantCulture) + " m";
            }
            // Keep the distance/best/goal group together beside the pause control, away from the road.
            skin.Box(new Rect(870, 24, 300, 88), Palette.A(Palette.Ground, .82f));
            GUI.Label(new Rect(886, 28, 268, 48), content, skin.Style(36, true, TextAnchor.MiddleRight));
            GUI.Label(new Rect(886, 77, 268, 26), bestContent, skin.Style(16, true, TextAnchor.MiddleRight));
        }
    }
}
