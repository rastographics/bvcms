using Xunit;
using SharedTestFixtures;
using SharedTestFixtures.Network;
using System;
using System.Text;
using CmsSharedTests.Properties;
using CmsData;
using CmsShared.Organizations.Extensions;
using System.Linq;
using Shouldly;
using System.Data.Linq;

namespace CmsShared.Organizations.ExtensionsTests
{
    [Collection(Collections.Database)]
    public class EspaceExtensionsTests : DatabaseTestBase
    {
        private NetworkFixture network;

        public EspaceExtensionsTests() : base()
        {
            MockAppSettings.Apply(("eSpaceEventService.BaseUrl", NetworkFixture.ProxyUrl));
            network = new NetworkFixture();
        }

        [Fact]
        public void SyncWithESpaceTest()
        {
            var organization = Organization.CreateOrganization(db, 1, RandomString());
            organization.ESpaceEventId = 874491;
            db.SubmitChanges();

            var meeting = Meeting.FetchOrCreateMeeting(db, organization.OrganizationId, new DateTime(2020, 2, 20, 18, 0, 0));
            meeting.AddEditExtraText("eSPACE_ID", "7392517");
            db.SubmitChanges();

            NetworkFixture.MockResponse($"/event/occurrences\\?nextDays=60&eventId=874491", new MockHttpResponse
            {
                Headers = NetworkFixture.JsonHeaders,
                ResponseBody = Encoding.Default.GetString(Resources.SyncWithESpaceTestResponse)
            });

            // Tests that we pulled in a new meeting
            organization.SyncWithESpace(db);
            var meetingCount = db.Meetings.Count(m => m.OrganizationId == organization.OrganizationId);
            meetingCount.ShouldBe(2);

            // Tests that we updated an existing meeting
            db.Refresh(RefreshMode.OverwriteCurrentValues, meeting);
            meeting.Location.ShouldBe("Conference Room\nE151");

            NetworkFixture.MockResponse($"/event/occurrences\\?nextDays=60&eventId=874491", new MockHttpResponse
            {
                Headers = NetworkFixture.JsonHeaders,
                ResponseBody = Encoding.Default.GetString(Resources.SyncWithESpaceTestDeletedResponse)
            });

            // Tests that we deleted a meeting in the future that no longer has an eSpace occurrence
            meeting.MeetingDate = DateTime.Now.AddDays(30);
            db.SubmitChanges();
            organization.SyncWithESpace(db);
            meetingCount = db.Copy().Meetings.Count(m => m.OrganizationId == organization.OrganizationId);
            meetingCount.ShouldBe(1);
        }

        public new void Dispose()
        {
            base.Dispose();
            network.Dispose();
            MockAppSettings.Remove("eSpaceEventService.BaseUrl");
        }
    }
}
