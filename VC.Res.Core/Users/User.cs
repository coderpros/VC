using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using VC.Res.Core.Database;
using VC.Res.Core.Utilities;

namespace VC.Res.Core.Users
{
    public class User
    {
        // UserActivity
        // ReferenceItem_Id1 = User Id

        private const LocalCache.Key CacheKey = LocalCache.Key.Users_Users;

        internal static readonly string OrderByDefault = nameof(Name_Last) + " ASC, " + nameof(Name_First) + " ASC";
        internal static readonly string OrderByDefaultDB = nameof(tblUser.tblUser_lastName) + " ASC, " + nameof(tblUser.tblUser_firstName) + " ASC";

        public enum FilterOption
        {
            Ids,
            Name,
            Email,
            Access_SysAdmin,
            //Access_Sub,
            //Access_SubSelf,
            //Access_Publish,
            Enabled,
            Date_Deleted_UTC
        };

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public string Email { get; set; } = "";

        public string Name_First { get; set; } = "";

        public string Name_Last { get; set; } = "";

        public string Name_Full
        {
            get
            {
                var strReturn = Name_First;

                if (!string.IsNullOrWhiteSpace(strReturn) && !string.IsNullOrWhiteSpace(Name_Last))
                {
                    strReturn += " ";
                }

                strReturn += Name_Last;

                return strReturn;
            }
        }

        public bool Password_IsSet { get; private set; } = false;

        public DateTime? Password_LastChanged_UTC { get; private set; } = null;

        public DateTime? Password_LastChanged_Local { get; private set; } = null;

        public bool TwoFA_Enabled { get; set; } = false;

        public Enums.Shared_TwoFAMethod TwoFA_Method { get; set; } = Enums.Shared_TwoFAMethod.Disabled;

        public string Tel_Mobile { get; set; } = "";

        public bool Tel_Mobile_Verified { get; set; } = false;

        public bool Access_SysAdmin { get; set; } = false;

        //public bool Access_Sub { get; set; } = true;

        //public bool Access_SubSelf { get; set; } = false;

        //public bool Access_Publish { get; set; } = true;

        public bool Enabled { get; set; } = true;

        public DateTime? LastLogin_UTC { get; private set; } = null;

        public DateTime? LastLogin_Local { get; private set; } = null;

        public string LastLogin_IP { get; private set; } = "";

        public int FailedLogin_Total { get; private set; } = 0;

        public DateTime? FailedLogin_Last_UTC { get; private set; } = null;

        public DateTime? FailedLogin_Last_Local { get; private set; } = null;

        public DateTime? FailedLogin_Lock_UTC { get; private set; } = null;

        public DateTime? FailedLogin_Lock_Local { get; private set; } = null;

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;

        public DateTime Created_Local { get; private set; } = DateTime.Now;

        public string Created_By { get; private set; } = "";

        public DateTime Edited_UTC { get; private set; } = DateTime.UtcNow;

        public DateTime Edited_Local { get; private set; } = DateTime.Now;

        public string Edited_By { get; private set; } = "";

        public DateTime? Deleted_UTC { get; private set; } = null;

        public DateTime? Deleted_Local { get; private set; } = null;

        public string Deleted_By { get; private set; } = "";

        #endregion properties


        #region Constructors

        public User() { }

        //private User(int iId) { _ = Load(iId); }

        //private User(string strEmail) { _ = Load(strEmail); }

