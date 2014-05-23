using System;
using System.Linq;
using System.Xml;
using UtilityExtensions;
using System.Text;
using System.IO;

namespace CmsData.API
{
    public class APIWriter
    {
        private XmlWriter w;
        private StringBuilder sb;
        public bool NoDefaults { get; set; }

        public APIWriter()
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = new System.Text.UTF8Encoding(false);
            sb = new StringBuilder();
            w = XmlWriter.Create(sb,settings);
        }
        public APIWriter(XmlWriter writer)
        {
			w = writer;
        }
        public APIWriter Start(string element)
        {
            w.WriteStartElement(element);
            return this;
        }
        public APIWriter End()
        {
            w.WriteEndElement();
            return this;
        }
        public APIWriter Attr(string attr, object i)
        {
            var s = tostr(i);
            if (s.HasValue() || NoDefaults)
                w.WriteAttributeString(attr, s);
            return this;
        }
        private string tostr(object i)
        {
            string s;
            if (i is DateTime)
                s = ((DateTime)i).FormatDateTm();
            else if (i == null)
                s = string.Empty;
            else
                s = i.ToString();
            return s;
        }
        public APIWriter Add(string element, object i)
        {
            var s = tostr(i);
            if (s.HasValue() || NoDefaults)
                w.WriteElementString(element, s);
            return this;
        }
        public APIWriter AddText(string text)
        {
            w.WriteString(text);
            return this;
        }
        public override string ToString()
        {
            w.Flush();
            return sb.ToString();
        }
        ~APIWriter()
        {
            w.Close();
        }
    }
}
