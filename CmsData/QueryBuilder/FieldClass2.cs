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

        public string Category { get; set; }
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
            foreach (var propname in ParamList)
            {
                var prop = Util.GetProperty(c, propname) ?? "";

                var attr = propname == "Quarters" ? (QuartersTitle ?? propname).Replace(" ", "") : propname;
                var param = (Param)Enum.Parse(typeof (Param), propname);
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (param)
                {
                    case Param.Program:
                        list.AddParamCode("Prog", prop);
                        break;
                    case Param.Division:
                        list.AddParamCode("Div", prop);
                        break;
                    case Param.Organization:
                        list.AddParamCode("Org", prop);
                        break;
                    case Param.Schedule:
                        list.AddParamCode("Sched", prop);
                        break;
                    case Param.SavedQueryIdDesc:
                        list.AddParamCode("SavedQuery", prop);
                        break;
                    case Param.Tags:
                    case Param.PmmLabels:
                        var tags = prop.ToString();
                        foreach (var t in tags.Split(';'))
                            list.AddParamCode("Tag", t);
                        break;
                    case Param.OnlineReg:
                        list.AddParamCode(attr, prop, -1);
                        break;
                    case Param.OrgStatus:
                    case Param.Campus:
                    case Param.OrgType:
                    case Param.OrgType2:
                    case Param.Ministry:
                        list.AddParamCode(attr, prop);
                        break;
                    case Param.OrgName:
                    case Param.Quarters:
                        list.AddParamStr(attr, prop);
                        break;
                    case Param.Days:
                    case Param.Age:
                        list.AddParamInt(attr, prop);
                        break;
                    case Param.StartDate:
                    case Param.EndDate:
                        list.AddParamDate(attr, prop);
                        break;
                }
            }
            if (list.Any(vv => vv.Name == "Div"))
                list.RemoveAll(vv => vv.Name == "Prog");
            if (list.Any(vv => vv.Name == "Org"))
                list.RemoveAll(vv => vv.Name == "Div");
            var parms = string.Join(", ", list.Select(vv => $"{vv.Name}={vv.Value}"));
            return $"{Name}( {parms} )";
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
        public static void AddParamCode(this List<ParamArg> d, string key, object o, int skip = 0)
        {
            var s = o.ToString();
            if (!s.HasValue())
                return;
            var n = s.GetCsvToken().ToInt2();
            if (n == skip)
                return;
            var v = s.GetCsvToken(2, 2);
            d.Add(new ParamArg(key, v.HasValue() ? $"{n}[{v}]" : n.ToString()));
        }
        public static void AddParamStr(this List<ParamArg> d, string key, object o)
        {
            var s = o.ToString();
            if (s.HasValue())
            d.Add(new ParamArg(key, $"'{s}'"));
        }
        public static void AddParamDate(this List<ParamArg> d, string key, object o)
        {
            var dt = o as DateTime?;
            if (dt.HasValue)
                d.Add(new ParamArg(key, $"'{dt.FormatDate()}'"));
        }
        public static void AddParamInt(this List<ParamArg> d, string key, object o)
        {
            var s = o.ToString();
            if (!s.HasValue())
                return;
            var i = o as int?;
            if(i.HasValue)
                d.Add(new ParamArg(key, $"{i}"));
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

    internal enum Param
    {
        Program,
        Division,
        Organization,
        StartDate,
        EndDate,
        Quarters,
        Age,
        Days,
        Ministry,
        OrgName,
        OrgStatus,
        OnlineReg,
        OrgType2,
        Schedule,
        Campus,
        OrgType,
        PmmLabels,
        Tags,
        Tag,
        SavedQueryIdDesc
    }
}
