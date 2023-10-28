using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using VC.Res.Core.Database;
using Z.EntityFramework.Plus;

namespace VC.Res.Core.Utilities
{
    public class Email
    {

        #region Properties

        public bool Loaded { get; private set; } = false;

        public int Id { get; private set; } = 0;

        public Enums.Utilities_Email_Type Type { get; private set; } = Enums.Utilities_Email_Type.Unknown;

        public string Key { get; private set; } = "";

        public int? User_Id { get; set; } = null;

        public int? ForeignRef { get; set; } = null;

        public List<string> To { get; set; } = new List<string>();

        public List<string> CC { get; set; } = new List<string>();

        public List<string> BCC { get; set; } = new List<string>();

        public string Subject { get; set; } = "";

        public string From_Name { get; set; } = "";

        public string From_Address { get; set; } = "";

        public string Template { get; set; } = "";

        public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, byte[]> Attachments { get; set; } = new Dictionary<string, byte[]>();

        public DateTime Created_UTC { get; private set; } = DateTime.UtcNow;

        public DateTime Created_Local { get; private set; } = DateTime.Now;

        public DateTime? Sent_UTC { get; private set; } = null;

        public DateTime? Sent_Local { get; private set; } = null;

        public DateTime? Used_UTC { get; private set; } = null;

        public DateTime? Used_Local { get; private set; } = null;

        public DateTime? Expires_UTC { get; private set; } = null;

        public DateTime? Expires_Local { get; private set; } = null;

        public bool CanBeUsed
        {
            get
            {
                switch (Type)
                {
                    case Enums.Utilities_Email_Type.User_AuthCode:
                    case Enums.Utilities_Email_Type.User_PasswordSet:
                    case Enums.Utilities_Email_Type.User_PasswordReset:
                        {
                            if (Used_UTC.HasValue) { return false; }

                            if (!Expires_UTC.HasValue || Expires_UTC >= DateTime.UtcNow) { return true; }
                        }
                        break;

                    default: return false;
                }

                return false;
            }
        }

        #endregion properties


        #region Constructors

        public Email() { }

        //private Email(int iId) { _ = Load(iId); }

        //private Email(Enums.Utilities_Email_Type enumType, string strKey) { _ = Load(enumType, strKey); }

        //private Email(tblSysEmail efmObject) { _ = Load(efmObject); }

        #endregion constructors


        #region Private Functions

        #region Private Functions-Loaders

        private async Task<bool> LoadAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblSysEmails.AsNoTracking().FirstOrDefaultAsync(r => r.tblSysEmail_id == iId);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "LoadAsync(int)", ex, "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private async Task<bool> LoadAsync(Enums.Utilities_Email_Type enumType, string strKey)
        {
            if (enumType == Enums.Utilities_Email_Type.Unknown || string.IsNullOrWhiteSpace(strKey)) { return false; }

            var bReturn = false;

            try
            {
                var strKeyToFind = strKey.Trim().ToLower();

                using var dB = Settings.Config.DBConnection();

                dB.ChangeTracker.AutoDetectChangesEnabled = false;

                var efmObject = await dB.tblSysEmails.AsNoTracking().FirstOrDefaultAsync(r => r.tblSysEmail_type == (int)enumType && r.tblSysEmail_key == strKeyToFind);

                if (efmObject != null)
                {
                    bReturn = Load(efmObject);
                }

                efmObject = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "LoadAsync(Enums.Utilities_Email_Type, string)", ex,
                    "enumType: " + ((int)enumType).ToString() +
                    ", strKey: " + strKey.ToString());
                return false;
            }

            return bReturn;
        }

