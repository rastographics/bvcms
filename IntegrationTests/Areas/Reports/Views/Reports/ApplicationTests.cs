using CmsData;
using CmsData.Codes;
using CmsWeb;
using CmsWeb.Areas.OnlineReg.Controllers;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Models;
using CMSWebTests;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using static CmsWeb.Models.MailingController;

namespace IntegrationTests.Areas.Reports.Views.Reports
{
    [Collection(Collections.Webapp)]
    public class ApplicationTests : AccountTestBase
    {
        private int OrgId { get; set; }
        private int SepacialContentId { get; set; }

        /* Application export should pull Head of Household PeopleID*/
        [Fact, FeatureTest]
        public void Application_Export_Should_Pull_HoHPeopleID()
        {
            driver.Manage().Window.Maximize();
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;
            var controller = new OnlineRegController(requestManager);
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);

            var m = OrganizationModel.Create(db, requestManager.CurrentUser);
            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager);
            var oid = FakeOrg.org.OrganizationId;
            m.OrgId = oid;
            var wife = CreateUser(RandomString(), RandomString());
            
            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Membership" });
            Login();

            //Create family and then Execute GetCouplesBothList to see if the right HeadOfHouseHoldId is retrieved...
            var p = CreateFakeFamily(oid, m, controller);
            var mailingModel = new MailingController(requestManager);
            var ExcelCouplesBoth = mailingModel.GetCouplesBothList(m.QueryId, 500);

            //assert

