namespace VC.Res.Core.Common
{
    using System.Collections;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Dynamic.Core;
    using Microsoft.EntityFrameworkCore;
    using LinqKit;
    using VC.Res.Core.Database;
    using VC.Res.Core.Premises;
    using VC.Res.Core.Utilities;
    using Z.EntityFramework.Plus;
    using System.Security.Cryptography;

    [SuppressMessage("ReSharper", "UseAwaitUsing")]
    public class Collection
    {
        public enum FilterOption
        {
            Ids,
            Name,
            Date_Deleted_UTC
        };

        private const LocalCache.Key CacheKey = LocalCache.Key.Common_Collections;

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool Enabled { get; set; } = false;

        public bool SaveToUmbraco { get; set; } = false;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;

        public string Created_By { get; private set; } = string.Empty;

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;

        public string Edited_By { get; private set; } = string.Empty;

        public DateTime? Deleted_UTC { get; private set; } = null;

        public string Deleted_By { get; private set; } = string.Empty;

        public int PropertiesInCollection { get; set; }

        #endregion properties


        #region Constructors

        public Collection() { }

        private Collection(tblCollection efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                var efmObject = await dB.tblCollections.Include(x => x.tblPropertyCollections).AsNoTracking().FirstOrDefaultAsync(r => r.tblCollection_id == iId);

                if (efmObject != null)
                {
                    bReturn = this.Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "LoadAsync(int)", ex, "iId: " + iId);
                return false;
            }

            return bReturn;
        }

