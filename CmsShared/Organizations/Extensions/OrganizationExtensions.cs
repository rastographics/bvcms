using CmsData;
using eSpace;
using System.Collections.Specialized;
using System.Linq;

namespace CMSShared.Organizations.Extensions
{
    public static class OrganizationExtensions
    {
        public static void SyncWithESpace(this Organization org, CMSDataContext db)
        {
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
                    var meeting = org.Meetings.Where(m => m.MeetingExtras.Any(e => e.Field == "eSPACE_ID" && e.Data == occurrenceId)).FirstOrDefault()
                        ?? Meeting.FetchOrCreateMeeting(db, id, occurrence.EventStart);
                    meeting.Location = string.Join("\n", occurrence.Items.Where(i => i.ItemType == "Space").Select(i => i.Name));
                    meeting.AddEditExtraText("eSPACE_ID", occurrenceId);
                }
                db.SubmitChanges();
            }
        }
    }
}
