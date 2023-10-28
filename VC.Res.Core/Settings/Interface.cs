using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;

namespace VC.Res.Core.Settings
{
    public class Interface
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Settings_Interface;

        private const int TypeId = 20;

        #region Properties

        public bool Loaded { get; private set; } = false;


        public string Name { get; set; } = "";
        public string URL { get; set; } = "";
        public string APIKey { get; set; } = "130d0022-8fe4-4878-8ec2-c44c939bb336";

        public bool BehindProxy { get; set; } = false;


        public static Interface Fetch
        {
            get
            {
                if (LocalCache.Get(CacheKey, bDBMonitorCheck: false) is not Interface obj)
                {
                    try
                    {
                        // lock that we are about to rebuild
                        s_singleCacheBuildLock.Wait();

                        // attempt to reget incase it was rebuilt while waiting
                        if (LocalCache.Get(CacheKey, bDBMonitorCheck: false) is Interface objRetry)
                        {
                            return objRetry;
                        }

                        obj = new Interface();

                        if (obj.Loaded)
                        {
                            LocalCache.Set(CacheKey, obj);
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

        public Interface()
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
                var lstSettings = dB.tblSysSettings.AsNoTracking().Where(r => r.tblSysSetting_type == TypeId).ToList();

                // loop over the types properties
                foreach (var vProperty in typeof(Interface).GetProperties())
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
                var lstSettings = dB.tblSysSettings.Where(r => r.tblSysSetting_type == TypeId).ToList();

                // loop over the types properties
                foreach (var vProperty in typeof(Interface).GetProperties())
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
                            tblSysSetting_type = TypeId,
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
                    LocalCache.RefreshCache(CacheKey);
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
