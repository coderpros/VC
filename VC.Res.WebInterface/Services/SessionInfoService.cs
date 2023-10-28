namespace VC.Res.WebInterface.Services
{
    public class SessionInfoService : IDisposable
    {
        private DateTime _dtLastRefresh = DateTime.UtcNow;

        public string IPAddress { get; set; } = "";

        public Core.Users.Session Current_Session { get; set; } = new Core.Users.Session();

        public Core.Users.User Current_User { get; set; } = new Core.Users.User();

        public void Configure(int iSessionId)
        {
            Current_Session = Core.Users.Session.Find(iSessionId);

            if (Current_Session.Loaded)
            {
                Task.Run(async () => { Current_User = await Core.Users.User.FindAsync(Current_Session.User_Id); }).Wait();
            }
        }

        public bool Valid(bool bForceRefresh = false)
        {
            // if current session elements are not in correct state, then not valid
            if (!Current_Session.Loaded || !Current_User.Loaded) { return false; }

            // record current ids so we can work out changes
            var iSessionId = Current_Session.Id;
            var iUserId = Current_User.Id;

            // refresh the items
            Refresh(bForceRefresh);

            // if session elements are not loaded then not valid
            if (!Current_Session.Loaded || !Current_User.Loaded) { return false; }

            // if there have been changes to the ids then failure
            if (iSessionId != Current_Session.Id || iUserId != Current_User.Id) { return false; }

            // the session must be claimed and authenticated
            if (!Current_Session.Claimed || !Current_Session.Authenticated) { return false; }

            // the user needs to be enabled and not deleted
            if (Current_User.Deleted_UTC.HasValue || !Current_User.Enabled) { return false; }

            // got to here then all fine
            return true;
        }

        public void Refresh(bool bForce = false)
        {
            if (!bForce)
            {
                // if we're not forcing the refresh, only refresh every 30 seconds to limit calls
                // this does create a 30 second window where one tab might be logged out and the other tabs/circuits
                // are still accessible until their next refresh.
                if (_dtLastRefresh > DateTime.UtcNow.AddSeconds(-30)) { return; }
            }

            Task.Run(async () =>
            {
                var lstTasks = new List<Task<bool>>
                    {
                        Current_Session.RefreshAsync(),
                        Current_User.RefreshAsync()
                    };

                _ = await Task.WhenAll(lstTasks);
            }
            ).Wait();

            _dtLastRefresh = DateTime.UtcNow;
        }

        public void Dispose()
        {

        }
    }
}
