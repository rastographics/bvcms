using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{

    // The specs for a single Standard Extra Value
    public class Value : CmsData.ExtraValue.Value
    {
        public static Value FromValue(CmsData.ExtraValue.Value ptr)
        {
            var v = new Value()
            {
                Name = ptr.Name,
                Type = ptr.Type,
                VisibilityRoles = ptr.VisibilityRoles,
                Codes = ptr.Codes,
                Order = ptr.Order,
                Standard = ptr.Standard,
                Link = ptr.Link,
            };
            return v;
        }

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
                return source.HasValue() ? source : "";
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
                var n = HttpUtility.UrlEncode(Name);
                if (Model.Location != "Adhoc")
                    return "/ExtraValue/Delete/{0}/{1}?name={2}".Fmt(Model.Table, Id, n);
                return "/ExtraValue/DeleteAdhoc/{0}/{1}?name={2}".Fmt(Model.Table, Id, n);
            }
        }
        public string EditableClass
        {
            get
            {
                return "click-{0}{1}".Fmt(Type, Type == "Code" && Standard ? "-Select" : "");
            }
        }
        public string DataValue
        {
            get
            {
                if (Type == "Bits")
                    return Model.ListBitsJson(Name).Replace('"', '\'');
                if (Type == "Code" && Standard && HasValue)
                    return ToString();
                return Type == "Bit" ? ToString() : "";
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
        public string DisplayName
        {
            get { return NoPrefix(Name); }
        }
        public static string NoPrefix(string s)
        {
            var a = s.SplitStr(":", 2);
            return a.Length > 1 ? a[1] : s;
        }

        public HtmlString HyperLink()
        {
            var s = HttpUtility.HtmlDecode(Link);
            s = s.Replace("{id}", Id.ToString());
            if(s.Contains("{queryid}"))
                s = s.Replace("{queryid}", Model.CurrentPersonQueryId().ToString());
            return new HtmlString(s);
        }
    }
}