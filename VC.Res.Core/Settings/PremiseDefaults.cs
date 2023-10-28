using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;

namespace VC.Res.Core.Settings
{
    public class PremiseDefaults
    {
        private readonly int _i_Type = 30;

        #region Properties

        public bool Loaded { get; private set; } = false;


        public Enums.Premises_Premise_Availability DefaultAvailability { get; set; } = Enums.Premises_Premise_Availability.AvailableEnquire;

        public bool RequireBookingApproval { get; set; } = false;

        public Enums.Shared_PriceValueType PriceEntryMode { get; set; } = Enums.Shared_PriceValueType.Net;

        public int Currency_Id { get; set; } = 0;

        public Enums.Shared_NumericValueType Commission_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public decimal Commission_Amount { get; set; } = 0;

        public TimeSpan Checkin { get; set; } = new TimeSpan(15, 0, 0);
        public TimeSpan Checkout { get; set; } = new TimeSpan(11, 0, 0);
        public DayOfWeek? ChangeoverDay { get; set; } = null; // null value represents open/flexible changeover day
        public int MinRental_Days { get; set; } = 3;

        public bool PaySchedule_Deposit_Required { get; set; } = true;
        public Enums.Shared_NumericValueType PaySchedule_Deposit_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public decimal PaySchedule_Deposit_Amount { get; set; } = 10;

        public bool PaySchedule_Interim_Required { get; set; } = false;
        public Enums.Shared_NumericValueType PaySchedule_Interim_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public decimal PaySchedule_Interim_Amount { get; set; } = 20;
        public int PaySchedule_Interim_Days { get; set; } = 28; // Number of days before arrival the interim is expected.

        public int PaySchedule_Balance_Days { get; set; } = 14; // Number of days before arrival the balance is expected.

        public bool SecurityDeposit_Required { get; set; } = false;
        public Enums.Shared_NumericValueType SecurityDeposit_AmountType { get; set; } = Enums.Shared_NumericValueType.Percentage;
        public decimal SecurityDeposit_Amount { get; set; } = 10;
        public Enums.Shared_PriceValueType SecurityDeposit_CalcFrom { get; set; } = Enums.Shared_PriceValueType.Gross;
        public int SecurityDeposit_DaysBeforeDue { get; set; } = 7; // number of days before arrival the security deposit is expected
        public int SecurityDeposit_DaysAfterDue { get; set; } = 7; // number of days after departure the security deposit is expected to be returned



        public static PremiseDefaults Fetch
        {
            get
            {
                if (LocalCache.Get(LocalCache.Key.Settings_PremiseDefaults, bDBMonitorCheck: false) is not PremiseDefaults obj)
                {
                    try
                    {
                        // lock that we are about to rebuild
                        s_singleCacheBuildLock.Wait();

                        // attempt to reget incase it was rebuilt while waiting
                        if (LocalCache.Get(LocalCache.Key.Settings_PremiseDefaults, bDBMonitorCheck: false) is PremiseDefaults objRetry)
                        {
                            return objRetry;
                        }

                        obj = new PremiseDefaults();

                        if (obj.Loaded)
                        {
                            LocalCache.Set(LocalCache.Key.Settings_PremiseDefaults, obj);
                        }
                    }
                    finally
                    {
                        // release lock
                        _ = s_singleCacheBuildLock.Release();
                    }
                }

                return obj;
            }
        }

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        #endregion properties


        #region Constructors

        public PremiseDefaults()
        {
            Loaded = Refresh();
        }

        #endregion


        #region Private functions

        private void ProcessChanges()
        {

        }

        #endregion


        #region Public Functions

