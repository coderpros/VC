using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using VC.Res.Core.Utilities;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Users
{
    public class IPHistory
    {
        public enum FilterOption
        {
            Ids,
            User_Id,
            User_Ids,
            Address,
            Authorised,
            Date_LastLogin_UTC,
            Date_LastLogin_Local,
            Date_Created_UTC,
            Date_Created_Local
        };

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public int User_Id { get; private set; } = 0;

        public string Address { get; private set; } = "";

        public bool Authorised { get; private set; } = false;

        public DateTime LastLogin_UTC { get; private set; } = DateTime.UtcNow;

        public DateTime LastLogin_Local { get; private set; } = DateTime.Now;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;

        public DateTime Created_Local { get; private set; } = DateTime.Now;

        #endregion properties


        #region Constructors

        private IPHistory() { }

        //private IPHistory(int iId) { _ = Load(iId); }

        //private IPHistory(int iUserId, string strAddress) { _ = Load(iUserId, strAddress); }

        private IPHistory(tblUserIP efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblUserIPs.AsNoTracking().FirstOrDefaultAsync(r => r.tblUserIP_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(IPHistory).ToString(), "LoadAsync(int)", ex,
                    "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        private async Task<bool> LoadAsync(int iUserId, string strAddress)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblUserIPs.AsNoTracking().FirstOrDefaultAsync(r => r.tblUser_id == iUserId && r.tblUserIP_ipAddress == strAddress);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(IPHistory).ToString(), "LoadAsync(int, string)", ex,
                    "iUserId: " + iUserId.ToString() +
                    ", strAddress: " + strAddress.ToString());
                return bReturn;
            }

            return bReturn;
        }

        private bool Load(tblUserIP efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblUserIP_id;
                    User_Id = efmObject.tblUser_id;

                    Address = efmObject.tblUserIP_ipAddress;
                    Authorised = efmObject.tblUserIP_authorised;

                    LastLogin_UTC = efmObject.tblUserIP_lastLoginUTC;
                    LastLogin_Local = efmObject.tblUserIP_lastLoginLocal;

                    Created_UTC = efmObject.tblUserIP_createdUTC;
                    Created_Local = efmObject.tblUserIP_createdLocal;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(IPHistory).ToString(), "Load(tblUserIP)", ex);
                return bReturn;
            }

            return bReturn;
        }

        #endregion private functions-loaders

        /// <summary>
        /// Deauthorises any IPs that have not been used in the last 60 days
        /// </summary>
        private static async Task DeAuthoriseAsync()
        {
            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                var dtCutOff = DateTime.UtcNow.AddDays(-60);

                _ = await dB.tblUserIPs.Where(r => r.tblUserIP_authorised && r.tblUserIP_lastLoginUTC <= dtCutOff).UpdateAsync(r => new tblUserIP { tblUserIP_authorised = false });
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(IPHistory).ToString(), "DeAuthoriseAsync()", ex);
            }
        }

        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        public static async Task<IPHistory> CreateAsync(int iUserId, string strIP, bool bAuthorised = false)
        {
            if (iUserId <= 0 || string.IsNullOrWhiteSpace(strIP)) { return new IPHistory(); }

            var objReturn = new IPHistory();

            try
            {
                using var dB = Settings.Config.DBConnection();

                // see if already exists
                var efmObject = await dB.tblUserIPs.FirstOrDefaultAsync(r => r.tblUser_id == iUserId && r.tblUserIP_ipAddress == strIP);

                if (efmObject == null)
                {
                    // not found, need to create
                    efmObject = new tblUserIP
                    {
                        tblUser_id = iUserId,
                        tblUserIP_ipAddress = strIP,
                        tblUserIP_authorised = bAuthorised,
                        tblUserIP_lastLoginUTC = DateTime.UtcNow,
                        tblUserIP_lastLoginLocal = DateTime.Now,
                        tblUserIP_createdUTC = DateTime.UtcNow,
                        tblUserIP_createdLocal = DateTime.Now
                    };

                    _ = dB.tblUserIPs.Add(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        _ = objReturn.Load(efmObject);
                    }

                    efmObject = null;
                }
                else
                {
                    // already exists... update
                    if (efmObject.tblUserIP_authorised != bAuthorised)
                    {
                        efmObject.tblUserIP_authorised = bAuthorised;
                    }

                    efmObject.tblUserIP_lastLoginUTC = DateTime.UtcNow;
                    efmObject.tblUserIP_lastLoginLocal = DateTime.Now;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        _ = objReturn.Load(efmObject);
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(IPHistory).ToString(), "CreateAsync(int, string, bool)", ex,
                    "iUserId: " + iUserId.ToString() +
                    ", strIP: " + strIP +
                    ", bAuthorised: " + bAuthorised.ToString());
                return objReturn;
            }

            return objReturn;
        }

        public static async Task<bool> AuthoriseAsync(int iId, bool bUpdateLastLogin = true)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblUserIPs.FirstOrDefaultAsync(r => r.tblUserIP_id == iId);

                if (efmObject != null)
                {
                    var bSaveRequired = false;

                    if (!efmObject.tblUserIP_authorised)
                    {
                        bSaveRequired = true;

                        efmObject.tblUserIP_authorised = true;
                    }
                    else
                    {
                        // already authorised
                        bReturn = true;
                    }

                    if (bUpdateLastLogin)
                    {
                        bSaveRequired = true;
                        efmObject.tblUserIP_lastLoginUTC = DateTime.UtcNow;
                        efmObject.tblUserIP_lastLoginLocal = DateTime.Now;
                    }

                    if (bSaveRequired)
                    {
                        // already accepted
                        bReturn = true;

                        if (bUpdateLastLogin)
                        {
                            if (await dB.SaveChangesAsync() > 0)
                            {
                                bReturn = true;
                            }
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(IPHistory).ToString(), "AuthoriseAsync(int, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", bUpdateLastLogin: " + bUpdateLastLogin.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> UpdateLastLoginAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblUserIPs.FirstOrDefaultAsync(r => r.tblUserIP_id == iId);

                if (efmObject != null)
                {
                    efmObject.tblUserIP_lastLoginUTC = DateTime.UtcNow;
                    efmObject.tblUserIP_lastLoginLocal = DateTime.Now;

                    if (dB.SaveChanges() > 0)
                    {
                        bReturn = true;
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(IPHistory).ToString(), "UpdateLastLoginAsync(int)", ex,
                    "iId: " + iId.ToString());

                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<IPHistory> FindAsync(int iId)
        {
            await DeAuthoriseAsync();

            var objReturn = new IPHistory();
            await objReturn.LoadAsync(iId);

            return objReturn;
        }

        public static async Task<IPHistory> FindAsync(int iUserId, string strAddress)
        {
            await DeAuthoriseAsync();

            var objReturn = new IPHistory();
            await objReturn.LoadAsync(iUserId, strAddress);

            return objReturn;
        }

        #endregion finders


        #region Lists

        public static async Task<List<IPHistory>> ListAsync(List<Filter<FilterOption>>? lstFilters = null)
        {
            await DeAuthoriseAsync();

            var lstElements = new List<IPHistory>();

            try
            {
                // no cache available so query needs to be more precise if possible
                var predicate = CompileWhereDB(lstFilters);

                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var lstItems = await dB.tblUserIPs.AsNoTracking().AsExpandable().Where(predicate).ToListAsync();
                predicate = null;

                foreach (var efmObject in lstItems)
                {
                    var obj = new IPHistory(efmObject);

                    if (obj.Loaded)
                    {
                        lstElements.Add(obj);
                    }

                    obj = null;
                }

                lstItems = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(IPHistory).ToString(), "ListAsync(List<Filter<FilterOption>>)", ex);
                return lstElements;
            }

            return lstElements;
        }

        private static Expression<Func<tblUserIP, bool>> CompileWhereDB(List<Filter<FilterOption>>? lstFilters, bool bTrue = true)
        {
            var predicate = PredicateBuilder.New<tblUserIP>(true);

            if (!bTrue) { predicate = PredicateBuilder.New<tblUserIP>(false); }

            if (lstFilters == null) { return predicate; }

            try
            {
                foreach (var vFilter in lstFilters)
                {
                    switch (vFilter.Option)
                    {
                        case FilterOption.Ids:
                            {
                                var lstIds = vFilter.Value_ListInt();
                                predicate = predicate.And(r => lstIds.Contains(r.tblUserIP_id));
                            }
                            break;

                        case FilterOption.User_Id:
                            {
                                var i = vFilter.Value_Int();
                                predicate = predicate.And(r => r.tblUser_id == i);
                            }
                            break;
                        case FilterOption.User_Ids:
                            {
                                var lstIds = vFilter.Value_ListInt();
                                predicate = predicate.And(r => lstIds.Contains(r.tblUser_id));
                            }
                            break;

                        case FilterOption.Address:
                            {
                                var strSearch = vFilter.Value_String().ToLower().Trim();
                                predicate = predicate.And(r => r.tblUserIP_ipAddress == strSearch);
                            }
                            break;

                        case FilterOption.Authorised:
                            {
                                var b = vFilter.Value_Bool();
                                predicate = predicate.And(r => r.tblUserIP_authorised == b);
                            }
                            break;

                        case FilterOption.Date_LastLogin_UTC:
                            {
                                var dfDates = vFilter.Value_DateFilter();

                                if (dfDates.From.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_lastLoginUTC >= dfDates.From.Value);
                                }

                                if (dfDates.To.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_lastLoginUTC <= dfDates.To.Value);
                                }

                                if (dfDates.Equal.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_lastLoginUTC == dfDates.Equal.Value);
                                }
                            }
                            break;
                        case FilterOption.Date_LastLogin_Local:
                            {
                                var dfDates = vFilter.Value_DateFilter();

                                if (dfDates.From.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_lastLoginLocal >= dfDates.From.Value);
                                }

                                if (dfDates.To.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_lastLoginLocal <= dfDates.To.Value);
                                }

                                if (dfDates.Equal.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_lastLoginLocal == dfDates.Equal.Value);
                                }
                            }
                            break;

                        case FilterOption.Date_Created_UTC:
                            {
                                var dfDates = vFilter.Value_DateFilter();

                                if (dfDates.From.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_createdUTC >= dfDates.From.Value);
                                }

                                if (dfDates.To.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_createdUTC <= dfDates.To.Value);
                                }

                                if (dfDates.Equal.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_createdUTC == dfDates.Equal.Value);
                                }
                            }
                            break;
                        case FilterOption.Date_Created_Local:
                            {
                                var dfDates = vFilter.Value_DateFilter();

                                if (dfDates.From.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_createdLocal >= dfDates.From.Value);
                                }

                                if (dfDates.To.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_createdLocal <= dfDates.To.Value);
                                }

                                if (dfDates.Equal.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserIP_createdLocal == dfDates.Equal.Value);
                                }
                            }
                            break;

                        default: continue;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(IPHistory).ToString(), "CompileWhereDB(List<Filter<FilterOption>>)", ex);
                return predicate;
            }

            return predicate;
        }

        #endregion lists
    }
}
