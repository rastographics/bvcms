using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UtilityExtensions;

namespace CmsData
{
    public class ExportQueryModel
    {
        private int level;
        private string Level => new string('\t', level);

        public string ToString(Condition c)
        {
            var s = ExportExpressionList(c);
            if (!s.HasValue())
                s = "MatchAnything = 1[True]";
            return s;
        }
        private string ExportExpressionList(Condition c)
        {
            if (!c.IsGroup)
                return c.ToString();
            if (!c.Conditions.Any())
                return null;
            var list = c.Conditions.Select(ExportExpression).ToList();
            string andOrNot;
            string not = "";
            switch (c.ComparisonType)
            {
                case CompareType.AllTrue:
                    andOrNot = $"\n{Level}AND ";
                    break;
                case CompareType.AnyTrue:
                    andOrNot = $"\n{Level}OR ";
                    break;
                case CompareType.AllFalse:
                    andOrNot = $"\n{Level}AND NOT ";
                    not = "NOT ";
                    break;
                case CompareType.AnyFalse:
                    andOrNot = $"\n{Level}OR NOT ";
                    not = "NOT ";
                    break;
                default:
                    throw new ArgumentException();
            }
            var inner = string.Join(andOrNot, list.Where(vv => vv.HasCode()));
            return $"{Level}{not}{inner}";
        }

        private string ExportExpression(Condition c)
        {
            QueryType qt;
            if (!Enum.TryParse(c.ConditionName, out qt))
                return null;

            level++;
            var inner = ExportExpressionList(c);
            level--;
            if (!c.IsGroup)
                return inner;
            if (inner == null)
                return null;
            return $"\n{Level}(\n{inner}\n{Level})";
        }
    }

    public static class ExportQuery
    {
        public static string ToCode(this Condition c)
        {
            var m = new ExportQueryModel();
            return m.ToString(c);
        }
        public static string ToSql(this Condition c, CMSDataContext db)
        {
            var qq = db.PeopleQueryCondition(c);
            return db.GetWhereClause(qq);
        }

        public static bool HasCode(this string s)
        {
            if (!s.HasValue())
                return false;
            var code = Regex.Replace(s, @"\s", "");
            if (code == "()")
                return false;
            return true;
        }
    }
}
