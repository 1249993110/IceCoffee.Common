using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NET45
using Newtonsoft.Json;
#else
using System.Text.Json;
#endif

namespace IceCoffee.Common.Extensions
{
    /// <summary>
    /// Object扩展
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 将对象转换为Json格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
#if NET45
            return JsonConvert.SerializeObject(obj);
#else
            return JsonSerializer.Serialize(obj);
#endif            
        }
    }
}
