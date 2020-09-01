using System;
using System.Text;
using System.Text.RegularExpressions;

namespace IceCoffee.Common.Extensions
{
    /// <summary>
    /// string扩展
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 从startIndex位置开始搜索，取出中间子文本，outEnd返回后面文本在原字符串中的位置
        /// </summary>
        /// <param name="src"></param>
        /// <param name="front"></param>
        /// <param name="rear"></param>
        /// <param name="outEnd"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string GetMidStr(this string src, string front, string rear, out int outEnd, int startIndex = 0)
        {
            int srcLength = src.Length;
            int frontLength = front.Length;

            outEnd = -1;
            if (startIndex > srcLength)// 越界
            {
                return string.Empty;
            }

            int start = src.IndexOf(front, startIndex);

            if (start == -1 || start + frontLength > srcLength)// 没找到或尾部越界
            {
                return string.Empty;
            }

            outEnd = src.IndexOf(rear, start + frontLength);

            if (outEnd == -1)
            {
                return string.Empty;
            }

            return src.Substring(start + frontLength, outEnd - frontLength - start);
        }

        /// <summary>
        /// 从startIndex位置开始搜索，取出中间子文本
        /// </summary>
        /// <param name="src"></param>
        /// <param name="front"></param>
        /// <param name="rear"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string GetMidStr(this string src, string front, string rear, int startIndex = 0)
        {
            return src.GetMidStr(front, rear, out _, startIndex);
        }

        /// <summary>
        /// 将string转换为十进制有符号短整数，如果格式错误将转换失败并返回默认值
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static short ToShort(this string str, short defaultValue = default)
        {
            if (short.TryParse(str, out short result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// 将string转换为32位十进制有符号整数，如果格式错误将转换失败并返回默认值
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static int ToInt(this string str, int defaultValue = default)
        {
            if(int.TryParse(str, out int result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// 将string转换为32位十进制无符号整数，如果格式错误将转换失败并返回默认值
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static uint ToUInt(this string str, uint defaultValue = default)
        {
            if(uint.TryParse(str, out uint result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// 将string转换为十进制有符号长整数，如果格式错误将转换失败并返回默认值
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static long ToLong(this string str, long defaultValue = default)
        {
            if(long.TryParse(str, out long result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// 将string转换为十进制双精度数，如果格式错误将转换失败并返回默认值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double ToDouble(this string str, double defaultValue = default)
        {
            if(double.TryParse(str, out double result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// 将字符串转为UTF-8编码的字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToUtf8(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 将字节数组按UTF-8编码转为字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormUtf8(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string[] Split(this string str, string pattern)
        {
            return Regex.Split(str, pattern);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="regexOption"></param>
        /// <returns></returns>
        public static string[] Split(this string str, string pattern, RegexOptions regexOption)
        {
            return Regex.Split(str, pattern, regexOption);
        }

        /// <summary>
        /// 将字符串转为Base64编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToBase64(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 从Base64编码的字符串解析出原字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FormBase64(string str)
        {
            if(string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }


    }
}