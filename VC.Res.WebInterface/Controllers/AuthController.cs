using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VC.Res.WebInterface.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        IDataProtectionProvider _protectorProvider;
        public AuthController(IDataProtectionProvider dpProvider)
        {
            this._protectorProvider = dpProvider;
        }

        [HttpGet]
        [Route("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var objSession = await VC.Res.WebInterface.Helpers.HttpContextHelper.UserSession_ValidateAsync(HttpContext, _protectorProvider);

            return Ok(objSession.Loaded);
        }
    }
}
