using CmsData;
using CmsWeb.Areas.Public.Controllers;
using CmsWeb.Areas.Public.Models.CheckInAPIv2;
using CmsWeb.Membership;
using CMSWebTests;
using CMSWebTests.Support;
using SharedTestFixtures;
using System.Web.Mvc;
using Shouldly;
using System;
using Xunit;

namespace CmsWeb.Areas.Public.ControllersTests
{
    [Collection(Collections.Database)]
    public class CheckInAPIv2ControllerTests : ControllerTestBase
    {
        [Fact]
        public void RecordAttendanceTest()
        {
            var now = DateTime.Now;
            var username = RandomString();
            var password = RandomString();
            var user = CreateUser(username, password);
            var org = CreateOrganization();
            org.FirstMeetingDate = now.AddDays(-7);
            org.RollSheetVisitorWks = 2;
            org.AllowAttendOverlap = true;
            org.CanSelfCheckin = true;
            org.NoSecurityLabel = true;
            org.NumCheckInLabels = 0;
            db.SubmitChanges();
            var meetingTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute - (now.Minute % 15), 0);
            Meeting.FetchOrCreateMeeting(db, org.OrganizationId, meetingTime, noautoabsents: true);

            var requestManager = FakeRequestManager.Create();
            var context = requestManager.CurrentHttpContext;
            var membershipProvider = new MockCMSMembershipProvider { ValidUser = true };
            var roleProvider = new MockCMSRoleProvider();
            roleProvider.UserRoles.Add("Checkin");
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);
            context.Request.Headers["Authorization"] = BasicAuthenticationString(username, password);

            var controller = new CheckInAPIv2Controller(requestManager);
            var json = $@"{{
    ""rebranded"": ""false"",
    ""argInt"": ""0"",
    ""version"": ""2"",
    ""id"": ""0"",
    ""data"": ""{{\""orgID\"":{org.OrganizationId},\""present\"":true,\""datetime\"":\""{meetingTime:s}\"",\""peopleID\"":{user.PeopleId}}}"",
    ""device"": ""1"",
    ""argString"": """",
    ""count"": ""0""
}}";
            Message result = controller.UpdateAttend(json) as Message;
            
            result.error.ShouldBe(0);
            result.count.ShouldBe(1);
        }
    }
}
