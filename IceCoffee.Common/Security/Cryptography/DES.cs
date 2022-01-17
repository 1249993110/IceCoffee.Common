using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.Security.Cryptography
{
    /// <summary>
    /// DES
    /// </summary>
    public static class DES
    {
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="input">加密数据</param>
        /// <param name="key">8位字符的密钥字符串</param>
        /// <param name="iv">8位字符的初始化向量字符串</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DESEncrypt(string input, string key, string iv, Encoding encoding)
        {
            byte[] byKey = encoding.GetBytes(key);
            byte[] byIV = encoding.GetBytes(iv);

            using var cryptoProvider = System.Security.Cryptography.DES.Create();
            using var ms = new MemoryStream();
            using var cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            using var sw = new StreamWriter(cst);
            sw.Write(input);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="input">解密数据</param>
        /// <param name="key">8位字符的密钥字符串(需要和加密时相同)</param>
        /// <param name="iv">8位字符的初始化向量字符串(需要和加密时相同)</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DESDecrypt(string input, string key, string iv, Encoding encoding)
        {
            byte[] byKey = encoding.GetBytes(key);
            byte[] byIV = encoding.GetBytes(iv);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(input);
            }
            catch
            {
                return string.Empty;
            }

            using var cryptoProvider = System.Security.Cryptography.DES.Create();
            using var ms = new MemoryStream(byEnc);
            using var cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            using var sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
    }
}
