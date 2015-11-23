/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;
using System.Web;
using System.Web.Caching;

namespace CmsData
{

    public class FieldClass2
    {
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                queryType = ConvertQueryType(value);
            }
        }
        private QueryType queryType;
        public QueryType QueryType
        {
            get
            {
                return queryType;
            }
            set
            {
                queryType = value;
                Name = value.ToString();
            }
        }
        public string CategoryTitle { get; set; }
        public string QuartersTitle { get; set; }
        private string title;
        public string Title
        {
            get { return title.HasValue() ? title : Name; }
            set { title = value; }
        }
        public FieldType Type { get; set; }
        private string @params;
        public string Params
        {
            get { return @params; }
            set
            {
                @params = value;
                if (value.HasValue())
                    ParamList = value.SplitStr(",").ToList();
            }
        }
        public List<string> ParamList { get; set; }
        public string DataSource { get; set; }
        public string DataValueField { get; set; }

        private string FormatArgs(Condition c)
        {
            var list = new List<ParamArg>();
            foreach (var s in ParamList)
            {
                string propname;
                switch (s)
                {
                    case "Week":
                        propname = "Quarters";
                        break;
                    case "View":
                        propname = "Quarters";
                        break;
                    case "PmmLabels":
                        propname = "Tags";
                        break;
                    case "SavedQueryIdDesc":
                        propname = "SavedQuery";
                        break;
                    case "Ministry":
                        propname = "Program";
                        break;
                    default:
                        propname = s;
                        break;
                }

                var prop = Util.GetProperty(c, propname) ?? "";

                //if (prop is DateTime?)
                //    prop = ((DateTime?)prop).FormatDate();
                //else if (propname == "SavedQuery" && ((string)prop).Contains(":"))
                //    prop = ((string)prop).Split(":".ToCharArray(), 2)[1];

                var attr = s == "Quarters" ? (QuartersTitle ?? propname).Replace(" ", "") : propname;
                switch (attr)
                {
                    case "Program":
                        list.AddParamArg("Prog", prop);
                        break;
                    case "Division":
                        list.AddParamArg("Div", prop);
                        break;
                    case "Organization":
                        list.AddParamArg("Org", prop);
                        break;
                    case "Schedule":
                        list.AddParamArg("Sched", prop);
                        break;
                    case "Tags":
                        var tags = prop.ToString();
                        foreach (var t in tags.Split(';'))
                            list.AddParamArg("Tag", t);
                        break;
                    default:
                        if (prop.ToString().HasValue())
                            list.AddParamArg(attr, prop);
                        break;
                }
            }

            if (list.Any(vv => vv.Name == "Div"))
                list.RemoveAll(vv => vv.Name == "Prog");
            if (list.Any(vv => vv.Name == "Org"))
                list.RemoveAll(vv => vv.Name == "Div");
            var parms = string.Join(", ", list.Select(vv => $"{vv.Name}={vv.Value}"));
            return $"{Name}({parms})";
        }
        internal string ToString(Condition c)
        {
            if (Params.HasValue())
                return FormatArgs(c);
            return Name;
        }

        public bool HasParam(string p)
        {
            return ParamList?.Contains(p) ?? false;
        }
        public static FieldType Convert(string type)
        {
            return (FieldType)Enum.Parse(typeof(FieldType), type);
        }
        public static QueryType ConvertQueryType(string type)
        {
            return (QueryType)Enum.Parse(typeof(QueryType), type);
        }
        public static Dictionary<string, FieldClass2> Fields
        {
            get
            {
                var fields = HttpRuntime.Cache["fields2"] as Dictionary<string, FieldClass2>;
                if (fields == null)
                {
                    var q = from c in CategoryClass2.Categories
                            from f in c.Fields
                            select f;
                    fields = q.ToDictionary(f => f.Name);
                    HttpRuntime.Cache.Insert("fields2", fields, null,
                        DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
                }
                return fields;
            }
        }
        public string Description { get; set; }
    }

    public static class Helper
    {
        public static void AddParamArg(this List<ParamArg> d, string key, object o)
        {
            var s = o.ToString();
            var dt = o as DateTime?;

            if(dt.HasValue)
            {
                d.Add(new ParamArg(key, $"'{dt.FormatDate()}'"));
                return;
            }
            var n = s.GetCsvToken().ToInt2();
            if ((!n.HasValue || n == 0) && o is string)
                return;
            var v = s.GetCsvToken(2, 2);
            d.Add(new ParamArg(key, v.HasValue() ? $"{n}[{v}]" : n.ToString()));
        }
    }
    public class ParamArg
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public ParamArg(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

}
