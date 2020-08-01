using IceCoffee.Common.Extensions;
using System.Net.Http;
using System.Threading.Tasks;

namespace IceCoffee.Common
{
    public static class NetHelper
    {
        /// <summary>
        /// 将 GET 请求发送到指定 URI 并在异步操作中以字符串的形式返回响应正文。
        /// </summary>
        public static async Task<string> GetStringAsync(string requestUri)
        {
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetStringAsync(requestUri);
            }
        }

        /// <summary>
        /// 将 GET 请求发送到指定 URI 并在同步操作中以字符串的形式返回响应正文。
        /// </summary>
        public static string GetString(string requestUri)
        {
            using (var httpClient = new HttpClient())
            {
                return httpClient.GetStringAsync(requestUri).WaitAndGetResult();
            }
        }
    }
}