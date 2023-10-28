using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VC.Res.WebInterface.Pages.Auth
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class LogoutModel : PageModel
    {
        private readonly IDataProtector _protector;

        public LogoutModel(IDataProtectionProvider dpProvider)
        {
            this._protector = dpProvider.CreateProtector("VCResAuth");
        }

        public async Task OnGet()
        {
            if (HttpContext.Request.Cookies.TryGetValue("VCRes.Auth.Token", out var strSessionIdEnc))
            {
                if (strSessionIdEnc != null)
                {
                    if (int.TryParse(_protector.Unprotect(strSessionIdEnc), out var iSessionId))
                    {
                        // delete the session
                        _ = await VC.Res.Core.Users.Session.DeleteFullAsync(iSessionId);
                    }
                }
            }

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            Response.Redirect("/");
        }
    }
}
