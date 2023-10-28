using System.Runtime.Caching;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;

namespace VC.Res.Core
{
    public static class LocalCache
    {
        public enum Key
        {
            Common_Collections,
            Common_Countries,
            Common_Currencies,
            Common_Regions,
            Common_Regions_IdxCountry,
            Common_Tags,
            Contacts_Addresses,
            Contacts_Addresses_IdxContact,
            Contacts_Contacts,
            Contacts_Emails,
            Contacts_Emails_IdxContact,
            Contacts_Tags,
            Contacts_Tags_IdxContact,
            Contacts_Tags_IdxTag,
            Contacts_TelephoneNos,
            Contacts_TelephoneNos_IdxContact,
            Premises_Availability,
            Premises_Availability_IdxPremiseDate,
            Premises_Collections,
            Premises_Configs,
            Premises_Configs_IdxContact,
            Premises_Configs_IdxPremise,
            Premises_Configs_IdxPremiseSeason,
            Premises_Contacts,
            Premises_Contacts_IdxContact,
            Premises_Contacts_IdxPremise,
            Premises_Contacts_IdxPremiseGroup,
            Premises_Distances,
            Premises_Distances_IdxPremise,
            Premises_Extras,
            Premises_Extras_IdxPremise,
            Premises_Groups,
            Premises_Premises,
            Premises_Premises_IdxGroup,
            Premises_Related,
            Premises_Related_IdxPremise,
            Premises_Related_IdxRelatedPremise,
            Premises_Rooms,
            Premises_Rooms_IdxPremise,
            Premises_Seasons_Extras,
            Premises_Seasons_Extras_IdxExtra,
            Premises_Seasons_Extras_IdxSeason,
            Premises_Seasons_Rates,
            Premises_Seasons_Rates_IdxPremise,
            Premises_Seasons_Rates_IdxSeason,
            Premises_Seasons_Seasons,
            Premises_Seasons_Seasons_IdxPremise,
            Premises_Tags,
            Premises_Tags_IdxPremise,
            Premises_Tags_IdxTag,
            Settings_PremiseDefaults,
            Settings_Global,
            Settings_Interface,
            Users_Users,
            Workflows_Items,
            Workflows_Templates_Templates,
            Workflows_Templates_Items,
            Workflows_Templates_Items_IdxItem,
            Workflows_Templates_Items_IdxTemplate,
            Zoho_Token,
        }


        #region Private Properties

        private static readonly string s_str_Prefix = "VC_Res_Core_LocalCache_";

        #endregion private properties


        #region Public Properties

        public static event EventHandler WebCacheClearRequest;

        #endregion public properties


        #region Private Functions



        #endregion private functions


        #region Internal Functions

        internal static object Get(Key enumKey, string strVariation = "", bool bDBMonitorCheck = true)
        {
            if (bDBMonitorCheck) { RefreshDBMonitored(false); }

            var strKey = string.IsNullOrWhiteSpace(strVariation) ? enumKey.ToString() : enumKey.ToString() + "_" + strVariation;

            return MemoryCache.Default.Get(s_str_Prefix + strKey);
        }

        internal static void Set(Key enumKey, object obj, string strVariation = "", bool bSliding = true, DateTime? dtExpires = null, TimeSpan? tsSlidingDuration = null)
        {
            var strKey = string.IsNullOrWhiteSpace(strVariation) ? enumKey.ToString() : enumKey.ToString() + "_" + strVariation;

            if (bSliding)
            {
                var ts = TimeSpan.FromMinutes(60);

                if (tsSlidingDuration.HasValue) { ts = tsSlidingDuration.Value; }

                _ = MemoryCache.Default.Add(s_str_Prefix + strKey, obj, new CacheItemPolicy { SlidingExpiration = ts });
            }
            else
            {
                var dtExpire = DateTime.Now.AddMinutes(10);

                if (dtExpires.HasValue) { dtExpire = dtExpires.Value; }

                _ = MemoryCache.Default.Add(s_str_Prefix + strKey, obj, new CacheItemPolicy { AbsoluteExpiration = dtExpire });
            }
        }

