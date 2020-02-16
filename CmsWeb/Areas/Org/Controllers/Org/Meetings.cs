using CmsData;
using CmsShared.Organizations.Extensions;
using CmsWeb.Areas.Org.Models;
using System;
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
            DbUtil.LogActivity($"Viewing Meetings for orgId={m.Id}", orgid: m.Id);
            return PartialView(m);
        }

        [Route("GotoMeetingForDate/{oid:int}/{ticks:long}")]
        public ActionResult GotoMeetingForDate(int oid, long ticks)
        {
            var dt = new DateTime(ticks);
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
        public ActionResult ImportMeetings(MeetingsModel m)
        {
            var org = CurrentDatabase.Organizations.Where(o => o.OrganizationId == m.Id).First();
            if (org.ESpaceEventId.HasValue)
            {
                DbUtil.LogActivity($"Retrieving meetings for (orgId={m.Id})", orgid: m.Id);
                org.SyncWithESpace(CurrentDatabase);
                m.Future = true;
            }
            return PartialView("Meetings", m);
        }
    }
}
