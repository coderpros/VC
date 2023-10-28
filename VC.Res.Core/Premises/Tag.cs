using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Premises
{
    public class Tag : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Tags;

        #region Properties

        public int Premise_Id { get; private set; } = 0;

        public int Tag_Id { get; private set; } = 0;
        public Enums.Common_Tag_Type Tag_Type { get; private set; } = Enums.Common_Tag_Type.Unknown;
        public string Tag_Name { get; private set; } = "";
        public string Tag_Description { get; private set; } = "";
        public Shared.Enums.Common_Tag_Icon Tag_Icon { get; private set; } = Shared.Enums.Common_Tag_Icon.None;

        public Shared.Enums.Premises_Tag_Category Category { get; private set; } = Shared.Enums.Premises_Tag_Category.Unknown;

        public string Description { get; set; } = "";

        public int Order { get; private set; } = 0;


        public async Task<Premise> Fetch_PremiseAsync()
        {
            if (!Loaded) { return new Premise(); }

            return await Premise.FindAsync(Premise_Id);
        }

        //public async Task<Common.Tag> Fetch_TagAsync()
        //{
        //    if (!Loaded) { return new Common.Tag(); }

        //    return await Common.Tag.FindAsync(Tag_Id);
        //}

        #endregion properties


        #region Constructors

        public Tag() { }

        private Tag(vwPropertyTag efmObject) { _ = Load(efmObject); }

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

                var efmObject = await dB.vwPropertyTags.AsNoTracking().FirstOrDefaultAsync(r => r.tblPropertyTag_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "LoadAsync(int)", ex,
                    "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(vwPropertyTag efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertyTag_id;

                    Premise_Id = efmObject.tblProperty_id;

                    Tag_Id = efmObject.tblTag_id;
                    Tag_Type = (Enums.Common_Tag_Type)efmObject.tblTag_type;
                    Tag_Name = efmObject.tblTag_name;
                    Tag_Description = efmObject.tblTag_desc;
                    Tag_Icon = (Shared.Enums.Common_Tag_Icon)efmObject.tblTag_icon;

                    Category = (Shared.Enums.Premises_Tag_Category)efmObject.tblPropertyTag_category;

                    Description = efmObject.tblPropertyTag_desc;

                    Order = efmObject.tblPropertyTag_order;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "Load(vwPropertyTag)", ex);
                return bReturn;
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

                var iChanges = await dB.tblPropertyTags.Where(r => r.tblProperty_id == iPremiseId).DeleteAsync();

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "DeleteFullBy_PremiseAsync(int)", ex,
                    "iPremiseId: " + iPremiseId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        internal static async Task<bool> DeleteFullBy_TagAsync(int iTagId, string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // get a list of the ids that reference this
                var lstIds = await dB.tblPropertyTags.AsNoTracking().Where(r => r.tblTag_id == iTagId).Select(r => r.tblPropertyTag_id).ToListAsync();

                if (lstIds.Count > 0)
                {
                    foreach (var iId in lstIds)
                    {
                        _ = await DeleteFullAsync(iId, strBy, bClearCache: false);
                    }

                    LocalCache.RefreshCache(CacheKey);
                }

                lstIds = null;

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "DeleteFullBy_TagAsync(int, string)", ex,
                    "iTagId: " + iTagId.ToString() +
                    ", strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion internal functions


        #region Public Functions

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(int iPremiseId, int iTagId, Shared.Enums.Premises_Tag_Category enumCategory)
        {
            if (Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                // get the tag we are assigning, if it is a property feature, need to involve the category
                var objTag = await Common.Tag.FindAsync(iTagId);

                if (objTag.Type == Enums.Common_Tag_Type.PropertyFeature)
                {
                    // use category for grouping

                    // check if already exists
                    var efmExisting = await dB.vwPropertyTags.AsNoTracking().FirstOrDefaultAsync(r => r.tblProperty_id == iPremiseId && r.tblTag_id == iTagId && r.tblPropertyTag_category == (int)enumCategory);

                    if (efmExisting != null)
                    {
                        // already exists
                        bReturn = Load(efmExisting);
                    }
                    else
                    {
                        // need to create
                        var efmObject = new tblPropertyTag
                        {
                            tblProperty_id = iPremiseId,
                            tblTag_id = iTagId,
                            tblPropertyTag_category = (int)enumCategory,
                            tblPropertyTag_desc = Description,
                            tblPropertyTag_order = await dB.tblPropertyTags.AsNoTracking().CountAsync(r => r.tblProperty_id == iPremiseId && r.tblPropertyTag_category == (int)enumCategory) + 1
                        };

                        _ = dB.tblPropertyTags.Add(efmObject);

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = await LoadAsync(efmObject.tblPropertyTag_id);

                            LocalCache.RefreshCache(CacheKey);
                        }

                        efmObject = null;
                    }

                    efmExisting = null;
                }
                else
                {
                    // don't use category category
                    // check if already exists
                    var efmExisting = await dB.vwPropertyTags.AsNoTracking().FirstOrDefaultAsync(r => r.tblProperty_id == iPremiseId && r.tblTag_id == iTagId);

                    if (efmExisting != null)
                    {
                        // already exists
                        bReturn = Load(efmExisting);
                    }
                    else
                    {
                        // need to create
                        var efmObject = new tblPropertyTag
                        {
                            tblProperty_id = iPremiseId,
                            tblTag_id = iTagId,
                            tblPropertyTag_category = (int)Shared.Enums.Premises_Tag_Category.Unknown,
                            tblPropertyTag_desc = Description,
                            tblPropertyTag_order = await dB.tblPropertyTags.AsNoTracking().CountAsync(r => r.tblProperty_id == iPremiseId && r.tblTag.tblTag_type == (int)objTag.Type) + 1
                        };

                        _ = dB.tblPropertyTags.Add(efmObject);

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = await LoadAsync(efmObject.tblPropertyTag_id);

                            LocalCache.RefreshCache(CacheKey);
                        }

                        efmObject = null;
                    }

                    efmExisting = null;
                }

                objTag = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "CreateAsync(int, int, Enums.Premises_Tag_Category)", ex,
                        "iPremiseId: " + iPremiseId.ToString() +
                        ", iTagId: " + iTagId.ToString() +
                        ", enumCategory: " + ((int)enumCategory).ToString());

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

                var efmObject = await dB.tblPropertyTags.FirstOrDefaultAsync(r => r.tblPropertyTag_id == Id);

                if (efmObject != null)
                {
                    if (efmObject.tblPropertyTag_desc != Description)
                    {
                        efmObject.tblPropertyTag_desc = Description;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = await LoadAsync(efmObject.tblPropertyTag_id);

                            LocalCache.RefreshCache(CacheKey);
                        }
                    }
                    else
                    {
                        // no update to be made as only one field that can be changed
                        bReturn = true;
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "SaveAsync(string)", ex,
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
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertyTags.FirstOrDefaultAsync(r => r.tblPropertyTag_id == iId);

                if (efmObject != null)
                {
                    var iPremiseId = efmObject.tblProperty_id;
                    var enumTagType = (await Common.Tag.FindAsync(efmObject.tblTag_id)).Type;
                    var iCategory = efmObject.tblPropertyTag_category;
                    var iOrder = efmObject.tblPropertyTag_order;

                    _ = dB.tblPropertyTags.Remove(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

                        // reorder the elements after it
                        if (enumTagType == Enums.Common_Tag_Type.PropertyFeature)
                        {
                            _ = await dB.tblPropertyTags.Where(r => r.tblProperty_id == iPremiseId &&
                                                                    r.tblPropertyTag_category == iCategory &&
                                                                    r.tblPropertyTag_order > iOrder)
                                                        .UpdateAsync(r => new tblPropertyTag { tblPropertyTag_order = r.tblPropertyTag_order - 1 });
                        }
                        else
                        {
                            _ = await dB.tblPropertyTags.Where(r => r.tblProperty_id == iPremiseId &&
                                                                    r.tblTag.tblTag_type == (int)enumTagType &&
                                                                    r.tblPropertyTag_order > iOrder)
                                                        .UpdateAsync(r => new tblPropertyTag { tblPropertyTag_order = r.tblPropertyTag_order - 1 });
                        }

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
                _ = Error.Exception(typeof(Tag).ToString(), "DeleteFullAsync(int, string, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Move_UpAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var iOldPosition = 0;
                var iNewPosition = 0;

                var efmMoveUp = await dB.tblPropertyTags.FirstOrDefaultAsync(r => r.tblPropertyTag_id == iId);

                if (efmMoveUp == null) { return false; }

                var enumTagType = (await Common.Tag.FindAsync(efmMoveUp.tblTag_id)).Type;

                iOldPosition = efmMoveUp.tblPropertyTag_order;
                iNewPosition = iOldPosition - 1;

                if (iNewPosition < 1)
                {
                    efmMoveUp = null;
                    return true;
                }

                if (enumTagType == Enums.Common_Tag_Type.PropertyFeature)
                {
                    var efmMoveDown = await dB.tblPropertyTags.FirstOrDefaultAsync(r => r.tblProperty_id == efmMoveUp.tblProperty_id &&
                                                                                        r.tblPropertyTag_category == efmMoveUp.tblPropertyTag_category &&
                                                                                        r.tblPropertyTag_order == iNewPosition);

                    if (efmMoveDown == null) { return false; }

                    efmMoveUp.tblPropertyTag_order = iNewPosition;
                    efmMoveDown.tblPropertyTag_order = iOldPosition;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;
                        LocalCache.RefreshCache(CacheKey);
                    }

                    efmMoveDown = null;
                }
                else
                {
                    var efmMoveDown = await dB.tblPropertyTags.FirstOrDefaultAsync(r => r.tblProperty_id == efmMoveUp.tblProperty_id &&
                                                                                        r.tblTag.tblTag_type == (int)enumTagType &&
                                                                                        r.tblPropertyTag_order == iNewPosition);

                    if (efmMoveDown == null) { return false; }

                    efmMoveUp.tblPropertyTag_order = iNewPosition;
                    efmMoveDown.tblPropertyTag_order = iOldPosition;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;
                        LocalCache.RefreshCache(CacheKey);
                    }

                    efmMoveDown = null;
                }

                efmMoveUp = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "Move_UpAsync(int)", ex,
                    "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Move_DownAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var iOldPosition = 0;
                var iNewPosition = 0;

                var efmMoveDown = await dB.tblPropertyTags.FirstOrDefaultAsync(r => r.tblPropertyTag_id == iId);

                if (efmMoveDown == null) { return false; }

                var enumTagType = (await Common.Tag.FindAsync(efmMoveDown.tblTag_id)).Type;

                iOldPosition = efmMoveDown.tblPropertyTag_order;
                iNewPosition = iOldPosition + 1;

                if (enumTagType == Enums.Common_Tag_Type.PropertyFeature)
                {
                    if (iNewPosition > (await dB.tblPropertyTags.AsNoTracking().CountAsync(r => r.tblProperty_id == efmMoveDown.tblProperty_id && r.tblPropertyTag_category == efmMoveDown.tblPropertyTag_category)))
                    {
                        efmMoveDown = null;
                        return true;
                    }

                    var efmMoveUp = await dB.tblPropertyTags.FirstOrDefaultAsync(r => r.tblProperty_id == efmMoveDown.tblProperty_id &&
                                                                                        r.tblPropertyTag_category == efmMoveDown.tblPropertyTag_category &&
                                                                                        r.tblPropertyTag_order == iNewPosition);

                    if (efmMoveUp == null) { return false; }

                    efmMoveUp.tblPropertyTag_order = iOldPosition;
                    efmMoveDown.tblPropertyTag_order = iNewPosition;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;
                        LocalCache.RefreshCache(CacheKey);
                    }

                    efmMoveUp = null;
                }
                else
                {
                    if (iNewPosition > (await dB.tblPropertyTags.AsNoTracking().CountAsync(r => r.tblProperty_id == efmMoveDown.tblProperty_id && r.tblTag.tblTag_type == (int)enumTagType)))
                    {
                        efmMoveDown = null;
                        return true;
                    }

                    var efmMoveUp = await dB.tblPropertyTags.FirstOrDefaultAsync(r => r.tblProperty_id == efmMoveDown.tblProperty_id &&
                                                                                        r.tblTag.tblTag_type == (int)enumTagType &&
                                                                                        r.tblPropertyTag_order == iNewPosition);

                    if (efmMoveUp == null) { return false; }

                    efmMoveUp.tblPropertyTag_order = iOldPosition;
                    efmMoveDown.tblPropertyTag_order = iNewPosition;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;
                        LocalCache.RefreshCache(CacheKey);
                    }

                    efmMoveUp = null;
                }

                efmMoveDown = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "Move_DownAsync(int)", ex, "iId: " + iId.ToString());
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

                var efmObject = await dB.tblPropertyTags.FirstOrDefaultAsync(r => r.tblPropertyTag_id == iId);

                if (efmObject != null)
                {
                    var enumTagType = (await Common.Tag.FindAsync(efmObject.tblTag_id)).Type;

                    if (enumTagType == Enums.Common_Tag_Type.PropertyFeature)
                    {
                        var lstToReOrder = await dB.tblPropertyTags.Where(r => r.tblProperty_id == efmObject.tblProperty_id &&
                                                                                r.tblPropertyTag_category == efmObject.tblPropertyTag_category)
                                                                    .OrderBy(r => r.tblPropertyTag_order)
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

                                if (efmTmpObj.tblPropertyTag_id == iId)
                                {
                                    efmTmpObj.tblPropertyTag_order = iPosition;
                                }
                                else
                                {
                                    efmTmpObj.tblPropertyTag_order = iNewPosition;
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
                    else
                    {
                        var lstToReOrder = await dB.tblPropertyTags.Where(r => r.tblProperty_id == efmObject.tblProperty_id &&
                                                                                r.tblTag.tblTag_type == (int)enumTagType)
                                                                    .OrderBy(r => r.tblPropertyTag_order)
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

                                if (efmTmpObj.tblPropertyTag_id == iId)
                                {
                                    efmTmpObj.tblPropertyTag_order = iPosition;
                                }
                                else
                                {
                                    efmTmpObj.tblPropertyTag_order = iNewPosition;
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
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "Move_ToPositionAsync(int, int)", ex,
                    "iId: " + iId.ToString() +
                    ", iPosition: " + iPosition.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Tag> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Tag(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Tag();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Tag();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Tag>> FindAllBy_PremiseAsync(int iPremiseId, List<Enums.Common_Tag_Type>? lstTagTypes = null, List<Shared.Enums.Premises_Tag_Category>? lstCategories = null)
        {
            return await FindAllBy_PremiseAsync(new List<int> { iPremiseId }, lstTagTypes, lstCategories);
        }

        public static async Task<List<Tag>> FindAllBy_PremiseAsync(List<int> lstPremiseIds, List<Enums.Common_Tag_Type>? lstTagTypes = null, List<Shared.Enums.Premises_Tag_Category>? lstCategories = null)
        {
            var lstReturn = new List<Tag>();

            // get global list to ensure caches are populated
            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Tags_IdxPremise) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var vId in lstPremiseIds)
                {
                    if (lstIndex.TryGetValue(vId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            if (lstTagTypes != null)
                            {
                                if (!lstTagTypes.Contains(lstGlobal[iId].Tag_Type)) { continue; }
                            }

                            if (lstCategories != null)
                            {
                                if (!lstCategories.Contains(lstGlobal[iId].Category)) { continue; }
                            }

                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstReturn = lstReturn.OrderBy(r => r.Order).ToList();

            return lstReturn;
        }

        public static async Task<List<Tag>> FindAllBy_TagAsync(int iTagId, List<Shared.Enums.Premises_Tag_Category>? lstCategories = null)
        {
            return await FindAllBy_TagAsync(new List<int> { iTagId }, lstCategories);
        }

        public static async Task<List<Tag>> FindAllBy_TagAsync(List<int> lstTagIds, List<Shared.Enums.Premises_Tag_Category>? lstCategories = null)
        {
            var lstReturn = new List<Tag>();

            // get global list to ensure caches are populated
            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Tags_IdxTag) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var vId in lstTagIds)
                {
                    if (lstIndex.TryGetValue(vId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            if (lstCategories != null)
                            {
                                if (!lstCategories.Contains(lstGlobal[iId].Category)) { continue; }
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

        private static async Task<Dictionary<int, Tag>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Tag> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Tags_IdxPremise) is not Dictionary<int, List<int>> dicIndexPremise ||
                LocalCache.Get(LocalCache.Key.Premises_Tags_IdxTag) is not Dictionary<int, List<int>> dicIndexTag)
            {
                lstElements = new Dictionary<int, Tag>();

                try
                {
                    await s_singleCacheBuildLock.WaitAsync();

                    if (LocalCache.Get(CacheKey) is Dictionary<int, Tag> lstElementsRetry)
                    {
                        return lstElementsRetry;
                    }

                    dicIndexPremise = new Dictionary<int, List<int>>();
                    dicIndexTag = new Dictionary<int, List<int>>();

                    using var dB = Settings.Config.DBPooledConnection();

                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var efmObject in await dB.vwPropertyTags.AsNoTracking().ToListAsync())
                    {
                        var obj = new Tag(efmObject);

                        if (obj.Loaded)
                        {
                            lstElements.Add(obj.Id, obj);

                            // add to contact index
                            if (!dicIndexPremise.ContainsKey(obj.Premise_Id))
                            {
                                dicIndexPremise.Add(obj.Premise_Id, new List<int>());
                            }

                            dicIndexPremise[obj.Premise_Id].Add(obj.Id);

                            // add to tag index
                            if (!dicIndexTag.ContainsKey(obj.Tag_Id))
                            {
                                dicIndexTag.Add(obj.Tag_Id, new List<int>());
                            }

                            dicIndexTag[obj.Tag_Id].Add(obj.Id);
                        }

                        obj = null;
                    }

                    LocalCache.Set(CacheKey, lstElements);
                    LocalCache.Set(LocalCache.Key.Premises_Tags_IdxPremise, dicIndexPremise);
                    LocalCache.Set(LocalCache.Key.Premises_Tags_IdxTag, dicIndexTag);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Tag).ToString(), "Global_ListAsync()", ex);
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
