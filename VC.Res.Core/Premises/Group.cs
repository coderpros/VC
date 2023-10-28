using System.Data;
using System.Linq.Dynamic.Core;
using LinqKit;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using VC.Res.Core.Utilities;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Premises
{
    public class Group : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Groups;

        internal static readonly string OrderByDefault = nameof(Name) + " ASC";
        internal static readonly string OrderByDefaultDB = nameof(tblPropertyGroup.tblPropertyGroup_name) + " ASC";

        public enum FilterOption
        {
            Ids,
            Name,
            Date_Deleted_UTC
        };

        #region Properties

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string EditedBy { get; private set; } = "";

        public DateTime? Deleted_UTC { get; private set; } = null;
        public string DeletedBy { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Group() { }

        private Group(tblPropertyGroup efmObject) { _ = Load(efmObject); }

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

                var efmObject = await dB.tblPropertyGroups.AsNoTracking().FirstOrDefaultAsync(r => r.tblPropertyGroup_id == iId);

                if (efmObject != null) { bReturn = Load(efmObject); }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Group).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblPropertyGroup efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertyGroup_id;

                    Name = efmObject.tblPropertyGroup_name;
                    Description = efmObject.tblPropertyGroup_desc;

                    Created_UTC = efmObject.tblPropertyGroup_createdUTC;
                    Created_By = efmObject.tblPropertyGroup_createdBy;

                    Edited_UTC = efmObject.tblPropertyGroup_editedUTC;
                    EditedBy = efmObject.tblPropertyGroup_editedBy;

                    Deleted_UTC = efmObject.tblPropertyGroup_deletedUTC;
                    DeletedBy = efmObject.tblPropertyGroup_deletedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblPropertyGroup)", ex);
                return false;
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

        public async Task<bool> CreateAsync(string strBy)
        {
            if (Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblPropertyGroup
                {
                    tblPropertyGroup_name = Name,
                    tblPropertyGroup_desc = Description,

                    tblPropertyGroup_createdUTC = DateTime.UtcNow,
                    tblPropertyGroup_createdBy = strBy,

                    tblPropertyGroup_editedUTC = DateTime.UtcNow,
                    tblPropertyGroup_editedBy = strBy
                };

                _ = dB.tblPropertyGroups.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Group).ToString(), "CreateAsync(string)", ex,
                        "strBy: " + strBy);

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

                var efmObject = await dB.tblPropertyGroups.FirstOrDefaultAsync(r => r.tblPropertyGroup_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblPropertyGroup_name = Name;
                    efmObject.tblPropertyGroup_desc = Description;

                    efmObject.tblPropertyGroup_createdUTC = DateTime.UtcNow;
                    efmObject.tblPropertyGroup_createdBy = strBy;

                    efmObject.tblPropertyGroup_editedUTC = DateTime.UtcNow;
                    efmObject.tblPropertyGroup_editedBy = strBy;

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
                _ = Error.Exception(typeof(Group).ToString(), "SaveAsync(string)", ex,
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

                var efmObject = await dB.tblPropertyGroups.FirstOrDefaultAsync(r => r.tblPropertyGroup_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblPropertyGroup_deletedUTC.HasValue)
                    {
                        // already in desired state
                        bReturn = true;
                    }
                    else
                    {
                        if (bDeleted)
                        {
                            efmObject.tblPropertyGroup_deletedUTC = DateTime.UtcNow;
                            efmObject.tblPropertyGroup_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblPropertyGroup_deletedUTC = null;
                            efmObject.tblPropertyGroup_deletedBy = "";
                        }

                        efmObject.tblPropertyGroup_deletedUTC = DateTime.UtcNow;
                        efmObject.tblPropertyGroup_deletedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            // clear the group from being used on any properties
                            _ = Premise.Clear_Group(iId);

                            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
                        }
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
                _ = Error.Exception(typeof(Group).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bDeleted: " + bDeleted.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteFullAsync(int iId, string strBy, bool bClearCache = true, bool bForce = false)
        {
            var bReturn = false;

            try
            {
                var dB = Settings.Config.DBConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblPropertyGroups.FirstOrDefaultAsync(r => r.tblPropertyGroup_id == iId);

                if (efmObject != null)
                {
                    // can only fully delete if already flagged for deletion
                    if (efmObject.tblPropertyGroup_deletedUTC != null)
                    {
                        // can only delete if flagged more than short recycle bin period days ago or this is a force action
                        if (efmObject.tblPropertyGroup_deletedUTC < DateTime.UtcNow.AddDays(-Settings.Variables.RecycleBin_EmptyAfterDaysShort) || bForce)
                        {
                            var lstTasks = new List<Task>();

                            lstTasks.Add(Premise.Clear_Group(iId));
                            lstTasks.Add(Contact.DeleteFullBy_GroupAsync(iId, strBy));

                            await Task.WhenAll(lstTasks);

                            _ = dB.tblPropertyGroups.Remove(efmObject);

                            if (await dB.SaveChangesAsync() > 0)
                            {
                                bReturn = true;

                                if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
                            }

                            lstTasks = null;
                        }
                    }
                }
                else
                {
                    // not found so already doesn't exist
                    bReturn = true;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Group).ToString(), "DeleteFullAsync(int, string, bool , boo)", ex,
                    "Id: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bClearCache: " + strBy.ToString() +
                    ", bForce: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Group> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Group(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Group();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Group();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Group>> FindAllAsync(List<int>? lstIds = null, bool bIncDeleted = false)
        {
            var lstReturn = new List<Group>();

            // get global list
            var lstGlobal = await Global_ListAsync();

            if (lstIds != null)
            {
                foreach (var iId in lstIds.Distinct())
                {
                    if (lstGlobal.TryGetValue(iId, out var value))
                    {
                        if (!bIncDeleted && lstGlobal[iId].Deleted_UTC.HasValue) { continue; }

                        lstReturn.Add(value);
                    }
                }
            }
            else
            {
                foreach (var obj in lstGlobal.Values)
                {
                    if (!bIncDeleted && obj.Deleted_UTC.HasValue) { continue; }

                    lstReturn.Add(obj);
                }
            }

            lstReturn = lstReturn.OrderBy(r => r.Name).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Group>> Global_ListAsync()
        {
            // check to see if cache is populated (including indexes)
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Group> lstElements)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, Group>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    await s_singleCacheBuildLock.WaitAsync();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Group> lstElementsRetry)
                    {
                        // cache now populated, no need to build
                        return lstElementsRetry;
                    }

                    // create instance of DB
                    using var dB = Settings.Config.DBPooledConnection();

                    // disable change tracking as only reading
                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    // get all the elements
                    foreach (var efmObject in await dB.tblPropertyGroups.AsNoTracking().ToListAsync())
                    {
                        var obj = new Group(efmObject);

                        if (obj.Loaded)
                        {
                            // successfully populated/loaded info, add to global list using id as key
                            lstElements.Add(obj.Id, obj);
                        }

                        obj = null;
                    }

                    // add the found elements to the cache
                    LocalCache.Set(CacheKey, lstElements);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Group).ToString(), "Global_ListAsync()", ex);
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

        public static async Task<List<Group>> ListAsync(List<Filter<FilterOption>>? lstFilters = null, bool bClearCache = false)
        {
            return (await ListPagedAsync(lstFilters: lstFilters, iPageSize: 1000000, bClearCache: bClearCache)).Elements;
        }

        public static async Task<PagedData<Group>> ListPagedAsync(List<Filter<FilterOption>>? lstFilters = null, int iPageSize = 25, int iPage = 1, List<SortOption>? lstOrdering = null, bool bClearCache = false)
        {
            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }

            var predicate = CompileWhere(lstFilters);

            var lstElements = (await Global_ListAsync()).Values.AsQueryable().AsExpandable().Where(predicate).ToList();

            var objPagedData = new PagedData<Group>(lstElements.Count, iPageSize, iPage);

            var strOrderBy = SortOption.CompileOrderBy<Group>(lstOrdering, OrderByDefault, OrderByDefaultDB, false);

            objPagedData.Elements = lstElements.AsQueryable().OrderBy(strOrderBy).Skip(objPagedData.ElementsToSkip).Take(objPagedData.ElementsToTake).ToList();

            return objPagedData;
        }

        private static Expression<Func<Group, bool>> CompileWhere(List<Filter<FilterOption>>? lstFilters, bool bTrue = true)
        {
            var predicate = PredicateBuilder.New<Group>(true);

            if (!bTrue) { predicate = PredicateBuilder.New<Group>(false); }

            if (lstFilters == null) { return predicate; }

            try
            {
                foreach (var vFilter in lstFilters)
                {
                    switch (vFilter.Option)
                    {
                        case FilterOption.Ids:
                            {
                                if (vFilter.Exclude)
                                {
                                    var lstIds = vFilter.Value_ListInt();
                                    predicate = predicate.And(r => !lstIds.Contains(r.Id));
                                }
                                else
                                {
                                    var lstIds = vFilter.Value_ListInt();
                                    predicate = predicate.And(r => lstIds.Contains(r.Id));
                                }
                            }
                            break;

                        case FilterOption.Name:
                            {
                                var strSearch = vFilter.Value_String().Trim().ToLower();
                                predicate = predicate.And(r => r.Name.ToLower().Contains(strSearch));
                            }
                            break;

                        case FilterOption.Date_Deleted_UTC:
                            {
                                var dfDates = vFilter.Value_DateFilter();

                                if (dfDates.From.HasValue)
                                {
                                    predicate = predicate.And(r => r.Deleted_UTC >= dfDates.From.Value);
                                }

                                if (dfDates.To.HasValue)
                                {
                                    predicate = predicate.And(r => r.Deleted_UTC <= dfDates.To.Value);
                                }

                                if (dfDates.Equal.HasValue)
                                {
                                    predicate = predicate.And(r => r.Deleted_UTC == dfDates.Equal.Value);
                                }

                                if (dfDates.EqualNull.HasValue)
                                {
                                    if (dfDates.EqualNull.Value)
                                    {
                                        predicate = predicate.And(r => r.Deleted_UTC == null);
                                    }
                                    else
                                    {
                                        predicate = predicate.And(r => r.Deleted_UTC != null);
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Group).ToString(), "CompileWhere(List<Filter<FilterOption>>)", ex);
                return predicate;
            }

            return predicate;
        }

        internal static string OrderByConvert(string strFieldName, bool bDBVersion)
        {
            try
            {
                var strObject = "";
                var strDB = "";

                switch (strFieldName)
                {
                    case nameof(Name):
                        strObject = nameof(Name);
                        strDB = nameof(tblPropertyGroup.tblPropertyGroup_name);
                        break;

                    case nameof(Created_UTC):
                        strObject = nameof(Created_UTC);
                        strDB = nameof(tblPropertyGroup.tblPropertyGroup_createdUTC);
                        break;
                    case nameof(Edited_UTC):
                        strObject = nameof(Edited_UTC);
                        strDB = nameof(tblPropertyGroup.tblPropertyGroup_editedUTC);
                        break;
                    default: return "";
                }

                if (bDBVersion)
                {
                    return strDB;
                }
                else
                {
                    return strObject;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Group).ToString(), "OrderByConvert(string, bool)", ex,
                    "strFieldName: " + strFieldName.ToString() +
                    ", bDBVersion: " + bDBVersion);
                return "";
            }
        }

        #endregion lists
    }
}
