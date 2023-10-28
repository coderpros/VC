using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using VC.Res.Core.Integrations.Zoho;

namespace VC.Res.Core.Settings
{
    public class Global
    {
        private readonly int _i_Type = 10;

        #region Properties

        public bool Loaded { get; private set; } = false;


        //public string General_Name { get; set; } = "";

        //public string General_AdminAddress { get; set; } = "";
        //public string General_WebsiteAddress { get; set; } = "";


        public bool Cache_DatabasePolling { get; set; } = true;

        public int Cache_DatabasePolling_Timeout { get; set; } = 60;


        //public string Files_Path { get; set; } = "";

        //public string Files_Path_System { get; set; } = "/content_sys/";

        //public string Files_Path_User { get; set; } = "/content/";


        public string Email_Logo { get; set; } = "";

        public string Email_Server_Address { get; set; } = "";

        public int Email_Server_Port { get; set; } = 25;

        public bool Email_Server_TLS { get; set; } = false;

        public bool Email_Server_ReqAuth { get; set; } = false;

        public string Email_Server_Username { get; set; } = "";

        public string Email_Server_Password { get; set; } = "";

        public string Email_Generic_FromName { get; set; } = "";

        public string Email_Generic_FromAddress { get; set; } = "";

        public string Email_Errors_To_Generic { get; set; } = "";


        public string TextMsg_SMSWorks_APIToken { get; set; } = "";

        public string TextMsg_SenderName { get; set; } = "";


        public string Website_URL { get; set; } = "";

        public bool Website_APIEnabled { get; set; } = false;

        public string Website_APIKey { get; set; } = "";

        //public ZConfigurationOption ZConfigOption { get; set; }

        public string Zoho_Api_Url { get; set; } = "";
        public string Zoho_Refresh_Token { get; set; } = "";
        public string Zoho_Token_Url { get; set; } = "";
        public string Zoho_Client_Secret { get; set; } = "";
        public string Zoho_Client_Id { get; set; } = "";
        public static Global Fetch
        {
            get
            {
                if (LocalCache.Get(LocalCache.Key.Settings_Global, bDBMonitorCheck: false) is not Global obj)
                {
                    try
                    {
                        // lock that we are about to rebuild
                        s_singleCacheBuildLock.Wait();

                        // attempt to reget incase it was rebuilt while waiting
                        if (LocalCache.Get(LocalCache.Key.Settings_Global, bDBMonitorCheck: false) is Global objRetry)
                        {
                            return objRetry;
                        }

                        obj = new Global();

                        if (obj.Loaded)
                        {
                            LocalCache.Set(LocalCache.Key.Settings_Global, obj);
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

        public Global()
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
                foreach (var vProperty in typeof(Global).GetProperties())
                {
                    // don't process 'Loaded'
                    if (vProperty.Name == nameof(Loaded)) { continue; }

                    // don't process 'Cached'
                    if (vProperty.Name == nameof(Fetch)) { continue; }

                    // find the settings
                    var efmSetting = lstSettings.FirstOrDefault(r => r.tblSysSetting_key == vProperty.Name);

                    if (efmSetting != null)
                    {// take action based on the system type
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

                            case "System.String":
                                vProperty.SetValue(this, efmSetting.tblSysSetting_value);
                                break;

                            default:
                                if (vProperty.PropertyType.DeclaringType != null)
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

        public bool Save()
        {
            try
            {
                using var dB = Config.DBPooledConnection();

                // get all the settings
                var lstSettings = dB.tblSysSettings.Where(r => r.tblSysSetting_type == _i_Type).ToList();

                // loop over the types properties
                foreach (var vProperty in typeof(Global).GetProperties())
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
                        case "System.Decimal":
                        case "System.Double":
                        case "System.Int32":
                        case "System.String":
                            strStringValue = vProperty.GetValue(this)?.ToString();
                            break;

                        default:
                            if (vProperty.PropertyType.DeclaringType != null)
                            {
                                if (vProperty.PropertyType.DeclaringType.FullName == "VC.Res.Core.Enums")
                                {
                                    // we want the int value of the enum
                                    strStringValue = Convert.ToInt32(vProperty.GetValue(this)).ToString();
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

                if (dB.SaveChanges() > 0)
                {
                    LocalCache.RefreshCache(LocalCache.Key.Settings_Global);
                }

                lstSettings = null;

                ProcessChanges();

                return true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Save()", ex);
                return false;
            }
        }

        #endregion public functions
    }
}
