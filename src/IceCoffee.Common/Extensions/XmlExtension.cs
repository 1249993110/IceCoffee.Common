using System.Xml;
using System.Xml.Linq;

namespace IceCoffee.Common.Extensions
{
    public static class XmlExtension
    {
        /// <summary>
        /// Converts an <see cref="XElement"/> to an <see cref="XmlElement"/>.
        /// </summary>
        /// <param name="xElement">The element.</param>
        /// <returns>An <see cref="XmlElement"/> instance.</returns>
        public static XmlElement? ToXmlElement(this XElement xElement)
        {
            using var reader = xElement.CreateReader();
            var xmlDocument = new XmlDocument();
            return xmlDocument.ReadNode(reader) as XmlElement;
        }

        /// <summary>
        /// Converts an <see cref="XDocument"/> to an <see cref="XmlDocument"/>.
        /// </summary>
        /// <param name="xDocument">The element.</param>
        /// <returns>An <see cref="XmlDocument"/> instance.</returns>
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            using var xmlReader = xDocument.CreateReader();
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlReader);
            return xmlDocument;
        }

        /// <summary>
        /// Converts an <see cref="XmlElement"/> to an <see cref="XElement"/>.
        /// </summary>
        /// <param name="xmlElement">The element.</param>
        /// <returns>An <see cref="XElement"/> instance.</returns>
        public static XElement ToXElement(this XmlElement xmlElement)
        {
            var xPathNavigator = xmlElement.CreateNavigator();
            if (xPathNavigator != null)
            {
                return XElement.Load(xPathNavigator.ReadSubtree());
            }

            return XElement.Parse(xmlElement.OuterXml);

            //var doc = new XmlDocument();
            //doc.AppendChild(doc.ImportNode(xmlElement, true));
            //return XElement.Parse(doc.InnerXml);
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            var xPathNavigator = xmlDocument.CreateNavigator();
            if (xPathNavigator != null)
            {
                return XDocument.Load(xPathNavigator.ReadSubtree());
            }

            using var nodeReader = new XmlNodeReader(xmlDocument);
            nodeReader.MoveToContent();
            return XDocument.Load(nodeReader);
        }
    }
}