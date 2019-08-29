using CmsData;
using CmsData.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmsDataTests.Support
{
    public class FinanceTestBase
    {
        protected BundleHeader CreateBundle(CMSDataContext db, DateTime? contributionDate = null)
        {
            var bundleHeader = new BundleHeader
            {
                BundleHeaderTypeId = BundleTypeCode.ChecksAndCash,
                BundleStatusId = BundleStatusCode.Open,
                ContributionDate = contributionDate ?? DateTime.Now,
                CreatedDate = contributionDate ?? DateTime.Now,
                CreatedBy = 1,
            };
            db.BundleHeaders.InsertOnSubmit(bundleHeader);
            db.SubmitChanges();

            return bundleHeader;
        }

        protected Contribution CreateContribution(CMSDataContext db, BundleHeader bundleHeader,
            DateTime date,
            decimal amount,
            int? peopleId = null,
            int contributionType = ContributionTypeCode.CheckCash,
            int fundId = 1,
            int statusId = ContributionStatusCode.Recorded)
        {
            var contribution = new Contribution
            {
                ContributionAmount = amount,
                ContributionDate = date,
                ContributionStatusId = statusId,
                ContributionTypeId = contributionType,
                CreatedDate = date,
                PledgeFlag = contributionType == ContributionTypeCode.Pledge,
                FundId = fundId,
                PeopleId = peopleId,
            };
            db.Contributions.InsertOnSubmit(contribution);
            db.SubmitChanges();

            db.BundleDetails.InsertOnSubmit(new BundleDetail
            {
                BundleHeader = bundleHeader,
                Contribution = contribution,
                CreatedDate = DateTime.Now,
            });
            db.SubmitChanges();

            return contribution;
        }
    }
}
