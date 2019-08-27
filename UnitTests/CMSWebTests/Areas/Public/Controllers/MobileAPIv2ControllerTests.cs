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
    [Collection(Collections.Database)]
    public class MobileAPIv2ControllerTests : ControllerTestBase
    {
        [Fact]
        public void FetchInvolvementTest()
        {
            var username = RandomString();
            var password = RandomString();
            var user = CreateUser(username, password);
            var requestManager = new FakeRequestManager();
            var membershipProvider = new MockCMSMembershipProvider { ValidUser = true };
            var roleProvider = new MockCMSRoleProvider();
            db.OrganizationMembers.InsertOnSubmit(new OrganizationMember
            {
                Organization = db.Organizations.First(),
                Person = user.Person,
                MemberTypeId = MemberTypeCode.Member
            });
            db.SubmitChanges();
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);
            ContextTestUtils.Headers["Authorization"] = BasicAuthenticationString(username, password);
            var controller = new MobileAPIv2Controller(requestManager);
            var message = new MobileMessage {
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
    }
}
