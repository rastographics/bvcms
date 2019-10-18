using CmsData.Codes;
using System;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class Contribution
    {
        public static BundleHeader GetBundleHeader(CMSDataContext db, DateTime date, DateTime now, int? btid = null)
        {
            var opentype = db.Roles.Any(rr => rr.RoleName == "FinanceDataEntry")
                ? BundleStatusCode.OpenForDataEntry
                : BundleStatusCode.Open;
            var bh = new BundleHeader
            {
                BundleHeaderTypeId = BundleTypeCode.PreprintedEnvelope,
                BundleStatusId = opentype,
                ContributionDate = date,
                CreatedBy = Util.UserId,
                CreatedDate = now,
                FundId = db.Setting("DefaultFundId", "1").ToInt()
            };
            db.BundleHeaders.InsertOnSubmit(bh);
            bh.BundleHeaderTypeId = btid ?? BundleTypeCode.ChecksAndCash;
            return bh;
        }
        public static void FinishBundle(CMSDataContext db, BundleHeader bh)
        {
            bh.TotalChecks = bh.BundleDetails.Sum(d => d.Contribution.ContributionAmount);
            bh.TotalCash = 0;
            bh.TotalEnvelopes = 0;
            db.SubmitChanges();
        }

        public static BundleDetail AddContribution(CMSDataContext db, DateTime date, int fundid, string amount, string checkno, string description, int peopleid, int? contributionTypeId = null)
        {
            var bd = NewBundleDetail(db, date, fundid, amount, contributionTypeId);
            bd.Contribution.CheckNo = checkno;
            if (!contributionTypeId.HasValue && checkno.HasValue() && bd.Contribution.ContributionTypeId == ContributionTypeCode.Reversed)
            {
                bd.Contribution.ContributionTypeId = ContributionTypeCode.ReturnedCheck;
            }
            bd.Contribution.ContributionDesc = description;
            if (peopleid > 0)
            {
                bd.Contribution.PeopleId = peopleid;
            }
            return bd;
        }

        public static BundleDetail AddContributionDetail(CMSDataContext db, DateTime date, int fundid,
            string amount, string checkno, string routing, string account, int? contributionTypeId = null)
        {
            var bd = NewBundleDetail(db, date, fundid, amount, contributionTypeId);
            bd.Contribution.CheckNo = checkno;
            int? pid = null;
            if (account.HasValue() && !account.Contains("E+"))
            {
                var eac = Util.Encrypt($"{routing}|{account}");
                var q = from kc in db.CardIdentifiers
                        where kc.Id == eac
                        select kc.PeopleId;
                pid = q.SingleOrDefault();
                bd.Contribution.BankAccount = eac;
            }
            if (pid > 0)
                bd.Contribution.PeopleId = pid;
            return bd;
        }

        public static BundleDetail NewBundleDetail(CMSDataContext db, DateTime date, int fundid, string amount, int? contributionTypeId)
        {
            var bd = new BundleDetail
            {
                CreatedBy = Util.UserId,
                CreatedDate = DateTime.Now
            };
            var value = amount.GetAmount();
            bd.Contribution = new Contribution
            {
                CreatedBy = Util.UserId,
                CreatedDate = DateTime.Now,
                ContributionDate = date,
                FundId = fundid,
                ContributionStatusId = 0,
                ContributionTypeId = contributionTypeId.GetValueOrDefault(value > 0 ? ContributionTypeCode.CheckCash : ContributionTypeCode.Reversed),
                ContributionAmount = value
            };
            return bd;
        }
        public static int FirstFundId(CMSDataContext db)
        {
            var firstfund = (from f in db.ContributionFunds
                             where f.FundStatusId == 1
                             orderby f.FundId
                             select f.FundId).First();
            return firstfund;
        }
    }
}
