using System.Reflection;
using System.Xml;

namespace IceCoffee.Common.Xml
{
    public static class XmlConfigHelper
    {
        /// <summary>
        /// 保存指定对象的配置信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="contextDoc"></param>
        /// <param name="baseNode"></param>
        public static void SaveConfig(object? obj, XmlDocument contextDoc, XmlNode? baseNode)
        {
            if (obj == null || baseNode == null)
            {
                return;
            }

            ConfigNodeAttribute? configNodeAttribute;
            object? value;

            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                configNodeAttribute = property.GetCustomAttribute<ConfigNodeAttribute>(false);

                if (configNodeAttribute != null)
                {
                    switch (configNodeAttribute.XmlNodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                // 将标记了ConfigNodeType.Element特性的属性名作为父节点
                                var currentNode = baseNode.GetSingleChildNode(contextDoc, property.PropertyType.Name);

                                SaveConfig(property.GetValue(obj), contextDoc, currentNode);
                            }
                            break;

                        case XmlNodeType.Attribute:
                            {
                                value = property.GetValue(obj);
                                if (value == null)
                                {
                                    baseNode.SaveAttribute(contextDoc, property.Name, string.Empty);
                                }
                                else
                                {
                                    var str = value.ToString();
                                    if (str == null)
                                    {
                                        baseNode.SaveAttribute(contextDoc, property.Name, string.Empty);
                                    }
                                    else
                                    {
                                        baseNode.SaveAttribute(contextDoc, property.Name, str);
                                    }
                                }
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 加载指定对象的配置信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="baseNode"></param>
        public static void LoadConfig(object? obj, XmlNode? baseNode)
        {
            if (obj == null || baseNode == null)
            {
                return;
            }

            ConfigNodeAttribute? configNodeAttribute;

            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                configNodeAttribute = property.GetCustomAttribute<ConfigNodeAttribute>(false);

                if (configNodeAttribute != null)
                {
                    switch (configNodeAttribute.XmlNodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                var propertyObj = property.GetValue(obj);

                                if (propertyObj != null)
                                {
                                    LoadConfig(propertyObj, baseNode.SelectSingleNode(property.PropertyType.Name));
                                }
                            }
                            break;

                        case XmlNodeType.Attribute:
                            {
                                baseNode.LoadAttribute(obj, property);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}