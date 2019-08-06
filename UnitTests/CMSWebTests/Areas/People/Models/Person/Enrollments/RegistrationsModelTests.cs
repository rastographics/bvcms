using Xunit;
using CmsWeb.Areas.People.Models;
using CmsData;
using Shouldly;

namespace CMSWebTests.Areas.People.Models.Person.Enrollments
{
    [Collection("Database collection")]
    public class RegistrationsModelTests
    {
        [Fact]
        public void Should_Run_FulfillmentList()
        {
            DbUtil.Db = CMSDataContext.Create(host: "localhost");
            var m = new RegistrationsModel();
            var fulfillmentlist = m.FulfillmentList();

            fulfillmentlist.ShouldNotBeNull();
        }
    }
}
