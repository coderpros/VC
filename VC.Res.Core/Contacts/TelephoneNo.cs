using System.Data;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using VC.Res.Core.Utilities;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Contacts
{
    public class TelephoneNo
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Contacts_TelephoneNos;

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public int Contact_Id { get; private set; } = 0;

        public string CountryCode { get; set; } = "";

        public string Number { get; set; } = "";

        public bool Primary { get; private set; } = false;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string EditedBy { get; private set; } = "";


        #endregion properties


        #region Constructors

        public TelephoneNo() { }

        private TelephoneNo(tblContactTel efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using (var dB = Settings.Config.DBPooledConnection())
                {
                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    var efmObject = await dB.tblContactTels.AsNoTracking().FirstOrDefaultAsync(r => r.tblContactTel_id == iId);

                    if (efmObject != null)
                    {
                        bReturn = Load(efmObject);
                    }

                    efmObject = null;
                };
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(TelephoneNo).ToString(), "Load(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblContactTel efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblContactTel_id;

                    Contact_Id = efmObject.tblContact_id;

                    CountryCode = efmObject.tblContactTel_countryCode;
                    Number = efmObject.tblContactTel_no;
                    Primary = efmObject.tblContactTel_primary;

                    Created_UTC = efmObject.tblContactTel_createdUTC;
                    Created_By = efmObject.tblContactTel_createdBy;

                    Edited_UTC = efmObject.tblContactTel_editedUTC;
                    EditedBy = efmObject.tblContactTel_editedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblContactTel)", ex);
                return false;
            }

            return bReturn;
        }


        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions

        internal static async Task<bool> DeleteFullBy_ContactAsync(int iContactId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var iChanges = await dB.tblContactTels.Where(r => r.tblContact_id == iContactId).DeleteAsync();

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(TelephoneNo).ToString(), "DeleteFullBy_ContactAsync(int)", ex,
                    "iContactId: " + iContactId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion internal functions


        #region Public Functions

        public async Task<bool> CreateAsync(int iContactId, string strBy)
        {
            if (Loaded) { return false; }

            if (string.IsNullOrWhiteSpace(CountryCode) || string.IsNullOrWhiteSpace(Number)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblContactTel
                {
                    tblContact_id = iContactId,

                    tblContactTel_countryCode = General.MakeFriendlyTelNo(CountryCode),
                    tblContactTel_no = General.MakeFriendlyTelNo(Number),

                    tblContactTel_primary = !await dB.tblContactTels.AsNoTracking().AnyAsync(r => r.tblContact_id == iContactId),

                    tblContactTel_createdUTC = DateTime.UtcNow,
                    tblContactTel_createdBy = strBy,

                    tblContactTel_editedUTC = DateTime.UtcNow,
                    tblContactTel_editedBy = strBy
                };

                _ = dB.tblContactTels.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(TelephoneNo).ToString(), "CreateAsync(int, string)", ex,
                        "iContactId: " + iContactId +
                        ", strBy: " + strBy);

                return bReturn;
            }

            return bReturn;
        }

        public async Task<bool> SaveAsync(string strBy)
        {
            if (!Loaded) { return false; }

            if (string.IsNullOrWhiteSpace(CountryCode) || string.IsNullOrWhiteSpace(Number)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblContactTels.FirstOrDefaultAsync(r => r.tblContactTel_id == Id);

                if (efmObject != null)
                {
                    // only country code and number can change so specifically check
                    if (efmObject.tblContactTel_countryCode != CountryCode || efmObject.tblContactTel_no != Number)
                    {
                        efmObject.tblContactTel_countryCode = General.MakeFriendlyTelNo(CountryCode);
                        efmObject.tblContactTel_no = General.MakeFriendlyTelNo(Number);

                        efmObject.tblContactTel_editedUTC = DateTime.UtcNow;
                        efmObject.tblContactTel_editedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = Load(efmObject);

                            LocalCache.RefreshCache(CacheKey);
                        }
                    }
                    else
                    {
                        // no change made
                        bReturn = true;
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(TelephoneNo).ToString(), "Save(string)", ex,
                        "Id: " + Id.ToString() +
                        ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteFullAsync(int iId)
        {
            var bReturn = false;

            try
            {
                var dB = Settings.Config.DBConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var iChanges = await dB.tblContactTels.Where(r => r.tblContactTel_id == iId).DeleteAsync();

                if (iChanges > 0)
                {
                    LocalCache.RefreshCache(CacheKey);
                }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(TelephoneNo).ToString(), "DeleteFullAsync(int, string, bool)", ex,
                    "Id: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Set_PrimaryAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                // find the entry to update
                var efmObject = await dB.tblContactTels.FirstOrDefaultAsync(r => r.tblContactTel_id == iId);

                if (efmObject != null)
                {
                    if (efmObject.tblContactTel_primary)
                    {
                        // already primary
                        bReturn = true;
                    }
                    else
                    {
                        // clear primary from any other entry assigned to this contact
                        _ = await dB.tblContactTels.Where(r => r.tblContact_id == efmObject.tblContact_id).UpdateAsync(r => new tblContactTel { tblContactTel_primary = false });

                        efmObject.tblContactTel_primary = true;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            LocalCache.RefreshCache(CacheKey);
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(TelephoneNo).ToString(), "Set_PrimaryAsync(int)", ex,
                    "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<TelephoneNo> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new TelephoneNo(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new TelephoneNo();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new TelephoneNo();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<TelephoneNo>> FindAllBy_ContactAsync(int iContactId)
        {
            return await FindAllBy_ContactAsync(new List<int> { iContactId });
        }

        public static async Task<List<TelephoneNo>> FindAllBy_ContactAsync(List<int> lstContactIds)
        {
            var lstReturn = new List<TelephoneNo>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Contacts_TelephoneNos_IdxContact) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var vId in lstContactIds)
                {
                    if (lstIndex.TryGetValue(vId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstGlobal = null;

            lstReturn = lstReturn.OrderByDescending(r => r.Primary).ThenBy(r => r.CountryCode).ThenBy(r => r.Number).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, TelephoneNo>> Global_ListAsync()
        {
            // check to see if cache is populated (including indexes)
            if (LocalCache.Get(CacheKey) is not Dictionary<int, TelephoneNo> lstElements ||
                LocalCache.Get(LocalCache.Key.Contacts_TelephoneNos_IdxContact) is not Dictionary<int, List<int>> dicIndexContact)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, TelephoneNo>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    await s_singleCacheBuildLock.WaitAsync();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, TelephoneNo> lstElementsRetry)
                    {
                        // cache now populated, no need to build
                        return lstElementsRetry;
                    }

                    // setup index dictionaries
                    dicIndexContact = new Dictionary<int, List<int>>();

                    // create instance of DB
                    using var dB = Settings.Config.DBPooledConnection();

                    // disable change tracking as only reading
                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    // get all the elements
                    foreach (var efmObject in await dB.tblContactTels.AsNoTracking().ToListAsync())
                    {
                        var obj = new TelephoneNo(efmObject);

                        if (obj.Loaded)
                        {
                            // successfully populated/loaded info, add to global list using id as key
                            lstElements.Add(obj.Id, obj);

                            // add to country index
                            if (!dicIndexContact.ContainsKey(obj.Contact_Id))
                            {
                                dicIndexContact.Add(obj.Contact_Id, new List<int>());
                            }

                            dicIndexContact[obj.Contact_Id].Add(obj.Id);
                        }

                        obj = null;
                    }

                    // add the found elements to the cache
                    LocalCache.Set(CacheKey, lstElements);

                    // add indexes to cache
                    LocalCache.Set(LocalCache.Key.Contacts_TelephoneNos_IdxContact, dicIndexContact);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(TelephoneNo).ToString(), "Global_ListAsync()", ex);
                    return lstElements;
                }
                finally
                {
                    // no matter what, always release lock when done
                    _ = s_singleCacheBuildLock.Release();
                }
            }

            return lstElements;
        }

        #endregion lists
    }
}
