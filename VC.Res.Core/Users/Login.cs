using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using VC.Res.Core.Utilities;

namespace VC.Res.Core.Users
{
    public class Login : User
    {
        #region Properties

        public bool Authenticated { get; private set; } = false;

        public bool Auth_Password_UpdateRequired { get; private set; } = false;

        public bool Auth_Account_Locked { get; private set; } = false;

        public DateTime? Auth_Account_Locked_UntilUTC { get; private set; } = null;
        public DateTime? Auth_Account_Locked_UntilLocal { get; private set; } = null;

        public bool Auth_AuthCodeRequired { get; private set; } = false;

        public DateTime? Auth_Login_UTC { get; private set; } = null;

        public DateTime? Auth_Login_Local { get; private set; } = null;

        public string Auth_Login_IP { get; private set; } = "";

        #endregion properties


        #region Constructors

        //public Login()
        //{
        //    try
        //    {
        //        using var dB = Settings.Config.DBConnection();
        //        var efmObject = new Database.tblUser();
        //        if (efmObject != null)
        //        {
        //            var strBy = "";
        //            if (string.IsNullOrWhiteSpace(strBy)) { strBy = "System"; }
        //            efmObject.tblUser_email = "connectusinfowaydemo12@gmail.com";
        //            efmObject.tblUser_enabled = true;
        //            efmObject.tblUser_pwdSalt = Security.Argon2_CreateSalt();
        //            efmObject.tblUser_pwd = Security.Argon2_CreateHash("123456", efmObject.tblUser_pwdSalt);
        //            efmObject.tblUser_pwdLastChangedUTC = DateTime.UtcNow;
        //            efmObject.tblUser_pwdLastChangedLocal = DateTime.Now;
        //            efmObject.tblUser_failedLoginTotal = 0;
        //            efmObject.tblUser_failedLoginLastUTC = null;
        //            efmObject.tblUser_failedLoginLastLocal = null;
        //            efmObject.tblUser_failedLoginLockUTC = null;
        //            efmObject.tblUser_failedLoginLockLocal = null;
        //            efmObject.tblUser_editedUTC = DateTime.UtcNow;
        //            efmObject.tblUser_editedLocal = DateTime.Now;
        //            efmObject.tblUser_editedBy = strBy;
        //            dB.Add(efmObject);
        //            if (dB.SaveChanges() > 0)
        //            {
        //                // user making update is account owner
        //                _ = Activity.CreateAsync(efmObject.tblUser_email,
        //                        Enums.Users_Activity_ActionGroup.User,
        //                        Enums.Users_Activity_ActionType.Edit,
        //                        "Account password set.",
        //                        strIP: "::1");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        #endregion constructors


