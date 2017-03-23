using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private const string MatchExtraValueRe = @"{extra(?<type>.*?):(?<field>.*?)}";
        private static readonly Regex ExtraValueRe = new Regex(MatchExtraValueRe, RegexOptions.Singleline);

        private string ExtraValueReplacement(string code, EmailQueueTo emailqueueto)
        {
            var match = ExtraValueRe.Match(code);
            var field = match.Groups["field"].Value;
            var type = match.Groups["type"].Value;
            var ev = db.PeopleExtras.SingleOrDefault(ee => ee.Field == field && emailqueueto.PeopleId == ee.PeopleId);
            if (ev == null)
                return "";

            switch (type)
            {
                case "value":
                case "code":
                    return ev.StrValue;
                case "data":
                case "text":
                    return ev.Data;
                case "date":
                    return ev.DateValue.FormatDate();
                case "int":
                    return ev.IntValue.ToString();
                case "bit":
                case "bool":
                    return ev.BitValue.ToString();
            }
            return code;
        }
    }
}
