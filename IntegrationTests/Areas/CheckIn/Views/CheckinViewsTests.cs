using CmsData;
using CmsData.Codes;
using IntegrationTests.Support;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Xunit;

namespace IntegrationTests.Areas.CheckIn.Views
{
    [Collection(Collections.Webapp)]
    public class CheckinViewsTests : AccountTestBase
    {
        [Fact]
        public void Should_Display_Correct_Class_Date()
        {
            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Checkin" });

            var org = MockOrganizations.CreateOrganization(db, RandomString());
            org.RegistrationTypeId = RegistrationTypeCode.JoinOrganization;
            db.SubmitChanges();

            var people = db.People.SingleOrDefault(p => p.PeopleId == user.PeopleId);
            var phone = RandomPhoneNumber();
            people.CellPhone = phone;
            db.SubmitChanges();

            var currentDate = DateTime.Now;
            var dayOfTheWeek = currentDate.DayOfWeek.ToString();
            var datePlus30 = currentDate.AddMinutes(30);
            var strinDatePlus30 = $"{datePlus30.Hour}:{datePlus30.Minute} {datePlus30.ToString("tt", new CultureInfo("en-US"))}";
         
            Login();
            Wait(5);

            Open($"{rootUrl}OnlineReg/{org.OrganizationId}");
            Wait(4);
            Find(id: "otheredit").Click();
            Wait(2);
            Find(id: "submitit").Click();
            Wait(2);

            Open($"{rootUrl}Org/{org.OrganizationId}#tab-Settings-tab");
            Wait(7);

            Find(xpath: "//a[contains(@href, '#Attendance')]").Click();
            Wait(5);
            Find(css: ".pull-right:nth-child(1) > .edit").Click();
            Wait(5);
            Find(id: "addsch").Click();
            Wait(5);

            var dropdown = Find(xpath: "//div[@id='schedules']/div/div/div/div/select");
            var selectElement = new SelectElement(dropdown);
            selectElement.SelectByText(dayOfTheWeek);

            Find(xpath: "//div[@id='schedules']/div/div/div[2]/div/div/input").Clear();
            Find(xpath: "//div[@id='schedules']/div/div/div[2]/div/div/input").SendKeys(strinDatePlus30);

            Find(xpath: "(//a[contains(text(),'Save')])[3]").Click();
            Wait(3);

            Find(xpath: "//a[contains(@href, '#CheckIn')]").Click();
            Wait(3);
            Find(xpath: "//div[@id='CheckIn']/form/div[2]/div[2]/div/div/div/label").Click();
            Find(xpath: "(//a[contains(text(),'Save')])[3]").Click();
            Wait(2);

            Open($"{rootUrl}Checkin/");
            WaitForElement("#CheckInApp", 30);

            Find(name: "username").SendKeys(username);
            Find(name: "password").SendKeys(password);
            Find(name: "kiosk").SendKeys("test");
            Find(css: "input[type=submit]").Click();
            Wait(10);

            Find(name: "phone").SendKeys(phone);
            Find(name: "phone").SendKeys(Keys.Enter);

            PageSource.ShouldContain(strinDatePlus30);
        }
    }
}
