using CmsData;
using CmsData.Codes;
using CMSWebTests;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Areas.OnlineReg.Models.OnlineReg
{
    [Collection(Collections.Webapp)]
    public class EnrollAndConfirmTests : AccountTestBase
    {
        private new CMSDataContext db = null;
        private int OrgId { get; set; }
        private string EmailAddress { get; set; }

        [Fact]
        public void Should_SendAllConfirmations_ToRegistrants()
        {
            db = CMSDataContext.Create(DatabaseFixture.Host);
            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.OnlineReg.Controllers.OnlineRegController(requestManager);
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);

            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, new CmsData.Organization()
            {
                OrganizationName = RandomString(),
                RegistrationTitle = RandomString(),
                Location = RandomString(),
                RegistrationTypeId = RegistrationTypeCode.JoinOrganization,
                NotifyIds = "1"
            });

            OrgId = FakeOrg.org.OrganizationId;

            username = RandomString();
            password = RandomString();
            var CurrentUser = CreateUser(username, password);
            EmailAddress = CurrentUser.EmailAddress;
            Login();

            Open($"{rootUrl}OnlineReg/{OrgId}");

            WaitForElement("#otheredit", 5);
            Find(id: "otheredit").Click();

            WaitForElement("#submitit", 5);
            Find(id: "submitit").Click();

            WaitForElement("p:nth-child(3) > a", 5);

            var log = db.ActivityLogs.FirstOrDefault(p => p.OrgId == OrgId & p.Activity.Contains("SentConfirmations"));
            log.ShouldNotBeNull();
        }

        [Fact]
        public void Should_SendAllConfirmations_ToStaff()
        {
            db = CMSDataContext.Create(DatabaseFixture.Host);
            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.OnlineReg.Controllers.OnlineRegController(requestManager);
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);

            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, new CmsData.Organization()
            {
                OrganizationName = RandomString(),
                RegistrationTitle = RandomString(),
                Location = RandomString(),
                RegistrationTypeId = RegistrationTypeCode.JoinOrganization,
                NotifyIds = "1"
            });

            OrgId = FakeOrg.org.OrganizationId;

            username = RandomString();
            password = RandomString();
            var CurrentUser = CreateUser(username, password);
            EmailAddress = CurrentUser.EmailAddress;
            Login();

            Open($"{rootUrl}OnlineReg/{OrgId}");

            WaitForElement("#otheredit", 5);
            Find(id: "otheredit").Click();

            WaitForElement("#submitit", 5);
            Find(id: "submitit").Click();

            WaitForElement("p:nth-child(3) > a", 5);

            var log = db.ActivityLogs.FirstOrDefault(p => p.OrgId == OrgId & p.Activity.Contains("SentConfirmationsToStaff"));
            log.ShouldNotBeNull();
        }

        public override void Dispose()
        {
            FakeOrganizationUtils.DeleteOrg(OrgId);
        }
    }
}
