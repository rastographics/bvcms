using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private static readonly Regex OrgBarCodeRe = new Regex(@"{orgbarcode(:\s*(?<id>\d*))?}", RegexOptions.Singleline);

        private string OrgBarCodeReplacement(string code, EmailQueueTo emailqueueto)
        {
            var match = OrgBarCodeRe.Match(code);
            var oid = match.Groups["id"]?.Value.ToInt2();
            if(!oid.HasValue)
                oid = emailqueueto.OrgId;
            return $@"<img src='{db.ServerLink($"/Track/Barcode/{oid}-{emailqueueto.PeopleId}")}' width='95%' />";
        }

    }
}
