using Xunit;
using CmsWeb.Areas.People.Models;
using Shouldly;
using SharedTestFixtures;
using CmsData;
using UtilityExtensions;

namespace CMSWebTests.Areas.People.Models.Person.Enrollments
{
    [Collection(Collections.Database)]
    public class RegistrationsModelTests
    {
        [Fact]
        public void Should_Run_FulfillmentList()
        {
            using (var db = CMSDataContext.Create(Util.Host))
            {
                ContextTestUtils.CreateMockHttpContext();
                var m = new RegistrationsModel(db);
                m.PeopleId = 1;
                var fulfillmentlist = m.FulfillmentList();

                fulfillmentlist.ShouldNotBeNull();
            }
        }
    }
}
