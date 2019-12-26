using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmsWeb.Areas.Public.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using CMSWebTests;
using SharedTestFixtures;
using CmsWeb.Areas.Public.Models.MobileAPIv2;
using Shouldly;
using CMSWebTests.Support;
using CmsWeb.Membership;
using CmsWeb.MobileAPI;
using Newtonsoft.Json;
using CmsData.Codes;
using CmsData;

namespace CmsWeb.Areas.Public.ControllersTests
{
    [TestClass()]
    [Collection(Collections.Database)]
    public class MobileAPIv2ControllerTests : ControllerTestBase
    {
        [Fact]
        public void FetchInvolvementTest()
        {
            var username = RandomString();
            var password = RandomString();
            var user = CreateUser(username, password);
            var requestManager = FakeRequestManager.Create();
            var membershipProvider = new MockCMSMembershipProvider { ValidUser = true };
            var roleProvider = new MockCMSRoleProvider();
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);
            db.OrganizationMembers.InsertOnSubmit(new OrganizationMember
            {
                Organization = db.Organizations.First(),
                Person = user.Person,
                MemberTypeId = MemberTypeCode.Member
            });
            db.SubmitChanges();
            requestManager.CurrentHttpContext.Request.Headers["Authorization"] = BasicAuthenticationString(username, password);
            var controller = new MobileAPIv2Controller(requestManager);
            var message = new MobileMessage
            {
                argInt = user.PeopleId.Value
            };
            var data = message.ToString();
            var result = controller.FetchInvolvement(data) as MobileMessage;
            result.ShouldNotBeNull();
            result.count.ShouldBe(1);
            result.error.ShouldBe(0);
            var orgs = JsonConvert.DeserializeObject<List<MobileInvolvement>>(result.data);
            orgs.Count.ShouldBe(1);
        }

        [Theory]
        [InlineData(0, "0", "No items found", "$0.00", "0")]
        [InlineData(3.33, "1", "", "$3.33", "1")]
        public void FetchGivingSummaryTest(decimal contribution, string count, string comment, string total, string contribCount)
        {
            var username = RandomString();
            var password = RandomString();
            var user = CreateUser(username, password);
            var requestManager = FakeRequestManager.Create();
            var membershipProvider = new MockCMSMembershipProvider { ValidUser = true };
            var roleProvider = new MockCMSRoleProvider();
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);
            requestManager.CurrentHttpContext.Request.Headers["Authorization"] = BasicAuthenticationString(username, password);
            var year = DateTime.Now.Year;
            if (contribution > 0)
            {
                var c = new Contribution() {
                    PeopleId = user.PeopleId,
                    ContributionAmount = contribution,
                    ContributionDate = new DateTime(year, 7, 2),
                    ContributionStatusId = ContributionStatusCode.Recorded,
                    ContributionTypeId = ContributionTypeCode.Online,
                    CreatedDate = DateTime.Now,
                    FundId = 1
                };
                db.Contributions.InsertOnSubmit(c);
                db.SubmitChanges();
            }
            var controller = new MobileAPIv2Controller(requestManager);
            var message = new MobileMessage
            {
                argInt = 0
            };
            var data = message.ToString();
            var result = controller.FetchGivingSummary(data) as MobileMessage;
            result.ShouldNotBeNull();
            result.count.ShouldBe(1);
            result.error.ShouldBe(0);
            var summary = JsonConvert.DeserializeObject<MobileGivingSummary>(result.data);
            summary.Count.ShouldBe(1);
            var current = summary[$"{year}"];
            current.ShouldNotBeNull();
            current.title.ShouldBe($"{year}");
            current.comment.ShouldBe(comment);
            current.count.ShouldBe(count);
            current.loaded.ShouldBe(1);
            current.total.ShouldBe(total);
            current.summary[0].title.ShouldBe("Contributions");
            current.summary[0].comment.ShouldBe(comment);
            current.summary[0].count.ShouldBe(contribCount);
            current.summary[0].showAsPledge.ShouldBe(0);
        }
    }
}
