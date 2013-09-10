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
using System.Text;
using System.Xml.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Caching;

namespace CmsData
{

    public class FieldClass2
    {
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                _QueryType = ConvertQueryType(value);
            }
        }
        private QueryType _QueryType;
        public QueryType QueryType
        {
            get
            {
                return _QueryType;
            }
            set
            {
                _QueryType = value;
                Name = value.ToString();
            }
        }
        public string CategoryTitle { get; set; }
        public string QuartersTitle { get; set; }
        private string _Title;
        public string Title
        {
            get { return _Title.HasValue() ? _Title : Name; }
            set { _Title = value; }
        }
        public FieldType Type { get; set; }
        public string DisplayAs { get; set; }
        private string _Params;
        public string Params
        {
            get { return _Params; }
            set
            {
                _Params = value;
                if (value.HasValue())
                    ParamList = value.SplitStr(",").ToList();
            }
        }
        public List<string> ParamList { get; set; }
        public string DataSource { get; set; }
        public string DataValueField { get; set; }
        private string formatArgs(string fmt, QueryBuilderClause2 c)
        {
            var p = new List<object>();
            foreach (var s in ParamList)
            {
                var s2 = s;
                if (s2 == "Week")
                    s2 = "Quarters";
                else if (s2 == "Ministry")
                    s2 = "Program";
                else if (s2 == "View")
                    s2 = "Quarters";
                else if (s2 == "PmmLabels")
                    s2 = "Tags";
                object prop = Util.GetProperty(c, s2);
                if (prop is DateTime?)
                    prop = ((DateTime?) prop).FormatDate();
                if (s == "SavedQueryValue")
                    prop = ((string)prop).Split(',')[1];
                p.Add(prop);
            }
            return fmt.Fmt(p.ToArray());
        }
        internal string Display(QueryBuilderClause2 c)
        {
            if (DisplayAs.HasValue() && Params.HasValue())
                return formatArgs(DisplayAs, c);
            return Util.PickFirst(DisplayAs, Name);
        }
        public bool HasParam(string p)
        {
            return ParamList == null ? false : ParamList.Contains(p);
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
}
