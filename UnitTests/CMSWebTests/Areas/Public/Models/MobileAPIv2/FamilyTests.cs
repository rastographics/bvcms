using CmsData;
using CmsWeb.Areas.Public.Models.MobileAPIv2;
using Family = CmsWeb.Areas.Public.Models.CheckInAPIv2.Family;
using CMSWebTests.Support;
using Shouldly;
using Xunit;
using SharedTestFixtures;
using System.Collections.Generic;
using System.Linq;

namespace CMSWebTests.Areas.Public.Models.MobileAPIv2
{
    [Collection(Collections.Database)]
    public class FamilyTests : DatabaseTestBase
    {
        [Fact]
        public void Should_Search_Family()
        {
            System.DateTime date = System.DateTime.Now.AddDays(1);
            var person = CreatePerson();
            List<Family> families = Family.forSearch(db, idb, person.LastName, 0, date, false);
            families.Count().ShouldBe(1);
        }
    }
}
