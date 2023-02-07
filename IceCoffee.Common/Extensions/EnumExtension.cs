using System.ComponentModel;
using System.Reflection;

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
            string? value = member.ToString();
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(member));
            }

            return (int)Enum.Parse(type, value, true);
        }

        /// <summary>
        /// 返回枚举项的描述信息。
        /// </summary>
        /// <param name="value">要获取描述信息的枚举项。</param>
        /// <returns>枚举想的描述信息。</returns>
        public static string? GetDescription(this Enum value)
        {
            var enumType = value.GetType();
            // 获取枚举常数名称。
            string name = Enum.GetName(enumType, value);
            // 获取枚举字段。
            FieldInfo? fieldInfo = enumType.GetField(name);
            if (fieldInfo != null)
            {
                // 获取描述的属性。
                if (Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) is DescriptionAttribute attr)
                {
                    return attr.Description;
                }
            }

            return null;
        }
    }
}