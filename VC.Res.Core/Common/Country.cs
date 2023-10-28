using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Common
{
    public class Country : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Common_Countries;

        #region Properties

        //public bool Loaded { get; private set; } = false;

        //public int Id { get; private set; } = 0;

        public int? Website_Id { get; private set; } = null;

        public string Name { get; set; } = "";

        public string A2 { get; set; } = "";

        public string A3 { get; set; } = "";

        public int? Number { get; set; } = null;

        public decimal? Tax_Rate { get; set; } = null;

        public bool Enabled { get; set; } = false;

        public int Order { get; private set; } = 0;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string Edited_By { get; private set; } = "";

        public DateTime? Deleted_UTC { get; private set; } = null;
        public string Deleted_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Country() { }

        private Country(tblCountry efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        protected override async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                var efmObject = await dB.tblCountries.AsNoTracking().FirstOrDefaultAsync(r => r.tblCountry_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblCountry efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblCountry_id;

                    Website_Id = efmObject.tblCountry_websiteId;
                    Name = efmObject.tblCountry_name;
                    A2 = efmObject.tblCountry_A2;
                    A3 = efmObject.tblCountry_A3;
                    Number = efmObject.tblCountry_number;
                    Order = efmObject.tblCountry_order;
                    Enabled = efmObject.tblCountry_enabled;
                    Tax_Rate = efmObject.tblCountry_taxRate;

                    Created_UTC = efmObject.tblCountry_createdUtc;
                    Created_By = efmObject.tblCountry_createdBy;

                    Edited_UTC = efmObject.tblCountry_editedUtc;
                    Edited_By = efmObject.tblCountry_editedBy;

                    Deleted_UTC = efmObject.tblCountry_deletedUtc;
                    Deleted_By = efmObject.tblCountry_deletedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblCountry)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions

        internal static async Task<bool> Update_WebsiteIntegration(int iId, int? iWebsiteId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblCountries.FirstOrDefaultAsync(r => r.tblCountry_id == iId);

                if (efmObject != null)
                {
                    efmObject.tblCountry_websiteId = iWebsiteId;

                    efmObject.tblCountry_editedUtc = DateTime.UtcNow;
                    efmObject.tblCountry_editedBy = strBy;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        LocalCache.RefreshCache(CacheKey);
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Country).ToString(), "Update_WebsiteIntegration(int, int?, string)", ex,
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

        public async Task<bool> CreateAsync(string strBy)
        {
            if (Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblCountry
                {
                    tblCountry_websiteId = null,
                    tblCountry_name = Name,
                    tblCountry_A2 = A2,
                    tblCountry_A3 = A3,
                    tblCountry_number = Number,
                    tblCountry_order = await dB.tblCountries.AsNoTracking().CountAsync(r => r.tblCountry_deletedUtc == null) + 1,
                    tblCountry_enabled = Enabled,
                    tblCountry_taxRate = Tax_Rate,
                    tblCountry_createdUtc = DateTime.UtcNow,
                    tblCountry_createdBy = strBy,
                    tblCountry_editedUtc = DateTime.UtcNow,
                    tblCountry_editedBy = strBy,
                    tblCountry_deletedUtc = null,
                    tblCountry_deletedBy = ""
                };

                _ = dB.tblCountries.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);

                    if ((await Integrations.Website.API.Country_CreateAsync(this)).Result)
                    {
                        _ = await RefreshAsync();
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Country).ToString(), "CreateAsync(string)", ex,
                    "strBy: " + strBy.ToString());
                return false;
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

                var efmObject = dB.tblCountries.FirstOrDefault(r => r.tblCountry_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblCountry_name = Name;
                    efmObject.tblCountry_A2 = A2;
                    efmObject.tblCountry_A3 = A3;
                    efmObject.tblCountry_number = Number;

                    efmObject.tblCountry_enabled = Enabled;
                    efmObject.tblCountry_taxRate = Tax_Rate;

                    efmObject.tblCountry_editedUtc = DateTime.UtcNow;
                    efmObject.tblCountry_editedBy = strBy;

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
                _ = Error.Exception(typeof(Country).ToString(), "SaveAsync(string)", ex,
                    "iId: " + Id.ToString() +
                    ", strBy: " + strBy.ToString());
                return false;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteAsync(int iId, string strBy, bool bDeleted = true, bool bClearCache = true)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblCountries.FirstOrDefaultAsync(r => r.tblCountry_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblCountry_deletedUtc.HasValue)
                    {
                        // everything is correct
                        bReturn = true;
                    }
                    else
                    {
                        var iOrder = 0;

                        // need to make update
                        if (bDeleted)
                        {
                            iOrder = efmObject.tblCountry_order;

                            efmObject.tblCountry_order = 0;
                            efmObject.tblCountry_deletedUtc = DateTime.UtcNow;
                            efmObject.tblCountry_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblCountry_order = await dB.tblCountries.AsNoTracking().CountAsync(r => r.tblCountry_deletedUtc == null) + 1;
                            efmObject.tblCountry_deletedUtc = null;
                            efmObject.tblCountry_deletedBy = "";
                        }

                        efmObject.tblCountry_editedUtc = DateTime.UtcNow;
                        efmObject.tblCountry_editedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            if (bDeleted)
                            {
                                // reorder the elements after it
                                _ = await dB.tblCountries.Where(r => r.tblCountry_order > iOrder &&
                                                                        r.tblCountry_deletedUtc == null)
                                                            .UpdateAsync(r => new tblCountry { tblCountry_order = r.tblCountry_order - 1 });
                            }

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
                _ = Error.Exception(typeof(Country).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
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

                var efmObject = await dB.tblCountries.FirstOrDefaultAsync(r => r.tblCountry_id == iId);

                if (efmObject != null)
                {
                    // record the order so we can reorder elements after it
                    var iOrder = efmObject.tblCountry_order;

                    // Delete related elements (i.e those that ref this)
                    _ = await Region.DeleteFullBy_CountryAsync(iId, strBy);

                    // delete the element
                    _ = dB.tblCountries.Remove(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

                        // reorder the elements after it
                        _ = await dB.tblCountries.Where(r => r.tblCountry_order > iOrder &&
                                                                r.tblCountry_deletedUtc == null)
                                                    .UpdateAsync(r => new tblCountry { tblCountry_order = r.tblCountry_order - 1 });

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
                _ = Error.Exception(typeof(Country).ToString(), "DeleteFullAsync(int, string, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Move_UpAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                // variables to remember positions
                var iOldPosition = 0;
                var iNewPosition = 0;

                // get the item to move up
                var efmMoveUp = await dB.tblCountries.FirstOrDefaultAsync(r => r.tblCountry_id == iId);

                if (efmMoveUp == null) { return false; }

                // calculate new positions
                iOldPosition = efmMoveUp.tblCountry_order;
                iNewPosition = iOldPosition - 1;

                // check new position is not above position 1 (i.e top)
                if (iNewPosition < 1)
                {
                    // the item is already at the top and so can go no higher
                    efmMoveUp = null;
                    return true;
                }

                // get the item to switch position with (i.e move down)
                var efmMoveDown = await dB.tblCountries.FirstOrDefaultAsync(r => r.tblCountry_order == iNewPosition);

                if (efmMoveDown == null) { return false; }

                // switch the order positions of the two items
                efmMoveUp.tblCountry_order = iNewPosition;
                efmMoveDown.tblCountry_order = iOldPosition;

                if (await dB.SaveChangesAsync() > 0)
                {
                    // at least one change made and no exception, assume both changes saved successfully
                    bReturn = true;

                    LocalCache.RefreshCache(CacheKey);
                }

                efmMoveUp = null;
                efmMoveDown = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Country).ToString(), "Move_UpAsync(int)", ex,
                    "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        public static async Task<bool> Move_DownAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var iOldPosition = 0;
                var iNewPosition = 0;

                var efmMoveDown = await dB.tblCountries.FirstOrDefaultAsync(r => r.tblCountry_id == iId);

                if (efmMoveDown == null) { return false; }

                // calculate new positions
                iOldPosition = efmMoveDown.tblCountry_order;
                iNewPosition = iOldPosition + 1;

                // check new position is not below the last item (i.e bottom)
                if (iNewPosition > (await dB.tblCountries.AsNoTracking().CountAsync(r => r.tblCountry_deletedUtc == null)))
                {
                    // the item is already at the bottom and so can go no lower
                    efmMoveDown = null;
                    return true;
                }

                // get the item to switch position with (i.e move up)
                var efmMoveUp = await dB.tblCountries.FirstOrDefaultAsync(r => r.tblCountry_order == iNewPosition);

                if (efmMoveUp == null) { return false; }

                // switch the order positions of the two items
                efmMoveUp.tblCountry_order = iOldPosition;
                efmMoveDown.tblCountry_order = iNewPosition;

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = true;

                    LocalCache.RefreshCache(CacheKey);
                }

                efmMoveUp = null;
                efmMoveDown = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Country).ToString(), "Move_DownAsync(int)", ex,
                    "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        public static async Task<bool> Move_ToPositionAsync(int iId, int iPosition)
        {
            if (iPosition < 1)
            {
                return false;
            }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblCountries.FirstOrDefaultAsync(r => r.tblCountry_id == iId);

                if (efmObject != null)
                {
                    // get the items to reorder
                    var lstToReOrder = await dB.tblCountries.Where(r => r.tblCountry_deletedUtc == null)
                                                            .OrderBy(r => r.tblCountry_order)
                                                            .ToListAsync();


                    if (iPosition <= lstToReOrder.Count)
                    {
                        var iNewPosition = 1;

                        foreach (var efmTmpObj in lstToReOrder)
                        {
                            if (iNewPosition == iPosition)
                            {
                                iNewPosition++;
                            }

                            if (efmTmpObj.tblCountry_id == iId)
                            {
                                efmTmpObj.tblCountry_order = iPosition;
                            }
                            else
                            {
                                efmTmpObj.tblCountry_order = iNewPosition;
                                iNewPosition++;
                            }
                        }

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            LocalCache.RefreshCache(CacheKey);
                        }
                    }

                    lstToReOrder = null;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Country).ToString(), "Move_ToPositionAsync(int, int)", ex,
                    "iId: " + iId.ToString() +
                    ", iPosition: " + iPosition.ToString());
                return false;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Country> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Country(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Country();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Country();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Country>> FindAllAsync(bool bIncDisabled = false, bool bIncDeleted = false)
        {
            var lstReturn = new List<Country>();

            foreach (var obj in (await Global_ListAsync()).Values)
            {
                if (!bIncDisabled && !obj.Enabled) { continue; }

                if (!bIncDeleted && obj.Deleted_UTC.HasValue) { continue; }

                lstReturn.Add(obj);
            }

            lstReturn = lstReturn.OrderBy(r => r.Order).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Country>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Country> lstElements)
            {
                lstElements = new Dictionary<int, Country>();

                try
                {
                    await s_singleCacheBuildLock.WaitAsync();

                    // attempt to reget incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Country> lstElementsRetry)
                    {
                        return lstElementsRetry;
                    }

                    using var dB = Settings.Config.DBPooledConnection();

                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var efmObject in await dB.tblCountries.AsNoTracking().ToListAsync())
                    {
                        var obj = new Country(efmObject);

                        if (obj.Loaded)
                        {
                            lstElements.Add(obj.Id, obj);
                        }

                        obj = null;
                    }

                    LocalCache.Set(CacheKey, lstElements);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Country).ToString(), "Global_ListAsync()", ex);
                    return lstElements;
                }
                finally
                {
                    // release lock
                    _ = s_singleCacheBuildLock.Release();
                }
            }

            return lstElements;
        }

        #endregion lists
    }
}
