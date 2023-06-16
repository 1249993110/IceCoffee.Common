using System.Xml;

namespace IceCoffee.Common.Xml
{
    /// <summary>
    /// <para>标记了ConfigNodeType.Element特性的属性作为父节点</para>
    /// <para>标记了ConfigNodeType.Attribute特性的属性作为需要直接读写属性的节点</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ConfigNodeAttribute : Attribute
    {
        /// <summary>
        /// 节点类型
        /// </summary>
        public XmlNodeType XmlNodeType { get; set; }

        public ConfigNodeAttribute(XmlNodeType xmlNodeType)
        {
            this.XmlNodeType = xmlNodeType;
        }
    }
}