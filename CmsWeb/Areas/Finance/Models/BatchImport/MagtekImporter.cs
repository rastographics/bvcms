using CmsData;
using CmsData.Codes;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Models.BatchImport
{
    internal class MagtekImporter : IContributionBatchImporter
    {
        public int? RunImport(string text, DateTime date, int? fundid, bool fromFile)
        {
            return BatchProcessMagTek(text, date);
        }

        private static int? BatchProcessMagTek(string lines, DateTime date)
        {
            var db = DbUtil.Db;
            var userId = db.UserId;
            var now = DateTime.Now;
            var bh = new BundleHeader
            {
                BundleHeaderTypeId = BundleTypeCode.ChecksAndCash,
                BundleStatusId = BundleStatusCode.Open,
                ContributionDate = date,
                CreatedBy = userId,
                CreatedDate = now,
                FundId = db.Setting("DefaultFundId", "1").ToInt()
            };
            db.BundleHeaders.InsertOnSubmit(bh);

            var re = new Regex(
@"(T(?<rt>[\d?]+)T(?<ac>[\d ?]*)U\s*(?<ck>[\d?]+))|
(CT(?<rt>[\d?]+)A(?<ac>[\d ?]*)C(?<ck>[\d?]+)M)",
                RegexOptions.IgnoreCase);
            var m = re.Match(lines);
            while (m.Success)
            {
                var rt = m.Groups["rt"].Value;
                var ac = m.Groups["ac"].Value;
                var ck = m.Groups["ck"].Value;
                var bd = new BundleDetail
                {
                    CreatedBy = userId,
                    CreatedDate = now,
                };
                bh.BundleDetails.Add(bd);
                var qf = from f in db.ContributionFunds
                         where f.FundStatusId == 1
                         orderby f.FundId
                         select f.FundId;

                bd.Contribution = new Contribution
                {
                    CreatedBy = userId,
                    CreatedDate = now,
                    ContributionDate = date,
                    FundId = qf.First(),
                    ContributionStatusId = 0,
                    ContributionTypeId = ContributionTypeCode.CheckCash,
                };
                bd.Contribution.ContributionDesc = ck;
                var eac = Util.Encrypt(rt + "," + ac);
                var q = from kc in db.CardIdentifiers
                        where kc.Id == eac
                        select kc.PeopleId;
                var pid = q.SingleOrDefault();
                if (pid != null)
                {
                    bd.Contribution.PeopleId = pid;
                }

                bd.Contribution.BankAccount = eac;
                bd.Contribution.ContributionDesc = ck;

                m = m.NextMatch();
            }
            bh.TotalChecks = 0;
            bh.TotalCash = 0;
            bh.TotalEnvelopes = 0;
            db.SubmitChanges();
            return bh.BundleHeaderId;
        }
    }
}