        #endregion internal functions


        #region Public Functions

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        public static void RefreshDBMonitored(bool bForce)
        {
            var objGlobalSettings = Settings.Global.Fetch;

            if (objGlobalSettings.Cache_DatabasePolling)
            {
                var bRequireCheck = bForce;

                if (!bRequireCheck)
                {
                    if (MemoryCache.Default.Get(s_str_Prefix + "DicDB") is not Dictionary<string, int> dicDBDataInitial)
                    {
                        // need to run check
                        bRequireCheck = true;
                    }
                }

                if (bRequireCheck)
                {
                    try
                    {
                        // lock that we are about to rebuild
                        s_singleCacheBuildLock.Wait();

                        if (!bForce)
                        {
                            // run recheck
                            if (MemoryCache.Default.Get(s_str_Prefix + "DicDB") is Dictionary<string, int> dicDBDataRetry)
                            {
                                // cache now exists, dont need to run
                                bRequireCheck = false;
                            }
                        }

                        if (bRequireCheck)
                        {
                            if (MemoryCache.Default.Get(s_str_Prefix + "DicLocal") is not Dictionary<string, int> dicLocalData)
                            {
                                // no local copy either so start from scratch
                                dicLocalData = new Dictionary<string, int>();
                            }

                            // go to the DB and get the contents of the cache montior to process to see if any cache elements need clearing
                            using (var dB = Settings.Config.DBPooledConnection())
                            {
                                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                                var lstCacheMonitors = dB.tblSysCacheMonitors.AsNoTracking().ToList();

                                // log if any cache is cleared
                                var bCacheCleared = false;

                                for (var i = 0; i < lstCacheMonitors.Count; i++)
                                {
                                    // is it in local
                                    if (dicLocalData.ContainsKey(lstCacheMonitors[i].tblSysCacheMonitor_table))
                                    {
                                        if (dicLocalData[lstCacheMonitors[i].tblSysCacheMonitor_table] == lstCacheMonitors[i].tblSysCacheMonitor_changeId)
                                        {
                                            // no change, continue
                                            continue;
                                        }
                                        else
                                        {
                                            // being a change, will need to update, erase from dictionary for re-add
                                            _ = dicLocalData.Remove(lstCacheMonitors[i].tblSysCacheMonitor_table);
                                        }
                                    }

                                    switch (lstCacheMonitors[i].tblSysCacheMonitor_table)
                                    {
                                        case nameof(tblContact):
                                            RefreshCache(Key.Contacts_Contacts, bClearOutput: false);
                                            break;

                                        case nameof(tblContactAddress):
                                            RefreshCache(Key.Contacts_Addresses, bClearOutput: false);
                                            break;

                                        case nameof(tblContactEmail):
                                            RefreshCache(Key.Contacts_Emails, bClearOutput: false);
                                            break;

                                        case nameof(tblContactTag):
                                            RefreshCache(Key.Contacts_Tags, bClearOutput: false);
                                            break;

                                        case nameof(tblContactTel):
                                            RefreshCache(Key.Contacts_TelephoneNos, bClearOutput: false);
                                            break;

                                        case nameof(tblCountry):
                                            RefreshCache(Key.Common_Countries, bClearOutput: false);
                                            break;

                                        case nameof(tblCurrency):
                                            RefreshCache(Key.Common_Currencies, bClearOutput: false);
                                            break;

                                        case nameof(tblProperty):
                                            RefreshCache(Key.Premises_Premises, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertyAvailability):
                                            RefreshCache(Key.Premises_Availability, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertyConfig):
                                            RefreshCache(Key.Premises_Configs, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertyContact):
                                            RefreshCache(Key.Premises_Contacts, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertyDistance):
                                            RefreshCache(Key.Premises_Distances, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertyExtra):
                                            RefreshCache(Key.Premises_Extras, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertyGroup):
                                            RefreshCache(Key.Premises_Groups, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertyRate):
                                            RefreshCache(Key.Premises_Seasons_Rates, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertyRelated):
                                            RefreshCache(Key.Premises_Related, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertyRoom):
                                            RefreshCache(Key.Premises_Rooms, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertySeason):
                                            RefreshCache(Key.Premises_Seasons_Seasons, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertySeasonExtra):
                                            RefreshCache(Key.Premises_Seasons_Extras, bClearOutput: false);
                                            break;

                                        case nameof(tblPropertyTag):
                                            RefreshCache(Key.Premises_Tags, bClearOutput: false);
                                            RefreshCache(Key.Premises_Collections, bClearOutput: false);
                                            break;

                                        case nameof(tblRegion):
                                            RefreshCache(Key.Common_Regions, bClearOutput: false);
                                            break;

                                        case nameof(tblSysSetting):
                                            RefreshCache(Key.Settings_PremiseDefaults, bClearOutput: false);
                                            RefreshCache(Key.Settings_Global, bClearOutput: false);
                                            RefreshCache(Key.Settings_Interface, bClearOutput: false);
                                            break;

                                        case nameof(tblTag):
                                            RefreshCache(Key.Common_Tags, bClearOutput: false);
                                            break;

                                        case nameof(tblCollection):
                                            RefreshCache(Key.Common_Collections, bClearOutput: false);
                                            break;

                                        case nameof(tblUser):
                                            RefreshCache(Key.Users_Users, bClearOutput: false);
                                            break;

                                        default: break;
                                    }

                                    bCacheCleared = true;

                                    dicLocalData.Add(lstCacheMonitors[i].tblSysCacheMonitor_table, lstCacheMonitors[i].tblSysCacheMonitor_changeId);
                                }

                                if (bCacheCleared) { RefreshOutputCache(); }

                                _ = MemoryCache.Default.Add(s_str_Prefix + "DicDB", dicLocalData, DateTime.Now.AddSeconds(objGlobalSettings.Cache_DatabasePolling_Timeout));

                                lstCacheMonitors = null;
                            }

                            _ = MemoryCache.Default.Add(s_str_Prefix + "DicLocal", dicLocalData, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(60) });
                        }
                    }
                    catch (Exception ex)
                    {
                        _ = Error.Exception(typeof(LocalCache).ToString(), "RefreshDBMonitored(bool)", ex, "bForce: " + bForce.ToString());
                        RefreshWholeCache();
                    }
                    finally
                    {
                        // release lock
                        s_singleCacheBuildLock.Release();
                    }
                }
            }

