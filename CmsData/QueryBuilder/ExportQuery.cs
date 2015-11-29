using System;
using System.Linq;

namespace CmsData
{
    public class ExportQueryModel
    {
        private int level;
        private string Level => new string('\t', level);

        public string ToString(Condition c)
        {
            try
            {
                var inner = ExportExpressionList(c);
                return c.ComparisonType == CompareType.AllFalse ? $"NOT(\n{Level}{inner}\n)" : $"{Level}{inner}";
            }
            catch (Exception)
            {
                return null;
            }
        }
        private string ExportExpressionList(Condition c)
        {
            if (!c.IsGroup)
                return c.ToString();
            if (!c.Conditions.Any())
                return null;
            var list = c.Conditions.Select(ExportExpression).ToList();
            var andor = c.ComparisonType == CompareType.AnyTrue ? $"\n{Level}OR " : $"\n{Level}AND ";
            var inner = string.Join(andor, list);
            return $"{Level}{inner}";
        }
        private string ExportExpression(Condition c)
        {
            level++;
            var inner = ExportExpressionList(c);
            level--;
            if (!c.IsGroup)
                return inner;
            if (inner == null)
                return null;
            var not = c.ComparisonType == CompareType.AllFalse ? "NOT" : "";
            return $"\n{Level}{not}(\n{inner}\n{Level})";
        }
    }

    public static class ExportQuery
    {
        public static string ToCode(this Condition c)
        {
            var m = new ExportQueryModel();
            return m.ToString(c);
        }
    }
}