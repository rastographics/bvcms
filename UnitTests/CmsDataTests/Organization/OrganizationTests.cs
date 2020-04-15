using CmsData;
using CmsData.Codes;
using DocuSign.eSign.Model;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CmsDataTests
{
    [Collection(Collections.Database)]
    public class OrganizationTests : DatabaseTestBase
    {
        [Theory]
        [InlineData("e191", 5)]
        [InlineData("191", 5)]
        [InlineData("2179950", 1)]
        public void OrgSearchByName(string searchString, int count)
        {
            List<Organization> orgList = new List<Organization>();
            for(int i = 0; i < count; i++)
            {
                string orgName = searchString + " Organization Testing " + i.ToString();
                Organization org = MockOrganizations.CreateOrganization(db, orgName);
                orgList.Add(org);
            }
            
            int? programId = 0;
            int? divisionId = 0;
            int? typeId = 0;
            int? campusId = 0;
            int? scheduleId = 0;
            int? statusId = 30;
            int? onlineReg = -1;
            int? userId = db.Users.Where(u => u.Username == "Admin").FirstOrDefault().UserId;
            int? tagDiv = null;

            int expectedCount = orgList.Count();

            var actualCount = db.OrgSearch(searchString, programId, divisionId, typeId, campusId, scheduleId, statusId, onlineReg, userId, tagDiv);

            Assert.Equal(expectedCount, actualCount.Count());

            foreach(var item in orgList)
            {
                MockOrganizations.DeleteOrganization(db, item);
            }
        }

        [Fact]
        public void Should_CopySettings2()
        {
            var org = new Organization
            {
                OrganizationName = "Org",
                Description = "TestDescription",
                BirthDayStart = new DateTime(1990, 6, 22),
                BirthDayEnd = new DateTime(2000, 6, 22),
                RegSetting = "This are reg settings"
            };

            var org2 = new Organization
            {
                OrganizationName = "Org Copied",
            };

            Organization.CopySettings2(org, org2);

            org2.Description.ShouldBe(org.Description);
            org2.BirthDayStart.ShouldBe(org.BirthDayStart);
            org2.BirthDayEnd.ShouldBe(org.BirthDayEnd);
            org2.RegSetting.ShouldBe(org.RegSetting);
        }
    }
}
