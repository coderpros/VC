namespace VC.Res.Core.Common
{
    using System.Linq.Dynamic.Core;
    using Microsoft.EntityFrameworkCore;
    using VC.Res.Core.Database;
    using VC.Res.Core.Utilities;
    using Z.EntityFramework.Plus;

    public class Tag
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Common_Tags;

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public Enums.Common_Tag_Type Type { get; private set; } = Enums.Common_Tag_Type.Unknown;

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public Shared.Enums.Common_Tag_Icon Icon { get; set; } = Shared.Enums.Common_Tag_Icon.None;

        public List<Shared.Enums.Premises_Tag_Category> PremiseCategories { get; set; } = new List<Shared.Enums.Premises_Tag_Category>();

        public int Order { get; private set; } = 0;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;

        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;

        public string Edited_By { get; private set; } = "";

        public DateTime? Deleted_UTC { get; private set; } = null;
        public string Deleted_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Tag() { }

        private Tag(tblTag efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                var efmObject = await dB.tblTags.AsNoTracking().FirstOrDefaultAsync(r => r.tblTag_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblTag efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblTag_id;

                    Type = (Enums.Common_Tag_Type)efmObject.tblTag_type;

                    Name = efmObject.tblTag_name;
                    Description = efmObject.tblTag_desc;
                    Icon = (Shared.Enums.Common_Tag_Icon)efmObject.tblTag_icon;
                    PremiseCategories = General.ConvertToListEnums<Shared.Enums.Premises_Tag_Category>(efmObject.tblTag_propertyCategories);

                    Order = efmObject.tblTag_order;

                    Created_UTC = efmObject.tblTag_createdUTC;
                    Created_By = efmObject.tblTag_createdBy;
                    Edited_UTC = efmObject.tblTag_editedUTC;
                    Edited_By = efmObject.tblTag_editedBy;
                    Deleted_UTC = efmObject.tblTag_deletedUTC;
                    Deleted_By = efmObject.tblTag_deletedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "Load(tblTag)", ex);
                return bReturn;
            }

            return bReturn;
        }

        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(Enums.Common_Tag_Type enumType, string strBy)
        {
            if (Loaded) { return false; }

            if (string.IsNullOrWhiteSpace(Name)) { return false; }

            if (!await Check_UniqueNameAsync(enumType, Name)) { return false; }

            Type = enumType;

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblTag
                {
                    tblTag_type = (int)Type,
                    tblTag_name = Name.Trim(),
                    tblTag_desc = Description,
                    tblTag_icon = (int)Icon,
                    tblTag_propertyCategories = General.ConvertToCommaString(PremiseCategories),
                    tblTag_order = await dB.tblTags.AsNoTracking().CountAsync(r => r.tblTag_type == (int)enumType && r.tblTag_deletedUTC == null) + 1,
                    tblTag_createdUTC = DateTime.UtcNow,
                    tblTag_createdBy = strBy,
                    tblTag_editedUTC = DateTime.UtcNow,
                    tblTag_editedBy = strBy,
                    tblTag_deletedUTC = null,
                    tblTag_deletedBy = "",
                };

                _ = dB.tblTags.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "CreateAsync(Enums.Common_Tag_Type, string)", ex,
                        "enumType: " + ((int)enumType).ToString() +
                        ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public async Task<bool> SaveAsync(string strBy)
        {
            if (!Loaded) { return false; }

            if (string.IsNullOrWhiteSpace(Name)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblTags.FirstOrDefaultAsync(r => r.tblTag_id == Id);

                if (efmObject != null)
                {
                    if (Name.Trim().ToLower() != efmObject.tblTag_name.ToLower())
                    {
                        // name change is taking place, check new name is going to be unique
                        if (!await Check_UniqueNameAsync(Type, Name)) { return false; }
                    }

                    efmObject.tblTag_name = Name.Trim();

                    efmObject.tblTag_desc = Description;
                    efmObject.tblTag_icon = (int)Icon;
                    efmObject.tblTag_propertyCategories = General.ConvertToCommaString(PremiseCategories);

                    efmObject.tblTag_editedUTC = DateTime.UtcNow;
                    efmObject.tblTag_editedBy = strBy;

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
                _ = Error.Exception(typeof(Tag).ToString(), "Save(string)", ex,
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

                var efmObject = await dB.tblTags.FirstOrDefaultAsync(r => r.tblTag_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblTag_deletedUTC.HasValue)
                    {
                        // everything is correct
                        bReturn = true;
                    }
                    else
                    {
                        var iOrder = 0;

                        // need to make update
                        if (bDeleted)
                        {
                            iOrder = efmObject.tblTag_order;

                            efmObject.tblTag_order = 0;
                            efmObject.tblTag_deletedUTC = DateTime.UtcNow;
                            efmObject.tblTag_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblTag_order = await dB.tblTags.AsNoTracking().CountAsync(r => r.tblTag_type == efmObject.tblTag_type && r.tblTag_deletedUTC == null) + 1;
                            efmObject.tblTag_deletedUTC = null;
                            efmObject.tblTag_deletedBy = "";
                        }

                        efmObject.tblTag_editedUTC = DateTime.UtcNow;
                        efmObject.tblTag_editedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            if (bDeleted)
                            {
                                // reorder the elements after it
                                _ = await dB.tblTags.Where(r => r.tblTag_type == efmObject.tblTag_type &&
                                                                r.tblTag_order > iOrder &&
                                                                r.tblTag_deletedUTC == null)
                                                            .UpdateAsync(r => new tblTag { tblTag_order = r.tblTag_order - 1 });
                            }

                            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
                        }
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
                _ = Error.Exception(typeof(Tag).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bDeleted: " + bDeleted.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
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

                var efmObject = await dB.tblTags.FirstOrDefaultAsync(r => r.tblTag_id == iId);

                if (efmObject != null)
                {
                    // record the order so we can reorder elements after it
                    var iTypeId = efmObject.tblTag_type;
                    var iOrder = efmObject.tblTag_order;

                    // Delete related elements
                    var lstTasks = new List<Task>
                    {
                        Contacts.Tag.DeleteFullBy_TagAsync(iId, strBy),
                        Premises.Tag.DeleteFullBy_TagAsync(iId, strBy)
                    };

                    await Task.WhenAll(lstTasks);

                    lstTasks = null;

                    // delete the page
                    _ = dB.tblTags.Remove(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

                        // reorder the elements after it
                        _ = dB.tblTags.Where(r => r.tblTag_type == iTypeId &&
                                                    r.tblTag_order > iOrder &&
                                                    r.tblTag_deletedUTC == null)
                                        .Update(r => new tblTag { tblTag_order = r.tblTag_order - 1 });

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

                var efmMoveUp = await dB.tblTags.FirstOrDefaultAsync(r => r.tblTag_id == iId);

                if (efmMoveUp == null) { return false; }

                iOldPosition = efmMoveUp.tblTag_order;
                iNewPosition = iOldPosition - 1;

                if (iNewPosition < 1)
                {
                    efmMoveUp = null;
                    return true;
                }

                var efmMoveDown = await dB.tblTags.FirstOrDefaultAsync(r => r.tblTag_type == efmMoveUp.tblTag_type &&
                                                                            r.tblTag_order == iNewPosition);

                if (efmMoveDown == null) { return false; }

                efmMoveUp.tblTag_order = iNewPosition;
                efmMoveDown.tblTag_order = iOldPosition;

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
                _ = Error.Exception(typeof(Tag).ToString(), "Move_UpAsync(int)", ex, "iId: " + iId.ToString());
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

                var efmMoveDown = await dB.tblTags.FirstOrDefaultAsync(r => r.tblTag_id == iId);

                if (efmMoveDown == null) { return false; }

                iOldPosition = efmMoveDown.tblTag_order;
                iNewPosition = iOldPosition + 1;

                if (iNewPosition > await dB.tblTags.AsNoTracking().CountAsync(r => r.tblTag_type == efmMoveDown.tblTag_type && r.tblTag_deletedUTC == null))
                {
                    efmMoveDown = null;
                    return true;
                }

                var efmMoveUp = await dB.tblTags.FirstOrDefaultAsync(r => r.tblTag_type == efmMoveDown.tblTag_type &&
                                                                            r.tblTag_order == iNewPosition);

                if (efmMoveUp == null) { return false; }

                efmMoveUp.tblTag_order = iOldPosition;
                efmMoveDown.tblTag_order = iNewPosition;

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

                var efmObject = await dB.tblTags.FirstOrDefaultAsync(r => r.tblTag_id == iId);

                if (efmObject != null)
                {
                    var lstToReOrder = await dB.tblTags.Where(r => r.tblTag_type == efmObject.tblTag_type && r.tblTag_deletedUTC == null)
                                                        .OrderBy(r => r.tblTag_order)
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

                            if (efmTmpObj.tblTag_id == iId)
                            {
                                efmTmpObj.tblTag_order = iPosition;
                            }
                            else
                            {
                                efmTmpObj.tblTag_order = iNewPosition;
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
                _ = Error.Exception(typeof(Tag).ToString(), "Move_ToPositionAsync(int, int)", ex,
                    "iId: " + iId.ToString() +
                    ", iPosition: " + iPosition.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Check_UniqueNameAsync(Enums.Common_Tag_Type enumType, string strName)
        {
            var bReturn = false;

            var strNameToLookup = strName.Trim().ToLower();

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                if (!await dB.tblTags.AsNoTracking().AnyAsync(r => r.tblTag_type == (int)enumType && r.tblTag_name.ToLower() == strNameToLookup && r.tblTag_deletedUTC == null))
                {
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "Check_UniqueNameAsync(Enums.Shared_Tag_Type, string)", ex,
                    "iParentId: " + ((int)enumType).ToString() +
                    ", strName: " + strName.ToString());
                return false;
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

        public static async Task<List<Tag>> FindAllAsync(List<int> iIds, bool bIncDeleted = true)
        {
            var lstReturn = new List<Tag>();

            // get global list
            var lstGlobal = await Global_ListAsync();

            foreach (var iId in iIds)
            {
                if (lstGlobal.TryGetValue(iId, out var value))
                {
                    if (!bIncDeleted)
                    {
                        if (lstGlobal[iId].Deleted_UTC.HasValue) { continue; }
                    }

                    lstReturn.Add(value);
                }
            }

            lstGlobal = null;

            lstReturn = lstReturn.OrderBy(r => r.Order).ToList();

            return lstReturn;
        }

        public static async Task<List<Tag>> FindAllAsync(Enums.Common_Tag_Type enumType = Enums.Common_Tag_Type.Unknown, bool bIncDeleted = false)
        {
            var lstReturn = new List<Tag>();

            foreach (var obj in (await Global_ListAsync()).Values)
            {
                if (enumType != Enums.Common_Tag_Type.Unknown && enumType != obj.Type) { continue; }

                if (!bIncDeleted && obj.Deleted_UTC.HasValue) { continue; }

                lstReturn.Add(obj);
            }

            lstReturn = lstReturn.OrderBy(r => r.Order).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Tag>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Tag> lstElements)
            {
                lstElements = new Dictionary<int, Tag>();

                try
                {
                    await s_singleCacheBuildLock.WaitAsync();

                    // attempt to reget incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Tag> lstElementsRetry)
                    {
                        return lstElementsRetry;
                    }

                    using var dB = Settings.Config.DBPooledConnection();

                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var efmObject in await dB.tblTags.AsNoTracking().ToListAsync())
                    {
                        var obj = new Tag(efmObject);

                        if (obj.Loaded)
                        {
                            lstElements.Add(obj.Id, obj);
                        }

                        obj = null;
                    }

                    LocalCache.Set(CacheKey, lstElements);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Tag).ToString(), "Global_ListAsync()", ex);
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
