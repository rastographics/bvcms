using CmsData;
using CmsData.Codes;
using CmsDataTests;
using CmsWeb.Areas.OnlineReg.Models;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace CMSWebTests.Areas.OnlineReg.Models
{
    [Collection(Collections.Database)]
    public class OnlineRegModelTest : DatabaseTestBase
    {
        [Fact]
        public void ShouldProcessZeroLimitOrgsAsFilled()
        {
            var requestManager = FakeRequestManager.Create();
            var orgRegLimitConfig = new Organization()
            {
                OrganizationName = "MockMasterName",
                RegistrationTitle = "MockMasterTitle",
                Location = "MockLocation",
                RegistrationTypeId = RegistrationTypeCode.JoinOrganization,
                Limit = 0
            };
            var fakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, orgRegLimitConfig);
            OnlineRegModel om = FakeOrganizationUtils.GetFakeOnlineRegModel((int)fakeOrg.org.OrganizationId);
            om.Filled().ShouldBe("registration is full");
            FakeOrganizationUtils.DeleteOrg(fakeOrg.org.OrganizationId);
        }

        [Fact]
        public void ShouldDetectDuplicateGift()
        {
            var requestManager = FakeRequestManager.Create();
            var testOrg = new Organization()
            {
                OrganizationName = "MockMasterName",
                RegistrationTitle = "MockMasterTitle",
                Location = "MockLocation",
                RegistrationTypeId = RegistrationTypeCode.JoinOrganization,
                Limit = 0
            };
            var org = FakeOrganizationUtils.MakeFakeOrganization(requestManager, testOrg);

            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                Transaction t = new Transaction()
                {
                    TransactionDate = DateTime.Now,
                    First = "Chester",
                    Last = "Tester",
                    Amt = 30,
                    Testing = true,
                    TransactionGateway = "Sage",
                    OrgId = org.org.OrganizationId
                };
                db.Transactions.InsertOnSubmit(t);
                db.SubmitChanges();
            }

            OnlineRegPersonModel person = new OnlineRegPersonModel(db) {
                FirstName = "Chester",
                LastName = "Tester"
            };

            OnlineRegModel om = FakeOrganizationUtils.GetFakeOnlineRegModel(org.org.OrganizationId);
            om.List = new List<OnlineRegPersonModel>() { person };
            om.CheckDuplicateGift(30).ShouldNotBeNull();
            FakeOrganizationUtils.DeleteOrg(org.org.OrganizationId);
        }
    }
}
