using CmsData;
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

namespace IntegrationTests.Areas.Reports.Views.Reports
{
    [Collection(Collections.Webapp)]
    public class ApplicationTests : AccountTestBase
    {
        private int OrgId { get; set; }

        [Fact]
        public void Application_Report_Should_Have_Awnsers()
        {
            var requestManager = FakeRequestManager.Create();

            var Orgconfig = new Organization()
            {
                OrganizationName = "MockName",
                RegistrationTitle = "MockTitle",
                Location = "MockLocation",
                RegistrationTypeId = 1,
                RegSettingXml = XMLSettings()
            };

            var FakeMasterOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, Orgconfig);
            OrgId = FakeMasterOrg.org.OrganizationId;

            var NewSpecialContent = SpecialContentUtils.CreateSpecialContent(0, "MembershipApp2017", null);
            SpecialContentUtils.UpdateSpecialContent(NewSpecialContent.Id, "MembershipApp2017", "MembershipApp2017", GetValidHtmlContent(), false, null, "", null);

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Membership" });
            Login();

            Open($"{rootUrl}OnlineReg/{OrgId}");

            var InputField = Find(id: "List0.Text0_0");
            InputField.Clear();
            InputField.SendKeys("ThisTextMustAppearInTests");

            Find(id: "otheredit").Click();

            Open($"{rootUrl}OnlineReg/{OrgId}");
            Open($"{rootUrl}Reports/Application/{OrgId}/{user.PeopleId}/MembershipApp2017");

            WaitForElement("h2", 10);

            PageSource.ShouldContain("ThisTextMustAppearInTests");
            SpecialContentUtils.DeleteSpecialContent(NewSpecialContent.Id);
        }

        private string XMLSettings()
        {
            string Settings = ReportsViewTestsResources.XMLSettings;

            return Settings;
        }

        private string GetValidHtmlContent()
        {
            string res = ReportsViewTestsResources.ValidHtmlContent;
            return res;
        }
    }
}
