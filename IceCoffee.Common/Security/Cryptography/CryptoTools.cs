using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.Security.Cryptography
{
    /// <summary>
    /// 加密解密工具
    /// </summary>
    public static class CryptoTools
    {
        /// <summary>
        /// Base64
        /// </summary>
        public static class Base64
        {
            /// <summary>
            /// Base64加密
            /// </summary>
            /// <param name="input">需要加密的字符串</param>
            /// <param name="encoding">字符编码</param>
            /// <returns></returns>
            public static string Encrypt(string input, Encoding encoding)
            {
                return Convert.ToBase64String(encoding.GetBytes(input));
            }

            /// <summary>
            /// Base64解密
            /// </summary>
            /// <param name="input">需要解密的字符串</param>
            /// <param name="encoding">字符编码</param>
            /// <returns></returns>
            public static string Decrypt(string input, Encoding encoding)
            {
                return encoding.GetString(Convert.FromBase64String(input));
            }
        }

        /// <summary>
        /// MD5
        /// </summary>
        public static class MD5
        {
            /// <summary>
            /// MD5加密
            /// </summary>
            /// <param name="input">需要加密的字符串</param>
            /// <param name="encoding">字符的编码</param>
            /// <returns></returns>
            public static string Encrypt(string input, Encoding encoding)
            {
                var md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(encoding.GetBytes(input));
                StringBuilder sb = new StringBuilder(32);
                for (int i = 0; i < retVal.Length; ++i)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }

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
                    var md5 = new MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(file);
                    file.Close();

                    StringBuilder sb = new StringBuilder(32);
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
        }

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

                var cryptoProvider = new DESCryptoServiceProvider();
                int i = cryptoProvider.KeySize;
                MemoryStream ms = new MemoryStream();
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

                StreamWriter sw = new StreamWriter(cst);
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

                var cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream(byEnc);
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cst);
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// DES3
        /// </summary>
        public static class DES3
        {
            /// <summary>
            /// 加密
            /// </summary>
            /// <param name="input"></param>
            /// <param name="key"></param>
            /// <param name="iv"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string Encrypt(string input, string key, string iv, Encoding encoding)
            {
                try
                {
                    using (var des = new TripleDESCryptoServiceProvider())
                    {
                        des.Key = encoding.GetBytes(key);
                        des.Mode = CipherMode.CBC;
                        des.Padding = PaddingMode.PKCS7;
                        des.IV = encoding.GetBytes(iv);

                        ICryptoTransform desEncrypt = des.CreateEncryptor();

                        byte[] buffer = encoding.GetBytes(input);
                        return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DES3 加密异常", ex);
                }
            }

            /// <summary>
            /// 解密
            /// </summary>
            /// <param name="input"></param>
            /// <param name="key"></param>
            /// <param name="iv"></param>
            /// <param name="encoding"></param>
            /// <returns></returns>
            public static string Decrypt(string input, string key, string iv, Encoding encoding)
            {
                try
                {
                    using (var des = new TripleDESCryptoServiceProvider())
                    {
                        des.Key = encoding.GetBytes(key);
                        des.Mode = CipherMode.CBC;
                        des.Padding = PaddingMode.PKCS7;
                        des.IV = encoding.GetBytes(iv);

                        ICryptoTransform desDecrypt = des.CreateDecryptor();

                        byte[] buffer = Convert.FromBase64String(input);

                        try
                        {
                            return encoding.GetString(desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));
                        }
                        catch
                        {
                            return string.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DES3 解密异常", ex);
                }
            }
        }

        /// <summary>
        /// PBKDF2 不可逆加密
        /// <para>通过一个伪随机函数（例如HMAC函数），把明文和一个盐值作为输入参数，然后重复进行运算，并最终产生密钥</para>
        /// <para>此实现使用 Rfc2898DeriveBytes 进行 1000 次迭代</para>
        /// </summary>
        public static class PBKDF2
        {
            /// <summary>
            /// 使用PBKDF2加密密码
            /// </summary>
            /// <param name="plaintext">明文</param>
            /// <param name="hashValue">哈希值</param>
            /// <param name="saltBase64">盐</param>
            public static void HashPassword(string plaintext, out string hashValue, out string saltBase64)
            {
                byte[] salt = new byte[24];
                RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();
                cryptoProvider.GetBytes(salt);

                Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, 1000);

                hashValue = Convert.ToBase64String(pbkdf2.GetBytes(20));// Size of PBKDF2-HMAC-SHA-1 Hash

                saltBase64 = Convert.ToBase64String(salt);
            }

            /// <summary>
            /// 使用PBKDF2验证密码
            /// </summary>
            /// <param name="plaintext">明文</param>
            /// <param name="hashValue">哈希值</param>
            /// <param name="saltBase64">盐</param>
            /// <returns></returns>
            public static bool VerifyPassword(string plaintext, string hashValue, string saltBase64)
            {
                byte[] salt = Convert.FromBase64String(saltBase64);

                Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, 1000);

                return hashValue == Convert.ToBase64String(pbkdf2.GetBytes(20)); // Size of PBKDF2-HMAC-SHA-1 Hash 
            }
        }
    }
}
