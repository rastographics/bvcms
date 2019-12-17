using Xunit;
using CmsWeb.Areas.People.Models;
using Shouldly;
using SharedTestFixtures;

namespace CMSWebTests.Areas.People.Models.Person.Enrollments
{
    [Collection(Collections.Database)]
    public class RegistrationsModelTests
    {
        [Fact]
        public void Should_Run_FulfillmentList()
        {
            ContextTestUtils.CreateMockHttpContext();
            var m = new RegistrationsModel();
            m.PeopleId = 1;
            var fulfillmentlist = m.FulfillmentList();

            fulfillmentlist.ShouldNotBeNull();
        }
    }
}
