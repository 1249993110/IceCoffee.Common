using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.Security.Cryptography
{
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
            using var md5 = System.Security.Cryptography.MD5.Create();
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
                using var file = new FileStream(path, FileMode.Open);
                using var md5 = System.Security.Cryptography.MD5.Create();
                byte[] retVal = md5.ComputeHash(file);

                StringBuilder sb = new StringBuilder(32);
                for (int i = 0; i < retVal.Length; ++i)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            }
            catch(Exception ex)
            {
                throw new Exception("Error in MD5.GetMD5HashFromFile", ex);
            }
        }
    }
}
