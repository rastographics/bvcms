using CmsData;
using CmsData.Codes;
using CMSWebTests;
using System.Collections.Generic;
using Xunit;

namespace IntegrationTests.Support
{
    /// <summary>
    /// Any feature test that needs a logged in account should inherit this class
    /// </summary>
    public class AccountTestBase : FeatureTestBase
    {
        protected const string profileMenu = "#navbar #me, #bluebar-menu > div.btn-group.btn-group-lg > button";

        //some commonly used variables
        protected string username;
        protected string password;

        public string loginUrl => rootUrl + "Logon?ReturnUrl=%2f";

        protected override bool UseSharedDriver => true;

        protected CmsData.User CreateUser()
        {
            return CreateUser(username, password);
        }

        protected void Login(string withPassword = null)
        {
            Open(rootUrl);

            WaitForElement("#inputEmail", 30);

            Find(name: "UsernameOrEmail").SendKeys(username);
            Find(name: "Password").SendKeys(withPassword ?? password);
            Find(css: "input[type=submit]").Click();
        }
        protected CmsData.User LoginAsAdmin()
        {
            username = RandomString();
            password = RandomString();
            var person = CreateUser(username, password, roles: new[] { "Access", "Edit", "Admin", "Finance" });
            Login();
            return person;
        }

        protected void Logout()
        {
            WaitForElementToDisappear(loadingUI);
            RepeatUntil(() => Find(css: profileMenu)?.Click(), () => Find(text: "Log Out") != null);
            Find(text: "Log Out").Click();

            Assert.Contains("Sign In", PageSource);

            Assert.True(driver.Url == loginUrl);
        }

        protected int CreateOrgWithFee()
        {
            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.OnlineReg.Controllers.OnlineRegController(requestManager);
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);

            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, new Organization()
            {
                OrganizationName = "MockName",
                RegistrationTitle = "MockTitle",
                Location = "MockLocation",
                RegistrationTypeId = RegistrationTypeCode.JoinOrganization
            });

            var OrgId = FakeOrg.org.OrganizationId;

            Open($"{rootUrl}Org/{OrgId}#tab-Registrations-tab");
            WaitForElementToDisappear(loadingUI, maxWaitTimeInSeconds: 10);

            ScrollTo(css: "#Registration > form > h4:nth-child(3)");
            Find(css: "#Fees-tab > a").Click();
            WaitForElementToDisappear(loadingUI, maxWaitTimeInSeconds: 10);

            Find(css: "#Fees .row .edit").Click();
            WaitForElementToDisappear(loadingUI, maxWaitTimeInSeconds: 10);

            ScrollTo(id: "Fee");
            Find(id: "Fee").Clear();
            Find(id: "Fee").SendKeys(RandomNumber(1, 1000).ToString());
            Find(css: ".pull-right:nth-child(1) > .validate").Click();
            Wait(5);

            return OrgId;
        }

        protected void PayRegistration(int OrgId)
        {
            Open($"{rootUrl}OnlineReg/{OrgId}");

            Find(id: "otheredit").Click();
            WaitForElement("#submitit", 3);
            Find(id: "submitit").Click();

            Wait(4);

            Find(id: "Address").Clear();
            Find(id: "Address").SendKeys("St 12");

            Find(id: "City").Clear();
            Find(id: "City").SendKeys("City");

            Find(id: "State").Clear();
            Find(id: "State").SendKeys("State");

            Find(id: "Country").Click();
            Find(css: ".form-group:nth-child(10)").Click();

            Find(id: "Zip").Clear();
            Find(id: "Zip").SendKeys("01000");

            Find(id: "Phone").Clear();
            Find(id: "Phone").SendKeys("1234567890");

            Find(css: ".btn-group > .btn:nth-child(2)").Click();

            Find(id: "Routing").Clear();
            Find(id: "Routing").SendKeys("123456780");

            Find(id: "Account").Clear();
            Find(id: "Account").SendKeys("111110");

            Find(id: "SavePayInfo").Click();

            WaitForElement("#submitit", maxWaitTimeInSeconds: 5);
            Find(id: "submitit").Click();
            Wait(5);
        }
    }
}
