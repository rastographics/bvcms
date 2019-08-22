using System;
using Xunit;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmsData.Codes;
using CmsWeb.Areas.Manage.Models;

namespace CMSWebTests.Areas.Manage.Models
{
    public class VolunteerCommitmentsModelTest
    {
        [Fact]
        public void ShouldCalculateFirstSundayOfTheMonth()
        {            
            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization(RegistrationTypeCode.ChooseVolunteerTimes);            
            var m = new VolunteerCommitmentsModel(FakeOrg.org.OrganizationId);            
            var calculatedSunday = m.GetFirstSundayOfTheMonth(DateTime.Now.Year, DateTime.Now.Month);
            Assert.Equal(DayOfWeek.Sunday, calculatedSunday.DayOfWeek);
            Assert.True(calculatedSunday.Day < 8, "The calculated Sunday did not fall in the first week of the month");
            FakeOrganizationUtils.DeleteOrg(FakeOrg.org.OrganizationId);
        }
    }
}
