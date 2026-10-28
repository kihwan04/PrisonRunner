using System;
using System.Collections.Generic;
using Muhanok.Application;
using Muhanok.Domain;

namespace Muhanok.Tests
{
    // In-memory account server for tests: answers at once and records every call.
    public sealed class FakeAccountGateway : IAccountGateway
    {
        public readonly List<AccountRun> SavedRuns = new List<AccountRun>();
        public readonly List<string> RunTokens = new List<string>();
        public readonly List<string> Calls = new List<string>();
        public AccountProfile Profile = new AccountProfile { id = "fake-member", name = "회원", coins = 0 };
        public bool FailRuns;
        public int Balance;
        public int Logouts;
        // Offline: /health fails. TakenName: /name-check refuses it. FailEnroll: the next /enroll is refused (like a face already registered).
        public bool Offline, FailEnroll;
        public string TakenName = "";
        public readonly List<AccountEnrollment> Enrollments = new List<AccountEnrollment>();

        public void Health(Action<AccountResult> done) { Calls.Add("health"); done?.Invoke(new AccountResult { ok = !Offline, message = Offline ? "얼굴 서버 연결 실패" : "" }); }
        public void NameCheck(string name, Action<AccountReply> done)
        {
            Calls.Add("name-check");
            bool taken = !string.IsNullOrEmpty(TakenName) && string.Equals(name?.Trim(), TakenName, StringComparison.OrdinalIgnoreCase);
            done?.Invoke(new AccountReply { ok = true, available = !taken, message = taken ? "이미 사용 중인 닉네임입니다." : "사용할 수 있는 닉네임입니다." });
        }
        public void Enroll(AccountEnrollment enrollment, Action<AccountReply> done)
        {
            Calls.Add("enroll"); Enrollments.Add(enrollment);
            if (FailEnroll) { FailEnroll = false; done?.Invoke(new AccountReply { ok = false, message = "이미 등록된 얼굴입니다.", status = 409 }); return; }
            Profile.name = enrollment.name; Profile.weight = enrollment.weight; Profile.style = enrollment.style; done?.Invoke(SignedIn());
        }
        public void Login(string[] images, Action<AccountReply> done) { Calls.Add("login"); done?.Invoke(SignedIn()); }
        public void FetchProfile(string token, Action<AccountReply> done) { Calls.Add("profile"); done?.Invoke(new AccountReply { ok = true, profile = Profile }); }
        public void UpdateProfile(string token, AccountProfileUpdate update, Action<AccountReply> done) { Calls.Add("profile/update"); done?.Invoke(new AccountReply { ok = true, profile = Profile }); }
        public void SaveRun(string token, AccountRun run, Action<AccountReply> done)
        {
            Calls.Add("runs"); SavedRuns.Add(run); RunTokens.Add(token);
            if (FailRuns) { done?.Invoke(new AccountReply { ok = false, message = "offline" }); return; }
            Balance += run.coins; done?.Invoke(new AccountReply { ok = true, coins = Balance });
        }
        public void Buy(string token, ItemKind item, Action<AccountReply> done) { Calls.Add("shop/buy"); done?.Invoke(new AccountReply { ok = true, profile = Profile }); }
        public void Leaderboard(string token, PlayStyle mode, bool today, Action<LeaderboardReply> done) { Calls.Add("leaderboard"); done?.Invoke(new LeaderboardReply { ok = true }); }
        public void Stats(string token, Action<StatsReply> done) { Calls.Add("stats"); done?.Invoke(new StatsReply { ok = true }); }
        public void Logout(string token, Action<AccountReply> done) { Calls.Add("logout"); Logouts++; done?.Invoke(new AccountReply { ok = true }); }
        public void Delete(string token, Action<AccountReply> done) { Calls.Add("delete"); done?.Invoke(new AccountReply { ok = true }); }
        private AccountReply SignedIn() => new AccountReply { ok = true, token = "fake-token", profile = Profile, id = Profile.id, name = Profile.name };
    }
}