        private bool Load(tblSysEmail efmObject)
        {
            var bReturn = false;

            try
            {
                if (efmObject != null)
                {
                    Id = efmObject.tblSysEmail_id;

                    Type = (Enums.Utilities_Email_Type)efmObject.tblSysEmail_type;
                    Key = efmObject.tblSysEmail_key;

                    User_Id = efmObject.tblUser_id;
                    ForeignRef = efmObject.tblSysEmail_foreignRef;

                    To = General.ConvertToListString(efmObject.tblSysEmail_to);
                    CC = General.ConvertToListString(efmObject.tblSysEmail_cc);
                    BCC = General.ConvertToListString(efmObject.tblSysEmail_bcc);

                    Subject = efmObject.tblSysEmail_subject;

                    From_Name = efmObject.tblSysEmail_fromName;
                    From_Address = efmObject.tblSysEmail_fromAddress;

                    Template = efmObject.tblSysEmail_template;

                    Variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(efmObject.tblSysEmail_variables);

                    Attachments = JsonConvert.DeserializeObject<Dictionary<string, byte[]>>(efmObject.tblSysEmail_attachments);

                    Created_UTC = efmObject.tblSysEmail_createdUtc;
                    Created_Local = efmObject.tblSysEmail_createdLocal;
                    Sent_UTC = efmObject.tblSysEmail_sentUtc;
                    Sent_Local = efmObject.tblSysEmail_sentLocal;
                    Used_UTC = efmObject.tblSysEmail_usedUtc;
                    Used_Local = efmObject.tblSysEmail_usedLocal;
                    Expires_UTC = efmObject.tblSysEmail_expiresUtc;
                    Expires_Local = efmObject.tblSysEmail_expiresLocal;

                    Loaded = true;
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "Load(tblSysEmail)", ex);
                return false;
            }

            return bReturn;
        }

        #endregion private functions-loaders

        private static async Task<bool> Update_SentAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var iUpdated = await dB.tblSysEmails.Where(r => r.tblSysEmail_id == iId)
                                                    .UpdateAsync(r => new tblSysEmail
                                                    {
                                                        tblSysEmail_sentUtc = DateTime.UtcNow,
                                                        tblSysEmail_sentLocal = DateTime.Now
                                                    });

                if (iUpdated > 0)
                {
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "Update_SentAsync(int)", ex,
                    "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        private static async Task ExpirePreviousAsync(Enums.Utilities_Email_Type enumType, int? iUserId)
        {
            try
            {
                using var dB = Settings.Config.DBConnection();

                var iUpdated = await dB.tblSysEmails.Where(r => r.tblSysEmail_type == (int)enumType && r.tblUser_id == iUserId && r.tblSysEmail_expiresUtc == null)
                                                    .UpdateAsync(r => new tblSysEmail
                                                    {
                                                        tblSysEmail_expiresUtc = DateTime.UtcNow,
                                                        tblSysEmail_expiresLocal = DateTime.Now
                                                    });
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "ExpirePreviousAsync(Enums.Utilities_Email_Type, int)", ex,
                    "enumType: " + ((int)enumType).ToString() +
                    ", iUserId: " + iUserId.ToString());
            }
        }

        #endregion private functions


        #region Internal Functions



        #endregion internal functions


        #region Public Functions

        public async Task<bool> RefreshAsync()
        {
            if (Loaded) { return await LoadAsync(Id); }

            return false;
        }

        public List<string> Verify(Enums.Utilities_Email_Type enumType)
        {
            var lstReturn = new List<string>();

            // checks on type
            switch (enumType)
            {
                case Enums.Utilities_Email_Type.Unknown:
                    lstReturn.Add("Unknown email type.");
                    break;

                case Enums.Utilities_Email_Type.User_PasswordSet:
                case Enums.Utilities_Email_Type.User_PasswordReset:
                case Enums.Utilities_Email_Type.User_AuthCode:
                    // need a user id
                    if (!User_Id.HasValue) { lstReturn.Add("No user id set."); }
                    break;

                default: break;
            }

            // check to
            if (To.Count < 1)
            {
                // no to addresses
                lstReturn.Add("No To address set.");
            }
            else
            {
                foreach (var vTo in To)
                {
                    if (!General.Validate_EmailAddress(vTo))
                    {
                        lstReturn.Add(vTo + " is not a valid email address.");
                    }
                }
            }

            // check cc addresses
            foreach (var vAddress in CC)
            {
                if (!General.Validate_EmailAddress(vAddress))
                {
                    lstReturn.Add("CC adddress " + vAddress + " is not a valid email address.");
                }
            }

            // check bcc addresses
            foreach (var vAddress in BCC)
            {
                if (!General.Validate_EmailAddress(vAddress))
                {
                    lstReturn.Add("BCC adddress " + vAddress + " is not a valid email address.");
                }
            }

            if (string.IsNullOrWhiteSpace(Subject)) { lstReturn.Add("No subject added."); }

            if (!General.Validate_EmailAddress(From_Address)) { lstReturn.Add("No valid from address set."); }

            if (string.IsNullOrWhiteSpace(Template)) { lstReturn.Add("No template content set."); }

            if (lstReturn.Count > 0)
            {
                // one or more errors, should log incase we need to lookup
                _ = Error.Generic(typeof(Email).ToString(), "Verify()",
                    JsonConvert.SerializeObject(this),
                    "lstReturn: " + General.ConvertToCommaString(lstReturn));
            }

            return lstReturn;
        }

