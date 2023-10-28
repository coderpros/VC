using System.Data;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using VC.Res.Core.Users;
using VC.Res.Core.Utilities;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Common
{
    public class Region : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Common_Regions;

        #region Properties

        public int Country_Id { get; private set; } = 0;

        public int? Website_Id { get; private set; } = null;

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string Edited_By { get; private set; } = "";

        public DateTime? Deleted_UTC { get; private set; } = null;
        public string Deleted_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Region() { }

        private Region(tblRegion efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        protected override async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblRegions.AsNoTracking().FirstOrDefaultAsync(r => r.tblRegion_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Region).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblRegion efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblRegion_id;

                    Country_Id = efmObject.tblCountry_id;

                    Website_Id = efmObject.tblRegion_websiteId;

                    Name = efmObject.tblRegion_name;
                    Description = efmObject.tblRegion_desc;

                    Created_UTC = efmObject.tblRegion_createdUtc;
                    Created_By = efmObject.tblRegion_createdBy;

                    Edited_UTC = efmObject.tblRegion_editedUtc;
                    Edited_By = efmObject.tblRegion_editedBy;

                    Deleted_UTC = efmObject.tblRegion_deletedUtc;
                    Deleted_By = efmObject.tblRegion_deletedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblRegion)", ex);
                return false;
            }

            return bReturn;
        }


        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions

        internal static async Task<bool> DeleteFullBy_CountryAsync(int iCountryId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // get a list of the ids that reference this
                var lstIds = await dB.tblRegions.AsNoTracking().Where(r => r.tblCountry_id == iCountryId).Select(r => r.tblRegion_id).ToListAsync();

                if (lstIds.Count > 0)
                {
                    foreach (var iId in lstIds)
                    {
                        _ = await DeleteFullAsync(iId, strBy, bClearCache: false);
                    }

                    LocalCache.RefreshCache(CacheKey);
                }

                lstIds = null;

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Region).ToString(), "DeleteFullBy_CountryAsync(int, string)", ex,
                    "iCountryId: " + iCountryId.ToString() +
                    ", strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        internal static async Task<bool> Update_WebsiteIntegration(int iId, int? iWebsiteId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblRegions.FirstOrDefaultAsync(r => r.tblRegion_id == iId);

                if (efmObject != null)
                {
                    efmObject.tblRegion_websiteId = iWebsiteId;

                    efmObject.tblRegion_editedUtc = DateTime.UtcNow;
                    efmObject.tblRegion_editedBy = strBy;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        LocalCache.RefreshCache(CacheKey);
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Region).ToString(), "Update_WebsiteIntegration(int, int?, string)", ex,
                        "iId: " + iId.ToString() +
                        ", iWebsiteId: " + (iWebsiteId.HasValue ? iWebsiteId.Value.ToString() : "") +
                        ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        #endregion internal functions


        #region Public Functions

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(int iCountryId, string strBy)
        {
            if (Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblRegion
                {
                    tblCountry_id = iCountryId,
                    tblRegion_websiteId = null,
                    tblRegion_name = Name,
                    tblRegion_desc = Description,
                    tblRegion_createdUtc = DateTime.UtcNow,
                    tblRegion_createdBy = strBy,
                    tblRegion_editedUtc = DateTime.UtcNow,
                    tblRegion_editedBy = strBy,
                    tblRegion_deletedUtc = null,
                    tblRegion_deletedBy = ""
                };

                _ = dB.tblRegions.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);

                    if ((await Integrations.Website.API.Region_CreateAsync(this)).Result)
                    {
                        _ = await RefreshAsync();
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Region).ToString(), "CreateAsync(int, string)", ex,
                        "iCountryId: " + iCountryId +
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

                var efmObject = await dB.tblRegions.FirstOrDefaultAsync(r => r.tblRegion_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblRegion_name = Name;
                    efmObject.tblRegion_desc = Description;

                    efmObject.tblRegion_editedUtc = DateTime.UtcNow;
                    efmObject.tblRegion_editedBy = strBy;

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
                _ = Error.Exception(typeof(Region).ToString(), "Save(string)", ex,
                        "Id: " + Id.ToString() +
                        ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteAsync(int iId, string strBy, bool bDeleted = true, bool bClearCache = true)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblRegions.FirstOrDefaultAsync(r => r.tblRegion_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblRegion_deletedUtc.HasValue)
                    {
                        // everything is correct
                        bReturn = true;
                    }
                    else
                    {
                        // need to make update
                        if (bDeleted)
                        {
                            efmObject.tblRegion_deletedUtc = DateTime.UtcNow;
                            efmObject.tblRegion_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblRegion_deletedUtc = null;
                            efmObject.tblRegion_deletedBy = "";
                        }

                        efmObject.tblRegion_editedUtc = DateTime.UtcNow;
                        efmObject.tblRegion_editedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
                        }
                    }
                }
                else
                {
                    // not found so already deleted
                    bReturn = true;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Region).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bDeleted: " + bDeleted.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteFullAsync(int iId, string strBy, bool bClearCache = true)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblRegions.FirstOrDefaultAsync(r => r.tblRegion_id == iId);

                if (efmObject != null)
                {
                    // Delete related elements (i.e those that ref this)


                    // delete the element
                    _ = dB.tblRegions.Remove(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

                        if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
                    }
                }
                else
                {
                    // not found so already deleted
                    bReturn = true;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Region).ToString(), "DeleteFullAsync(int, string, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Region> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Region(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Region();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Region();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Region>> FindAllAsync(bool bIncDeleted = false)
        {
            var lstReturn = new List<Region>();

            foreach (var obj in (await Global_ListAsync()).Values)
            {
                if (!bIncDeleted && obj.Deleted_UTC.HasValue) { continue; }

                lstReturn.Add(obj);
            }

            lstReturn = lstReturn.OrderBy(r => r.Name).ToList();

            return lstReturn;
        }

        public static async Task<List<Region>> FindAllBy_CountryAsync(int iCountryId, bool bIncDeleted = false)
        {
            return await FindAllBy_CountryAsync(new List<int> { iCountryId }, bIncDeleted);
        }

        public static async Task<List<Region>> FindAllBy_CountryAsync(List<int> lstCountryIds, bool bIncDeleted = false)
        {
            var lstReturn = new List<Region>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Common_Regions_IdxCountry) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iCountryId in lstCountryIds)
                {
                    if (lstIndex.TryGetValue(iCountryId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            if (!bIncDeleted)
                            {
                                if (lstGlobal[iId].Deleted_UTC.HasValue) { continue; }
                            }

                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstGlobal = null;

            lstReturn = lstReturn.OrderBy(r => r.Name).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Region>> Global_ListAsync()
        {
            // check to see if cache is populated (including indexes)
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Region> lstElements ||
                LocalCache.Get(LocalCache.Key.Common_Regions_IdxCountry) is not Dictionary<int, List<int>> dicIndexCountry)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, Region>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    await s_singleCacheBuildLock.WaitAsync();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Region> lstElementsRetry)
                    {
                        // cache now populated, no need to build
                        return lstElementsRetry;
                    }

                    // setup index dictionaries
                    dicIndexCountry = new Dictionary<int, List<int>>();

                    // create instance of DB
                    using var dB = Settings.Config.DBPooledConnection();

                    // disable change tracking as only reading
                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    // get all the elements
                    foreach (var efmObject in await dB.tblRegions.AsNoTracking().ToListAsync())
                    {
                        var obj = new Region(efmObject);

                        if (obj.Loaded)
                        {
                            // successfully populated/loaded info, add to global list using id as key
                            lstElements.Add(obj.Id, obj);

                            // add to country index
                            if (!dicIndexCountry.ContainsKey(obj.Country_Id))
                            {
                                dicIndexCountry.Add(obj.Country_Id, new List<int>());
                            }

                            dicIndexCountry[obj.Country_Id].Add(obj.Id);
                        }

                        obj = null;
                    }

                    // add the found elements to the cache
                    LocalCache.Set(CacheKey, lstElements);

                    // add indexes to cache
                    LocalCache.Set(LocalCache.Key.Common_Regions_IdxCountry, dicIndexCountry);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Region).ToString(), "Global_ListAsync()", ex);
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
