using System;
using System.Collections.Generic;

namespace Muhanok.Domain
{
    public enum PlayStyle { FullBody, UpperBody, Keyboard }

    // The player on this PC. In developer (exhibition) mode each run gets its own name instead.
    [Serializable]
    public sealed class PlayerProfile
    {
        public string Name = "";
        public string Weight = "";
        public double CalorieGoal = 50;
        public PlayStyle Style = PlayStyle.FullBody;
        public bool HasName => !string.IsNullOrWhiteSpace(Name);
        public string DisplayName => HasName ? Name.Trim() : "플레이어";
        public PlayerProfile Clone() => (PlayerProfile)MemberwiseClone();
    }

    public static class PlayStyleInfo
    {
        public static string Label(PlayStyle style) => style == PlayStyle.FullBody ? "전신" : style == PlayStyle.UpperBody ? "상체" : "키보드";
        // 2024 Adult Compendium: 22240 total-body active game 4 MET; upper-body active game about 2.5 MET; seated play 1.3 MET.
        public static double Met(PlayStyle style) => style == PlayStyle.FullBody ? 4 : style == PlayStyle.UpperBody ? 2.5 : 1.3;
        public static bool UsesCamera(PlayStyle style) => style != PlayStyle.Keyboard;
    }

    [Serializable]
    public sealed class RankingEntry
    {
        public string Name; public int Score; public float Distance; public int Coins; public string Style; public long Ticks;
        // RunScore.Version of Score. Records saved before the field existed load as 0 and become 1.
        public int ScoreVersion;
        public bool Ranked => ScoreVersion == RunScore.Version;
        public string Day => Leaderboard.DayKey(new DateTime(Ticks));
    }

    // Score ranking of every run on this PC; "today" is the same list filtered by day.
    // Older-formula records stay saved but are not ranked, since their scores are not comparable.
    public sealed class Leaderboard
    {
        public const int MaxEntries = 500;
        private readonly List<RankingEntry> entries = new List<RankingEntry>();
        public IReadOnlyList<RankingEntry> All => entries;
        public static string DayKey(DateTime time) => time.ToString("yyyyMMdd");
        public List<RankingEntry> Day(string day) => entries.FindAll(e => e.Day == day);
        public List<RankingEntry> View(bool todayOnly, DateTime now)
        {
            string day = DayKey(now);
            return entries.FindAll(e => e.Ranked && (!todayOnly || e.Day == day));
        }
        // Adds a run and returns its 1-based rank among all ranked runs.
        public int Add(RankingEntry entry)
        {
            entries.Add(entry); Sort(); Trim();
            return RankOf(entry, false);
        }
        // 1-based rank within all runs or within the entry's own day; 0 when it is not ranked.
        public int RankOf(RankingEntry entry, bool sameDay)
        {
            if (entry == null || !entry.Ranked || !entries.Contains(entry)) return 0;
            return View(sameDay, new DateTime(entry.Ticks)).IndexOf(entry) + 1;
        }
        public void Load(IEnumerable<RankingEntry> saved)
        {
            entries.Clear();
            if(saved!=null)foreach(var entry in saved)if(entry!=null){if(entry.ScoreVersion<=0)entry.ScoreVersion=1;entries.Add(entry);}
            Sort(); Trim();
        }
        // Best ranked score for one play style (and one name when given); 0 when there is none.
        public int BestScore(string style, string name = null)
        {
            int best = 0;
            foreach (var e in entries) if (e.Ranked && e.Style == style && (name == null || e.Name == name)) best = Math.Max(best, e.Score);
            return best;
        }
        public void ClearDay(string day) => entries.RemoveAll(e => e.Day == day);
        // Ranked runs first by score, then unranked older records in their old distance order; ties go to the earlier run.
        private void Sort() => entries.Sort((a, b) => a.Ranked != b.Ranked ? (a.Ranked ? -1 : 1)
            : a.Ranked && b.Score != a.Score ? b.Score.CompareTo(a.Score)
            : !a.Ranked && b.Distance != a.Distance ? b.Distance.CompareTo(a.Distance) : a.Ticks.CompareTo(b.Ticks));
        // The cap applies to ranked runs; older records are kept.
        private void Trim()
        {
            int ranked = entries.FindAll(e => e.Ranked).Count;
            if (ranked > MaxEntries) entries.RemoveRange(MaxEntries, ranked - MaxEntries);
        }
    }

    // Lifetime totals shown on the profile screen.
    [Serializable]
    public sealed class PlayerStats
    {
        public int Runs;
        public float BestDistance, TotalDistance;
        public double TotalKcal;
        public void Add(float distance, double kcal)
        {
            Runs++; TotalDistance += Math.Max(0f, distance); TotalKcal += Math.Max(0, kcal);
            BestDistance = Math.Max(BestDistance, distance);
        }
    }
}