        private bool Load(tblCollection efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    this.Id = efmObject.tblCollection_id;

                    this.Name = efmObject.tblCollection_name;
                    this.Description = efmObject.tblCollection_desc;
                    this.Enabled = efmObject.tblCollection_enabled;
                    this.SaveToUmbraco = efmObject.tblCollection_saveToUmbraco;

                    this.Created_UTC = efmObject.tblCollection_createdUTC;
                    this.Created_By = efmObject.tblCollection_createdBy;
                    this.Edited_UTC = efmObject.tblCollection_editedUTC;
                    this.Edited_By = efmObject.tblCollection_editedBy;
                    this.Deleted_UTC = efmObject.tblCollection_deletedUTC;
                    this.Deleted_By = efmObject.tblCollection_deletedBy;

                    this.Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Collection).ToString(), "Load(tblCollection)", ex);

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

        public async Task<bool> CreateAsync(string strBy)
        {
            if (Loaded) { return false; }

            if (string.IsNullOrWhiteSpace(Name)) { return false; }

            if (!await Check_UniqueNameAsync(Name)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblCollection
                {
                    tblCollection_name = this.Name.Trim(),
                    tblCollection_desc = this.Description,
                    tblCollection_enabled = this.Enabled,
                    tblCollection_saveToUmbraco = this.SaveToUmbraco,

                    tblCollection_createdUTC = DateTime.UtcNow,
                    tblCollection_createdBy = strBy,
                    tblCollection_editedUTC = DateTime.UtcNow,
                    tblCollection_editedBy = strBy,
                    tblCollection_deletedUTC = null,
                    tblCollection_deletedBy = string.Empty,
                };

                _ = dB.tblCollections.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = this.Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Collection).ToString(), "CreateAsync(string)", ex,
                        "strBy: " + strBy);

                return bReturn;
            }

            return bReturn;
        }

        public async Task<bool> CreateCollectionPremisesAsync(List<int> premisesIds, int collectionId, string strBy)
        {
            if (Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                foreach (var premiseId in premisesIds)
                {
                    var efmObject = new tblPropertyCollection()
                    {
                        tblCollection_id = collectionId,
                        tblProperty_id = premiseId
                    };

                    _ = dB.tblPropertyCollections.Add(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        // bReturn = this.Load();
                        bReturn = true;
                        LocalCache.RefreshCache(CacheKey);
                    }

                    efmObject = null;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Collection).ToString(), "CreateAsync(string)", ex,
                    "strBy: " + strBy);

                return bReturn;
            }

            return bReturn;
        }

        public async Task<bool> CreatePremiseCollectionsAsync(List<int> collectionIds, int premiseId, string strBy)
        {
            if (this.Loaded) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                foreach (var collectionId in collectionIds)
                {
                    var efmObject = new tblPropertyCollection()
                    {
                        tblCollection_id = collectionId,
                        tblProperty_id = premiseId
                    };

                    _ = dB.tblPropertyCollections.Add(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;
                        LocalCache.RefreshCache(CacheKey);
                    }

                    efmObject = null;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Collection).ToString(), "CreatePremiseCollectionsAsync(List<int>, int, string)", ex,
                    "collectionIds:" + collectionIds + "," +
                    "premiseId: " + premiseId + "," +
                    "strBy: " + strBy);

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

                var efmObject = await dB.tblCollections.FirstOrDefaultAsync(r => r.tblCollection_id == Id);

                if (efmObject != null)
                {
                    if (Name.Trim().ToLower() != efmObject.tblCollection_name.ToLower())
                    {
                        // name change is taking place, check new name is going to be unique
                        if (!await Check_UniqueNameAsync(Name)) { return false; }
                    }

                    efmObject.tblCollection_name = this.Name.Trim();
                    efmObject.tblCollection_desc = this.Description.Trim();
                    efmObject.tblCollection_enabled = this.Enabled;
                    efmObject.tblCollection_saveToUmbraco = this.SaveToUmbraco;

                    efmObject.tblCollection_editedUTC = DateTime.UtcNow;
                    efmObject.tblCollection_editedBy = strBy;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = this.Load(efmObject);

                        LocalCache.RefreshCache(CacheKey);
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Collection).ToString(), "Save(string)", ex,
                        "Id: " + Id +
                        ", strBy: " + strBy);

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

                var efmObject = await dB.tblCollections.FirstOrDefaultAsync(r => r.tblCollection_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblCollection_deletedUTC.HasValue)
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
                            efmObject.tblCollection_deletedUTC = DateTime.UtcNow;
                            efmObject.tblCollection_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblCollection_deletedUTC = null;
                            efmObject.tblCollection_deletedBy = string.Empty;
                        }

                        efmObject.tblCollection_editedUTC = DateTime.UtcNow;
                        efmObject.tblCollection_editedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

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
                _ = Error.Exception(typeof(Collection).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
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

                var efmObject = await dB.tblCollections.FirstOrDefaultAsync(r => r.tblCollection_id == iId);

                if (efmObject != null)
                {
                    // delete the page
                    _ = dB.tblCollections.Remove(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

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
                _ = Error.Exception(typeof(Collection).ToString(), "DeleteFullAsync(int, string, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Check_UniqueNameAsync(string strName)
        {
            var bReturn = false;
            var strNameToLookup = strName.Trim().ToLower();

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                if (!await dB.tblCollections.AsNoTracking().AnyAsync(r => r.tblCollection_name.ToLower() == strNameToLookup && r.tblCollection_deletedUTC == null))
                {
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Collection).ToString(), "Check_UniqueNameAsync(string)", ex,
                    "strName: " + strName);
                return false;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Collection> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Collection(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Collection();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Collection();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Premises.Premise>> FindCollectionPremisesAsync(int collectionId, bool bIncDeleted = false)
        {
            var lstReturn = new Dictionary<int, Premises.Premise>();

            foreach (var obj in (await Global_ListCollectionPremisesAsync(collectionId)))
            {
                if (!bIncDeleted && obj.Value.Deleted_UTC.HasValue) { continue; }

                lstReturn.Add(obj.Key, obj.Value);
            }

            return lstReturn.Values.ToList();
        }

        public static async Task<int> FindPremiseCollectionId(int propertyId, int collectionId)
        {
            var bReturn = 0;
            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = dB.tblPropertyCollections.FirstOrDefault(x => x.tblProperty_id == propertyId && x.tblCollection_id == collectionId);

                if (efmObject != null)
                {
                    bReturn = efmObject.tblPropertyCollection_id;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Premise).ToString(), "GetPremisesCollectionId(int, int)", ex,
                    "propertyId: " + propertyId +
                    ", collectionId: " + collectionId);

                return bReturn;
            }

            return bReturn;
        }

        public static async Task<List<Collection>> FindAllAsync(List<int>? lstIds = null, bool bIncDeleted = false)
        {
            // xxx
            var lstReturn = new List<Collection>();

            // Get global list
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

        #endregion Finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Collection>> Global_ListAsync()
        {
            var lstElements = new Dictionary<int, Collection>();

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                foreach (var efmObject in await dB.tblCollections.AsNoTracking().ToListAsync())
                {
                    var obj = new Collection(efmObject);

                    if (obj.Loaded)
                    {
                        lstElements.Add(obj.Id, obj);
                    }

                    obj = null;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Collection).ToString(), "Global_ListAsync()", ex);
                return lstElements;
            }

            return lstElements;
        }

        private static async Task<Dictionary<int, Collection>> Global_ListPremiseCollectionsAsync(int premiseId)
        {
            var lstElements = new Dictionary<int, Collection>();

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                using var dB2 = Settings.Config.DBPooledConnection();
                var existingPropertyCollections = dB2.tblPropertyCollections.Where(x => x.tblProperty_id == premiseId);

                foreach (var efmObject in existingPropertyCollections.Include(tblPropertyCollection => tblPropertyCollection.tblCollection))
                {
                    var obj = new Collection(efmObject.tblCollection);

                    obj.PropertiesInCollection = await dB.tblPropertyCollections.CountAsync(r => r.tblCollection_id == obj.Id);

                    if (obj.Loaded)
                    {
                        lstElements.Add(obj.Id, obj);
                    }

                    obj = null;
                }

            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Collection).ToString(), "Global_ListPremiseCollectionsAsync(int)", ex);

                return lstElements;
            }

            return lstElements;
        }


        private static async Task<Dictionary<int, Premises.Premise>> Global_ListCollectionPremisesAsync(int collectionId)
        {
            var lstElements = new Dictionary<int, Premises.Premise>();

            try
            {
                // attempt to re-get in case it was rebuilt while waiting
                if (LocalCache.Get(CacheKey) is Dictionary<int, Premises.Premise> lstElementsRetry)
                {
                    return lstElementsRetry;
                }

                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var existingPropertyCollections = dB.tblPropertyCollections.Where(x => x.tblCollection_id == collectionId);

                foreach (var efmObject in existingPropertyCollections.Include(tblPropertyCollection => tblPropertyCollection.tblProperty))
                {
                    var obj = new Core.Premises.Premise(efmObject.tblProperty);

                    if (obj.Loaded)
                    {
                        lstElements.Add(obj.Id, obj);
                    }

                    obj = null;
                }

            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Collection).ToString(), "Global_ListCollectionPremisesAsync()", ex);
                return lstElements;
            }

            return lstElements;
        }

        #endregion lists

        public static async Task<List<Collection>> ListAsync(bool bClearCache = false)
        {
            var bReturn = new List<Collection>();

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                var efmObject = await dB.tblCollections.AsNoTracking().ToDynamicListAsync<Collection>();

                if (efmObject != null)
                {
                    if (bClearCache) { LocalCache.RefreshCache(CacheKey); }

                    bReturn = efmObject;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception("Collection", "ListAsync(bool)", ex, "bClearCache: " + bClearCache);
                return null;
            }

            return bReturn;
        }

        public static async Task<List<Collection>> FindPremiseCollectionsAsync(int premiseId, bool bIncDeleted = false)
        {
            var lstReturn = new Dictionary<int, Collection>();

            foreach (var obj in (await Global_ListPremiseCollectionsAsync(premiseId)))
            {
                if (!bIncDeleted && obj.Value.Deleted_UTC.HasValue) { continue; }

                lstReturn.Add(obj.Key, obj.Value);
            }

            return lstReturn.Values.ToList();
        }

    }
}
