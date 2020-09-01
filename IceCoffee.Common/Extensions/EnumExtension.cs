using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceCoffee.Common.Extensions
{
    public static class EnumExtension
    {
        /// <summary>
        /// 获取成员值
        /// </summary>
        /// <param name="instance">枚举实例</param>
        public static int GetValue(this Enum instance)
        {
            return GetValue(instance.GetType(), instance);
        }

        /// <summary>
        /// 获取成员值
        /// </summary>
        /// <param name="type">枚举类型</param>
        /// <param name="member">成员名、值、实例均可</param>
        private static int GetValue(Type type, object member)
        {
            string value = member.ToString();
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException("member");
            return (int)Enum.Parse(type, member.ToString(), true);
        }
    }
}