            objGlobalSettings = null;
        }

        public static void RefreshCache(Key enumKey, string strVariation = "", bool bClearOutput = true)
        {
            var strKey = string.IsNullOrWhiteSpace(strVariation) ? enumKey.ToString() : enumKey.ToString() + "_" + strVariation;

            _ = MemoryCache.Default.Remove(s_str_Prefix + strKey);

            // clear any other caches that depend on the one just cleared
            switch (enumKey)
            {
                case Key.Common_Regions:
                    RefreshCache(Key.Common_Regions_IdxCountry, bClearOutput: false);
                    break;

                case Key.Common_Tags:
                    RefreshCache(Key.Contacts_Tags, bClearOutput: false);
                    RefreshCache(Key.Premises_Tags, bClearOutput: false);
                    break;

                case Key.Contacts_Addresses:
                    RefreshCache(Key.Contacts_Addresses_IdxContact, bClearOutput: false);
                    break;

                case Key.Contacts_Emails:
                    RefreshCache(Key.Contacts_Emails_IdxContact, bClearOutput: false);
                    break;

                case Key.Contacts_Tags:
                    RefreshCache(Key.Contacts_Tags_IdxContact, bClearOutput: false);
                    RefreshCache(Key.Contacts_Tags_IdxTag, bClearOutput: false);
                    break;

                case Key.Contacts_TelephoneNos:
                    RefreshCache(Key.Contacts_TelephoneNos_IdxContact, bClearOutput: false);
                    break;

                case Key.Premises_Availability:
                    RefreshCache(Key.Premises_Availability_IdxPremiseDate, bClearOutput: false);
                    break;

                case Key.Premises_Configs:
                    RefreshCache(Key.Premises_Configs_IdxContact, bClearOutput: false);
                    RefreshCache(Key.Premises_Configs_IdxPremise, bClearOutput: false);
                    RefreshCache(Key.Premises_Configs_IdxPremiseSeason, bClearOutput: false);

                    RefreshCache(Key.Premises_Extras, bClearOutput: false);
                    break;

                case Key.Premises_Contacts:
                    RefreshCache(Key.Premises_Contacts_IdxContact, bClearOutput: false);
                    RefreshCache(Key.Premises_Contacts_IdxPremise, bClearOutput: false);
                    RefreshCache(Key.Premises_Contacts_IdxPremiseGroup, bClearOutput: false);
                    break;

                case Key.Premises_Distances:
                    RefreshCache(Key.Premises_Distances_IdxPremise, bClearOutput: false);
                    break;

                case Key.Premises_Extras:
                    RefreshCache(Key.Premises_Extras_IdxPremise, bClearOutput: false);
                    break;

                case Key.Premises_Premises:
                    RefreshCache(Key.Premises_Premises_IdxGroup, bClearOutput: false);
                    break;

                case Key.Premises_Related:
                    RefreshCache(Key.Premises_Related_IdxPremise, bClearOutput: false);
                    RefreshCache(Key.Premises_Related_IdxRelatedPremise, bClearOutput: false);
                    break;

                case Key.Premises_Rooms:
                    RefreshCache(Key.Premises_Rooms_IdxPremise, bClearOutput: false);
                    break;

                case Key.Premises_Seasons_Extras:
                    RefreshCache(Key.Premises_Seasons_Extras_IdxExtra, bClearOutput: false);
                    RefreshCache(Key.Premises_Seasons_Extras_IdxSeason, bClearOutput: false);
                    break;

                case Key.Premises_Seasons_Rates:
                    RefreshCache(Key.Premises_Seasons_Rates_IdxPremise, bClearOutput: false);
                    RefreshCache(Key.Premises_Seasons_Rates_IdxSeason, bClearOutput: false);
                    break;

                case Key.Premises_Seasons_Seasons:
                    RefreshCache(Key.Premises_Seasons_Seasons_IdxPremise, bClearOutput: false);
                    break;

                case Key.Premises_Tags:
                    RefreshCache(Key.Premises_Tags_IdxPremise, bClearOutput: false);
                    RefreshCache(Key.Premises_Tags_IdxTag, bClearOutput: false);
                    break;

                case Key.Workflows_Templates_Items:
                    RefreshCache(Key.Workflows_Templates_Items_IdxItem, bClearOutput: false);
                    RefreshCache(Key.Workflows_Templates_Items_IdxTemplate, bClearOutput: false);
                    break;

                default:
                    break;
            }

            // to be on safe side, also clear output cache... worst case a couple of requests will take longer while the output is re-cached
            if (bClearOutput)
            {
                RefreshOutputCache();
            }
        }

        public static void RefreshWholeCache()
        {
            var lstKeys = MemoryCache.Default.Select(r => r.Key).Distinct().ToList();

            foreach (var strKey in lstKeys)
            {
                _ = MemoryCache.Default.Remove(strKey);
            }

            lstKeys = null;

            // to be on safe side, also clear output cache... worst case a couple of requests will take longer while the output is re-cached
            RefreshOutputCache();
        }

        public static void RefreshOutputCache()
        {
            try
            {
                WebCacheClearRequest?.Invoke(null, EventArgs.Empty);
            }
            catch (Exception)
            {

            }
        }

        #endregion public functions
    }
}
