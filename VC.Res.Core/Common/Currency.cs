using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Common
{
    public class Currency
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Common_Currencies;

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public string Name { get; set; } = "";

        public string Code { get; set; } = "";

        public string Symbol { get; set; } = "";

        public bool SymbolAfter { get; set; } = false;

        public bool Default { get; private set; } = false;

        public int Order { get; private set; } = 0;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;

        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;

        public string Edited_By { get; private set; } = "";

        public DateTime? Deleted_UTC { get; private set; } = null;

        public string Deleted_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Currency() { }

        private Currency(tblCurrency efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                var efmObject = await dB.tblCurrencies.AsNoTracking().FirstOrDefaultAsync(r => r.tblCurrency_id == iId);

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

        private bool Load(tblCurrency efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblCurrency_id;

                    Name = efmObject.tblCurrency_name;
                    Code = efmObject.tblCurrency_code;
                    Symbol = efmObject.tblCurrency_symbol;
                    SymbolAfter = efmObject.tblCurrency_symbolAfter;
                    Default = efmObject.tblCurrency_default;
                    Order = efmObject.tblCurrency_order;

                    Created_UTC = efmObject.tblCurrency_createdUTC;
                    Created_By = efmObject.tblCurrency_createdBy;
                    Edited_UTC = efmObject.tblCurrency_editedUTC;
                    Edited_By = efmObject.tblCurrency_editedBy;
                    Deleted_UTC = efmObject.tblCurrency_deletedUTC;
                    Deleted_By = efmObject.tblCurrency_deletedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblCurrency)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        public async Task<bool> CreateAsync(string strBy)
        {
            if (Loaded) { return false; }

            if (string.IsNullOrWhiteSpace(Name)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblCurrency
                {
                    tblCurrency_name = Name,
                    tblCurrency_code = Code.Trim().ToUpper(),
                    tblCurrency_symbol = Symbol,
                    tblCurrency_symbolAfter = SymbolAfter,
                    tblCurrency_default = !await dB.tblCurrencies.AsNoTracking().AnyAsync(r => r.tblCurrency_default),
                    tblCurrency_order = await dB.tblCurrencies.CountAsync(g => g.tblCurrency_deletedUTC == null) + 1,
                    tblCurrency_createdUTC = DateTime.UtcNow,
                    tblCurrency_createdBy = strBy,
                    tblCurrency_editedUTC = DateTime.UtcNow,
                    tblCurrency_editedBy = strBy,
                    tblCurrency_deletedUTC = null,
                    tblCurrency_deletedBy = ""
                };

                _ = dB.tblCurrencies.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Currency).ToString(), "CreateAsync(string)", ex,
                    "strBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public async Task<bool> SaveAsync(string strBy = "")
        {
            if (!Loaded) { return false; }

            if (string.IsNullOrWhiteSpace(Name)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblCurrencies.FirstOrDefaultAsync(r => r.tblCurrency_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblCurrency_name = Name;
                    efmObject.tblCurrency_code = Code.Trim().ToUpper();
                    efmObject.tblCurrency_symbol = Symbol;
                    efmObject.tblCurrency_symbolAfter = SymbolAfter;

                    efmObject.tblCurrency_editedUTC = DateTime.UtcNow;
                    efmObject.tblCurrency_editedBy = strBy;

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
                _ = Error.Exception(typeof(Currency).ToString(), "SaveAsync(string)", ex,
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

                var efmObject = await dB.tblCurrencies.FirstOrDefaultAsync(r => r.tblCurrency_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblCurrency_deletedUTC.HasValue)
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
                            iOrder = efmObject.tblCurrency_order;

                            efmObject.tblCurrency_order = 0;
                            efmObject.tblCurrency_deletedUTC = DateTime.UtcNow;
                            efmObject.tblCurrency_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblCurrency_order = await dB.tblCurrencies.AsNoTracking().CountAsync(r => r.tblCurrency_deletedUTC == null) + 1;
                            efmObject.tblCurrency_deletedUTC = null;
                            efmObject.tblCurrency_deletedBy = "";
                        }

                        efmObject.tblCurrency_editedUTC = DateTime.UtcNow;
                        efmObject.tblCurrency_editedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            if (bDeleted)
                            {
                                // reorder the elements after it
                                _ = await dB.tblCurrencies.Where(r => r.tblCurrency_order > iOrder &&
                                                                        r.tblCurrency_deletedUTC == null)
                                                            .UpdateAsync(r => new tblCurrency { tblCurrency_order = r.tblCurrency_order - 1 });
                            }

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
                _ = Error.Exception(typeof(Currency).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
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

                var efmObject = await dB.tblCurrencies.FirstOrDefaultAsync(r => r.tblCurrency_id == iId);

                if (efmObject != null)
                {
                    // record the order so we can reorder elements after it
                    var iOrder = efmObject.tblCurrency_order;

                    // Delete related elements (i.e those that ref this)


                    // delete the element
                    _ = dB.tblCurrencies.Remove(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

                        // reorder the elements after it
                        _ = await dB.tblCurrencies.Where(r => r.tblCurrency_order > iOrder &&
                                                                r.tblCurrency_deletedUTC == null)
                                                    .UpdateAsync(r => new tblCurrency { tblCurrency_order = r.tblCurrency_order - 1 });

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
                _ = Error.Exception(typeof(Currency).ToString(), "DeleteFullAsync(int, string, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bClearCache: " + bClearCache.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Default_SetAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblCurrencies.FirstOrDefaultAsync(r => r.tblCurrency_id == iId);

                if (efmObject != null)
                {
                    if (efmObject.tblCurrency_deletedUTC == null)
                    {
                        if (efmObject.tblCurrency_default)
                        {
                            bReturn = true;
                        }
                        else
                        {
                            _ = await dB.tblCurrencies.Where(r => r.tblCurrency_default)
                                                        .UpdateAsync(r => new tblCurrency { tblCurrency_default = false });

                            efmObject.tblCurrency_default = true;

                            if (await dB.SaveChangesAsync() > 0)
                            {
                                bReturn = true;
                                LocalCache.RefreshCache(CacheKey);
                            }
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Currency).ToString(), "Default_SetAsync(int)", ex,
                    "iId: " + iId.ToString());
                return false;
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

                // get the item to move up
                var efmMoveUp = await dB.tblCurrencies.FirstOrDefaultAsync(r => r.tblCurrency_id == iId);

                if (efmMoveUp == null) { return false; }

                // calculate new positions
                iOldPosition = efmMoveUp.tblCurrency_order;
                iNewPosition = iOldPosition - 1;

                // check new position is not above position 1 (i.e top)
                if (iNewPosition < 1)
                {
                    // the item is already at the top and so can go no higher
                    efmMoveUp = null;
                    return true;
                }

                // get the item to switch position with (i.e move down)
                var efmMoveDown = await dB.tblCurrencies.FirstOrDefaultAsync(r => r.tblCurrency_order == iNewPosition);

                if (efmMoveDown == null) { return false; }

                // switch the order positions of the two items
                efmMoveUp.tblCurrency_order = iNewPosition;
                efmMoveDown.tblCurrency_order = iOldPosition;

                if (await dB.SaveChangesAsync() > 0)
                {
                    // at least one change made and no exception, assume both changes saved successfully
                    bReturn = true;

                    LocalCache.RefreshCache(CacheKey);
                }

                efmMoveUp = null;
                efmMoveDown = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Currency).ToString(), "Move_UpAsync(int)", ex,
                    "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        public static async Task<bool> Move_DownAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                // variables to remember positions
                var iOldPosition = 0;
                var iNewPosition = 0;

                var efmMoveDown = await dB.tblCurrencies.FirstOrDefaultAsync(r => r.tblCurrency_id == iId);

                if (efmMoveDown == null) { return false; }

                // calculate new positions
                iOldPosition = efmMoveDown.tblCurrency_order;
                iNewPosition = iOldPosition + 1;

                // check new position is not below the last item (i.e bottom)
                if (iNewPosition > (await dB.tblCurrencies.AsNoTracking().CountAsync(r => r.tblCurrency_deletedUTC == null)))
                {
                    // the item is already at the bottom and so can go no lower
                    efmMoveDown = null;
                    return true;
                }

                // get the item to switch position with (i.e move up)
                var efmMoveUp = await dB.tblCurrencies.FirstOrDefaultAsync(r => r.tblCurrency_order == iNewPosition);

                if (efmMoveUp == null) { return false; }

                // switch the order positions of the two items
                efmMoveUp.tblCurrency_order = iOldPosition;
                efmMoveDown.tblCurrency_order = iNewPosition;

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
                _ = Error.Exception(typeof(Currency).ToString(), "Move_DownAsync(int)", ex,
                    "iId: " + iId.ToString());
                return false;
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

                var efmObject = await dB.tblCurrencies.FirstOrDefaultAsync(r => r.tblCurrency_id == iId);

                if (efmObject != null)
                {
                    // get the items to reorder
                    var lstToReOrder = await dB.tblCurrencies.Where(r => r.tblCurrency_deletedUTC == null)
                                                                .OrderBy(r => r.tblCurrency_order)
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

                            if (efmTmpObj.tblCurrency_id == iId)
                            {
                                efmTmpObj.tblCurrency_order = iPosition;
                            }
                            else
                            {
                                efmTmpObj.tblCurrency_order = iNewPosition;
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
                _ = Error.Exception(typeof(Currency).ToString(), "Move_ToPositionAsync(int, int)", ex,
                    "iId: " + iId.ToString() +
                    ", iPosition: " + iPosition.ToString());
                return false;
            }

            return bReturn;
        }

        public static async Task<string> FormatAsync(int iId, string strValue)
        {
            return Format(await FindAsync(iId), strValue);
        }

        //public static string Format(string strCode, string strValue)
        //{
        //    string strReturn = strValue;

        //    var obj = Find(strCode);
        //    strReturn = Format(obj, strValue);
        //    obj = null;

        //    return strReturn;
        //}

        public static string Format(Currency objCurrency, string strValue)
        {
            var strReturn = strValue;

            if (objCurrency.Loaded)
            {
                if (objCurrency.SymbolAfter)
                {
                    strReturn += objCurrency.Symbol;
                }
                else
                {
                    strReturn = objCurrency.Symbol + strReturn;
                }
            }

            return strReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Currency> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Currency(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Currency();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Currency();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<List<Currency>> FindAllAsync(bool bIncDeleted = false)
        {
            var lstReturn = new List<Currency>();

            foreach (var obj in (await Global_ListAsync()).Values)
            {
                if (!bIncDeleted && obj.Deleted_UTC.HasValue) { continue; }

                lstReturn.Add(obj);
            }

            lstReturn = lstReturn.OrderBy(r => r.Order).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Currency>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Currency> lstElements)
            {
                lstElements = new Dictionary<int, Currency>();

                try
                {
                    await s_singleCacheBuildLock.WaitAsync();

                    // attempt to reget incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Currency> lstElementsRetry)
                    {
                        return lstElementsRetry;
                    }

                    using var dB = Settings.Config.DBPooledConnection();

                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var efmObject in await dB.tblCurrencies.AsNoTracking().ToListAsync())
                    {
                        var obj = new Currency(efmObject);

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
                    _ = Error.Exception(typeof(Currency).ToString(), "Global_ListAsync()", ex);
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