        public bool Refresh()
        {
            try
            {
                using var dB = Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // get all the settings
                var lstSettings = dB.tblSysSettings.AsNoTracking().Where(r => r.tblSysSetting_type == _i_Type).ToList();

                // loop over the types properties
                foreach (var vProperty in typeof(PremiseDefaults).GetProperties())
                {
                    // don't process 'Loaded'
                    if (vProperty.Name == nameof(Loaded)) { continue; }

                    // don't process 'Cached'
                    if (vProperty.Name == nameof(Fetch)) { continue; }

                    // find the settings
                    var efmSetting = lstSettings.FirstOrDefault(r => r.tblSysSetting_key == vProperty.Name);

                    if (efmSetting != null)
                    {
                        // take action based on the system type
                        switch (vProperty.PropertyType.FullName)
                        {
                            case "System.Boolean":
                                {
                                    if (bool.TryParse(efmSetting.tblSysSetting_value, out var bTemp))
                                    {
                                        vProperty.SetValue(this, bTemp);
                                    }
                                }
                                break;

                            case "System.DateTime":
                                {
                                    if (DateTime.TryParse(efmSetting.tblSysSetting_value, out var dt))
                                    {
                                        vProperty.SetValue(this, dt);
                                    }
                                }
                                break;

                            //case nameof(typeof(DayOfWeek?)):
                            //    {
                            //        if (Enum.TryParse(typeof(DayOfWeek), efmSetting.tblSysSetting_value, out var enumDayOfWeek)) { vProperty.SetValue(this, enumDayOfWeek); }
                            //    }
                            //    break;

                            case "System.Decimal":
                                {
                                    if (decimal.TryParse(efmSetting.tblSysSetting_value, out var dTemp))
                                    {
                                        vProperty.SetValue(this, dTemp);
                                    }
                                }
                                break;

                            case "System.Double":
                                {
                                    if (double.TryParse(efmSetting.tblSysSetting_value, out var dblTemp))
                                    {
                                        vProperty.SetValue(this, dblTemp);
                                    }
                                }
                                break;

                            case "System.Int32":
                                {
                                    if (int.TryParse(efmSetting.tblSysSetting_value, out var iTemp))
                                    {
                                        vProperty.SetValue(this, iTemp);
                                    }
                                }
                                break;

                            case "System.TimeSpan":
                                {
                                    if (TimeSpan.TryParse(efmSetting.tblSysSetting_value, out var tsTemp)) { vProperty.SetValue(this, tsTemp); }
                                }
                                break;

                            case "System.String":
                                vProperty.SetValue(this, efmSetting.tblSysSetting_value);
                                break;

                            default:
                                if (vProperty.PropertyType == typeof(DayOfWeek?))
                                {
                                    vProperty.SetValue(this, null);
                                    if (Enum.TryParse(typeof(DayOfWeek), efmSetting.tblSysSetting_value, out var enumTemp)) { vProperty.SetValue(this, enumTemp); }
                                }
                                else if (vProperty.PropertyType.DeclaringType != null)
                                {
                                    if (vProperty.PropertyType.DeclaringType.FullName == "VC.Res.Core.Enums")
                                    {
                                        // Enums, safe to assume int
                                        if (int.TryParse(efmSetting.tblSysSetting_value, out var iTemp))
                                        {
                                            vProperty.SetValue(this, iTemp);
                                        }
                                    }
                                }
                                else if (vProperty.PropertyType.IsGenericType)
                                {
                                    // converter for list
                                    switch (vProperty.PropertyType.GenericTypeArguments[0].FullName)
                                    {
                                        case "System.String":
                                            vProperty.SetValue(this, Utilities.General.ConvertToListString(efmSetting.tblSysSetting_value));
                                            break;

                                        case "System.Int32":
                                            vProperty.SetValue(this, Utilities.General.ConvertToListInt(efmSetting.tblSysSetting_value));
                                            break;

                                        default: break;
                                    }
                                }
                                break;
                        }
                    }

                    efmSetting = null;
                }

                lstSettings = null;

                return true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Refresh()", ex);
                return false;
            }
        }

        public async Task<bool> SaveAsync()
        {
            try
            {
                using var dB = Config.DBConnection();

                // get all the settings
                var lstSettings = await dB.tblSysSettings.Where(r => r.tblSysSetting_type == _i_Type).ToListAsync();

                // loop over the types properties
                foreach (var vProperty in typeof(PremiseDefaults).GetProperties())
                {
                    // don't process 'Loaded'
                    if (vProperty.Name == nameof(Loaded)) { continue; }

                    // don't process 'Cached'
                    if (vProperty.Name == nameof(Fetch)) { continue; }

                    // find the settings
                    var efmSetting = lstSettings.FirstOrDefault(r => r.tblSysSetting_key == vProperty.Name);

                    // calculate string value
                    var strStringValue = "";
                    switch (vProperty.PropertyType.FullName)
                    {
                        case "System.Boolean":
                        case "System.DateTime":
                        case "System.DayOfWeek":
                        case "System.Decimal":
                        case "System.Double":
                        case "System.Int32":
                        case "System.String":
                        case "System.TimeSpan":
                            strStringValue = vProperty.GetValue(this)?.ToString();
                            break;

                        default:
                            if (vProperty.PropertyType == typeof(DayOfWeek?))
                            {
                                strStringValue = vProperty.GetValue(this)?.ToString();
                            }
                            else if (vProperty.PropertyType.DeclaringType != null)
                            {
                                if (vProperty.PropertyType.DeclaringType.FullName == "VC.Res.Core.Enums")
                                {
                                    // we want the int value of the enum
                                    if (vProperty.GetValue(this) != null)
                                    {
                                        strStringValue = Convert.ToInt32(vProperty.GetValue(this)).ToString();
                                    }
                                }
                            }
                            else if (vProperty.PropertyType.IsGenericType)
                            {
                                // converter for list
                                switch (vProperty.PropertyType.GenericTypeArguments[0].FullName)
                                {
                                    case "System.String":
                                        if (vProperty.GetValue(this) is List<string> lstString)
                                        {
                                            strStringValue = Utilities.General.ConvertToCommaString(lstString);
                                        }
                                        break;

                                    case "System.Int32":
                                        if (vProperty.GetValue(this) is List<int> lstInt)
                                        {
                                            strStringValue = Utilities.General.ConvertToCommaString(lstInt);
                                        }
                                        break;

                                    default: break;
                                }
                            }
                            break;
                    }

                    if (efmSetting != null)
                    {
                        if (strStringValue != efmSetting.tblSysSetting_value)
                        {
                            efmSetting.tblSysSetting_value = strStringValue;
                            efmSetting.tblSysSetting_editedUtc = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        var efmNewSetting = new tblSysSetting()
                        {
                            tblSysSetting_type = _i_Type,
                            tblSysSetting_key = vProperty.Name,
                            tblSysSetting_value = strStringValue,
                            tblSysSetting_createdUtc = DateTime.UtcNow,
                            tblSysSetting_editedUtc = DateTime.UtcNow
                        };

                        _ = dB.tblSysSettings.Add(efmNewSetting);

                        efmNewSetting = null;
                    }

                    efmSetting = null;
                }

                if (await dB.SaveChangesAsync() > 0)
                {
                    LocalCache.RefreshCache(LocalCache.Key.Settings_PremiseDefaults);
                }

                lstSettings = null;

                ProcessChanges();

                return true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "SaveAsync()", ex);
                return false;
            }
        }

        #endregion public functions
    }
}
