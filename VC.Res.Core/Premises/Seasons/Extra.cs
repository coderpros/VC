using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Premises.Seasons
{
    public class Extra : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Seasons_Extras;

        #region Properties

        public int Season_Id { get; private set; } = 0;
        public int? Extra_Id { get; private set; } = null;

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public Enums.Shared_PriceValueType Price_EntryMode { get; set; } = Enums.Shared_PriceValueType.Net;
        public decimal Price { get; set; } = 0;

        public bool Commission_SubjectTo { get; set; } = false;
        public Enums.Shared_NumericValueType Commission_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public decimal Commission_Amount { get; set; } = 0;
        public string Commission_Note { get; set; } = "";

        public bool Tax_Exempt { get; set; } = false;
        public decimal Tax_Value { get; set; } = 0;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";
        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string Edited_By { get; private set; } = "";
        public DateTime? Deleted_UTC { get; private set; } = null;
        public string Deleted_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Extra() { }

        private Extra(tblPropertySeasonExtra efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Properties

        #region Private Properties-Loaders

        protected override async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblPropertySeasonExtras.AsNoTracking().FirstOrDefaultAsync(r => r.tblPropertySeasonExtra_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Extra).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblPropertySeasonExtra efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertySeasonExtra_id;

                    Extra_Id = efmObject.tblPropertyExtra_id;

                    Season_Id = efmObject.tblPropertySeason_id;

                    Name = efmObject.tblPropertySeasonExtra_name;
                    Description = efmObject.tblPropertySeasonExtra_desc;

                    Price_EntryMode = (Enums.Shared_PriceValueType)efmObject.tblPropertySeasonExtra_priceEntryMode;
                    Price = efmObject.tblPropertySeasonExtra_price;

                    Commission_SubjectTo = efmObject.tblPropertySeasonExtra_commissionSubjectTo;
                    Commission_AmountType = (Enums.Shared_NumericValueType)efmObject.tblPropertySeasonExtra_commissionAmountType;
                    Commission_Amount = efmObject.tblPropertySeasonExtra_commissionAmount;
                    Commission_Note = efmObject.tblPropertySeasonExtra_commissionNote;

                    Tax_Exempt = efmObject.tblPropertySeasonExtra_taxExempt;
                    Tax_Value = efmObject.tblPropertySeasonExtra_taxValue;

                    Created_UTC = efmObject.tblPropertySeasonExtra_createdUTC;
                    Created_By = efmObject.tblPropertySeasonExtra_createdBy;

                    Edited_UTC = efmObject.tblPropertySeasonExtra_editedUTC;
                    Edited_By = efmObject.tblPropertySeasonExtra_editedBy;

                    Deleted_UTC = efmObject.tblPropertySeasonExtra_deletedUTC;
                    Deleted_By = efmObject.tblPropertySeasonExtra_deletedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblPropertySeasonExtra)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private properties-loaders

        #endregion private properties


        #region Internal Functions

        internal static async Task CloneAsync(int iOriginalSeasonId, int iNewSeasonId, string strBy)
        {
            try
            {
                // loop through the extras from the original season and create new version
                // in the new season
                var lstOriginalSeasonExtras = await FindAllBy_SeasonAsync(iOriginalSeasonId);

                foreach (var objOriginalSeasonExtra in lstOriginalSeasonExtras)
                {
                    var objNewSeasonExtra = new Extra
                    {
                        Name = objOriginalSeasonExtra.Name,
                        Description = objOriginalSeasonExtra.Description,
                        Price_EntryMode = objOriginalSeasonExtra.Price_EntryMode,
                        Price = objOriginalSeasonExtra.Price,
                        Commission_SubjectTo = objOriginalSeasonExtra.Commission_SubjectTo,
                        Commission_AmountType = objOriginalSeasonExtra.Commission_AmountType,
                        Commission_Amount = objOriginalSeasonExtra.Commission_Amount,
                        Commission_Note = objOriginalSeasonExtra.Commission_Note,
                        Tax_Exempt = objOriginalSeasonExtra.Tax_Exempt,
                        Tax_Value = objOriginalSeasonExtra.Tax_Value
                    };

                    _ = await objNewSeasonExtra.CreateAsync(iNewSeasonId, objOriginalSeasonExtra.Extra_Id, strBy);

                    objNewSeasonExtra = null;
                }

                lstOriginalSeasonExtras = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Extra).ToString(), "CloneAsync(int, int, string)", ex,
                    "iOriginalSeasonId: " + iOriginalSeasonId.ToString() +
                    ", iNewSeasonId: " + iNewSeasonId.ToString() +
                    ", strBy: " + strBy.ToString());
            }
        }

        internal static async Task<bool> DeleteBy_SeasonAsync(int iSeasonId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var iChanges = await dB.tblPropertySeasonExtras.Where(r => r.tblPropertySeason_id == iSeasonId && r.tblPropertySeasonExtra_deletedUTC == null)
                                                        .UpdateAsync(r => new tblPropertySeasonExtra
                                                        {
                                                            tblPropertySeasonExtra_deletedUTC = DateTime.UtcNow,
                                                            tblPropertySeasonExtra_deletedBy = strBy
                                                        });

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Extra).ToString(), "DeleteBy_SeasonAsync(int, string)", ex,
                    "iSeasonId: " + iSeasonId.ToString() +
                    ", strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        internal static async Task<bool> DeleteFullBy_SeasonAsync(int iSeasonId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var iChanges = await dB.tblPropertySeasonExtras.Where(r => r.tblPropertySeason_id == iSeasonId).DeleteAsync();

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Extra).ToString(), "DeleteFullBy_SeasonAsync(int, string)", ex,
                    "iSeasonId: " + iSeasonId.ToString() +
                    ", strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion internal functions


        #region Public Functions

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(int iSeasonId, int? iExtraId, string strBy)
        {
            if (Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblPropertySeasonExtra
                {
                    tblPropertySeason_id = iSeasonId,
                    tblPropertyExtra_id = iExtraId,

                    tblPropertySeasonExtra_name = Name,
                    tblPropertySeasonExtra_desc = Description,

                    tblPropertySeasonExtra_priceEntryMode = (int)Price_EntryMode,
                    tblPropertySeasonExtra_price = Price,

                    tblPropertySeasonExtra_commissionSubjectTo = Commission_SubjectTo,
                    tblPropertySeasonExtra_commissionAmountType = (int)Commission_AmountType,
                    tblPropertySeasonExtra_commissionAmount = Commission_Amount,
                    tblPropertySeasonExtra_commissionNote = Commission_Note,

                    tblPropertySeasonExtra_taxExempt = Tax_Exempt,
                    tblPropertySeasonExtra_taxValue = Tax_Value,

                    tblPropertySeasonExtra_createdUTC = DateTime.UtcNow,
                    tblPropertySeasonExtra_createdBy = strBy,
                    tblPropertySeasonExtra_editedUTC = DateTime.UtcNow,
                    tblPropertySeasonExtra_editedBy = strBy,
                    tblPropertySeasonExtra_deletedUTC = null,
                    tblPropertySeasonExtra_deletedBy = ""
                };

                _ = dB.tblPropertySeasonExtras.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Extra).ToString(), "CreateAsync(int, int?, string)", ex,
                        "iSeasonId: " + iSeasonId.ToString() +
                        ", iExtraId: " + (iExtraId.HasValue ? iExtraId.Value.ToString() : "") +
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

                var efmObject = await dB.tblPropertySeasonExtras.FirstOrDefaultAsync(r => r.tblPropertySeasonExtra_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblPropertySeasonExtra_name = Name;
                    efmObject.tblPropertySeasonExtra_desc = Description;

                    efmObject.tblPropertySeasonExtra_priceEntryMode = (int)Price_EntryMode;
                    efmObject.tblPropertySeasonExtra_price = Price;

                    efmObject.tblPropertySeasonExtra_commissionSubjectTo = Commission_SubjectTo;
                    efmObject.tblPropertySeasonExtra_commissionAmountType = (int)Commission_AmountType;
                    efmObject.tblPropertySeasonExtra_commissionAmount = Commission_Amount;
                    efmObject.tblPropertySeasonExtra_commissionNote = Commission_Note;

                    efmObject.tblPropertySeasonExtra_taxExempt = Tax_Exempt;
                    efmObject.tblPropertySeasonExtra_taxValue = Tax_Value;

                    efmObject.tblPropertySeasonExtra_editedUTC = DateTime.UtcNow;
                    efmObject.tblPropertySeasonExtra_editedBy = strBy;

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
                _ = Error.Exception(typeof(Extra).ToString(), "Save(string)", ex,
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

                var efmObject = await dB.tblPropertySeasonExtras.FirstOrDefaultAsync(r => r.tblPropertySeasonExtra_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblPropertySeasonExtra_deletedUTC.HasValue)
                    {
                        // already in desired state
                        bReturn = true;
                    }
                    else
                    {
                        if (bDeleted)
                        {
                            efmObject.tblPropertySeasonExtra_deletedUTC = DateTime.UtcNow;
                            efmObject.tblPropertySeasonExtra_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblPropertySeasonExtra_deletedUTC = null;
                            efmObject.tblPropertySeasonExtra_deletedBy = "";
                        }

                        efmObject.tblPropertySeasonExtra_editedUTC = DateTime.UtcNow;
                        efmObject.tblPropertySeasonExtra_editedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
                        }
                    }
                }
                else
                {
                    // not found, probably fully deleted, only return success if wanted deleted
                    if (bDeleted)
                    {
                        bReturn = true;
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Extra).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bDeleted: " + bDeleted.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteFullAsync(int iId, string strBy, bool bClearCache = true, bool bForce = false)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertySeasonExtras.FirstOrDefaultAsync(r => r.tblPropertySeasonExtra_id == iId);

                if (efmObject != null)
                {
                    // can only fully delete if already flagged for deletion
                    if (efmObject.tblPropertySeasonExtra_deletedUTC != null)
                    {
                        // can only delete if flagged more than short recycle bin period days ago or this is a force action
                        if (efmObject.tblPropertySeasonExtra_deletedUTC < DateTime.UtcNow.AddDays(-Settings.Variables.RecycleBin_EmptyAfterDaysShort) || bForce)
                        {
                            _ = dB.tblPropertySeasonExtras.Remove(efmObject);

                            if (await dB.SaveChangesAsync() > 0)
                            {
                                bReturn = true;

                                if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
                            }
                        }
                    }
                }
                else
                {
                    // already fully deleted as can't find
                    bReturn = true;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Extra).ToString(), "DeleteFullAsync(int, string, bool, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bClearCache: " + bClearCache.ToString() +
                    ", bForce: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Extra> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Extra(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Extra();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Extra();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Extra>> FindAllAsync(List<int> iIds)
        {
            var lstReturn = new List<Extra>();

            // get global list
            var lstGlobal = await Global_ListAsync();

            foreach (var iId in iIds)
            {
                if (lstGlobal.TryGetValue(iId, out var value))
                {
                    lstReturn.Add(value);
                }
            }

            lstGlobal = null;

            return lstReturn;
        }

        public static async Task<List<Extra>> FindAllBy_ExtraAsync(int iExtraId, bool bIncDeleted = false)
        {
            return await FindAllBy_ExtraAsync(new List<int> { iExtraId }, bIncDeleted: bIncDeleted);
        }

        public static async Task<List<Extra>> FindAllBy_ExtraAsync(List<int> lstExtraIds, bool bIncDeleted = false)
        {
            var lstReturn = new List<Extra>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Seasons_Extras_IdxExtra) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iExtraId in lstExtraIds)
                {
                    if (lstIndex.TryGetValue(iExtraId, out var value))
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

        public static async Task<List<Extra>> FindAllBy_SeasonAsync(int iSeasonId, bool bIncDeleted = false)
        {
            return await FindAllBy_SeasonAsync(new List<int> { iSeasonId }, bIncDeleted: bIncDeleted);
        }

        public static async Task<List<Extra>> FindAllBy_SeasonAsync(List<int> lstSeasonIds, bool bIncDeleted = false)
        {
            var lstReturn = new List<Extra>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Seasons_Extras_IdxSeason) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iSeasonId in lstSeasonIds)
                {
                    if (lstIndex.TryGetValue(iSeasonId, out var value))
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

        private static async Task<Dictionary<int, Extra>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Extra> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Seasons_Extras_IdxExtra) is not Dictionary<int, List<int>> dicIndexExtra ||
                LocalCache.Get(LocalCache.Key.Premises_Seasons_Extras_IdxSeason) is not Dictionary<int, List<int>> dicIndexSeason)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, Extra>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    await s_singleCacheBuildLock.WaitAsync();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Extra> lstElementsRetry)
                    {
                        // cache now populated, no need to build
                        return lstElementsRetry;
                    }

                    // setup index dictionaries
                    dicIndexExtra = new Dictionary<int, List<int>>();
                    dicIndexSeason = new Dictionary<int, List<int>>();

                    // create instance of DB
                    using var dB = Settings.Config.DBPooledConnection();

                    // disable change tracking as only reading
                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    // get all the elements
                    foreach (var efmObject in await dB.tblPropertySeasonExtras.AsNoTracking().ToListAsync())
                    {
                        var obj = new Extra(efmObject);

                        if (obj.Loaded)
                        {
                            // successfully populated/loaded info, add to global list using id as key
                            lstElements.Add(obj.Id, obj);

                            // add to season index
                            if (!dicIndexSeason.ContainsKey(obj.Season_Id))
                            {
                                dicIndexSeason.Add(obj.Season_Id, new List<int>());
                            }
                            dicIndexSeason[obj.Season_Id].Add(obj.Id);

                            // add to extras index if applicable
                            if (obj.Extra_Id.HasValue)
                            {
                                if (!dicIndexExtra.ContainsKey(obj.Extra_Id.Value))
                                {
                                    dicIndexExtra.Add(obj.Extra_Id.Value, new List<int>());
                                }
                                dicIndexExtra[obj.Extra_Id.Value].Add(obj.Id);
                            }
                        }

                        obj = null;
                    }

                    // add the found elements to the cache
                    LocalCache.Set(CacheKey, lstElements);

                    // add indexes to cache
                    LocalCache.Set(LocalCache.Key.Premises_Seasons_Extras_IdxExtra, dicIndexExtra);
                    LocalCache.Set(LocalCache.Key.Premises_Seasons_Extras_IdxSeason, dicIndexSeason);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Extra).ToString(), "Global_ListAsync()", ex);
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
