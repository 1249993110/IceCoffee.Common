using IceCoffee.Common.Extensions;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Net;
using System.IO;
#if NET45
using Newtonsoft.Json;
#else
using System.Text.Json;
#endif

namespace IceCoffee.Common
{
    /// <summary>
    /// 使用全局唯一 HttpClient 实例执行 Http 请求
    /// </summary>
    public class HttpHelper : IDisposable
    {
        public static class ContentType
        {
            public const string Json = "application/json";// StringContent

            public const string Xml = "application/xml";// StringContent

            public const string FormUrlencoded = "application/x-www-form-urlencoded";// FormUrlEncodedContent

            public const string FormData = "multipart/form-data";// MultipartFormDataContent

            public const string TextXml = "text/xml";// StringContent

            public const string Binary = "binary";// StreamContent
        }

        private HttpClient _httpClient;


        private static HttpHelper _defaultInstance = null;
        private static readonly object _singleton_Lock = new object();

        /// <summary>
        /// 获得默认实例，请求超时前等待的时间跨度默认为 20 秒
        /// </summary>
        public static HttpHelper Default
        {
            get
            {
                if (_defaultInstance == null) //双if + lock
                {
                    lock (_singleton_Lock)
                    {
                        if (_defaultInstance == null)
                        {
                            _defaultInstance = new HttpHelper();
                        }
                    }
                }
                return _defaultInstance;
            }
        }