        private User(tblUser efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                // clear loaded state
                Loaded = false;

                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblUsers.AsNoTracking().FirstOrDefaultAsync(r => r.tblUser_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "LoadAsync(int)", ex,
                    "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private async Task<bool> LoadAsync(string strEmailAddress)
        {
            var strEmailToUse = strEmailAddress.ToLower().Trim();

            if (!General.Validate_EmailAddress(strEmailToUse)) { return false; }

            var bReturn = false;

            try
            {
                // clear loaded state
                Loaded = false;

                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblUsers.AsNoTracking().FirstOrDefaultAsync(r => r.tblUser_email == strEmailToUse);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "LoadAsync(string)", ex,
                    "strEmailAddress: " + strEmailAddress.ToString());
                return false;
            }

            return bReturn;
        }

        protected bool Load(tblUser efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblUser_id;

                    Email = efmObject.tblUser_email;
                    Name_First = efmObject.tblUser_firstName;
                    Name_Last = efmObject.tblUser_lastName;

                    Password_IsSet = !string.IsNullOrWhiteSpace(efmObject.tblUser_pwd);

                    Password_LastChanged_UTC = efmObject.tblUser_pwdLastChangedUTC;
                    Password_LastChanged_Local = efmObject.tblUser_pwdLastChangedLocal;

                    TwoFA_Enabled = efmObject.tblUser_twoFAEnabled;
                    TwoFA_Method = (Enums.Shared_TwoFAMethod)efmObject.tblUser_twoFAMethod;

                    Tel_Mobile = efmObject.tblUser_telMobile;
                    Tel_Mobile_Verified = efmObject.tblUser_telMobileVerified;

                    Access_SysAdmin = efmObject.tblUser_accessSysAdmin;
                    //Access_Sub = efmObject.tblUser_accessCanSub;
                    //Access_SubSelf = efmObject.tblUser_accessCanSubSelf;
                    //Access_Publish = efmObject.tblUser_accessCanPub;

                    Enabled = efmObject.tblUser_enabled;

                    LastLogin_UTC = efmObject.tblUser_lastLoginUTC;
                    LastLogin_Local = efmObject.tblUser_lastLoginLocal;
                    LastLogin_IP = efmObject.tblUser_lastLoginIP;

                    FailedLogin_Total = efmObject.tblUser_failedLoginTotal;
                    FailedLogin_Last_UTC = efmObject.tblUser_failedLoginLastUTC;
                    FailedLogin_Last_Local = efmObject.tblUser_failedLoginLastLocal;

                    FailedLogin_Lock_UTC = efmObject.tblUser_failedLoginLockUTC;
                    FailedLogin_Lock_Local = efmObject.tblUser_failedLoginLockLocal;

                    Created_UTC = efmObject.tblUser_createdUTC;
                    Created_Local = efmObject.tblUser_createdLocal;
                    Created_By = efmObject.tblUser_createdBy;

                    Edited_UTC = efmObject.tblUser_editedUTC;
                    Edited_Local = efmObject.tblUser_editedLocal;
                    Edited_By = efmObject.tblUser_editedBy;

                    Deleted_UTC = efmObject.tblUser_deletedUTC;
                    Deleted_Local = efmObject.tblUser_deletedLocal;
                    Deleted_By = efmObject.tblUser_deletedBy;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "Load(tblUser)", ex);
                return bReturn;
            }

            return bReturn;
        }

        #endregion private functions-loaders

        protected static async Task<bool> LastLogin_UpdateAsync(int iUserId, string strIPAddress)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblUsers.FirstOrDefaultAsync(r => r.tblUser_id == iUserId);

