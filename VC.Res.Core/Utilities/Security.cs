using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace VC.Res.Core.Utilities
{
    public static class Security
    {
        /// <summary>
        /// Generates a random string of x characters and x numbers
        /// </summary>
        /// <returns>Random string</returns>
        public static string GenerateRandomCode(int iNumberOfLetters, int iNumberOfDigits)
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;

            for (var i = 0; i < iNumberOfLetters; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(d: (26 * random.NextDouble()) + 65)));
                _ = builder.Append(ch);
            }

            var strLower = "1";
            var strUpper = "9";

            for (var i = 1; i < iNumberOfDigits; i++)
            {
                strLower += "0";
                strUpper += "9";
            }

            return builder.ToString().ToUpper() + random.Next(Convert.ToInt32(strLower), Convert.ToInt32(strUpper)).ToString();
        }

        public static string GenerateRandomAlphanumeric(int iLength)
        {
            Thread.Sleep(20);
            var strAvailableChars = "abcdefghijklmnopqrstuvwxyz1234567890";
            var sbReturn = new StringBuilder();
            var rnd = new Random();

            while (iLength-- > 0)
            {
                _ = sbReturn.Append(strAvailableChars[(int)(rnd.NextDouble() * strAvailableChars.Length)]);
            }

            return sbReturn.ToString();
        }

        //public static string HashPlainSHA256(string strInput)
        //{
        //    var bytes = Encoding.UTF8.GetBytes(strInput);

        //    var hashstring = new SHA256Managed();

        //    var hash = hashstring.ComputeHash(bytes);

        //    var hashString = string.Empty;
        //    foreach (var x in hash)
        //    {
        //        hashString += string.Format("{0:x2}", x);
        //    }

        //    hashstring.Dispose();

        //    return hashString;
        //}

        //public static string HashPlainSHA512(string strInput)
        //{
        //    var bytes = Encoding.UTF8.GetBytes(strInput);

        //    var sha512 = new SHA512Managed();

        //    var hash = sha512.ComputeHash(bytes);

        //    var hashString = string.Empty;
        //    foreach (var x in hash)
        //    {
        //        hashString += string.Format("{0:x2}", x);
        //    }

        //    sha512.Dispose();

        //    return hashString;
        //}


        public static string EncryptString(string strToEncrypt, string strPassword = "", string strSalt = "")
        {
            if (string.IsNullOrWhiteSpace(strToEncrypt)) { return ""; }

            var strPasswordToUse = strPassword;
            if (string.IsNullOrWhiteSpace(strPasswordToUse))
            {
                strPasswordToUse = Settings.Variables.Encryption_DefaultPassword;
            }

            var strSaltToUse = strSalt;
            if (string.IsNullOrWhiteSpace(strSaltToUse))
            {
                strSaltToUse = Settings.Variables.Encryption_DefaultSalt;
            }

            // Get the bytes of the string
            var bytesToBeEncrypted = Encoding.UTF8.GetBytes(strToEncrypt);
            var passwordBytes = Encoding.UTF8.GetBytes(strPasswordToUse);
            var saltBytes = Encoding.UTF8.GetBytes(strSaltToUse);

            // Hash the password and key with SHA256
            saltBytes = SHA256.Create().ComputeHash(saltBytes);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[]? bytesEncrypted = null;

            using (var ms = new MemoryStream())
            {
                using var aES = Aes.Create("AesManaged");

                if (aES != null)
                {
                    aES.KeySize = 256;
                    aES.BlockSize = 128;

                    using var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    aES.Key = key.GetBytes(aES.KeySize / 8);
                    aES.IV = key.GetBytes(aES.BlockSize / 8);

                    aES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }

                    bytesEncrypted = ms.ToArray();
                }
            }

            if (bytesEncrypted != null)
            {
                return Convert.ToBase64String(bytesEncrypted);
            }
            else
            {
                return "";
            }
        }

        public static string DecryptString(string strToDecrypt, string strPassword = "", string strSalt = "")
        {
            if (string.IsNullOrWhiteSpace(strToDecrypt)) { return ""; }

            var strPasswordToUse = strPassword;
            if (string.IsNullOrWhiteSpace(strPasswordToUse))
            {
                strPasswordToUse = Settings.Variables.Encryption_DefaultPassword;
            }

            var strSaltToUse = strSalt;
            if (string.IsNullOrWhiteSpace(strSaltToUse))
            {
                strSaltToUse = Settings.Variables.Encryption_DefaultSalt;
            }

            // Get the bytes of the string
            var bytesToBeDecrypted = Convert.FromBase64String(strToDecrypt);
            var passwordBytes = Encoding.UTF8.GetBytes(strPasswordToUse);
            var saltBytes = Encoding.UTF8.GetBytes(strSaltToUse);

            // Hash the password and key with SHA256
            saltBytes = SHA256.Create().ComputeHash(saltBytes);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[]? bytesDecrypted = null;

            using (var ms = new MemoryStream())
            {
                using var aES = Aes.Create("AesManaged");

                if (aES != null)
                {
                    aES.KeySize = 256;
                    aES.BlockSize = 128;

                    using var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);

                    aES.Key = key.GetBytes(aES.KeySize / 8);
                    aES.IV = key.GetBytes(aES.BlockSize / 8);

                    aES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, aES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }

                    bytesDecrypted = ms.ToArray();
                }
            }

            if (bytesDecrypted != null)
            {
                return Encoding.UTF8.GetString(bytesDecrypted);
            }

            return "";
        }

        /// <summary>
        /// Generate a random hash byte array and converts to base64 encoded string
        /// </summary>
        /// <returns>Base64 string of hash</returns>
        public static string Argon2_CreateSalt()
        {
            var buffer = RandomNumberGenerator.GetBytes(16);
            //var buffer = new byte[16];
            //var rng = new RSACryptoServiceProvider();

            //rng.GetBytes(buffer);

            var strReturn = Convert.ToBase64String(buffer);

            //rng.Dispose();

            return strReturn;
        }

        public static string Argon2_CreateHash(string strPassword, string strSalt)
        {
            // get the salt from the string
            var salt = Convert.FromBase64String(strSalt);

            // setup argon hasher
            var argon2 = new Argon2i(Encoding.UTF8.GetBytes(strPassword))
            {
                Salt = salt,
                DegreeOfParallelism = 4, // dual core (double number of cores machine has, server only has 2 tho)
                Iterations = 16,
                MemorySize = 1024 * 8 // 8mb
            };

            // calculate the hash
            var hash = argon2.GetBytes(64);

            var strReturn = Convert.ToBase64String(hash);

            argon2.Dispose();

            return strReturn;
        }

        public static bool Argon2_VerifyHash(string strPassword, string strSalt, string strHash)
        {
            var newHash = Argon2_CreateHash(strPassword, strSalt);
            return strHash.Equals(newHash);
        }
    }
}
