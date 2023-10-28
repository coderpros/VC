using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;

namespace VC.Res.Core.Premises.Seasons
{
    public class Season : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Seasons_Seasons;

        #region Properties

        public int Premise_Id { get; private set; } = 0;

        public string Name { get; set; } = "";

        public string Note_Internal { get; set; } = "";

        public List<Date> Dates { get; set; } = new List<Date>();

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string Edited_By { get; private set; } = "";

        public DateTime? Deleted_UTC { get; private set; } = null;
        public string Deleted_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public Season() { }

        private Season(tblPropertySeason efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Properties

        #region Private Properties-Loaders

        protected override async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblPropertySeasons.AsNoTracking().Include(r => r.tblPropertySeasonDates).FirstOrDefaultAsync(r => r.tblPropertySeason_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Season).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblPropertySeason efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertySeason_id;

                    Premise_Id = efmObject.tblProperty_id;

                    Name = efmObject.tblPropertySeason_name;
                    Note_Internal = string.IsNullOrWhiteSpace(efmObject.tblPropertySeason_noteInt) ? "" : efmObject.tblPropertySeason_noteInt;

                    Dates = new List<Date>();
                    foreach (var vItem in efmObject.tblPropertySeasonDates.OrderBy(r => r.tblPropertySeasonDate_from))
                    {
                        Dates.Add(new Date(vItem));
                    }

                    Created_UTC = efmObject.tblPropertySeason_createdUTC;
                    Created_By = efmObject.tblPropertySeason_createdBy;

                    Edited_UTC = efmObject.tblPropertySeason_editedUTC;
                    Edited_By = efmObject.tblPropertySeason_editedBy;

                    Deleted_UTC = efmObject.tblPropertySeason_deletedUTC;
                    Deleted_By = efmObject.tblPropertySeason_deletedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(GetType().ToString(), "Load(tblPropertySeason)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private properties-loaders



        #endregion private properties


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        public async Task<List<string>> ValidateAsync(int iPremiseId)
        {
            var lstReturn = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
            {
                lstReturn.Add("Please enter a name for the season.");
            }

            if (Dates.Count < 1) { lstReturn.Add("Please enter one or more date ranges for the season."); }

            if (Dates.Count > 0)
            {
                // ensure all the dates are set to date start/ends
                foreach (var vDate in Dates)
                {
                    vDate.Start = vDate.Start.Date;
                    vDate.End = vDate.End.Date;

                    if (vDate.Start >= vDate.End) { lstReturn.Add("Please ensure the date end is after the start."); }
                }

                // ensure no duplicates
                var bDuplicateDates = false;
                foreach (var vDate in Dates)
                {
                    if (Dates.Count(r => r.Start == vDate.Start && r.End == vDate.End) > 1)
                    {
                        bDuplicateDates = true;
                    }
                }
                if (bDuplicateDates) { lstReturn.Add("Please enter none duplicate date ranges."); }

                // ensure they do not overlap with each other
                var bOverlapDates = false;
                foreach (var vDate in Dates)
                {
                    // we've already checked for duplicates, so excluding rows where specifically start and end do not match will exclude
                    // the row we are wanting to check against the other rows
                    if (Dates.Any(r => r.Start != vDate.Start && r.End != vDate.End &&
                                        ((r.Start >= vDate.Start && r.Start <= vDate.End) || (r.End >= vDate.Start && r.End <= vDate.End))))
                    {
                        bOverlapDates = true;
                    }
                }
                if (bOverlapDates) { lstReturn.Add("Entered date ranges overlap."); }

                if (lstReturn.Count < 1)
                {
                    // no errors so far, run more intensive checks on database
                    try
                    {
                        var dB = Settings.Config.DBPooledConnection();

                        dB.ChangeTracker.AutoDetectChangesEnabled = false;

                        // run check for overlaps in other seasons from db
                        var bDBOverlapDates = false;
                        foreach (var vDate in Dates)
                        {
                            if (await dB.tblPropertySeasonDates.AnyAsync(r => r.tblPropertySeason_id != Id &&
                                                                                r.tblPropertySeason.tblProperty_id == iPremiseId &&
                                                                                ((r.tblPropertySeasonDate_from >= vDate.Start && r.tblPropertySeasonDate_from <= vDate.End) || (r.tblPropertySeasonDate_to >= vDate.Start && r.tblPropertySeasonDate_to <= vDate.End)) &&
                                                                                r.tblPropertySeason.tblPropertySeason_deletedUTC == null))
                            {
                                bDBOverlapDates = true;
                            }
                        }
                        if (bDBOverlapDates) { lstReturn.Add("Selected dates overlap with another season."); }
                    }
                    catch (Exception ex)
                    {
                        _ = Error.Exception(typeof(Season).ToString(), "ValidateAsync(int)", ex);
                        return lstReturn;
                    }
                }
            }

            return lstReturn;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "<Pending>")]
        public async Task<bool> RefreshAsync() { return Loaded ? await LoadAsync(Id) : false; }

        public async Task<bool> CreateAsync(int iPremiseId, string strBy, int? iSourceSeasonId = null)
        {
            if (Loaded || iPremiseId <= 0) { return false; }

            Premise_Id = iPremiseId;

            if ((await ValidateAsync(iPremiseId)).Count > 0) { return false; }

            var objSourceSeason = new Season();
            if (iSourceSeasonId.HasValue)
            {
                objSourceSeason = await FindAsync(iSourceSeasonId.Value);

                if (!objSourceSeason.Loaded || objSourceSeason.Premise_Id != iPremiseId)
                {
                    return false;
                }
            }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblPropertySeason
                {
                    tblProperty_id = iPremiseId,
                    tblPropertySeason_name = Name.Trim(),
                    tblPropertySeason_noteInt = Note_Internal,
                    tblPropertySeason_createdUTC = DateTime.UtcNow,
                    tblPropertySeason_createdBy = strBy,
                    tblPropertySeason_editedUTC = DateTime.UtcNow,
                    tblPropertySeason_editedBy = strBy,
                    tblPropertySeason_deletedUTC = null,
                    tblPropertySeason_deletedBy = ""
                };

                // add dates
                foreach (var vDate in Dates)
                {
                    efmObject.tblPropertySeasonDates.Add(new tblPropertySeasonDate
                    {
                        tblPropertySeasonDate_from = vDate.Start.Date,
                        tblPropertySeasonDate_to = vDate.End.Date,
                        tblPropertySeasonDate_createdUTC = DateTime.UtcNow,
                        tblPropertySeasonDate_createdBy = strBy,
                        tblPropertySeasonDate_editedUTC = DateTime.UtcNow,
                        tblPropertySeasonDate_editedBy = strBy
                    });
                }

                _ = dB.tblPropertySeasons.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);

                    // create config (the find will automatically create one)
                    var objSeasonConfig = await Config.FindBy_SeasonAsync(Id, bUseCache: false);

                    if (iSourceSeasonId.HasValue)
                    {
                        // Copy Config
                        _ = await objSeasonConfig.CloneAsync(await Config.FindBy_SeasonAsync(iSourceSeasonId.Value), strBy);

                        // Clone the extras
                        var objCloneExtras = Extra.CloneAsync(iSourceSeasonId.Value, Id, strBy);

                        // wait for the cloning tasks to complete
                        await objCloneExtras;
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Season).ToString(), "CreateAsync(int, string, int?)", ex,
                    "iPremiseId: " + iPremiseId.ToString() +
                    ", strBy: " + strBy +
                    ", iSourceSeasonId: " + (iSourceSeasonId.HasValue ? iSourceSeasonId.Value.ToString() : ""));

