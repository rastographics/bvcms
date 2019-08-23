using Xunit;
using CmsWeb.Areas.People.Models;
using CmsData;
using Shouldly;
using UtilityExtensions;
using SharedTestFixtures;

namespace CMSWebTests.Areas.People.Models.Person.Enrollments
{
    [Collection(Collections.Database)]
    public class RegistrationsModelTests
    {
        [Fact]
        public void Should_Run_FulfillmentList()
        {
            DbUtil.Db = CMSDataContext.Create(Util.Host);
            var m = new RegistrationsModel();
            m.PeopleId = 1;
            var fulfillmentlist = m.FulfillmentList();

            fulfillmentlist.ShouldNotBeNull();
        }
    }
}
