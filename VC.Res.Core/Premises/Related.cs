using System.Data;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Premises
{
    public class Related
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Related;

        internal static readonly string OrderByDefault = nameof(Order) + " ASC";
        internal static readonly string OrderByDefaultDB = nameof(tblPropertyRelated.tblPropertyRelated_order) + " ASC";

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public int Premise_Id { get; private set; } = 0;

        public Enums.Premises_Related_Type Type { get; private set; } = Enums.Premises_Related_Type.Unknown;

        public int Premise_RelatedId { get; private set; } = 0;

        public int Order { get; private set; } = 0;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;

        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;

        public string Edited_By { get; private set; } = "";


        public async Task<Premise> Fetch_PremiseAsync()
        {
            if (!Loaded) { return new Premise(); }

            return await Premise.FindAsync(Premise_Id);
        }

        public async Task<Premise> Fetch_PremiseRelatedAsync()
        {
            if (!Loaded) { return new Premise(); }

            return await Premise.FindAsync(Premise_RelatedId);
        }

        #endregion properties


        #region Constructors

        private Related() { }

        private Related(tblPropertyRelated efmObject) { _ = Load(efmObject); }

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

                var efmObject = await dB.tblPropertyRelateds.AsNoTracking().FirstOrDefaultAsync(r => r.tblPropertyRelated_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Related).ToString(), "LoadAsync(int)", ex,
                    "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblPropertyRelated efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertyRelated_id;

                    Premise_Id = efmObject.tblProperty_id;
                    Type = (Enums.Premises_Related_Type)efmObject.tblPropertyRelated_type;
                    Premise_RelatedId = efmObject.tblProperty_relatedId;

                    Order = efmObject.tblPropertyRelated_order;

                    Created_UTC = efmObject.tblPropertyRelated_createdUTC;
                    Created_By = efmObject.tblPropertyRelated_createdBy;
                    Edited_UTC = efmObject.tblPropertyRelated_editedUTC;
                    Edited_By = efmObject.tblPropertyRelated_editedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Related).ToString(), "Load(tblPropertyRelated)", ex);
                return bReturn;
            }

            return bReturn;
        }

        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions

        internal static async Task<bool> DeleteFullBy_PremiseAsync(int iPremiseId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                if (await dB.tblPropertyRelateds.Where(r => r.tblProperty_id == iPremiseId).DeleteAsync() > 0)
                {
                    LocalCache.RefreshCache(CacheKey);
                }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Related).ToString(), "DeleteFullBy_PremiseAsync(int, string)", ex,
                    "iPremiseId: " + iPremiseId.ToString() +
                    ", strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        internal static async Task<bool> DeleteFullBy_RelatedAsync(int iRelatedPremiseId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // get a list of the ids that reference this
                var lstIds = await dB.tblPropertyRelateds.AsNoTracking().Where(r => r.tblProperty_relatedId == iRelatedPremiseId).Select(r => r.tblProperty_relatedId).ToListAsync();

                if (lstIds.Count > 0)
                {
                    foreach (var iId in lstIds)
                    {
                        _ = DeleteFullAsync(iId, strBy, bClearCache: false);
                    }

                    LocalCache.RefreshCache(CacheKey);
                }

                lstIds = null;

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Related).ToString(), "DeleteFullBy_RelatedAsync(int, string)", ex,
                    "iRelatedPremiseId: " + iRelatedPremiseId.ToString() +
                    ", strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion internal functions


        #region Public Functions

        public static async Task<Related> CreateAsync(int iPremiseId, Enums.Premises_Related_Type enumType, int iRelatedPremiseId, string strBy)
        {
            var objReturn = new Related();

            if (iPremiseId == iRelatedPremiseId)
            {
                // cannot be related to itself
                return objReturn;
            }

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmExisting = await dB.tblPropertyRelateds.FirstOrDefaultAsync(r => r.tblProperty_id == iPremiseId && r.tblPropertyRelated_type == (int)enumType && r.tblProperty_relatedId == iRelatedPremiseId);

                if (efmExisting != null)
                {
                    _ = objReturn.Load(efmExisting);
                }
                else
                {
                    var efmObject = new tblPropertyRelated
                    {
                        tblProperty_id = iPremiseId,
                        tblPropertyRelated_type = (int)enumType,
                        tblProperty_relatedId = iRelatedPremiseId,
                        tblPropertyRelated_order = await dB.tblPropertyRelateds.AsNoTracking().CountAsync(r => r.tblProperty_id == iPremiseId && r.tblPropertyRelated_type == (int)enumType) + 1,
                        tblPropertyRelated_createdUTC = DateTime.UtcNow,
                        tblPropertyRelated_createdBy = strBy,
                        tblPropertyRelated_editedUTC = DateTime.UtcNow,
                        tblPropertyRelated_editedBy = strBy
                    };

                    _ = dB.tblPropertyRelateds.Add(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        _ = objReturn.Load(efmObject);

                        LocalCache.RefreshCache(CacheKey);
                    }

                    efmObject = null;
                }

                efmExisting = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Related).ToString(), "CreateAsync(int,  Enums.Premises_Related_Type, int, string)", ex,
                        "iPropertyId: " + iPremiseId.ToString() +
                        ", enumType: " + ((int)enumType).ToString() +
                        ", iRelatedPropertyId: " + iRelatedPremiseId.ToString() +
                        ", strBy: " + strBy.ToString());

                return objReturn;
            }

            return objReturn;
        }

        public static async Task<bool> DeleteFullAsync(int iId, string strBy, bool bClearCache = true)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertyRelateds.FirstOrDefaultAsync(r => r.tblPropertyRelated_id == iId);

                if (efmObject != null)
                {
                    // record the elements to make updates after delete
                    var iPropertyId = efmObject.tblProperty_id;
                    var iType = efmObject.tblPropertyRelated_type;
                    var iOrder = efmObject.tblPropertyRelated_order;

                    // delete the page
                    _ = dB.tblPropertyRelateds.Remove(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

                        // reorder the elements after it
                        _ = dB.tblPropertyRelateds.Where(r => r.tblProperty_id == iPropertyId &&
                                                                r.tblPropertyRelated_type == iType &&
                                                                r.tblPropertyRelated_order > iOrder)
                                                    .Update(r => new tblPropertyRelated { tblPropertyRelated_order = r.tblPropertyRelated_order - 1 });

                        if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
                    }
                }
                else
                {
                    // not found so already deleted
                    bReturn = true;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Related).ToString(), "DeleteFullAsync(int, string, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Move_UpAsync(int iId)
        {
            // set default return success to failure (false)
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                // variables to remember positions
                var iOldPosition = 0;
                var iNewPosition = 0;

                // get the item to move up
                var efmMoveUp = await dB.tblPropertyRelateds.FirstOrDefaultAsync(r => r.tblPropertyRelated_id == iId);

                if (efmMoveUp == null) { return false; }

                // calculate new positions
                iOldPosition = efmMoveUp.tblPropertyRelated_order;
                iNewPosition = iOldPosition - 1;

                // check new position is not above position 1 (i.e top)
                if (iNewPosition < 1)
                {
                    // the item is already at the top and so can go no higher
                    efmMoveUp = null;
                    return true;
                }

                // get the item to switch position with (i.e move down)
                var efmMoveDown = await dB.tblPropertyRelateds.FirstOrDefaultAsync(r => r.tblProperty_id == efmMoveUp.tblProperty_id &&
                                                                                        r.tblPropertyRelated_type == efmMoveUp.tblPropertyRelated_type &&
                                                                                        r.tblPropertyRelated_order == iNewPosition);

                if (efmMoveDown == null) { return false; }

                // switch the order positions of the two items
                efmMoveUp.tblPropertyRelated_order = iNewPosition;
                efmMoveDown.tblPropertyRelated_order = iOldPosition;

                if (await dB.SaveChangesAsync() > 0)
                {
                    // at least one change made and no exception, assume both changes
                    // saved successfully
                    bReturn = true;
                    LocalCache.RefreshCache(CacheKey);
                }

                efmMoveUp = null;
                efmMoveDown = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Related).ToString(), "Move_UpAsync(int)", ex, "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        // move down
        public static async Task<bool> Move_DownAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                // variables to remember positions
                var iOldPosition = 0;
                var iNewPosition = 0;

                var efmMoveDown = dB.tblPropertyRelateds.FirstOrDefault(r => r.tblPropertyRelated_id == iId);

                if (efmMoveDown == null) { return false; }

                // calculate new positions
                iOldPosition = efmMoveDown.tblPropertyRelated_order;
                iNewPosition = iOldPosition + 1;

                // check new position is not below the last item (i.e bottom)
                if (iNewPosition > (await dB.tblPropertyRelateds.AsNoTracking().CountAsync(r => r.tblProperty_id == efmMoveDown.tblProperty_id && r.tblPropertyRelated_type == efmMoveDown.tblPropertyRelated_type)))
                {
                    // the item is already at the bottom and so can go no lower
                    efmMoveDown = null;
                    return true;
                }

                // get the item to switch position with (i.e move up)
                var efmMoveUp = dB.tblPropertyRelateds.FirstOrDefault(r => r.tblProperty_id == efmMoveDown.tblProperty_id &&
                                                                            r.tblPropertyRelated_type == efmMoveDown.tblPropertyRelated_type &&
                                                                            r.tblPropertyRelated_order == iNewPosition);

                if (efmMoveUp == null) { return false; }

                // switch the order positions of the two items
                efmMoveUp.tblPropertyRelated_order = iOldPosition;
                efmMoveDown.tblPropertyRelated_order = iNewPosition;

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = true;
                    LocalCache.RefreshCache(CacheKey);
                }

                efmMoveUp = null;
                efmMoveDown = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Related).ToString(), "Move_DownAsync(int)", ex, "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        // move to position
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

                var efmObject = await dB.tblPropertyRelateds.FirstOrDefaultAsync(r => r.tblPropertyRelated_id == iId);

                if (efmObject != null)
                {
                    // get the items to reorder
                    var lstToReOrder = await dB.tblPropertyRelateds.Where(r => r.tblProperty_id == efmObject.tblProperty_id &&
                                                                                r.tblPropertyRelated_type == efmObject.tblPropertyRelated_type)
                                                                    .OrderBy(r => r.tblPropertyRelated_order)
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

                            if (efmTmpObj.tblPropertyRelated_id == iId)
                            {
                                efmTmpObj.tblPropertyRelated_order = iPosition;
                            }
                            else
                            {
                                efmTmpObj.tblPropertyRelated_order = iNewPosition;
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
                _ = Error.Exception(typeof(Related).ToString(), "Move_ToPositionAsync(int, int)", ex,
                    "iId: " + iId.ToString() +
                    ", iPosition: " + iPosition.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Related> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Related(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Related();
            }
            else
            {
                var objReturn = new Related();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Related>> FindAllBy_PremiseAsync(int iPremiseId, Enums.Premises_Related_Type enumType = Enums.Premises_Related_Type.Unknown)
        {
            return await FindAllBy_PremiseAsync(new List<int> { iPremiseId }, enumType);
        }

        public static async Task<List<Related>> FindAllBy_PremiseAsync(List<int> iPremiseIds, Enums.Premises_Related_Type enumType = Enums.Premises_Related_Type.Unknown)
        {
            var lstReturn = new List<Related>();

            // get global list
            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Related_IdxPremise) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iPropertyId in iPremiseIds)
                {
                    if (lstIndex.TryGetValue(iPropertyId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            if (enumType != Enums.Premises_Related_Type.Unknown)
                            {
                                // check type
                                if (lstGlobal[iId].Type != enumType) { continue; }
                            }

                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstGlobal = null;

            lstReturn = lstReturn.OrderBy(r => r.Order).ToList();

            return lstReturn;
        }

        public static async Task<List<Related>> FindAllBy_RelatedPremiseAsync(int iRelatedPremiseId, Enums.Premises_Related_Type enumType = Enums.Premises_Related_Type.Unknown)
        {
            return await FindAllBy_RelatedPremiseAsync(new List<int> { iRelatedPremiseId }, enumType);
        }

        public static async Task<List<Related>> FindAllBy_RelatedPremiseAsync(List<int> lstRelatedPremiseIds, Enums.Premises_Related_Type enumType = Enums.Premises_Related_Type.Unknown)
        {
            var lstReturn = new List<Related>();

            // get global list
            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Related_IdxRelatedPremise) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iPropertyId in lstRelatedPremiseIds)
                {
                    if (lstIndex.TryGetValue(iPropertyId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            if (enumType != Enums.Premises_Related_Type.Unknown)
                            {
                                // check type
                                if (lstGlobal[iId].Type != enumType) { continue; }
                            }

                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstReturn = lstReturn.OrderBy(r => r.Order).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Related>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Related> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Related_IdxPremise) is not Dictionary<int, List<int>> dicIndexPremise ||
                LocalCache.Get(LocalCache.Key.Premises_Related_IdxRelatedPremise) is not Dictionary<int, List<int>> dicRelatedPremise)
            {
                lstElements = new Dictionary<int, Related>();

                try
                {
                    // lock that we are about to rebuild
                    await s_singleCacheBuildLock.WaitAsync();

                    // attempt to reget incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Related> lstElementsRetry)
                    {
                        return lstElementsRetry;
                    }

                    dicIndexPremise = new Dictionary<int, List<int>>();
                    dicRelatedPremise = new Dictionary<int, List<int>>();

                    using var dB = Settings.Config.DBPooledConnection();

                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var efmObject in await dB.tblPropertyRelateds.AsNoTracking().ToListAsync())
                    {
                        var obj = new Related(efmObject);

                        if (obj.Loaded)
                        {
                            lstElements.Add(obj.Id, obj);

                            // add to Premise index
                            if (!dicIndexPremise.ContainsKey(obj.Premise_Id))
                            {
                                dicIndexPremise.Add(obj.Premise_Id, new List<int>());
                            }

                            dicIndexPremise[obj.Premise_Id].Add(obj.Id);

                            // add to related content item
                            if (!dicRelatedPremise.ContainsKey(obj.Premise_RelatedId))
                            {
                                dicRelatedPremise.Add(obj.Premise_RelatedId, new List<int>());
                            }

                            dicRelatedPremise[obj.Premise_RelatedId].Add(obj.Id);
                        }

                        obj = null;
                    }

                    LocalCache.Set(CacheKey, lstElements);
                    LocalCache.Set(LocalCache.Key.Premises_Related_IdxPremise, dicIndexPremise);
                    LocalCache.Set(LocalCache.Key.Premises_Related_IdxRelatedPremise, dicRelatedPremise);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Related).ToString(), "Global_ListAsync()", ex);
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