            //Finalize
            FakeOrganizationUtils.DeleteOrg(FakeOrg.org.OrganizationId);
            RemoveFakePeopleFromDB(ToPeople(ExcelCouplesBoth), db);
            Logout();
        }

        private List<Person> ToPeople(List<CouplesBothInfo> couples)
        {
            var ppl = new List<Person>();
            foreach (var c in couples)
            {
                ppl.Add(c.ToPerson());
            }
            return ppl;
        }

        private void RemoveFakePeopleFromDB(List<Person> peopleList, CMSDataContext db)
        {
            var families = new List<Family>();
            foreach (var p in peopleList)
            {
                var f = db.Families.SingleOrDefault(ff => ff.FamilyId == p.FamilyId);
                db.PurgePerson(p.PeopleId);
                if (!families.Contains(f)) { families.Add(f); }
                db.SubmitChanges();
            }
            foreach (var f in families)
            {
                db.Families.DeleteOnSubmit(f);
            }
        }

        private User CreateFamilyMember(int genderID, int maritalStatusId, int positionInFamilyId, Family f = null)
        {
            var p = CreateUser(RandomString(), RandomString(), family: f, positionInFamilyId: positionInFamilyId, maritalStatusId: maritalStatusId, genderId: genderID);
            db.SubmitChanges();
            return p;
        }

        private List<Person> CreateFakeFamily(int oid, OrganizationModel om, OnlineRegController orc)
        {
            var family = new List<Person>();

            var husband = CreateFamilyMember(GenderCode.Male, MaritalStatusCode.Married, PositionInFamily.PrimaryAdult);
            var orm = FakeOrganizationUtils.GetFakeOnlineRegModel(oid, husband.UserId);
            var resultSubmitQuestions = orc.SubmitQuestions(0, orm);
            var resultCompleteRegistration = orc.CompleteRegistration(orm);
            family.Add(husband.Person);

            var wife = CreateFamilyMember(GenderCode.Female, MaritalStatusCode.Married, PositionInFamily.PrimaryAdult, husband.Person.Family);
            orm = FakeOrganizationUtils.GetFakeOnlineRegModel(oid, wife.UserId);
            resultSubmitQuestions = orc.SubmitQuestions(0, orm);
            resultCompleteRegistration = orc.CompleteRegistration(orm);
            family.Add(wife.Person);

            var child1 = CreateFamilyMember(GenderCode.Female, MaritalStatusCode.Single, PositionInFamily.Child, husband.Person.Family);
            orm = FakeOrganizationUtils.GetFakeOnlineRegModel(oid, child1.UserId);
            resultSubmitQuestions = orc.SubmitQuestions(0, orm);
            resultCompleteRegistration = orc.CompleteRegistration(orm);
            family.Add(child1.Person);

            var child2 = CreateFamilyMember(GenderCode.Male, MaritalStatusCode.Single, PositionInFamily.Child, husband.Person.Family);
            orm = FakeOrganizationUtils.GetFakeOnlineRegModel(oid, child2.UserId);
            resultSubmitQuestions = orc.SubmitQuestions(0, orm);
            resultCompleteRegistration = orc.CompleteRegistration(orm);
            family.Add(child2.Person);

            var secAdult = CreateFamilyMember(GenderCode.Male, MaritalStatusCode.Single, PositionInFamily.SecondaryAdult, husband.Person.Family);
            orm = FakeOrganizationUtils.GetFakeOnlineRegModel(oid, secAdult.UserId);
            resultSubmitQuestions = orc.SubmitQuestions(0, orm);
            resultCompleteRegistration = orc.CompleteRegistration(orm);
            family.Add(secAdult.Person);

            return family;
        }

        [Fact, FeatureTest]
        public void Application_Report_Should_Have_Answers()
        {
            driver.Manage().Window.Maximize();
            var requestManager = FakeRequestManager.Create();

            var Orgconfig = new Organization()
            {
                OrganizationName = "MockName",
                RegistrationTitle = "MockTitle",
                Location = "MockLocation",
                RegistrationTypeId = 1
            };

            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, Orgconfig);
            OrgId = FakeOrg.org.OrganizationId;

            var NewSpecialContent = SpecialContentUtils.CreateSpecialContent(0, "MembershipApp2017", null);
            SepacialContentId = NewSpecialContent.Id;
            SpecialContentUtils.UpdateSpecialContent(NewSpecialContent.Id, "MembershipApp2017", "MembershipApp2017", GetValidHtmlContent(), false, null, "", null);

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Membership" });
            Login();

            /*
                Using WaitForElement() doesn't work in this test
                WaitForElement() only makes the duration of loading spinner longer
                and causes a "reference not set to an instance of an object" error in the Find() functions below
            */

            Open($"{rootUrl}Org/{OrgId}#tab-Registrations-tab");
            WaitForElementToDisappear(loadingUI, 30);

            ScrollTo(css: "#Questions-tab > .ajax");
            Find(css: "#Questions-tab > .ajax").Click();
            WaitForElementToDisappear(loadingUI);

            Find(css: ".col-sm-12 .edit").Click();
            WaitForElementToDisappear(loadingUI);

            Find(css: ".pull-right > .btn-success:nth-child(2)").Click();
            Wait(1);

            Find(css: ".AskText > a").Click();
            WaitForElement("#QuestionList > div.type-AskText");

            Find(css: ".modal-footer > .btn-primary:nth-child(1)").Click();
            var swal = ".sweet-alert.visible";
            WaitForElement(swal);

            Find(xpath: "//button[contains(.,'Yes, Add Questions')]").Click();
            WaitForElementToDisappear(".sweet-overlay");

            Find(css: "#QuestionList > div.type-AskText a.btn.btn-success").Click();
            WaitForElementToDisappear(loadingUI);

            var input = "div.ask-texts > div.well.movable > div.form-group > div.controls > input.form-control:nth-child(1)";
            WaitForElement(input);

            var InputAskItem = Find(css: input);
            InputAskItem.Clear();
            InputAskItem.SendKeys("Vow 1 reads: \"Do you acknowledge yourself to be a sinner in the sight of God, justly deserving his displeasure and without hope except through his sovereign mercy?\"");

            Find(css: ".ask-texts > .well").Click();
            WaitForElementToDisappear(loadingUI);

            ScrollTo(css: "#Questions-tab > .ajax");
            Find(xpath: "(//a[contains(text(),'Save')])[3]").Click();
            WaitForElementToDisappear(loadingUI);

            Open($"{rootUrl}OnlineReg/{OrgId}");

            var InputField = Find(id: "List0.Text0_0");
            InputField.Clear();
            InputField.SendKeys("ThisTextMustAppearInTests");
            Find(id: "otheredit").Click();

            WaitForElement("#submitit", 5);
            Find(id: "submitit").Click();
            Wait(2);

            Open($"{rootUrl}Reports/Application/{OrgId}/{user.PeopleId}/MembershipApp2017");
            WaitForElement("h2", 5);

            PageSource.ShouldContain("<h2>");
        }

        private string GetValidHtmlContent()
        {
            string res = ReportsViewTestsResources.ValidHtmlContent;
            return res;
        }

        public override void Dispose()
        {
            SpecialContentUtils.DeleteSpecialContent(SepacialContentId);
            FakeOrganizationUtils.DeleteOrg(OrgId);
            base.Dispose();
        }
    }
}
