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
    public class Address
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Contacts_Addresses;

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public int Contact_Id { get; private set; } = 0;

        public string Line1 { get; set; } = "";
        public string Line2 { get; set; } = "";
        public string Line3 { get; set; } = "";
        public string Town { get; set; } = "";
        public string Region { get; set; } = "";
        public string Post_Code { get; set; } = "";
        public int Country_Id { get; set; } = 0;

        public bool Primary { get; private set; } = false;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string Edited_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Address() { }

        private Address(tblContactAddress efmObject) { _ = Load(efmObject); }

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

                var efmObject = await dB.tblContactAddresses.AsNoTracking().FirstOrDefaultAsync(r => r.tblContactAddress_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Address).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblContactAddress efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblContactAddress_id;

                    Contact_Id = efmObject.tblContact_id;

                    Line1 = efmObject.tblContactAddress_line1;
                    Line2 = efmObject.tblContactAddress_line2;
                    Line3 = efmObject.tblContactAddress_line3;

                    Town = efmObject.tblContactAddress_town;
                    Region = efmObject.tblContactAddress_region;
                    Post_Code = efmObject.tblContactAddress_postCode;

                    Country_Id = efmObject.tblCountry_id;

                    Primary = efmObject.tblContactAddress_primary;

                    Created_UTC = efmObject.tblContactAddress_createdUTC;
                    Created_By = efmObject.tblContactAddress_createdBy;

                    Edited_UTC = efmObject.tblContactAddress_editedUTC;
                    Edited_By = efmObject.tblContactAddress_editedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblContactAddress)", ex);
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

                var iChanges = await dB.tblContactAddresses.Where(r => r.tblContact_id == iContactId).DeleteAsync();

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Address).ToString(), "DeleteFullBy_ContactAsync(int)", ex,
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

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblContactAddress
                {
                    tblContact_id = iContactId,
                    tblContactAddress_line1 = Line1,
                    tblContactAddress_line2 = Line2,
                    tblContactAddress_line3 = Line3,
                    tblContactAddress_town = Town,
                    tblContactAddress_region = Region,
                    tblContactAddress_postCode = Post_Code,
                    tblCountry_id = Country_Id,
                    tblContactAddress_primary = !await dB.tblContactAddresses.AsNoTracking().AnyAsync(r => r.tblContact_id == iContactId),
                    tblContactAddress_createdUTC = DateTime.UtcNow,
                    tblContactAddress_createdBy = strBy,
                    tblContactAddress_editedUTC = DateTime.UtcNow,
                    tblContactAddress_editedBy = strBy
                };

                _ = dB.tblContactAddresses.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Address).ToString(), "CreateAsync(int, string)", ex,
                        "iContactId: " + iContactId +
                        ", strBy: " + strBy);

                return bReturn;
            }

            return bReturn;
        }

        public async Task<bool> SaveAsync(string strBy)
        {
            if (!Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblContactAddresses.FirstOrDefaultAsync(r => r.tblContactAddress_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblContactAddress_line1 = Line1;
                    efmObject.tblContactAddress_line2 = Line2;
                    efmObject.tblContactAddress_line3 = Line3;

                    efmObject.tblContactAddress_town = Town;
                    efmObject.tblContactAddress_region = Region;
                    efmObject.tblContactAddress_postCode = Post_Code;

                    efmObject.tblCountry_id = Country_Id;

                    efmObject.tblContactAddress_editedUTC = DateTime.UtcNow;
                    efmObject.tblContactAddress_editedBy = strBy;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = Load(efmObject);

                        LocalCache.RefreshCache(CacheKey);
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Address).ToString(), "SaveAsync(string)", ex,
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

                var iChanges = await dB.tblContactAddresses.Where(r => r.tblContactAddress_id == iId).DeleteAsync();

                if (iChanges > 0)
                {
                    LocalCache.RefreshCache(CacheKey);
                }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Address).ToString(), "DeleteFullAsync(int, string, bool)", ex,
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
                var efmObject = await dB.tblContactAddresses.FirstOrDefaultAsync(r => r.tblContactAddress_id == iId);

                if (efmObject != null)
                {
                    if (efmObject.tblContactAddress_primary)
                    {
                        // already primary
                        bReturn = true;
                    }
                    else
                    {
                        // clear primary from any other entry assigned to this contact
                        _ = await dB.tblContactAddresses.Where(r => r.tblContact_id == efmObject.tblContact_id).UpdateAsync(r => new tblContactAddress { tblContactAddress_primary = false });

                        efmObject.tblContactAddress_primary = true;

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
                _ = Error.Exception(typeof(Address).ToString(), "Set_PrimaryAsync(int)", ex,
                    "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public async Task<string> Render(string strSeparator = ", ")
        {
            var lstAddressElements = new List<string>();

            if (!string.IsNullOrWhiteSpace(Line1)) { lstAddressElements.Add(Line1); }
            if (!string.IsNullOrWhiteSpace(Line2)) { lstAddressElements.Add(Line2); }
            if (!string.IsNullOrWhiteSpace(Line3)) { lstAddressElements.Add(Line3); }
            if (!string.IsNullOrWhiteSpace(Town)) { lstAddressElements.Add(Town); }
            if (!string.IsNullOrWhiteSpace(Region)) { lstAddressElements.Add(Region); }
            if (!string.IsNullOrWhiteSpace(Post_Code)) { lstAddressElements.Add(Post_Code); }

            var strCountry = (await Common.Country.FindAsync(Country_Id)).Name;

            if (!string.IsNullOrWhiteSpace(strCountry)) { lstAddressElements.Add(strCountry); }

            return string.Join(strSeparator, lstAddressElements);
        }

        #endregion public functions


        #region Finders

        public static async Task<Address> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Address(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Address();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Address();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Address>> FindAllBy_ContactAsync(int iContactId)
        {
            return await FindAllBy_ContactAsync(new List<int> { iContactId });
        }

        public static async Task<List<Address>> FindAllBy_ContactAsync(List<int> lstContactIds)
        {
            var lstReturn = new List<Address>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Contacts_Addresses_IdxContact) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iContactId in lstContactIds)
                {
                    if (lstIndex.TryGetValue(iContactId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstGlobal = null;

            lstReturn = lstReturn.OrderByDescending(r => r.Primary).ThenBy(r => r.Town).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Address>> Global_ListAsync()
        {
            // check to see if cache is populated (including indexes)
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Address> lstElements ||
                LocalCache.Get(LocalCache.Key.Contacts_Addresses_IdxContact) is not Dictionary<int, List<int>> dicIndexContact)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, Address>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    await s_singleCacheBuildLock.WaitAsync();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Address> lstElementsRetry)
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
                    foreach (var efmObject in await dB.tblContactAddresses.AsNoTracking().ToListAsync())
                    {
                        var obj = new Address(efmObject);

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
                    LocalCache.Set(LocalCache.Key.Contacts_Addresses_IdxContact, dicIndexContact);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Address).ToString(), "Global_ListAsync()", ex);
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
