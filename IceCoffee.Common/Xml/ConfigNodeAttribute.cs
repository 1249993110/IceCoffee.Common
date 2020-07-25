using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IceCoffee.Common.Xml
{
    /// <summary>
    /// 标记需要读写的配置属性节点
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigNodeAttribute : Attribute
    {
        public XmlNodeType XmlNodeType { get; set; }

        public ConfigNodeAttribute(XmlNodeType xmlNodeType)
        {
            this.XmlNodeType = xmlNodeType;
        }
    }
}
