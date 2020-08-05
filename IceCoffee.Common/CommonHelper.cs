using System;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace IceCoffee.Common
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class CommonHelper
    {
        #region 获取配置项

        /// <summary>
        /// 获取 appSettings
        /// </summary>
        /// <param name="key">键名</param>
        public static string GetAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        /// 获取连接字符串
        /// </summary>
        /// <param name="key">键名</param>
        public static string GetConnectionString(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ConnectionString;
        }

        /// <summary>
        /// 获取数据提供程序名称
        /// </summary>
        /// <param name="key">键名</param>
        public static string GetProviderName(string key)
        {
            return ConfigurationManager.ConnectionStrings[key].ProviderName;
        }

        #endregion 获取配置项

        /// <summary>
        /// CRC16校验
        /// </summary>
        /// <param name="arg">需要校验的字符串</param>
        /// <returns>CRC16 校验码</returns>
        public static string CRC16_Checkout(string arg)
        {
            char[] puchMsg = arg.ToCharArray();
            uint i, j, crc_reg, check;
            crc_reg = 0xFFFF;
            for (i = 0; i < puchMsg.Length; i++)
            {
                crc_reg = (crc_reg >> 8) ^ puchMsg[i];
                for (j = 0; j < 8; j++)
                {
                    check = crc_reg & 0x0001;
                    crc_reg >>= 1;
                    if (check == 0x0001)
                    {
                        crc_reg ^= 0xA001;
                    }
                }
            }
            return crc_reg.ToString("X2");
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
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
        /// 通过PropertyInfo创建Expression表达式
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static Expression CreateExpressionByPropertyInfo<TModel, TProperty>(PropertyInfo propertyInfo)
        {
            var parameter = Expression.Parameter(propertyInfo.DeclaringType);
            var property = Expression.Property(parameter, propertyInfo);
            var conversion = Expression.Convert(property, typeof(object));
            var lambda = Expression.Lambda<Func<TModel, TProperty>>(conversion, parameter);
            return lambda;
        }

        /// <summary>
        /// 生成随机字符串
        /// </summary>
        /// <param name="length">目标字符串的长度</param>
        /// <param name="useNum">是否包含数字，1=包含，默认为包含</param>
        /// <param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
        /// <param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
        /// <param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
        /// <param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
        /// <returns>指定长度的随机字符串</returns>
        public static string GetRandomString(int length, bool useNum = true, bool useLow = false, bool useUpp = false, bool useSpe = false, string custom = null)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(r.Next(0, str.Length - 1), 1);
            }
            return s;
        }
    }
}