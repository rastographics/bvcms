using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms.VisualStyles;
using System.Xml.Serialization;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    public class Value
    {
        [XmlAttribute] public string Name { get; set; }
        [XmlAttribute] public string Type { get; set; }
        [XmlAttribute] public string VisibilityRoles { get; set; }

        [XmlElement("Code")] 
        public List<string> Codes { get; set; }

        [XmlIgnore] public int Order;
        [XmlIgnore] public int Id;
        [XmlIgnore] public bool Standard;

        internal ExtraValue Extravalue;
        internal ExtraValueModel Model; // only applies to standard extra values

        internal static Value AddField(Value f, ExtraValue ev, ExtraValueModel m = null)
        {
            if (f == null)
            {
                f = new Value
                {
                    Name = ev.Field,
                    Standard = false,
                };
                f.Type = ev.Type;
            }
            else
                f.Standard = true;

            f.Id = ev != null ? ev.Id : m != null ? m.Id : 0;
            f.Extravalue = ev;
            f.Model = m;
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
            get { return Extravalue != null && Extravalue.ToString().HasValue(); }
        }

        public string DataSource
        {
            get
            {
                var n = HttpUtility.UrlEncode(Name);
                var source = "";
                if (Type == "Code" && Standard)
                    source = "/ExtraValue/Codes/{0}?name={1}".Fmt(Model.Table, n);
                else if (Type == "Bits")
                    source = "/ExtraValue/Bits/{0}/{1}?name={2}".Fmt(Model.Table, Id, n);
                return source.HasValue() ? "data-source={0}".Fmt(source) : "";
            }
        }
        public string EditUrl
        {
            get
            {
                var n = HttpUtility.UrlEncode(Name);
                return "/ExtraValue/Edit/{0}/{1}".Fmt(Model.Table, Type, Id, n);
            }
        }
        public string DeleteUrl
        {
            get
            {
                if (Model.Location != "Adhoc")
                    return "";
                var n = HttpUtility.UrlEncode(Name);
                return "/ExtraValue/DeleteAdhoc/{0}/{1}?name={2}".Fmt(Model.Table, Id, n);
            }
        }
        public string EditableClass
        {
            get
            {
                return "click-pencil click-{0}{1}".Fmt(Type, Type == "Code" && Standard ? "-Select" : "");
            }
        }
        public string DataValue
        {
            get
            {
                if (Type == "Code" && Standard && HasValue)
                    return "data-value={0}".Fmt(this);
                if (Type == "Bits")
                    return "data-value={0}".Fmt(Model.ListBitsJson(Name)).Replace('"', '\'');
                if (Type == "Bit")
                    return "data-value={0}".Fmt(this);
                return "";
            }
        }
        public string DataName
        {
            get
            {
                return HttpUtility.UrlEncode(Name);
            }
        }

        public string SwitchMultiLineText
        {
            get
            {
                if (Type == "Text")
                    return "Switch to Multiline";
                if (Type == "Text2")
                    return "Switch to Singleline";
                return "";
            }
        }

        public override string ToString()
        {
            var ev = Extravalue;
            if (ev == null)
                ev = new ExtraValue();
            switch (Type)
            {
                case "Bit":
                    if (Extravalue == null)
                        if (!Standard)
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
                        var q = from e in Model.ListExtraValues()
                                where e.BitValue == true
                                where e.Id == Id
                                where Codes.Contains(e.Field)
                                select e.Field;
                        return string.Join("<br/>\n", q);
                    }
                case "Int":
                    return ev.IntValue.ToString2("d");
            }
            return "";
        }
    }
}