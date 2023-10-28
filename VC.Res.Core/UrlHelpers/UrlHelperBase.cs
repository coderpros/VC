namespace VC.Res.Core.UrlHelpers
{
    public class UrlHelperBase
    {
        public static string URLProcess_PagesString(string strUrl)
        {
            var strRawUrl = strUrl.TrimStart('/').TrimEnd('/').ToLower();

            if (strRawUrl.Contains('?'))
            {
                if (strRawUrl.LastIndexOf("?") > 0)
                {
                    strRawUrl = strRawUrl.Substring(0, strRawUrl.LastIndexOf("?")).TrimEnd('/');
                }
                else
                {
                    strRawUrl = "";
                }
            }

            if (strRawUrl.Contains('.'))
            {
                strRawUrl = strRawUrl.Substring(0, strRawUrl.LastIndexOf("."));
            }

            //if (string.IsNullOrWhiteSpace(strRawUrl) || (strRawUrl == "default"))
            //{
            //    strRawUrl = "home";
            //}

            return strRawUrl;
        }

        public static List<string> URLProcess_PagesList(string strUrl)
        {
            var thePageStartName = URLProcess_PagesString(strUrl);

            try
            {
                if (thePageStartName.Contains('/'))
                {
                    // multiple pages
                    return thePageStartName.Split('/').ToList();
                }
                else
                {
                    // single page
                    return new List<string> { thePageStartName };
                }
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public static string URLProcess_Extension(string strUrl)
        {
            var strExtension = strUrl.TrimStart('/').TrimEnd('/').ToLower();

            // check if url has parameters
            if (strExtension.Contains('?'))
            {
                foreach (var strTemp in strExtension.Split('?'))
                {
                    if (strTemp.Contains('.'))
                    {
                        return strTemp.Substring(strTemp.LastIndexOf(".") + 1, strTemp.Length - (strTemp.LastIndexOf(".") + 1));
                    }
                }
            }

            // check if url is sub
            if (strExtension.Contains('/'))
            {
                foreach (var strTemp in strExtension.Split('/'))
                {
                    if (strTemp.Contains('.'))
                    {
                        return strTemp.Substring(strTemp.LastIndexOf(".") + 1, strTemp.Length - (strTemp.LastIndexOf(".") + 1));
                    }
                }
            }

            if (strExtension.Contains('.'))
            {
                return strExtension.Substring(strExtension.LastIndexOf(".") + 1, strExtension.Length - (strExtension.LastIndexOf(".") + 1));
            }
            else
            {
                return "none";
            }
        }

        public static Dictionary<string, string> URLProcess_QueryString(string strURL)
        {
            var dicReturn = new Dictionary<string, string>();

            if (!strURL.Contains('?')) { return dicReturn; }

            try
            {
                var strSplit = strURL.Split('?');

                if (strSplit.Length > 1)
                {
                    dicReturn = strSplit[1].Split('&').ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);
                }
            }
            catch(Exception)
            {
                return dicReturn;
            }

            return dicReturn;
        }
    }
}
