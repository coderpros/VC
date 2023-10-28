namespace VC.Res.Core.UrlHelpers
{
    using System.Text;
    using System.Web;

    public class Interface : UrlHelperBase
    {
        public enum Pages
        {
            Unknown,
            Login,
            Install,
            Gateway,
            Password_Forgot,
            Password_Set,
            Password_Reset,

            Auth_Login,
            Auth_Logout,
            Auth_Refresh,

            Dashboard,

            Contacts_List,
            Contacts_Edit,

            Countries_List,
            Countries_Edit,

            Currencies_List,

            Invoices_List,
            Invoices_Edit,

            Premises_List,
            Premises_Edit,
            Premises_Groups_List,
            Premises_Groups_Edit,
            Premises_Quote,
            Premises_Seasons_Add,
            Premises_Seasons_Edit,

            Tags_List,
            Tags_Edit,

            Collections_List,
            Collections_Edit,

            Users_List,
            Users_Edit,

            Config,
            Tools,
        };

        protected static readonly Dictionary<string, Pages> URLRules = new()
        {
            { "", Pages.Login },
            { "install", Pages.Install },
            { "gateway", Pages.Gateway },
            { "forgot-password", Pages.Password_Forgot },
            { "set-password", Pages.Password_Set },
            { "reset-password", Pages.Password_Reset },

            { "auth/login", Pages.Auth_Login },
            { "auth/logout", Pages.Auth_Logout },
            { "auth/refresh", Pages.Auth_Refresh },

            { "dashboard", Pages.Dashboard },

            { "contacts", Pages.Contacts_List },
            { "contacts/edit", Pages.Contacts_Edit },

            { "countries", Pages.Countries_List },
            { "countries/edit", Pages.Countries_Edit },

            { "currencies", Pages.Currencies_List },

            { "invoices", Pages.Invoices_List },
            { "invoices/edit", Pages.Invoices_Edit },

            { "properties", Pages.Premises_List },
            { "properties/edit", Pages.Premises_Edit },
            { "properties/quote", Pages.Premises_Quote },

            { "properties/groups", Pages.Premises_Groups_List },
            { "properties/groups/edit", Pages.Premises_Groups_Edit },

            { "properties/seasons/add", Pages.Premises_Seasons_Add },
            { "properties/seasons/edit", Pages.Premises_Seasons_Edit },

            { "collections", Pages.Collections_List },
            { "collections/edit", Pages.Collections_Edit},

            { "tags", Pages.Tags_List },
            { "tags/edit", Pages.Tags_Edit },

            { "users", Pages.Users_List },
            { "users/edit", Pages.Users_Edit },

            { "config", Pages.Config },
            { "tools", Pages.Tools }
        };

        public static string URL_Get(Pages enumRule, int? iParameterId1 = null, int? iParameterId2 = null, bool bFull = false, Dictionary<string, string>? dicQuerystringParms = null)
        {
            var strReturn = "/";

            try
            {
                if (URLRules.ContainsValue(enumRule))
                {
                    strReturn += URLRules.FirstOrDefault(r => r.Value == enumRule).Key;
                }

                if (iParameterId1.HasValue)
                {
                    strReturn += "/" + iParameterId1.Value.ToString();

                    if (iParameterId2.HasValue)
                    {
                        strReturn += "/" + iParameterId2.Value.ToString();
                    }
                }

                var strQuerystring = "";

                if (dicQuerystringParms != null)
                {
                    foreach (var dicEntry in dicQuerystringParms)
                    {
                        if (!string.IsNullOrWhiteSpace(dicEntry.Value))
                        {
                            strQuerystring += "&" + dicEntry.Key + "=" + HttpUtility.UrlEncode(dicEntry.Value);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(strQuerystring))
                {
                    strReturn = strReturn + "?" + strQuerystring.TrimStart('&');
                }
            }
            catch (Exception)
            {
                return bFull ? Settings.Interface.Fetch.URL.TrimEnd('/') + strReturn : strReturn;
            }

            return bFull ? Settings.Interface.Fetch.URL.TrimEnd('/') + strReturn : strReturn;
        }

        public static Pages URLProcess_CurrentRule(string strUrl)
        {
            var enumReturn = Pages.Unknown;

            try
            {
                var strURL = new StringBuilder();
                //var lstPages = ListPages();

                foreach (var str in URLProcess_PagesList(strUrl))
                {
                    // check the element doesn't perfect map to a number, as probably a id parameter
                    if (!int.TryParse(str, out var iId))
                    {
                        _ = strURL.Append("/" + str);
                    }
                }

                if (URLRules.ContainsKey(strURL.ToString().TrimStart('/')))
                {
                    enumReturn = URLRules[strURL.ToString().TrimStart('/')];
                }
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Interface).ToString(), "URLProcess_CurrentRule()", ex,
                    "strUrl: " + strUrl);
                return Pages.Unknown;
            }

            return enumReturn;
        }

        public static bool HasAccess(Pages enumRule, Users.User user)
        {
            var bReturn = false;

            switch (enumRule)
            {
                case Pages.Gateway:
                case Pages.Login:
                case Pages.Password_Forgot:
                case Pages.Password_Set:
                case Pages.Password_Reset:
                    // these pages either always have access or determined on the page themselves
                    return true;

                default:
                    break;
            }

            if (enumRule == Pages.Install)
            {
                // only allow access if no users on the system (at all)
                if (!Users.User.AnyDB()) { return true; }

                return false;
            }

            // from here they need to be a user
            if (user == null) { return false; }

            if (!user.Loaded || !user.Enabled || user.Deleted_UTC.HasValue) { return false; }


            switch (enumRule)
            {
                case Pages.Dashboard:
                    return true;

                case Pages.Contacts_List:
                case Pages.Contacts_Edit:
                    bReturn = true;
                    break;

                case Pages.Countries_List:
                case Pages.Countries_Edit:
                    if (user.Access_SysAdmin) { return true; }
                    break;

                case Pages.Currencies_List:
                    if (user.Access_SysAdmin) { return true; }
                    break;

                case Pages.Invoices_List:
                case Pages.Invoices_Edit:
                    bReturn = true;
                    break;

                case Pages.Collections_List:
                case Pages.Collections_Edit:
                case Pages.Premises_List:
                case Pages.Premises_Edit:
                case Pages.Premises_Quote:
                case Pages.Premises_Seasons_Add:
                case Pages.Premises_Seasons_Edit:
                    bReturn = true;
                    break;
                case Pages.Premises_Groups_List:
                case Pages.Premises_Groups_Edit:
                    bReturn = true;
                    break;

                case Pages.Tags_List:
                case Pages.Tags_Edit:
                    bReturn = true;
                    break;

                case Pages.Users_List:
                case Pages.Users_Edit:
                    if (user.Access_SysAdmin) { return true; }
                    break;

                case Pages.Config:
                case Pages.Tools:
                    if (user.Access_SysAdmin) { return true; }
                    break;

                default: break;
            }

            return bReturn;
        }
    }
}
