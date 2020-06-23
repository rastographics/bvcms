using CmsData;
using CmsData.Codes;
using CmsData.Finance;
using CmsWeb.Areas.Dialog.Models;
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

namespace IntegrationTests.Areas.OnlineReg.Views
{
    [Collection(Collections.Webapp)]
    public class OnlineRegViewsTests : AccountTestBase
    {
        private int OrgId { get; set; }

        [Fact]
        public void Should_Change_Payment_Methods()
        {
            MaximizeWindow();

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            FinanceTestUtils.CreateMockPaymentProcessor(db, PaymentProcessTypes.OnlineRegistration, GatewayTypes.Transnational);
            Login();

            OrgId = CreateOrgWithFee();

            SettingUtils.UpdateSetting("UseRecaptcha", "false");

            PayRegistration(OrgId, true);            

            var startNewTransaction = Find(xpath: "//a[contains(text(),'Start a New Transaction')]");
            startNewTransaction.ShouldNotBeNull();

            var paymentInfo = db.PaymentInfos.SingleOrDefault(x => x.PeopleId == user.PeopleId);
            if (paymentInfo != null)
            {
                paymentInfo.PreferredPaymentType.ShouldBe("B");
            }
        }

        [Fact]
        public void Should_Payment_Form_Contain_Recaptcha()
        {
            MaximizeWindow();

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            FinanceTestUtils.CreateMockPaymentProcessor(db, PaymentProcessTypes.OnlineRegistration, GatewayTypes.Transnational);
            Login();

            OrgId = CreateOrgWithFee();

            SettingUtils.UpdateSetting("UseRecaptcha", "true");
            SettingUtils.UpdateSetting("googleReCaptchaSiteKey", RandomString());

            Open($"{rootUrl}OnlineReg/{OrgId}");

            Find(id: "otheredit").Click();
            WaitForElement("#submitit", 3);
            Find(id: "submitit").Click();

            Wait(4);

            var element = Find(css: ".recaptcha");
            element.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Payment_Form_Contain_NoRecaptcha()
        {
            MaximizeWindow();

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            FinanceTestUtils.CreateMockPaymentProcessor(db, PaymentProcessTypes.OnlineRegistration, GatewayTypes.Transnational);
            Login();

            OrgId = CreateOrgWithFee();

            SettingUtils.UpdateSetting("UseRecaptcha", "false");            

            Open($"{rootUrl}OnlineReg/{OrgId}");

            Find(id: "otheredit").Click();
            WaitForElement("#submitit", 3);
            Find(id: "submitit").Click();

            Wait(4);

            var element = Find(css: ".noRecaptcha");
            element.ShouldNotBeNull();
        }

        [Fact]
        public void Should_Not_Show_Deceased_Person_In_Family_Attendance()
        {
            MaximizeWindow();

            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });

            var org = MockOrganizations.CreateOrganization(db, RandomString());
            org.RegistrationTypeId = RegistrationTypeCode.RecordFamilyAttendance;
            db.SubmitChanges();

            var family = user.Person.Family;

            var deceasedPerson = CreatePerson(family);
            deceasedPerson.DeceasedDate = DateTime.Now.AddYears(-10);            
            db.SubmitChanges();

            Login();
            Wait(3);
            Open($"{rootUrl}OnlineReg/{org.OrganizationId}");
            Wait(3);
            PageSource.ShouldContain(user.Person.FirstName);
            PageSource.ShouldNotContain(deceasedPerson.FirstName);
        }

        public override void Dispose()
        {
            FakeOrganizationUtils.DeleteOrg(OrgId);
        }
    }
}
