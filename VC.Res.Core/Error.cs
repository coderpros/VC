using LinqKit;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core
{
    public class Error
    {
        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public string Logger { get; private set; } = "";

        public string Level { get; private set; } = "";

        public int Priority { get; private set; } = 3;

        public string Class { get; private set; } = "";

        public string Method { get; private set; } = "";

        public string Parameters { get; private set; } = "";

        public string Message { get; private set; } = "";

        public string StackTrace { get; private set; } = "";

        public string InnerEx { get; private set; } = "";

        public string Other { get; private set; } = "";

        public DateTime Occurred_UTC { get; private set; } = DateTime.UtcNow;

        public DateTime? Dismissed_UTC { get; private set; } = null;

        public string Dismissed_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        private Error() { }

        private Error(int iId) { _ = Load(iId); }

        private Error(tblSysError efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private bool Load(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = dB.tblSysErrors.AsNoTracking().FirstOrDefault(r => r.tblSysError_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Exception(GetType().ToString(), "Load(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblSysError efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblSysError_id;

                    Logger = string.IsNullOrWhiteSpace(efmObject.tblSysError_logger) ? "" : efmObject.tblSysError_logger;
                    Level = string.IsNullOrWhiteSpace(efmObject.tblSysError_level) ? "" : efmObject.tblSysError_level;
                    Priority = efmObject.tblSysErorr_priority;
                    Class = string.IsNullOrWhiteSpace(efmObject.tblSysError_class) ? "" : efmObject.tblSysError_class;
                    Method = string.IsNullOrWhiteSpace(efmObject.tblSysError_method) ? "" : efmObject.tblSysError_method;
                    Parameters = string.IsNullOrWhiteSpace(efmObject.tblSysError_parameters) ? "" : efmObject.tblSysError_parameters;
                    Message = string.IsNullOrWhiteSpace(efmObject.tblSysError_message) ? "" : efmObject.tblSysError_message;
                    StackTrace = string.IsNullOrWhiteSpace(efmObject.tblSysError_stackTrace) ? "" : efmObject.tblSysError_stackTrace;
                    InnerEx = string.IsNullOrWhiteSpace(efmObject.tblSysError_innerEx) ? "" : efmObject.tblSysError_innerEx;
                    Other = string.IsNullOrWhiteSpace(efmObject.tblSysError_other) ? "" : efmObject.tblSysError_other;

                    Occurred_UTC = efmObject.tblSysError_occurredUTC;
                    Dismissed_UTC = efmObject.tblSysError_dismissedUTC;
                    Dismissed_By = string.IsNullOrWhiteSpace(efmObject.tblSysError_dismissedBy) ? "" : efmObject.tblSysError_dismissedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Exception(GetType().ToString(), "Load(tblSysError)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private functions-loaders

        private static bool Check_Ignore(Exception except)
        {
            var bReturn = false;

            try
            {
                if (!string.IsNullOrWhiteSpace(except.Message))
                {
                    if (except.Message.Trim() == "Thread was being aborted.")
                    {
                        // ignore the error
                        bReturn = true;
                    }

                    if (except.Message.Contains("owa/auth/logon.aspx"))
                    {
                        // ignore the error
                        bReturn = true;
                    }

                    if (except.Message.Contains("A potentially dangerous Request"))
                    {
                        // ignore the error
                        bReturn = true;
                    }
                }
            }
            catch (Exception)
            {

            }

            return bReturn;
        }

        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        public static bool Generic(string strClass, string strMethod, string strParameters, string strOther)
        {
            var bReturn = false;

            try
            {
                // connection to db
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // create new error record
                var eTemp = new tblSysError
                {
                    tblSysError_logger = "Core",
                    tblSysError_level = "Generic",
                    tblSysErorr_priority = 3,
                    tblSysError_class = strClass,
                    tblSysError_method = strMethod,
                    tblSysError_parameters = strParameters,
                    tblSysError_other = strOther,
                    tblSysError_occurredUTC = DateTime.UtcNow,
                    tblSysError_dismissedUTC = null,
                    tblSysError_dismissedBy = ""
                };

                // add and save the record
                _ = dB.tblSysErrors.Add(eTemp);

                if (dB.SaveChanges() > 0)
                {
                    bReturn = true;
                }
            }
            catch (Exception)
            {
                if (Settings.Variables.Errors_Throw) { throw; }

                return bReturn;
            }

            return bReturn;
        }

        public static bool Exception(string strClass, string strMethod, Exception ex, string strParameters = "None")
        {
            var bReturn = false;

            try
            {
                if (Check_Ignore(ex)) { return true; }

                // connection to db
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // create new error record
                var eTemp = new tblSysError
                {
                    tblSysError_logger = "Core",
                    tblSysError_level = "Exception",
                    tblSysErorr_priority = 4,
                    tblSysError_class = strClass,
                    tblSysError_method = strMethod,
                    tblSysError_parameters = strParameters,
                    tblSysError_message = ex.Message
                };

                if (ex.InnerException != null)
                {
                    eTemp.tblSysError_innerEx = ex.InnerException.ToString();
                }
                else
                {
                    eTemp.tblSysError_stackTrace = ex.StackTrace;
                }

                eTemp.tblSysError_occurredUTC = DateTime.UtcNow;

                eTemp.tblSysError_dismissedUTC = null;
                eTemp.tblSysError_dismissedBy = "";

                // add and save the record
                _ = dB.tblSysErrors.Add(eTemp);

                if (dB.SaveChanges() > 0)
                {
                    bReturn = true;
                }
            }
            catch (Exception)
            {
                if (Settings.Variables.Errors_Throw) { throw; }

                return bReturn;
            }

            return bReturn;
        }

        public static bool Dismiss(int iId, string strBy = "")
        {
            var bReturn = false;

            try
            {
                // connection to db
                using var dB = Settings.Config.DBPooledConnection();

                _ = dB.tblSysErrors.Where(r => r.tblSysError_id == iId && r.tblSysError_dismissedUTC == null)
                                    .Update(r => new tblSysError { tblSysError_dismissedUTC = DateTime.UtcNow, tblSysError_dismissedBy = strBy });

                bReturn = true;
            }
            catch (Exception)
            {
                if (Settings.Variables.Errors_Throw) { throw; }

                return bReturn;
            }

            return bReturn;
        }

        public static bool Dismiss_All(string strBy = "")
        {
            var bReturn = false;

            try
            {
                // connection to db
                using var dB = Settings.Config.DBPooledConnection();

                _ = dB.tblSysErrors.Where(r => r.tblSysError_dismissedUTC == null)
                                    .Update(r => new tblSysError { tblSysError_dismissedUTC = DateTime.UtcNow, tblSysError_dismissedBy = strBy });

                bReturn = true;
            }
            catch (Exception)
            {
                if (Settings.Variables.Errors_Throw) { throw; }

                return bReturn;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static Error Find(int iId)
        {
            return new Error(iId);
        }

        #endregion finders


        #region Lists

        public static List<Error> List_Latest(int iNoToGet = 20, bool bIncDismissed = false, int iMinPriority = 3)
        {
            var lstReturn = new List<Error>();

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var predicate = PredicateBuilder.New<tblSysError>(true);

                if (!bIncDismissed)
                {
                    predicate = predicate.And(r => r.tblSysError_dismissedUTC == null);
                }

                predicate = predicate.And(r => r.tblSysErorr_priority >= iMinPriority);

                foreach (var efmObject in dB.tblSysErrors.AsNoTracking().AsExpandable().Where(predicate).OrderByDescending(r => r.tblSysError_occurredUTC).Take(iNoToGet).ToList())
                {
                    var obj = new Error(efmObject);

                    if (obj.Loaded)
                    {
                        lstReturn.Add(obj);
                    }

                    obj = null;
                }
            }
            catch (Exception ex)
            {
                _ = Exception(typeof(Error).ToString(), "List_Latest(int, bool, int)", ex,
                    "iNoToGet: " + iNoToGet.ToString() +
                    ", bIncDismissed: " + bIncDismissed.ToString() +
                    ", iMinPriority: " + iMinPriority.ToString());
                return lstReturn;
            }

            return lstReturn;
        }

        #endregion lists
    }
}
