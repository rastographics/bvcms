using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using Xunit;
using CMSWebTests;
using System.Linq;
using CmsData;
using UtilityExtensions;
using OpenQA.Selenium.Support.UI;

namespace IntegrationTests.Areas.People.Views.Person
{
    [Collection(Collections.Webapp)]
    public class PersonViewsTests : AccountTestBase
    {
        [Fact, FeatureTest]
        public void Should_Hide_Giving_Tab()
        {
            SettingUtils.UpdateSetting("HideGivingTabMyDataUsers", "false");

            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password);
            Login();

            Open($"{rootUrl}Person2/{user.PeopleId}");
            WaitForElement(".active:nth-child(2) > a", 10);
            PageSource.ShouldContain("<a href=\"#giving\" aria-controls=\"giving\" data-toggle=\"tab\">Giving</a>");

            SettingUtils.UpdateSetting("HideGivingTabMyDataUsers", "true");

            Open($"{rootUrl}Person2/Current"); //refresh page
            WaitForElement(".active:nth-child(2) > a", 5);
            PageSource.ShouldNotContain("<a href=\"#giving\" aria-controls=\"giving\" data-toggle=\"tab\">Giving</a>");

            SettingUtils.DeleteSetting("HideGivingTabMyDataUsers");
        }

        [Fact]
        public void Should_Hide_Deceased_People()
        {
            username = RandomString();
            password = RandomString();

            var FamilyMember = FamilyUtils.CreateFamilyWithDeceasedMember();

            var user = CreateUser(username, password, person: FamilyMember);
            Login();

            SettingUtils.UpdateSetting("HideDeceasedFromFamily", "false");

            Open($"{rootUrl}Person2/{FamilyMember.PeopleId}");
            WaitForElement("#family_members", 10);

            var deceasedMember = Find(css: "#family_members > li.alert-danger");
            deceasedMember.ShouldNotBe(null);

            SettingUtils.UpdateSetting("HideDeceasedFromFamily", "true");

            Open($"{rootUrl}Person2/Current");
            WaitForElement("#family_members", 10);

            deceasedMember = Find(css: "#family_members > li.alert-danger");
            deceasedMember.ShouldBe(null);
        }

        [Fact, FeatureTest]
        public void Should_Show_Combine_Giving_Summaries()
        {
            SettingUtils.UpdateSetting("CombinedGivingSummary", "true");

            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password);
            Login();

            Open($"{rootUrl}Person2/{user.PeopleId}");
            WaitForElement(".active:nth-child(2) > a", 10);
            PageSource.ShouldContain("<a href=\"#giving\" aria-controls=\"giving\" data-toggle=\"tab\">Giving</a>");

            Find(text: "Giving").Click();
            WaitForElement("#Pledge-detail-section", 10);
            WaitForElement("#Giving-detail-section", 10);

            PageSource.ShouldContain("Pledge Summary");
            PageSource.ShouldContain("Pledge Detail");
            PageSource.ShouldContain("Giving Summary");
            PageSource.ShouldContain("Giving Detail");
        }

        [Fact, FeatureTest]
        public void Should_Show_Pledge_Giving_Tab()
        {
            SettingUtils.UpdateSetting("CombinedGivingSummary", "false");

            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password);
            Login();

            Open($"{rootUrl}Person2/{user.PeopleId}");
            WaitForElement(".active:nth-child(2) > a", 10);
            PageSource.ShouldContain("<a href=\"#giving\" aria-controls=\"giving\" data-toggle=\"tab\">Giving</a>");

            Find(text: "Giving").Click();
            WaitForElement("#giving", 10);

            PageSource.ShouldContain("Contributions");
            PageSource.ShouldContain("Pledges");
        }

        [Fact, FeatureTest]
        public void Should_Remember_Last_Year_Selected()
        {
            SettingUtils.UpdateSetting("CombinedGivingSummary", "false");

            var username1 = RandomString();
            var password1 = RandomString();
            var user1 = CreateUser(username1, password1);

            var username2 = RandomString();
            var password2 = RandomString();
            var user2 = CreateUser(username2, password2);

            var admin = LoginAsAdmin();

            Open($"{rootUrl}Person2/{user1.PeopleId}");
            WaitForElement(".active:nth-child(2) > a", 10);
            PageSource.ShouldContain("<a href=\"#giving\" aria-controls=\"giving\" data-toggle=\"tab\">Giving</a>");

            Find(text: "Giving").Click();
            WaitForElement("#GivingYear", 10);

            var dropdown = Find(css: "#GivingYear");
            var selectElement = new SelectElement(dropdown);
            selectElement.SelectByText("All Years");
            
            WaitForElement("#Giving-detail-section", 10);

            Open($"{rootUrl}Person2/{user2.PeopleId}");
            WaitForElement("#GivingYear", 10);

            PageSource.ShouldContain("<input name=\"Year\" type=\"hidden\" value=\"All Years\">");            
        }

        [Fact]
        public void New_Resources_Should_Be_Displayed()
        {
            SettingUtils.UpdateSetting("Resources-Enabled", "true");

            username = RandomString();
            password = RandomString();

            Role roleManageResources = db.Roles.Where(x => x.RoleName == "ManageResources").SingleOrDefault();
            if(roleManageResources == null)
            {
                roleManageResources = new Role { RoleName = "ManageResources" };
                db.Roles.InsertOnSubmit(roleManageResources);
                db.SubmitChanges();
            }

            Role roleViewResources = db.Roles.Where(x => x.RoleName == "ViewResources").SingleOrDefault();
            if(roleViewResources == null)
            {
                roleViewResources = new Role { RoleName = "ViewResources" };
                db.Roles.InsertOnSubmit(roleViewResources);
                db.SubmitChanges();
            }

            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "ManageResources", "ViewResources" });
            Login();

            using (var db = CMSDataContext.Create(Util.Host))
            {
                var ResourceType = new ResourceType()
                {
                    Name = "MockResourceType",
                    DisplayOrder = 0
                };
                db.ResourceTypes.InsertOnSubmit(ResourceType);
                db.SubmitChanges();

                var ResourceCategory = new ResourceCategory()
                {
                    Name = "MockResourceCategory",
                    ResourceTypeId = ResourceType.ResourceTypeId,
                    DisplayOrder = 0
                };
                db.ResourceCategories.InsertOnSubmit(ResourceCategory);
                db.SubmitChanges();
            }

            Open($"{rootUrl}Resources");
            WaitForElement("#addresource", maxWaitTimeInSeconds: 5);

            Find(id: "addresource").Click();
            WaitForElement("#Name", maxWaitTimeInSeconds: 5);

            Find(id: "Name").Clear();
            Find(id: "Name").SendKeys("MockResource");
            Find(css: "#new-resource-modal>div>form>div.modal-footer>a.btn.btn-primary.validate.submit").Click();

            MaximizeWindow();
            Open($"{rootUrl}Person2/1#tab-resources");
            WaitForElement(css: "#resources>ul>li>a", maxWaitTimeInSeconds: 5);

            PageSource.ShouldContain("MockResource");
        }
    }
}
