using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Community.CsharpSqlite;
using IronPython.Modules;
using Microsoft.Scripting.Debugging;
using UtilityExtensions;
using System.Linq;
using CmsData;
namespace CmsData
{
    public partial class Condition
    {
        public void Save(CMSDataContext Db, bool increment = false)
        {
            var q = Db.LoadQueryById2(Id);
            if (q == null)
            {
                q = new Query 
                {
                    QueryId = Id,
                    Owner = Util.UserName,
                    Created = DateTime.Now, 
                };
                Db.Queries.InsertOnSubmit(q);
            }

            if (CopiedFrom.HasValue)
                q.CopiedFrom = CopiedFrom;
            q.Name = Description;
            q.LastRun = DateTime.Now;
            if(increment)
                q.RunCount = q.RunCount + 1;
            q.Text = ToXml();
	        Db.SubmitChanges();
        }

        public string ToXml()
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = new UTF8Encoding(false);
            var sb = new StringBuilder();
            using (var w = XmlWriter.Create(sb, settings))
                SendToWriter(w);
            return sb.ToString();
        }
        public void SendToWriter(XmlWriter w)
        {
            w.WriteStartElement("Condition");
            WriteAttributes(w);
            foreach (var qc in Conditions)
                qc.SendToWriter(w);
            w.WriteEndElement();
        }
        private void WriteAttributes(XmlWriter w)
        {
            w.WriteAttributeString("Id", Id.ToString());
            w.WriteAttributeString("Order", Order.ToString());
            w.WriteAttributeString("Field", Field);
            w.WriteAttributeString("Comparison", Comparison);
            if (Description.HasValue())
                w.WriteAttributeString("Description", Description);
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
            if (ExtraData.HasValue())
                w.WriteAttributeString("Quarters", ExtraData);
            if (Tags.HasValue())
                w.WriteAttributeString("Tags", Tags);
            if (Schedule > 0)
                w.WriteAttributeString("Schedule", Schedule.ToString());
            if (Age.HasValue)
                w.WriteAttributeString("Age", Age.ToString());
        }
        public static Condition Import(string text, string name = null, bool newGuids = false)
        {
            if (!text.HasValue())
                return CreateNewGroupClause(name);
            var x = XDocument.Parse(text);
            Debug.Assert(x.Root != null, "x.Root != null");
            var c = ImportClause(x.Root, null, newGuids);
            return c;
        }
        private static Condition ImportClause(XElement r, Condition p, bool newGuids)
        {
            var allClauses = p == null ? new Dictionary<Guid, Condition>() : p.AllConditions;
            Guid? parentGuid = null;
            if (p != null)
                parentGuid = p.Id;
            var c = new Condition
            {
                ParentId = parentGuid,
                Id = AttributeGuid(r, "Id"),
                Order = AttributeInt(r, "Order"),
                Field = Attribute(r, "Field"),
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
                ExtraData = Attribute(r, "Quarters"),
                Tags = Attribute(r, "Tags"),
                Schedule = Attribute(r, "Schedule").ToInt(),
                Age = Attribute(r, "Age").ToInt(),
                Owner = Attribute(r, "Owner"),
                AllConditions = allClauses
            };
            if(p == null)
                c.Description = Attribute(r, "Description");
            c.AllConditions.Add(c.Id, c);
            if (newGuids)
                c.Id = Guid.NewGuid();
            if (c.Field == "Group")
                foreach (var rr in r.Elements())
                    ImportClause(rr, c, newGuids);
            return c;
        }
        private static string Attribute(XElement r, string attr, string def = null)
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
        private static int AttributeInt(XElement r, string attr)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return 0;
            return a.Value.ToInt();
        }
        private static Guid AttributeGuid(XElement r, string attr)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return Guid.NewGuid();
            return new Guid(a.Value);
        }
    }
}