        #region Private Functions



        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        public static async Task<Login> Credentials_WebAsync(string strEmailAddress, string strPassword, string strIP, bool bUpdateLastLogin = true)
        {
            var objReturn = new Login();
            if (string.IsNullOrWhiteSpace(strEmailAddress) || string.IsNullOrWhiteSpace(strPassword))
            {
                return objReturn;
            }

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                //var dtTempLockCutoff = DateTime.UtcNow.AddMinutes(-Settings.Variables.AccountLock_Minutes);

                var strEmailToUse = strEmailAddress.ToLower().Trim();

                // find the user
                var efmUser = dB.tblUsers.AsNoTracking().FirstOrDefault(r => r.tblUser_email == strEmailToUse && r.tblUser_deletedUTC == null);

                if (efmUser == null)
                {
                    // user not found or deleted
                    _ = await Activity.AuthenticationAsync(null, strEmailToUse, Enums.Users_Activity_ActionType.Attempt, "Incorrect email or password.", false, strIP);

                    return objReturn;
                }

                // check if the account is locked (don't even check the password)
                if (efmUser.tblUser_failedLoginLockUTC > DateTime.UtcNow)
                {
                    // account is currently locked
                    objReturn.Auth_Account_Locked = true;
                    objReturn.Auth_Account_Locked_UntilUTC = efmUser.tblUser_failedLoginLockUTC;
                    objReturn.Auth_Account_Locked_UntilLocal = efmUser.tblUser_failedLoginLockLocal;

                    // log another failed attempt
                    var tskFailedLogin = FailedLoginAttemptAsync(efmUser.tblUser_id);

                    var tskActivity = Activity.AuthenticationAsync(efmUser.tblUser_id, strEmailToUse, Enums.Users_Activity_ActionType.Attempt, "Account is locked.", false, strIP);

                    await Task.WhenAll(tskFailedLogin, tskActivity);

                    tskFailedLogin = null;
                    tskActivity = null;

                    return objReturn;
                }

                if (string.IsNullOrWhiteSpace(efmUser.tblUser_pwdSalt) || string.IsNullOrWhiteSpace(efmUser.tblUser_pwd))
                {
                    // no password set
                    objReturn.Auth_Password_UpdateRequired = true;
                    return objReturn;
                }

                // the account is open for login
                if (Utilities.Security.Argon2_VerifyHash(strPassword, efmUser.tblUser_pwdSalt, efmUser.tblUser_pwd))
                {
                    // load the user
                    _ = objReturn.Load(efmUser);
                }
                else
                {
                    // incorrect password
                    // record the failed attempt
                    var tskFailedLogin = FailedLoginAttemptAsync(efmUser.tblUser_id);
                    var tskActivity = Activity.AuthenticationAsync(efmUser.tblUser_id, strEmailToUse, Enums.Users_Activity_ActionType.Attempt, "Incorrect email or password.", false, strIP);

                    await Task.WhenAll(tskFailedLogin, tskActivity);

                    tskFailedLogin = null;
                    tskActivity = null;

                    return objReturn;
                }

                // check if disabled
                if (!efmUser.tblUser_enabled)
                {
                    // account isn't enabled, not strictly a failure, but shouldn't be allowed
                    _ = await Activity.AuthenticationAsync(efmUser.tblUser_id, strEmailToUse, Enums.Users_Activity_ActionType.Attempt, "Account disabled.", false, strIP);

                    return objReturn;
                }

                // if got to here then they are basically authenticated, but may need further verification
                objReturn.Authenticated = true;
                objReturn.Auth_Login_UTC = DateTime.UtcNow;
                objReturn.Auth_Login_Local = DateTime.Now;
                objReturn.Auth_Login_IP = strIP;

                _ = await Activity.AuthenticationAsync(efmUser.tblUser_id, strEmailToUse, Enums.Users_Activity_ActionType.Login, "Email and Password authenticated.", true, strIP);

                // check if we need authcode
                if (objReturn.TwoFA_Enabled && objReturn.TwoFA_Method != Enums.Shared_TwoFAMethod.Disabled)
                {
                    var objIPHistory = await IPHistory.FindAsync(objReturn.Id, strIP);

                    if (objIPHistory.Loaded)
                    {
                        if (!objIPHistory.Authorised)
                        {
                            // ip is recorded, but not authorised, need auth code
                            objReturn.Auth_AuthCodeRequired = true;
                        }
                    }
                    else
                    {
                        // no ip found, create and request an auth code
                        _ = await IPHistory.CreateAsync(objReturn.Id, strIP);
                        objReturn.Auth_AuthCodeRequired = true;
                    }
                }

                if (objReturn.Auth_AuthCodeRequired)
                {
                    //create auth code
                    if (!(await AuthCode.SendAsync(objReturn.Id, strIP)))
                    {
                        // log failure, but required
                        _ = await Activity.AuthenticationAsync(efmUser.tblUser_id, strEmailToUse, Enums.Users_Activity_ActionType.Attempt, "Auth code required but failed to send.", false, strIP);
                    }
                }
                else
                {
                    if (bUpdateLastLogin)
                    {
                        _ = await LastLogin_UpdateAsync(objReturn.Id, strIP);
                    }
                }

                efmUser = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Login).ToString(), "Credentials_WebAsync(string, string, string, bool)", ex,
                    "strEmailAddress: " + strEmailAddress +
                    ", strIP: " + strIP.ToString() +
                    ", bUpdateLastLogin: " + bUpdateLastLogin.ToString());

