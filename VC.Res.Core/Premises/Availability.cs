using System.Collections.Concurrent;
using System.Data;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Premises
{
    public class Availability : Bases.BasicCached
    {
        private const LocalCache.Key CacheKey = LocalCache.Key.Premises_Availability;

        #region Properties

        public int Premise_Id { get; private set; } = 0;

        public DateTime Night { get; private set; } = DateTime.Today.Date;

        public Enums.Premises_Premise_Availability State { get; private set; } = Enums.Premises_Premise_Availability.Unknown;

        public string Note { get; set; } = "";

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;
        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;
        public string Edited_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        private Availability() { }

        private Availability(tblPropertyAvailability efmObject) { _ = Load(efmObject);}

        #endregion constructors


        #region Private Properties

        protected override async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblPropertyAvailabilities.AsNoTracking().FirstOrDefaultAsync(r => r.tblPropertyAvailability_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Availability).ToString(), "LoadAsync(int)", ex,
                    "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblPropertyAvailability efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblPropertyAvailability_id;

                    Premise_Id = efmObject.tblProperty_id;

                    Night = efmObject.tblPropertyAvailability_night;
                    State = (Enums.Premises_Premise_Availability)efmObject.tblPropertyAvailability_state;
                    Note = efmObject.tblPropertyAvailability_note;

                    Created_UTC = efmObject.tblPropertyAvailability_createdUTC;
                    Created_By = efmObject.tblPropertyAvailability_createdBy;

                    Edited_UTC = efmObject.tblPropertyAvailability_editedUTC;
                    Edited_By = efmObject.tblPropertyAvailability_editedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Availability).ToString(), "Load(tblPropertyAvailability)", ex);
                return bReturn;
            }

            return bReturn;
        }

        #endregion private properties


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        public async Task<bool> SaveAsync(string strBy)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblPropertyAvailabilities.FirstOrDefaultAsync(r => r.tblPropertyAvailability_id == Id);

                if (efmObject != null)
                {
                    efmObject.tblPropertyAvailability_note = Note;

                    efmObject.tblPropertyAvailability_editedUTC = DateTime.UtcNow;
                    efmObject.tblPropertyAvailability_editedBy = strBy;

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
                _ = Error.Exception(typeof(Availability).ToString(), "SaveAsync(string)", ex,
                    "Id: " + Id.ToString() +
                    ", strBy: " + strBy.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> UpdateAsync(int iPremiseId, Enums.Premises_Premise_Availability enumState, DateTime dtFrom, DateTime dtTo, string strNote = "", string strBy = "")
        {
            var bReturn = false;

            if (dtTo < DateTime.UtcNow.Date)
            {
                // updating the past
                return false;
            }

            var dtFromToUse = dtFrom.Date;
            var dtToToUse = dtTo.Date;

            try
            {
                using var dB = Settings.Config.DBConnection();

                // get database rows for the date range
                var lstDBRows = dB.tblPropertyAvailabilities.Where(r => r.tblProperty_id == iPremiseId && r.tblPropertyAvailability_night >= dtFromToUse && r.tblPropertyAvailability_night <= dtToToUse).ToList();

                // get the premise default state (only changable up to premise level, not season)
                var enumDefaultState = (await Config.FindBy_PremiseAsync(iPremiseId)).DefaultAvailability_Calculated;

                // go through each day
                while (dtFromToUse <= dtToToUse)
                {
                    if (enumState == enumDefaultState)
                    {
                        // setting to default state, don't want a row
                        var efmExisting = lstDBRows.FirstOrDefault(r => r.tblPropertyAvailability_night == dtFromToUse);
                        if (efmExisting != null)
                        {
                            _ = dB.tblPropertyAvailabilities.Remove(efmExisting);
                        }
                    }
                    else
                    {
                        // setting to something that isn't default, i.e overriding
                        var efmExisting = lstDBRows.FirstOrDefault(r => r.tblPropertyAvailability_night == dtFromToUse);
                        if (efmExisting != null)
                        {
                            // existing record exists, update if required
                            var bChanged = false;

                            if (efmExisting.tblPropertyAvailability_state != (int)enumState)
                            {
                                efmExisting.tblPropertyAvailability_state = (int)enumState;
                                bChanged = true;
                            }

                            if (efmExisting.tblPropertyAvailability_note != strNote)
                            {
                                efmExisting.tblPropertyAvailability_note = strNote;
                                bChanged = true;
                            }

                            if (bChanged)
                            {
                                efmExisting.tblPropertyAvailability_editedUTC = DateTime.UtcNow;
                                efmExisting.tblPropertyAvailability_editedBy = strBy;
                            }
                        }
                        else
                        {
                            // no record, need to create
                            var efmNew = new tblPropertyAvailability
                            {
                                tblProperty_id = iPremiseId,
                                tblPropertyAvailability_night = dtFromToUse,
                                tblPropertyAvailability_state = (int)enumState,
                                tblPropertyAvailability_note = strNote,
                                tblPropertyAvailability_createdUTC = DateTime.UtcNow,
                                tblPropertyAvailability_createdBy = strBy,
                                tblPropertyAvailability_editedUTC = DateTime.UtcNow,
                                tblPropertyAvailability_editedBy = strBy
                            };

                            _ = dB.tblPropertyAvailabilities.Add(efmNew);
                        }
                    }

                    dtFromToUse = dtFromToUse.AddDays(1);
                }

                // save any changes to the context
                if (await dB.SaveChangesAsync() > 0)
                {
                    // changes made so clear the cache
                    LocalCache.RefreshCache(CacheKey);
                }

                // processing completed successfully without error
                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Availability).ToString(), "Status_UpdateAsync(int, Enums.Premises_Premise_Availability, DateTime, DateTime, string, string)", ex,
                       "iPropertyId: " + iPremiseId.ToString() +
                        ", enumState: " + ((int)enumState).ToString() +
                        ", dtFrom: " + dtFrom.ToString() +
                        ", dtTo: " + dtTo.ToString() +
                        ", strNote: " + strNote.ToString() +
                        ", strEditedBy: " + strBy.ToString());
                return bReturn;
            }

            return bReturn;
        }

        //public static async Task<Enums.Premises_Premise_Availability> Lookup_DefaultAsync(int iPremiseId, DateTime dtNight)
        //{
        //    var enumReturn = Enums.Premises_Premise_Availability.Unknown;

        //    try
        //    {
        //        // find out the season for the premise on the given date

        //        // if season found, get the config for the season and use its default state

        //        // if not season found, get the config for the premise and use the default state from that
        //        enumReturn = (await Config.FindBy_PremiseAsync(iPremiseId)).DefaultAvailability_Calculated;
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = Error.Exception(typeof(Availability).ToString(), "Lookup_DefaultAsync(int, DateTime)", ex,
        //            "iPremiseId: " + iPremiseId.ToString() +
        //            "dtNight: " + dtNight.ToString("dd/MM/yyyy"));

        //        return enumReturn;
        //    }

        //    return enumReturn;
        //}

        public static async Task<bool> Lookup_AvailableAsync(int iPremiseId, DateTime dtStart, int iNights)
        {
            return await Lookup_AvailableAsync(iPremiseId, dtStart, dtStart.AddDays(iNights - 1));
        }

        public static async Task<bool> Lookup_AvailableAsync(int iPremiseId, DateTime dtStart, DateTime dtEnd)
        {
            return (await FindAllBy_PremiseDatesAsync(iPremiseId, dtStart, dtEnd)).All(r => r.State == Enums.Premises_Premise_Availability.Available || r.State == Enums.Premises_Premise_Availability.AvailableEnquire);
        }

        #endregion public functions


        #region Finders

        public static async Task<Availability> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new Availability(); }

            if (bUseCache)
            {
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new Availability();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new Availability();

                _ = await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        //public static async Task<Availability> FindBy_PremiseDateAsync(int iPremiseId, DateTime dtNight)
        //{
        //    var lstGlobal = await Global_ListAsync();

        //    if (LocalCache.Get(LocalCache.Key.Premises_Availability_IdxPremiseDate) is Dictionary<string, int> lstIndex)
        //    {
        //        if (lstIndex.ContainsKey(string.Format("{0}-{1}", iPremiseId, dtNight.ToString("yyyyMMdd"))))
        //        {
        //            return lstGlobal[lstIndex[string.Format("{0}-{1}", iPremiseId, dtNight.ToString("yyyyMMdd"))]];
        //        }
        //    }

        //    lstGlobal = null;

        //    // if got to here then the night has not been overridden with a custom entry, return default state
        //    //var enumDefaultState = await Lookup_DefaultAsync(iPremiseId, dtNight);
        //    return new Availability
        //    {
        //        Premise_Id = iPremiseId,
        //        Night = dtNight.Date,
        //        State = await Lookup_DefaultAsync(iPremiseId, dtNight)
        //    };
        //}

        public static async Task<List<Availability>> FindAllBy_PremiseDatesAsync(int iPremiseId, DateTime dtStart, DateTime dtEnd)
        {
            // create list of dates to cycle/lookup
            var lstDates = new List<DateTime>();
            var dtCurrentDate = dtStart.Date;
            while (dtCurrentDate <= dtEnd.Date)
            {
                lstDates.Add(dtCurrentDate);
                dtCurrentDate = dtCurrentDate.AddDays(1);
            }

            // get the premise default state (only changable up to premise level, not season)
            //Enums.Premises_Premise_Availability.AvailableEnquire;
            var enumDefaultState = (await Config.FindBy_PremiseAsync(iPremiseId)).DefaultAvailability_Calculated;

            var lstGlobal = await Global_ListAsync();

            // get index
            if (LocalCache.Get(LocalCache.Key.Premises_Availability_IdxPremiseDate) is not Dictionary<string, int> lstIndex)
            {
                lstIndex = new Dictionary<string, int>();
            }

            var lstReturn = new List<Availability>();

            for (var i = 0; i < lstDates.Count; i++)
            {
                if (lstIndex.TryGetValue(string.Format("{0}-{1}", iPremiseId, lstDates[i].ToString("yyyyMMdd")), out var value))
                {
                    lstReturn.Add(lstGlobal[value]);
                }
                else
                {
                    lstReturn.Add(new Availability
                    {
                        Premise_Id = iPremiseId,
                        Night = lstDates[i],
                        State = enumDefaultState
                    });
                }
            }

            lstGlobal = null;

            return lstReturn;
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim s_singleCacheBuildLock = new(1, 1);

        private static async Task<Dictionary<int, Availability>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, Availability> lstElements ||
                LocalCache.Get(LocalCache.Key.Premises_Availability_IdxPremiseDate) is not Dictionary<string, int> dicIndexPremiseDate)
            {
                lstElements = new Dictionary<int, Availability>();

                try
                {
                    await s_singleCacheBuildLock.WaitAsync();

                    // attempt to reget incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, Availability> lstElementsRetry)
                    {
                        return lstElementsRetry;
                    }

                    dicIndexPremiseDate = new Dictionary<string, int>();

                    using var dB = Settings.Config.DBPooledConnection();

                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var efmObject in await dB.tblPropertyAvailabilities.AsNoTracking().ToListAsync())
                    {
                        var obj = new Availability(efmObject);

                        if (obj.Loaded)
                        {
                            lstElements.Add(obj.Id, obj);

                            if (!dicIndexPremiseDate.ContainsKey(string.Format("{0}-{1}", obj.Premise_Id, obj.Night.ToString("yyyyMMdd"))))
                            {
                                dicIndexPremiseDate.Add(string.Format("{0}-{1}", obj.Premise_Id, obj.Night.ToString("yyyyMMdd")), obj.Id);
                            }
                        }

                        obj = null;
                    }

                    // add the found elements to the cache
                    LocalCache.Set(CacheKey, lstElements);

                    LocalCache.Set(LocalCache.Key.Premises_Availability_IdxPremiseDate, dicIndexPremiseDate);
                }
                catch (Exception ex)
                {
                    _ = Error.Exception(typeof(Availability).ToString(), "Global_ListAsync()", ex);
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
