using DbmlBuilder.TableSchema;
using System.Collections.Generic;

namespace DbmlBuilder.CodeGeneration
{
    internal class ColumnAttributes
    {
        const string WhenChanged = "WhenChanged";
        const string Always = "Always";
        const string Never = "Never";

        static Dictionary<string, string> concurrencyRules = new Dictionary<string, string>
        {
            ["SMSItem.ResultStatus"] = WhenChanged,
            ["SMSItem.ErrorMessage"] = WhenChanged,
        };

        public static string GetConcurrencyUpdateCheckRule(TableColumn column)
        {
            string key = $"{column.Table.ClassName}.{column.ColumnName}";
            return concurrencyRules.ContainsKey(key) ? concurrencyRules[key] : Never;
        }
    }
}
