using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData;
using CmsData.API;
using DocumentFormat.OpenXml.Office.CustomUI;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class Fields : IXmlSerializable
    {
        public List<Field> fields { get; set; }
        public XmlSchema GetSchema() { throw new NotImplementedException(); }

        public void ReadXml(XmlReader reader)
        {
            var s = reader.ReadOuterXml();
            var x = XDocument.Parse(s);
            if (x.Root == null) return;
            fields = new List<Field>();
            foreach (var e in x.Root.Elements("Field"))
                fields.Add(Util.DeSerialize<Field>(e.ToString()));
        }

        public void WriteXml(XmlWriter writer)
        {
            var w = new APIWriter(writer);
            writer.WriteComment(DateTime.Now.ToString());
            foreach (var f in fields)
                Util.Serialize(f, writer);
        }
    }
    public class Field : IXmlSerializable
    {
        public string name { get; set; }
        public string type { get; set; }

        public string location
        {
            get
            {
                if (_location.HasValue())
                    return _location;
                return "Standard";
            }
            set
            {
                if (value != "MemberProfile")
                    _location = value;
            }
        }

        public string table
        {
            get
            {
                if (_table.HasValue())
                    return _table;
                return "Person";
            }
            set { _table = value; }
        }

        public string VisibilityRoles { get; set; }

        public List<string> Codes { get; set; }

        private string _table;
        private string _location;

        internal int order;
        public int Id;
        public bool NonStandard;
        public bool standard { get { return !NonStandard; } }

        internal ExtraValue extravalue;
        internal ExtraValueModel model; // only applies to standard extra values

        internal static Field AddField(Field f, ExtraValue ev, ExtraValueModel m = null)
        {
            if (f == null)
            {
                f = new Field
                {
                    name = ev.Field,
                    NonStandard = true,
                };
                f.type = ev.StrValue.HasValue() ? "Code"
                    : ev.Data.HasValue() ? "Data"
                    : ev.DateValue.HasValue ? "Date"
                    : ev.IntValue.HasValue ? "Int"
                    : ev.BitValue.HasValue ? "Bit"
                    : "Code";
            }
            f.Id = ev != null ? ev.Id : m != null ? m.Id : 0;
            f.extravalue = ev;
            f.model = m;
            return f;
        }
        public bool UserCanView()
        {
            if (!VisibilityRoles.HasValue())
                return true;
            var a = VisibilityRoles.SplitStr(",");
            var user = HttpContext.Current.User;
            return a.Any(role => user.IsInRole(role.Trim()));
        }
        public bool UserCanEdit()
        {
            var user = HttpContext.Current.User;
            return user.IsInRole("Edit");
        }

        public bool HasValue
        {
            get { return extravalue != null && extravalue.ToString().HasValue(); }
        }

        public override string ToString()
        {
            var ev = extravalue;
            if (ev == null)
                ev = new ExtraValue();
            switch (type)
            {
                case "Bit":
                    if (extravalue == null)
                        if (NonStandard)
                            return "Click to edit";
                        else return "false";
                    return ev.BitValue.ToString();
                case "Code":
                    return ev.StrValue;
                case "Text":
                case "Text2":
                case "Data":
                    return (ev.Data ?? "").Trim();
                case "Date":
                    return ev.DateValue.FormatDate();
                case "Bits":
                    {
                        var q = from e in DbUtil.Db.PeopleExtras
                                where e.BitValue == true
                                where e.PeopleId == Id
                                where Codes.Contains(e.Field)
                                select e.Field;
                        return string.Join(", ", q);
                    }
                case "Int":
                    return extravalue.IntValue.ToString();
            }
            return "";
        }

        public string DataSource
        {
            get
            {
                var n = HttpUtility.UrlEncode(name);
                var source = "";
                if (type == "Code" && standard)
                    source = "/ExtraValue/Codes/{0}?name={1}".Fmt(table, n);
                else if (type == "Bits")
                    source = "/ExtraValue/Bits/{0}/{1}?name={2}".Fmt(model.Table, Id, n);
                return source.HasValue() ? "data-source={0}".Fmt(source) : "";
            }
        }
        public string EditUrl
        {
            get
            {
                var n = HttpUtility.UrlEncode(name);
                return "/ExtraValue/Edit/{0}/{1}".Fmt(model.Table, type, Id, n);
            }
        }
        public string DeleteUrl
        {
            get
            {
                if (standard)
                    return "";
                var n = HttpUtility.UrlEncode(name);
                return "/ExtraValue/Delete/{0}?name={1}".Fmt(Id, n);
            }
        }
        public string EditableClass
        {
            get
            {
                return "click-pencil click-{0}{1}".Fmt(type, type == "Code" && standard ? "-Select" : "");
            }
        }
        public string DataValue
        {
            get
            {
                if (type == "Code" && standard && HasValue)
                    return "data-value={0}".Fmt(this);
                if (type == "Bits")
                    return "data-value={0}".Fmt(model.ListBitsJson(name)).Replace('"', '\'');
                if (type == "Bit")
                    return "data-value={0}".Fmt(this);
                return "";
            }
        }
        public string DataName
        {
            get
            {
                return HttpUtility.UrlEncode(name);
            }
        }

        public XmlSchema GetSchema() { throw new NotImplementedException(); }

        public void ReadXml(XmlReader reader)
        {
            var s = reader.ReadOuterXml();
            var x = XDocument.Parse(s);
            if (x.Root == null) 
                return;
            var e = x.Root;
            name = e.GetAttr("name");
            type = e.GetAttr("type");
            table = e.GetAttr("table");
            location = e.GetAttr("location");
            VisibilityRoles = e.GetAttr("VisibilityRoles");
            Codes = new List<string>();
            if (type == "Code" || type == "Bits")
            {
                var codes = e.Element("Codes");
                foreach (var c in codes.Elements("string"))
                    Codes.Add(c.Value);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            var w = new APIWriter(writer);
            w.Attr("name", name);
            w.Attr("location", location);
            w.Attr("type", type);
            w.Attr("table", table);
            w.Attr("VisibilityRoles", VisibilityRoles);
            if (type == "Code" || type == "Bits")
            {
                w.Start("Codes");
                foreach (var c in Codes)
                    w.Add("string", c);
                w.End();
            }
        }
    }
}