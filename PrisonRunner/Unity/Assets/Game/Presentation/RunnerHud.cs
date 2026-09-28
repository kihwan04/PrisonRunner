using PrisonRunner.Application;
using UnityEngine;

namespace PrisonRunner.Presentation
{
    public sealed class RunnerHud : MonoBehaviour
    {
        private DistanceScore score;
        private GUIStyle style;

        public void Configure(DistanceScore distanceScore)
        {
            score = distanceScore;
        }

        private void OnGUI()
        {
            if (score == null)
            {
                return;
            }

            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label);
                style.fontSize = 22;
                style.normal.textColor = Color.white;
            }

            GUI.Label(new Rect(20f, 20f, 380f, 35f), "DISTANCE  " + score.Metres + " m", style);
            GUI.Label(new Rect(20f, 55f, 600f, 35f), "A/D or Arrows: lane   Space: jump   S/Down: crouch", style);
        }
    }
}
