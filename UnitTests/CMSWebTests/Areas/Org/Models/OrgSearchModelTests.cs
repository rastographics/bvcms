using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmsData.View;
using Shouldly;
using CmsWeb.Areas.Search.Models;
using SharedTestFixtures;
using Xunit;

namespace CMSWebTests.Areas.Org.Models
{
    [Collection(Collections.Database)]
    public class OrgSearchModelTests : DatabaseTestBase
    {
        [Fact]
        public void OrgSearchSortModelTest()
        {
            var orgsearch = new OrgSearchModel(db) { StatusId = 30, OnlineReg = -1 };
            var username = RandomString();
            var password = RandomString();
            var user = CreateUser(username, password,
                roles: new string[] { "Access", "Edit", "Admin", "Finance" });
            db.CurrentUser = user;

            var orgs = orgsearch.OrganizationList().ToList();
            orgsearch.Pager.Sort.ShouldBe("Name");
            for (var i = 1; i < orgs.Count; i++)
            {
                var name = orgs[i].OrganizationName;
                var prevname = orgs[i - 1].OrganizationName;
                name.ShouldBeGreaterThan(prevname);
            }
        }
    }
}
