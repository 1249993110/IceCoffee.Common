using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.Security
{
    public static class Cryptography
    {
        /// <summary>
        /// 获取指定文件的MD5
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string path)
        {
            try
            {
                FileStream file = new FileStream(path, FileMode.Open);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; ++i)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch
            {
                //throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// 使用PBKDF2加密密码
        /// </summary>
        /// <param name="clearText">明文</param>
        /// <param name="hashValue">哈希值</param>
        /// <param name="saltBase64">盐</param>
        public static void HashPassword(string clearText, out string hashValue, out string saltBase64)
        {
            byte[] salt = new byte[24];
            RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();
            cryptoProvider.GetBytes(salt);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(clearText, salt, 1000);

            hashValue = Convert.ToBase64String(pbkdf2.GetBytes(20));// Size of PBKDF2-HMAC-SHA-1 Hash

            saltBase64 = Convert.ToBase64String(salt);
        }

        /// <summary>
        /// 使用PBKDF2验证密码
        /// </summary>
        /// <param name="clearText">明文</param>
        /// <param name="hashValue">哈希值</param>
        /// <param name="saltBase64">盐</param>
        /// <returns></returns>
        public static bool VerifyPassword(string clearText, string hashValue, string saltBase64)
        {
            byte[] salt = Convert.FromBase64String(saltBase64);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(clearText, salt, 1000);

            return hashValue == Convert.ToBase64String(pbkdf2.GetBytes(20)); // Size of PBKDF2-HMAC-SHA-1 Hash 
        }
    }
}
