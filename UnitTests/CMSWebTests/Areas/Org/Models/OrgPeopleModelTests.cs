using CmsData;
using CmsData.View;
using CmsWeb.Areas.Org.Controllers;
using CmsWeb.Areas.Org.Models;
using SharedTestFixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CMSWebTests.Areas.Org.Models
{
    [Collection(Collections.Database)]
    public class OrgPeopleModelTests : DatabaseTestBase
    {
        Guid OrgFilterQueryId = new Guid("11111111-1111-1111-1111-111111111111");
        int organizationId = 2179950;
        int peopleId = 0;
        string organizationName = "Jason Rice Test Organization";

        [Fact]
        public void OrgFilterPeopleTest()
        {
            var controller = new OrgGroupsController(FakeRequestManager.Create());
            var routeDataValues = new Dictionary<string, string>();
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);
            CMSDataContext db = DatabaseFixture.NewDbContext();
            var ShowMinistryInfo = false;

            AddSamplePerson(db);
            peopleId = db.People.Where(p => p.Name == "Jason Rice" && p.BirthYear == 1987 && p.BirthMonth == 5 && p.BirthDay == 21).FirstOrDefault().PeopleId;
            AddSampleFilter(db);
            CreateOrganization(organizationName);
            organizationId = db.Organizations.Where(o => o.OrganizationName == organizationName).FirstOrDefault().OrganizationId;
            AddSampleOrganizationMember(db);

            IQueryable<OrgFilterPerson> q = from p in db.OrgFilterPeople(OrgFilterQueryId, ShowMinistryInfo) select p;

            var expectedName = "Jason Rice";
            var actualName = q.FirstOrDefault().Name;
            Assert.Equal(expectedName, actualName);

            DeleteSampleFilter(db);
            DeleteSampleOrganizationMember(db);
            DeleteSamplePerson(db);
            DeleteSampleOrganization(db);
        }

        private void AddSamplePerson(CMSDataContext db)
        {
            var person = new Person();
            person.FirstName = "Jason";
            person.LastName = "Rice";
            person.Name = "Jason Rice";
            person.BirthYear = 1987;
            person.BirthMonth = 5;
            person.BirthDay = 21;

            person.CreatedBy = 3;
            person.CreatedDate = DateTime.Now;
            person.DropCodeId = 0;
            person.GenderId = 1;
            person.DoNotMailFlag = false;
            person.DoNotCallFlag = false;
            person.DoNotVisitFlag = false;
            person.AddressTypeId = 10;
            person.PhonePrefId = 10;
            person.MaritalStatusId = 20;
            person.PositionInFamilyId = 10;
            person.MemberStatusId = 10;
            person.FamilyId = 2207072;
            person.JoinCodeId = 50;
            person.InterestedInJoining = false;
            person.PleaseVisit = false;
            person.InfoBecomeAChristian = false;
            person.ContributionsStatement = false;
            person.ReceiveSMS = false;
            db.People.InsertOnSubmit(person);
            db.SubmitChanges();
        }
        private void DeleteSamplePerson(CMSDataContext db)
        {
            var person = new Person();
            person = db.People.Where(p => p.Name == "Jason Rice" && p.BirthYear == 1987 && p.BirthMonth == 5 && p.BirthDay == 21).FirstOrDefault();
            db.People.DeleteOnSubmit(person);
            db.SubmitChanges();
        }

        private void AddSampleFilter(CMSDataContext db)
        {
            var filter = new OrgFilter();
            filter.QueryId = OrgFilterQueryId;
            filter.Id = 2179950;
            filter.GroupSelect = "10";
            filter.FirstName = peopleId.ToString();
            filter.LastName = null;
            filter.SgFilter = null;
            filter.ShowHidden = false;
            filter.FilterIndividuals = false;
            filter.FilterTag = false;
            filter.TagId = 235651;
            filter.LastUpdated = DateTime.Now;
            filter.UserId = null;
            db.OrgFilters.InsertOnSubmit(filter);
            db.SubmitChanges();
        }
        private void DeleteSampleFilter(CMSDataContext db)
        {
            var filter = new OrgFilter();
            var QueryId = OrgFilterQueryId;
            filter = db.OrgFilters.Where(o => o.QueryId == QueryId).FirstOrDefault();
            db.OrgFilters.DeleteOnSubmit(filter);
            db.SubmitChanges();
        }

        private void AddSampleOrganization(CMSDataContext db)
        {
            var organization = new Organization();
            organization.OrganizationId = organizationId;
            organization.CreatedBy = 3;
            organization.CreatedDate = DateTime.Now;
            organization.OrganizationStatusId = 30;
            organization.DivisionId = 221;  //maybe
            organization.LeaderMemberTypeId = 130;  //maybe
            organization.LeaderId = 5488;   //maybe
            organization.RegistrationTypeId = 1;    //maybe
            organization.OrganizationTypeId = 40;
            organization.AllowAttendOverlap = false;
            organization.IsRecreationTeam = false;
            organization.SendAttendanceLink = false;
            organization.TripFundingPagesEnable = false;
            organization.TripFundingPagesPublic = false;
            organization.TripFundingPagesShowAmounts = false;
            organization.OrganizationName = organizationName;
            db.Organizations.InsertOnSubmit(organization);
            db.SubmitChanges();
        }
        private void DeleteSampleOrganization(CMSDataContext db)
        {
            var organization = new Organization();
            organization = db.Organizations.Where(o => o.OrganizationName == organizationName).FirstOrDefault();
            db.Organizations.DeleteOnSubmit(organization);
            db.SubmitChanges();
        }

        private void AddSampleOrganizationMember(CMSDataContext db)
        {
            var organizationMember = new OrganizationMember();
            organizationMember.OrganizationId = organizationId;
            organizationMember.PeopleId = peopleId;
            organizationMember.MemberTypeId = 111;
            db.OrganizationMembers.InsertOnSubmit(organizationMember);
            db.SubmitChanges();
        }
        private void DeleteSampleOrganizationMember(CMSDataContext db)
        {
            var organizationMember = new OrganizationMember();
            organizationMember = db.OrganizationMembers.Where(o => o.OrganizationId == organizationId && o.PeopleId == peopleId).FirstOrDefault();
            db.OrganizationMembers.DeleteOnSubmit(organizationMember);
            db.SubmitChanges();
        }

        
    }
}
