using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        [HttpPost]
        public ActionResult Meetings(MeetingsModel m)
        {
            //var m = new MeetingsModel(id, future ?? false);
            DbUtil.LogActivity($"Viewing Meetings for {Session["ActiveOrganization"]}");
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
                return Redirect("/Meeting/" + meeting.MeetingId);
            return Message("no meeting at " + dt.FormatDateTm());
        }
    }
}