        public async Task<bool> CreateAndSendAsync(Enums.Utilities_Email_Type enumType)
        {
            if (await CreateAsync(enumType))
            {
                return await SendAsync();
            }

            return false;
        }

        public async Task<bool> CreateAsync(Enums.Utilities_Email_Type enumType)
        {
            var bReturn = false;

            try
            {
                using (var dB = Settings.Config.DBConnection())
                {
                    var efmObject = new tblSysEmail
                    {
                        tblSysEmail_type = (int)enumType,
                        tblSysEmail_key = Guid.NewGuid().ToString("N"),
                        tblUser_id = User_Id,
                        tblSysEmail_foreignRef = ForeignRef,
                        tblSysEmail_to = General.ConvertToCommaString(To),
                        tblSysEmail_cc = General.ConvertToCommaString(CC),
                        tblSysEmail_bcc = General.ConvertToCommaString(BCC),
                        tblSysEmail_subject = Subject,
                        tblSysEmail_fromName = From_Name,
                        tblSysEmail_fromAddress = From_Address,
                        tblSysEmail_template = Template,
                        tblSysEmail_variables = JsonConvert.SerializeObject(Variables),
                        tblSysEmail_attachments = JsonConvert.SerializeObject(Attachments),
                        tblSysEmail_createdUtc = DateTime.UtcNow,
                        tblSysEmail_createdLocal = DateTime.Now,
                        tblSysEmail_sentUtc = null,
                        tblSysEmail_sentLocal = null,
                        tblSysEmail_usedUtc = null,
                        tblSysEmail_usedLocal = null,
                        tblSysEmail_expiresUtc = null,
                        tblSysEmail_expiresLocal = null
                    };

                    // configure any type specific elements
                    switch (enumType)
                    {
                        case Enums.Utilities_Email_Type.User_AuthCode:
                            // valid for 2 hours
                            efmObject.tblSysEmail_expiresUtc = DateTime.UtcNow.AddHours(2);
                            efmObject.tblSysEmail_expiresLocal = DateTime.Now.AddHours(2);

                            // expire old ones
                            if (User_Id.HasValue) { await ExpirePreviousAsync(enumType, User_Id.Value); }
                            break;

                        case Enums.Utilities_Email_Type.User_PasswordSet:
                            // valid for 1 day
                            efmObject.tblSysEmail_expiresUtc = DateTime.UtcNow.AddDays(1);
                            efmObject.tblSysEmail_expiresLocal = DateTime.Now.AddDays(1);

                            // expire old ones
                            if (User_Id.HasValue) { await ExpirePreviousAsync(enumType, User_Id.Value); }
                            break;

                        case Enums.Utilities_Email_Type.User_PasswordReset:
                            // valid for 4 hours
                            efmObject.tblSysEmail_expiresUtc = DateTime.UtcNow.AddHours(4);
                            efmObject.tblSysEmail_expiresLocal = DateTime.Now.AddHours(4);

                            // expire old ones
                            if (User_Id.HasValue) { await ExpirePreviousAsync(enumType, User_Id.Value); }
                            break;

                        default: break;
                    }

                    _ = dB.tblSysEmails.Add(efmObject);

                    if (await dB.SaveChangesAsync() > 0)
                    {
                        bReturn = Load(efmObject);
                    }

                    efmObject = null;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "CreateAsync(Enums.Utilities_Email_Type)", ex,
                    "enumType: " + ((int)enumType).ToString() +
                    ", this: " + JsonConvert.SerializeObject(this));
                return false;
            }

            return bReturn;
        }

