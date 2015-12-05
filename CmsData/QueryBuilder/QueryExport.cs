using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
                return ExportExpressionList(c);
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
                default:
                    throw new ArgumentException();
            }
            var inner = string.Join(andOrNot, list);
            return $"{Level}{not}{inner}";
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

        public class ConditionConfig
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string DataValueField { get; set; }
            public string DataSource { get; set; }
            public string QuartersLabel { get; set; }
            public string Category { get; set; }
            public string Title { get; set; }
            public string Params { get; set; }
            public string Description { get; set; }
        }

        public static IEnumerable<ConditionConfig> ConditionConfigs()
        {
            var xdoc = XDocument.Parse(Properties.Resources.FieldMap3);
            var q = from c in xdoc.Root?.Elements()
                    from f in c.Elements()
                    select new ConditionConfig
                    {
                        Name = f.Name.LocalName,
                        Type = f.Attribute("Type")?.Value,
                        DataValueField = f.Attribute("DataValueField")?.Value,
                        DataSource = f.Attribute("DataSource")?.Value,
                        Category = c.Name.LocalName,
                        Title = f.Attribute("Title")?.Value,
                        Params = f.Attribute("Params")?.Value,
                        QuartersLabel = f.Attribute("QuartersLabel")?.Value,
                        Description = f.Value.Trim(),
                    };
            return q;
        }
    }
}