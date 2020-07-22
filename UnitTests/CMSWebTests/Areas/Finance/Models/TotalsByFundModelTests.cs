using CmsData;
using CmsWeb.Membership;
using CmsWeb.Models;
using CMSWebTests.Support;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CMSWebTests.Areas.Finance.Models
{
    [Collection(Collections.Database)]
    public class TotalsByFundModelTests : ControllerTestBase
    {
        [Fact]
        public void Should_Not_Return_CustomReports_For_Finance_Role()
        {
            var username = RandomString();
            var password = RandomString();
            CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            var requestManager = FakeRequestManager.Create();
            var db = requestManager.CurrentDatabase;
            requestManager.CurrentHttpContext.Request.Headers["Authorization"] = BasicAuthenticationString(username, password);
            var membershipProvider = new MockCMSMembershipProvider { ValidUser = true };
            var roleProvider = new MockCMSRoleProvider();
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);

            var content = new Content
            {
                Name = RandomString(),
                Title = RandomString(),
                Body = "--class=TotalsByFund --Roles=Finance",
                DateCreated = DateTime.Now
            };

            db.Contents.InsertOnSubmit(content);
            db.SubmitChanges();

            var totalsByFundModel = new TotalsByFundModel(db);
            var reports = totalsByFundModel.CustomReports();

            foreach (var item in reports)
            {
                var reportBody = db.Contents.SingleOrDefault(c => c.Name == item).Body;
                reportBody.ShouldNotContain("--Roles=Finance");
            }

            db.Contents.DeleteOnSubmit(content);
            db.SubmitChanges();
        }
    }
}
