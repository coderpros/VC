using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using VC.Res.Core.Utilities;

namespace VC.Res.Core.Users
{
    public class Activity
    {
        internal static readonly string OrderByDefault = nameof(Created_UTC) + " DESC";
        internal static readonly string OrderByDefaultDB = nameof(tblUserActivity.tblUserActivity_createdUTC) + " DESC";

        public enum FilterOption
        {
            Ids,
            User_Id,
            User_Ids,
            Email,
            ActionGroup,
            ActionGroups,
            ActionType,
            ActionTypes,
            ActionText,
            Success,
            Date_Created_UTC,
            Date_Created_Local
        };


        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public int? User_Id { get; private set; } = null;

        public string EmailAddress { get; private set; } = "";

        public Enums.Users_Activity_ActionGroup Action_Group { get; private set; } = Enums.Users_Activity_ActionGroup.Unknown;

        public Enums.Users_Activity_ActionType Action_Type { get; private set; } = Enums.Users_Activity_ActionType.Unknown;

        public string Action_Text { get; private set; } = "";

        public int? ReferenceItem_Id1 { get; private set; } = null;
        public int? ReferenceItem_Id2 { get; private set; } = null;
        public int? ReferenceItem_Id3 { get; private set; } = null;
        public int? ReferenceItem_Id4 { get; private set; } = null;
        public int? ReferenceItem_Id5 { get; private set; } = null;

        public int? Audit_Id { get; private set; } = null;

        public bool Success { get; private set; } = false;

        public string Note { get; private set; } = "";

        public string IPAddress { get; private set; } = "";

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;

        public DateTime Created_Local { get; private set; } = DateTime.Now;

        #endregion properties


        #region Constructors

        private Activity() { }

        //private Activity(int iId) { _ = Load(iId); }

        private Activity(tblUserActivity efmObject) { _ = Load(efmObject); }

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

                var efmObject = await dB.tblUserActivities.AsNoTracking().FirstOrDefaultAsync(r => r.tblUserActivity_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Activity).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        private bool Load(tblUserActivity efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblUserActivity_id;

                    User_Id = efmObject.tblUser_id;
                    EmailAddress = efmObject.tblUserActivity_email;
                    IPAddress = efmObject.tblUserActivity_ipAddress;

                    Action_Group = (Enums.Users_Activity_ActionGroup)efmObject.tblUserActivity_actionGroup;
                    Action_Type = (Enums.Users_Activity_ActionType)efmObject.tblUserActivity_actionType;
                    Action_Text = efmObject.tblUserActivity_actionText;

                    ReferenceItem_Id1 = efmObject.tblUserActivity_refItemId1;
                    ReferenceItem_Id2 = efmObject.tblUserActivity_refItemId2;
                    ReferenceItem_Id3 = efmObject.tblUserActivity_refItemId3;
                    ReferenceItem_Id4 = efmObject.tblUserActivity_refItemId4;
                    ReferenceItem_Id5 = efmObject.tblUserActivity_refItemId5;
                    Audit_Id = efmObject.tblSysAudit_id;

                    Success = efmObject.tblUserActivity_success;
                    Note = efmObject.tblUserActivity_note;

                    Created_UTC = efmObject.tblUserActivity_createdUTC;
                    Created_Local = efmObject.tblUserActivity_createdLocal;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(tblUserActivity).ToString(), "Load(tblUserActivity)", ex);
                return bReturn;
            }

