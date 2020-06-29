using Xunit;
using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedTestFixtures;
using Shouldly;

namespace CmsData.Tests
{
    [Collection(Collections.Database)]
    public class OrganizationTests : DatabaseTestBase
    {
        [Fact()]
        public void OrganizationTest()
        {
            var orgName = "Test Organization" + DateTime.Now.ToString();
            var campus = MockCampus.CreateCampus(db, "AP", "Main Number 2");
            var newOrg = MockOrganizations.CreateOrganization(db, orgName, campus.Id, null, null, "http://localhost:50171/Give/Test%20222%20Title");

            var org = (from o in db.Organizations where o.OrganizationId == newOrg.OrganizationId select o).FirstOrDefault();
            org.OrganizationName.ShouldBe(orgName);
            org.RedirectUrl.ShouldBe(newOrg.RedirectUrl);

            MockOrganizations.DeleteOrganization(db, org);
            MockCampus.DeleteCampus(db, campus);
        }
    }
}
