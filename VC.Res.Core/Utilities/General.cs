using System.Data;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace VC.Res.Core.Utilities
{
    public static class General
    {
        private static readonly HttpClient s_httpClient = new();

        public static async Task<string> ContentFromURLAsync(string strURL)
        {
            var strReturn = "";

            try
            {
                using var response = await s_httpClient.GetAsync(strURL);

                using var content = response.Content;

                strReturn = await content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(Email).ToString(), "ContentFromURLAsync(string)", ex,
                    "strURL: " + strURL.ToString());
                return strReturn;
            }

            return strReturn;
        }

        public static bool Validate_URL(object strURL)
        {
            var bReturn = false;

            try
            {
                if (strURL == null)
                {
                    return false;
                }

                if (Uri.IsWellFormedUriString(strURL.ToString(), UriKind.RelativeOrAbsolute))
                {
                    bReturn = true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return bReturn;
        }

        public static bool Validate_EmailAddress(object str)
        {
            try
            {
                var strToCheck = str?.ToString()?.ToLower().Trim();

                if (string.IsNullOrWhiteSpace(strToCheck)) { return false; }

                if (Regex.IsMatch(strToCheck, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*"))
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool Validate_PasswordComplexity(object str)
        {
            try
            {
                var strToCheck = str?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(strToCheck)) { return false; }

                if (Regex.IsMatch(strToCheck, "^(?=.*[A-Z])(?=.*[0-9])(?=.*[a-z]).*$"))
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ConvertToCommaString(List<string> lstStrings)
        {
            if (lstStrings.Count > 0)
            {
                return string.Join(",", lstStrings);
            }

            return "";
        }

        public static string ConvertToCommaString(List<int> lstInts)
        {
            if (lstInts.Count > 0)
            {
                return string.Join(",", lstInts);
            }

            return "";
        }

        public static string ConvertToCommaString<T>(List<T> lstEnums)
        {
            var lstInts = ConvertToListInt(lstEnums);

            return ConvertToCommaString(lstInts);
        }

        public static string[] ConvertToArrayString(List<int> lstInts)
        {
            var lstReturn = new List<string>();

            foreach (var iValue in lstInts)
            {
                lstReturn.Add(iValue.ToString());
            }
            return lstReturn.ToArray();
        }

        public static List<string> ConvertToListString(string strCommaList)
        {
            var lstReturn = new List<string>();

            if (string.IsNullOrWhiteSpace(strCommaList)) { return lstReturn; }

            lstReturn = strCommaList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return lstReturn;
        }

        public static List<int> ConvertToListInt(string strCommaList)
        {
            var lstReturn = new List<int>();

            if (string.IsNullOrWhiteSpace(strCommaList)) { return lstReturn; }

            var strArr = strCommaList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < strArr.Length; i++)
            {
                if (int.TryParse(strArr[i], out var iTempId))
                {
                    lstReturn.Add(iTempId);
                }
            }

            return lstReturn;
        }

        public static List<int> ConvertToListInt(string[] arrStrings)
        {
            if (arrStrings == null) { return new List<int>(); }

            if (arrStrings.Length < 1) { return new List<int>(); }

            var lstReturn = new List<int>();

            foreach (var vString in arrStrings)
            {
                if (int.TryParse(vString, out var iValue))
                {
                    lstReturn.Add(iValue);
                }
            }

            return lstReturn;
        }

        public static List<int> ConvertToListInt<T>(List<T> lstEnums)
        {
            if (lstEnums.Count < 1) 
            { 
                return new List<int>(); 
            }

            var lstReturn = new List<int>();

            var enumType = typeof(T);

            foreach (var vEnum in lstEnums)
            {
                if (vEnum == null) { continue; }

                lstReturn.Add((int)Enum.ToObject(enumType, vEnum));
            }

            return lstReturn;
        }

        public static List<T> ConvertToListEnums<T>(string strCommaList)
        {
            var enumType = typeof(T);

            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T must be of type System.Enum");
            }

            return ConvertToListEnums<T>(ConvertToListInt(strCommaList));
        }

        public static List<T> ConvertToListEnums<T>(List<int> lstInts)
        {
            var enumType = typeof(T);

            if (enumType.BaseType != typeof(Enum))
            {
                throw new ArgumentException("T must be of type System.Enum");
            }

            var lstReturn = new List<T>();

            foreach (var vInt in lstInts)
            {
                if (Enum.IsDefined(enumType, vInt))
                {
                    lstReturn.Add((T)Enum.Parse(enumType, vInt.ToString(), true));
                }
            }

            return lstReturn;
        }

        public static string MakeFriendlyTelNo(object objString)
        {
            var strToUse = objString?.ToString()?.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(strToUse)) { return ""; }

            var builder = new StringBuilder();

            foreach (var c in strToUse)
            {
                if (char.IsDigit(c) || c.Equals('+'))
                {
                    _ = builder.Append(c);
                }
            }

            return builder.ToString();
        }

        public static string MakeFriendlyURL(object objString)
        {
            var strToUse = objString?.ToString()?.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(strToUse)) { return ""; }

            var builder = new StringBuilder();

            foreach (var c in strToUse.Replace(" - ", "-").Replace(" & ", " and ").Replace(" ", "-").Replace("/", "-").Replace("\\", "-").Replace("_", "-").Replace("é", "e"))
            {
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c.Equals('-'))
                {
                    _ = builder.Append(c);
                }
            }

            return builder.ToString();
        }

        public static string MakeFriendlyFileName(object objString)
        {
            var strToUse = objString?.ToString()?.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(strToUse)) { return ""; }

            var builder = new StringBuilder();

            foreach (var c in strToUse.Replace(" - ", "-").Replace(" ", "-").Replace("_", "-"))
            {
                if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c.Equals('-') || c.Equals('.'))
                {
                    _ = builder.Append(c);
                }
            }

            return builder.ToString();
        }

        public static string MakeFriendlyTag(object objString, bool bAllowApostrophe = true)
        {
            var strToUse = objString?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(strToUse)) { return ""; }

            var builder = new StringBuilder();

            foreach (var c in strToUse)
            {
                if (bAllowApostrophe)
                {
                    if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c.Equals(' ') || c.Equals('\'') || c.Equals('-') || c.Equals(',') || c.Equals('.') || c.Equals('|') || c.Equals('&') || c.Equals('!') || c.Equals(';') || c.Equals('?') || c.Equals('\\') || c.Equals('/'))
                    {
                        _ = builder.Append(c);
                    }
                }
                else
                {
                    if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c.Equals(' ') || c.Equals('-') || c.Equals(',') || c.Equals('.') || c.Equals('|') || c.Equals('&') || c.Equals('!') || c.Equals(';') || c.Equals('?') || c.Equals('\\') || c.Equals('/'))
                    {
                        _ = builder.Append(c);
                    }
                }
            }

            return builder.ToString();
        }

        public static string ParseEvalString(object str)
        {
            var strToUse = str?.ToString();

            if (string.IsNullOrWhiteSpace(strToUse)) { return ""; }

            return strToUse;
        }

        public static DateTime ParseEvalDatetime(object dt)
        {
            if (dt != null)
            {
                if (DateTime.TryParse(dt.ToString(), out var dtTemp))
                {
                    return dtTemp;
                }
            }

            return new DateTime();
        }

        public static string ParseEvalDatetime(object dt, bool bIncHours)
        {
            if (dt != null)
            {
                if (DateTime.TryParse(dt.ToString(), out var dtTemp))
                {
                    if (bIncHours)
                    {
                        return dtTemp.ToString("dd/MM/yyyy HH:mm");
                    }
                    else
                    {
                        return dtTemp.ToString("dd/MM/yyyy");
                    }
                }
            }

            return "";
        }

        public static string ParseEvalBool(object objBool)
        {
            if (objBool != null)
            {
                if (!bool.TryParse(objBool.ToString(), out var bTemp))
                {
                    bTemp = true;
                }

                if (bTemp)
                {
                    return "Yes";
                }
                else
                {
                    return "No";
                }
            }

            return "";
        }

        public static string ParseEvalDecimal(object objDecimal)
        {
            if (objDecimal != null)
            {
                if (decimal.TryParse(objDecimal.ToString(), out var dTmp))
                {
                    return dTmp.ToString("0.00");
                }
            }

            return "";
        }

        public static DataColumn CreateColumn(string strName, string strCaption, Type typColumnType, string strDefault = "")
        {
            var dc = new DataColumn(strName, typColumnType)
            {
                Caption = strCaption,
                AllowDBNull = true
            };

            if (typColumnType == typeof(string))
            {
                if (!string.IsNullOrWhiteSpace(strDefault))
                {
                    dc.DefaultValue = strDefault;
                }
            }

            return dc;
        }



        /// <summary>
        /// Trims a given object (that can parse to a string) to a given length
        /// </summary>
        /// <param name="objToTrim">Object to trim</param>
        /// <param name="iNoChars">The number of characters to limit to</param>
        /// <param name="bClosestFullWord">if the trim should go to the closest full word as to prevent half a word</param>
        /// <returns>Trimmed string</returns>
        public static string LimitTextToLength(object objToTrim, int iNoChars, bool bClosestFullWord, string ellipsis = "")
        {
            if (objToTrim == null)
            {
                return "";
            }

            var strToTrim = objToTrim.ToString();

            if (string.IsNullOrWhiteSpace(strToTrim)) { return ""; }

            var iNoCharToGet = iNoChars;

            if (bClosestFullWord)
            {
                iNoCharToGet = iNoChars + 5;
            }

            if (strToTrim.Length <= iNoCharToGet)
            {
                return strToTrim;
            }

            var strReturn = strToTrim.Substring(0, iNoCharToGet);

            if (bClosestFullWord && strReturn.Contains(" "))
            {
                strReturn = strReturn.Substring(0, strReturn.LastIndexOf(" "));
            }

            strReturn = strReturn.Trim() + ellipsis;

            return strReturn;
        }

        public static string SplitTextToLines(object objToSplit, int iNoCharsPerLine, string strNewLineMarker)
        {
            var strToUse = objToSplit?.ToString();

            if (string.IsNullOrWhiteSpace(strToUse)) { return ""; }

            var strReturn = "";

            try
            {
                var strCurrentLine = "";

                var iCount = 0;

                foreach (var c in strToUse)
                {
                    if (c == ' ')
                    {
                        if (iCount >= iNoCharsPerLine)
                        {
                            if (string.IsNullOrWhiteSpace(strReturn))
                            {
                                strReturn = strCurrentLine;
                            }
                            else
                            {
                                strReturn = strReturn + strNewLineMarker + strCurrentLine;
                            }

                            iCount = 0;
                            strCurrentLine = "";
                        }
                        else
                        {
                            strCurrentLine += c;
                            iCount++;
                        }
                    }
                    else
                    {
                        strCurrentLine += c;
                        iCount++;
                    }
                }

                if (!string.IsNullOrWhiteSpace(strCurrentLine))
                {
                    if (string.IsNullOrWhiteSpace(strReturn))
                    {
                        strReturn = strCurrentLine;
                    }
                    else
                    {
                        strReturn = strReturn + strNewLineMarker + strCurrentLine;
                    }
                }
            }
            catch (Exception)
            {
                return strToUse;
            }

            return strReturn;
        }

        public static string StripHTML(string input)
        {
            var str = input;

            if (!string.IsNullOrWhiteSpace(str))
            {
                str = Regex.Replace(str, "<.*?>", "");
            }

            return str;
        }

        public static DataTable ToTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        public static string SplitCamelCase(string strCamelCase)
        {
            return Regex.Replace(Regex.Replace(strCamelCase, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"),@"(\p{Ll})(\P{Ll})","$1 $2");
        }
    }
}
