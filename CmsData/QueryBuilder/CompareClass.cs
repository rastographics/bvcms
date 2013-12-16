/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class CompareClass
    {
        public FieldType FieldType { get; set; }
        public CompareType CompType { get; set; }
        public string Display { get; set; }
        internal string ToString(QueryBuilderClause c)
        {
            string fld = c.FieldInfo.Display(c);
            if (c.Field == "MatchAnything")
                return fld;
            switch (FieldType)
            {
                case FieldType.NullBit:
                case FieldType.Bit:
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                    return Display.Fmt(fld, c.CodeValues);
                case FieldType.String:
                case FieldType.StringEqual:
                case FieldType.StringEqualOrStartsWith:
                case FieldType.Number:
                case FieldType.NumberLG:
                case FieldType.NullNumber:
                case FieldType.Integer:
                case FieldType.IntegerSimple:
                case FieldType.IntegerEqual:
                case FieldType.NullInteger:
                    return Display.Fmt(fld, c.TextValue);
                case FieldType.Date:
                case FieldType.DateSimple:
                    return Display.Fmt(fld, c.DateValue);
                case FieldType.DateField:
                    return Display.Fmt(fld, c.CodeIdValue);
                default:
                    throw new ArgumentException();
            }
        }
        internal Expression Expression(QueryBuilderClause qbc, ParameterExpression parm, CMSDataContext Db)
        {
            var c = new Condition()
            {
                Age = qbc.Age,
                Campus = qbc.Campus,
                Comparison = qbc.Comparison,
                DateValue = qbc.DateValue,
                CodeIdValue = qbc.CodeIdValue,
                Days = qbc.Days,
                Division = qbc.Division,
                EndDate = qbc.EndDate,
                ConditionName = qbc.Field,
                Organization = qbc.Organization,
                OrgType = qbc.OrgType,
                Program = qbc.Program,
                Quarters = qbc.Quarters,
                Schedule = qbc.Schedule,
                StartDate = qbc.StartDate,
                Tags = qbc.Tags,
                TextValue = qbc.TextValue,
                SavedQuery = qbc.SavedQueryIdDesc
            };
            return c.GetExpression(parm, Db, qbc.SetIncludeDeceased, qbc.SetParentsOf);
        }
        public static CompareType Convert(string type)
        {
            if (type == null)
                return CompareType.Equal;
            return (CompareType)Enum.Parse(typeof(CompareType), type);
        }
        public static List<CompareClass> Comparisons
        {
            get
            {
                var _Comparisons = (List<CompareClass>)HttpRuntime.Cache["comparisons"];
                if (_Comparisons == null)
                {
                    var xdoc = XDocument.Parse(Properties.Resources.CompareMap);
                    var q = from f in xdoc.Descendants("FieldType")
                            from c in f.Elements("Comparison")
                            select new CompareClass
                            {
                                FieldType = FieldClass.Convert((string)f.Attribute("Name")),
                                CompType = Convert((string)c.Attribute("Type")),
                                Display = (string)c.Attribute("Display")
                            };
                    _Comparisons = q.ToList();
                    HttpRuntime.Cache.Insert("comparisons", _Comparisons, null,
                        DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
                }
                return _Comparisons;
            }
        }
    }
}
