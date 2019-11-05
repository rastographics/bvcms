using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;
using CMSWebTests;
using System.Linq;
using CmsData;
using UtilityExtensions;

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
            WaitForElement(".active:nth-child(2) > a", 5);
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
            WaitForElement("#family_members", 5);

            var deceasedMember = Find(css: "#family_members > li.alert-danger");
            deceasedMember.ShouldNotBe(null);

            SettingUtils.UpdateSetting("HideDeceasedFromFamily", "true");

            Open($"{rootUrl}Person2/Current");
            WaitForElement("#family_members", 5);

            deceasedMember = Find(css: "#family_members > li.alert-danger");
            deceasedMember.ShouldBe(null);
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

            Open($"{rootUrl}Person2/1#tab-resources");
            WaitForElement(css: "#resources>ul>li>a", maxWaitTimeInSeconds: 5);

            PageSource.ShouldContain("MockResource");
        }
    }
}
