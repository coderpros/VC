using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VC.Res.WebInterface.Pages
{
    public class GatewayModel : PageModel
    {
        IDataProtector _protector;
        public GatewayModel(IDataProtectionProvider dpProvider)
        {
            this._protector = dpProvider.CreateProtector("VCResAuth");
        }

        [FromQuery(Name = "view")]
        public string View { get; set; } = "";

        [FromQuery(Name = "type")]
        public int Email_Type { get; set; } = 0;

        [FromQuery(Name = "key")]
        public string Email_Key { get; set; } = "";

        public async Task OnGet()
        {
            // if using a portal link, shouldn't be logged in
            if (HttpContext.Request.Cookies.TryGetValue("VCRes.Auth.Token", out var strSessionIdEnc))
            {
                if (strSessionIdEnc != null)
                {
                    if (int.TryParse(_protector.Unprotect(strSessionIdEnc), out var iSessionId))
                    {
                        // delete the session
                        _ = await Core.Users.Session.DeleteFullAsync(iSessionId);
                    }
                }
            }

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            switch (View.ToLower())
            {
                case "email":
                    {
                        // find the email
                        var objEmail = await Core.Utilities.Email.FindAsync((Core.Enums.Utilities_Email_Type)Email_Type, Email_Key);

                        if (objEmail.Loaded && objEmail.CanBeUsed)
                        {
                            switch (objEmail.Type)
                            {
                                case Core.Enums.Utilities_Email_Type.User_PasswordSet:
                                    Response.Redirect(Core.UrlHelpers.Interface.URL_Get(Core.UrlHelpers.Interface.Pages.Password_Set, dicQuerystringParms: new Dictionary<string, string> { { "id", Core.Utilities.Security.EncryptString(objEmail.Id.ToString()) }, { "key", objEmail.Key } }));
                                    break;

                                case Core.Enums.Utilities_Email_Type.User_PasswordReset:
                                    Response.Redirect(Core.UrlHelpers.Interface.URL_Get(Core.UrlHelpers.Interface.Pages.Password_Reset, dicQuerystringParms: new Dictionary<string, string> { { "id", Core.Utilities.Security.EncryptString(objEmail.Id.ToString()) }, { "key", objEmail.Key } }));
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
