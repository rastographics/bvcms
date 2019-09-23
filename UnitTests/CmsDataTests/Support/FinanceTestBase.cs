using CmsData;
using CmsData.Codes;
using System.Linq;
using UtilityExtensions;

namespace CmsDataTests.Support
{
    public class FinanceTestBase
    {
        protected void DeleteContribution(CMSDataContext db, int contributionId)
        {
            db.ExecuteCommand("DELETE FROM [Contribution] WHERE [ContributionId] = {0}", contributionId);
            db.SubmitChanges();
        }

        protected void DeleteBundleDetail(CMSDataContext db, int bundleHeaderId, int contributionId)
        {
            db.ExecuteCommand("DELETE FROM [BundleDetail] WHERE [BundleHeaderId] = {0} AND [ContributionId] = {1}", bundleHeaderId, contributionId);
            db.SubmitChanges();
        }

        protected void DeleteBundleHeader(CMSDataContext db, int bundleHeaderId)
        {
            db.ExecuteCommand("DELETE FROM [BundleHeader] WHERE [BundleHeaderId] = {0}", bundleHeaderId);
            db.SubmitChanges();
        }

        protected static Contribution CreateContributionRecord(Contribution c)
        {
            var now = Util.Now;
            var r = new Contribution
            {
                ContributionStatusId = ContributionStatusCode.Recorded,
                CreatedBy = Util.UserId1,
                CreatedDate = now,
                PeopleId = c.PeopleId,
                ContributionAmount = c.ContributionAmount,
                ContributionDate = now.Date,
                PostingDate = now,
                FundId = c.FundId,
            };
            return r;
        }

        protected void ReturnContribution(CMSDataContext db, int cid)
        {
            var c = db.Contributions.Single(ic => ic.ContributionId == cid);
            var r = CreateContributionRecord(c);
            c.ContributionStatusId = ContributionStatusCode.Returned;
            r.ContributionTypeId = ContributionTypeCode.ReturnedCheck;
            r.ContributionDesc = "Returned Check for Contribution Id = " + c.ContributionId;

            db.Contributions.InsertOnSubmit(r);
            db.SubmitChanges();
        }

        protected void ReverseContribution(CMSDataContext db, int cid)
        {
            var c = db.Contributions.Single(ic => ic.ContributionId == cid);
            var r = CreateContributionRecord(c);
            c.ContributionStatusId = ContributionStatusCode.Reversed;
            r.ContributionTypeId = ContributionTypeCode.Reversed;
            r.ContributionDesc = "Reversed Contribution Id = " + c.ContributionId;
            db.Contributions.InsertOnSubmit(r);
            db.SubmitChanges();
        }
    }
}
