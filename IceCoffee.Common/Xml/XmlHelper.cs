using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IceCoffee.Common.Xml
{
    public static class XmlHelper
    {
        /// <summary>
        /// 保存指定对象的配置信息
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="contextDoc"></param>
        /// <param name="baseNode"></param>
        public static void SaveConfig(object obj, XmlDocument contextDoc, XmlNode baseNode)
        {
            if (obj == null || baseNode == null)
            {
                return;
            }

            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                ConfigNodeAttribute configNodeAttribute = property.GetCustomAttribute<ConfigNodeAttribute>(false);

                if (configNodeAttribute != null)
                {
                    switch (configNodeAttribute.XmlNodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                // 将标记了ConfigNodeType.Element特性的属性名作为父节点
                                XmlNode currentNode = baseNode.GetSingleChildNode(contextDoc, property.PropertyType.Name);

                                SaveConfig(property.GetValue(obj), contextDoc, currentNode);
                            }
                            break;
                        case XmlNodeType.Attribute:
                            {
                                baseNode.SaveAttribute(contextDoc, property.Name, property.GetValue(obj)?.ToString());
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
        public static void LoadConfig(object obj, XmlNode baseNode)
        {
            if(obj == null || baseNode == null)
            {
                return;
            }

            foreach (PropertyInfo property in obj.GetType().GetProperties())
            {
                ConfigNodeAttribute configNodeAttribute = property.GetCustomAttribute<ConfigNodeAttribute>(false);

                if (configNodeAttribute != null)
                {
                    switch (configNodeAttribute.XmlNodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                object propertyObj = property.GetValue(obj);

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
