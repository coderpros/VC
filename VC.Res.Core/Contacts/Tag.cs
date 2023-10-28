using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using VC.Res.Core.Utilities;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Contacts
{
    public class Tag : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Contacts_Tags;

        #region Properties

        public int Contact_Id { get; private set; } = 0;

        public int Tag_Id { get; private set; } = 0;

        public Enums.Common_Tag_Type Tag_Type { get; private set; } = Enums.Common_Tag_Type.Unknown;
        public string Tag_Name { get; private set; } = "";
        public string Tag_Description { get; private set; } = "";
        public Shared.Enums.Common_Tag_Icon Tag_Icon { get; private set; } = Shared.Enums.Common_Tag_Icon.None;

        public int Order { get; private set; } = 0;


        public async Task<Contact> Fetch_ContactAsync()
        {
            if (!Loaded) { return new Contact(); }

            return await Contact.FindAsync(Contact_Id);
        }

        //public async Task<Common.Tag> Fetch_TagAsync()
        //{
        //    if (!Loaded) { return new Common.Tag(); }

        //    return await Common.Tag.FindAsync(Tag_Id);
        //}

        #endregion properties


        #region Constructors

        public Tag() { }

        private Tag(vwContactTag efmObject) { _ = Load(efmObject); }

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

                var efmObject = await dB.vwContactTags.AsNoTracking().FirstOrDefaultAsync(r => r.tblContactTag_id == iId);

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

        private bool Load(vwContactTag efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblContactTag_id;

                    Contact_Id = efmObject.tblContact_id;

                    Tag_Id = efmObject.tblTag_id;
                    Tag_Type = (Enums.Common_Tag_Type)efmObject.tblTag_type;
                    Tag_Name = efmObject.tblTag_name;
                    Tag_Description = efmObject.tblTag_desc;
                    Tag_Icon = (Shared.Enums.Common_Tag_Icon)efmObject.tblTag_icon;

                    Order = efmObject.tblContactTag_order;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "Load(vwContactTag)", ex);
                return bReturn;
            }

            return bReturn;
        }

        #endregion private functions-loaders

        #endregion private functions


        #region Internal Functions

        internal static async Task<bool> DeleteFullBy_ContactAsync(int iContactId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var iChanges = await dB.tblContactTags.Where(r => r.tblContact_id == iContactId).DeleteAsync();

                if (iChanges > 0) { LocalCache.RefreshCache(CacheKey); }

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "DeleteFullBy_ContactAsync(int)", ex,
                    "iContactId: " + iContactId.ToString());
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
                var lstIds = await dB.tblContactTags.AsNoTracking().Where(r => r.tblTag_id == iTagId).Select(r => r.tblContactTag_id).ToListAsync();

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

        public static async Task<Tag> CreateAsync(int iContactId, int iTagId)
        {
            var objReturn = new Tag();

            try
            {
                using var dB = Settings.Config.DBConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // check if already exists
                var efmExisting = await dB.vwContactTags.AsNoTracking().FirstOrDefaultAsync(r => r.tblContact_id == iContactId && r.tblTag_id == iTagId);

                if (efmExisting != null)
                {
                    // already exists
                    _ = objReturn.Load(efmExisting);
                }
                else
                {
                    // need to create
                    var objTag = await Common.Tag.FindAsync(iTagId);

                    var efmObject = new tblContactTag
                    {
                        tblContact_id = iContactId,
                        tblTag_id = iTagId,
                        tblContactTag_order = await dB.tblContactTags.AsNoTracking().CountAsync(r => r.tblContact_id == iContactId && r.tblTag.tblTag_type == (int)objTag.Type) + 1
                    };

                    _ = dB.tblContactTags.Add(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        _ = await objReturn.LoadAsync(efmObject.tblContactTag_id);

                        LocalCache.RefreshCache(CacheKey);
                    }

                    efmObject = null;

                    objTag = null;
                }

                efmExisting = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Tag).ToString(), "CreateAsync(int, int)", ex,
                        "iContactId: " + iContactId.ToString() +
                        ", iTagId: " + iTagId.ToString());

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

                var efmObject = await dB.tblContactTags.FirstOrDefaultAsync(r => r.tblContactTag_id == iId);

                if (efmObject != null)
                {
                    // record the elements to make updates after delete
                    var iContactId = efmObject.tblContact_id;
                    var iTagType = (int)(await Common.Tag.FindAsync(efmObject.tblTag_id)).Type;
                    var iOrder = efmObject.tblContactTag_order;

                    // delete the page
                    _ = dB.tblContactTags.Remove(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

                        // reorder the elements after it
                        _ = await dB.tblContactTags.Where(r => r.tblContact_id == iContactId &&
                                                                r.tblTag.tblTag_type == iTagType &&
                                                                r.tblContactTag_order > iOrder)
                                                        .UpdateAsync(r => new tblContactTag { tblContactTag_order = r.tblContactTag_order - 1 });

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

        public static async Task<bool> DeleteFullAsync(int iContactId, int iTagId, string strBy, bool bClearCache = true)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblContactTags.FirstOrDefaultAsync(r => r.tblContact_id == iContactId && r.tblTag_id == iTagId);

                if (efmObject != null)
                {
                    // record the elements to make updates after delete
                    var iTagType = (int)(await Common.Tag.FindAsync(efmObject.tblTag_id)).Type;
                    var iOrder = efmObject.tblContactTag_order;

                    // delete the page
                    _ = dB.tblContactTags.Remove(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

                        // reorder the elements after it
                        _ = await dB.tblContactTags.Where(r => r.tblContact_id == iContactId &&
                                                                r.tblTag.tblTag_type == iTagType &&
                                                                r.tblContactTag_order > iOrder)
                                                        .UpdateAsync(r => new tblContactTag { tblContactTag_order = r.tblContactTag_order - 1 });

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
                    "iContactId: " + iContactId.ToString() +
                    ", iTagId: " + iTagId.ToString() +
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

                var efmMoveUp = await dB.tblContactTags.FirstOrDefaultAsync(r => r.tblContactTag_id == iId);

                if (efmMoveUp == null) { return false; }

                var iTagType = (int)(await Common.Tag.FindAsync(efmMoveUp.tblTag_id)).Type;

                iOldPosition = efmMoveUp.tblContactTag_order;
                iNewPosition = iOldPosition - 1;

                if (iNewPosition < 1)
                {
                    efmMoveUp = null;
                    return true;
                }

                var efmMoveDown = await dB.tblContactTags.FirstOrDefaultAsync(r => r.tblContact_id == efmMoveUp.tblContact_id &&
                                                                                    r.tblTag.tblTag_type == iTagType &&
                                                                                    r.tblContactTag_order == iNewPosition);

                if (efmMoveDown == null) { return false; }

                efmMoveUp.tblContactTag_order = iNewPosition;
                efmMoveDown.tblContactTag_order = iOldPosition;

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

        // move down
        public static async Task<bool> Move_DownAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var iOldPosition = 0;
                var iNewPosition = 0;

                var efmMoveDown = await dB.tblContactTags.FirstOrDefaultAsync(r => r.tblContactTag_id == iId);

                if (efmMoveDown == null) { return false; }

                var iTagType = (int)(await Common.Tag.FindAsync(efmMoveDown.tblTag_id)).Type;

                iOldPosition = efmMoveDown.tblContactTag_order;
                iNewPosition = iOldPosition + 1;

                if (iNewPosition > (await dB.tblContactTags.AsNoTracking().CountAsync(r => r.tblContact_id == efmMoveDown.tblContact_id && r.tblTag.tblTag_type == iTagType)))
                {
                    efmMoveDown = null;
                    return true;
                }

                var efmMoveUp = await dB.tblContactTags.FirstOrDefaultAsync(r => r.tblContact_id == efmMoveDown.tblContact_id &&
                                                                                    r.tblTag.tblTag_type == iTagType &&
                                                                                    r.tblContactTag_order == iNewPosition);

                if (efmMoveUp == null) { return false; }

                efmMoveUp.tblContactTag_order = iOldPosition;
                efmMoveDown.tblContactTag_order = iNewPosition;

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

        // move to position
        public static async Task<bool> Move_ToPositionAsync(int iId, int iPosition)
        {
            if (iPosition < 1) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblContactTags.FirstOrDefaultAsync(r => r.tblContactTag_id == iId);

                if (efmObject != null)
                {
                    var iTagType = (int)(await Common.Tag.FindAsync(efmObject.tblTag_id)).Type;

                    // get the items to reorder
                    var lstToReOrder = await dB.tblContactTags.Where(r => r.tblContact_id == efmObject.tblContact_id &&
                                                                            r.tblTag.tblTag_type == iTagType)
                                                                .OrderBy(r => r.tblContactTag_order)
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

                            if (efmTmpObj.tblContactTag_id == iId)
                            {
                                efmTmpObj.tblContactTag_order = iPosition;
                            }
                            else
                            {
                                efmTmpObj.tblContactTag_order = iNewPosition;
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

        public static async Task<List<Tag>> FindAllBy_ContactAsync(int iContactId, List<Enums.Common_Tag_Type>? lstTagTypes = null)
        {
            return await FindAllBy_ContactAsync(new List<int> { iContactId }, lstTagTypes);
        }

        public static async Task<List<Tag>> FindAllBy_ContactAsync(List<int> lstContactIds, List<Enums.Common_Tag_Type>? lstTagTypes = null)
        {
            var lstReturn = new List<Tag>();

            // get global list to ensure caches are populated
            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Contacts_Tags_IdxContact) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var vId in lstContactIds)
                {
                    if (lstIndex.TryGetValue(vId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            if (lstTagTypes != null)
                            {
                                if (!lstTagTypes.Contains(lstGlobal[iId].Tag_Type)) { continue; }
                            }

                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstReturn = lstReturn.OrderBy(r => r.Order).ToList();

            return lstReturn;
        }

        public static async Task<List<Tag>> FindAllBy_TagAsync(int iTagId)
        {
            return await FindAllBy_TagAsync(new List<int> { iTagId });
        }

        public static async Task<List<Tag>> FindAllBy_TagAsync(List<int> lstTagIds)
        {
            var lstReturn = new List<Tag>();

            // get global list to ensure caches are populated
            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Contacts_Tags_IdxTag) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var vId in lstTagIds)
                {
                    if (lstIndex.TryGetValue(vId, out var value))
                    {
                        foreach (var iId in value)
                        {
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
                LocalCache.Get(LocalCache.Key.Contacts_Tags_IdxContact) is not Dictionary<int, List<int>> dicIndexContact ||
                LocalCache.Get(LocalCache.Key.Contacts_Tags_IdxTag) is not Dictionary<int, List<int>> dicIndexTag)
            {
                lstElements = new Dictionary<int, Tag>();

                try
                {
                    await s_singleCacheBuildLock.WaitAsync();

                    if (LocalCache.Get(CacheKey) is Dictionary<int, Tag> lstElementsRetry)
                    {
                        return lstElementsRetry;
                    }

                    dicIndexContact = new Dictionary<int, List<int>>();
                    dicIndexTag = new Dictionary<int, List<int>>();

                    using var dB = Settings.Config.DBPooledConnection();

                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var efmObject in await dB.vwContactTags.AsNoTracking().ToListAsync())
                    {
                        var obj = new Tag(efmObject);

                        if (obj.Loaded)
                        {
                            lstElements.Add(obj.Id, obj);

                            // add to contact index
                            if (!dicIndexContact.ContainsKey(obj.Contact_Id))
                            {
                                dicIndexContact.Add(obj.Contact_Id, new List<int>());
                            }

                            dicIndexContact[obj.Contact_Id].Add(obj.Id);

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
                    LocalCache.Set(LocalCache.Key.Contacts_Tags_IdxContact, dicIndexContact);
                    LocalCache.Set(LocalCache.Key.Contacts_Tags_IdxTag, dicIndexTag);
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
