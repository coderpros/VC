using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using VC.Res.Core.Utilities;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Users
{
    public class AuthCode
    {
        public enum FilterOption
        {
            Ids,
            User_Id,
            User_Ids,
            Date_Expires_UTC,
            Date_Expires_Local
        };


        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public int User_Id { get; private set; } = 0;

        public string Code { get; private set; } = "";

        public DateTime Expires_UTC { get; private set; } = DateTime.UtcNow.AddHours(1);

        public DateTime Expires_Local { get; private set; } = DateTime.Now.AddHours(1);

        #endregion properties


        #region Constructors

        private AuthCode() { }

        //private AuthCode(int iId) { _ = Load(iId); }

        //private AuthCode(int iUserId, string strCode) { _ = Load(iUserId, strCode); }

        private AuthCode(tblUserAuthCode efmObject) { _ = Load(efmObject); }

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

                var efmObject = await dB.tblUserAuthCodes.AsNoTracking().FirstOrDefaultAsync(r => r.tblUserAuthCode_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(AuthCode).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        private async Task<bool> LoadAsync(int iUserId, string strCode)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblUserAuthCodes.AsNoTracking().FirstOrDefaultAsync(r => r.tblUser_id == iUserId && r.tblUserAuthCode_code == strCode);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(AuthCode).ToString(), "LoadAsync(int, string)", ex,
                    "iUserId: " + iUserId.ToString() +
                    ", strCode: " + strCode);
                return bReturn;
            }

            return bReturn;
        }

        private bool Load(tblUserAuthCode efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblUserAuthCode_id;

                    User_Id = efmObject.tblUser_id;
                    Code = efmObject.tblUserAuthCode_code;
                    Expires_UTC = efmObject.tblUserAuthCode_expiresUTC;
                    Expires_Local = efmObject.tblUserAuthCode_expiresLocal;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(AuthCode).ToString(), "Load(tblUserAuthCode)", ex);
                return bReturn;
            }

            return bReturn;
        }

        #endregion private functions-loaders

        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        public static async Task<AuthCode> CreateAsync(int iUserId)
        {
            var tmpReturn = new AuthCode();

            if (iUserId == 0) { return tmpReturn; }

            var generator = new Random();
            var strCode = generator.Next(0, 999999).ToString("D6");

            //need to check that there are no active codes matching this
            while (!(await Check_UniqueCodeAsync(strCode)))
            {
                strCode = generator.Next(0, 999999).ToString("D6");
            }

            //delete existing codes for this user, if one exists
            _ = await Delete_ByUserAsync(iUserId);

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = new tblUserAuthCode
                {
                    tblUser_id = iUserId,
                    tblUserAuthCode_code = strCode,
                    tblUserAuthCode_expiresUTC = DateTime.UtcNow.AddHours(1),
                    tblUserAuthCode_expiresLocal = DateTime.Now.AddHours(1)
                };

                _ = dB.tblUserAuthCodes.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    tmpReturn = new AuthCode(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(AuthCode).ToString(), "CreateAsync(Enum, int, string, bool)", ex,
                    "iUserId: " + iUserId.ToString());
                return tmpReturn;
            }

            return tmpReturn;
        }

        public static async Task<bool> DeleteAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                _ = await dB.tblUserAuthCodes.Where(r => r.tblUserAuthCode_id == iId).DeleteAsync();

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(AuthCode).ToString(), "DeleteAsync(int)", ex,
                    "iId: " + iId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Delete_ByUserAsync(int iUserId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                _ = await dB.tblUserAuthCodes.Where(r => r.tblUser_id == iUserId).DeleteAsync();

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(AuthCode).ToString(), "Delete_ByUserAsync(int)", ex,
                    "iUserId: " + iUserId.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Delete_ExpiredAsync()
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                var dtExpiry = DateTime.UtcNow;

                _ = await dB.tblUserAuthCodes.Where(r => r.tblUserAuthCode_expiresUTC < dtExpiry).DeleteAsync();

                bReturn = true;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(AuthCode).ToString(), "Delete_ExpiredAsync()", ex, "");
                return bReturn;
            }

            return bReturn;
        }


        public static async Task<bool> Check_UniqueCodeAsync(string strCode)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                if (!(await dB.tblUserAuthCodes.AsNoTracking().AnyAsync(r => r.tblUserAuthCode_code == strCode)))
                {
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(AuthCode).ToString(), "Check_UniqueCodeAsync(string)", ex,
                    "strCode: " + strCode.ToString());
                return false;
            }

            return bReturn;
        }

        public static async Task<bool> SendAsync(int iUserId, string strIP, Enums.Shared_TwoFAMethod enumMethodOverride = Enums.Shared_TwoFAMethod.Disabled)
        {
            var bReturn = false;

            try
            {
                // get the user we're sending the code to
                var objUser = await User.FindAsync(iUserId);

                if (!objUser.Loaded) { return false; }

                var enumSendMethod = Enums.Shared_TwoFAMethod.Disabled;
                var strSendAddress = "";

                // check we can send a code
                if (enumMethodOverride != Enums.Shared_TwoFAMethod.Disabled)
                {
                    // use the override
                    switch (enumMethodOverride)
                    {
                        case Enums.Shared_TwoFAMethod.Disabled:
                            // can't send code
                            return false;

                        case Enums.Shared_TwoFAMethod.Email:
                            enumSendMethod = Enums.Shared_TwoFAMethod.Email;
                            strSendAddress = objUser.Email;
                            break;

                        case Enums.Shared_TwoFAMethod.MobileText:
                            if (string.IsNullOrWhiteSpace(objUser.Tel_Mobile))
                            {
                                // can't send
                                return false;
                            }

                            // use tel
                            enumSendMethod = Enums.Shared_TwoFAMethod.MobileText;
                            strSendAddress = objUser.Tel_Mobile;
                            break;
                    }
                }
                else
                {
                    switch (objUser.TwoFA_Method)
                    {
                        case Enums.Shared_TwoFAMethod.Disabled:
                            // can't send code
                            return false;

                        case Enums.Shared_TwoFAMethod.Email:
                            enumSendMethod = Enums.Shared_TwoFAMethod.Email;
                            strSendAddress = objUser.Email;
                            break;

                        case Enums.Shared_TwoFAMethod.MobileText:
                            if (string.IsNullOrWhiteSpace(objUser.Tel_Mobile) || !objUser.Tel_Mobile_Verified)
                            {
                                // revert to email
                                enumSendMethod = Enums.Shared_TwoFAMethod.Email;
                                strSendAddress = objUser.Email;
                            }
                            else
                            {
                                // use tel
                                enumSendMethod = Enums.Shared_TwoFAMethod.MobileText;
                                strSendAddress = objUser.Tel_Mobile;
                            }
                            break;
                    }
                }

                // should be ok to send if got to here

                // create an auth code to send
                var objAuthCode = await CreateAsync(iUserId);

                if (!objAuthCode.Loaded) { return false; }

                switch (enumSendMethod)
                {
                    case Enums.Shared_TwoFAMethod.Email:
                        {
                            var objGSettings = Settings.Global.Fetch;
                            var objISettings = Settings.Interface.Fetch;

                            var objEmail = new Email
                            {
                                User_Id = iUserId
                            };

                            objEmail.To.Add(strSendAddress);
                            objEmail.From_Address = objGSettings.Email_Generic_FromAddress;
                            objEmail.From_Name = objGSettings.Email_Generic_FromName;
                            objEmail.Subject = objISettings.Name + " Authentication Code";
                            objEmail.Template = await General.ContentFromURLAsync(objISettings.URL.TrimEnd('/') + "/email-templates/User-AuthCode.html");

                            objEmail.Variables.Add("AuthCode", objAuthCode.Code);

                            if (await objEmail.CreateAndSendAsync(Enums.Utilities_Email_Type.User_AuthCode))
                            {
                                bReturn = true;

                                // log the action
                                _ = await Activity.AuthenticationAsync(iUserId, "", Enums.Users_Activity_ActionType.Request, "Email Auth Code Sent.", true, strIP);
                            }
                        }
                        break;

                    //case Enums.Shared_TwoFAMethod.MobileText:
                    //    {
                    //        //token generated via POSTMAN, need to create new API https://thesmsworks.co.uk/
                    //        //then https://api.thesmsworks.co.uk/v1/auth/token

                    //        if (!Configuration.Default.ApiKey.ContainsKey("Authorization"))
                    //        {
                    //            //Configuration.Default.ApiKey.Add("Authorization", "JWT eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJrZXkiOiIzZTY5NjkzYy03NWZhLTQzMTQtYmM3NS1lYWRmYmRiODM1OGEiLCJzZWNyZXQiOiIyMWE2NWQyYTc0MDllZDBiNTNiZjcxYmU3NDc3OTIyNDEzZTZjNjU3Mzc0ODhhMWY0MTYzMjAwMzk0ZmI1NzY3IiwiaWF0IjoxNjI5OTg4MjM3LCJleHAiOjI0MTgzODgyMzd9.OzsY2yeUy9eWJOzUyLbAY6LqTRxa_2a6TO9U01b4OZM");
                    //            Configuration.Default.ApiKey.Add("Authorization", string.Format("JWT {0}", Settings.Global.Fetch.TextMsg_SMSWorks_APIToken));
                    //        }

                    //        var apiInstance = new MessagesApi();
                    //        var smsMessage = new Message(sender: Settings.Global.Fetch.TextMsg_SenderName, destination: strSendAddress, content: objAuthCode.Code + " is your security code for DACB Crisis Room.", tag: "AuthCode");

                    //        var result = apiInstance.SendMessage(smsMessage);

                    //        if (result.Status.Trim().ToLower() == "sent")
                    //        {
                    //            bReturn = true;

                    //            // log the action
                    //            _ = Activity.Authentication(iUserId, "", Enums.Users_Activity_ActionType.Request, "SMS Auth Code Sent.", true, strIP);
                    //        }

                    //        result = null;

                    //        apiInstance = null;
                    //        smsMessage = null;
                    //    }
                    //    break;
                    default: break;
                }

                objAuthCode = null;
                objUser = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(AuthCode).ToString(), "SendAsync(int, string, Enums.Shared_TwoFAMethod)", ex,
                    "iUserId: " + iUserId.ToString() +
                    ", strIP: " + strIP.ToString() +
                    ", enumMethodOverride: " + ((int)enumMethodOverride).ToString());
                return false;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<AuthCode> FindAsync(int iId)
        {
            // before finding anything, delete expired
            _ = await Delete_ExpiredAsync();

            var objReturn = new AuthCode();
            await objReturn.LoadAsync(iId);

            return objReturn;
        }

        public static async Task<AuthCode> FindAsync(int iUserId, string strCode)
        {
            // before finding anything, delete expired
            _ = await Delete_ExpiredAsync();

            var objReturn = new AuthCode();
            await objReturn.LoadAsync(iUserId, strCode);

            return objReturn;
        }

        #endregion finders


        #region Lists

        public static async Task<List<AuthCode>> ListAsync(List<Filter<FilterOption>>? lstFilters = null)
        {
            // before finding anything, delete expired
            _ = await Delete_ExpiredAsync();

            var lstElements = new List<AuthCode>();

            try
            {
                // no cache available so query needs to be more precise if possible
                var predicate = CompileWhereDB(lstFilters);

                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var lstItems = await dB.tblUserAuthCodes.AsNoTracking().AsExpandable().Where(predicate).ToListAsync();

                foreach (var efmObject in lstItems)
                {
                    var obj = new AuthCode(efmObject);

                    if (obj.Loaded)
                    {
                        lstElements.Add(obj);
                    }

                    obj = null;
                }

                lstItems = null;

                predicate = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(AuthCode).ToString(), "List(List<Filter<FilterOption>>)", ex);
                return lstElements;
            }

            return lstElements;
        }

        private static Expression<Func<tblUserAuthCode, bool>> CompileWhereDB(List<Filter<FilterOption>>? lstFilters, bool bTrue = true)
        {
            var predicate = PredicateBuilder.New<tblUserAuthCode>(true);

            if (!bTrue) { predicate = PredicateBuilder.New<tblUserAuthCode>(false); }

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
                                predicate = predicate.And(r => lstIds.Contains(r.tblUserAuthCode_id));
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
                                predicate = predicate.And(r => lstIds.Contains(r.tblUser_id));
                            }
                            break;

                        case FilterOption.Date_Expires_UTC:
                            {
                                var dfDates = vFilter.Value_DateFilter();

                                if (dfDates.From.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserAuthCode_expiresUTC >= dfDates.From.Value);
                                }

                                if (dfDates.To.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserAuthCode_expiresUTC <= dfDates.To.Value);
                                }

                                if (dfDates.Equal.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserAuthCode_expiresUTC == dfDates.Equal.Value);
                                }
                            }
                            break;
                        case FilterOption.Date_Expires_Local:
                            {
                                var dfDates = vFilter.Value_DateFilter();

                                if (dfDates.From.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserAuthCode_expiresLocal >= dfDates.From.Value);
                                }

                                if (dfDates.To.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserAuthCode_expiresLocal <= dfDates.To.Value);
                                }

                                if (dfDates.Equal.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUserAuthCode_expiresLocal == dfDates.Equal.Value);
                                }
                            }
                            break;

                        default: continue;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(AuthCode).ToString(), "CompileWhereDB(List<Filter<FilterOption>>)", ex);
                return predicate;
            }

            return predicate;
        }

        #endregion lists
    }
}
