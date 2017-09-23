using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Models.ExtraValues
{
    // The specs for a single Standard Extra Value
    public class Value : CmsData.ExtraValue.Value
    {
        internal ExtraValue Extravalue;
        internal ExtraValueModel Model; // only applies to standard extra values
        public bool HasValue => Extravalue != null && Extravalue.ToString().HasValue();

        public string PrimaryKey => Model.Id2.HasValue ? $"{Model.Id}.{Model.Id2}" : Id.ToString();
        public int? Id2 => Extravalue.Id2;

        public string DataSource
        {
            get
            {
                var n = HttpUtility.UrlEncode(Name);
                var source = "";
                if (Type == "Code" && Standard)
                    source = $"/ExtraValue/Codes/{Model.Table}/{Model.Location}?name={n}";
                else if (Type == "Bits")
                    source = $"/ExtraValue/Bits/{Model.Table}/{Model.Location}/{Id}?name={n}";
                return source.HasValue() ? source : "";
            }
        }

        public string EditUrl => $"/ExtraValue/Edit/{Model.Table}/{Model.Location}/{Type}";

        public string DeleteUrl
        {
            get
            {
                var n = HttpUtility.UrlEncode(Name);
                if (Model.Location != "Adhoc")
                    return $"/ExtraValue/Delete/{Model.Table}/{Model.Location}/{Id}?name={n}";
                return $"/ExtraValue/DeleteAdhoc/{Model.Table}/{Id}?name={n}";
            }
        }

        public string EditableClass => $"click-{Type}{(Type == "Code" && Standard ? "-Select" : "")}";

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

        public string DataName => HttpUtility.UrlEncode(Name);

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

        public string DisplayName => !Standard ? Name : NoPrefix(Name);

        public static Value FromValue(CmsData.ExtraValue.Value ptr)
        {
            var v = new Value
            {
                Name = ptr.Name,
                Type = ptr.Type,
                VisibilityRoles = ptr.VisibilityRoles,
                EditableRoles = ptr.EditableRoles,
                Codes = ptr.Codes,
                Order = ptr.Order,
                Standard = ptr.Standard,
                Link = ptr.Link
            };
            return v;
        }

        internal static Value AddField(Value f, ExtraValue ev, ExtraValueModel m = null)
        {
            if (f == null)
            {
                f = new Value
                {
                    Name = ev.Field,
                    Standard = false
                };
                f.Type = ev.Type;
            }
            else
            {
                f.Standard = true;
            }

            f.Id = ev?.Id ?? (m?.Id ?? 0);
            f.Extravalue = ev;
            f.Model = m;
            return f;
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
                case "Attr":
                    return "Attributes";
                case "Date":
                    return ev.DateValue.FormatDate();
                case "Bits":
                {
                    var q = (from e in Model.ListExtraValues()
                             where e.BitValue == true
                             where e.Id == Id
                             where Codes.Select(x => x.Text).Contains(e.Field)
                             select NoPrefix(e.Field)).ToList();

                    if (DbUtil.Db.Setting("UX-RenderCheckboxBullets"))
                    {
                        if (q.ToList().Count == 0) return string.Empty;

                        var bullets = string.Join("</span></li><li><span>", q);
                        return "<ul class=\"extra-value-list\"><li><span>" + bullets + "</span></li></ul>";
                    }

                    return string.Join("<br/>", q);
                }
                case "Int":
                    return ev.IntValue.ToString2("d");
            }
            return "";
        }

        public static string NoPrefix(string s)
        {
            var a = s.SplitStr(":", 2);
            return a.Length > 1 ? a[1] : s;
        }

        public HtmlString HyperLink()
        {
            if (Link == null)
                Link = "missing html hyperlink";
            var s = HttpUtility.HtmlDecode(Link);
            s = s.Replace("{id}", Id.ToString());
            if (s.Contains("{queryid}"))
                s = s.Replace("{queryid}", Model.CurrentPersonQueryId().ToString());
            if (s.Contains("{mfid}"))
                s = s.Replace("{mfid}", Model.CurrentPersonMainFellowshipId().ToString());
            return new HtmlString(s);
        }
    }
}
