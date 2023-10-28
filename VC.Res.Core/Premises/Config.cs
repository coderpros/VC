using System.Collections.Concurrent;
using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Premises
{
    public class Config : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Configs;

        #region Properties

        public int? Parent_Id { get; private set; } = null;

        private Config Parent
        {
            get
            {
                if (Parent_Id.HasValue) { return Find(Parent_Id.Value); }
                return new Config();
            }
        }

        public int? Contact_Id { get; private set; } = null;

        public int? Premise_Id { get; private set; } = null;

        public int? Season_Id { get; private set; } = null;

        public bool DescriptionForQuote_Inherit { get; set; } = true;
        public string DescriptionForQuote { get; set; } = ""; // Property description for quoting
        public string DescriptionForQuote_Calculated
        {
            get
            {
                if (DescriptionForQuote_Inherit && Parent_Id.HasValue) { return Parent.DescriptionForQuote_Calculated; }
                return DescriptionForQuote;
            }
        }

        public bool HouseRules_Inherit { get; set; } = true;
        public string HouseRules { get; set; } = "";
        public string HouseRules_Calculated
        {
            get
            {
                if (HouseRules_Inherit && Parent_Id.HasValue) { return Parent.HouseRules_Calculated; }
                return HouseRules;
            }
        }

        public bool Inclusions_Inherit { get; set; } = true;
        public string Inclusions { get; set; } = "";
        public string Inclusions_Calculated
        {
            get
            {
                if (Inclusions_Inherit && Parent_Id.HasValue) { return Parent.Inclusions_Calculated; }
                return Inclusions;
            }
        }

        public bool DefaultAvailability_Inherit { get; set; } = true;
        public Enums.Premises_Premise_Availability DefaultAvailability { get; set; } = Enums.Premises_Premise_Availability.Available;
        public Enums.Premises_Premise_Availability DefaultAvailability_Calculated
        {
            get
            {
                if (DefaultAvailability_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.DefaultAvailability_Calculated; }

                    return Settings.PremiseDefaults.Fetch.DefaultAvailability;
                }
                return DefaultAvailability;
            }
        }

        public bool RequireBookingApproval_Inherit { get; set; } = true;
        public bool RequireBookingApproval { get; set; } = false;
        public bool RequireBookingApproval_Calculated
        {
            get
            {
                if (RequireBookingApproval_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.RequireBookingApproval_Calculated; }

                    return Settings.PremiseDefaults.Fetch.RequireBookingApproval;
                }
                return RequireBookingApproval;
            }
        }

        public bool PriceEntryMode_Inherit { get; set; } = true;
        public Enums.Shared_PriceValueType PriceEntryMode { get; set; } = Enums.Shared_PriceValueType.Net;
        public Enums.Shared_PriceValueType PriceEntryMode_Calculated
        {
            get
            {
                if (PriceEntryMode_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.PriceEntryMode_Calculated; }

                    return Settings.PremiseDefaults.Fetch.PriceEntryMode;
                }
                return PriceEntryMode;
            }
        }

        public bool Currency_Id_Inherit { get; set; } = true;
        public int? Currency_Id { get; set; } = null;
        public int Currency_Id_Calculated
        {
            get
            {
                if (Currency_Id_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.Currency_Id_Calculated; }

                    return Settings.PremiseDefaults.Fetch.Currency_Id;
                }
                return Currency_Id ?? Settings.PremiseDefaults.Fetch.Currency_Id;
            }
        }

        public bool Tax_Inherit { get; set; } = true;
        public bool Tax_Exempt { get; set; } = false;
        public bool Tax_Exempt_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Tax_Inherit) { return Parent.Tax_Exempt_Calculated; }
                return Tax_Exempt;
            }
        }
        public decimal? Tax_Value { get; set; } = null;
        public decimal Tax_Value_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Tax_Inherit) { return Parent.Tax_Value_Calculated; }
                return Tax_Value ?? 0;
            }
        }
        public string Tax_Number { get; set; } = "";
        public string Tax_Number_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Tax_Inherit) { return Parent.Tax_Number_Calculated; }
                return Tax_Number;
            }
        }

        public bool Bank_Inherit { get; set; } = true;

        public string Bank_AccountName { get; set; } = "";
        public string Bank_AccountName_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_AccountName_Calculated; }
                return Bank_AccountName;
            }
        }

        public string Bank_AccountNo { get; set; } = "";
        public string Bank_AccountNo_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_AccountNo_Calculated; }
                return Bank_AccountNo;
            }
        }

        public string Bank_AccountSort { get; set; } = "";
        public string Bank_AccountSort_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_AccountSort_Calculated; }
                return Bank_AccountSort;
            }
        }

        public string Bank_AccountIBAN { get; set; } = "";
        public string Bank_AccountIBAN_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_AccountIBAN_Calculated; }
                return Bank_AccountIBAN;
            }
        }

        public string Bank_AccountBIC { get; set; } = "";
        public string Bank_AccountBIC_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_AccountBIC_Calculated; }
                return Bank_AccountBIC;
            }
        }

        public string Bank_Address1 { get; set; } = "";
        public string Bank_Address1_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_Address1_Calculated; }
                return Bank_Address1;
            }
        }

        public string Bank_Address2 { get; set; } = "";
        public string Bank_Address2_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_Address2_Calculated; }
                return Bank_Address2;
            }
        }

        public string Bank_Address3 { get; set; } = "";
        public string Bank_Address3_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_Address3_Calculated; }
                return Bank_Address3;
            }
        }

        public string Bank_AddressTown { get; set; } = "";
        public string Bank_AddressTown_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_AddressTown; }
                return Bank_AddressTown;
            }
        }

        public string Bank_AddressCounty { get; set; } = "";
        public string Bank_AddressCounty_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_AddressCounty_Calculated; }
                return Bank_AddressCounty;
            }
        }

        public string Bank_AddressPostcode { get; set; } = "";
        public string Bank_AddressPostcode_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_AddressPostcode_Calculated; }
                return Bank_AddressPostcode;
            }
        }

        public string Bank_AddressCountry { get; set; } = "";
        public string Bank_AddressCountry_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Bank_Inherit) { return Parent.Bank_AddressCountry_Calculated; }
                return Bank_AddressCountry;
            }
        }

        public bool Commission_Inherit { get; set; } = true;
        public Enums.Shared_NumericValueType Commission_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public Enums.Shared_NumericValueType Commission_AmountType_Calculated
        {
            get
            {
                if (Commission_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.Commission_AmountType_Calculated; }

                    return Settings.PremiseDefaults.Fetch.Commission_AmountType;
                }
                return Commission_AmountType;
            }
        }
        public decimal? Commission_Amount { get; set; } = null;
        public decimal Commission_Amount_Calculated
        {
            get
            {
                if (Commission_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.Commission_Amount_Calculated; }

                    return Settings.PremiseDefaults.Fetch.Commission_Amount;
                }
                return Commission_Amount ?? Settings.PremiseDefaults.Fetch.Commission_Amount;
            }
        }
        public string Commission_Note { get; set; } = "";
        public string Commission_Note_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && Commission_Inherit) { return Parent.Commission_Note_Calculated; }
                return Commission_Note;
            }
        }

        public bool Checkin_Inherit { get; set; } = true;
        public TimeSpan? Checkin { get; set; } = null;
        public TimeSpan Checkin_Calculated
        {
            get
            {
                if (Checkin_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.Checkin_Calculated; }

                    return Settings.PremiseDefaults.Fetch.Checkin;
                }
                return Checkin ?? Settings.PremiseDefaults.Fetch.Checkin;
            }
        }

        public bool Checkout_Inherit { get; set; } = true;
        public TimeSpan? Checkout { get; set; } = null;
        public TimeSpan Checkout_Calculated
        {
            get
            {
                if (Checkout_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.Checkout_Calculated; }

                    return Settings.PremiseDefaults.Fetch.Checkout;
                }
                return Checkout ?? Settings.PremiseDefaults.Fetch.Checkout;
            }
        }

        public bool ChangeoverDay_Inherit { get; set; } = true;
        public DayOfWeek? ChangeoverDay { get; set; } = null; // null value represents open/flexible changeover day
        public DayOfWeek? ChangeoverDay_Calculated
        {
            get
            {
                if (ChangeoverDay_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.ChangeoverDay_Calculated; }

                    return Settings.PremiseDefaults.Fetch.ChangeoverDay;
                }
                return ChangeoverDay;
            }
        }

        public bool MinRental_Inherit { get; set; } = true;
        public int? MinRental_Days { get; set; } = null;
        public int MinRental_Days_Calculated
        {
            get
            {
                if (MinRental_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.MinRental_Days_Calculated; }

                    return Settings.PremiseDefaults.Fetch.MinRental_Days;
                }
                return MinRental_Days ?? Settings.PremiseDefaults.Fetch.MinRental_Days;
            }
        }
        public string MinRental_Note { get; set; } = "";
        public string MinRental_Note_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && MinRental_Inherit) { return Parent.MinRental_Note_Calculated; }
                return MinRental_Note;
            }
        }

        public bool NightlyPrice_Inherit { get; set; } = true;
        public decimal? NightlyPrice { get; set; } = null;
        public decimal? NightlyPrice_Calculated
        {
            get
            {
                if (Parent_Id.HasValue && NightlyPrice_Inherit) { return Parent.NightlyPrice_Calculated; }
                return NightlyPrice;
            }
        }

        public bool PaySchedule_Inherit { get; set; } = true;
        public bool PaySchedule_Deposit_Required { get; set; } = true;
        public bool PaySchedule_Deposit_Required_Calculated
        {
            get
            {
                if (PaySchedule_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.PaySchedule_Deposit_Required_Calculated; }

                    return Settings.PremiseDefaults.Fetch.PaySchedule_Deposit_Required;
                }
                return PaySchedule_Deposit_Required;
            }
        }
        public Enums.Shared_NumericValueType PaySchedule_Deposit_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public Enums.Shared_NumericValueType PaySchedule_Deposit_AmountType_Calculated
        {
            get
            {
                if (PaySchedule_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.PaySchedule_Deposit_AmountType_Calculated; }

                    return Settings.PremiseDefaults.Fetch.PaySchedule_Deposit_AmountType;
                }
                return PaySchedule_Deposit_AmountType;
            }
        }
        public decimal? PaySchedule_Deposit_Amount { get; set; } = null;
        public decimal PaySchedule_Deposit_Amount_Calculated
        {
            get
            {
                if (PaySchedule_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.PaySchedule_Deposit_Amount_Calculated; }

                    return Settings.PremiseDefaults.Fetch.PaySchedule_Deposit_Amount;
                }
                return PaySchedule_Deposit_Amount ?? Settings.PremiseDefaults.Fetch.PaySchedule_Deposit_Amount;
            }
        }

        public bool PaySchedule_Interim_Required { get; set; } = false;
        public bool PaySchedule_Interim_Required_Calculated
        {
            get
            {
                if (PaySchedule_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.PaySchedule_Interim_Required_Calculated; }

                    return Settings.PremiseDefaults.Fetch.PaySchedule_Interim_Required;
                }
                return PaySchedule_Interim_Required;
            }
        }
        public Enums.Shared_NumericValueType PaySchedule_Interim_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public Enums.Shared_NumericValueType PaySchedule_Interim_AmountType_Calculated
        {
            get
            {
                if (PaySchedule_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.PaySchedule_Interim_AmountType_Calculated; }

                    return Settings.PremiseDefaults.Fetch.PaySchedule_Interim_AmountType;
                }
                return PaySchedule_Interim_AmountType;
            }
        }
        public decimal? PaySchedule_Interim_Amount { get; set; } = null;
        public decimal PaySchedule_Interim_Amount_Calculated
        {
            get
            {
                if (PaySchedule_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.PaySchedule_Interim_Amount_Calculated; }

                    return Settings.PremiseDefaults.Fetch.PaySchedule_Interim_Amount;
                }
                return PaySchedule_Interim_Amount ?? Settings.PremiseDefaults.Fetch.PaySchedule_Interim_Amount;
            }
        }
        public int? PaySchedule_Interim_Days { get; set; } = null; // Number of days before arrival the interim is expected.
        public int PaySchedule_Interim_Days_Calculated
        {
            get
            {
                if (PaySchedule_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.PaySchedule_Interim_Days_Calculated; }

                    return Settings.PremiseDefaults.Fetch.PaySchedule_Interim_Days;
                }
                return PaySchedule_Interim_Days ?? Settings.PremiseDefaults.Fetch.PaySchedule_Interim_Days;
            }
        }
        public int? PaySchedule_Balance_Days { get; set; } = null; // Number of days before arrival the balance is expected.
        public int PaySchedule_Balance_Days_Calculated
        {
            get
            {
                if (PaySchedule_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.PaySchedule_Balance_Days_Calculated; }

                    return Settings.PremiseDefaults.Fetch.PaySchedule_Balance_Days;
                }
                return PaySchedule_Balance_Days ?? Settings.PremiseDefaults.Fetch.PaySchedule_Balance_Days;
            }
        }

        public bool SecurityDeposit_Inherit { get; set; } = true;
        public bool SecurityDeposit_Required { get; set; } = false;
        public bool SecurityDeposit_Required_Calculated
        {
            get
            {
                if (SecurityDeposit_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.SecurityDeposit_Required_Calculated; }

                    return Settings.PremiseDefaults.Fetch.SecurityDeposit_Required;
                }
                return SecurityDeposit_Required;
            }
        }
        public Enums.Shared_NumericValueType SecurityDeposit_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public Enums.Shared_NumericValueType SecurityDeposit_AmountType_Calculated
        {
            get
            {
                if (SecurityDeposit_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.SecurityDeposit_AmountType_Calculated; }

                    return Settings.PremiseDefaults.Fetch.SecurityDeposit_AmountType;
                }
                return SecurityDeposit_AmountType;
            }
        }
        public decimal? SecurityDeposit_Amount { get; set; } = null;
        public decimal SecurityDeposit_Amount_Calculated
        {
            get
            {
                if (SecurityDeposit_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.SecurityDeposit_Amount_Calculated; }

                    return Settings.PremiseDefaults.Fetch.SecurityDeposit_Amount;
                }
                return SecurityDeposit_Amount ?? Settings.PremiseDefaults.Fetch.SecurityDeposit_Amount;
            }
        }
        public Enums.Shared_PriceValueType SecurityDeposit_CalcFrom { get; set; } = Enums.Shared_PriceValueType.Gross;
        public Enums.Shared_PriceValueType SecurityDeposit_CalcFrom_Calculated
        {
            get
            {
                if (SecurityDeposit_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.SecurityDeposit_CalcFrom_Calculated; }

                    return Settings.PremiseDefaults.Fetch.SecurityDeposit_CalcFrom;
                }
                return SecurityDeposit_CalcFrom;
            }
        }
        public int? SecurityDeposit_DaysBeforeDue { get; set; } = null; // number of days before arrival the security deposit is expected
        public int SecurityDeposit_DaysBeforeDue_Calculated
        {
            get
            {
                if (SecurityDeposit_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.SecurityDeposit_DaysBeforeDue_Calculated; }

                    return Settings.PremiseDefaults.Fetch.SecurityDeposit_DaysBeforeDue;
                }
                return SecurityDeposit_DaysBeforeDue ?? Settings.PremiseDefaults.Fetch.SecurityDeposit_DaysBeforeDue;
            }
        }
        public int? SecurityDeposit_DaysAfterDue { get; set; } = null; // number of days after departure the security deposit is expected to be returned
        public int SecurityDeposit_DaysAfterDue_Calculated
        {
            get
            {
                if (SecurityDeposit_Inherit)
                {
                    if (Parent_Id.HasValue) { return Parent.SecurityDeposit_DaysAfterDue_Calculated; }

                    return Settings.PremiseDefaults.Fetch.SecurityDeposit_DaysAfterDue;
                }
                return SecurityDeposit_DaysAfterDue ?? Settings.PremiseDefaults.Fetch.SecurityDeposit_DaysAfterDue;
            }
        }

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string Edited_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Config() { }

        private Config(tblPropertyConfig efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private bool Load(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = dB.tblPropertyConfigs.AsNoTracking().FirstOrDefault(r => r.tblPropertyConfig_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Config).ToString(), "Load(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        protected override async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblPropertyConfigs.AsNoTracking().FirstOrDefaultAsync(r => r.tblPropertyConfig_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
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

        private bool Load(tblPropertyConfig efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertyConfig_id;
                    Parent_Id = efmObject.tblPropertyConfig_parentId;

                    Contact_Id = efmObject.tblContact_id;
                    Premise_Id = efmObject.tblProperty_id;
                    Season_Id = efmObject.tblPropertySeason_id;

                    DescriptionForQuote_Inherit = efmObject.tblPropertyConfig_descForQuoteInh;
                    DescriptionForQuote = string.IsNullOrWhiteSpace(efmObject.tblPropertyConfig_descForQuote) ? "" : efmObject.tblPropertyConfig_descForQuote;

                    HouseRules_Inherit = efmObject.tblPropertyConfig_houseRulesInh;
                    HouseRules = string.IsNullOrWhiteSpace(efmObject.tblPropertyConfig_houseRules) ? "" : efmObject.tblPropertyConfig_houseRules;

                    Inclusions_Inherit = efmObject.tblPropertyConfig_inclusionsInh;
                    Inclusions = string.IsNullOrWhiteSpace(efmObject.tblPropertyConfig_inclusions) ? "" : efmObject.tblPropertyConfig_inclusions;

                    DefaultAvailability_Inherit = efmObject.tblPropertyConfig_defAvailStateInh;
                    DefaultAvailability = (Enums.Premises_Premise_Availability)efmObject.tblPropertyConfig_defAvailState;

                    RequireBookingApproval_Inherit = efmObject.tblPropertyConfig_reqBookingApvlInh;
                    RequireBookingApproval = efmObject.tblPropertyConfig_reqBookingApvl;

                    PriceEntryMode_Inherit = efmObject.tblPropertyConfig_priceEntryModeInh;
                    PriceEntryMode = (Enums.Shared_PriceValueType)efmObject.tblPropertyConfig_priceEntryMode;

                    Currency_Id_Inherit = efmObject.tblPropertyConfig_currencyInh;
                    Currency_Id = efmObject.tblCurrency_id;

                    Tax_Inherit = efmObject.tblPropertyConfig_taxInh;
                    Tax_Exempt = efmObject.tblPropertyConfig_taxExempt;
                    Tax_Value = efmObject.tblPropertyConfig_taxValue;
                    Tax_Number = string.IsNullOrWhiteSpace(efmObject.tblPropertyConfig_taxNo) ? "" : efmObject.tblPropertyConfig_taxNo;

                    Bank_Inherit = efmObject.tblPropertyConfig_bankInh;
                    Bank_AccountName = efmObject.tblPropertyConfig_bankAccName;
                    Bank_AccountNo = efmObject.tblPropertyConfig_bankAccNo;
                    Bank_AccountSort = efmObject.tblPropertyConfig_bankAccSort;
                    Bank_AccountIBAN = efmObject.tblPropertyConfig_bankAccIBAN;
                    Bank_AccountBIC = efmObject.tblPropertyConfig_bankAccBIC;
                    Bank_Address1 = efmObject.tblPropertyConfig_bankAddress1;
                    Bank_Address2 = efmObject.tblPropertyConfig_bankAddress2;
                    Bank_Address3 = efmObject.tblPropertyConfig_bankAddress3;
                    Bank_AddressTown = efmObject.tblPropertyConfig_bankAddressTown;
                    Bank_AddressCounty = efmObject.tblPropertyConfig_bankAddressCounty;
                    Bank_AddressPostcode = efmObject.tblPropertyConfig_bankAddressPost;
                    Bank_AddressCountry = efmObject.tblPropertyConfig_bankAddressCountry;

                    Commission_Inherit = efmObject.tblPropertyConfig_commissionInh;
                    Commission_AmountType = (Enums.Shared_NumericValueType)efmObject.tblPropertyConfig_commissionAmountType;
                    Commission_Amount = efmObject.tblPropertyConfig_commissionAmount;
                    Commission_Note = efmObject.tblPropertyConfig_commissionNote;

                    Checkin_Inherit = efmObject.tblPropertyConfig_checkinInh;
                    Checkin = efmObject.tblPropertyConfig_checkin;

                    Checkout_Inherit = efmObject.tblPropertyConfig_checkoutInh;
                    Checkout = efmObject.tblPropertyConfig_checkout;

                    ChangeoverDay_Inherit = efmObject.tblPropertyConfig_changeOverDayInh;
                    if (efmObject.tblPropertyConfig_changeOverDay.HasValue)
                    {
                        ChangeoverDay = (DayOfWeek)efmObject.tblPropertyConfig_changeOverDay.Value;
                    }

                    MinRental_Inherit = efmObject.tblPropertyConfig_minRentalInh;
                    MinRental_Days = efmObject.tblPropertyConfig_minRentalDays;
                    MinRental_Note = efmObject.tblPropertyConfig_minRentalNote;

                    NightlyPrice_Inherit = efmObject.tblPropertyConfig_nightlyPriceInh;
                    NightlyPrice = efmObject.tblPropertyConfig_nightlyPrice;

                    PaySchedule_Inherit = efmObject.tblPropertyConfig_paySchInh;
                    PaySchedule_Deposit_Required = efmObject.tblPropertyConfig_paySchDeposReq;
                    PaySchedule_Deposit_AmountType = (Enums.Shared_NumericValueType)efmObject.tblPropertyConfig_paySchDeposAmountType;
                    PaySchedule_Deposit_Amount = efmObject.tblPropertyConfig_paySchDeposAmount;
                    PaySchedule_Interim_Required = efmObject.tblPropertyConfig_paySchInterReq;
                    PaySchedule_Interim_AmountType = (Enums.Shared_NumericValueType)efmObject.tblPropertyConfig_paySchInterAmountType;
                    PaySchedule_Interim_Amount = efmObject.tblPropertyConfig_paySchInterAmount;
                    PaySchedule_Interim_Days = efmObject.tblPropertyConfig_paySchInterDays;
                    PaySchedule_Balance_Days = efmObject.tblPropertyConfig_paySchBalDays;

                    SecurityDeposit_Inherit = efmObject.tblPropertyConfig_paySecDeposInh;
                    SecurityDeposit_Required = efmObject.tblPropertyConfig_paySecDeposReq;
                    SecurityDeposit_AmountType = (Enums.Shared_NumericValueType)efmObject.tblPropertyConfig_paySecDeposAmountType;
                    SecurityDeposit_Amount = efmObject.tblPropertyConfig_paySecDeposAmount;
                    SecurityDeposit_CalcFrom = (Enums.Shared_PriceValueType)efmObject.tblPropertyConfig_paySecDeposCalcFrom;
                    SecurityDeposit_DaysBeforeDue = efmObject.tblPropertyConfig_paySecDeposDaysBefore;
                    SecurityDeposit_DaysAfterDue = efmObject.tblPropertyConfig_paySecDeposDaysAfter;

                    Created_UTC = efmObject.tblPropertyConfig_createdUTC;
                    Created_By = efmObject.tblPropertyConfig_createdBy;

                    Edited_UTC = efmObject.tblPropertyConfig_editedUTC;
                    Edited_By = efmObject.tblPropertyConfig_editedBy;

                    // ensure inherit isn't set on certain fields
                    if (Contact_Id.HasValue)
                    {
                        HouseRules_Inherit = false;
                        Inclusions_Inherit = false;

                        Tax_Inherit = false;
                        Bank_Inherit = false;
                    }

                    if (Premise_Id.HasValue)
                    {
                        DescriptionForQuote_Inherit = false;
                        NightlyPrice_Inherit = false;

                        if (!Parent_Id.HasValue)
                        {
                            HouseRules_Inherit = false;
                            Inclusions_Inherit = false;

                            Tax_Inherit = false;
                            Bank_Inherit = false;
                        }
                    }

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblPropertyConfig)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private functions-loaders

        private static async Task<Config> CreateAsync(int? iContactId, int? iPremiseId, int? iSeasonId, string strBy)
        {
            var objReturn = new Config();

            // setup the element the config will be linked to, only one will be used
            var iValuesSet = 0;

            if (iContactId.HasValue) { iValuesSet++; }
            if (iPremiseId.HasValue) { iValuesSet++; }
            if (iSeasonId.HasValue) { iValuesSet++; }

            // value not set or too many uset
            if (iValuesSet != 1) { return objReturn; }

            try
            {
                using var dB = Settings.Config.DBConnection();

                // check if already exists
                var efmExisting = await dB.tblPropertyConfigs.FirstOrDefaultAsync(r => r.tblContact_id == iContactId && r.tblProperty_id == iPremiseId && r.tblPropertySeason_id == iSeasonId);

                if (efmExisting != null)
                {
                    // already exists, load
                    _ = objReturn.Load(efmExisting);
                }
                else
                {
                    // need to create/setup
                    // use the defaults from the existing blank return object
                    var efmObject = new tblPropertyConfig
                    {
                        tblPropertyConfig_parentId = null,
                        tblContact_id = iContactId,
                        tblProperty_id = iPremiseId,
                        tblPropertySeason_id = iSeasonId,
                        tblPropertyConfig_descForQuoteInh = objReturn.DescriptionForQuote_Inherit,
                        tblPropertyConfig_descForQuote = objReturn.DescriptionForQuote,
                        tblPropertyConfig_houseRulesInh = objReturn.HouseRules_Inherit,
                        tblPropertyConfig_houseRules = objReturn.HouseRules,
                        tblPropertyConfig_inclusionsInh = objReturn.Inclusions_Inherit,
                        tblPropertyConfig_inclusions = objReturn.Inclusions,
                        tblPropertyConfig_defAvailStateInh = objReturn.DefaultAvailability_Inherit,
                        tblPropertyConfig_defAvailState = (int)objReturn.DefaultAvailability,
                        tblPropertyConfig_reqBookingApvlInh = objReturn.RequireBookingApproval_Inherit,
                        tblPropertyConfig_reqBookingApvl = objReturn.RequireBookingApproval,
                        tblPropertyConfig_priceEntryModeInh = objReturn.PriceEntryMode_Inherit,
                        tblPropertyConfig_priceEntryMode = (int)objReturn.PriceEntryMode,
                        tblPropertyConfig_currencyInh = objReturn.Currency_Id_Inherit,
                        tblCurrency_id = objReturn.Currency_Id,
                        tblPropertyConfig_taxInh = objReturn.Tax_Inherit,
                        tblPropertyConfig_taxExempt = objReturn.Tax_Exempt,
                        tblPropertyConfig_taxValue = objReturn.Tax_Value,
                        tblPropertyConfig_taxNo = objReturn.Tax_Number,
                        tblPropertyConfig_bankInh = objReturn.Bank_Inherit,
                        tblPropertyConfig_bankAccName = objReturn.Bank_AccountName,
                        tblPropertyConfig_bankAccNo = objReturn.Bank_AccountNo,
                        tblPropertyConfig_bankAccSort = objReturn.Bank_AccountSort,
                        tblPropertyConfig_bankAccIBAN = objReturn.Bank_AccountIBAN,
                        tblPropertyConfig_bankAccBIC = objReturn.Bank_AccountBIC,
                        tblPropertyConfig_bankAddress1 = objReturn.Bank_Address1,
                        tblPropertyConfig_bankAddress2 = objReturn.Bank_Address2,
                        tblPropertyConfig_bankAddress3 = objReturn.Bank_Address3,
                        tblPropertyConfig_bankAddressTown = objReturn.Bank_AddressTown,
                        tblPropertyConfig_bankAddressCounty = objReturn.Bank_AddressCounty,
                        tblPropertyConfig_bankAddressPost = objReturn.Bank_AddressPostcode,
                        tblPropertyConfig_bankAddressCountry = objReturn.Bank_AddressCountry,
                        tblPropertyConfig_commissionInh = objReturn.Commission_Inherit,
                        tblPropertyConfig_commissionAmountType = (int)objReturn.Commission_AmountType,
                        tblPropertyConfig_commissionAmount = objReturn.Commission_Amount,
                        tblPropertyConfig_commissionNote = objReturn.Commission_Note,
                        tblPropertyConfig_checkinInh = objReturn.Checkin_Inherit,
                        tblPropertyConfig_checkin = objReturn.Checkin,
                        tblPropertyConfig_checkoutInh = objReturn.Checkout_Inherit,
                        tblPropertyConfig_checkout = objReturn.Checkout,
                        tblPropertyConfig_changeOverDayInh = objReturn.ChangeoverDay_Inherit,
                        tblPropertyConfig_changeOverDay = null,
                        tblPropertyConfig_minRentalInh = objReturn.MinRental_Inherit,
                        tblPropertyConfig_minRentalDays = objReturn.MinRental_Days,
                        tblPropertyConfig_minRentalNote = objReturn.MinRental_Note,
                        tblPropertyConfig_nightlyPriceInh = objReturn.NightlyPrice_Inherit,
                        tblPropertyConfig_nightlyPrice = objReturn.NightlyPrice,
                        tblPropertyConfig_paySchInh = objReturn.PaySchedule_Inherit,
                        tblPropertyConfig_paySchDeposReq = objReturn.PaySchedule_Deposit_Required,
                        tblPropertyConfig_paySchDeposAmountType = (int)objReturn.PaySchedule_Deposit_AmountType,
                        tblPropertyConfig_paySchDeposAmount = objReturn.PaySchedule_Deposit_Amount,
                        tblPropertyConfig_paySchInterReq = objReturn.PaySchedule_Interim_Required,
                        tblPropertyConfig_paySchInterAmountType = (int)objReturn.PaySchedule_Interim_AmountType,
                        tblPropertyConfig_paySchInterAmount = objReturn.PaySchedule_Interim_Amount,
                        tblPropertyConfig_paySchInterDays = objReturn.PaySchedule_Interim_Days,
                        tblPropertyConfig_paySchBalDays = objReturn.PaySchedule_Balance_Days,
                        tblPropertyConfig_paySecDeposInh = objReturn.SecurityDeposit_Inherit,
                        tblPropertyConfig_paySecDeposReq = objReturn.SecurityDeposit_Required,
                        tblPropertyConfig_paySecDeposAmountType = (int)objReturn.SecurityDeposit_AmountType,
                        tblPropertyConfig_paySecDeposAmount = objReturn.SecurityDeposit_Amount,
                        tblPropertyConfig_paySecDeposCalcFrom = (int)objReturn.SecurityDeposit_CalcFrom,
                        tblPropertyConfig_paySecDeposDaysBefore = objReturn.SecurityDeposit_DaysBeforeDue,
                        tblPropertyConfig_paySecDeposDaysAfter = objReturn.SecurityDeposit_DaysAfterDue,
                        tblPropertyConfig_createdUTC = DateTime.UtcNow,
                        tblPropertyConfig_createdBy = strBy,
                        tblPropertyConfig_editedUTC = DateTime.UtcNow,
                        tblPropertyConfig_editedBy = strBy
                    };

                    if (objReturn.ChangeoverDay.HasValue)
                    {
                        efmObject.tblPropertyConfig_changeOverDay = (int)objReturn.ChangeoverDay.Value;
                    }

                    if (iContactId.HasValue)
                    {
                        // certain fields cannot be inherited when a contact, as they are top of the stack
                        efmObject.tblPropertyConfig_houseRulesInh = false;
                        efmObject.tblPropertyConfig_inclusionsInh = false;

                        efmObject.tblPropertyConfig_taxInh = false;

                        efmObject.tblPropertyConfig_bankInh = false;
                    }

                    if (iPremiseId.HasValue)
                    {
                        // certain fields cannot be inherited when a premise
                        efmObject.tblPropertyConfig_descForQuoteInh = false;

                        efmObject.tblPropertyConfig_nightlyPriceInh = false;

                        // work out if a contacts config should be set as a parent
                        var objPremise = await Premise.FindAsync(iPremiseId.Value);
                        if (objPremise.Group_Id.HasValue && objPremise.Group_Use_Contacts)
                        {
                            // find groups primary contact
                            var objPrimaryContact = (await Contact.FindAllBy_GroupAsync(objPremise.Group_Id.Value)).FirstOrDefault(r => r.Config_Primary);
                            if (objPrimaryContact != null)
                            {
                                efmObject.tblPropertyConfig_parentId = (await FindBy_ContactAsync(objPrimaryContact.Contact_Id)).Id;
                            }
                            objPrimaryContact = null;
                        }
                        else
                        {
                            // find premises primary contact
                            var objPrimaryContact = (await Contact.FindAllBy_PremiseAsync(objPremise.Id)).FirstOrDefault(r => r.Config_Primary);
                            if (objPrimaryContact != null)
                            {
                                efmObject.tblPropertyConfig_parentId = (await FindBy_ContactAsync(objPrimaryContact.Contact_Id)).Id;
                            }
                            objPrimaryContact = null;
                        }
                        objPremise = null;
                    }

                    if (iSeasonId.HasValue)
                    {
                        // work out the parent premise id by getting the season
                        var objSeason = await Seasons.Season.FindAsync(iSeasonId.Value);
                        if (!objSeason.Loaded)
                        {
                            // season not found, incorrect id passed maybe?
                            return objReturn;
                        }

                        // get the premise config
                        efmObject.tblPropertyConfig_parentId = (await FindBy_PremiseAsync(objSeason.Premise_Id)).Id;

                        objSeason = null;
                    }

                    _ = dB.tblPropertyConfigs.Add(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        _ = objReturn.Load(efmObject);

                        LocalCache.RefreshCache(CacheKey);
                    }
                }

                efmExisting = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Config).ToString(), "CreateAsync(int?, int?, int?, string)", ex,
                    "iContactId: " + (iContactId.HasValue ? iContactId.Value.ToString() : "") +
                    ", iPremiseId: " + (iPremiseId.HasValue ? iPremiseId.Value.ToString() : "") +
                    ", iSeasonId: " + (iSeasonId.HasValue ? iSeasonId.Value.ToString() : "") +
                    ", strBy: " + strBy.ToString());
                return objReturn;
            }

            return objReturn;
        }

        #endregion private functions


        #region Internal Functions

        /// <summary>
        /// Recalculates the parent config of the given config
        /// </summary>
        /// <param name="iId"></param>
        /// <param name="bClearCache"></param>
        /// <returns>If a change was made</returns>
        internal static async Task<bool> Inheritance_RecalculateAsync(int iId, bool bClearCache = true)
        {
            // recalculation cannot create configs as this may result in freshly deleted configs getting recreated

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertyConfigs.FirstOrDefaultAsync(r => r.tblPropertyConfig_id == iId);

                if (efmObject != null)
                {
                    var bChangeMade = false;

                    if (efmObject.tblContact_id.HasValue)
                    {
                        // config is for a contact, no inheritance to process (ensure parent is null)
                        if (efmObject.tblPropertyConfig_parentId.HasValue)
                        {
                            efmObject.tblPropertyConfig_parentId = null;
                            bChangeMade = true;
                        }
                    }
                    else if (efmObject.tblProperty_id.HasValue)
                    {
                        // config is for a premise, my inherit from a contact
                        // work out if a contacts config should be set as a parent
                        var objPremise = await Premise.FindAsync(efmObject.tblProperty_id.Value);
                        if (objPremise.Group_Id.HasValue && objPremise.Group_Use_Contacts)
                        {
                            // find groups primary contact
                            var objPrimaryContact = (await Contact.FindAllBy_GroupAsync(objPremise.Group_Id.Value)).FirstOrDefault(r => r.Config_Primary);
                            if (objPrimaryContact != null)
                            {
                                // find the config of the contact
                                var efmContactConfig = await dB.tblPropertyConfigs.AsNoTracking().FirstOrDefaultAsync(r => r.tblContact_id == objPrimaryContact.Contact_Id);
                                if (efmContactConfig != null)
                                {
                                    if (efmObject.tblPropertyConfig_parentId != efmContactConfig.tblPropertyConfig_id)
                                    {
                                        efmObject.tblPropertyConfig_parentId = efmContactConfig.tblPropertyConfig_id;
                                        bChangeMade = true;
                                    }
                                }
                                else
                                {
                                    // no config found for the primary contact
                                    if (efmObject.tblPropertyConfig_parentId.HasValue)
                                    {
                                        // clear the parent
                                        efmObject.tblPropertyConfig_parentId = null;
                                        bChangeMade = true;
                                    }
                                }
                                efmContactConfig = null;
                            }
                            else
                            {
                                // no primary contact found
                                if (efmObject.tblPropertyConfig_parentId.HasValue)
                                {
                                    // clear the parent
                                    efmObject.tblPropertyConfig_parentId = null;
                                    bChangeMade = true;
                                }
                            }
                            objPrimaryContact = null;
                        }
                        else
                        {
                            // find premises primary contact
                            var objPrimaryContact = (await Contact.FindAllBy_PremiseAsync(objPremise.Id)).FirstOrDefault(r => r.Config_Primary);
                            if (objPrimaryContact != null)
                            {
                                // find the config of the contact
                                var efmContactConfig = await dB.tblPropertyConfigs.AsNoTracking().FirstOrDefaultAsync(r => r.tblContact_id == objPrimaryContact.Contact_Id);
                                if (efmContactConfig != null)
                                {
                                    if (efmObject.tblPropertyConfig_parentId != efmContactConfig.tblPropertyConfig_id)
                                    {
                                        efmObject.tblPropertyConfig_parentId = efmContactConfig.tblPropertyConfig_id;
                                        bChangeMade = true;
                                    }
                                }
                                else
                                {
                                    // no config found for the primary contact
                                    if (efmObject.tblPropertyConfig_parentId.HasValue)
                                    {
                                        // clear the parent
                                        efmObject.tblPropertyConfig_parentId = null;
                                        bChangeMade = true;
                                    }
                                }
                                efmContactConfig = null;
                            }
                            else
                            {
                                // no primary contact found
                                if (efmObject.tblPropertyConfig_parentId.HasValue)
                                {
                                    // clear the parent
                                    efmObject.tblPropertyConfig_parentId = null;
                                    bChangeMade = true;
                                }
                            }
                            objPrimaryContact = null;
                        }
                        objPremise = null;
                    }
                    else if (efmObject.tblPropertySeason_id.HasValue)
                    {
                        // config is for a premise season, this inherits from a premise
                        // and set on creation so never changes
                    }

                    if (bChangeMade)
                    {
                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            if (bClearCache)
                            {
                                LocalCache.RefreshCache(CacheKey);
                            }
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Config).ToString(), "Inheritance_RecalculateAsync(int, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        internal static async Task Inheritance_RecalculateBy_PremiseAsync(List<int> lstPremiseIds)
        {
            try
            {
                // create a list of all the config ids that needs to be checked/updated
                var lstPremiseConfigIds = new ConcurrentBag<int>();

                // setup options to limit number of concurrent tasks
                var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 5 };

                await Parallel.ForEachAsync(lstPremiseIds, parallelOptions, async (iPremiseId, cancellationToken) =>
                {
                    lstPremiseConfigIds.Add((await FindBy_PremiseAsync(iPremiseId)).Id);
                });

                if (!lstPremiseConfigIds.IsEmpty)
                {
                    // configs to process
                    var lstPremiseConfigChanges = new ConcurrentBag<bool>();

                    await Parallel.ForEachAsync(lstPremiseConfigIds, parallelOptions, async (iPremiseConfigId, cancellationToken) =>
                    {
                        lstPremiseConfigChanges.Add(await Inheritance_RecalculateAsync(iPremiseConfigId, bClearCache: false));
                    });

                    if (lstPremiseConfigChanges.Any(r => r))
                    {
                        LocalCache.RefreshCache(CacheKey);
                    }

                    lstPremiseConfigChanges = null;
                }

                parallelOptions = null;

                lstPremiseConfigIds = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Config).ToString(), "Inheritance_RecalculateBy_PremiseAsync(List<int>)", ex);
            }
        }

        /// <summary>
        /// Changes this config settings to match those of the source then saves
        /// </summary>
        /// <param name="objSource">Source config to change this config to</param>
        /// <param name="strBy">Who is making the change</param>
        /// <returns></returns>
        internal async Task<bool> CloneAsync(Config objSource, string strBy)
        {
            if (!Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertyConfigs.FirstOrDefaultAsync(r => r.tblPropertyConfig_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblPropertyConfig_descForQuoteInh = objSource.DescriptionForQuote_Inherit;
                    efmObject.tblPropertyConfig_descForQuote = objSource.DescriptionForQuote;

                    efmObject.tblPropertyConfig_houseRulesInh = objSource.HouseRules_Inherit;
                    efmObject.tblPropertyConfig_houseRules = objSource.HouseRules;

                    efmObject.tblPropertyConfig_inclusionsInh = objSource.Inclusions_Inherit;
                    efmObject.tblPropertyConfig_inclusions = objSource.Inclusions;

                    efmObject.tblPropertyConfig_defAvailStateInh = objSource.DefaultAvailability_Inherit;
                    efmObject.tblPropertyConfig_defAvailState = (int)objSource.DefaultAvailability;

                    efmObject.tblPropertyConfig_reqBookingApvlInh = objSource.RequireBookingApproval_Inherit;
                    efmObject.tblPropertyConfig_reqBookingApvl = objSource.RequireBookingApproval;

                    efmObject.tblPropertyConfig_priceEntryModeInh = objSource.PriceEntryMode_Inherit;
                    efmObject.tblPropertyConfig_priceEntryMode = (int)objSource.PriceEntryMode;

                    efmObject.tblPropertyConfig_currencyInh = objSource.Currency_Id_Inherit;
                    efmObject.tblCurrency_id = objSource.Currency_Id;

                    efmObject.tblPropertyConfig_taxInh = objSource.Tax_Inherit;
                    efmObject.tblPropertyConfig_taxExempt = objSource.Tax_Exempt;
                    efmObject.tblPropertyConfig_taxValue = objSource.Tax_Value;
                    efmObject.tblPropertyConfig_taxNo = objSource.Tax_Number;

                    efmObject.tblPropertyConfig_bankInh = objSource.Bank_Inherit;
                    efmObject.tblPropertyConfig_bankAccName = objSource.Bank_AccountName;
                    efmObject.tblPropertyConfig_bankAccNo = objSource.Bank_AccountNo;
                    efmObject.tblPropertyConfig_bankAccSort = objSource.Bank_AccountSort;
                    efmObject.tblPropertyConfig_bankAccIBAN = objSource.Bank_AccountIBAN;
                    efmObject.tblPropertyConfig_bankAccBIC = objSource.Bank_AccountBIC;
                    efmObject.tblPropertyConfig_bankAddress1 = objSource.Bank_Address1;
                    efmObject.tblPropertyConfig_bankAddress2 = objSource.Bank_Address2;
                    efmObject.tblPropertyConfig_bankAddress3 = objSource.Bank_Address3;
                    efmObject.tblPropertyConfig_bankAddressTown = objSource.Bank_AddressTown;
                    efmObject.tblPropertyConfig_bankAddressCounty = objSource.Bank_AddressCounty;
                    efmObject.tblPropertyConfig_bankAddressPost = objSource.Bank_AddressPostcode;
                    efmObject.tblPropertyConfig_bankAddressCountry = objSource.Bank_AddressCountry;

                    efmObject.tblPropertyConfig_commissionInh = objSource.Commission_Inherit;
                    efmObject.tblPropertyConfig_commissionAmountType = (int)objSource.Commission_AmountType;
                    efmObject.tblPropertyConfig_commissionAmount = objSource.Commission_Amount;
                    efmObject.tblPropertyConfig_commissionNote = objSource.Commission_Note;

                    efmObject.tblPropertyConfig_checkinInh = objSource.Checkin_Inherit;
                    efmObject.tblPropertyConfig_checkin = objSource.Checkin;

                    efmObject.tblPropertyConfig_checkoutInh = objSource.Checkout_Inherit;
                    efmObject.tblPropertyConfig_checkout = objSource.Checkout;

                    efmObject.tblPropertyConfig_changeOverDayInh = objSource.ChangeoverDay_Inherit;
                    efmObject.tblPropertyConfig_changeOverDay = null;
                    if (objSource.ChangeoverDay.HasValue) { efmObject.tblPropertyConfig_changeOverDay = (int)objSource.ChangeoverDay.Value; }

                    efmObject.tblPropertyConfig_minRentalInh = objSource.MinRental_Inherit;
                    efmObject.tblPropertyConfig_minRentalDays = objSource.MinRental_Days;
                    efmObject.tblPropertyConfig_minRentalNote = objSource.MinRental_Note;

                    efmObject.tblPropertyConfig_nightlyPriceInh = objSource.NightlyPrice_Inherit;
                    efmObject.tblPropertyConfig_nightlyPrice = objSource.NightlyPrice;

                    efmObject.tblPropertyConfig_paySchInh = objSource.PaySchedule_Inherit;
                    efmObject.tblPropertyConfig_paySchDeposReq = objSource.PaySchedule_Deposit_Required;
                    efmObject.tblPropertyConfig_paySchDeposAmountType = (int)objSource.PaySchedule_Deposit_AmountType;
                    efmObject.tblPropertyConfig_paySchDeposAmount = objSource.PaySchedule_Deposit_Amount;
                    efmObject.tblPropertyConfig_paySchInterReq = objSource.PaySchedule_Interim_Required;
                    efmObject.tblPropertyConfig_paySchInterAmountType = (int)objSource.PaySchedule_Interim_AmountType;
                    efmObject.tblPropertyConfig_paySchInterAmount = objSource.PaySchedule_Interim_Amount;
                    efmObject.tblPropertyConfig_paySchInterDays = objSource.PaySchedule_Interim_Days;
                    efmObject.tblPropertyConfig_paySchBalDays = objSource.PaySchedule_Balance_Days;

                    efmObject.tblPropertyConfig_paySecDeposInh = objSource.SecurityDeposit_Inherit;
                    efmObject.tblPropertyConfig_paySecDeposReq = objSource.SecurityDeposit_Required;
                    efmObject.tblPropertyConfig_paySecDeposAmountType = (int)objSource.SecurityDeposit_AmountType;
                    efmObject.tblPropertyConfig_paySecDeposAmount = objSource.SecurityDeposit_Amount;
                    efmObject.tblPropertyConfig_paySecDeposCalcFrom = (int)objSource.SecurityDeposit_CalcFrom;
                    efmObject.tblPropertyConfig_paySecDeposDaysBefore = objSource.SecurityDeposit_DaysBeforeDue;
                    efmObject.tblPropertyConfig_paySecDeposDaysAfter = objSource.SecurityDeposit_DaysAfterDue;

                    efmObject.tblPropertyConfig_editedUTC = DateTime.UtcNow;
                    efmObject.tblPropertyConfig_editedBy = strBy;

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
                _ = Error.Exception(typeof(Config).ToString(), "CloneAsync(Config, string)", ex,
                        "Id: " + Id.ToString() +
                        ", objSource: " + objSource.Id.ToString() +
                        ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        // NOTE: We don't need full delete on contact or premise as neither will ever be fully
        // deleted due to maintaining history for bookings

        internal static async Task<bool> DeleteFullBy_SeasonAsync(int iSeasonId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var iChanges = await dB.tblPropertyConfigs.Where(r => r.tblPropertySeason_id == iSeasonId).DeleteAsync();

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Config).ToString(), "DeleteFullBy_SeasonAsync(int, string)", ex,
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

        public async Task<bool> SaveAsync(string strBy)
        {
            if (!Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertyConfigs.FirstOrDefaultAsync(r => r.tblPropertyConfig_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblPropertyConfig_descForQuoteInh = DescriptionForQuote_Inherit;
                    efmObject.tblPropertyConfig_descForQuote = DescriptionForQuote;

                    efmObject.tblPropertyConfig_houseRulesInh = HouseRules_Inherit;
                    efmObject.tblPropertyConfig_houseRules = HouseRules;

                    efmObject.tblPropertyConfig_inclusionsInh = Inclusions_Inherit;
                    efmObject.tblPropertyConfig_inclusions = Inclusions;

                    efmObject.tblPropertyConfig_defAvailStateInh = DefaultAvailability_Inherit;
                    efmObject.tblPropertyConfig_defAvailState = (int)DefaultAvailability;

                    efmObject.tblPropertyConfig_reqBookingApvlInh = RequireBookingApproval_Inherit;
                    efmObject.tblPropertyConfig_reqBookingApvl = RequireBookingApproval;

                    efmObject.tblPropertyConfig_priceEntryModeInh = PriceEntryMode_Inherit;
                    efmObject.tblPropertyConfig_priceEntryMode = (int)PriceEntryMode;

                    efmObject.tblPropertyConfig_currencyInh = Currency_Id_Inherit;
                    efmObject.tblCurrency_id = Currency_Id;

                    efmObject.tblPropertyConfig_taxInh = Tax_Inherit;
                    efmObject.tblPropertyConfig_taxExempt = Tax_Exempt;
                    efmObject.tblPropertyConfig_taxValue = Tax_Value;
                    efmObject.tblPropertyConfig_taxNo = Tax_Number;

                    efmObject.tblPropertyConfig_bankInh = Bank_Inherit;
                    efmObject.tblPropertyConfig_bankAccName = Bank_AccountName;
                    efmObject.tblPropertyConfig_bankAccNo = Bank_AccountNo;
                    efmObject.tblPropertyConfig_bankAccSort = Bank_AccountSort;
                    efmObject.tblPropertyConfig_bankAccIBAN = Bank_AccountIBAN;
                    efmObject.tblPropertyConfig_bankAccBIC = Bank_AccountBIC;
                    efmObject.tblPropertyConfig_bankAddress1 = Bank_Address1;
                    efmObject.tblPropertyConfig_bankAddress2 = Bank_Address2;
                    efmObject.tblPropertyConfig_bankAddress3 = Bank_Address3;
                    efmObject.tblPropertyConfig_bankAddressTown = Bank_AddressTown;
                    efmObject.tblPropertyConfig_bankAddressCounty = Bank_AddressCounty;
                    efmObject.tblPropertyConfig_bankAddressPost = Bank_AddressPostcode;
                    efmObject.tblPropertyConfig_bankAddressCountry = Bank_AddressCountry;

                    efmObject.tblPropertyConfig_commissionInh = Commission_Inherit;
                    efmObject.tblPropertyConfig_commissionAmountType = (int)Commission_AmountType;
                    efmObject.tblPropertyConfig_commissionAmount = Commission_Amount;
                    efmObject.tblPropertyConfig_commissionNote = Commission_Note;

                    efmObject.tblPropertyConfig_checkinInh = Checkin_Inherit;
                    efmObject.tblPropertyConfig_checkin = Checkin;

                    efmObject.tblPropertyConfig_checkoutInh = Checkout_Inherit;
                    efmObject.tblPropertyConfig_checkout = Checkout;

                    efmObject.tblPropertyConfig_changeOverDayInh = ChangeoverDay_Inherit;
                    efmObject.tblPropertyConfig_changeOverDay = null;
                    if (ChangeoverDay.HasValue) { efmObject.tblPropertyConfig_changeOverDay = (int)ChangeoverDay.Value; }

                    efmObject.tblPropertyConfig_minRentalInh = MinRental_Inherit;
                    efmObject.tblPropertyConfig_minRentalDays = MinRental_Days;
                    efmObject.tblPropertyConfig_minRentalNote = MinRental_Note;

                    efmObject.tblPropertyConfig_nightlyPriceInh = NightlyPrice_Inherit;
                    efmObject.tblPropertyConfig_nightlyPrice = NightlyPrice;

                    efmObject.tblPropertyConfig_paySchInh = PaySchedule_Inherit;
                    efmObject.tblPropertyConfig_paySchDeposReq = PaySchedule_Deposit_Required;
                    efmObject.tblPropertyConfig_paySchDeposAmountType = (int)PaySchedule_Deposit_AmountType;
                    efmObject.tblPropertyConfig_paySchDeposAmount = PaySchedule_Deposit_Amount;
                    efmObject.tblPropertyConfig_paySchInterReq = PaySchedule_Interim_Required;
                    efmObject.tblPropertyConfig_paySchInterAmountType = (int)PaySchedule_Interim_AmountType;
                    efmObject.tblPropertyConfig_paySchInterAmount = PaySchedule_Interim_Amount;
                    efmObject.tblPropertyConfig_paySchInterDays = PaySchedule_Interim_Days;
                    efmObject.tblPropertyConfig_paySchBalDays = PaySchedule_Balance_Days;

                    efmObject.tblPropertyConfig_paySecDeposInh = SecurityDeposit_Inherit;
                    efmObject.tblPropertyConfig_paySecDeposReq = SecurityDeposit_Required;
                    efmObject.tblPropertyConfig_paySecDeposAmountType = (int)SecurityDeposit_AmountType;
                    efmObject.tblPropertyConfig_paySecDeposAmount = SecurityDeposit_Amount;
                    efmObject.tblPropertyConfig_paySecDeposCalcFrom = (int)SecurityDeposit_CalcFrom;
                    efmObject.tblPropertyConfig_paySecDeposDaysBefore = SecurityDeposit_DaysBeforeDue;
                    efmObject.tblPropertyConfig_paySecDeposDaysAfter = SecurityDeposit_DaysAfterDue;

                    efmObject.tblPropertyConfig_editedUTC = DateTime.UtcNow;
                    efmObject.tblPropertyConfig_editedBy = strBy;

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
                _ = Error.Exception(typeof(Config).ToString(), "SaveAsync(string)", ex,
                        "Id: " + Id.ToString() +
                        ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        private static Config Find(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Config(); }

            if (bUseCache)
            {
                return Global_List().TryGetValue(iId, out var value) ? value : new Config();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Config();

                _ = objReturn.Load(iId);

                return objReturn;
            }
        }

        public static async Task<Config> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Config(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Config();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Config();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<Config> FindBy_ContactAsync(int iContactId, bool bUseCache = true)
        {
            if (bUseCache)
            {
                var lstGlobal = await Global_ListAsync();

                if (LocalCache.Get(LocalCache.Key.Premises_Configs_IdxContact) is Dictionary<int, List<int>> lstIndex)
                {
                    if (lstIndex.TryGetValue(iContactId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            // this should return the first item it finds
                            return lstGlobal[iId];
                        }
                    }
                }
            }

            // at this stage we either haven't found anything in cache or not using cache
            // we can just call create as that will automatically check for existing and load if found
            // if not it will create a config for the contact
            var objReturn = await CreateAsync(iContactId, null, null, "System");

            return objReturn;
        }

        public static async Task<Config> FindBy_PremiseAsync(int iPremiseId, bool bUseCache = true)
        {
            if (bUseCache)
            {
                var lstGlobal = await Global_ListAsync();

                if (LocalCache.Get(LocalCache.Key.Premises_Configs_IdxPremise) is Dictionary<int, List<int>> lstIndex)
                {
                    if (lstIndex.TryGetValue(iPremiseId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            // this should return the first item it finds
                            return lstGlobal[iId];
                        }
                    }
                }
            }

            // at this stage we either haven't found anything in cache or not using cache
            // we can just call create as that will automatically check for existing and load if found
            // if not it will create a config for the premise
            var objReturn = await CreateAsync(null, iPremiseId, null, "System");

            return objReturn;
        }

        public static async Task<Config> FindBy_SeasonAsync(int iSeasonId, bool bUseCache = true)
        {
            if (bUseCache)
            {
                var lstGlobal = await Global_ListAsync();

                if (LocalCache.Get(LocalCache.Key.Premises_Configs_IdxPremiseSeason) is Dictionary<int, List<int>> lstIndex)
                {
                    if (lstIndex.TryGetValue(iSeasonId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            // this should return the first item it finds
                            return lstGlobal[iId];
                        }
                    }
                }
            }

            // at this stage we either haven't found anything in cache or not using cache
            // we can just call create as that will automatically check for existing and load if found
            // if not it will create a config for the season
            var objReturn = await CreateAsync(null, null, iSeasonId, "System");

            return objReturn;
        }

        public static async Task<List<Config>> FindAllBy_SeasonAsync(List<int> lstSeasonIds)
        {
            var lstReturn = new List<Config>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Configs_IdxPremiseSeason) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iSeasonId in lstSeasonIds)
                {
                    if (lstIndex.TryGetValue(iSeasonId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstGlobal = null;

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static Dictionary<int, Config> Global_List()
        {
            // check to see if cache is populated (including indexes)
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Config> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Configs_IdxContact) is not Dictionary<int, List<int>> dicIndexContact ||
                LocalCache.Get(LocalCache.Key.Premises_Configs_IdxPremise) is not Dictionary<int, List<int>> dicIndexPremise ||
                LocalCache.Get(LocalCache.Key.Premises_Configs_IdxPremiseSeason) is not Dictionary<int, List<int>> dicIndexSeason)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, Config>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    s_singleCacheBuildLock.Wait();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Config> lstElementsRetry)
                    {
                        // cache now populated, no need to build
                        return lstElementsRetry;
                    }

                    // setup index dictionaries
                    dicIndexContact = new Dictionary<int, List<int>>();
                    dicIndexPremise = new Dictionary<int, List<int>>();
                    dicIndexSeason = new Dictionary<int, List<int>>();

                    // create instance of DB
                    using var dB = Settings.Config.DBPooledConnection();

                    // disable change tracking as only reading
                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    // get all the elements
                    foreach (var efmObject in dB.tblPropertyConfigs.AsNoTracking().ToList())
                    {
                        var obj = new Config(efmObject);

                        if (obj.Loaded)
                        {
                            // successfully populated/loaded info, add to global list using id as key
                            lstElements.Add(obj.Id, obj);

                            // add to contacts index
                            if (obj.Contact_Id.HasValue)
                            {
                                if (!dicIndexContact.ContainsKey(obj.Contact_Id.Value))
                                {
                                    dicIndexContact.Add(obj.Contact_Id.Value, new List<int>());
                                }

                                dicIndexContact[obj.Contact_Id.Value].Add(obj.Id);
                            }

                            // add to premises index
                            if (obj.Premise_Id.HasValue)
                            {
                                if (!dicIndexPremise.ContainsKey(obj.Premise_Id.Value))
                                {
                                    dicIndexPremise.Add(obj.Premise_Id.Value, new List<int>());
                                }

                                dicIndexPremise[obj.Premise_Id.Value].Add(obj.Id);
                            }

                            // add to season index
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
                    LocalCache.Set(LocalCache.Key.Premises_Configs_IdxContact, dicIndexContact);
                    LocalCache.Set(LocalCache.Key.Premises_Configs_IdxPremise, dicIndexPremise);
                    LocalCache.Set(LocalCache.Key.Premises_Configs_IdxPremiseSeason, dicIndexSeason);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Config).ToString(), "Global_List()", ex);
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

        private static async Task<Dictionary<int, Config>> Global_ListAsync()
        {
            // check to see if cache is populated (including indexes)
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Config> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Configs_IdxContact) is not Dictionary<int, List<int>> dicIndexContact ||
                LocalCache.Get(LocalCache.Key.Premises_Configs_IdxPremise) is not Dictionary<int, List<int>> dicIndexPremise ||
                LocalCache.Get(LocalCache.Key.Premises_Configs_IdxPremiseSeason) is not Dictionary<int, List<int>> dicIndexSeason)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, Config>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    await s_singleCacheBuildLock.WaitAsync();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Config> lstElementsRetry)
                    {
                        // cache now populated, no need to build
                        return lstElementsRetry;
                    }

                    // setup index dictionaries
                    dicIndexContact = new Dictionary<int, List<int>>();
                    dicIndexPremise = new Dictionary<int, List<int>>();
                    dicIndexSeason = new Dictionary<int, List<int>>();

                    // create instance of DB
                    using var dB = Settings.Config.DBPooledConnection();

                    // disable change tracking as only reading
                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    // get all the elements
                    foreach (var efmObject in await dB.tblPropertyConfigs.AsNoTracking().ToListAsync())
                    {
                        var obj = new Config(efmObject);

                        if (obj.Loaded)
                        {
                            // successfully populated/loaded info, add to global list using id as key
                            lstElements.Add(obj.Id, obj);

                            // add to contacts index
                            if (obj.Contact_Id.HasValue)
                            {
                                if (!dicIndexContact.ContainsKey(obj.Contact_Id.Value))
                                {
                                    dicIndexContact.Add(obj.Contact_Id.Value, new List<int>());
                                }

                                dicIndexContact[obj.Contact_Id.Value].Add(obj.Id);
                            }

                            // add to premises index
                            if (obj.Premise_Id.HasValue)
                            {
                                if (!dicIndexPremise.ContainsKey(obj.Premise_Id.Value))
                                {
                                    dicIndexPremise.Add(obj.Premise_Id.Value, new List<int>());
                                }

                                dicIndexPremise[obj.Premise_Id.Value].Add(obj.Id);
                            }

                            // add to season index
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
                    LocalCache.Set(LocalCache.Key.Premises_Configs_IdxContact, dicIndexContact);
                    LocalCache.Set(LocalCache.Key.Premises_Configs_IdxPremise, dicIndexPremise);
                    LocalCache.Set(LocalCache.Key.Premises_Configs_IdxPremiseSeason, dicIndexSeason);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Config).ToString(), "Global_ListAsync()", ex);
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
