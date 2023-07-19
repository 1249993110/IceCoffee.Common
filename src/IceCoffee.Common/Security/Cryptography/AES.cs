using System.Security.Cryptography;
using System.Text;

namespace IceCoffee.Common.Security.Cryptography
{
    /// <summary>
    /// AES
    /// </summary>
    public static class AES
    {
        public static byte[] Encrypt(byte[] input, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(key, iv);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
#if NET6_0_OR_GREATER
                        csEncrypt.Write(input);
#else
                        csEncrypt.Write(input, 0, input.Length);
#endif
                    }
                    return msEncrypt.ToArray();
                }
            }
        }

        public static byte[] Decrypt(byte[] input, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(key, iv);
                byte[] decryptedBytes;
                using (MemoryStream msDecrypt = new MemoryStream(input))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (MemoryStream msPlain = new MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlain);
                            decryptedBytes = msPlain.ToArray();
                        }
                    }
                }
                return decryptedBytes;
            }
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="input">加密数据</param>
        /// <param name="key">16字节字符的密钥字符串</param>
        /// <param name="iv">16字节字符的初始化向量字符串</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Encrypt(string input, string key, string iv, Encoding encoding)
        {
            byte[] bKey = new byte[16];
            Array.Copy(encoding.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            byte[] bVector = new byte[16];
            Array.Copy(encoding.GetBytes(iv.PadRight(bVector.Length)), bVector, bVector.Length);

            using (Aes aesAlg = Aes.Create())
            {
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(bKey, bVector);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                        return Convert.ToBase64String(msEncrypt.GetBuffer(), 0, (int)msEncrypt.Length);
                    }
                }
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="input">解密数据</param>
        /// <param name="key">16字节字符的密钥字符串(需要和加密时相同)</param>
        /// <param name="iv">16字节字符的初始化向量字符串(需要和加密时相同)</param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string Decrypt(string input, string key, string iv, Encoding encoding)
        {
            byte[] bKey = new byte[16];
            Array.Copy(encoding.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            byte[] bVector = new byte[16];
            Array.Copy(encoding.GetBytes(iv.PadRight(bVector.Length)), bVector, bVector.Length);

            using (Aes aesAlg = Aes.Create())
            {
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(bKey, bVector);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(input)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}