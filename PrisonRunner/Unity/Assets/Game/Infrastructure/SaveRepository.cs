using UnityEngine;

namespace Muhanok.Infrastructure
{
    public sealed class SaveRepository
    {
        public float BestDistance => PlayerPrefs.GetFloat("Muhanok.BestDistance", 0f);
        public int BestScore => PlayerPrefs.GetInt("Muhanok.BestScore", 0);
        public void Record(float distance, int score)
        {
            PlayerPrefs.SetFloat("Muhanok.BestDistance", Mathf.Max(BestDistance, distance));
            PlayerPrefs.SetInt("Muhanok.BestScore", Mathf.Max(BestScore, score));
            PlayerPrefs.Save();
        }
    }
}
