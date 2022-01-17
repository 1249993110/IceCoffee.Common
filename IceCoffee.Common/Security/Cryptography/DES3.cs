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
    /// DES3
    /// </summary>
    public static class DES3
    {
        /// <summary>
        /// 加密 返回 Base64 加密值
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
                using var des = TripleDES.Create();
                des.Key = encoding.GetBytes(key);
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                des.IV = encoding.GetBytes(iv);

                ICryptoTransform desEncrypt = des.CreateEncryptor();

                byte[] buffer = encoding.GetBytes(input);
                return Convert.ToBase64String(desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
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
                using (var des = TripleDES.Create())
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

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        public static byte[] Encrypt(byte[] input, byte[] key, byte[] iv)
        {
            try
            {
                using (var des = TripleDES.Create())
                {
                    des.Key = key;
                    des.Mode = CipherMode.CBC;
                    des.Padding = PaddingMode.PKCS7;
                    des.IV = iv;

                    ICryptoTransform desEncrypt = des.CreateEncryptor();

                    byte[] buffer = input;
                    return desEncrypt.TransformFinalBlock(buffer, 0, buffer.Length);
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
        /// <returns></returns>
        public static byte[]? Decrypt(byte[] input, byte[] key, byte[] iv)
        {
            try
            {
                using (var des = TripleDES.Create())
                {
                    des.Key = key;
                    des.Mode = CipherMode.CBC;
                    des.Padding = PaddingMode.PKCS7;
                    des.IV = iv;

                    ICryptoTransform desDecrypt = des.CreateDecryptor();

                    byte[] buffer = input;

                    try
                    {
                        return desDecrypt.TransformFinalBlock(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DES3 解密异常", ex);
            }
        }
    }
}
