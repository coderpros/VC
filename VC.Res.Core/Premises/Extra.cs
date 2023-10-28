using System.Data;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;

namespace VC.Res.Core.Premises
{
    public class Extra : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Extras;

        #region Properties

        public int Premise_Id { get; private set; } = 0;

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";


        public bool Price_EntryMode_Inherit { get; set; } = true;
        public Enums.Shared_PriceValueType Price_EntryMode { get; set; } = Enums.Shared_PriceValueType.Net;
        public Enums.Shared_PriceValueType Price_EntryMode_Calculated
        {
            get
            {
                if (Price_EntryMode_Inherit) { return _inherited_PriceEntryMode; }
                return Price_EntryMode;
            }
        }
        public decimal Price { get; set; } = 0;

        public bool Commission_SubjectTo { get; set; } = false;
        public bool Commission_Inherit { get; set; } = true;
        public Enums.Shared_NumericValueType Commission_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public Enums.Shared_NumericValueType Commission_AmountType_Calculated
        {
            get
            {
                if (Commission_Inherit) { return _inherited_Commission_AmountType; }
                return Commission_AmountType;
            }
        }
        public decimal? Commission_Amount { get; set; } = null;
        public decimal Commission_Amount_Calculated
        {
            get
            {
                if (Commission_Inherit) { return _inherited_Commission_Amount; }
                return Commission_Amount ?? 0;
            }
        }
        public string Commission_Note { get; set; } = "";
        public string Commission_Note_Calculated
        {
            get
            {
                if (Commission_Inherit) { return _inherited_Commission_Note; }
                return Commission_Note;
            }
        }

        public bool Tax_Inherit { get; set; } = true;
        public bool Tax_Exempt { get; set; } = false;
        public bool Tax_Exempt_Calculated
        {
            get
            {
                if (Tax_Inherit) { return _inherited_Tax_Exempt; }
                return Tax_Exempt;
            }
        }
        public decimal? Tax_Value { get; set; } = null;
        public decimal Tax_Value_Calculated
        {
            get
            {
                if (Tax_Inherit) { return _inherited_Tax_Value; }
                return Tax_Value ?? 0;
            }
        }

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string Edited_By { get; private set; } = "";

        public DateTime? Deleted_UTC { get; private set; } = null;
        public string Deleted_By { get; private set; } = "";


        // private properties to hold calculated inherited values from premise config (to avoid async)
        // dependency added to cache so that when configs are changed, extras cache is also cleared forcing these to be
        // recalculated so always correct if needed/used.
        private Enums.Shared_PriceValueType _inherited_PriceEntryMode = Enums.Shared_PriceValueType.Net;

        private Enums.Shared_NumericValueType _inherited_Commission_AmountType = Enums.Shared_NumericValueType.Percentage;
        private decimal _inherited_Commission_Amount = 0;
        private string _inherited_Commission_Note = "";

        private bool _inherited_Tax_Exempt = false;
        private decimal _inherited_Tax_Value = 0;

        #endregion properties


        #region Constructors

        public Extra() { }

        //private Extra(tblPropertyExtra efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Properties

        #region Private Functions-Loaders

        protected override async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblPropertyExtras.AsNoTracking().FirstOrDefaultAsync(r => r.tblPropertyExtra_id == iId);

