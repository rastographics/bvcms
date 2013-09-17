using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Community.CsharpSqlite;
using UtilityExtensions;
using System.Linq;
using CmsData;

namespace CmsData
{
    public partial class QueryBuilderClause
    {
        public string ToXml(string from, int id)
        {
            XmlWriter w;
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = new UTF8Encoding(false);
            var sb = new StringBuilder();
            using (w = XmlWriter.Create(sb, settings))
                SendToWriter(w, from, id);
            return sb.ToString();
        }
        public void SendToWriter(XmlWriter w, string from, int id)
        {
            using (w)
            {
                w.WriteStartElement("Condition");
                w.WriteAttributeString("from", from);
                w.WriteAttributeString("fromid", id.ToString());
                w.WriteAttributeString("dbname", Util.Host);
                WriteAttributes(w);
                foreach (var qc in Clauses)
                    qc.SendToWriter(w);
                w.WriteEndElement();
            }
        }
        public void SendToWriter(XmlWriter w)
        {
            w.WriteStartElement("Condition");
            WriteAttributes(w);
            foreach (var qc in Clauses)
                qc.SendToWriter(w);
            w.WriteEndElement();
        }

        private void WriteAttributes(XmlWriter w)
        {
            w.WriteAttributeString("ClauseOrder", ClauseOrder.ToString());
            w.WriteAttributeString("Field", Field);
            if (Description.HasValue())
                w.WriteAttributeString("Description", Description);
            w.WriteAttributeString("Comparison", Comparison);
            if (TextValue.HasValue())
                w.WriteAttributeString("TextValue", TextValue);
            if (DateValue.HasValue)
                w.WriteAttributeString("DateValue", DateValue.ToString());
            if (CodeIdValue.HasValue())
                w.WriteAttributeString("CodeIdValue", CodeIdValue);
            if (StartDate.HasValue)
                w.WriteAttributeString("StartDate", StartDate.ToString());
            if (EndDate.HasValue)
                w.WriteAttributeString("EndDate", EndDate.ToString());
            if (Program > 0)
                w.WriteAttributeString("Program", Program.ToString());
            if (Division > 0)
                w.WriteAttributeString("Division", Division.ToString());
            if (Organization > 0)
                w.WriteAttributeString("Organization", Organization.ToString());
            if (Days > 0)
                w.WriteAttributeString("Days", Days.ToString());
            if (Quarters.HasValue())
                w.WriteAttributeString("Quarters", Quarters);
            if (Tags.HasValue())
                w.WriteAttributeString("Tags", Tags);
            if (Schedule > 0)
                w.WriteAttributeString("Schedule", Schedule.ToString());
            if (Age.HasValue)
                w.WriteAttributeString("Age", Age.ToString());
        }
        public class QueryImportInfo
        {
            public int newid { get; set; }
            public string dbname { get; set; }
            public string from { get; set; }
            public int fromid { get; set; }
        }

        public static QueryImportInfo Import(CMSDataContext Db, string text, string name = null)
        {
            var x = XDocument.Parse(text);
            Debug.Assert(x.Root != null, "x.Root != null");
            var c = ImportClause(x.Root);
            if (name.HasValue())
                c.Description = name;
            Db.QueryBuilderClauses.InsertOnSubmit(c);
            Db.SubmitChanges();
            var ret = new QueryImportInfo()
            {
                newid = c.QueryId,
            };
            return ret;
        }
        private static QueryBuilderClause ImportClause(XElement r)
        {
            var c = new QueryBuilderClause
            {
                Field = Attribute(r, "Field"),
                ClauseOrder = Attribute(r, "ClauseOrder").ToInt(),
                Comparison = Attribute(r, "Comparison"),
                TextValue = Attribute(r, "TextValue"),
                DateValue = AttributeDate(r, "DateValue"),
                CodeIdValue = Attribute(r, "CodeIdValue"),
                StartDate = AttributeDate(r, "StartDate"),
                EndDate = AttributeDate(r, "EndDate"),
                Program = Attribute(r, "Program").ToInt(),
                Division = Attribute(r, "Division").ToInt(),
                Organization = Attribute(r, "Organization").ToInt(),
                Days = Attribute(r, "Days").ToInt(),
                Quarters = Attribute(r, "Quarters"),
                Tags = Attribute(r, "Tags"),
                Schedule = Attribute(r, "Schedule").ToInt(),
                Age = Attribute(r, "Age").ToInt(),
                SavedBy = Util.UserName
            };
            if (c.Field == "Group")
                foreach (var rr in r.Elements())
                    c.Clauses.Add(ImportClause(rr));
            return c;
        }
        private static string Attribute(XElement r, string attr)
        {
            return Attribute(r, attr, null);
        }
        private static string Attribute(XElement r, string attr, string def)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return def;
            return a.Value;
        }
        private static DateTime? AttributeDate(XElement r, string attr)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return null;
            return a.Value.ToDate();
        }
    }
}