            return bReturn;
        }

        #endregion private functions-loaders



        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        public static async Task<bool> AuthenticationAsync(int? iUserId, string strEmail, Enums.Users_Activity_ActionType enumActionType, string strActionText, bool bSuccess, string strIP, string strNote = "")
        {
            // we need to know who the activity is logged against
            if (iUserId == null && string.IsNullOrWhiteSpace(strEmail)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = new tblUserActivity
                {
                    tblUser_id = iUserId,
                    tblUserActivity_email = strEmail.Trim().ToLower(),
                    tblUserActivity_actionGroup = (int)Enums.Users_Activity_ActionGroup.Authentication,
                    tblUserActivity_actionType = (int)enumActionType,
                    tblUserActivity_actionText = strActionText,
                    tblUserActivity_refItemId1 = null,
                    tblUserActivity_refItemId2 = null,
                    tblUserActivity_refItemId3 = null,
                    tblUserActivity_refItemId4 = null,
                    tblUserActivity_refItemId5 = null,
                    tblUserActivity_success = bSuccess,
                    tblUserActivity_note = strNote,
                    tblUserActivity_ipAddress = strIP,
                    tblUserActivity_createdUTC = DateTime.UtcNow,
                    tblUserActivity_createdLocal = DateTime.Now
                };

                _ = dB.tblUserActivities.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = true;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Activity).ToString(), "AuthenticationAsync(int?, string, Enums.Users_Activity_ActionType, string, bool, string, string)", ex,
                    "iUserId: " + (iUserId.HasValue ? iUserId.Value.ToString() : "") +
                    ", strEmail: " + strEmail.ToString() +
                    ", enumActionType: " + ((int)enumActionType).ToString() +
                    ", strActionText: " + strActionText.ToString() +
                    ", bSuccess: " + bSuccess.ToString() +
                    ", strNote: " + strNote.ToString() +
                    ", strIP: " + strIP.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> CreateAsync(string strEmail, Enums.Users_Activity_ActionGroup enumGroup, Enums.Users_Activity_ActionType enumType, string strActionText, int? iRefItemId1 = null, int? iRefItemId2 = null, int? iRefItemId3 = null, int? iRefItemId4 = null, int? iRefItemId5 = null, int? iAuditId = null, bool bSuccess = true, string strIP = "", string strNote = "")
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = new tblUserActivity
                {
                    tblUser_id = null,
                    tblUserActivity_email = strEmail,
                    tblUserActivity_actionGroup = (int)enumGroup,
                    tblUserActivity_actionType = (int)enumType,
                    tblUserActivity_actionText = strActionText,
                    tblUserActivity_refItemId1 = iRefItemId1,
                    tblUserActivity_refItemId2 = iRefItemId2,
                    tblUserActivity_refItemId3 = iRefItemId3,
                    tblUserActivity_refItemId4 = iRefItemId4,
                    tblUserActivity_refItemId5 = iRefItemId5,
                    tblSysAudit_id = iAuditId,
                    tblUserActivity_success = bSuccess,
                    tblUserActivity_note = strNote,
                    tblUserActivity_ipAddress = strIP,
                    tblUserActivity_createdUTC = DateTime.UtcNow,
                    tblUserActivity_createdLocal = DateTime.Now
                };

                _ = dB.tblUserActivities.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = true;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Activity).ToString(), "CreateAsync(string, Enums.Users_Activity_ActionGroup, Enums.Users_Activity_ActionType, string, int?, int?, int?, int?, int?, int?, bool, string, string)", ex,
                    "strEmail: " + strEmail.ToString() +
                    ", enumGroup: " + ((int)enumGroup).ToString() +
                    ", enumType: " + ((int)enumType).ToString() +
                    ", strActionText: " + strActionText.ToString() +
                    ", iRefItemId1: " + (iRefItemId1.HasValue ? iRefItemId1.Value.ToString() : "") +
                    ", iRefItemId2: " + (iRefItemId2.HasValue ? iRefItemId2.Value.ToString() : "") +
                    ", iRefItemId3: " + (iRefItemId3.HasValue ? iRefItemId3.Value.ToString() : "") +
                    ", iRefItemId4: " + (iRefItemId4.HasValue ? iRefItemId4.Value.ToString() : "") +
                    ", iRefItemId5: " + (iRefItemId5.HasValue ? iRefItemId5.Value.ToString() : "") +
                    ", iAuditId: " + (iAuditId.HasValue ? iAuditId.Value.ToString() : "") +
                    ", bSuccess: " + bSuccess.ToString() +
                    ", strNote: " + strNote.ToString() +
                    ", strIP: " + strIP.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> CreateAsync(int iUserId, Enums.Users_Activity_ActionGroup enumGroup, Enums.Users_Activity_ActionType enumType, string strActionText, int? iRefItemId1 = null, int? iRefItemId2 = null, int? iRefItemId3 = null, int? iRefItemId4 = null, int? iRefItemId5 = null, int? iAuditId = null, bool bSuccess = true, string strIP = "", string strNote = "")
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = new tblUserActivity
                {
                    tblUser_id = iUserId,
                    tblUserActivity_email = "",
                    tblUserActivity_actionGroup = (int)enumGroup,
                    tblUserActivity_actionType = (int)enumType,
                    tblUserActivity_actionText = strActionText,
                    tblUserActivity_refItemId1 = iRefItemId1,
                    tblUserActivity_refItemId2 = iRefItemId2,
                    tblUserActivity_refItemId3 = iRefItemId3,
                    tblUserActivity_refItemId4 = iRefItemId4,
                    tblUserActivity_refItemId5 = iRefItemId5,
                    tblSysAudit_id = iAuditId,
                    tblUserActivity_success = bSuccess,
                    tblUserActivity_note = strNote,
                    tblUserActivity_ipAddress = strIP,
                    tblUserActivity_createdUTC = DateTime.UtcNow,
                    tblUserActivity_createdLocal = DateTime.Now
                };

                _ = dB.tblUserActivities.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = true;
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Activity).ToString(), "CreateAsync(int, Enums.Users_Activity_ActionGroup, Enums.Users_Activity_ActionType, string, int?, int?, int?, int?, int?, int?, string, string)", ex,
                    "iUserId: " + iUserId.ToString() +
                    ", enumGroup: " + ((int)enumGroup).ToString() +
                    ", enumType: " + ((int)enumType).ToString() +
                    ", strActionText: " + strActionText.ToString() +
                    ", iRefItemId1: " + (iRefItemId1.HasValue ? iRefItemId1.Value.ToString() : "") +
                    ", iRefItemId2: " + (iRefItemId2.HasValue ? iRefItemId2.Value.ToString() : "") +
                    ", iRefItemId3: " + (iRefItemId3.HasValue ? iRefItemId3.Value.ToString() : "") +
                    ", iRefItemId4: " + (iRefItemId4.HasValue ? iRefItemId4.Value.ToString() : "") +
                    ", iRefItemId5: " + (iRefItemId5.HasValue ? iRefItemId5.Value.ToString() : "") +
                    ", iAuditId: " + (iAuditId.HasValue ? iAuditId.Value.ToString() : "") +
                    ", bSuccess: " + bSuccess.ToString() +
                    ", strNote: " + strNote.ToString() +
                    ", strIP: " + strIP.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<Activity> CreateReturnAsync(int iUserId, Enums.Users_Activity_ActionGroup enumGroup, Enums.Users_Activity_ActionType enumType, string strActionText, int? iRefItemId1 = null, int? iRefItemId2 = null, int? iRefItemId3 = null, int? iRefItemId4 = null, int? iRefItemId5 = null, int? iAuditId = null, bool bSuccess = true, string strIP = "", string strNote = "")
        {
            var objReturn = new Activity();

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblUserActivity
                {
                    tblUser_id = iUserId,
                    tblUserActivity_email = "",
                    tblUserActivity_actionGroup = (int)enumGroup,
                    tblUserActivity_actionType = (int)enumType,
                    tblUserActivity_actionText = strActionText,
                    tblUserActivity_refItemId1 = iRefItemId1,
                    tblUserActivity_refItemId2 = iRefItemId2,
                    tblUserActivity_refItemId3 = iRefItemId3,
                    tblUserActivity_refItemId4 = iRefItemId4,
                    tblUserActivity_refItemId5 = iRefItemId5,
                    tblSysAudit_id = iAuditId,
                    tblUserActivity_success = bSuccess,
                    tblUserActivity_note = strNote,
                    tblUserActivity_ipAddress = strIP,
                    tblUserActivity_createdUTC = DateTime.UtcNow,
                    tblUserActivity_createdLocal = DateTime.Now
                };

                _ = dB.tblUserActivities.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    _ = objReturn.Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Activity).ToString(), "CreateReturnAsync(int, Enums.Users_Activity_ActionGroup, Enums.Users_Activity_ActionType, string, int?, int?, int?, int?, int?, int?, bool, string, string)", ex,
                    "iUserId: " + iUserId.ToString() +
                    ", enumGroup: " + ((int)enumGroup).ToString() +
                    ", enumType: " + ((int)enumType).ToString() +
                    ", strActionText: " + strActionText.ToString() +
                    ", iRefItemId1: " + (iRefItemId1.HasValue ? iRefItemId1.Value.ToString() : "") +
                    ", iRefItemId2: " + (iRefItemId2.HasValue ? iRefItemId2.Value.ToString() : "") +
                    ", iRefItemId3: " + (iRefItemId3.HasValue ? iRefItemId3.Value.ToString() : "") +
                    ", iRefItemId4: " + (iRefItemId4.HasValue ? iRefItemId4.Value.ToString() : "") +
                    ", iRefItemId5: " + (iRefItemId5.HasValue ? iRefItemId5.Value.ToString() : "") +
                    ", iAuditId: " + (iAuditId.HasValue ? iAuditId.Value.ToString() : "") +
                    ", bSuccess: " + bSuccess.ToString() +
                    ", strNote: " + strNote.ToString() +
                    ", strIP: " + strIP.ToString());
                return objReturn;
            }

            return objReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Activity> FindAsync(int iId)
        {
            var objReturn = new Activity();
            await objReturn.LoadAsync(iId);

            return objReturn;
        }

        #endregion finders


        #region Lists

        public static async Task<List<Activity>> ListAsync(List<Filter<FilterOption>>? lstFilters = null)
        {
            return (await ListPagedAsync(lstFilters: lstFilters, iPageSize: 1000000)).Elements;
        }

        public static async Task<PagedData<Activity>> ListPagedAsync(List<Filter<FilterOption>>? lstFilters = null, int iPageSize = 25, int iPage = 1, List<SortOption>? lstOrdering = null)
        {
            var objPagedData = new PagedData<Activity>(0, iPageSize, iPage);

            var lstElements = new List<Activity>();

            try
            {
                var predicate = CompileWhereDB(lstFilters);

                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var iElementsFound = await dB.tblUserActivities.AsNoTracking().AsExpandable().CountAsync(predicate);

                objPagedData = new PagedData<Activity>(iElementsFound, iPageSize, iPage);

                if (iElementsFound > 0)
                {
                    var strOrderBy = SortOption.CompileOrderBy<Activity>(lstOrdering, OrderByDefault, OrderByDefaultDB, true);

                    var lstToProcess = await dB.tblUserActivities.AsNoTracking().AsExpandable().Where(predicate).OrderBy(strOrderBy).Skip(objPagedData.ElementsToSkip).Take(objPagedData.ElementsToTake).ToListAsync();

                    foreach (var efmObject in lstToProcess)
                    {
                        var obj = new Activity(efmObject);

                        if (obj.Loaded)
                        {
                            lstElements.Add(obj);
                        }

                        obj = null;
                    }

                    lstToProcess = null;
                }

                objPagedData.Elements = lstElements;

                predicate = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Activity).ToString(), "ListPagedAsync(List<Filter<FilterOption>>, int, int, List<SortOption>)", ex);
                return objPagedData;
            }

            return objPagedData;
        }

