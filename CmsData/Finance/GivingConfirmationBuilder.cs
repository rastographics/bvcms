using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData.Finance
{
    public class GivingConfirmation
    {
        public class FundItem
        {
            public string Desc { get; set; }
            public int Fundid { get; set; }
            public decimal Amt { get; set; }
        }
        public static string PostAndBuild(CMSDataContext db, Person staff, Person person, string body, int orgId, IEnumerable<FundItem> fundItems, Transaction tran, string desc, int? donationFundId = null)
        {
            var org = db.LoadOrganizationById(orgId);
            var text = body ?? "No Body";
            text = text.Replace("{church}", db.Setting("NameOfChurch", "church"), ignoreCase: true);
            text = text.Replace("{amt}", tran.Amt.ToString2("N2"));
            text = text.Replace("{total}", tran.Amt.ToString2("N2"));
            text = text.Replace("{date}", DateTime.Today.ToShortDateString());
            text = text.Replace("{tranid}", tran.Id.ToString());
            text = text.Replace("{account}", "");
            text = text.Replace("{email}", person.EmailAddress);
            text = text.Replace("{phone}", person.HomePhone.FmtFone());
            text = text.Replace("{contact}", staff.Name);
            text = text.Replace("{contactemail}", staff.EmailAddress);
            text = text.Replace("{contactphone}", org.PhoneNumber.FmtFone());
            var re = new Regex(@"(?<b>.*?)<!--ITEM\sROW\sSTART-->(?<row>.*?)\s*<!--ITEM\sROW\sEND-->(?<e>.*)",
                RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            var match = re.Match(text);
            var b = match.Groups["b"].Value;
            var row = match.Groups["row"].Value.Replace("{funditem}", "{0}").Replace("{itemamt}", "{1:N2}");
            var sb = new StringBuilder();
            var e = match.Groups["e"].Value;
            if(b.HasValue())
                sb.Append(b);

            foreach (var g in fundItems)
            {
                if (g.Amt <= 0)
                    continue;
                if(row.HasValue())
                    sb.AppendFormat(row, g.Desc, g.Amt);
                person.PostUnattendedContribution(db, g.Amt, g.Fundid, desc, tranid: tran.Id);
            }
            tran.TransactionPeople.Add(new TransactionPerson
            {
                PeopleId = person.PeopleId,
                Amt = tran.Amt,
                OrgId = orgId,
            });
            tran.Financeonly = true;
            if (tran.Donate > 0 && donationFundId > 0)
            {
                var fundname = db.ContributionFunds.Single(ff => ff.FundId == donationFundId).FundName;
                if(row.HasValue())
                    sb.AppendFormat(row, fundname, tran.Donate);
                tran.Fund = (from f in db.ContributionFunds where f.FundId == donationFundId select f.FundName).SingleOrDefault();
                person.PostUnattendedContribution(db, tran.Donate ?? 0, donationFundId, desc, tranid: tran.Id);
            }
            if(e.HasValue())
                sb.Append(e);
            if (sb.Length == 0)
                return text;
            return sb.ToString();
        }
    }
}