                if (efmObject != null)
                {
                    efmObject.tblUser_failedLoginTotal = 0;
                    efmObject.tblUser_failedLoginLockUTC = null;
                    efmObject.tblUser_failedLoginLockLocal = null;

                    efmObject.tblUser_lastLoginUTC = DateTime.UtcNow;
                    efmObject.tblUser_lastLoginLocal = DateTime.Now;
                    efmObject.tblUser_lastLoginIP = strIPAddress;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "LastLogin_UpdateAsync(int, string)", ex,
                    "iUserId: " + iUserId.ToString() +
                    ", strIPAddress: " + strIPAddress.ToString());
                return bReturn;
            }

            return bReturn;
        }

        protected static async Task FailedLoginAttemptAsync(int iUserId)
        {
            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmUser = await dB.tblUsers.FirstOrDefaultAsync(r => r.tblUser_id == iUserId);

                if (efmUser != null)
                {
                    efmUser.tblUser_failedLoginTotal++;
                    efmUser.tblUser_failedLoginLastUTC = DateTime.UtcNow;
                    efmUser.tblUser_failedLoginLastLocal = DateTime.Now;

                    if (efmUser.tblUser_failedLoginTotal >= 5)
                    {
                        // 5 minute intervals
                        efmUser.tblUser_failedLoginLockUTC = DateTime.UtcNow.AddMinutes(5);
                        efmUser.tblUser_failedLoginLockLocal = DateTime.Now.AddMinutes(5);
                    }

                    if (efmUser.tblUser_failedLoginTotal >= 10)
                    {
                        // 15 minute intervals
                        efmUser.tblUser_failedLoginLockUTC = DateTime.UtcNow.AddMinutes(15);
                        efmUser.tblUser_failedLoginLockLocal = DateTime.Now.AddMinutes(15);
                    }

                    if (efmUser.tblUser_failedLoginTotal >= 15)
                    {
                        // 1 hour intervals
                        efmUser.tblUser_failedLoginLockUTC = DateTime.UtcNow.AddHours(1);
                        efmUser.tblUser_failedLoginLockLocal = DateTime.Now.AddHours(1);
                    }

                    if (efmUser.tblUser_failedLoginTotal >= 20)
                    {
                        // 1 day intervals
                        efmUser.tblUser_failedLoginLockUTC = DateTime.UtcNow.AddDays(1);
                        efmUser.tblUser_failedLoginLockLocal = DateTime.Now.AddDays(1);
                    }

                    _ = await dB.SaveChangesAsync();
                }

                efmUser = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "FailedLoginAttemptAsync(int)", ex,
                        "iUserId: " + iUserId.ToString());
            }
        }

        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0075:Simplify conditional expression", Justification = "Suggestion would fire Load regardless of if Loaded")]
        public async Task<bool> RefreshAsync()
        {
            return Loaded ? await LoadAsync(Id) : false;
        }

        public async Task<bool> CreateAsync(bool bSendPasswordSet, int iByUserId)
        {
            if (Loaded) { return false; }

            if (iByUserId == 0)
            {
                // this can only happen if no other users on the system
                if ((await ListAsync()).Count > 0) { return false; }
            }

            if (!General.Validate_EmailAddress(Email)) { return false; }

            if (!(await Check_UniqueEmailAsync(Email))) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var strBy = (await FindAsync(iByUserId)).Name_Full;

                if (string.IsNullOrWhiteSpace(strBy)) { strBy = "System"; }

                if (TwoFA_Method == Enums.Shared_TwoFAMethod.MobileText && !Tel_Mobile_Verified) { TwoFA_Method = Enums.Shared_TwoFAMethod.Email; }

                var efmObject = new tblUser
                {
                    tblUser_email = Email.ToLower().Trim(),
                    tblUser_firstName = Name_First,
                    tblUser_lastName = Name_Last,
                    tblUser_twoFAEnabled = TwoFA_Enabled,
                    tblUser_twoFAMethod = (int)TwoFA_Method,
                    tblUser_telMobile = General.MakeFriendlyTelNo(Tel_Mobile),
                    tblUser_telMobileVerified = false,
                    tblUser_accessSysAdmin = Access_SysAdmin,
                    //tblUser_accessCanSub = Access_Sub,
                    //tblUser_accessCanSubSelf = Access_SubSelf,
                    //tblUser_accessCanPub = Access_Publish,
                    tblUser_enabled = Enabled,
                    tblUser_lastLoginUTC = null,
                    tblUser_lastLoginLocal = null,
                    tblUser_lastLoginIP = "",
                    tblUser_failedLoginTotal = 0,
                    tblUser_failedLoginLastUTC = null,
                    tblUser_failedLoginLastLocal = null,
                    tblUser_failedLoginLockUTC = null,
                    tblUser_failedLoginLockLocal = null,
                    tblUser_createdUTC = DateTime.UtcNow,
                    tblUser_createdLocal = DateTime.Now,
                    tblUser_createdBy = strBy,
                    tblUser_editedUTC = DateTime.UtcNow,
                    tblUser_editedLocal = DateTime.Now,
                    tblUser_editedBy = strBy,
                    tblUser_deletedUTC = null,
                    tblUser_deletedLocal = null,
                    tblUser_deletedBy = ""
                };

                _ = dB.tblUsers.Add(efmObject);

                if (await dB.SaveChangesAsync() > 0)
                {
                    bReturn = Load(efmObject);

                    LocalCache.RefreshCache(CacheKey);

                    if (bReturn)
                    {
                        if (bSendPasswordSet) { _ = await Password_RequestSetAsync(Id); }

                        var iAuditId = await Audit.CreateAsync(Enums.Utilities_Audit_Action.Add, Id, objNewData: this, strCreatedBy: strBy);

                        if (iByUserId != 0)
                        {
                            _ = await Activity.CreateAsync(iByUserId,
                                    Enums.Users_Activity_ActionGroup.User,
                                    Enums.Users_Activity_ActionType.Add,
                                    string.Format("Created new user {0} {1}.", efmObject.tblUser_firstName, efmObject.tblUser_lastName),
                                    iRefItemId1: efmObject.tblUser_id,
                                    iAuditId: iAuditId);
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "CreateAsync(bool, int)", ex,
                        "bSendPasswordSet: " + bSendPasswordSet.ToString() +
                        ", iByUserId: " + iByUserId.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public async Task<bool> SaveAsync(int iByUserId)
        {
            if (!Loaded) { return false; }

            if (!General.Validate_EmailAddress(Email)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblUsers.FirstOrDefaultAsync(r => r.tblUser_id == Id);

                if (efmObject != null)
                {
                    var strBy = (await FindAsync(iByUserId)).Name_Full;

                    var objForAudit = new User(efmObject);

                    if (efmObject.tblUser_email != Email.ToLower().Trim())
                    {
                        // email address has changed
                        if (!(await Check_UniqueEmailAsync(Email.ToLower().Trim())))
                        {
                            efmObject = null;
                            //objOriginal = null;
                            return false;
                        }

                        efmObject.tblUser_email = Email.Trim().ToLower();
                    }

                    efmObject.tblUser_firstName = Name_First;
                    efmObject.tblUser_lastName = Name_Last;

                    if (TwoFA_Method == Enums.Shared_TwoFAMethod.MobileText && !Tel_Mobile_Verified) { TwoFA_Method = Enums.Shared_TwoFAMethod.Email; }
                    efmObject.tblUser_twoFAEnabled = TwoFA_Enabled;
                    efmObject.tblUser_twoFAMethod = (int)TwoFA_Method;
                    efmObject.tblUser_telMobile = General.MakeFriendlyTelNo(Tel_Mobile);
                    efmObject.tblUser_telMobileVerified = Tel_Mobile_Verified;

                    efmObject.tblUser_accessSysAdmin = Access_SysAdmin;
                    //efmObject.tblUser_accessCanSub = Access_Sub;
                    //efmObject.tblUser_accessCanSubSelf = Access_SubSelf;
                    //efmObject.tblUser_accessCanPub = Access_Publish;

                    var bEnabledChanged = efmObject.tblUser_enabled != Enabled;
                    efmObject.tblUser_enabled = Enabled;

                    efmObject.tblUser_editedUTC = DateTime.UtcNow;
                    efmObject.tblUser_editedLocal = DateTime.Now;
                    efmObject.tblUser_editedBy = strBy;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = Load(efmObject);

                        LocalCache.RefreshCache(CacheKey);

                        var iAuditId = await Audit.CreateAsync(Enums.Utilities_Audit_Action.Update, Id, objNewData: this, objOldData: objForAudit, strCreatedBy: strBy);

                        // log the create against the user
                        _ = await Activity.CreateAsync(iByUserId,
                                Enums.Users_Activity_ActionGroup.User,
                                Enums.Users_Activity_ActionType.Edit,
                                iByUserId == Id ? "Updated account details." : string.Format("Updated account details of {0}.", Name_Full),
                                iRefItemId1: Id,
                                iAuditId: iAuditId);

                        if (bEnabledChanged && !Enabled)
                        {
                            // end any sessions/logins
                            _ = await Session.DeleteFull_ByUserAsync(Id);
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "SaveAsync(int)", ex,
                        "Id: " + Id.ToString() +
                        ", iByUserId: " + iByUserId.ToString());

                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> DeleteAsync(int iId, int iByUserId, bool bDeleted = true)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblUsers.FirstOrDefaultAsync(r => r.tblUser_id == iId);

                if (efmObject != null)
                {
                    if (bDeleted == efmObject.tblUser_deletedUTC.HasValue)
                    {
                        // everything is correct
                        bReturn = true;
                    }
                    else
                    {
                        var strBy = (await FindAsync(iByUserId)).Name_Full;

                        // need to make update
                        if (bDeleted)
                        {
                            efmObject.tblUser_deletedUTC = DateTime.UtcNow;
                            efmObject.tblUser_deletedLocal = DateTime.Now;
                            efmObject.tblUser_deletedBy = strBy;
                        }
                        else
                        {
                            efmObject.tblUser_deletedUTC = null;
                            efmObject.tblUser_deletedLocal = null;
                            efmObject.tblUser_deletedBy = "";
                        }

                        efmObject.tblUser_editedUTC = DateTime.UtcNow;
                        efmObject.tblUser_editedLocal = DateTime.Now;
                        efmObject.tblUser_editedBy = strBy;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            LocalCache.RefreshCache(CacheKey);

                            // log the delete against the user
                            _ = await Activity.CreateAsync(iByUserId,
                                    Enums.Users_Activity_ActionGroup.User,
                                    bDeleted ? Enums.Users_Activity_ActionType.Delete : Enums.Users_Activity_ActionType.Edit,
                                    string.Format(bDeleted ? "Deleted user {0} {1}." : "Restored user {0} {1}.", efmObject.tblUser_firstName, efmObject.tblUser_lastName),
                                    iRefItemId1: efmObject.tblUser_id);

                            if (efmObject.tblUser_deletedUTC.HasValue)
                            {
                                // delete any current sessions
                                _ = await Session.DeleteFull_ByUserAsync(iId);
                            }
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "DeleteAsync(int, int, bool)", ex,
                        "Id: " + iId.ToString() +
                        ", iByUserId: " + iByUserId.ToString() +
                        ", bDeleted: " + bDeleted.ToString());
                return false;
            }

            return bReturn;
        }

        public static async Task<bool> Password_RequestSetAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // find the user (must be enabled and not deleted)
                var efmObject = await dB.tblUsers.AsNoTracking().FirstOrDefaultAsync(r => r.tblUser_id == iId && r.tblUser_deletedUTC == null);

                if (efmObject != null)
                {
                    // check if password is already set
                    if (!string.IsNullOrWhiteSpace(efmObject.tblUser_pwd))
                    {
                        // password is already set, should be doing a reset

                    }
                    else
                    {
                        // password not set, send initial set request

                        // construct email
                        var objGSettings = Settings.Global.Fetch;
                        var objASettings = Settings.Interface.Fetch;

                        var objEmail = new Email
                        {
                            User_Id = efmObject.tblUser_id
                        };

                        objEmail.To.Add(efmObject.tblUser_email);

                        objEmail.From_Address = objGSettings.Email_Generic_FromAddress;
                        objEmail.From_Name = objGSettings.Email_Generic_FromName;
                        objEmail.Subject = "Set your password";
                        objEmail.Template = await Utilities.General.ContentFromURLAsync(objASettings.URL.TrimEnd('/') + "/email-templates/User-PasswordSet.html");

                        if (await objEmail.CreateAndSendAsync(Enums.Utilities_Email_Type.User_PasswordSet))
                        {
                            bReturn = true;
                        }

                        objEmail = null;
                        objASettings = null;
                        objGSettings = null;
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "Password_RequestSetAsync(int)", ex,
                        "iId: " + iId.ToString());
            }

            return bReturn;
        }

        public static async Task<bool> Password_SetAsync(int iId, string strPassword, int iByUserId, string strIP)
        {
            if (iByUserId == 0)
            {
                // this can only happen if only a single user on the system (as no one else to change pass of)
                if ((await ListAsync()).Count != 1)
                {
                    return false;
                }
            }

            if (string.IsNullOrWhiteSpace(strPassword)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblUsers.FirstOrDefaultAsync(r => r.tblUser_id == iId);

                if (efmObject != null)
                {
                    var strBy = (await FindAsync(iByUserId)).Name_Full;

                    if (string.IsNullOrWhiteSpace(strBy)) { strBy = "System"; }

                    efmObject.tblUser_enabled = true;
                    efmObject.tblUser_pwdSalt = Security.Argon2_CreateSalt();
                    efmObject.tblUser_pwd = Security.Argon2_CreateHash(strPassword, efmObject.tblUser_pwdSalt);
                    efmObject.tblUser_pwdLastChangedUTC = DateTime.UtcNow;
                    efmObject.tblUser_pwdLastChangedLocal = DateTime.Now;

                    efmObject.tblUser_failedLoginTotal = 0;
                    efmObject.tblUser_failedLoginLastUTC = null;
                    efmObject.tblUser_failedLoginLastLocal = null;

                    efmObject.tblUser_failedLoginLockUTC = null;
                    efmObject.tblUser_failedLoginLockLocal = null;

                    efmObject.tblUser_editedUTC = DateTime.UtcNow;
                    efmObject.tblUser_editedLocal = DateTime.Now;
                    efmObject.tblUser_editedBy = strBy;

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = true;

                        if (iByUserId > 0)
                        {
                            if (iId == iByUserId)
                            {
                                // user making update is account owner
                                _ = await Activity.CreateAsync(iByUserId,
                                        Enums.Users_Activity_ActionGroup.User,
                                        Enums.Users_Activity_ActionType.Edit,
                                        "Account password set.",
                                        strIP: strIP);
                            }
                            else
                            {
                                // user making update is another user
                                _ = await Activity.CreateAsync(iByUserId,
                                        Enums.Users_Activity_ActionGroup.User,
                                        Enums.Users_Activity_ActionType.Edit,
                                        string.Format("Account password set for {0} {1}.", efmObject.tblUser_firstName, efmObject.tblUser_lastName),
                                        iRefItemId1: efmObject.tblUser_id,
                                        strIP: strIP);
                            }
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "Password_SetAsync(int, string, int)", ex,
                        "iId: " + iId.ToString() +
                        ", iByUserId: " + iByUserId.ToString());
                return false;
            }

            return bReturn;
        }

        public static async Task<bool> Password_RequestResetAsync(string strEmail, string strIP)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                // find the user
                var strTmpEmail = strEmail.Trim().ToLower();

                var efmUser = await dB.tblUsers.AsNoTracking().FirstOrDefaultAsync(r => r.tblUser_email == strTmpEmail && r.tblUser_deletedUTC == null);

                if (efmUser != null)
                {
                    if (efmUser.tblUser_enabled)
                    {
                        if (!string.IsNullOrWhiteSpace(efmUser.tblUser_pwdSalt) && !string.IsNullOrWhiteSpace(efmUser.tblUser_pwd))
                        {
                            // user found and password previously set, do a reset request (email goes to the accounts primary address)
                            // construct email
                            var objGSettings = Settings.Global.Fetch;
                            var objASettings = Settings.Interface.Fetch;

                            var objEmail = new Email
                            {
                                User_Id = efmUser.tblUser_id
                            };

                            objEmail.To.Add(efmUser.tblUser_email);

                            objEmail.From_Address = objGSettings.Email_Generic_FromAddress;
                            objEmail.From_Name = objGSettings.Email_Generic_FromName;
                            objEmail.Subject = "Reset your password";
                            objEmail.Template = await Utilities.General.ContentFromURLAsync(objASettings.URL.TrimEnd('/') + "/email-templates/User-PasswordReset.html");

                            if (await objEmail.CreateAndSendAsync(Enums.Utilities_Email_Type.User_PasswordReset))
                            {
                                bReturn = true;

                                // log the action and clear any current sessions
                                _ = await Activity.CreateAsync(efmUser.tblUser_id, Enums.Users_Activity_ActionGroup.User, Enums.Users_Activity_ActionType.Request, "Password Reset Requested.", strIP: strIP);

                                _ = await Session.DeleteFull_ByUserAsync(efmUser.tblUser_id);
                            }

                            objEmail = null;
                            objASettings = null;
                            objGSettings = null;
                        }
                        else
                        {
                            // user is found, but no password set so needs to be a password set request
                            bReturn = await Password_RequestSetAsync(efmUser.tblUser_id);
                        }
                    }
                    else
                    {
                        // account is disabled
                        _ = await Activity.CreateAsync(efmUser.tblUser_id, Enums.Users_Activity_ActionGroup.User, Enums.Users_Activity_ActionType.Request, "Password Reset Requested.", bSuccess: false, strIP: strIP, strNote: "Account is disabled.");
                    }
                }
                else
                {
                    // no user found
                    _ = await Activity.CreateAsync(strTmpEmail, Enums.Users_Activity_ActionGroup.User, Enums.Users_Activity_ActionType.Request, "Password Reset Requested.", bSuccess: false, strIP: strIP, strNote: "Account not found.");
                }

                efmUser = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "Password_RequestResetAsync(string, string)", ex,
                        "strEmail: " + strEmail.ToString() +
                        ", strIP: " + strIP.ToString());
            }

            return bReturn;
        }

        public static async Task<bool> Password_ChangeAsync(int iId, string strCurrentPassword, string strNewPassword, string strIP)
        {
            if (string.IsNullOrWhiteSpace(strCurrentPassword)) { return false; }

            if (string.IsNullOrWhiteSpace(strNewPassword)) { return false; }

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var efmObject = await dB.tblUsers.FirstOrDefaultAsync(r => r.tblUser_id == iId);

                if (efmObject != null)
                {
                    var bPasswordVerified = false;

                    if (Security.Argon2_VerifyHash(strCurrentPassword, efmObject.tblUser_pwdSalt, efmObject.tblUser_pwd))
                    {
                        bPasswordVerified = true;
                    }

                    if (bPasswordVerified)
                    {
                        efmObject.tblUser_pwdSalt = Security.Argon2_CreateSalt();
                        efmObject.tblUser_pwd = Security.Argon2_CreateHash(strNewPassword, efmObject.tblUser_pwdSalt);
                        efmObject.tblUser_pwdLastChangedUTC = DateTime.UtcNow;
                        efmObject.tblUser_pwdLastChangedLocal = DateTime.Now;

                        efmObject.tblUser_editedUTC = DateTime.UtcNow;
                        efmObject.tblUser_editedLocal = DateTime.Now;
                        efmObject.tblUser_editedBy = efmObject.tblUser_firstName + " " + efmObject.tblUser_lastName;

                        if (await dB.SaveChangesAsync() > 0)
                        {
                            bReturn = true;

                            _ = await Activity.CreateAsync(efmObject.tblUser_id,
                                    Enums.Users_Activity_ActionGroup.User,
                                    Enums.Users_Activity_ActionType.Edit,
                                    "Changed account password.",
                                    strIP: strIP);
                        }
                    }
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "Password_ChangeAsync(int, string, string)", ex,
                    "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        public static async Task<bool> Check_UniqueEmailAsync(string strEmail)
        {
            var strEmailToUse = strEmail.Trim().ToLower();

            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                if (!(await dB.tblUsers.AsNoTracking().AnyAsync(r => r.tblUser_email == strEmailToUse)))
                {
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "Check_UniqueEmailAsync(string)", ex,
                    "strEmail: " + strEmail.ToString());
                return false;
            }

            return bReturn;
        }

        public static bool AnyDB(List<Filter<FilterOption>>? lstFilters = null)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBPooledConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var predicate = CompileWhereDB(lstFilters);

                bReturn = dB.tblUsers.AsNoTracking().AsExpandable().Any(predicate);
            }
            catch(Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "AnyDB(List<Filter<FilterOption>>?)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<User> FindAsync(int iId, bool bUseCache = true)
        {
            if (iId == 0) { return new User(); }

            if (bUseCache)
            {
                //return List().FirstOrDefault(r => r.Id == iId) ?? new User();
                return (await Global_ListAsync()).TryGetValue(iId, out var value) ? value : new User();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new User();
                await objReturn.LoadAsync(iId);

                return objReturn;
            }
        }

        public static async Task<User> FindAsync(string strEmail, bool bUseCache = true)
        {
            var strEmailToUse = strEmail.ToLower().Trim();

            if (bUseCache)
            {
                return (await ListAsync()).FirstOrDefault(r => r.Email == strEmailToUse) ?? new User();
            }
            else
            {
                // not to use cache or cache is not allowed
                var objReturn = new User();
                await objReturn.LoadAsync(strEmailToUse);

                return objReturn;
            }
        }

        #endregion finders


        #region Lists

        private static readonly SemaphoreSlim SingleCacheBuildLock = new SemaphoreSlim(1, 1);

        private static async Task<Dictionary<int, User>> Global_ListAsync()
        {
            if (LocalCache.Get(CacheKey) is not Dictionary<int, User> lstElements)
            {
                lstElements = new Dictionary<int, User>();

                try
                {
                    // lock that we are about to rebuild
                    await SingleCacheBuildLock.WaitAsync();

                    // attempt to reget incase it was rebuilt while waiting
                    if (LocalCache.Get(CacheKey) is Dictionary<int, User> lstElementsRetry)
                    {
                        return lstElementsRetry;
                    }

                    using var dB = Settings.Config.DBPooledConnection();

                    dB.ChangeTracker.AutoDetectChangesEnabled = false;

                    foreach (var efmObject in await dB.tblUsers.AsNoTracking().ToListAsync())
                    {
                        var obj = new User(efmObject);

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
                    _ = Error.Exception(typeof(User).ToString(), "Global_ListAsync()", ex);
                    return lstElements;
                }
                finally
                {
                    // release lock
                    SingleCacheBuildLock.Release();
                }
            }

            return lstElements;
        }

        public static async Task<List<User>> ListAsync(List<Filter<FilterOption>>? lstFilters = null, bool bClearCache = false)
        {
            return (await ListPagedAsync(lstFilters: lstFilters, iPageSize: 1000000, bClearCache: bClearCache)).Elements;
        }

        public static async Task<PagedData<User>> ListPagedAsync(List<Filter<FilterOption>>? lstFilters = null, int iPageSize = 25, int iPage = 1, List<SortOption>? lstOrdering = null, bool bClearCache = false)
        {
            if (bClearCache) { LocalCache.RefreshCache(CacheKey); }

            // only perform this if cache is enabled, otherwise it should have being done at DB end
            var predicate = CompileWhere(lstFilters);

            var lstElements = (await Global_ListAsync()).Values.AsQueryable().AsExpandable().Where(predicate).ToList();

            var objPagedData = new PagedData<User>(lstElements.Count, iPageSize, iPage);

            var strOrderBy = SortOption.CompileOrderBy<User>(lstOrdering, OrderByDefault, OrderByDefaultDB, false);

            objPagedData.Elements = lstElements.AsQueryable().OrderBy(strOrderBy).Skip(objPagedData.ElementsToSkip).Take(objPagedData.ElementsToTake).ToList();

            return objPagedData;
        }

        private static Expression<Func<User, bool>> CompileWhere(List<Filter<FilterOption>>? lstFilters, bool bTrue = true)
        {
            var predicate = PredicateBuilder.New<User>(true);

            if (!bTrue) { predicate = PredicateBuilder.New<User>(false); }

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
                                predicate = predicate.And(r => lstIds.Contains(r.Id));
                            }
                            break;

                        case FilterOption.Name:
                            {
                                var strSearch = vFilter.Value_String().Trim().ToLower();
                                predicate = predicate.And(r => r.Name_First.ToLower().Contains(strSearch) || r.Name_Last.ToLower().Contains(strSearch));
                            }
                            break;

                        case FilterOption.Email:
                            {
                                var strSearch = vFilter.Value_String().Trim().ToLower();
                                predicate = predicate.And(r => r.Email.Contains(strSearch));
                            }
                            break;

                        case FilterOption.Access_SysAdmin:
                            {
                                var b = vFilter.Value_Bool();
                                predicate = predicate.And(r => r.Access_SysAdmin == b);
                            }
                            break;
                        //case FilterOption.Access_Sub:
                        //    {
                        //        var b = vFilter.Value_Bool();
                        //        predicate = predicate.And(r => r.Access_Sub == b);
                        //    }
                        //    break;
                        //case FilterOption.Access_SubSelf:
                        //    {
                        //        var b = vFilter.Value_Bool();
                        //        predicate = predicate.And(r => r.Access_SubSelf == b);
                        //    }
                        //    break;
                        //case FilterOption.Access_Publish:
                        //    {
                        //        var b = vFilter.Value_Bool();
                        //        predicate = predicate.And(r => r.Access_Publish == b);
                        //    }
                        //    break;

                        case FilterOption.Enabled:
                            {
                                var b = vFilter.Value_Bool();
                                predicate = predicate.And(r => r.Enabled == b);
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

                        default: continue;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "CompileWhereDB(List<Filter<FilterOption>>)", ex);
                return predicate;
            }

            return predicate;
        }

        private static Expression<Func<tblUser, bool>> CompileWhereDB(List<Filter<FilterOption>>? lstFilters, bool bTrue = true)
        {
            var predicate = PredicateBuilder.New<tblUser>(true);

            if (!bTrue) { predicate = PredicateBuilder.New<tblUser>(false); }

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
                                predicate = predicate.And(r => lstIds.Contains(r.tblUser_id));
                            }
                            break;

                        case FilterOption.Name:
                            {
                                var strSearch = vFilter.Value_String().Trim().ToLower();
                                predicate = predicate.And(r => r.tblUser_firstName.ToLower().Contains(strSearch) || r.tblUser_lastName.ToLower().Contains(strSearch));
                            }
                            break;

                        case FilterOption.Email:
                            {
                                var strSearch = vFilter.Value_String().Trim().ToLower();
                                predicate = predicate.And(r => r.tblUser_email.Contains(strSearch));
                            }
                            break;

                        case FilterOption.Access_SysAdmin:
                            {
                                var b = vFilter.Value_Bool();
                                predicate = predicate.And(r => r.tblUser_accessSysAdmin == b);
                            }
                            break;
                        //case FilterOption.Access_Sub:
                        //    {
                        //        var b = vFilter.Value_Bool();
                        //        predicate = predicate.And(r => r.tblUser_accessCanSub == b);
                        //    }
                        //    break;
                        //case FilterOption.Access_SubSelf:
                        //    {
                        //        var b = vFilter.Value_Bool();
                        //        predicate = predicate.And(r => r.tblUser_accessCanSubSelf == b);
                        //    }
                        //    break;
                        //case FilterOption.Access_Publish:
                        //    {
                        //        var b = vFilter.Value_Bool();
                        //        predicate = predicate.And(r => r.tblUser_accessCanPub == b);
                        //    }
                        //    break;

                        case FilterOption.Enabled:
                            {
                                var b = vFilter.Value_Bool();
                                predicate = predicate.And(r => r.tblUser_enabled == b);
                            }
                            break;

                        case FilterOption.Date_Deleted_UTC:
                            {
                                var dfDates = vFilter.Value_DateFilter();

                                if (dfDates.From.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUser_deletedUTC >= dfDates.From.Value);
                                }

                                if (dfDates.To.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUser_deletedUTC <= dfDates.To.Value);
                                }

                                if (dfDates.Equal.HasValue)
                                {
                                    predicate = predicate.And(r => r.tblUser_deletedUTC == dfDates.Equal.Value);
                                }

                                if (dfDates.EqualNull.HasValue)
                                {
                                    if (dfDates.EqualNull.Value)
                                    {
                                        predicate = predicate.And(r => r.tblUser_deletedUTC == null);
                                    }
                                    else
                                    {
                                        predicate = predicate.And(r => r.tblUser_deletedUTC != null);
                                    }
                                }
                            }
                            break;

                        default: continue;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(User).ToString(), "CompileWhereDB(List<Filter<FilterOption>>)", ex);
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
                    case nameof(Name_First):
                        strObject = nameof(Name_First);
                        strDB = nameof(tblUser.tblUser_firstName);
                        break;
                    case nameof(Name_Last):
                        strObject = nameof(Name_Last);
                        strDB = nameof(tblUser.tblUser_lastName);
                        break;
                    case nameof(LastLogin_UTC):
                        strObject = nameof(LastLogin_UTC);
                        strDB = nameof(tblUser.tblUser_lastLoginUTC);
                        break;
                    case nameof(Created_UTC):
                        strObject = nameof(Created_UTC);
                        strDB = nameof(tblUser.tblUser_createdUTC);
                        break;
                    case nameof(Edited_UTC):
                        strObject = nameof(Edited_UTC);
                        strDB = nameof(tblUser.tblUser_editedUTC);
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
                _ = Error.Exception(typeof(User).ToString(), "OrderByConvert(string, bool)", ex, "strFieldName: " + strFieldName.ToString() +
                                                                                                        ", bDBVersion: " + bDBVersion);
                return "";
            }
        }

        #endregion lists
    }
}
