using System;
using Xunit;
using CmsData;
using Shouldly;
using CmsData.Codes;
using CmsWeb.Areas.Manage.Models;

namespace CMSWebTests.Areas.Manage.Models
{
    public class VolunteerCommitmentsModelTest
    {
        [Theory]
        [InlineData(2019, 9, "09/01/2019")]
        [InlineData(2019, 12, "12/01/2019")]
        [InlineData(2020, 2, "02/02/2020")]
        public void ShouldCalculateFirstSundayOfTheMonth(int year, int month, string result)
        {
            var requestManager = FakeRequestManager.FakeRequest();
            var VolunteerOrgconfig = new Organization()
            {
                OrganizationName = "MockMasterName",
                RegistrationTitle = "MockMasterTitle",
                Location = "MockLocation",
                RegistrationTypeId = RegistrationTypeCode.ChooseVolunteerTimes,
            };

            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, VolunteerOrgconfig);            
            var m = new VolunteerCommitmentsModel(FakeOrg.org.OrganizationId);            
            var calculatedSunday = m.GetFirstSundayOfTheMonth(year, month);
            calculatedSunday.ToString("MM/dd/yyyy").ShouldBe(result);
            
            FakeOrganizationUtils.DeleteOrg(FakeOrg.org.OrganizationId);
        }
    }
}
