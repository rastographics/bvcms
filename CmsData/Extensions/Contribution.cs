using CmsData.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class Contribution
    {
        public static BundleHeader GetBundleHeader(
            CMSDataContext db,
            DateTime date,
            DateTime now,
            int? btid = null,
            DateTime? depositDate = null,
            decimal? bundleTotal = null)
        {
            var opentype = db.Roles.Any(rr => rr.RoleName == "FinanceDataEntry")
                ? BundleStatusCode.OpenForDataEntry
                : BundleStatusCode.Open;
            var bh = new BundleHeader
            {
                BundleHeaderTypeId = BundleTypeCode.PreprintedEnvelope,
                BundleStatusId = opentype,
                ContributionDate = date,
                CreatedBy = db.UserId,
                CreatedDate = now,
                FundId = db.Setting("DefaultFundId", "1").ToInt(),
                DepositDate = depositDate,
                BundleTotal = bundleTotal
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
            string amount, string checkno, string routing, string account, int? contributionTypeId = null, int? pid = null, string description = null)
        {
            string eac = null;
            var bd = NewBundleDetail(db, date, fundid, amount, contributionTypeId);
            bd.Contribution.CheckNo = checkno;

            if (account.HasValue() && !account.Contains("E+"))
            {
                eac = Util.Encrypt($"{routing}|{account}");                               
                bd.Contribution.BankAccount = eac;
            }

            if (pid == null && eac != null)
            {
                var q = from kc in db.CardIdentifiers
                        where kc.Id == eac
                        select kc.PeopleId;
                pid = q.SingleOrDefault();
            }

            if (pid > 0)
                bd.Contribution.PeopleId = pid;

            if (!string.IsNullOrEmpty(description))
                bd.Contribution.ContributionDesc = description;

            return bd;
        }

        public static BundleDetail NewBundleDetail(CMSDataContext db, DateTime date, int fundid, string amount, int? contributionTypeId)
        {
            var bd = new BundleDetail
            {
                CreatedBy = db.UserId,
                CreatedDate = DateTime.Now
            };
            var value = amount.GetAmount();
            bd.Contribution = new Contribution
            {
                CreatedBy = db.UserId,
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

        public static List<Contribution> GetContributions(CMSDataContext db, int[] fundids = null, bool includeReturnedReversed = false, bool includePledges = false)
        {
            var fundFlag = fundids.IsNull();
            var q = (
                from c in db.Contributions
                where (!includeReturnedReversed ? !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId) : 1 == 1)
                && (!includeReturnedReversed ? c.ContributionStatusId != ContributionStatusCode.Returned : 1 == 1)
                && (!includeReturnedReversed ? c.ContributionStatusId != ContributionStatusCode.Reversed : 1 == 1)
                && (!includePledges ? c.ContributionTypeId != ContributionTypeCode.Pledge : 1 == 1)
                && (c.ContributionDate != null)

                select c
                   );
            return (fundids.IsNotNull()) ? q.Where(x => fundids.Contains(x.FundId)).ToList() : q.ToList();
        }

        public static List<Contribution> GetContributionsPerYear(CMSDataContext db, int? year, int[] fundids = null, bool includeReturnedReversed = false, bool includePledges = false)
        {
            var yearFlag = year.IsNull();
            return (from c in Contribution.GetContributions(db, fundids, includeReturnedReversed, includePledges)
                    where (!yearFlag ? c.ContributionDate.Value.Year == (year) : 1 == 1)
                    select c).ToList();
        }
    }
}
