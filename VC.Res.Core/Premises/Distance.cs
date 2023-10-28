using System.Data;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Premises
{
    public class Distance : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Distances;

        #region Properties

        public int Premise_Id { get; private set; } = 0;

        public Shared.Enums.Premises_Distance_Type Type { get; set; } = Shared.Enums.Premises_Distance_Type.Unknown;

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public double KM { get; set; } = 0;

        public decimal? Latitude { get; set; } = null;

        public decimal? Longitude { get; set; } = null;

        public int? MinBy_Walk { get; set; } = null;

        public int? MinBy_Drive { get; set; } = null;

        public int? MinBy_Boat { get; set; } = null;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string EditedBy { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Distance() { }

        private Distance(tblPropertyDistance efmObject) { _ = Load(efmObject); }

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

                var efmObject = await dB.tblPropertyDistances.AsNoTracking().FirstOrDefaultAsync(r => r.tblPropertyDistance_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Distance).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblPropertyDistance efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertyDistance_id;

                    Premise_Id = efmObject.tblProperty_id;

                    Type = (Shared.Enums.Premises_Distance_Type)efmObject.tblPropertyDistance_type;

                    Name = efmObject.tblPropertyDistance_name;
                    Description = efmObject.tblPropertyDistance_desc;

                    KM = efmObject.tblPropertyDistance_distanceKM;
                    Latitude = efmObject.tblPropertyDistance_lat;
                    Longitude = efmObject.tblPropertyDistance_long;

                    MinBy_Walk = efmObject.tblPropertyDistance_minByWalk;
                    MinBy_Drive = efmObject.tblPropertyDistance_minByDrive;
                    MinBy_Boat = efmObject.tblPropertyDistance_minByBoat;

                    Created_UTC = efmObject.tblPropertyDistance_createdUTC;
                    Created_By = efmObject.tblPropertyDistance_createdBy;

                    Edited_UTC = efmObject.tblPropertyDistance_editedUTC;
                    EditedBy = efmObject.tblPropertyDistance_editedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblPropertyDistance)", ex);
                return false;
            }

            return bReturn;
        }


        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions

        internal static async Task<bool> DeleteFullBy_PremiseAsync(int iPremiseId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var iChanges = await dB.tblPropertyDistances.Where(r => r.tblProperty_id == iPremiseId).DeleteAsync();

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Distance).ToString(), "DeleteFullBy_PremiseAsync(int)", ex,
                    "iPremiseId: " + iPremiseId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion internal functions


        #region Public Functions

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(int iPropertyId, string strBy)
        {
            if (Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblPropertyDistance
                {
                    tblProperty_id = iPropertyId,
                    tblPropertyDistance_type = (int)Type,

                    tblPropertyDistance_name = Name,
                    tblPropertyDistance_desc = Description,

                    tblPropertyDistance_distanceKM = KM,
                    tblPropertyDistance_lat = Latitude,
                    tblPropertyDistance_long = Longitude,

                    tblPropertyDistance_minByWalk = MinBy_Walk,
                    tblPropertyDistance_minByDrive = MinBy_Drive,
                    tblPropertyDistance_minByBoat = MinBy_Boat,

                    tblPropertyDistance_createdUTC = DateTime.UtcNow,
                    tblPropertyDistance_createdBy = strBy,

                    tblPropertyDistance_editedUTC = DateTime.UtcNow,
                    tblPropertyDistance_editedBy = strBy
                };

                _ = dB.tblPropertyDistances.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Distance).ToString(), "CreateAsync(int, string)", ex,
                        "iPropertyId: " + iPropertyId +
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

                var efmObject = await dB.tblPropertyDistances.FirstOrDefaultAsync(r => r.tblPropertyDistance_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblPropertyDistance_type = (int)Type;

                    efmObject.tblPropertyDistance_name = Name;
                    efmObject.tblPropertyDistance_desc = Description;

                    efmObject.tblPropertyDistance_distanceKM = KM;
                    efmObject.tblPropertyDistance_lat = Latitude;
                    efmObject.tblPropertyDistance_long = Longitude;

                    efmObject.tblPropertyDistance_minByWalk = MinBy_Walk;
                    efmObject.tblPropertyDistance_minByDrive = MinBy_Drive;
                    efmObject.tblPropertyDistance_minByBoat = MinBy_Boat;

                    efmObject.tblPropertyDistance_createdUTC = DateTime.UtcNow;
                    efmObject.tblPropertyDistance_createdBy = strBy;

                    efmObject.tblPropertyDistance_editedUTC = DateTime.UtcNow;
                    efmObject.tblPropertyDistance_editedBy = strBy;

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
                _ = Error.Exception(typeof(Distance).ToString(), "SaveAsync(string)", ex,
                        "Id: " + Id.ToString() +
                        ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteFullAsync(int iId)
        {
            var bReturn = false;

            try
            {
                var dB = Settings.Config.DBConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var iChanges = await dB.tblPropertyDistances.Where(r => r.tblPropertyDistance_id == iId).DeleteAsync();

                if (iChanges > 0)
                {
                    LocalCache.RefreshCache(CacheKey);
                }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Distance).ToString(), "DeleteFullAsync(int)", ex,
                    "Id: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Distance> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Distance(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Distance();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Distance();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Distance>> FindAllBy_PremiseAsync(int iPremiseId, List<Shared.Enums.Premises_Distance_Type>? lstTypes = null)
        {
            return await FindAllBy_PremiseAsync(new List<int> { iPremiseId }, lstTypes);
        }

        public static async Task<List<Distance>> FindAllBy_PremiseAsync(List<int> lstPremiseIds, List<Shared.Enums.Premises_Distance_Type>? lstTypes = null)
        {
            var lstReturn = new List<Distance>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Distances_IdxPremise) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var vId in lstPremiseIds)
                {
                    if (lstIndex.TryGetValue(vId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            if (lstTypes != null)
                            {
                                if (!lstTypes.Contains(lstGlobal[iId].Type)) { continue; }
                            }

                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstGlobal = null;

            lstReturn = lstReturn.OrderByDescending(r => r.Type).ThenBy(r => r.KM).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Distance>> Global_ListAsync()
        {
            // check to see if cache is populated (including indexes)
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Distance> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Distances_IdxPremise) is not Dictionary<int, List<int>> dicIndexPremise)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, Distance>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    await s_singleCacheBuildLock.WaitAsync();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Distance> lstElementsRetry)
                    {
                        // cache now populated, no need to build
                        return lstElementsRetry;
                    }

                    // setup index dictionaries
                    dicIndexPremise = new Dictionary<int, List<int>>();

                    // create instance of DB
                    using var dB = Settings.Config.DBPooledConnection();

                    // disable change tracking as only reading
                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    // get all the elements
                    foreach (var efmObject in await dB.tblPropertyDistances.AsNoTracking().ToListAsync())
                    {
                        var obj = new Distance(efmObject);

                        if (obj.Loaded)
                        {
                            // successfully populated/loaded info, add to global list using id as key
                            lstElements.Add(obj.Id, obj);

                            // add to country index
                            if (!dicIndexPremise.ContainsKey(obj.Premise_Id))
                            {
                                dicIndexPremise.Add(obj.Premise_Id, new List<int>());
                            }

                            dicIndexPremise[obj.Premise_Id].Add(obj.Id);
                        }

                        obj = null;
                    }

                    // add the found elements to the cache
                    LocalCache.Set(CacheKey, lstElements);

                    // add indexes to cache
                    LocalCache.Set(LocalCache.Key.Premises_Distances_IdxPremise, dicIndexPremise);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Distance).ToString(), "Global_ListAsync()", ex);
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
