using System.Reflection;
using System.Xml;

namespace IceCoffee.Common.Xml
{
    public static class XmlNodeExtension
    {
        /// <summary>
        /// 得到单个子节点, 如果节点不存在则自动创建Element元素
        /// </summary>
        /// <param name="thisNode"></param>
        /// <param name="contextDoc"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static XmlNode? GetSingleChildNode(this XmlNode thisNode, XmlDocument contextDoc, string childName)
        {
            return thisNode.SelectSingleNode(childName) ?? thisNode.AppendChild(contextDoc.CreateElement(childName));
        }

        /// <summary>
        /// 在根节点baseNode下查找具有name Attribute的子节点, 加载value
        /// </summary>
        /// <param name="baseNode"></param>
        /// <param name="obj"></param>
        /// <param name="property"></param>
        public static void LoadAttribute(this XmlNode baseNode, object obj, PropertyInfo property)
        {
            var currentNode = baseNode.SelectSingleNode(string.Format("property[@name='{0}']", property.Name)) as XmlElement;

            if (currentNode == null)
            {
                return;
            }

            string value = currentNode.GetAttribute("value");

            Type metaType = property.PropertyType;

            if (metaType == typeof(string))
            {
                property.SetValue(obj, value);
            }
            else if (string.IsNullOrEmpty(value) == false)
            {
                if (metaType.IsGenericType && metaType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    metaType = property.PropertyType.GetGenericArguments()[0];
                }

                property.SetValue(obj, Convert.ChangeType(value, metaType));
            }
        }

        /// <summary>
        /// 在根节点baseNode下查找具有name Attribute的子节点, 保存value, 自动创建不存在的子节点
        /// </summary>
        /// <param name="baseNode"></param>
        /// <param name="contextDoc"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SaveAttribute(this XmlNode baseNode, XmlDocument contextDoc, string name, string value)
        {
            var currentNode = baseNode.SelectSingleNode(string.Format("property[@name='{0}']", name)) as XmlElement;
            if (currentNode == null)
            {
                currentNode = contextDoc.CreateElement("property");
                currentNode.SetAttribute("name", name);
                baseNode.AppendChild(currentNode);
            }
            currentNode.SetAttribute("value", value);
        }
    }
}