using CmsData;
using CmsData.Codes;
using CmsData.View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedTestFixtures
{
    public class MockContributions
    {
        public static List<PledgesSummary> CreatePledgesSummary() =>
            new List<PledgesSummary>
            {
                new PledgesSummary
                {
                    FundId = 1,
                    FundName = "General Operation",
                    AmountContributed = 100,
                    AmountPledged = 200,
                    Balance = 100
                },
                new PledgesSummary
                {
                    FundId = 2,
                    FundName = "Pledge",
                    AmountContributed = 100,
                    AmountPledged = 200,
                    Balance = 100
                }
            };

        public static List<PledgesSummary> FilteredPledgesSummary() =>
            new List<PledgesSummary>
            {
                new PledgesSummary
                {
                    FundId = 1,
                    FundName = "General Operation",
                    AmountContributed = 100,
                    AmountPledged = 200,
                    Balance = 100
                }
            };

        public static BundleHeader CreateSaveBundle(CMSDataContext db, DateTime? contributionDate = null)
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

        public static Contribution CreateSaveContribution(CMSDataContext db, BundleHeader bundleHeader,
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

        public static void DeleteAllFromBundle(CMSDataContext db, BundleHeader bundleHeader)
        {
            try
            {
                var bundleDetails = db.BundleDetails.Where(b => b.BundleHeaderId == bundleHeader.BundleHeaderId);
                foreach (var item in bundleDetails)
                {
                    var contribution = db.Contributions.SingleOrDefault(c => c.ContributionId == item.ContributionId);
                    db.Contributions.DeleteOnSubmit(contribution);
                }
                db.BundleDetails.DeleteAllOnSubmit(bundleDetails);
                db.BundleHeaders.DeleteOnSubmit(bundleHeader);
                db.SubmitChanges();
            }
            catch { }
        }
    }
}
