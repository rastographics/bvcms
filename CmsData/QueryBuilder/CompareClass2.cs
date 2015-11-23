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
using System.Xml.Linq;
using System.Web;
using System.Web.Caching;

namespace CmsData
{
    public class CompareClass2
    {
        public FieldType FieldType { get; set; }
        public CompareType CompType { get; set; }
        public string Display { get; set; }
        internal string ToString(Condition c)
        {
            string fld = c.FieldInfo.ToString(c);
            switch (FieldType)
            {
                case FieldType.EqualBit:
                    return fld;
                case FieldType.NullBit:
                case FieldType.Bit:
                case FieldType.Code:
                case FieldType.NullCode:
                case FieldType.CodeStr:
                    return string.Format(Display, fld, c.CodeIdText);
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
                    return string.Format(Display, fld, c.TextValue);
                case FieldType.Date:
                case FieldType.DateSimple:
                    return string.Format(Display, fld, c.DateValue);
                case FieldType.DateField:
                    return string.Format(Display, fld, c.CodeIdValue);
                default:
                    throw new ArgumentException();
            }
        }
        public static CompareType Convert(string type)
        {
            if (!type.HasValue())
                return CompareType.Equal;
            return (CompareType)Enum.Parse(typeof(CompareType), type);
        }
        public static List<CompareClass2> Comparisons
        {
            get
            {
                var comparisons = (List<CompareClass2>)HttpRuntime.Cache["comparisons2"];
                if (comparisons == null)
                {
                    var xdoc = XDocument.Parse(Properties.Resources.CompareMap);
                    var q = from f in xdoc.Descendants("FieldType")
                            from c in f.Elements("Comparison")
                            select new CompareClass2
                            {
                                FieldType = FieldClass2.Convert((string)f.Attribute("Name")),
                                CompType = Convert((string)c.Attribute("Type")),
                                Display = (string)c.Attribute("Display")
                            };
                    comparisons = q.ToList();
                    HttpRuntime.Cache.Insert("comparisons2", comparisons, null,
                        DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
                }
                return comparisons;
            }
        }
    }
}
