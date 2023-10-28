using System.Data;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Contacts
{
    public class Email
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Contacts_Emails;

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public int Contact_Id { get; private set; } = 0;

        public string Address { get; set; } = "";

        public bool Primary { get; private set; } = false;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";
        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string EditedBy { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Email() { }

        private Email(tblContactEmail efmObject) { _ = Load(efmObject); }

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

                    var efmObject = await dB.tblContactEmails.AsNoTracking().FirstOrDefaultAsync(r => r.tblContactEmail_id == iId);

                    if (efmObject != null)
                    {
                        bReturn = Load(efmObject);
                    }

                    efmObject = null;
                };
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "Load(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblContactEmail efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblContactEmail_id;

                    Contact_Id = efmObject.tblContact_id;

                    Address = efmObject.tblContactEmail_address;
                    Primary = efmObject.tblContactEmail_primary;

                    Created_UTC = efmObject.tblContactEmail_createdUTC;
                    Created_By = efmObject.tblContactEmail_createdBy;

                    Edited_UTC = efmObject.tblContactEmail_editedUTC;
                    EditedBy = efmObject.tblContactEmail_editedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblContactEmail)", ex);
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

                var iChanges = await dB.tblContactEmails.Where(r => r.tblContact_id == iContactId).DeleteAsync();

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "DeleteFullBy_ContactAsync(int)", ex,
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

            if (!Utilities.General.Validate_EmailAddress(Address)) { return false; }

            // check the address is unique for the contact
            if (!await Check_UniqueAsync(iContactId, Address)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblContactEmail
                {
                    tblContact_id = iContactId,
                    tblContactEmail_address = Address.Trim().ToLower(),
                    tblContactEmail_primary = !await dB.tblContactEmails.AsNoTracking().AnyAsync(r => r.tblContact_id == iContactId),

                    tblContactEmail_createdUTC = DateTime.UtcNow,
                    tblContactEmail_createdBy = strBy,

                    tblContactEmail_editedUTC = DateTime.UtcNow,
                    tblContactEmail_editedBy = strBy
                };

                _ = dB.tblContactEmails.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "CreateAsync(int, string)", ex,
                        "iContactId: " + iContactId +
                        ", strBy: " + strBy);

                return bReturn;
            }

            return bReturn;
        }

        public async Task<bool> SaveAsync(string strBy)
        {
            if (!Loaded) { return false; }

            if (!Utilities.General.Validate_EmailAddress(Address)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblContactEmails.FirstOrDefaultAsync(r => r.tblContactEmail_id == Id);

                if (efmObject != null)
                {
                    if (efmObject.tblContactEmail_address != Address.Trim().ToLower())
                    {
                        // check the new address is unique for the contact
                        if (!await Check_UniqueAsync(Id, Address)) { return false; }

                        efmObject.tblContactEmail_address = Address.Trim().ToLower();

                        efmObject.tblContactEmail_editedUTC = DateTime.UtcNow;
                        efmObject.tblContactEmail_editedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = Load(efmObject);

                            LocalCache.RefreshCache(CacheKey);
                        }
                    }
                    else
                    {
                        // no update to be made as only one field that can be changed
                        bReturn = true;
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "SaveAsync(string)", ex,
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

                var iChanges = await dB.tblContactEmails.Where(r => r.tblContactEmail_id == iId).DeleteAsync();

                if (iChanges > 0)
                {
                    LocalCache.RefreshCache(CacheKey);
                }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "DeleteFullAsync(int, string, bool)", ex,
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
                var efmObject = await dB.tblContactEmails.FirstOrDefaultAsync(r => r.tblContactEmail_id == iId);

                if (efmObject != null)
                {
                    if (efmObject.tblContactEmail_primary)
                    {
                        // already primary
                        bReturn = true;
                    }
                    else
                    {
                        // clear primary from any other entry assigned to this contact
                        _ = await dB.tblContactEmails.Where(r => r.tblContact_id == efmObject.tblContact_id).UpdateAsync(r => new tblContactEmail { tblContactEmail_primary = false });

                        efmObject.tblContactEmail_primary = true;

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
                _ = Error.Exception(typeof(Email).ToString(), "Set_PrimaryAsync(int)", ex,
                    "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Check_UniqueAsync(int? iContactId, string strAddress)
        {
            if (!Utilities.General.Validate_EmailAddress(strAddress)) { return false; }

            // set default return to false, as in not unique, for safety
            var bReturn = false;

            try
            {
                var strAddressToCheck = strAddress.Trim().ToLower();

                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                if (iContactId.HasValue)
                {
                    if (!await dB.tblContactEmails.AsNoTracking().AnyAsync(r => r.tblContact_id == iContactId.Value && r.tblContactEmail_address == strAddressToCheck))
                    {
                        bReturn = true;
                    }
                }
                else
                {
                    if (!await dB.tblContactEmails.AsNoTracking().AnyAsync(r => r.tblContactEmail_address == strAddressToCheck))
                    {
                        bReturn = true;
                    }
                }
            }
            catch (Exception ex)
            {

                _ = Error.Exception(typeof(Email).ToString(), "Check_UniqueAsync(int?, strAddress)", ex,
                    "iContactId: " + (iContactId.HasValue ? iContactId.Value.ToString() : "") +
                    ", strAddress: " + strAddress);
                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Email> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Email(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Email();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Email();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Email>> FindAll(string strEmail)
        {
            var lstReturn = new List<Email>();

            var lstGlobal = await Global_ListAsync();

            var strSearchFor = strEmail.ToLower().Trim();

            foreach (var vEmail in lstGlobal.Values)
            {
                if (vEmail.Address.Contains(strSearchFor))
                {
                    lstReturn.Add(vEmail);
                }
            }

            lstGlobal = null;

            return lstReturn;
        }

        public static async Task<List<Email>> FindAllBy_ContactAsync(int iContactId)
        {
            return await FindAllBy_ContactAsync(new List<int> { iContactId });
        }

        public static async Task<List<Email>> FindAllBy_ContactAsync(List<int> lstContactIds)
        {
            var lstReturn = new List<Email>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Contacts_Emails_IdxContact) is Dictionary<int, List<int>> lstIndex)
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

            lstReturn = lstReturn.OrderByDescending(r => r.Primary).ThenBy(r => r.Address).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Email>> Global_ListAsync()
        {
            // check to see if cache is populated (including indexes)
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Email> lstElements ||
                LocalCache.Get(LocalCache.Key.Contacts_Emails_IdxContact) is not Dictionary<int, List<int>> dicIndexContact)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, Email>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    await s_singleCacheBuildLock.WaitAsync();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Email> lstElementsRetry)
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
                    foreach (var efmObject in await dB.tblContactEmails.AsNoTracking().ToListAsync())
                    {
                        var obj = new Email(efmObject);

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
                    LocalCache.Set(LocalCache.Key.Contacts_Emails_IdxContact, dicIndexContact);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Email).ToString(), "Global_ListAsync()", ex);
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
