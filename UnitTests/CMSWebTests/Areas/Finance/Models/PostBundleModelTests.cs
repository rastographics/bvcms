using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CMSWebTests;
using SharedTestFixtures;
using CMSWebTests.Support;
using Shouldly;
using CmsWeb.Areas.Manage.Models;
using CmsWeb.Membership;
using CmsData;
using CmsData.Codes;
using System.Data.Linq;

namespace CmsWeb.ModelsTests
{
    [Collection(Collections.Database)]
    public class PostBundleModelTests : ControllerTestBase
    {
        [Fact]
        public void DeleteContributionWithTagsTest()
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var bundle = new BundleHeader
                {
                    ChurchId = 1,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now,
                    RecordStatus = false,
                    BundleStatusId = BundleStatusCode.OpenForDataEntry,
                    ContributionDate = DateTime.Now,
                    BundleHeaderTypeId = BundleTypeCode.Online,
                    DepositDate = null,
                    BundleTotal = 0,
                    TotalCash = 0,
                    TotalChecks = 0
                };

                db.BundleHeaders.InsertOnSubmit(bundle);
                db.SubmitChanges();

                var contribution = new Contribution
                {
                    PeopleId = 1,
                    ContributionDate = DateTime.Now,
                    ContributionAmount = 50,
                    ContributionTypeId = ContributionTypeCode.Online,
                    ContributionStatusId = ContributionStatusCode.Recorded,
                    CreatedDate = DateTime.Now,
                    FundId = 1
                };

                db.Contributions.InsertOnSubmit(contribution);
                db.SubmitChanges();

                BundleDetail bd = new BundleDetail
                {
                    BundleHeaderId = bundle.BundleHeaderId,
                    ContributionId = contribution.ContributionId,
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now
                };
                db.BundleDetails.InsertOnSubmit(bd);
                bundle.BundleDetails.Add(bd);

                var tag = new ContributionTag
                {
                    ContributionId = contribution.ContributionId,
                    TagName = "Tag Test"
                };

                db.ContributionTags.InsertOnSubmit(tag);
                db.SubmitChanges();

                var model = new PostBundleModel(db, bundle.BundleHeaderId);
                model.editid = contribution.ContributionId;
                model.DeleteContribution();

                db.ContributionTags.SingleOrDefault(t => t.ContributionId == contribution.ContributionId).ShouldBeNull();
                db.Contributions.SingleOrDefault(c => c.ContributionId == contribution.ContributionId).ShouldBeNull();

                // cleanup
                db.BundleDetails.DeleteOnSubmit(bd);
                db.BundleHeaders.DeleteOnSubmit(bundle);
                db.SubmitChanges();
            }
        }
    }
}
