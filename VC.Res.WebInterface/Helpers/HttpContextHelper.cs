using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Primitives;

namespace VC.Res.WebInterface.Helpers
{
    public class HttpContextHelper
    {
        public static string GetIP(HttpContext currentContext)
        {
            if (currentContext == null) { return ""; }

            var strIPAddress = "";

            try
            {
                if (Core.Settings.Interface.Fetch.BehindProxy)
                {
                    if (currentContext.Request?.Headers?.TryGetValue("X-Forwarded-For", out var values) ?? false)
                    {
                        strIPAddress = values.ToString();   // writes out as Csv when there are multiple.

                        // check for multiple addresses and if so, split
                        if (!string.IsNullOrWhiteSpace(strIPAddress))
                        {
                            if (strIPAddress.Contains(','))
                            {
                                var strAddressSplit = strIPAddress.Split(',');

                                if (strAddressSplit.Length != 0)
                                {
                                    // multiple addresses found, use the first one in the list as that should be the client
                                    // addresses after that should be IPs of proxy/load balancers the request went through
                                    strIPAddress = strAddressSplit[0];
                                }
                            }
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(strIPAddress) && currentContext.Connection?.RemoteIpAddress != null)
                {
                    strIPAddress = currentContext.Connection.RemoteIpAddress.ToString();
                }

                if (string.IsNullOrWhiteSpace(strIPAddress))
                {
                    if (currentContext.Request?.Headers?.TryGetValue("REMOTE_ADDR", out var values) ?? false)
                    {
                        strIPAddress = values.ToString();
                    }
                }
            }
            catch
            {
                return strIPAddress;
            }

            return strIPAddress;
        }

        public static async Task<Core.Users.Session> UserSession_ValidateAsync(HttpContext currentContext, IDataProtectionProvider dataProtectionProvider)
        {
            var objReturn = new Core.Users.Session();

            try
            {
                if (!currentContext.Request.Cookies.TryGetValue("VCRes.Auth.User", out var strUserIdEnc)) { return objReturn; }

                if (string.IsNullOrWhiteSpace(strUserIdEnc)) { return objReturn; }

                // get provider to get secured elements
                var dpProvider = dataProtectionProvider.CreateProtector("VCResAuth");

                // decode user id
                if (!int.TryParse(dpProvider.Unprotect(strUserIdEnc), out var iUserId)) { return objReturn; }

                // get the user
                var objUser = await Core.Users.User.FindAsync(iUserId);

                // check user
                if (!objUser.Loaded || !objUser.Enabled || objUser.Deleted_UTC.HasValue) { return objReturn; }

                // get clients ip
                var strIP = GetIP(currentContext);

                // try and get short token and long token
                _ = currentContext.Request.Cookies.TryGetValue("VCRes.Auth.Token", out var strSessionIdEnc);
                _ = currentContext.Request.Cookies.TryGetValue("VCRes.Auth.LToken", out var strSessionKey4);

                if (!string.IsNullOrWhiteSpace(strSessionIdEnc))
                {
                    // short term token available, try and validate first
                    if (int.TryParse(dpProvider.Unprotect(strSessionIdEnc), out var iSessionId))
                    {
                        // get the user and the session
                        objReturn = await Core.Users.Session.FindAsync(iSessionId);

                        // check session
                        if (!objReturn.Loaded || objReturn.Type != Core.Enums.Users_Session_Type.Web || !objReturn.Authenticated || !objReturn.Claimed || objReturn.User_Id != objUser.Id || objReturn.Created_IP != strIP)
                        {
                            // session isn't valid
                            objReturn = new Core.Users.Session();
                        }
                        else
                        {
                            // check second cookie
                            if (!currentContext.Request.Cookies.TryGetValue(objReturn.Key1, out var strKey2Enc))
                            {
                                // session isn't valid
                                objReturn = new Core.Users.Session();
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(strKey2Enc))
                                {
                                    // session isn't valid
                                    objReturn = new Core.Users.Session();
                                }
                                else
                                {
                                    if (dpProvider.Unprotect(strKey2Enc) != objReturn.Key2)
                                    {
                                        // session isn't valid
                                        objReturn = new Core.Users.Session();
                                    }
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(strSessionKey4) && !objReturn.Loaded)
                {
                    // long term token available and no session loaded yet, try and validate
                    var strKey4 = dpProvider.Unprotect(strSessionKey4);

                    if (!string.IsNullOrWhiteSpace(strKey4))
                    {
                        objReturn = await Core.Users.Session.Find_ByLongTermAsync(objUser.Id, strKey4);

                        if (!objReturn.Loaded || objReturn.Type != Core.Enums.Users_Session_Type.Web || !objReturn.Authenticated || !objReturn.Claimed)
                        {
                            // session isn't valid
                            objReturn = new Core.Users.Session();

                            // delete cookies
                            currentContext.Response.Cookies.Delete("VCRes.Auth.User");
                            currentContext.Response.Cookies.Delete("VCRes.Auth.LToken");
                        }
                        else
                        {
                            // session is valid... delete it and setup a new one
                            _ = await Core.Users.Session.DeleteFullAsync(objReturn.Id);

                            objReturn = await Core.Users.Session.CreateAsync(Core.Enums.Users_Session_Type.Web, objUser.Id, strIP, bAuthenticated: true, bClaimed: true);
                        }
                    }
                }

                // if session is loaded... ok to set cookies
                if (objReturn.Loaded)
                {
                    if (await Core.Users.Session.Update_LastActivityAsync(objReturn.Id, strIP))
                    {
                        // update the cookie
                        currentContext.Response.Cookies.Append("VCRes.Auth.User",
                            dpProvider.Protect(objUser.Id.ToString()),
                            new CookieOptions
                            {
                                Secure = true,
                                HttpOnly = true,
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTime.Now.AddDays(Core.Settings.Variables.SessionExpire_LongTermDays),
                                IsEssential = true
                            });

                        currentContext.Response.Cookies.Append("VCRes.Auth.LToken",
                            dpProvider.Protect(objReturn.Key4),
                            new CookieOptions
                            {
                                Secure = true,
                                HttpOnly = true,
                                SameSite = SameSiteMode.Strict,
                                Expires = DateTime.Now.AddDays(Core.Settings.Variables.SessionExpire_LongTermDays),
                                IsEssential = true
                            });

                        currentContext.Response.Cookies.Append("VCRes.Auth.Token",
                            dpProvider.Protect(objReturn.Id.ToString()),
                            new CookieOptions
                            {
                                Secure = true,
                                HttpOnly = true,
                                SameSite = SameSiteMode.Strict,
                                //Expires = DateTime.Now.AddMinutes(Core.Settings.Variables.SessionExpire_ShortTermMinutes),
                                IsEssential = true
                            });

                        currentContext.Response.Cookies.Append(objReturn.Key1,
                            dpProvider.Protect(objReturn.Key2),
                            new CookieOptions
                            {
                                Secure = true,
                                HttpOnly = true,
                                SameSite = SameSiteMode.Strict,
                                //Expires = DateTime.Now.AddMinutes(Core.Settings.Variables.SessionExpire_ShortTermMinutes),
                                IsEssential = true
                            });
                    }
                }

                objUser = null;
                dpProvider = null;
            }
            catch (Exception ex)
            {
                _ = Core.Error.Exception(typeof(HttpContextHelper).ToString(), "UserSession_ValidateAsync(HttpContext, IDataProtectionProvider)", ex);
                return objReturn;
            }

            return objReturn;
        }
    }
}