        private HttpHelper()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(20);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ContentType.Json));
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.01));
        }

        public HttpHelper(HttpClient httpClient)
        {
            if(httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }
            _httpClient = httpClient;
        }

        #region Get
        /// <summary>
        /// 将 Get 请求发送到指定 Url 并在同步操作中以字节数组的形式返回响应正文。
        /// </summary>
        public byte[] GetByteArray(string requestUri)
        {
            return _httpClient.GetByteArrayAsync(requestUri).Result;
        }
        /// <summary>
        /// 将 Get 请求发送到指定 Url 并在异步操作中以字节数组的形式返回响应正文。
        /// </summary>
        public async Task<byte[]> GetByteArrayAsync(string requestUri)
        {
            return await _httpClient.GetByteArrayAsync(requestUri);
        }

        /// <summary>
        /// 将 Get 请求发送到指定 Url 并在同步操作中以流的形式返回响应正文。
        /// </summary>
        public Stream GetStream(string requestUri)
        {
            return _httpClient.GetStreamAsync(requestUri).Result;
        }
        /// <summary>
        /// 将 Get 请求发送到指定 Url 并在异步操作中以流的形式返回响应正文。
        /// </summary>
        public async Task<Stream> GetStreamAsync(string requestUri)
        {
            return await _httpClient.GetStreamAsync(requestUri);
        }

        /// <summary>
        /// 将 Get 请求发送到指定 Url 并在同步操作中以字符串的形式返回响应正文。
        /// </summary>
        public string GetString(string requestUri)
        {
            return _httpClient.GetStringAsync(requestUri).Result;
        }
        /// <summary>
        /// 将 Get 请求发送到指定 Url 并在异步操作中以字符串的形式返回响应正文。
        /// </summary>
        public async Task<string> GetStringAsync(string requestUri)
        {
            return await _httpClient.GetStringAsync(requestUri);
        }

        /// <summary>
        /// 将 Get 请求发送到指定 Url 并在同步操作中以字符串的形式返回响应正文。
        /// </summary>
        public string GetString(string requestUri)
        {
            return _httpClient.GetStringAsync(requestUri).Result;
        }
        /// <summary>
        /// 将 Get 请求发送到指定 Url 并在异步操作中以字符串的形式返回响应正文。
        /// </summary>
        public async Task<string> GetStringAsync(string requestUri)
        {
            return await _httpClient.GetStringAsync(requestUri);
        }

        /// <summary>
        /// 使用 Get 返回同步请求直接返回对象
        /// </summary>
        /// <typeparam name="T">请求对象类型</typeparam>
        /// <param name="url">请求链接</param>
        /// <returns>返回请求的对象</returns>
        public T GetObject<T>(string url)
        {
            string responseBody = GetString(url);
#if NET45
            return JsonConvert.DeserializeObject<T>(responseBody);
#else
            return JsonSerializer.Deserialize<T>(responseBody);
#endif
        }
        /// <summary>
        /// 使用 Get 返回异步请求直接返回对象
        /// </summary>
        /// <typeparam name="T">请求对象类型</typeparam>
        /// <param name="url">请求链接</param>
        /// <returns>返回请求的对象</returns>
        public async Task<T> GetObjectAsync<T>(string url)
        {
            string responseBody = await GetStringAsync(url);
#if NET45
            return JsonConvert.DeserializeObject<T>(responseBody);
#else
            return JsonSerializer.Deserialize<T>(responseBody);
#endif
        }

        /// <summary>
        /// 将object转换为查询参数附加到url后
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static string AttachQueryString(string url, object obj)
        {
            PropertyInfo[] propertis = obj.GetType().GetProperties();
            StringBuilder sb = new StringBuilder(url);

            sb.Append("?");
            foreach (var p in propertis)
            {
                var v = p.GetValue(obj, null);
                if (v == null)
                    continue;

                sb.Append(p.Name);
                sb.Append("=");
                sb.Append(v.ToString());
                sb.Append("&");
            }

            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        /// <summary>
        /// 使用 Get 返回同步请求直接返回对象
        /// </summary>
        /// <typeparam name="T">请求对象类型</typeparam>
        /// <param name="url">请求链接</param>
        /// <param name="param">使用反射自动转化为QueryString</param>
        /// <returns>返回请求的对象</returns>
        public T GetObject<T>(string url, object param)
        {
            return GetObjectAsync<T>(url, param).Result;
        }
        /// <summary>
        /// 使用 Get 返回异步请求直接返回对象
        /// </summary>
        /// <typeparam name="T">请求对象类型</typeparam>
        /// <param name="url">请求链接</param>
        /// <param name="param">使用反射自动转化为QueryString</param>
        /// <returns>返回请求的对象</returns>
        public async Task<T> GetObjectAsync<T>(string url, object param)
        {
            string responseBody = await GetStringAsync(AttachQueryString(url, param));
#if NET45
            return JsonConvert.DeserializeObject<T>(responseBody);
#else
            return JsonSerializer.Deserialize<T>(responseBody);
#endif
        }

        #endregion

        #region Post

        /// <summary>
        /// 使用 Post 方法同步请求
        /// </summary>
        /// <param name="url">目标链接</param>
        /// <param name="param">发送的对象,自动转换为Json对象</param>
        /// <returns>返回的字符串</returns>
        public string PostString(string url, object param)
        {
            return PostStringAsync(url, param).Result;
        }
        /// <summary>
        /// 使用 Post 方法异步请求
        /// </summary>
        /// <param name="url">目标链接</param>
        /// <param name="param">发送的对象,自动转换为Json对象</param>
        /// <returns>返回的字符串</returns>
        public async Task<string> PostStringAsync(string url, object param)
        {
            string json = (param as string) ?? param.ToJson();

            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue(ContentType.Json);
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 使用 Post 返回同步请求直接返回对象
        /// </summary>
        /// <typeparam name="T">请求对象类型</typeparam>
        /// <param name="url">请求链接</param>
        /// <param name="param">发送的对象,自动转换为Json对象</param>
        /// <returns>返回请求的对象</returns>
        public T PostObject<T>(string url, object param)
        {
            return PostObjectAsync<T>(url, param).Result;
        }
        /// <summary>
        /// 使用 Post 返回异步请求直接返回对象
        /// </summary>
        /// <typeparam name="T">请求对象类型</typeparam>
        /// <param name="url">请求链接</param>
        /// <param name="param">发送的对象,自动转换为Json对象</param>
        /// <returns>返回请求的对象</returns>
        public async Task<T> PostObjectAsync<T>(string url, object param)
        {
            string responseBody = await PostStringAsync(url, param);
#if NET45
            return JsonConvert.DeserializeObject<T>(responseBody);
#else
            return JsonSerializer.Deserialize<T>(responseBody);
#endif
        }

        /// <summary>
        /// 释放由 HttpMessageInvoker 使用的非托管资源和托管资源
        /// </summary>
        public void Dispose()
        {
            _httpClient.Dispose();
        }
        #endregion
    }
}