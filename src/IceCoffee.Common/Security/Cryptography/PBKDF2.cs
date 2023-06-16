using System.Security.Cryptography;

namespace IceCoffee.Common.Security.Cryptography
{
    /// <summary>
    /// PBKDF2 不可逆加密
    /// <para>通过一个伪随机函数（例如HMAC函数）, 把明文和一个盐值作为输入参数, 然后重复进行运算, 并最终产生密钥</para>
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

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using var pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, 1000);

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

            using var pbkdf2 = new Rfc2898DeriveBytes(plaintext, salt, 1000);

            return hashValue == Convert.ToBase64String(pbkdf2.GetBytes(20)); // Size of PBKDF2-HMAC-SHA-1 Hash
        }
    }
}