                return bReturn;
            }

            return bReturn;
        }

        public async Task<bool> SaveAsync(string strBy)
        {
            if (!Loaded) { return false; }

            // in theory most things can be changed as long as validation is re done
            if ((await ValidateAsync(Premise_Id)).Count > 0) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertySeasons.Include(r => r.tblPropertySeasonDates).FirstOrDefaultAsync(r => r.tblPropertySeason_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblPropertySeason_name = Name.Trim();
                    efmObject.tblPropertySeason_noteInt = Note_Internal;

                    efmObject.tblPropertySeason_editedUTC = DateTime.UtcNow;
                    efmObject.tblPropertySeason_editedBy = strBy;

                    // sync/match up dates
                    // loop over db rows to delete any no longer wanted
                    var lstDatesToDelete = new List<tblPropertySeasonDate>();
                    foreach (var vDateRow in efmObject.tblPropertySeasonDates)
                    {
                        if (!Dates.Any(r => r.Id == vDateRow.tblPropertySeasonDate_id))
                        {
                            lstDatesToDelete.Add(vDateRow);
                        }
                    }
                    foreach (var vDateRowToDelete in lstDatesToDelete)
                    {
                        _ = dB.tblPropertySeasonDates.Remove(vDateRowToDelete);
                    }
                    lstDatesToDelete = null;

                    foreach (var vDate in Dates)
                    {
                        if (vDate.Id == 0)
                        {
                            // new date
                            efmObject.tblPropertySeasonDates.Add(new tblPropertySeasonDate
                            {
                                tblPropertySeasonDate_from = vDate.Start.Date,
                                tblPropertySeasonDate_to = vDate.End.Date,
                                tblPropertySeasonDate_createdUTC = DateTime.UtcNow,
                                tblPropertySeasonDate_createdBy = strBy,
                                tblPropertySeasonDate_editedUTC = DateTime.UtcNow,
                                tblPropertySeasonDate_editedBy = strBy
                            });
                        }
                        else
                        {
                            // change exist
                            var efmObjectDate = efmObject.tblPropertySeasonDates.FirstOrDefault(r => r.tblPropertySeasonDate_id == vDate.Id);

                            if (efmObjectDate != null)
                            {
                                var bChanges = false;

                                if (efmObjectDate.tblPropertySeasonDate_from != vDate.Start.Date)
                                {
                                    efmObjectDate.tblPropertySeasonDate_from = vDate.Start.Date;
                                    bChanges = true;
                                }

                                if (efmObjectDate.tblPropertySeasonDate_to != vDate.End.Date)
                                {
                                    efmObjectDate.tblPropertySeasonDate_to = vDate.End.Date;
                                    bChanges = true;
                                }

                                if (bChanges)
                                {
                                    efmObjectDate.tblPropertySeasonDate_editedUTC = DateTime.UtcNow;
                                    efmObjectDate.tblPropertySeasonDate_editedBy = strBy;
                                }
                            }
                        }
                    }

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
                _ = Error.Exception(typeof(Season).ToString(), "SaveAsync(string)", ex,
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

                var efmObject = await dB.tblPropertySeasons.FirstOrDefaultAsync(r => r.tblPropertySeason_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblPropertySeason_deletedUTC.HasValue)
                    {
                        // already in desired state
                        bReturn = true;
                    }
                    else
                    {
                        if (bDeleted)
                        {
                            efmObject.tblPropertySeason_deletedUTC = DateTime.UtcNow;
                            efmObject.tblPropertySeason_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblPropertySeason_deletedUTC = null;
                            efmObject.tblPropertySeason_deletedBy = "";
                        }

                        efmObject.tblPropertySeason_editedUTC = DateTime.UtcNow;
                        efmObject.tblPropertySeason_editedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }

                            if (bDeleted)
                            {
                                // need to soft delete all rates and extras for the season so they don't get used
                                _ = await Rate.DeleteBy_SeasonAsync(iId, strBy);
                                _ = await Extra.DeleteBy_SeasonAsync(iId, strBy);
                            }
                        }
                    }
                }
                else
                {
                    // not found, probably fully deleted, only return success if wanted deleted
                    if (bDeleted)
                    {
                        bReturn = true;
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Season).ToString(), "DeleteAsync(int, string, bool, bool)", ex,
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
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertySeasons.FirstOrDefaultAsync(r => r.tblPropertySeason_id == iId);

                if (efmObject != null)
                {
                    // can only fully delete if already flagged for deletion
                    if (efmObject.tblPropertySeason_deletedUTC != null)
                    {
                        // can only delete if flagged more than short recycle bin period days ago or this is a force action
                        if (efmObject.tblPropertySeason_deletedUTC < DateTime.UtcNow.AddDays(-Settings.Variables.RecycleBin_EmptyAfterDaysShort) || bForce)
                        {
                            // Full delete items that relate to the season
                            _ = await Rate.DeleteFullBy_SeasonAsync(iId, strBy);
                            _ = await Extra.DeleteFullBy_SeasonAsync(iId, strBy);
                            _ = await Date.DeleteFullBy_SeasonAsync(iId, strBy);

                            _ = await Config.DeleteFullBy_SeasonAsync(iId, strBy);

                            _ = dB.tblPropertySeasons.Remove(efmObject);

                            if (await dB.SaveChangesAsync() > 0)
                            {
                                bReturn = true;

                                if (bClearCache) { LocalCache.RefreshCache(CacheKey); }
                            }
                        }
                    }
                }
                else
                {
                    // already fully deleted as can't find
                    bReturn = true;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Season).ToString(), "DeleteFullAsync(int, string, bool, bool)", ex,
                    "iId: " + iId.ToString() +
                    ", strBy: " + strBy.ToString() +
                    ", bClearCache: " + bClearCache.ToString() +
                    ", bForce: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Season> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Season(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Season();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Season();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<Season> FindBy_PremiseDateAsync(int iPremiseId, DateTime dtDate)
        {
            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Seasons_Seasons_IdxPremise) is Dictionary<int, List<int>> lstIndex)
            {
                if (lstIndex.TryGetValue(iPremiseId, out var value))
                {
                    foreach (var iId in value)
                    {
                        if (lstGlobal[iId].Deleted_UTC.HasValue) { continue; }

                        var bDateMatch = false;
                        foreach (var vDate in lstGlobal[iId].Dates)
                        {
                            if (vDate.Start > dtDate.Date) { continue; }

                            if (vDate.End < dtDate.Date) { continue; }

                            bDateMatch = true;
                        }
                        if (!bDateMatch) { continue; }

                        return lstGlobal[iId];
                    }

                }
            }

            lstGlobal = null;

            // no match found
            return new Season();
        }

        public static async Task<List<Season>> FindAllAsync(List<int> iIds)
        {
            var lstReturn = new List<Season>();

            // get global list
            var lstGlobal = await Global_ListAsync();

            foreach (var iId in iIds)
            {
                if (lstGlobal.TryGetValue(iId, out var value))
                {
                    lstReturn.Add(value);
                }
            }

            lstGlobal = null;

            return lstReturn;
        }

        public static async Task<List<Season>> FindAllBy_DateAsync(DateTime dtDate)
        {
            var lstReturn = new List<Season>();

            // get global list
            var lstGlobal = await Global_ListAsync();

            foreach (var vSeason in lstGlobal.Values)
            {
                if (vSeason.Deleted_UTC.HasValue) { continue; }

                var bDateMatch = false;
                foreach (var vDate in vSeason.Dates)
                {
                    if (vDate.Start > dtDate.Date) { continue; }

                    if (vDate.End < dtDate.Date) { continue; }

                    bDateMatch = true;
                }
                if (!bDateMatch) { continue; }

                lstReturn.Add(vSeason);
            }

            lstGlobal = null;

            return lstReturn;
        }

        public static async Task<List<Season>> FindAllBy_PremiseAsync(int iPremiseId)
        {
            return await FindAllBy_PremiseAsync(new List<int> { iPremiseId });
        }

        public static async Task<List<Season>> FindAllBy_PremiseAsync(List<int> lstPremiseIds, DateTime? dtDate = null)
        {
            var lstReturn = new List<Season>();

            var lstGlobal = await Global_ListAsync();

            if (LocalCache.Get(LocalCache.Key.Premises_Seasons_Seasons_IdxPremise) is Dictionary<int, List<int>> lstIndex)
            {
                foreach (var iPremiseId in lstPremiseIds)
                {
                    if (lstIndex.TryGetValue(iPremiseId, out var value))
                    {
                        foreach (var iId in value)
                        {
                            if (lstGlobal[iId].Deleted_UTC.HasValue) { continue; }

                            if (dtDate.HasValue)
                            {
                                var bDateMatch = false;
                                foreach (var vDate in lstGlobal[iId].Dates)
                                {
                                    if (vDate.Start > dtDate.Value.Date) { continue; }

                                    if (vDate.End < dtDate.Value.Date) { continue; }

                                    bDateMatch = true;
                                }
                                if (!bDateMatch) { continue; }
                            }

                            lstReturn.Add(lstGlobal[iId]);
                        }
                    }
                }
            }

            lstGlobal = null;

            lstReturn = lstReturn.OrderByDescending(r => r.Dates.Min(r1 => (DateTime?)r1.Start)).ToList();

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Season>> Global_ListAsync()
        {
            // check to see if cache is populated (including indexes)
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Season> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Seasons_Seasons_IdxPremise) is not Dictionary<int, List<int>> dicIndexPremise)
            {
                // not populated, need to get items and cache
                lstElements = new Dictionary<int, Season>();

                try
                {
                    // lock that we are about to rebuild (prevent other threads from populating at same time)
                    // will only lock/complete when free (i.e another thread might have locked)
                    await s_singleCacheBuildLock.WaitAsync();

                    // lock now in place, attempt to re-get incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Season> lstElementsRetry)
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
                    foreach (var efmObject in await dB.tblPropertySeasons.AsNoTracking().Include(r => r.tblPropertySeasonDates).ToListAsync())
                    {
                        var obj = new Season(efmObject);

                        if (obj.Loaded)
                        {
                            // successfully populated/loaded info, add to global list using id as key
                            lstElements.Add(obj.Id, obj);

                            // add to premises index
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
                    LocalCache.Set(LocalCache.Key.Premises_Seasons_Seasons_IdxPremise, dicIndexPremise);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Season).ToString(), "Global_ListAsync()", ex);
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
