using CmsData;
using eSpace;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace CmsShared.Organizations.Extensions
{
    public static class EspaceExtensions
    {
        public static void SyncWithESpace(this Organization org, CMSDataContext db)
        {
            const string extraValueField = "eSPACE_ID";
            if (org.ESpaceEventId.HasValue)
            {
                var espace = new eSpaceClient
                {
                    Username = db.Setting("eSpaceUserName", ""),
                    Password = db.Setting("eSpacePassword", "")
                };
                var id = org.OrganizationId;
                var daysToSync = db.Setting("eSpaceDaysToSync", "60");
                var list = espace.Event.Occurrences(org.ESpaceEventId.Value, new NameValueCollection
                {
                    { "nextDays", daysToSync }
                });

                foreach (var occurrence in list)
                {
                    var occurrenceId = occurrence.OccurrenceId.ToString();
                    var meeting = db.MeetingExtras
                        .Where(m => m.Meeting.OrganizationId == org.OrganizationId)
                        .Where(m => m.Field == extraValueField && m.Data == occurrenceId)
                        .Select(m => m.Meeting).FirstOrDefault();
                    if (meeting == null)
                    {
                        meeting = Meeting.FetchOrCreateMeeting(db, id, occurrence.EventStart);
                    }
                    meeting.Location = string.Join("\n", occurrence.Items.Where(i => i.ItemType == "Space").Select(i => i.Name));
                    meeting.AddEditExtraText(extraValueField, occurrenceId);
                }
                db.SubmitChanges();

                var current = list.Select(o => o.OccurrenceId.ToString());
                var meetingsToDelete = db.MeetingExtras
                    .Where(e => e.Meeting.OrganizationId == org.OrganizationId)
                    .Where(e => e.Field == extraValueField && e.Data.Length > 0)
                    .Where(e => !current.Contains(e.Data))
                    .Where(e => e.Meeting.MeetingDate > DateTime.Now)
                    .Select(m => m.MeetingId)
                    .ToList();
                foreach (var m in meetingsToDelete)
                {
                    DeleteMeeting(db, m);
                }
            }
        }

        private static void DeleteMeeting(CMSDataContext db, int meetingId)
        {
            db.ExecuteCommand(@"DELETE dbo.MeetingExtra WHERE MeetingId={0};
                DELETE dbo.Meetings WHERE MeetingId={0}", meetingId);
        }
    }
}
