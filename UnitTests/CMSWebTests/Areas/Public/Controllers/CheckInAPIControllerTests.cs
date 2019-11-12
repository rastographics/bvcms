using CmsData;
using CmsWeb.Areas.Public.Controllers;
using CmsWeb.CheckInAPI;
using CmsWeb.Membership;
using CMSWebTests;
using CMSWebTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;

namespace CmsWeb.Areas.Public.ControllersTests
{
    [Collection(Collections.Database)]
    public class CheckInAPIControllerTests : ControllerTestBase
    {
        [Theory]
        [InlineData("true")]
        [InlineData("false")]
        public void RecordAttendTest(string setting)
        {
            var now = DateTime.Now;
            db.SetSetting("AttendCountUpdatesOffline", setting);
            db.SubmitChanges();
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

            var controller = new CheckInAPIController(requestManager);
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
            var result = controller.RecordAttend(json) as CheckInMessage;

            db.DeleteSetting("AttendCountUpdatesOffline");

            result.error.ShouldBe(0);
            result.count.ShouldBe(1);
        }
    }
}
