namespace Muhanok.Application
{
    // Saves a member's finished runs. A failed save stays in memory and is sent once more before the next run;
    // the server ignores a resent id, so a retry never credits coins twice.
    public sealed class AccountRunUploader
    {
        private sealed class Pending { public string AccountId; public AccountRun Run; }
        private readonly AccountSession account;
        private Pending pending;
        public IAccountGateway Gateway { get; set; }
        // The last run could not be saved (shown on the result screen).
        public bool LastFailed { get; private set; }
        public bool HasPending => pending != null;
        public AccountRunUploader(AccountSession session, IAccountGateway gateway) { account = session; Gateway = gateway; }

        public void Save(AccountRun run)
        {
            if (run == null || Gateway == null || !account.IsMember) return;
            string id = account.Id; LastFailed = false;
            var retry = pending; pending = null;
            // The older run goes first so the balance from the newest reply is the latest one.
            if (retry != null && retry.AccountId == id) Gateway.SaveRun(account.Token, retry.Run, reply => { Finished(id, reply, null); Send(id, run); });
            else Send(id, run);
        }
        private void Send(string id, AccountRun run)
        {
            if (!account.IsMember || account.Id != id) { Keep(id, run); return; }
            Gateway.SaveRun(account.Token, run, reply => Finished(id, reply, run));
        }
        private void Finished(string id, AccountReply reply, AccountRun run)
        {
            if (reply != null && reply.ok) { if (account.IsMember && account.Id == id) account.SetCoins(reply.coins); return; }
            if (run != null) Keep(id, run);
        }
        private void Keep(string id, AccountRun run) { pending = new Pending { AccountId = id, Run = run }; LastFailed = true; }
    }
}
