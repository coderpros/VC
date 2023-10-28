using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Users
{
    public class Session
    {
        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public Enums.Users_Session_Type Type { get; private set; } = Enums.Users_Session_Type.Web;

        public int User_Id { get; private set; } = 0;

        public string Key1 { get; private set; } = "";

        public string Key2 { get; private set; } = "";

        public string Key3 { get; private set; } = "";

        public string Key4 { get; private set; } = "";

        //public int? APIRefreshToken_Id { get; private set; } = null;

        public bool Authenticated { get; private set; } = false;

        public bool Claimed { get; private set; } = false;

        public DateTime Created_UTC { get; private set; } = new DateTime();

        public DateTime Created_Local { get; private set; } = new DateTime();

        public string Created_IP { get; private set; } = "";

        public DateTime LastActivity_UTC { get; private set; } = new DateTime();

        public DateTime LastActivity_Local { get; private set; } = new DateTime();

        public string LastActivity_IP { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Session() { }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private bool Load(tblUserSession efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblUserSession_id;

                    Type = (Enums.Users_Session_Type)efmObject.tblUserSession_type;
                    User_Id = efmObject.tblUser_id;

                    Key1 = efmObject.tblUserSession_key1;
                    Key2 = efmObject.tblUserSession_key2;
                    Key3 = efmObject.tblUserSession_key3;
                    if (!string.IsNullOrWhiteSpace(efmObject.tblUserSession_key4)) { Key4 = efmObject.tblUserSession_key4; }

                    //APIRefreshToken_Id = efmObject.tblUserAPIRefreshToken_id;
                    Authenticated = efmObject.tblUserSession_authenticated;
                    Claimed = efmObject.tblUserSession_claimed;

                    Created_UTC = efmObject.tblUserSession_createdUTC;
                    Created_Local = efmObject.tblUserSession_createdLocal;
                    Created_IP = efmObject.tblUserSession_createdIP;

                    LastActivity_UTC = efmObject.tblUserSession_lastActivityUTC;
                    LastActivity_Local = efmObject.tblUserSession_lastActivityLocal;
                    LastActivity_IP = efmObject.tblUserSession_lastActivityIP;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "Load(tblUserSession)", ex);
                return bReturn;
            }

            return bReturn;
        }

        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        public async Task<bool> RefreshAsync()
        {
            if (!Loaded) { return false; }

            var bReturn = false;

            try
            {
                // clear loaded until reloaded
                Loaded = false;

                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // find by id based on short term session expire
                var dtCutoff = DateTime.UtcNow.AddMinutes(-Settings.Variables.SessionExpire_ShortTermMinutes);

                // find session where id matches and has activity in the last X minutes
                var efmObject = await dB.tblUserSessions.AsNoTracking().FirstOrDefaultAsync(r => r.tblUserSession_id == Id && r.tblUserSession_lastActivityUTC >= dtCutoff);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "RefreshAsync(int)", ex,
                    "Id: " + Id.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<Session> CreateAsync(Enums.Users_Session_Type enumType, int iUserId, string strIP, bool bAuthenticated = true, bool bClaimed = false)
        {
            // run clean ups to remove stale records
            //_ = Delete_ByUser(iUserId);
            _ = await DeleteFull_ExpiredAsync();

            // pre new session object to be returned
            var objReturn = new Session();

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblUserSession
                {
                    tblUser_id = iUserId,
                    tblUserSession_type = (int)enumType,
                    tblUserSession_key1 = Guid.NewGuid().ToString().ToLower(),
                    tblUserSession_key2 = Guid.NewGuid().ToString().ToLower(),
                    tblUserSession_key3 = Guid.NewGuid().ToString().ToLower(),
                    tblUserSession_key4 = Guid.NewGuid().ToString().ToLower(),
                    tblUserSession_authenticated = bAuthenticated,
                    tblUserSession_claimed = bClaimed,
                    tblUserSession_createdUTC = DateTime.UtcNow,
                    tblUserSession_createdLocal = DateTime.Now,
                    tblUserSession_createdIP = strIP,
                    tblUserSession_lastActivityUTC = DateTime.UtcNow,
                    tblUserSession_lastActivityLocal = DateTime.Now,
                    tblUserSession_lastActivityIP = strIP
                };

                _ = dB.tblUserSessions.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    _ = objReturn.Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "CreateAsync(Enums.Users_Session_Type, int, string, bool, bool)", ex,
                    "enumType: " + ((int)enumType).ToString() +
                    ", iUserId: " + iUserId.ToString() +
                    ", strIP: " + strIP.ToString() +
                    ", bAuthenticated: " + bAuthenticated.ToString() +
                    ", bClaimed: " + bClaimed.ToString());
                return objReturn;
            }

            return objReturn;
        }

        //public static void Update_APIRefreshToken(int iId, int iAPIRefreshTokenId)
        //{
        //    try
        //    {
        //        using (var dB = Settings.Config.DBConnection())
        //        {
        //            var iUpdates = dB.tblUserSessions.Where(r => r.tblUserSession_id == iId)
        //                                                .Update(r => new tblUserSession
        //                                                {
        //                                                    tblUserAPIRefreshToken_id = iAPIRefreshTokenId
        //                                                });

        //            //bReturn = iUpdates > 0;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = Error.Exception(typeof(Session).ToString(), "Update_APIRefreshToken(int, int)", ex,
        //            "iId: " + iId.ToString() +
        //            ", iAPIRefreshTokenId: " + iAPIRefreshTokenId.ToString());
        //        //return bReturn;
        //    }
        //}

        public static async Task Update_AuthenticatedAsync(int iId, bool bAuthenticated)
        {
            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                var iUpdates = await dB.tblUserSessions.Where(r => r.tblUserSession_id == iId)
                                                        .UpdateAsync(r => new tblUserSession
                                                        {
                                                            tblUserSession_authenticated = bAuthenticated
                                                        });
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "Update_AuthenticatedAsync(int, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", bAuthenticated: " + bAuthenticated.ToString());
            }
        }

        public static async Task Update_ClaimedAsync(int iId, bool bClaimed)
        {
            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                var iUpdates = await dB.tblUserSessions.Where(r => r.tblUserSession_id == iId)
                                                        .UpdateAsync(r => new tblUserSession
                                                        {
                                                            tblUserSession_claimed = bClaimed
                                                        });
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "Update_ClaimedAsync(int, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", bClaimed: " + bClaimed.ToString());
            }
        }

        public static async Task<bool> Update_LastActivityAsync(int iId, string strIP)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                var iUpdates = await dB.tblUserSessions.Where(r => r.tblUserSession_id == iId)
                                                        .UpdateAsync(r => new tblUserSession
                                                        {
                                                            tblUserSession_lastActivityUTC = DateTime.UtcNow,
                                                            tblUserSession_lastActivityLocal = DateTime.Now,
                                                            tblUserSession_lastActivityIP = strIP
                                                        });

                bReturn = iUpdates > 0;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "Update_LastActivityAsync(int, string)", ex,
                    "iId: " + iId.ToString() +
                    ", strIP: " + strIP.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteFullAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                _ = await dB.tblUserSessions.Where(r => r.tblUserSession_id == iId).DeleteAsync();

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "DeleteFullAsync(int)", ex,
                    "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteFull_ByUserAsync(int iUserId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                _ = await dB.tblUserSessions.Where(r => r.tblUser_id == iUserId).DeleteAsync();

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "DeleteFull_ByUserAsync(int)", ex,
                    "iUserId: " + iUserId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        //public static bool Delete_ByAPIRefreshToken(int iAPIRefreshTokenId)
        //{
        //    var bReturn = false;

        //    try
        //    {
        //        using (var dB = Settings.Config.DBConnection())
        //        {
        //            _ = dB.tblUserSessions.Where(r => r.tblUserAPIRefreshToken_id == iAPIRefreshTokenId).Delete();

        //            bReturn = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = Error.Exception(typeof(Session).ToString(), "Delete_ByAPIRefreshToken(iAPIRefreshTokenId)", ex,
        //            "iAPIRefreshTokenId: " + iAPIRefreshTokenId.ToString());
        //        return bReturn;
        //    }

        //    return bReturn;
        //}

        public static async Task<bool> DeleteFull_ExpiredAsync()
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                var dtCutoff = DateTime.UtcNow.AddDays(-Settings.Variables.SessionExpire_LongTermDays);

                _ = await dB.tblUserSessions.Where(r => r.tblUserSession_lastActivityUTC < dtCutoff).DeleteAsync();

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "DeleteFull_ExpiredAsync()", ex);
                return bReturn;
            }

            return bReturn;
        }

        /// <summary>
        /// Clears all sessions from the database, effectively logging everyone out
        /// </summary>
        /// <returns>If the operation was successful</returns>
        public static async Task<bool> DeleteFull_AllAsync()
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                _ = await dB.tblUserSessions.DeleteAsync();

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "DeleteFull_AllAsync()", ex);
                return bReturn;
            }

            return bReturn;
        }


        #endregion public functions


        #region Finders

        public static Session Find(int iId)
        {
            var objReturn = new Session();

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // find by id based on short term session expire
                var dtCutoff = DateTime.UtcNow.AddMinutes(-Settings.Variables.SessionExpire_ShortTermMinutes);

                // find session where id matches and has activity in the last X minutes
                var efmObject = dB.tblUserSessions.AsNoTracking().FirstOrDefault(r => r.tblUserSession_id == iId && r.tblUserSession_lastActivityUTC >= dtCutoff);

                if (efmObject != null)
                {
                    _ = objReturn.Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "Find(int)", ex,
                    "iId: " + iId.ToString());
                return objReturn;
            }

            return objReturn;
        }

        public static async Task<Session> FindAsync(int iId)
        {
            var objReturn = new Session();

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // find by id based on short term session expire
                var dtCutoff = DateTime.UtcNow.AddMinutes(-Settings.Variables.SessionExpire_ShortTermMinutes);

                // find session where id matches and has activity in the last X minutes
                var efmObject = await dB.tblUserSessions.AsNoTracking().FirstOrDefaultAsync(r => r.tblUserSession_id == iId && r.tblUserSession_lastActivityUTC >= dtCutoff);

                if (efmObject != null)
                {
                    _ = objReturn.Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "FindAsync(int)", ex,
                    "iId: " + iId.ToString());
                return objReturn;
            }

            return objReturn;
        }

        public static async Task<Session> Find_ByUserAsync(int iUserId)
        {
            var objReturn = new Session();

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // find by user based on short term session expire
                var dtCutoff = DateTime.UtcNow.AddMinutes(-Settings.Variables.SessionExpire_ShortTermMinutes);

                // find session where id matches and has activity in the last 15 minutes
                var efmObject = await dB.tblUserSessions.AsNoTracking().FirstOrDefaultAsync(r => r.tblUser_id == iUserId && r.tblUserSession_lastActivityUTC >= dtCutoff);

                if (efmObject != null)
                {
                    _ = objReturn.Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "Find_ByUserAsync(int)", ex,
                    "iUserId: " + iUserId.ToString());
                return objReturn;
            }

            return objReturn;
        }

        public static async Task<Session> Find_ByClaimAsync(int iId, string strKey3, string strIP)
        {
            var objReturn = new Session();

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var dtCutoff = DateTime.UtcNow.AddSeconds(-20);

                var efmObject = await dB.tblUserSessions.AsNoTracking()
                                                        .FirstOrDefaultAsync(r => r.tblUserSession_id == iId &&
                                                                                    r.tblUserSession_key3 == strKey3 &&
                                                                                    r.tblUserSession_claimed == false &&
                                                                                    r.tblUserSession_createdUTC >= dtCutoff &&
                                                                                    r.tblUserSession_createdIP == strIP);

                if (efmObject != null)
                {
                    _ = objReturn.Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "Find_ByClaimAsync(int, string, string)", ex,
                    "iId: " + iId.ToString() +
                    ", strKey3: " + strKey3.ToString() +
                    ", strIP: " + strIP.ToString());
                return objReturn;
            }

            return objReturn;
        }

        public static async Task<Session> Find_ByLongTermAsync(int iUserId, string strKey4)
        {
            var objReturn = new Session();

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var dtCutoff = DateTime.UtcNow.AddDays(-Settings.Variables.SessionExpire_LongTermDays);

                var efmObject = await dB.tblUserSessions.AsNoTracking()
                                                        .FirstOrDefaultAsync(r => r.tblUser_id == iUserId &&
                                                                                    r.tblUserSession_key4 == strKey4 &&
                                                                                    r.tblUserSession_claimed == true &&
                                                                                    r.tblUserSession_lastActivityUTC >= dtCutoff);

                if (efmObject != null)
                {
                    _ = objReturn.Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Session).ToString(), "Find_ByLongTermAsync(int, string)", ex,
                    "iUserId: " + iUserId.ToString() +
                    ", strKey4: " + strKey4.ToString());
                return objReturn;
            }

            return objReturn;
        }

        #endregion finders


        #region Lists



        #endregion lists
    }
}
