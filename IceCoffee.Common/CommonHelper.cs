using System;
using System.Configuration;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
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

        //#define MB_OK                       0x00000000L
        //#define MB_OKCANCEL                 0x00000001L
        //#define MB_ABORTRETRYIGNORE         0x00000002L
        //#define MB_YESNOCANCEL              0x00000003L
        //#define MB_YESNO                    0x00000004L
        //#define MB_RETRYCANCEL              0x00000005L

        [DllImport("user32.dll", EntryPoint = "MessageBoxTimeoutW", SetLastError = true,
            CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Winapi)]
        private static extern IntPtr MessageBoxTimeoutW(IntPtr hWnd,
            [MarshalAs(UnmanagedType.LPWStr)] string content,
            [MarshalAs(UnmanagedType.LPWStr)] string title,
            [MarshalAs(UnmanagedType.U4)] uint utype = 0,
            ushort wLanguageId = 0, uint dwMilliseconds = 3000);

        public static IntPtr MessageBoxTimeout(string content, string title = "提示", uint dwMilliseconds = 3000)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                return MessageBoxTimeoutW(IntPtr.Zero, content, title, 0, 0, dwMilliseconds);
            }
            else
            {
                throw new Exception("定时关闭消息框仅支持Win32平台");
            }
        }

        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// <param name="path">文件路径</param>
        /// <returns>文件的编码类型</returns>
        public static Encoding GetType(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static Encoding GetType(FileStream fs)
        {
            // byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            // byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            // byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; // 带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1;　 //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                return false;
            }
            return true;
        }

    }
}