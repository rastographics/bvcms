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
            for (int i = 0; i < count; i++)
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

            var actualCount = db.OrgSearch(null, searchString, programId, divisionId, typeId, campusId, scheduleId, statusId, onlineReg, userId, tagDiv);

            Assert.Equal(expectedCount, actualCount.Count());

            foreach (var item in orgList)
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

        [Fact]
        public void Should_Ticketing_Attendance_Exclude_Deceased_People()
        {
            var org = db.Organizations.Where(o => o.OrganizationName.Equals("Online Giving")).FirstOrDefault();
            int orgId = org.OrganizationId;

            var rollList = db.RollList(1, DateTime.Now, orgId, false, false);

            foreach (var row in rollList)
            {
                var personId = row.PeopleId;

                Person person = db.People.Where(p => p.PeopleId == personId).FirstOrDefault();

                Assert.True(person.IsDeceased == false);
            }

            var rollListHighlight = db.RollListHighlight(1, DateTime.Now, orgId, false, "");

            foreach (var row in rollListHighlight)
            {
                var personId = row.PeopleId;

                Person person = db.People.Where(p => p.PeopleId == personId).FirstOrDefault();

                Assert.True(person.IsDeceased == false);
            }

            var rollListFilteredBySubgroups = db.RollListFilteredBySubgroups(1, DateTime.Now, orgId, false, false, "", false);

            foreach (var row in rollListFilteredBySubgroups)
            {
                var personId = row.PeopleId;

                Person person = db.People.Where(p => p.PeopleId == personId).FirstOrDefault();

                Assert.True(person.IsDeceased == false);
            }
        }
    }
}
