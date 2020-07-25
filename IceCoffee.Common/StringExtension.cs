using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IceCoffee.Common
{
    public static class StringExtension
    {
        /// <summary>
        /// 从startIndex位置开始搜索，取出中间子文本，outEnd返回后面文本在原字符串中的位置，startIndex七日杀获取聊天信息为26最佳
        /// </summary>
        /// <param name="src"></param>
        /// <param name="front"></param>
        /// <param name="rear"></param>
        /// <param name="outEnd"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string GetMidStr(this string src, string front, string rear, out int outEnd, int startIndex = 0)
        {
            outEnd = -1;
            if (startIndex > src.Length)// 越界
            {
                return string.Empty;
            }

            int start = src.IndexOf(front, startIndex);

            if (start == -1 || start + front.Length > src.Length)// 没找到或尾部越界
            {
                return string.Empty;
            }

            outEnd = src.IndexOf(rear, start + front.Length);            

            if (outEnd == -1)
            {
                return string.Empty;
            }

            return src.Substring(start + front.Length, outEnd - front.Length - start);
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
            return src.GetMidStr(front, rear, out int outEnd, startIndex);
        }

        /// <summary>
        /// 将string转换为十进制整数，如果格式错误将转换失败并返回0
        /// </summary>
        /// <param name="src"></param>
        /// <param name="front"></param>
        /// <param name="rear"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            int.TryParse(str, out int result);
            return result;
        }

        /// <summary>
        /// 将string转换为十进制双精度数，如果格式错误将转换失败并返回0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static double ToDouble(this string str)
        {
            double.TryParse(str, out double result);
            return result;
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
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
        }

        /// <summary>
        /// 从Base64编码的字符串解析出原字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FormBase64(string str)
        {            
            return Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }
    }
}
