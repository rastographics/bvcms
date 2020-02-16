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
                    var meeting = org.Meetings.Where(m => m.MeetingExtras.Any(e => e.Field == extraValueField && e.Data == occurrenceId)).FirstOrDefault()
                        ?? Meeting.FetchOrCreateMeeting(db, id, occurrence.EventStart);
                    meeting.Location = string.Join("\n", occurrence.Items.Where(i => i.ItemType == "Space").Select(i => i.Name));
                    meeting.AddEditExtraText(extraValueField, occurrenceId);
                }
                db.SubmitChanges();

                var current = list.Select(o => o.OccurrenceId.ToString());
                var meetingsToDelete = org.Meetings
                    .Where(m => m.MeetingDate > DateTime.Now)
                    .Where(m => m.MeetingExtras.Any(e => e.Field == extraValueField && !string.IsNullOrEmpty(e.Data)))
                    .Where(m => !current.Contains(m.MeetingExtras.Single(e => e.Field == extraValueField).Data))
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
