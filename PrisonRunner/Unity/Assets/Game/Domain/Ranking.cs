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
        // Member id on the account server; empty for guest runs.
        public string AccountId = "";
        public string Day => Leaderboard.DayKey(new DateTime(Ticks));
    }

    // Distance ranking of every run on this PC; "today" is the same list filtered by day.
    public sealed class Leaderboard
    {
        public const int MaxEntries = 500;
        private readonly List<RankingEntry> entries = new List<RankingEntry>();
        public IReadOnlyList<RankingEntry> All => entries;
        public static string DayKey(DateTime time) => time.ToString("yyyyMMdd");
        public List<RankingEntry> Day(string day) => entries.FindAll(e => e.Day == day);
        public List<RankingEntry> View(bool todayOnly, DateTime now) => todayOnly ? Day(DayKey(now)) : new List<RankingEntry>(entries);
        // Adds a run and returns its 1-based rank among all runs.
        public int Add(RankingEntry entry)
        {
            entries.Add(entry); Sort();
            if (entries.Count > MaxEntries) entries.RemoveRange(MaxEntries, entries.Count - MaxEntries);
            return entries.IndexOf(entry) + 1;
        }
        // 1-based rank within all runs or within the entry's own day; 0 when it is not on the board.
        public int RankOf(RankingEntry entry, bool sameDay)
        {
            if (entry == null || !entries.Contains(entry)) return 0;
            if (!sameDay) return entries.IndexOf(entry) + 1;
            return Day(entry.Day).IndexOf(entry) + 1;
        }
        public void Load(IEnumerable<RankingEntry> saved)
        {
            entries.Clear();
            if(saved!=null)foreach(var entry in saved)if(entry!=null)entries.Add(entry);
            Sort();
            if(entries.Count>MaxEntries)entries.RemoveRange(MaxEntries,entries.Count-MaxEntries);
        }
        public void ClearDay(string day) => entries.RemoveAll(e => e.Day == day);
        // A deleted member's runs leave this PC's board too.
        public void RemoveAccount(string id) { if (!string.IsNullOrEmpty(id)) entries.RemoveAll(e => e.AccountId == id); }
        private void Sort() => entries.Sort((a, b) => b.Distance != a.Distance ? b.Distance.CompareTo(a.Distance) : a.Ticks.CompareTo(b.Ticks));
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