        public async Task<bool> SendAsync()
        {
            // if we can't verify, we can't send
            if (Verify(Type).Count > 0) { return false; }

            if (Sent_UTC.HasValue)
            {
                // already sent
                return false;
            }

            var bReturn = false;

            try
            {
                // compile the content
                var strContent = Template;

                // replace portal link first as going to fill in with other variables
                strContent = strContent.Replace("/:InterfaceGatewayURL:", ":InterfaceURL:/gateway?view=email&type=:EmailType:&key=:EmailKey:");
                strContent = strContent.Replace(":InterfaceGatewayURL:", ":InterfaceURL:/gateway?view=email&type=:EmailType:&key=:EmailKey:");

                foreach (var variable in Variables)
                {
                    strContent = strContent.Replace(string.Format(":{0}:", variable.Key), variable.Value);
                }

                // replace set variables
                strContent = strContent.Replace(":InterfaceURL:", Settings.Interface.Fetch.URL.TrimEnd('/'));
                strContent = strContent.Replace(":EmailType:", ((int)Type).ToString());
                strContent = strContent.Replace(":EmailKey:", Key);

                strContent = strContent.Replace(":EmailLogo:", string.Format("{0}?width=300&rmode=max&bgcolor=ffffff&format=jpg&quality=85", string.IsNullOrWhiteSpace(Settings.Global.Fetch.Email_Logo) ? "/imgs/Email-Logo.png" : Settings.Global.Fetch.Email_Logo));

                // construct email
                var mmEmail = new MailMessage
                {
                    From = new MailAddress(From_Address, From_Name),
                    Subject = Subject,
                    Body = strContent,
                    IsBodyHtml = true
                };

                // process who the message is to
                foreach (var vTo in To)
                {
                    mmEmail.To.Add(vTo);
                }

                foreach (var vTo in CC)
                {
                    mmEmail.CC.Add(vTo);
                }

                foreach (var vTo in BCC)
                {
                    mmEmail.Bcc.Add(vTo);
                }

                // add any attachments
                foreach (var vAttachment in Attachments)
                {
                    mmEmail.Attachments.Add(new Attachment(new MemoryStream(vAttachment.Value), vAttachment.Key));
                }

                // send the email
                var objSettings = Settings.Global.Fetch;

                var smtpClient = new SmtpClient(objSettings.Email_Server_Address, objSettings.Email_Server_Port)
                {
                    EnableSsl = objSettings.Email_Server_TLS
                };

                if (objSettings.Email_Server_ReqAuth)
                {
                    smtpClient.Credentials = new NetworkCredential(objSettings.Email_Server_Username, objSettings.Email_Server_Password);
                }

                await smtpClient.SendMailAsync(mmEmail);

                _ = await Update_SentAsync(Id);

                bReturn = true;

                mmEmail.Dispose();
                smtpClient.Dispose();

                objSettings = null;
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "SendAsync()", ex,
                    "Id: " + Id.ToString());
                return bReturn;
            }

            return bReturn;
        }

        public static async Task<bool> Update_UsedAsync(int iId)
        {
            var bReturn = false;

            try
            {
                using var dB = Settings.Config.DBConnection();

                var iUpdated = await dB.tblSysEmails.Where(r => r.tblSysEmail_id == iId)
                                                    .UpdateAsync(r => new tblSysEmail
                                                    {
                                                        tblSysEmail_usedUtc = DateTime.UtcNow,
                                                        tblSysEmail_usedLocal = DateTime.Now
                                                    });

                if (iUpdated > 0)
                {
                    bReturn = true;
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "Update_UsedAsync(int)", ex,
                    "iId: " + iId.ToString());
                return false;
            }

            return bReturn;
        }

        #endregion public functions


        #region Finders

        public static async Task<Email> FindAsync(int iId)
        {
            var objReturn = new Email();
            await objReturn.LoadAsync(iId);

            return objReturn;
        }

        public static async Task<Email> FindAsync(Enums.Utilities_Email_Type enumType, string strKey)
        {
            var objReturn = new Email();
            await objReturn.LoadAsync(enumType, strKey);

            return objReturn;
        }

        #endregion finders


        #region Lists



        #endregion lists
    }
}
