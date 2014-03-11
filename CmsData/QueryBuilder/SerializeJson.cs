using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CmsData.Registration;
using UtilityExtensions;
using System.Linq;
namespace CmsData
{
    public partial class Condition
    {
        public string ToJson(bool newGuids = false)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = new UTF8Encoding(false);
            var sb = new StringBuilder();
            using (var w = XmlWriter.Create(sb, settings))
                SendToWriter(w, newGuids);
            return sb.ToString();
        }
        public void SendToJson(XmlWriter w, bool newGuids = false)
        {
            w.WriteStartElement("Condition");
            WriteJson(w, newGuids);
            foreach (var qc in Conditions)
                qc.SendToJson(w, newGuids);
            w.WriteEndElement();
        }
        private void WriteJson(XmlWriter w, bool newGuids = false)
        {
            if (newGuids)
                w.WriteAttributeString("Id", Guid.NewGuid().ToString());
            else
                w.WriteAttributeString("Id", Id.ToString());

            w.WriteAttributeString("Order", Order.ToString());
            w.WriteAttributeString("Field", ConditionName);
            w.WriteAttributeString("Comparison", Comparison);
            if (Description.HasValue())
                w.WriteAttributeString("Description", Description);
            if (PreviousName.HasValue())
                w.WriteAttributeString("PreviousName", Description);
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
            if (OrgType > 0)
                w.WriteAttributeString("OrgType", OrgType.ToString());
            if (Days > 0)
                w.WriteAttributeString("Days", Days.ToString());
            if (Quarters.HasValue())
                w.WriteAttributeString("Quarters", Quarters);
            if (Tags.HasValue())
                w.WriteAttributeString("Tags", Tags);
            if (Schedule > 0)
                w.WriteAttributeString("Schedule", Schedule.ToString());
            if (ConditionName != "FamilyHasChildrenAged")
                Age = null;
            if (Age.HasValue)
                w.WriteAttributeString("Age", Age.ToString());
            if (SavedQuery.HasValue())
                w.WriteAttributeString("SavedQueryIdDesc", SavedQuery);
        }
        public static Condition ImportFromJson(string text, string name = null, bool newGuids = false, Guid? topguid = null)
        {
            if (!text.HasValue())
                return CreateNewGroupClause(name);
            var x = XDocument.Parse(text);
            Debug.Assert(x.Root != null, "x.Root != null");
            var c = ImportJsonClause(x.Root, null, newGuids, topguid);
            return c;
        }
        private static Condition ImportJsonClause(XElement r, Condition p, bool newGuids, Guid? topguid)
        {
            var allClauses = p == null ? new Dictionary<Guid, Condition>() : p.AllConditions;
            Guid? parentGuid = null;
            if (p != null)
                parentGuid = p.Id;
            var c = new Condition
            {
                ParentId = parentGuid,
                Id = topguid ?? (newGuids ? Guid.NewGuid() : AttributeGuid(r, "Id")),
                Order = PropertyInt(r, "Order"),
                ConditionName = Property(r, "Field"),
                Comparison = Property(r, "Comparison"),
                TextValue = Property(r, "TextValue"),
                DateValue = AttributeDate(r, "DateValue"),
                CodeIdValue = Property(r, "CodeIdValue"),
                StartDate = AttributeDate(r, "StartDate"),
                EndDate = AttributeDate(r, "EndDate"),
                Program = Property(r, "Program").ToInt(),
                Division = Property(r, "Division").ToInt(),
                Organization = Property(r, "Organization").ToInt(),
                OrgType = Property(r, "OrgType").ToInt(),
                Days = Property(r, "Days").ToInt(),
                Quarters = Property(r, "Quarters"),
                Tags = Property(r, "Tags"),
                Schedule = Property(r, "Schedule").ToInt(),
                Age = Property(r, "Age").ToInt2(),
                Owner = Property(r, "Owner"),
                SavedQuery = Property(r, "SavedQueryIdDesc"),
                AllConditions = allClauses
            };
            if (c.ConditionName != "FamilyHasChildrenAged")
                c.Age = null;
            if (p == null)
            {
                c.Description = Property(r, "Description");
                c.PreviousName = Property(r, "PreviousName");
            }
            c.AllConditions.Add(c.Id, c);
            if (c.ConditionName == "Group")
                foreach (var rr in r.Elements())
                    ImportJsonClause(rr, c, newGuids, null);
            return c;
        }
        private static string Property(XElement r, string attr, string def = null)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return def;
            return a.Value;
        }
        private static DateTime? PropertyDate(XElement r, string attr)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return null;
            return a.Value.ToDate();
        }
        private static int PropertyInt(XElement r, string attr)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return 0;
            return a.Value.ToInt();
        }
        private static Guid PropertyGuid(XElement r, string attr)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return Guid.NewGuid();
            Guid g;
            return Guid.TryParse(a.Value, out g) ? g : Guid.NewGuid();
        }
    }
}