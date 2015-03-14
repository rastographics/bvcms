using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;
using CmsWeb.Areas.Org2.Models;

namespace CmsWeb.Areas.Org2.Controllers
{
    public partial class Org2Controller
    {
        [HttpPost]
        public ActionResult Meetings(MeetingsModel m)
        {
            //var m = new MeetingsModel(id, future ?? false);
            DbUtil.LogActivity("Viewing Meetings for {0}".Fmt(Session["ActiveOrganization"]));
            return PartialView(m);
        }
        [Route("GotoMeetingForDate/{oid:int}/{ticks:long}")]
        public ActionResult GotoMeetingForDate(int oid, long ticks)
        {
			var dt = new DateTime(ticks); // ticks here is meeting time
            var q = from m in DbUtil.Db.Meetings
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