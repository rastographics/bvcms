using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UtilityExtensions;
using System.Linq;
namespace CmsData
{
    public partial class Condition
    {
        public void Save(CMSDataContext Db, bool increment = false, string owner = null)
        {
            var q = (from e in Db.Queries
                     where e.QueryId == Id
                     select e).FirstOrDefault();

            if (q == null)
            {
                q = new Query
                {
                    QueryId = Id,
                    Owner = Util.UserName,
                    Created = Util.Now,
                    Ispublic = IsPublic,
                    Name = Description
                };
                Db.Queries.InsertOnSubmit(q);
            }
            if(increment)
                q.LastRun = Util.Now;

            if (Description != q.Name)
            {
                var same = (from v in Db.Queries
                            where !v.Ispublic
                            where v.Owner == Util.UserName
                            where v.Name == Description
                            orderby v.LastRun descending
                            select v).FirstOrDefault();
                if (same != null)
                    same.Text = ToXml();
                else
                {
                    var c = Clone();
                    var cq = new Query
                    {
                        QueryId = c.Id,
                        Owner = Util.UserName,
                        Created = q.Created,
                        Ispublic = q.Ispublic,
                        Name = q.Name,
                        Text = c.ToXml(),
                        RunCount = q.RunCount,
                        CopiedFrom = q.CopiedFrom,
                        LastRun = q.LastRun
                    };
                    Db.Queries.InsertOnSubmit(cq);
                }
            }
            q.Name = Description;
            q.LastRun = Util.Now;
            if(owner.HasValue())
                q.Owner = owner;
            q.Ispublic = IsPublic;
            if (increment)
                q.RunCount = q.RunCount + 1;
            q.Text = ToXml();
            Db.SubmitChanges();
        }
        public string ToXml(bool newGuids = false)
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = new UTF8Encoding(false);
            var sb = new StringBuilder();
            using (var w = XmlWriter.Create(sb, settings))
                SendToWriter(w, newGuids);
            return sb.ToString();
        }
        public void SendToWriter(XmlWriter w, bool newGuids = false, string name = null)
        {
            w.WriteStartElement("Condition");
            WriteAttributes(w, newGuids, name);
            foreach (var qc in Conditions)
                qc.SendToWriter(w, newGuids);
            w.WriteEndElement();
        }
        private void WriteAttributes(XmlWriter w, bool newGuids = false, string name = null)
        {
            w.WriteAttr("Id", newGuids 
                ? Guid.NewGuid().ToString() : Id.ToString());

            w.WriteAttr("Order", Order.ToString());
            w.WriteAttr("Field", ConditionName);
            w.WriteAttr("Comparison", Comparison, "AllTrue");
            if (name.HasValue())
            {
                Description = name;
            }
            w.WriteAttr("Description", Description, "scratchpad");
            w.WriteAttr("PreviousName", Description, "scratchpad");
            w.WriteAttr("TextValue", TextValue);
            w.WriteAttr("DateValue", DateValue);
            w.WriteAttr("CodeIdValue", CodeIdValue);
            w.WriteAttr("StartDate", StartDate);
            w.WriteAttr("EndDate", EndDate);
            w.WriteAttr("Program", Program);
            w.WriteAttr("Ministry", Ministry);
            w.WriteAttr("Division", Division);
            w.WriteAttr("Organization", Organization);
            w.WriteAttr("OrgType", OrgType);
            w.WriteAttr("Days", Days);
            w.WriteAttr("Quarters", Quarters);
            w.WriteAttr("Tags", Tags);
            w.WriteAttr("Schedule", Schedule);
            w.WriteAttr("Campus", Campus);
            if (ConditionName == "FamilyHasChildrenAged")
                w.WriteAttr("Age", Age ?? 0, 12);
            w.WriteAttr("SavedQueryIdDesc", SavedQuery, "scratchpad");
            w.WriteAttr("OnlineReg", OnlineReg);
            w.WriteAttr("OrgStatus", OrgStatus);
            w.WriteAttr("OrgType2", OrgType2);
            w.WriteAttr("OrgName", OrgName);
            if(IsScratchPad) // disabled conditions only work on scratchpad
                w.WriteAttr("DisableOnScratchpad", DisableOnScratchpad);
        }
        public static Condition Import(string text, string name = null, bool newGuids = false, Guid? topguid = null)
        {
            if (!text.HasValue())
                return CreateNewGroupClause(name);
            var x = XDocument.Parse(text);
            Debug.Assert(x.Root != null, "x.Root != null");
            var c = ImportClause(x.Root, null, newGuids, topguid, name);
            return c;
        }
        private static Condition ImportClause(XElement r, Condition p, bool newGuids, Guid? topguid = null, string name = null)
        {
            var allClauses = p == null ? new Dictionary<Guid, Condition>() : p.AllConditions;
            Guid? parentGuid = null;
            if (p != null)
                parentGuid = p.Id;
            var c = new Condition
            {
                ParentId = parentGuid,
                Id = topguid ?? (newGuids ? Guid.NewGuid() : AttributeGuid(r, "Id")),
                Order = AttributeInt(r, "Order"),
                ConditionName = Attribute(r, "Field"),
                Comparison = Attribute(r, "Comparison", "AllTrue"),
                TextValue = Attribute(r, "TextValue"),
                DateValue = AttributeDate(r, "DateValue"),
                CodeIdValue = Attribute(r, "CodeIdValue"),
                StartDate = AttributeDate(r, "StartDate"),
                EndDate = AttributeDate(r, "EndDate"),
                Program = Attribute(r, "Program"),
                Division = Attribute(r, "Division"),
                Organization = Attribute(r, "Organization"),
                OrgType = Attribute(r, "OrgType"),
                Days = Attribute(r, "Days").ToInt(),
                Quarters = Attribute(r, "Quarters"),
                Tags = Attribute(r, "Tags"),
                Ministry = Attribute(r, "Ministry"),
                Schedule = Attribute(r, "Schedule"),
                Campus = Attribute(r, "Campus"),
                Age = Attribute(r, "Age").ToInt2(),
                Owner = Attribute(r, "Owner"),
                SavedQuery = Attribute(r, "SavedQueryIdDesc"),
                OrgName = Attribute(r, "OrgName"),
                OrgStatus = Attribute(r, "OrgStatus"),
                OnlineReg = Attribute(r, "OnlineReg"),
                OrgType2 = Attribute(r, "OrgType2").ToInt(),
                AllConditions = allClauses
            };
            if (p == null)
            {
                c.Description = Attribute(r, "Description");
                if (!c.Description.HasValue() && name == Util.ScratchPad2)
                    c.Description = name;
                c.PreviousName = Attribute(r, "PreviousName");
            }
            if (c.IsScratchPad)
            {
                c.DisableOnScratchpad = AttributeBool(r, "DisableOnScratchpad");
            }
            if (c.ConditionName == "MatchAnything")
            {
                c.Comparison = "Equal";
            }
            if (c.ConditionName != "FamilyHasChildrenAged")
            {
                c.Age = null;
            }
            c.AllConditions.Add(c.Id, c);
            if (c.ConditionName == "Group")
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
            return a.ToDate();
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
            Guid g;
            return Guid.TryParse(a.Value, out g) ? g : Guid.NewGuid();
        }
        private static bool AttributeBool(XElement r, string attr)
        {
            var a = r.Attributes(attr).FirstOrDefault();
            if (a == null)
                return false;
            return a.ToBool();
        }
        public static object TryParse(string s)
        {
            try
            {
                return Parse(s);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
        public static Condition Parse(string s, Guid? id = null)
        {
            var p = new Condition
            {
                Id = id ?? Guid.NewGuid(),
                ConditionName = "Group",
                AllConditions = new Dictionary<Guid, Condition>()
            };
            p.AllConditions.Add(p.Id, p);
            var m = new QueryParser(s);
            var c = m.ParseConditions(p);
            if (p.Conditions.Count() == 1 && c.IsGroup)
                return c; // Surrounding parentheses not needed for a single group
            return p; // return outer group
        }
    }

    public static class AttributeWriter
    {
        public static void WriteAttr(this XmlWriter w, string name, int n, int def = 0)
        {
            if (n.Equals(def))
                return;
            w.WriteAttributeString(name, n.ToString());
        }
        public static void WriteAttr(this XmlWriter w, string name, int? n, int def = 0)
        {
            if(!n.HasValue || n == def)
                return;
            w.WriteAttributeString(name, n.ToString());
        }
        public static void WriteAttr(this XmlWriter w, string name, string s, string def = null)
        {
            if (!s.HasValue() || s == def)
                return;
            w.WriteAttributeString(name, s);
        }
        public static void WriteAttr(this XmlWriter w, string name, DateTime? d)
        {
            if (!d.HasValue)
                return;
            w.WriteAttributeString(name, d.ToString());
        }
        public static void WriteAttr(this XmlWriter w, string name, bool b, bool def = false)
        {
            if (b.Equals(def))
                return;
            w.WriteAttributeString(name, b.ToString());
        }
    }
}
