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

namespace IntegrationTests.Areas.OnlineReg.Models.OnlineRegPerson
{
    [Collection(Collections.Webapp)]
    public class EnrollTests: AccountTestBase
    {
        private new CMSDataContext db = null;
        private int OrgId { get; set; }
        private string EmailAddress { get; set; }

        [Fact]
        public void Should_Store_and_Populate_RegEmail()
        {
            db = CMSDataContext.Create(DatabaseFixture.Host);
            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.OnlineReg.Controllers.OnlineRegController(requestManager);
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);

            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, new CmsData.Organization()
            {
                OrganizationName = "MockName",
                RegistrationTitle = "MockTitle",
                Location = "MockLocation",
                RegistrationTypeId = RegistrationTypeCode.JoinOrganization
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

            var RegEmail = db.RecRegs.Where(x => x.PeopleId == CurrentUser.PeopleId).Select(x => x.Email).FirstOrDefault();

            RegEmail.ShouldBe(CurrentUser.EmailAddress);
        }

        public override void Dispose()
        {
            FakeOrganizationUtils.DeleteOrg(OrgId);
            db.ExecuteCommand("DELETE FROM [RecReg] WHERE [email] = '{0}'", EmailAddress);
        }
    }
}
