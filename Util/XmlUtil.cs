using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SSharing.Ubtrip.Common
{
    public static class XmlUtil
    {
        /// <summary>
        /// 得到节点内的文本值
        /// </summary>
        public static string SelectSingleNodeValue(XmlNode xmlNode, string xpath)
        {
            if (xmlNode == null)
                throw new ArgumentNullException("xmlNode");

            XmlNode node = xmlNode.SelectSingleNode(xpath);
            return node == null ? string.Empty : node.InnerXml.Trim();
        }
    
        public static XElement ElementIgnoreCase(this XContainer container, XName name)
        {
            foreach (XElement element in container.Elements())
            {
                if (element.Name.NamespaceName == name.NamespaceName &&  
                String.Equals(element.Name.LocalName, name.LocalName, StringComparison.OrdinalIgnoreCase))
                {
                    return element;
                }
            }
            return null;
        }

        public static IEnumerable<XElement> ElementsIgnoreCase(this XContainer container, XName name)
        {
            foreach (XElement element in container.Elements())
            {
                if (element.Name.NamespaceName == name.NamespaceName &&  
                String.Equals(element.Name.LocalName, name.LocalName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return element;
                }
            }
        }

        public static IEnumerable<XAttribute> AttributesIgnoreCase(this XElement element, XName name)
        {
            foreach (XAttribute attr in element.Attributes())
            {
                if (attr.Name.NamespaceName == name.NamespaceName &&  
                String.Equals(attr.Name.LocalName, name.LocalName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return attr;
                }
            }
        }
    } 
}
