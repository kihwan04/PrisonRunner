using System;
using System.Collections.Generic;
using System.IO;
using Muhanok.Domain;
using UnityEngine;

namespace Muhanok.Infrastructure
{
    // Player data on this PC: profile, wallet and unlocked items, the distance leaderboard and lifetime stats.
    // Everything goes through this class so a server-backed store can replace it later.
    public sealed class PlayerProgressStore
    {
        [Serializable] private sealed class WalletFile { public int Coins; public List<int> Unlocked = new List<int>(); }
        [Serializable] private sealed class RecordsFile { public List<RankingEntry> Entries = new List<RankingEntry>(); public PlayerStats Stats = new PlayerStats(); }
        [Serializable] private sealed class LegacyWallet { public int CoinPool; public List<int> Unlocked = new List<int>(); }
        private const string LegacyBestKey = "Muhanok.BestDistance";
        private readonly string folder;
        private readonly string legacyFolder;
        private readonly bool useLegacyPrefs;
        public PlayerProfile Profile { get; private set; } = new PlayerProfile();
        public readonly PlayerWallet Wallet = new PlayerWallet();
        public readonly Leaderboard Board = new Leaderboard();
        public PlayerStats Stats { get; private set; } = new PlayerStats();
        public PlayerProgressStore(string root = null)
        {
            folder = Path.Combine(root ?? UnityEngine.Application.persistentDataPath, "Player");
            legacyFolder = Path.Combine(root ?? UnityEngine.Application.persistentDataPath,"Exhibition");
            useLegacyPrefs=root==null;
            Profile = Read<PlayerProfile>(ProfilePath) ?? new PlayerProfile();
            LoadWallet(); LoadRecords();
        }
        private string ProfilePath => Path.Combine(folder, "profile.json");
        private string WalletPath => Path.Combine(folder, "wallet.json");
        private string RecordsPath => Path.Combine(folder, "records.json");
        public void SaveProfile(PlayerProfile profile) { Profile = profile.Clone(); Write(ProfilePath, Profile); }
        // Adds a finished run; returns its rank among all runs.
        public int Record(RankingEntry entry, double kcal)
        {
            int rank = Board.Add(entry);
            Stats.Add(entry.Distance, kcal);
            SaveRecords();
            return rank;
        }
        public void ClearToday() { Board.ClearDay(Leaderboard.DayKey(DateTime.Now)); SaveRecords(); }
        public void DepositCoins(int coins) { Wallet.Deposit(coins); SaveWallet(); }
        public bool Unlock(ItemKind kind) { bool done = Wallet.TryUnlock(kind); if (done) SaveWallet(); return done; }
        private void LoadWallet()
        {
            var file = Read<WalletFile>(WalletPath);
            if(file==null&&!File.Exists(WalletPath))
            {
                var legacy=Read<LegacyWallet>(Path.Combine(legacyFolder,"progress.json"));
                if(legacy!=null)file=new WalletFile{Coins=legacy.CoinPool,Unlocked=legacy.Unlocked};
            }
            if (file == null) return;
            var kinds = new List<ItemKind>(); foreach (int id in file.Unlocked) if (Enum.IsDefined(typeof(ItemKind), id)) kinds.Add((ItemKind)id);
            Wallet.Restore(file.Coins, kinds);
            if(!File.Exists(WalletPath))SaveWallet();
        }
        private void SaveWallet()
        {
            var file = new WalletFile { Coins = Wallet.Coins };
            foreach (var kind in Wallet.Unlocked) file.Unlocked.Add((int)kind);
            Write(WalletPath, file);
        }
        private void LoadRecords()
        {
            var file = Read<RecordsFile>(RecordsPath);
            bool migrate=file==null&&!File.Exists(RecordsPath)&&Directory.Exists(legacyFolder);
            if(migrate)
            {
                file=new RecordsFile();
                foreach(string path in Directory.GetFiles(legacyFolder,"ranking-*.json"))
                {
                    var old=Read<RecordsFile>(path);
                    if(old?.Entries==null)continue;
                    foreach(var entry in old.Entries)
                    {
                        if(entry==null)continue;
                        file.Entries.Add(entry);file.Stats.Add(entry.Distance,0);
                    }
                }
            }
            Board.Load(file?.Entries);
            Stats = file?.Stats ?? new PlayerStats();
            // The best distance used to live in PlayerPrefs; keep it when this file is new.
            if(useLegacyPrefs)Stats.BestDistance = Mathf.Max(Stats.BestDistance, PlayerPrefs.GetFloat(LegacyBestKey, 0f));
            if(migrate&&file.Entries.Count>0)SaveRecords();
        }
        private void SaveRecords() => Write(RecordsPath, new RecordsFile { Entries = new List<RankingEntry>(Board.All), Stats = Stats });
        private static T Read<T>(string path) where T : class
        {
            try { return File.Exists(path) ? JsonUtility.FromJson<T>(File.ReadAllText(path)) : null; }
            catch (Exception error) { Debug.LogWarning("Player data unreadable: " + path + " " + error.Message); return null; }
        }
        private void Write(string path, object data)
        {
            try { Directory.CreateDirectory(folder); File.WriteAllText(path, JsonUtility.ToJson(data, true)); }
            catch (Exception error) { Debug.LogWarning("Player data not saved: " + error.Message); }
        }
    }
}
