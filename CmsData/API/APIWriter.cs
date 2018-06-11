using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UtilityExtensions;
using System.Text;

namespace CmsData.API
{
    public class APIWriter
    {
        public XmlWriter writer;
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
            writer = XmlWriter.Create(sb, settings);
        }

        public APIWriter(XmlWriter writer)
        {
            this.writer = writer;
        }
        public APIWriter(Stream stream)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = new System.Text.UTF8Encoding(false);
            writer = XmlWriter.Create(stream, settings);
        }
        public APIWriter Start(string element)
        {
            CheckPendingStart();
            writer.WriteStartElement(element);
            return this;
        }

        public APIWriter AddComment(string s)
        {
            writer.WriteComment(s);
            return this;
        }

        private void CheckPendingStart()
        {
            if (stack.Count > 0)
            {
                var p = stack.Peek();
                if (!p.PendingEnd)
                {
                    writer.WriteStartElement(p.PendingStart);
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
            writer.WriteEndElement();
            return this;
        }
        public APIWriter EndPending()
        {
            var p = stack.Pop();
            if (p.PendingEnd)
                writer.WriteEndElement();
            return this;
        }
        public APIWriter Attr(string attr, object i)
        {
            var s = tostr(i);
            if (s.HasValue() || NoDefaults)
            {
                CheckPendingStart();
                writer.WriteAttributeString(attr, s);
            }
            return this;
        }
        public APIWriter AttrIfTrue(string attr, bool i)
        {
            if (i == false)
                return this;
            CheckPendingStart();
            writer.WriteAttributeString(attr, i.ToString());
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
                writer.WriteElementString(element.ToString(), s);
            }
            return this;
        }
        public APIWriter AddIfTrue(object element, bool b)
        {
            var s = tostr(b);
            if (b)
            {
                CheckPendingStart();
                writer.WriteElementString(element.ToString(), s);
            }
            return this;
        }
        public APIWriter AddCdata(string element, string cdata)
        {
            if (cdata.HasValue())
            {
                CheckPendingStart();
                Start(element);
                //w.WriteCData(cdata);
                writer.WriteCData("\n" + cdata.Trim() + "\n");
                End();
            }
            return this;
        }
        public APIWriter AddText(string text)
        {
            writer.WriteString(text);
            return this;
        }
        public override string ToString()
        {
            writer.Flush();
            return sb?.ToString() ?? "";
        }
        ~APIWriter()
        {
            writer.Close();
        }
    }
}
