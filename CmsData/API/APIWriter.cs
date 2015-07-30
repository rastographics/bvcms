using System;
using System.Collections.Generic;
using System.Xml;
using UtilityExtensions;
using System.Text;

namespace CmsData.API
{
    public class APIWriter
    {
        private XmlWriter w;
        private StringBuilder sb;
        public bool NoDefaults { get; set; }

        class Pending
        {
            public string PendingStart { get; set; }
            public bool PendingEnd { get; set; }

            public Pending(string ele)
            {
                PendingStart = ele;
            }
        }
        private Stack<Pending> stack = new Stack<Pending>(); 

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
            CheckPendingStart();
            w.WriteStartElement(element);
            return this;
        }

        private void CheckPendingStart()
        {
            if (stack.Count > 0)
            {
                var p = stack.Peek();
                if (!p.PendingEnd)
                {
                    w.WriteStartElement(p.PendingStart);
                    p.PendingEnd = true;
                }
            }
        }

        public APIWriter StartPending(object element)
        {
            stack.Push(new Pending(element.ToString()));
            return this;
        }
        public APIWriter StartPending(string element)
        {
            stack.Push(new Pending(element));
            return this;
        }
        public APIWriter End()
        {
            w.WriteEndElement();
            return this;
        }
        public APIWriter EndPending()
        {
            var p = stack.Pop();
            if(p.PendingEnd)
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
        public APIWriter Add(object element, object i)
        {
            var s = tostr(i);
            if (s.HasValue() || NoDefaults)
            {
                CheckPendingStart();
                w.WriteElementString(element.ToString(), s);
            }
            return this;
        }
        public APIWriter AddIfTrue(object element, bool b)
        {
            var s = tostr(b);
            if(b)
            {
                CheckPendingStart();
                w.WriteElementString(element.ToString(), s);
            }
            return this;
        }
        public APIWriter AddCdata(string element, string cdata)
        {
            if(cdata.HasValue())
            {
                CheckPendingStart();
                Start(element);
                w.WriteCData("\n" + cdata + "\n");
                End();
            }
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