                return objReturn;
            }

            return objReturn;
        }

        public static async Task<Login> AuthCode_WebAsync(int iUserId, string strAuthCode, string strIP, bool bUpdateLastLogin = true)
        {
            var objReturn = new Login();

            if (iUserId < 1 || string.IsNullOrWhiteSpace(strAuthCode))
            {
                return objReturn;
            }

            // find the authcode
            var objAuthCode = await AuthCode.FindAsync(iUserId, strAuthCode);

            if (!objAuthCode.Loaded)
            {
                // authcode not found/valid
                var tskActivity = Activity.AuthenticationAsync(iUserId, "", Enums.Users_Activity_ActionType.Attempt, "Invalid auth code.", false, strIP);

                var tskFailedLogin = FailedLoginAttemptAsync(iUserId);

                await Task.WhenAll(tskFailedLogin, tskActivity);

                tskFailedLogin = null;
                tskActivity = null;

                return objReturn;
            }

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // find the user
                var efmUser = dB.tblUsers.AsNoTracking().FirstOrDefault(r => r.tblUser_id == objAuthCode.User_Id && r.tblUser_deletedUTC == null);

                if (efmUser == null) { return objReturn; }

                // check if the account is locked (don't even check the password)
                if (efmUser.tblUser_failedLoginLockUTC > DateTime.UtcNow)
                {
                    // account is currently locked
                    objReturn.Auth_Account_Locked = true;
                    objReturn.Auth_Account_Locked_UntilUTC = efmUser.tblUser_failedLoginLockUTC;
                    objReturn.Auth_Account_Locked_UntilLocal = efmUser.tblUser_failedLoginLockLocal;

                    // log another failed attempt
                    var tskFailedLogin = FailedLoginAttemptAsync(efmUser.tblUser_id);

                    var tskActivity = Activity.AuthenticationAsync(efmUser.tblUser_id, "", Enums.Users_Activity_ActionType.Attempt, "Account is locked.", false, strIP);

                    await Task.WhenAll(tskFailedLogin, tskActivity);

                    tskFailedLogin = null;
                    tskActivity = null;

                    return objReturn;
                }

                // check if disabled
                if (!efmUser.tblUser_enabled)
                {
                    // account isn't enabled, not strictly a failure, but shouldn't be allowed
                    _ = await Activity.AuthenticationAsync(efmUser.tblUser_id, "", Enums.Users_Activity_ActionType.Attempt, "Account disabled.", false, strIP);

                    return objReturn;
                }


                // if got to here then they are authenticated
                _ = objReturn.Load(efmUser);

                objReturn.Authenticated = true;
                objReturn.Auth_Login_UTC = DateTime.UtcNow;
                objReturn.Auth_Login_Local = DateTime.Now;
                objReturn.Auth_Login_IP = strIP;

                // authorise the IP getting used due to auth code
                var objIP = await IPHistory.FindAsync(objReturn.Id, strIP);

                if (objIP.Loaded && !objIP.Authorised)
                {
                    _ = await IPHistory.AuthoriseAsync(objIP.Id);
                }

                // rerun checks to see if auth code needs to be reset (i.e incase IP has changed or something along those lines)
                if (objReturn.TwoFA_Enabled && objReturn.TwoFA_Method != Enums.Shared_TwoFAMethod.Disabled)
                {
                    var objIPHistory = await IPHistory.FindAsync(objReturn.Id, strIP);

                    if (objIPHistory.Loaded)
                    {
                        if (!objIPHistory.Authorised)
                        {
                            // ip is recorded, but not authorised, need auth code
                            objReturn.Auth_AuthCodeRequired = true;
                        }
                    }
                    else
                    {
                        // no ip found, create and request an auth code
                        _ = await IPHistory.CreateAsync(objReturn.Id, strIP);
                        objReturn.Auth_AuthCodeRequired = true;
                    }

                    objIPHistory = null;
                }

                if (objReturn.Auth_AuthCodeRequired)
                {
                    //create auth code
                    if (!(await AuthCode.SendAsync(objReturn.Id, strIP)))
                    {
                        // log failure, but required
                        _ = await Activity.AuthenticationAsync(efmUser.tblUser_id, "", Enums.Users_Activity_ActionType.Attempt, "Auth code required but failed to send.", false, strIP);
                    }
                }
                else
                {
                    // successfully logged in
                    _ = await Activity.AuthenticationAsync(efmUser.tblUser_id, "", Enums.Users_Activity_ActionType.Login, "Auth code accepted.", true, strIP);

                    if (bUpdateLastLogin)
                    {
                        _ = await LastLogin_UpdateAsync(objReturn.Id, strIP);
                    }
                }

                efmUser = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Login).ToString(), "AuthCode_WebAsync(int, string, string, bool)", ex,
                    "iUserId: " + iUserId.ToString() +
                    ", strAuthCode: " + strAuthCode.ToString() +
                    ", strIP: " + strIP.ToString() +
                    ", bUpdateLastLogin: " + bUpdateLastLogin.ToString());

                return objReturn;
            }

            return objReturn;
        }

        #endregion public functions
    }
}