                if (efmObject != null)
                {
                    bReturn = await LoadAsync(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Config).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private async Task<bool> LoadAsync(tblPropertyExtra efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertyExtra_id;
                    Premise_Id = efmObject.tblProperty_id;

                    Name = efmObject.tblPropertyExtra_name;
                    Description = efmObject.tblPropertyExtra_desc;

                    Price_EntryMode_Inherit = efmObject.tblPropertyExtra_priceEntryModeInh;
                    Price_EntryMode = (Enums.Shared_PriceValueType)efmObject.tblPropertyExtra_priceEntryMode;
                    Price = efmObject.tblPropertyExtra_price;

                    Commission_SubjectTo = efmObject.tblPropertyExtra_commissionSubjectTo;
                    Commission_Inherit = efmObject.tblPropertyExtra_commissionInh;
                    Commission_AmountType = (Enums.Shared_NumericValueType)efmObject.tblPropertyExtra_commissionAmountType;
                    Commission_Amount = efmObject.tblPropertyExtra_commissionAmount;
                    Commission_Note = efmObject.tblPropertyExtra_commissionNote;

                    Tax_Inherit = efmObject.tblPropertyExtra_taxInh;
                    Tax_Value = efmObject.tblPropertyExtra_taxValue;
                    Tax_Exempt = efmObject.tblPropertyExtra_taxExempt;

                    Created_UTC = efmObject.tblPropertyExtra_createdUTC;
                    Created_By = efmObject.tblPropertyExtra_createdBy;

                    Edited_UTC = efmObject.tblPropertyExtra_editedUTC;
                    Edited_By = efmObject.tblPropertyExtra_editedBy;

                    Deleted_UTC = efmObject.tblPropertyExtra_deletedUTC;
                    Deleted_By = efmObject.tblPropertyExtra_deletedBy;

                    // work out inherited values
                    var objPremiseConfig = await Config.FindBy_PremiseAsync(Premise_Id);

                    _inherited_PriceEntryMode = objPremiseConfig.PriceEntryMode_Calculated;

                    _inherited_Commission_AmountType = objPremiseConfig.Commission_AmountType_Calculated;
                    _inherited_Commission_Amount = objPremiseConfig.Commission_Amount_Calculated;
                    _inherited_Commission_Note = objPremiseConfig.Commission_Note_Calculated;

                    _inherited_Tax_Exempt = objPremiseConfig.Tax_Exempt_Calculated;
                    _inherited_Tax_Value = objPremiseConfig.Tax_Value_Calculated;

                    objPremiseConfig = null;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "LoadAsync(tblPropertyExtra)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private functions-loaders



        #endregion private properties


        #region Internal Functions



        #endregion internal functions


        #region Public Functions
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(int iPremiseId, string strBy)
        {
            if (Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblPropertyExtra
                {
                    tblProperty_id = iPremiseId,

                    tblPropertyExtra_name = Name,
                    tblPropertyExtra_desc = Description,

                    tblPropertyExtra_priceEntryModeInh = Price_EntryMode_Inherit,
                    tblPropertyExtra_priceEntryMode = (int)Price_EntryMode,
                    tblPropertyExtra_price = Price,

                    tblPropertyExtra_commissionSubjectTo = Commission_SubjectTo,
                    tblPropertyExtra_commissionInh = Commission_Inherit,
                    tblPropertyExtra_commissionAmountType = (int)Commission_AmountType,
                    tblPropertyExtra_commissionAmount = Commission_Amount,
                    tblPropertyExtra_commissionNote = Commission_Note,

                    tblPropertyExtra_taxInh = Tax_Inherit,
                    tblPropertyExtra_taxExempt = Tax_Exempt,
                    tblPropertyExtra_taxValue = Tax_Value,

                    tblPropertyExtra_createdUTC = DateTime.UtcNow,
                    tblPropertyExtra_createdBy = strBy,
                    tblPropertyExtra_editedUTC = DateTime.UtcNow,
                    tblPropertyExtra_editedBy = strBy,
                    tblPropertyExtra_deletedUTC = null,
                    tblPropertyExtra_deletedBy = ""
                };

                _ = dB.tblPropertyExtras.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = await LoadAsync(efmObject);

                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Extra).ToString(), "CreateAsync(int, string)", ex,
                    "iPremiseId: " + iPremiseId.ToString() +
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

                var efmObject = await dB.tblPropertyExtras.FirstOrDefaultAsync(r => r.tblPropertyExtra_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblPropertyExtra_name = Name;
                    efmObject.tblPropertyExtra_desc = Description;

                    efmObject.tblPropertyExtra_priceEntryModeInh = Price_EntryMode_Inherit;
                    efmObject.tblPropertyExtra_priceEntryMode = (int)Price_EntryMode;
                    efmObject.tblPropertyExtra_price = Price;

                    efmObject.tblPropertyExtra_commissionSubjectTo = Commission_SubjectTo;
                    efmObject.tblPropertyExtra_commissionInh = Commission_Inherit;
                    efmObject.tblPropertyExtra_commissionAmountType = (int)Commission_AmountType;
                    efmObject.tblPropertyExtra_commissionAmount = Commission_Amount;
                    efmObject.tblPropertyExtra_commissionNote = Commission_Note;

                    efmObject.tblPropertyExtra_taxInh = Tax_Inherit;
                    efmObject.tblPropertyExtra_taxExempt = Tax_Exempt;
                    efmObject.tblPropertyExtra_taxValue = Tax_Value;

                    efmObject.tblPropertyExtra_editedUTC = DateTime.UtcNow;
                    efmObject.tblPropertyExtra_editedBy = strBy;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = await LoadAsync(efmObject);

                        LocalCache.RefreshCache(CacheKey);
                    }
                }
                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Extra).ToString(), "SaveAsync(string)", ex,
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

                var efmObject = await dB.tblPropertyExtras.FirstOrDefaultAsync(r => r.tblPropertyExtra_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblPropertyExtra_deletedUTC.HasValue)
                    {
                        // already in desired state
                        bReturn = true;
                    }
                    else
                    {
                        if (bDeleted)
                        {
                            efmObject.tblPropertyExtra_deletedUTC = DateTime.UtcNow;
                            efmObject.tblPropertyExtra_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblPropertyExtra_deletedUTC = null;
                            efmObject.tblPropertyExtra_deletedBy = "";
                        }

                        efmObject.tblPropertyExtra_deletedUTC = DateTime.UtcNow;
                        efmObject.tblPropertyExtra_deletedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
                        }
                    }
                }
                else
                {
                    bReturn = true;
                }
                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Group).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bDeleted: " + bDeleted.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
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

        public static async Task<List<Extra>> FindAllAsync(List<int> lstIds)
        {
            var lstReturn = new List<Extra>();

            // get global list
            var lstGlobal = await Global_ListAsync();

            foreach (var iId in lstIds.Distinct())
            {
                if (lstGlobal.TryGetValue(iId, out var value))
                {
                    lstReturn.Add(value);
                }
            }

            lstGlobal = null;

            return lstReturn;
        }

        public static async Task<List<Extra>> FindAllBy_PremiseAsync(int iPremiseId, bool bIncDeleted = false)
        {
            return await FindAllBy_PremiseAsync(new List<int> { iPremiseId }, bIncDeleted);
        }

        public static async Task<List<Extra>> FindAllBy_PremiseAsync(List<int> lstPremiseIds, bool bIncDeleted = false)
        {
            var lstReturn = new List<Extra>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Extras_IdxPremise) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iPremiseId in lstPremiseIds.Distinct())
                {
                    if (lstIndex.TryGetValue(iPremiseId, out var value))
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
                LocalCache.Get(LocalCache.Key.Premises_Extras_IdxPremise) is not Dictionary<int, List<int>> dicIndexPremise)
            {
                lstElements = new Dictionary<int, Extra>();

                try
                {
                    await s_singleCacheBuildLock.WaitAsync();

                    // attempt to reget incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Extra> lstElementsRetry)
                    {
                        return lstElementsRetry;
                    }

                    dicIndexPremise = new Dictionary<int, List<int>>();

                    using var dB = Settings.Config.DBPooledConnection();

                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var efmObject in await dB.tblPropertyExtras.AsNoTracking().ToListAsync())
                    {
                        var obj = new Extra();

                        if (await obj.LoadAsync(efmObject))
                        {
                            lstElements.Add(obj.Id, obj);

                            // add to premise index
                            if (!dicIndexPremise.ContainsKey(obj.Premise_Id))
                            {
                                dicIndexPremise.Add(obj.Premise_Id, new List<int>());
                            }

                            dicIndexPremise[obj.Premise_Id].Add(obj.Id);
                        }

                        obj = null;
                    }

                    LocalCache.Set(CacheKey, lstElements);
                    LocalCache.Set(LocalCache.Key.Premises_Extras_IdxPremise, dicIndexPremise);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Extra).ToString(), "Global_ListAsync()", ex);
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
