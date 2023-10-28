using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VC.Res.WebInterface.Pages.Auth
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class LoginModel : PageModel
    {
        private readonly IDataProtector _protector;

        public LoginModel(IDataProtectionProvider dpProvider)
        {
            this._protector = dpProvider.CreateProtector("VCResAuth");
        }

        [FromQuery(Name = "t1")]
        public string Token_Id { get; set; } = "";

        [FromQuery(Name = "t2")]
        public string Token_Key3 { get; set; } = "";

        [FromQuery(Name = "rtnpath")]
        public string ReturnPath { get; set; } = "";

        public async Task OnGet()
        {
            if (Token_Id == null || Token_Key3 == null) { return; }

            if (!int.TryParse(Core.Utilities.Security.DecryptString(Token_Id), out var tokenId)) { return; }

            var objSession = await Core.Users.Session.Find_ByClaimAsync(tokenId, Core.Utilities.Security.DecryptString(Token_Key3), Helpers.HttpContextHelper.GetIP(HttpContext));

            if (objSession.Loaded)
            {
                // claim the session
                var tskUpdateClaimed = Core.Users.Session.Update_ClaimedAsync(objSession.Id, true);

                // add auth cookies
                HttpContext.Response.Cookies.Append("VCRes.Auth.User",
                        _protector.Protect(objSession.User_Id.ToString()),
                        new CookieOptions
                        {
                            Secure = true,
                            HttpOnly = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.Now.AddDays(Core.Settings.Variables.SessionExpire_LongTermDays),
                            IsEssential = true
                        });

                HttpContext.Response.Cookies.Append("VCRes.Auth.LToken",
                    _protector.Protect(objSession.Key4),
                    new CookieOptions
                    {
                        Secure = true,
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddDays(Core.Settings.Variables.SessionExpire_LongTermDays),
                        IsEssential = true
                    });

                HttpContext.Response.Cookies.Append("VCRes.Auth.Token",
                    _protector.Protect(objSession.Id.ToString()),
                    new CookieOptions
                    {
                        Secure = true,
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        //Expires = DateTime.Now.AddMinutes(Core.Settings.Variables.SessionExpire_ShortTermMinutes),
                        IsEssential = true
                    });

                HttpContext.Response.Cookies.Append(objSession.Key1,
                    _protector.Protect(objSession.Key2),
                    new CookieOptions
                    {
                        Secure = true,
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        //Expires = DateTime.Now.AddMinutes(Core.Settings.Variables.SessionExpire_ShortTermMinutes),
                        IsEssential = true
                    });

                await tskUpdateClaimed;

                var bDefaultRedirect = true;

                if (!string.IsNullOrWhiteSpace(ReturnPath))
                {
                    if (!ReturnPath.ToLower().StartsWith("http") && !ReturnPath.ToLower().StartsWith("www"))
                    {
                        bDefaultRedirect = false;
                        HttpContext.Response.Redirect(ReturnPath);
                    }
                }

                // redirect to dashboard
                if (bDefaultRedirect)
                {
                    Response.Redirect("/dashboard");
                }
            }

            objSession = null;
        }
    }
}
