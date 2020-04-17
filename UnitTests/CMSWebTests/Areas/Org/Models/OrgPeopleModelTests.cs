using CmsData.View;
using SharedTestFixtures;
using System.Linq;
using Xunit;

namespace CMSWebTests.Areas.Org.Models
{
    [Collection(Collections.Database)]
    public class OrgPeopleModelTests : DatabaseTestBase
    {
        [Fact]
        public void OrgFilterPeopleTest()
        {
            bool ShowMinistryInfo = false;
            var person = CreatePerson();
            var organization = CreateOrganization();
            var orgFilter = CreateOrgFilter(organization.OrganizationId, person.PeopleId);
            var organizationMember = CreateOrganizationMember(organization.OrganizationId, person.PeopleId);
            IQueryable<OrgFilterPerson> q = from p in db.OrgFilterPeople(orgFilter.QueryId, ShowMinistryInfo) select p;
            var expectedName = person.Name;
            var actualName = q.FirstOrDefault().Name;
            Assert.Equal(expectedName, actualName);
            DeleteOrgFilter(orgFilter);
            DeleteOrganizationMember(organizationMember);
            DeleteOrganization(organization);
        }
    }
}
