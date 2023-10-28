using System.Data;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Premises.Seasons
{
    public class Rate : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Seasons_Rates;

        #region Properties

        public int Premise_Id { get; private set; } = 0;
        public int? Season_Id { get; private set; } = null;
        public int? Parent_Id { get; private set; } = null;

        public bool Provisional { get; set; } = false;
        public bool RequireReview { get; set; } = false;

        public DateTime Arrive { get; set; } = DateTime.Today.Date;
        public DateTime Depart { get; set; } = DateTime.Today.AddDays(7).Date;

        public int No_Nights { get; private set; } = 0;
        public int Min_PartySize { get; set; } = 0;

        public bool Available { get; set; } = true;

        public bool Price_POA { get; set; } = false;
        public Enums.Shared_PriceValueType Price_EntryMode { get; set; } = Enums.Shared_PriceValueType.Net;
        public decimal Price { get; set; } = 0;

        public Enums.Shared_NumericValueType Commission_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public decimal Commission_Amount { get; set; } = 0;
        public string Commission_Note { get; set; } = "";

        public bool Tax_Exempt { get; set; } = false;
        public decimal Tax_Value { get; set; } = 0;

        public bool Discount { get; set; } = false;
        public int Discount_Nights { get; set; } = 0;
        public Enums.Shared_PriceValueType Discount_EntryMode { get; set; } = Enums.Shared_PriceValueType.Gross;
        public Enums.Shared_NumericValueType Discount_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public decimal Discount_Amount { get; set; } = 0;
        public string Discount_Note { get; set; } = "";

        public List<int> SeasonExtra_ExcludeIds { get; set; } = new List<int>();

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";
        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string Edited_By { get; private set; } = "";
        public DateTime? Deleted_UTC { get; private set; } = null;
        public string Deleted_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Rate() { }

        private Rate(tblPropertyRate efmObject) { _ = Load(efmObject); }

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

                var efmObject = await dB.tblPropertyRates.AsNoTracking().FirstOrDefaultAsync(r => r.tblPropertyRate_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Rate).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblPropertyRate efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertyRate_id;

                    Premise_Id = efmObject.tblProperty_id;

                    Season_Id = efmObject.tblPropertySeason_id;

                    Parent_Id = efmObject.tblPropertyRate_parentId;

                    Provisional = efmObject.tblPropertyRate_provisional;
                    RequireReview = efmObject.tblPropertyRate_reqReview;

                    Arrive = efmObject.tblPropertyRate_dateArrive;
                    Depart = efmObject.tblPropertyRate_dateDepart;

                    No_Nights = efmObject.tblPropertyRate_noNights;
                    Min_PartySize = efmObject.tblPropertyRate_minPartySize;

                    Available = efmObject.tblPropertyRate_available;

                    Price_POA = efmObject.tblPropertyRate_pricePOA;
                    Price_EntryMode = (Enums.Shared_PriceValueType)efmObject.tblPropertyRate_priceEntryMode;
                    Price = efmObject.tblPropertyRate_price;

                    if (Price <= 0) { Price_POA = true; }

                    Commission_AmountType = (Enums.Shared_NumericValueType)efmObject.tblPropertyRate_commissionAmountType;
                    Commission_Amount = efmObject.tblPropertyRate_commissionAmount;
                    Commission_Note = efmObject.tblPropertyRate_commissionNote;

                    Tax_Exempt = efmObject.tblPropertyRate_taxExempt;
                    Tax_Value = efmObject.tblPropertyRate_taxValue;

                    Discount = efmObject.tblPropertyRate_discount;
                    Discount_Nights = efmObject.tblPropertyRate_discountNights;
                    Discount_EntryMode = (Enums.Shared_PriceValueType)efmObject.tblPropertyRate_discountEntryMode;
                    Discount_AmountType = (Enums.Shared_NumericValueType)efmObject.tblPropertyRate_discountAmountType;
                    Discount_Amount = efmObject.tblPropertyRate_discountAmount;

                    SeasonExtra_ExcludeIds = Utilities.General.ConvertToListInt(efmObject.tblPropertyRate_extraExcludes);

                    Created_UTC = efmObject.tblPropertyRate_createdUTC;
                    Created_By = efmObject.tblPropertyRate_createdBy;

                    Edited_UTC = efmObject.tblPropertyRate_editedUTC;
                    Edited_By = efmObject.tblPropertyRate_editedBy;

                    Deleted_UTC = efmObject.tblPropertyRate_deletedUTC;
                    Deleted_By = efmObject.tblPropertyRate_deletedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblPropertyRate)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private properties-loaders

        #endregion private properties


        #region Internal Functions

        internal static async Task<bool> DeleteBy_SeasonAsync(int iSeasonId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var iChanges = await dB.tblPropertyRates.Where(r => r.tblPropertySeason_id == iSeasonId && r.tblPropertyRate_deletedUTC == null)
                                                        .UpdateAsync(r => new tblPropertyRate
                                                        {
                                                            tblPropertyRate_deletedUTC = DateTime.UtcNow,
                                                            tblPropertyRate_deletedBy = strBy
                                                        });

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Rate).ToString(), "DeleteBy_SeasonAsync(int, string)", ex,
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

                var iChanges = await dB.tblPropertyRates.Where(r => r.tblPropertySeason_id == iSeasonId).DeleteAsync();

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Rate).ToString(), "DeleteFullBy_SeasonAsync(int, string)", ex,
                    "iSeasonId: " + iSeasonId.ToString() +
                    ", strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion internal functions


        #region Public Functions

        public async Task<List<string>> ValidateAsync(int iPremiseId, int? iSeasonId)
        {
            var lstReturn = new List<string>();

            Arrive = Arrive.Date;
            Depart = Depart.Date;

            if (Price == 0)
            {
                Price_POA = true;
            }

            if (Arrive == Depart)
            {
                lstReturn.Add("Arrival and departure dates cannot be the same.");
            }

            if (Depart < Arrive)
            {
                lstReturn.Add("Please select a departure date after the arrival date.");
            }

            if ((Depart - Arrive).Days < 1)
            {
                lstReturn.Add("Please select a arrival and departure date range that results in 1 or more nights.");
            }

            if (iSeasonId.HasValue)
            {
                var objSeason = await Season.FindAsync(iSeasonId.Value);

                // check season belongs to premise
                if (objSeason.Premise_Id != iPremiseId)
                {
                    lstReturn.Add("Selected season does not belong to selected property.");
                }

                // check the arrival date is within one of the seasons date ranges
                var bWithinSeasonDates = false;
                foreach (var vDate in objSeason.Dates)
                {
                    if (Arrive >= vDate.Start && Arrive <= vDate.End) { bWithinSeasonDates = true; break; }
                }
                if (!bWithinSeasonDates) { lstReturn.Add("Please select an arrival date within the season."); }

                objSeason = null;
            }

            return lstReturn;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(int iPremiseId, int? iSeasonId, string strBy, bool bUseConfigDefaults = false)
        {
            if (Loaded || iPremiseId <= 0) { return false; }

            Premise_Id = iPremiseId;
            Season_Id = iSeasonId;

            if ((await ValidateAsync(iPremiseId, iSeasonId)).Count > 0) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblPropertyRate
                {
                    tblProperty_id = Premise_Id,
                    tblPropertySeason_id = Season_Id,

                    tblPropertyRate_parentId = null,
                    tblPropertyRate_provisional = Provisional,
                    tblPropertyRate_reqReview = !Provisional && RequireReview,

                    tblPropertyRate_dateArrive = Arrive,
                    tblPropertyRate_dateDepart = Depart,
                    tblPropertyRate_noNights = (Depart - Arrive).Days,
                    tblPropertyRate_minPartySize = Min_PartySize,
                    tblPropertyRate_available = Available,

                    tblPropertyRate_pricePOA = Price_POA,
                    tblPropertyRate_priceEntryMode = (int)Price_EntryMode,
                    tblPropertyRate_price = Price,

                    tblPropertyRate_commissionAmountType = (int)Commission_AmountType,
                    tblPropertyRate_commissionAmount = Commission_Amount,
                    tblPropertyRate_commissionNote = Commission_Note,

                    tblPropertyRate_taxExempt = Tax_Exempt,
                    tblPropertyRate_taxValue = Tax_Value,

                    tblPropertyRate_discount = Discount,
                    tblPropertyRate_discountNights = Discount_Nights,
                    tblPropertyRate_discountEntryMode = (int)Discount_EntryMode,
                    tblPropertyRate_discountAmountType = (int)Discount_AmountType,
                    tblPropertyRate_discountAmount = Discount_Amount,
                    tblPropertyRate_discountNote = Discount_Note,

                    tblPropertyRate_extraExcludes = Utilities.General.ConvertToCommaString(SeasonExtra_ExcludeIds),

                    tblPropertyRate_createdUTC = DateTime.UtcNow,
                    tblPropertyRate_createdBy = strBy,
                    tblPropertyRate_editedUTC = DateTime.UtcNow,
                    tblPropertyRate_editedBy = strBy,
                    tblPropertyRate_deletedUTC = null,
                    tblPropertyRate_deletedBy = ""
                };

                if (bUseConfigDefaults)
                {
                    if (Season_Id.HasValue)
                    {
                        // lookup and use season config
                        var objConfig = await Config.FindBy_SeasonAsync(Season_Id.Value);
                        if (objConfig.Loaded)
                        {
                            //efmObject.tblPropertyRate_priceEntryMode = (int)objConfig.PriceEntryMode_Calculated;

                            efmObject.tblPropertyRate_commissionAmountType = (int)objConfig.Commission_AmountType_Calculated;
                            efmObject.tblPropertyRate_commissionAmount = objConfig.Commission_Amount_Calculated;
                            efmObject.tblPropertyRate_commissionNote = objConfig.Commission_Note;

                            efmObject.tblPropertyRate_taxExempt = objConfig.Tax_Exempt_Calculated;
                            efmObject.tblPropertyRate_taxValue = objConfig.Tax_Value_Calculated;
                        }
                        objConfig = null;
                    }
                    else
                    {
                        // lookup and use premise config
                        var objConfig = await Config.FindBy_PremiseAsync(Premise_Id);
                        if (objConfig.Loaded)
                        {
                            //efmObject.tblPropertyRate_priceEntryMode = (int)objConfig.PriceEntryMode_Calculated;

                            efmObject.tblPropertyRate_commissionAmountType = (int)objConfig.Commission_AmountType_Calculated;
                            efmObject.tblPropertyRate_commissionAmount = objConfig.Commission_Amount_Calculated;
                            efmObject.tblPropertyRate_commissionNote = objConfig.Commission_Note;

                            efmObject.tblPropertyRate_taxExempt = objConfig.Tax_Exempt_Calculated;
                            efmObject.tblPropertyRate_taxValue = objConfig.Tax_Value_Calculated;
                        }
                        objConfig = null;
                    }
                }

                _ = dB.tblPropertyRates.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }

            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Rate).ToString(), "CreateAsync(int, int?, string, bool)", ex,
                        "iPremiseId: " + iPremiseId.ToString() +
                        ", iSeasonId: " + (iSeasonId.HasValue ? iSeasonId.Value.ToString() : "") +
                        ", strBy: " + strBy +
                        ", bUseConfigDefaults: " + bUseConfigDefaults.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public async Task<bool> SaveAsync(string strBy)
        {
            if (!Loaded) { return false; }

            // in theory most things can be changed as long as validation is re done
            if ((await ValidateAsync(Premise_Id, Season_Id)).Count > 0) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertyRates.FirstOrDefaultAsync(r => r.tblPropertyRate_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblPropertyRate_provisional = Provisional;
                    efmObject.tblPropertyRate_reqReview = !Provisional && RequireReview;

                    efmObject.tblPropertyRate_dateArrive = Arrive;
                    efmObject.tblPropertyRate_dateDepart = Depart;
                    efmObject.tblPropertyRate_noNights = (Depart - Arrive).Days;
                    efmObject.tblPropertyRate_minPartySize = Min_PartySize;
                    efmObject.tblPropertyRate_available = Available;

                    efmObject.tblPropertyRate_pricePOA = Price_POA;
                    efmObject.tblPropertyRate_priceEntryMode = (int)Price_EntryMode;
                    efmObject.tblPropertyRate_price = Price;

                    efmObject.tblPropertyRate_commissionAmountType = (int)Commission_AmountType;
                    efmObject.tblPropertyRate_commissionAmount = Commission_Amount;
                    efmObject.tblPropertyRate_commissionNote = Commission_Note;

                    efmObject.tblPropertyRate_taxExempt = Tax_Exempt;
                    efmObject.tblPropertyRate_taxValue = Tax_Value;

                    efmObject.tblPropertyRate_discount = Discount;
                    efmObject.tblPropertyRate_discountNights = Discount_Nights;
                    efmObject.tblPropertyRate_discountEntryMode = (int)Discount_EntryMode;
                    efmObject.tblPropertyRate_discountAmountType = (int)Discount_AmountType;
                    efmObject.tblPropertyRate_discountAmount = Discount_Amount;
                    efmObject.tblPropertyRate_discountNote = Discount_Note;

                    efmObject.tblPropertyRate_extraExcludes = Utilities.General.ConvertToCommaString(SeasonExtra_ExcludeIds);

                    efmObject.tblPropertyRate_editedUTC = DateTime.UtcNow;
                    efmObject.tblPropertyRate_editedBy = strBy;

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
                _ = Error.Exception(typeof(Rate).ToString(), "SaveAsync(string)", ex,
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

                var efmObject = await dB.tblPropertyRates.FirstOrDefaultAsync(r => r.tblPropertyRate_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblPropertyRate_deletedUTC.HasValue)
                    {
                        // already in desired state
                        bReturn = true;
                    }
                    else
                    {
                        if (bDeleted)
                        {
                            efmObject.tblPropertyRate_deletedUTC = DateTime.UtcNow;
                            efmObject.tblPropertyRate_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblPropertyRate_deletedUTC = null;
                            efmObject.tblPropertyRate_deletedBy = "";
                        }

                        efmObject.tblPropertyRate_editedUTC = DateTime.UtcNow;
                        efmObject.tblPropertyRate_editedBy = strBy;

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
                _ = Error.Exception(typeof(Rate).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
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

                var efmObject = await dB.tblPropertyRates.FirstOrDefaultAsync(r => r.tblPropertyRate_id == iId);

                if (efmObject != null)
                {
                    // can only fully delete if already flagged for deletion
                    if (efmObject.tblPropertyRate_deletedUTC != null)
                    {
                        // can only delete if flagged more than short recycle bin period days ago or this is a force action
                        if (efmObject.tblPropertyRate_deletedUTC < DateTime.UtcNow.AddDays(-Settings.Variables.RecycleBin_EmptyAfterDaysShort) || bForce)
                        {
                            _ = dB.tblPropertyRates.Remove(efmObject);

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
                _ = Error.Exception(typeof(Rate).ToString(), "DeleteFullAsync(int, string, bool, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bClearCache: " + bClearCache.ToString() +
                    ", bForce: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static DataTable Export(List<Rate> lstRates)
        {
            var dtblExport = new DataTable();

            dtblExport.Columns.Add(Utilities.General.CreateColumn("Id", "ID", typeof(int)));
            //dtblExport.Columns.Add(Utilities.General.CreateColumn("PremiseId", "Property ID", typeof(int)));
            //dtblExport.Columns.Add(Utilities.General.CreateColumn("SeasonId", "Season ID", typeof(int)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("Provisional", "Provisional", typeof(bool)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("ReqReview", "Requires Review", typeof(bool)));

            dtblExport.Columns.Add(Utilities.General.CreateColumn("Arrive", "Arrive", typeof(DateTime)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("Depart", "Depart", typeof(DateTime)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("Nights", "Nights", typeof(int)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("MinParty", "Min. Party Size", typeof(int)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("Available", "Available", typeof(bool)));

            dtblExport.Columns.Add(Utilities.General.CreateColumn("PricePOA", "Price POA", typeof(bool)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("PriceEntryMode", "Price Type", typeof(string)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("Price", "Price", typeof(decimal)));

            dtblExport.Columns.Add(Utilities.General.CreateColumn("CommissionAmountType", "Commission Type", typeof(string)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("CommissionAmount", "Commission Amount", typeof(decimal)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("CommissionNote", "Commission Note", typeof(string)));

            dtblExport.Columns.Add(Utilities.General.CreateColumn("TaxExempt", "Tax Exempt", typeof(bool)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("TaxValue", "Tax Percentage", typeof(decimal)));

            dtblExport.Columns.Add(Utilities.General.CreateColumn("Discount", "Discount Available", typeof(bool)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("DiscountNights", "Discount Min. Nights", typeof(int)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("DiscountEntryMode", "Discount Applied To", typeof(string)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("DiscountAmountType", "Discount Type", typeof(string)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("DiscountAmount", "Discount Amount", typeof(decimal)));
            dtblExport.Columns.Add(Utilities.General.CreateColumn("DiscountNote", "Discount Note", typeof(string)));

            foreach (var vRate in lstRates.OrderBy(r => r.Arrive).ThenBy(r => r.Min_PartySize))
            {
                var dr = dtblExport.NewRow();

                dr["Id"] = vRate.Id;
                //dr["PremiseId"] = vRate.Premise_Id;
                //if (vRate.Season_Id.HasValue) { dr["SeasonId"] = vRate.Season_Id.Value; }
                dr["Provisional"] = vRate.Provisional;
                dr["ReqReview"] = vRate.RequireReview;

                dr["Arrive"] = vRate.Arrive;
                dr["Depart"] = vRate.Depart;
                dr["Nights"] = vRate.No_Nights;
                dr["MinParty"] = vRate.Min_PartySize;
                dr["Available"] = vRate.Available;

                dr["PricePOA"] = vRate.Price_POA;

                switch (vRate.Price_EntryMode)
                {
                    case Enums.Shared_PriceValueType.Gross:
                        dr["PriceEntryMode"] = "Gross";
                        break;
                    case Enums.Shared_PriceValueType.Net:
                        dr["PriceEntryMode"] = "Net";
                        break;
                    default: break;
                }

                dr["Price"] = vRate.Price;

                switch (vRate.Commission_AmountType)
                {
                    case Enums.Shared_NumericValueType.Percentage:
                        dr["CommissionAmountType"] = "Percentage";
                        break;
                    case Enums.Shared_NumericValueType.Fixed:
                        dr["CommissionAmountType"] = "Fixed";
                        break;
                    default: break;
                }

                dr["CommissionAmount"] = vRate.Commission_Amount;
                dr["CommissionNote"] = vRate.Commission_Note;

                dr["TaxExempt"] = vRate.Tax_Exempt;
                dr["TaxValue"] = vRate.Tax_Value;

                dr["Discount"] = vRate.Discount;
                dr["DiscountNights"] = vRate.Discount_Nights;

                switch (vRate.Discount_EntryMode)
                {
                    case Enums.Shared_PriceValueType.Gross:
                        dr["DiscountEntryMode"] = "Gross";
                        break;
                    case Enums.Shared_PriceValueType.Net:
                        dr["DiscountEntryMode"] = "Net";
                        break;
                    default: break;
                }

                switch (vRate.Discount_AmountType)
                {
                    case Enums.Shared_NumericValueType.Percentage:
                        dr["DiscountAmountType"] = "Percentage";
                        break;
                    case Enums.Shared_NumericValueType.Fixed:
                        dr["DiscountAmountType"] = "Fixed";
                        break;
                    default: break;
                }

                dr["DiscountAmount"] = vRate.Discount_Amount;
                dr["DiscountNote"] = vRate.Discount_Note;


                dtblExport.Rows.Add(dr);

                dr = null;
            }

            return dtblExport;
        }

        #endregion public functions


        #region Finders

        public static async Task<Rate> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Rate(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Rate();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Rate();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Rate>> FindAllAsync(List<int> lstIds)
        {
            var lstReturn = new List<Rate>();

            // get global list
            var lstGlobal = await Global_ListAsync();

            for (var iRow = 0; iRow < lstIds.Count; iRow++)
            {
                if (lstGlobal.TryGetValue(lstIds[iRow], out var value))
                {
                    lstReturn.Add(value);
                }
            }

            lstGlobal = null;

            return lstReturn;
        }

        public static async Task<List<Rate>> FindAllAsync(DateTime? dtArriveFrom = null,
            DateTime? dtArriveTo = null,
            DateTime? dtIncludes = null,
            DateTime? dtDepartFrom = null,
            DateTime? dtDepartTo = null,
            int? iPartySize = null,
            bool? bProvisional = null,
            bool? bReqReview = null,
            bool bIncDeleted = false,
            bool bIncPast = true)
        {
            var lstReturn = new List<Rate>();

            // get global list
            var lstGlobal = (await Global_ListAsync()).Values.ToList();

            for (var iRow = 0; iRow < lstGlobal.Count; iRow++)
            {
                var obj = lstGlobal[iRow];

                if (dtArriveFrom.HasValue) { if (obj.Arrive < dtArriveFrom.Value.Date) { continue; } }

                if (dtArriveTo.HasValue) { if (obj.Arrive > dtArriveTo.Value.Date) { continue; } }

                if (dtIncludes.HasValue) { if (obj.Arrive > dtIncludes || obj.Depart <= dtIncludes) { continue; } }

                if (dtDepartFrom.HasValue) { if (obj.Depart < dtDepartFrom.Value.Date) { continue; } }

                if (dtDepartTo.HasValue) { if (obj.Depart > dtDepartTo.Value.Date) { continue; } }

                if (iPartySize.HasValue) { if (obj.Min_PartySize > iPartySize.Value) { continue; } }

                if (bProvisional.HasValue) { if (obj.Provisional != bProvisional.Value) { continue; } }

                if (bReqReview.HasValue) { if (obj.RequireReview != bReqReview.Value) { continue; } }

                if (!bIncDeleted) { if (obj.Deleted_UTC.HasValue) { continue; } }

                if (!bIncPast) { if (obj.Depart <= DateTime.Today.Date) { continue; } }

                lstReturn.Add(obj);
            }

            lstGlobal = null;

            return lstReturn;
        }

        public static async Task<List<Rate>> FindAllBy_PremiseAsync(int iPremiseId,
            DateTime? dtArriveFrom = null,
            DateTime? dtArriveTo = null,
            DateTime? dtIncludes = null,
            DateTime? dtDepartFrom = null,
            DateTime? dtDepartTo = null,
            int? iPartySize = null,
            bool? bProvisional = null,
            bool? bReqReview = null,
            bool bIncDeleted = false,
            bool bIncPast = true)
        {
            return await FindAllBy_PremiseAsync(new List<int> { iPremiseId },
                dtArriveFrom: dtArriveFrom,
                dtArriveTo: dtArriveTo,
                dtIncludes: dtIncludes,
                dtDepartFrom: dtDepartFrom,
                dtDepartTo: dtDepartTo,
                iPartySize: iPartySize,
                bProvisional: bProvisional,
                bReqReview: bReqReview,
                bIncDeleted: bIncDeleted,
                bIncPast: bIncPast);
        }

        public static async Task<List<Rate>> FindAllBy_PremiseAsync(List<int> lstPremiseIds,
            DateTime? dtArriveFrom = null,
            DateTime? dtArriveTo = null,
            DateTime? dtIncludes = null,
            DateTime? dtDepartFrom = null,
            DateTime? dtDepartTo = null,
            int? iPartySize = null,
            bool? bProvisional = null,
            bool? bReqReview = null,
            bool bIncDeleted = false,
            bool bIncPast = true)
        {
            var lstReturn = new List<Rate>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Seasons_Rates_IdxPremise) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iPremiseId in lstPremiseIds)
                {
                    if (lstIndex.TryGetValue(iPremiseId, out var value))
                    {
                        for (var iRow = 0; iRow < value.Count; iRow++)
                        {
                            var obj = lstGlobal[value[iRow]];

                            if (dtArriveFrom.HasValue) { if (obj.Arrive < dtArriveFrom.Value.Date) { continue; } }

                            if (dtArriveTo.HasValue) { if (obj.Arrive > dtArriveTo.Value.Date) { continue; } }

                            if (dtIncludes.HasValue) { if (obj.Arrive > dtIncludes || obj.Depart <= dtIncludes) { continue; } }

                            if (dtDepartFrom.HasValue) { if (obj.Depart < dtDepartFrom.Value.Date) { continue; } }

                            if (dtDepartTo.HasValue) { if (obj.Depart > dtDepartTo.Value.Date) { continue; } }

                            if (iPartySize.HasValue) { if (obj.Min_PartySize > iPartySize.Value) { continue; } }

                            if (bProvisional.HasValue) { if (obj.Provisional != bProvisional.Value) { continue; } }

                            if (bReqReview.HasValue) { if (obj.RequireReview != bReqReview.Value) { continue; } }

                            if (!bIncDeleted) { if (obj.Deleted_UTC.HasValue) { continue; } }

                            if (!bIncPast) { if (obj.Depart <= DateTime.Today.Date) { continue; } }

                            lstReturn.Add(obj);
                        }
                    }
                }
            }

            lstGlobal = null;

            lstReturn = lstReturn.OrderBy(r => r.Arrive).ThenBy(r => r.Min_PartySize).ToList();

            return lstReturn;
        }

        public static async Task<List<Rate>> FindAllBy_SeasonAsync(int iSeasonId,
            DateTime? dtArriveFrom = null,
            DateTime? dtArriveTo = null,
            DateTime? dtIncludes = null,
            DateTime? dtDepartFrom = null,
            DateTime? dtDepartTo = null,
            int? iPartySize = null,
            bool? bProvisional = null,
            bool? bReqReview = null,
            bool bIncDeleted = false,
            bool bIncPast = true)
        {
            return await FindAllBy_SeasonAsync(new List<int> { iSeasonId },
                dtArriveFrom: dtArriveFrom,
                dtArriveTo: dtArriveTo,
                dtIncludes: dtIncludes,
                dtDepartFrom: dtDepartFrom,
                dtDepartTo: dtDepartTo,
                iPartySize: iPartySize,
                bProvisional: bProvisional,
                bReqReview: bReqReview,
                bIncDeleted: bIncDeleted,
                bIncPast: bIncPast);
        }

        public static async Task<List<Rate>> FindAllBy_SeasonAsync(List<int> lstSeasonIds,
            DateTime? dtArriveFrom = null,
            DateTime? dtArriveTo = null,
            DateTime? dtIncludes = null,
            DateTime? dtDepartFrom = null,
            DateTime? dtDepartTo = null,
            int? iPartySize = null,
            bool? bProvisional = null,
            bool? bReqReview = null,
            bool bIncDeleted = false,
            bool bIncPast = true)
        {
            var lstReturn = new List<Rate>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Seasons_Rates_IdxSeason) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iSeasonId in lstSeasonIds)
                {
                    if (lstIndex.TryGetValue(iSeasonId, out var value))
                    {
                        for (var iRow = 0; iRow < value.Count; iRow++)
                        {
                            var obj = lstGlobal[value[iRow]];

                            if (dtArriveFrom.HasValue) { if (obj.Arrive < dtArriveFrom.Value.Date) { continue; } }

                            if (dtArriveTo.HasValue) { if (obj.Arrive > dtArriveTo.Value.Date) { continue; } }

                            if (dtIncludes.HasValue) { if (obj.Arrive > dtIncludes || obj.Depart <= dtIncludes) { continue; } }

                            if (dtDepartFrom.HasValue) { if (obj.Depart < dtDepartFrom.Value.Date) { continue; } }

                            if (dtDepartTo.HasValue) { if (obj.Depart > dtDepartTo.Value.Date) { continue; } }

                            if (iPartySize.HasValue) { if (obj.Min_PartySize > iPartySize.Value) { continue; } }

                            if (bProvisional.HasValue) { if (obj.Provisional != bProvisional.Value) { continue; } }

                            if (bReqReview.HasValue) { if (obj.RequireReview != bReqReview.Value) { continue; } }

                            if (!bIncDeleted) { if (obj.Deleted_UTC.HasValue) { continue; } }

                            if (!bIncPast) { if (obj.Depart <= DateTime.Today.Date) { continue; } }

                            lstReturn.Add(obj);
                        }
                    }
                }
            }

            lstGlobal = null;

            lstReturn = lstReturn.OrderBy(r => r.Arrive).ThenBy(r => r.Min_PartySize).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Rate>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Rate> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Seasons_Rates_IdxPremise) is not Dictionary<int, List<int>> dicIndexPremise ||
                LocalCache.Get(LocalCache.Key.Premises_Seasons_Rates_IdxSeason) is not Dictionary<int, List<int>> dicIndexSeason)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, Rate>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    await s_singleCacheBuildLock.WaitAsync();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Rate> lstElementsRetry)
                    {
                        // cache now populated, no need to build
                        return lstElementsRetry;
                    }

                    // setup index dictionaries
                    dicIndexPremise = new Dictionary<int, List<int>>();
                    dicIndexSeason = new Dictionary<int, List<int>>();

                    // create instance of DB
                    using var dB = Settings.Config.DBPooledConnection();

                    // disable change tracking as only reading
                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    var lstRates = await dB.tblPropertyRates.AsNoTracking().ToListAsync();

                    // get all the elements
                    for (var iRow = 0; iRow < lstRates.Count; iRow++)
                    {
                        var obj = new Rate(lstRates[iRow]);

                        if (obj.Loaded)
                        {
                            // successfully populated/loaded info, add to global list using id as key
                            lstElements.Add(obj.Id, obj);

                            // add to premises index
                            if (!dicIndexPremise.ContainsKey(obj.Premise_Id))
                            {
                                dicIndexPremise.Add(obj.Premise_Id, new List<int>());
                            }
                            dicIndexPremise[obj.Premise_Id].Add(obj.Id);

                            // add to seasons index
                            if (obj.Season_Id.HasValue)
                            {
                                if (!dicIndexSeason.ContainsKey(obj.Season_Id.Value))
                                {
                                    dicIndexSeason.Add(obj.Season_Id.Value, new List<int>());
                                }
                                dicIndexSeason[obj.Season_Id.Value].Add(obj.Id);
                            }
                        }

                        obj = null;
                    }

                    // add the found elements to the cache
                    LocalCache.Set(CacheKey, lstElements);

                    // add indexes to cache
                    LocalCache.Set(LocalCache.Key.Premises_Seasons_Rates_IdxPremise, dicIndexPremise);
                    LocalCache.Set(LocalCache.Key.Premises_Seasons_Rates_IdxSeason, dicIndexSeason);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Rate).ToString(), "Global_ListAsync()", ex);
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
