using System.Data;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Premises
{
    public class Room
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Rooms;

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public int Premise_Id { get; private set; } = 0;

        public Shared.Enums.Premises_Room_Type Type { get; private set; } = Shared.Enums.Premises_Room_Type.Unknown;

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public int Beds_Double { get; set; } = 0;
        public int Beds_TwinDouble { get; set; } = 0;
        public int Beds_Twin { get; set; } = 0;
        public int Beds_Single { get; set; } = 0;
        public int Beds_Bunk { get; set; } = 0;
        public int Beds_Sofa { get; set; } = 0;
        public int Beds_Child { get; set; } = 0;
        public bool Ensuite { get; set; } = false;

        public bool Access_Inside { get; set; } = false;
        public bool Access_Outside { get; set; } = false;

        public string Note { get; set; } = "";

        public int Order { get; private set; } = 0;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string EditedBy { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Room() { }

        private Room(tblPropertyRoom efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblPropertyRooms.AsNoTracking().FirstOrDefaultAsync(r => r.tblPropertyRoom_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Room).ToString(), "Load(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblPropertyRoom efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertyRoom_id;

                    Premise_Id = efmObject.tblProperty_id;

                    Type = (Shared.Enums.Premises_Room_Type)efmObject.tblPropertyRoom_type;

                    Name = efmObject.tblPropertyRoom_name;
                    Description = efmObject.tblPropertyRoom_desc;

                    Beds_Double = efmObject.tblPropertyRoom_bedsDouble;
                    Beds_TwinDouble = efmObject.tblPropertyRoom_bedsTwinDouble;
                    Beds_Twin = efmObject.tblPropertyRoom_bedsTwin;
                    Beds_Single = efmObject.tblPropertyRoom_bedsSingle;
                    Beds_Bunk = efmObject.tblPropertyRoom_bedsBunk;
                    Beds_Sofa = efmObject.tblPropertyRoom_bedsSofa;
                    Beds_Child = efmObject.tblPropertyRoom_bedsChild;

                    Ensuite = efmObject.tblPropertyRoom_ensuite;

                    Access_Inside = efmObject.tblPropertyRoom_accessInside;
                    Access_Outside = efmObject.tblPropertyRoom_accessOutside;

                    Note = efmObject.tblPropertyRoom_note;

                    Order = efmObject.tblPropertyRoom_order;

                    Created_UTC = efmObject.tblPropertyRoom_createdUTC;
                    Created_By = efmObject.tblPropertyRoom_createdBy;

                    Edited_UTC = efmObject.tblPropertyRoom_editedUTC;
                    EditedBy = efmObject.tblPropertyRoom_editedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblPropertyRoom)", ex);
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

                var iChanges = await dB.tblPropertyRooms.Where(r => r.tblProperty_id == iPremiseId).DeleteAsync();

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Room).ToString(), "DeleteFullBy_PremiseAsync(int)", ex,
                    "iPremiseId: " + iPremiseId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion internal functions


        #region Public Functions

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(int iPremiseId, Shared.Enums.Premises_Room_Type enumType, string strBy)
        {
            if (Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblPropertyRoom
                {
                    tblProperty_id = iPremiseId,
                    tblPropertyRoom_type = (int)enumType,

                    tblPropertyRoom_name = Name,
                    tblPropertyRoom_desc = Description,

                    tblPropertyRoom_bedsDouble = Beds_Double,
                    tblPropertyRoom_bedsTwinDouble = Beds_TwinDouble,
                    tblPropertyRoom_bedsTwin = Beds_Twin,
                    tblPropertyRoom_bedsSingle = Beds_Single,
                    tblPropertyRoom_bedsBunk = Beds_Bunk,
                    tblPropertyRoom_bedsSofa = Beds_Sofa,
                    tblPropertyRoom_bedsChild = Beds_Child,

                    tblPropertyRoom_ensuite = Ensuite,

                    tblPropertyRoom_accessInside = Access_Inside,
                    tblPropertyRoom_accessOutside = Access_Outside,

                    tblPropertyRoom_note = Note,

                    tblPropertyRoom_order = await dB.tblPropertyRooms.AsNoTracking().CountAsync(r => r.tblProperty_id == iPremiseId && r.tblPropertyRoom_type == (int)enumType) + 1,

                    tblPropertyRoom_createdUTC = DateTime.UtcNow,
                    tblPropertyRoom_createdBy = strBy,

                    tblPropertyRoom_editedUTC = DateTime.UtcNow,
                    tblPropertyRoom_editedBy = strBy
                };

                _ = dB.tblPropertyRooms.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);

                    await Premise.MaxGuests_Recalculate(iPremiseId);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Room).ToString(), "CreateAsync(int, string)", ex,
                        "iPremiseId: " + iPremiseId +
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

                var efmObject = await dB.tblPropertyRooms.FirstOrDefaultAsync(r => r.tblPropertyRoom_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblPropertyRoom_name = Name;
                    efmObject.tblPropertyRoom_desc = Description;

                    efmObject.tblPropertyRoom_bedsDouble = Beds_Double;
                    efmObject.tblPropertyRoom_bedsTwinDouble = Beds_TwinDouble;
                    efmObject.tblPropertyRoom_bedsTwin = Beds_Twin;
                    efmObject.tblPropertyRoom_bedsSingle = Beds_Single;
                    efmObject.tblPropertyRoom_bedsBunk = Beds_Bunk;
                    efmObject.tblPropertyRoom_bedsSofa = Beds_Sofa;
                    efmObject.tblPropertyRoom_bedsChild = Beds_Child;

                    efmObject.tblPropertyRoom_ensuite = Ensuite;

                    efmObject.tblPropertyRoom_accessInside = Access_Inside;
                    efmObject.tblPropertyRoom_accessOutside = Access_Outside;

                    efmObject.tblPropertyRoom_note = Note;

                    efmObject.tblPropertyRoom_editedUTC = DateTime.UtcNow;
                    efmObject.tblPropertyRoom_editedBy = strBy;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = Load(efmObject);

                        LocalCache.RefreshCache(CacheKey);

                        await Premise.MaxGuests_Recalculate(efmObject.tblProperty_id);
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Room).ToString(), "SaveAsync(string)", ex,
                    "Id: " + Id.ToString() +
                    ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteFullAsync(int iId, string strBy, bool bClearCache = true)
        {
            var bReturn = false;

            try
            {
                var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertyRooms.FirstOrDefaultAsync(r => r.tblPropertyRoom_id == iId);

                if (efmObject != null)
                {
                    var iPremiseId = efmObject.tblProperty_id;
                    var iType = efmObject.tblPropertyRoom_type;
                    var iOrder = efmObject.tblPropertyRoom_order;

                    _ = dB.tblPropertyRooms.Remove(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

                        // reorder the elements after it
                        _ = await dB.tblPropertyRooms.Where(r => r.tblProperty_id == iPremiseId &&
                                                                    r.tblPropertyRoom_type == iType &&
                                                                    r.tblPropertyRoom_order > iOrder)
                                                        .UpdateAsync(r => new tblPropertyRoom { tblPropertyRoom_order = r.tblPropertyRoom_order - 1 });

                        if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
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
                _ = Error.Exception(typeof(Room).ToString(), "DeleteFullAsync(int, string, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
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

                var efmObject = await dB.tblPropertyRooms.FirstOrDefaultAsync(r => r.tblPropertyRoom_id == iId);

                if (efmObject != null)
                {
                    // get the items to reorder
                    var lstToReOrder = await dB.tblPropertyRooms.Where(r => r.tblProperty_id == efmObject.tblProperty_id &&
                                                                            r.tblPropertyRoom_type == efmObject.tblPropertyRoom_type)
                                                                .OrderBy(r => r.tblPropertyRoom_order)
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

                            if (efmTmpObj.tblPropertyRoom_id == iId)
                            {
                                efmTmpObj.tblPropertyRoom_order = iPosition;
                            }
                            else
                            {
                                efmTmpObj.tblPropertyRoom_order = iNewPosition;
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
                _ = Error.Exception(typeof(Room).ToString(), "Move_ToPositionAsync(int, int)", ex,
                    "iId: " + iId.ToString() +
                    ", iPosition: " + iPosition.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Room> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Room(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Room();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Room();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Room>> FindAllBy_PremiseAsync(int iPremiseId, List<Shared.Enums.Premises_Room_Type>? lstTypes = null)
        {
            return await FindAllBy_PremiseAsync(new List<int> { iPremiseId }, lstTypes);
        }

        public static async Task<List<Room>> FindAllBy_PremiseAsync(List<int> lstPremiseIds, List<Shared.Enums.Premises_Room_Type>? lstTypes = null)
        {
            var lstReturn = new List<Room>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Rooms_IdxPremise) is Dictionary<int, List<int>> lstIndex)
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

            lstReturn = lstReturn.OrderByDescending(r => r.Type).ThenBy(r => r.Order).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Room>> Global_ListAsync()
        {
            // check to see if cache is populated (including indexes)
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Room> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Rooms_IdxPremise) is not Dictionary<int, List<int>> dicIndexPremise)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, Room>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    await s_singleCacheBuildLock.WaitAsync();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Room> lstElementsRetry)
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
                    foreach (var efmObject in await dB.tblPropertyRooms.AsNoTracking().ToListAsync())
                    {
                        var obj = new Room(efmObject);

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
                    LocalCache.Set(LocalCache.Key.Premises_Rooms_IdxPremise, dicIndexPremise);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Room).ToString(), "Global_ListAsync()", ex);
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