#pragma warning disable CS8629 // Nullable value type may be null.
        private static Expression<Func<tblUserActivity, bool>> CompileWhereDB(List<Filter<FilterOption>>? lstFilters, bool bTrue = true)
        {
            var predicate = PredicateBuilder.New<tblUserActivity>(true);

            if (!bTrue) { predicate = PredicateBuilder.New<tblUserActivity>(false); }

            if (lstFilters == null) { return predicate; }

            try
            {
                foreach (var vFilter in lstFilters)
                {
                    switch (vFilter.Option)
                    {
                        case FilterOption.Ids:
                            {
                                var lstIds = vFilter.Value_ListInt();
                                predicate = predicate.And(r => lstIds.Contains(r.tblUserActivity_id));
                            }
                            break;

                        case FilterOption.User_Id:
                            {
                                var i = vFilter.Value_Int();
                                predicate = predicate.And(r => r.tblUser_id == i);
                            }
                            break;
                        case FilterOption.User_Ids:
                            {
                                var lstIds = vFilter.Value_ListInt();
                                predicate = predicate.And(r => r.tblUser_id != null).And(r => lstIds.Contains(r.tblUser_id.Value));
                            }
                            break;

                        case FilterOption.Email:
                            {
                                var strSearch = vFilter.Value_String().ToLower().Trim();
                                predicate = predicate.And(r => r.tblUserActivity_email.Contains(strSearch));
                            }
                            break;

                        case FilterOption.ActionGroup:
                            {
                                var i = vFilter.Value_Int();
                                predicate = predicate.And(r => r.tblUserActivity_actionGroup == i);
                            }
                            break;
                        case FilterOption.ActionGroups:
                            {
                                var lstIds = vFilter.Value_ListInt();
                                predicate = predicate.And(r => lstIds.Contains(r.tblUserActivity_actionGroup));
                            }
                            break;

                        case FilterOption.ActionType:
                            {
                                var i = vFilter.Value_Int();
                                predicate = predicate.And(r => r.tblUserActivity_actionType == i);
                            }
                            break;
                        case FilterOption.ActionTypes:
                            {
                                var lstIds = vFilter.Value_ListInt();
                                predicate = predicate.And(r => lstIds.Contains(r.tblUserActivity_actionType));
                            }
                            break;

                        case FilterOption.ActionText:
                            {
                                var str = vFilter.Value_String();
                                predicate = predicate.And(r => r.tblUserActivity_actionText == str);
                            }
                            break;

                        case FilterOption.Success:
                            {
                                var b = vFilter.Value_Bool();
                                predicate = predicate.And(r => r.tblUserActivity_success == b);
                            }
                            break;

                        case FilterOption.Date_Created_UTC:
                            {
                                var dfDates = vFilter.Value_DateFilter();

                                if (dfDates.From.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserActivity_createdUTC >= dfDates.From.Value);
                                }

                                if (dfDates.To.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserActivity_createdUTC <= dfDates.To.Value);
                                }

                                if (dfDates.Equal.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserActivity_createdUTC == dfDates.Equal.Value);
                                }
                            }
                            break;
                        case FilterOption.Date_Created_Local:
                            {
                                var dfDates = vFilter.Value_DateFilter();

                                if (dfDates.From.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserActivity_createdLocal >= dfDates.From.Value);
                                }

                                if (dfDates.To.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserActivity_createdLocal <= dfDates.To.Value);
                                }

                                if (dfDates.Equal.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserActivity_createdLocal == dfDates.Equal.Value);
                                }
                            }
                            break;

                        default: continue;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Activity).ToString(), "CompileWhereDB(List<Filter<FilterOption>>)", ex);
                return predicate;
            }

            return predicate;
        }
#pragma warning restore CS8629 // Nullable value type may be null.

        #endregion lists
    }
}
