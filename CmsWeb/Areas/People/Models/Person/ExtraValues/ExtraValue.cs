using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Xml.Serialization;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public partial class StandardExtraValues
    {
        [Serializable]
        public class Fields
        {
            [XmlElement("Field")]
            public Field[] fields { get; set; }
        }
        [Serializable]
        public class Field
        {
            [XmlAttribute]
            public string name { get; set; }
            [XmlAttribute]
            public string type { get; set; }
            [XmlAttribute]
            public string location { get; set; }
            [XmlAttribute]
            public string VisibilityRoles { get; set; }
            public List<string> Codes { get; set; }
            internal int order;
            public int peopleid;
            public bool nonstandard;
            public bool standard { get { return !nonstandard; } }

            internal PeopleExtra extravalue;
            internal static Field AddField(Field f, PeopleExtra v)
            {
                if (f == null)
                {
                    f = new Field
                    {
                        name = v.Field,
                        nonstandard = true,
                        peopleid = v.PeopleId,
                        extravalue = v,
                    };
                    f.type = v.StrValue.HasValue() ? "Code"
                        : v.Data.HasValue() ? "Data"
                        : v.DateValue.HasValue ? "Date"
                        : v.IntValue.HasValue ? "Int"
                        : v.BitValue.HasValue ? "Bit"
                        : "Code";
                }
                f.extravalue = v;
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
                if (extravalue == null)
                    extravalue = new PeopleExtra();
                switch (type)
                {
                    case "Bit":
                        if (extravalue == null)
                            if (nonstandard)
                                return "Click to edit";
                            else return "false";
                        return extravalue.BitValue.ToString();
                    case "Code":
                        return extravalue.StrValue;
                    case "Data":
                        return extravalue.Data;
                    case "Date":
                        return extravalue.DateValue.FormatDate();
                    case "Bits":
                        {
                            var q = from e in DbUtil.Db.PeopleExtras
                                    where e.BitValue == true
                                    where e.PeopleId == peopleid
                                    where Codes.Contains(e.Field)
                                    select e.Field;
                            return string.Join(",", q);
                        }
                    case "Int":
                        if (extravalue.IntValue2.HasValue)
                            return "{0} {1}".Fmt(extravalue.IntValue, extravalue.IntValue2);
                        return extravalue.IntValue.ToString();
                }
                return "";
            }

            public string EditableDataSource
            {
                get
                {
                    var n = HttpUtility.UrlEncode(name);
                    var source = "";
                    if (type == "Code" && standard)
                        source = "/Person2/ExtraValueCodes?name={0}".Fmt(n);
                    else if (type == "Bits")
                        source = "/Person2/ExtraValueBits/{0}?name={1}".Fmt(peopleid, n);
                    return source.HasValue() ? "data-source={0}".Fmt(source) : "";
                }
            }
            public string EditableDeleteUrl
            {
                get
                {
                    if (standard)
                        return "";
                    var n = HttpUtility.UrlEncode(name);
                    return "/Person2/DeleteExtra/{0}?name={1}".Fmt(peopleid, n);
                }
            }
            public string EditableDataType
            {
                get
                {
                    return "click-{0}{1}".Fmt(type, type == "Code" && standard ? "-Select" : "");
                }
            }
            public string EditableDataValue
            {
                get
                {
                    if (type == "Code" && standard && HasValue)
                        return "data-value={0}".Fmt(this);
                    if (type == "Bits")
                        return "data-value={0}".Fmt(ExtraValueSetBitsJson(peopleid, name)).Replace('"', '\'');
                    if (type == "Bit")
                        return "data-value={0}".Fmt(this);
                    return "";
                }
            }
            public string EditableDataKey
            {
                get
                {
                    return "{0}-{1}".Fmt(type, peopleid);
                }
            }
            public string EditableDataName
            {
                get
                {
                    return HttpUtility.UrlEncode(name);
                }
            }
        }
    }
}