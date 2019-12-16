using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.OnlineReg.Controllers;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Models;
using SharedTestFixtures;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static CmsWeb.Models.MailingController;

namespace CMSWebTests.Areas.Reports.Models.Reports
{
    [Collection(Collections.Database)]
    public class MailingControllerTests : DatabaseTestBase
    {

        [Fact]
        public void GetCouplesBothList_Should_Pull_Proper_HoHPeopleID()
        {
            /* Excel export should pull proper Head of Household PeopleID*/
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

            //Create family and then Execute GetCouplesBothList to see if the right HeadOfHouseHoldId is retrieved...
            var p = CreateFakeFamily(oid, m, controller);
            var mailingModel = new MailingController(requestManager);
            var ExcelCouplesBoth = mailingModel.GetCouplesBothList(m.QueryId, 500);

            FakeOrganizationUtils.DeleteOrg(FakeOrg.org.OrganizationId);
            RemoveFakePeopleFromDB(ToPeople(ExcelCouplesBoth), db);
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
    }
}
