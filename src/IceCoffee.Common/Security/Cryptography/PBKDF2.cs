﻿using System.Security.Cryptography;

namespace IceCoffee.Common.Security.Cryptography
{
    /// <summary>
    /// PBKDF2 不可逆加密
    /// <para>通过一个伪随机函数（例如HMAC函数）, 把明文和一个盐值作为输入参数, 然后重复进行运算, 并最终产生密钥</para>
    /// <para>Size of PBKDF2-HMAC Hash: SHA-1 为 20 字节，SHA-224 为 28 字节，SHA-256 为 32 字节，SHA-384 为 48 字节，SHA-512 为 64 字节</para>
    /// </summary>
    public static class PBKDF2
    {
        /// <summary>
        /// 使用PBKDF2加密密码
        /// <para>此实现使用 Rfc2898DeriveBytes 基于 SHA1 算法进行 1000 次迭代</para>
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="hashValue">哈希值</param>
        /// <param name="saltBase64">盐</param>
        public static void HashPassword(string plaintext, out string hashValue, out string saltBase64)
        {
            byte[] salt = new byte[24];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

#if NET8_0_OR_GREATER
            using var pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, 1000, HashAlgorithmName.SHA1);
#else
            using var pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, 1000);
#endif
            hashValue = Convert.ToBase64String(pbkdf2.GetBytes(20));// Size of PBKDF2-HMAC-SHA-1 Hash
            saltBase64 = Convert.ToBase64String(salt);
        }

        /// <summary>
        /// 使用PBKDF2验证密码
        /// <para>此实现使用 Rfc2898DeriveBytes 基于 SHA1 算法进行 1000 次迭代</para>
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="hashValue">哈希值</param>
        /// <param name="saltBase64">盐</param>
        /// <returns></returns>
        public static bool VerifyPassword(string plaintext, string hashValue, string saltBase64)
        {
            byte[] salt = Convert.FromBase64String(saltBase64);

#if NET8_0_OR_GREATER
            using var pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, 1000, HashAlgorithmName.SHA1);
#else
            using var pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, 1000);
#endif
            return hashValue == Convert.ToBase64String(pbkdf2.GetBytes(20)); // Size of PBKDF2-HMAC-SHA-1 Hash
        }

        private static int GetSize(HashAlgorithmName hashAlgorithm)
        {
            int size;
            if (hashAlgorithm == HashAlgorithmName.SHA1)
            {
                size = 20;
            }
            else if (hashAlgorithm == HashAlgorithmName.SHA256)
            {
                size = 32;
            }
            else if (hashAlgorithm == HashAlgorithmName.SHA384)
            {
                size = 48;
            }
            else if (hashAlgorithm == HashAlgorithmName.SHA512)
            {
                size = 64;
            }
            else
            {
                throw new ArgumentException("Onlyu support SHA1/SHA256/SHA384/SHA512.");
            }

            return size;
        }

#if NET8_0_OR_GREATER
        /// <summary>
        /// 使用PBKDF2加密密码
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="hashValue">哈希值</param>
        /// <param name="saltBase64">盐</param>
        /// <param name="iterations">迭代次数</param>
        /// <param name="hashAlgorithm">哈希算法</param>
        public static void HashPassword(string plaintext, out string hashValue, out string saltBase64, int iterations, HashAlgorithmName hashAlgorithm)
        {
            int size = GetSize(hashAlgorithm);
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, iterations, hashAlgorithm);
            hashValue = Convert.ToBase64String(pbkdf2.GetBytes(size));
            saltBase64 = Convert.ToBase64String(salt);
        }                                                              

        /// <summary>
        /// 使用PBKDF2验证密码
        /// </summary>
        /// <param name="plaintext">明文</param>
        /// <param name="hashValue">哈希值</param>
        /// <param name="saltBase64">盐</param>
        /// <param name="iterations">迭代次数</param>
        /// <param name="hashAlgorithm">哈希算法</param>
        /// <returns></returns>
        public static bool VerifyPassword(string plaintext, string hashValue, string saltBase64, int iterations, HashAlgorithmName hashAlgorithm)
        {
            byte[] salt = Convert.FromBase64String(saltBase64);

            using var pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, iterations, hashAlgorithm);
            return hashValue == Convert.ToBase64String(pbkdf2.GetBytes(GetSize(hashAlgorithm)));
        }
#endif
    }
}