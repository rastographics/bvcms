using CmsData;
using CmsWeb.Areas.Org.Models;
using eSpace;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        [HttpPost]
        public ActionResult Meetings(MeetingsModel m)
        {
            //var m = new MeetingsModel(id, future ?? false);
            DbUtil.LogActivity($"Viewing Meetings for orgId={m.Id}", orgid: m.Id);
            return PartialView(m);
        }

        [Route("GotoMeetingForDate/{oid:int}/{ticks:long}")]
        public ActionResult GotoMeetingForDate(int oid, long ticks)
        {
            var dt = new DateTime(ticks); // ticks here is meeting time
            var q = from m in CurrentDatabase.Meetings
                    where m.OrganizationId == oid
                    where m.MeetingDate == dt
                    select m;
            var meeting = q.FirstOrDefault();
            if (meeting != null)
            {
                return Redirect("/Meeting/" + meeting.MeetingId);
            }

            return Message("no meeting at " + dt.FormatDateTm());
        }

        [HttpPost]
        [Route("ImportMeetings/{id:int}")]
        public ActionResult ImportMeetings(MeetingsModel m)
        {
            eSpaceClient client = new eSpaceClient
            {
                Username = CurrentDatabase.Setting("eSpaceUserName", ""),
                Password = CurrentDatabase.Setting("eSpacePassword", "")
            };
            var org = CurrentDatabase.Organizations.Where(o => o.OrganizationId == m.Id).First();
            if (org.ESpaceEventId.HasValue)
            {
                var daysToSync = CurrentDatabase.Setting("eSpaceDaysToSync", "60");
                DbUtil.LogActivity($"Retrieving meetings for next {daysToSync} days (orgId={m.Id})", orgid: m.Id);
                var list = client.Event.Occurrences(org.ESpaceEventId.Value, new NameValueCollection
                {
                    { "nextDays", daysToSync }
                });
                foreach(var occurrence in list)
                {
                    Meeting.FetchOrCreateMeeting(CurrentDatabase, m.Id, occurrence.EventStart);
                }
                m.Future = true;
            }
            return PartialView("Meetings", m);
        }
    }
}
