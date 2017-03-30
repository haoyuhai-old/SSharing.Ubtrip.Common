using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SSharing.Ubtrip.Common
{
    public class ErrorEmailConfig
    {
        public ErrorEmailConfig()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.BaseDirectory + "\\" + "Config\\ErrorEmails.config");

            XmlNodeList nodes = xmlDoc.SelectNodes("/Email/ErrorEmails/item");

            foreach (XmlNode node in nodes)
            {
                _ErrorEmails.Add(node.InnerText);
            }

            XmlNode replaceNode = xmlDoc.SelectSingleNode("/Email/ReplaceToEmail");
            ReplaceToEmail = replaceNode.InnerText;
        }

        private List<string> _ErrorEmails = new List<string>();
        public List<string> ErrorEmails
        {
            get
            {
                return _ErrorEmails;
            }
            set
            {
                _ErrorEmails = value;
            }
        }

        private string _ReplaceToEmail = string.Empty;
        public string ReplaceToEmail
        {
            get
            {
                return _ReplaceToEmail;
            }
            set
            {
                _ReplaceToEmail = value;
            }
        }
    }
}
