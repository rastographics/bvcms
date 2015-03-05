using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;
using CmsData.Codes;
using CmsWeb.Areas.Organization.Models;
using CmsWeb.Code;
using CmsWeb.Controllers;

namespace CmsWeb.Areas.Organization.Controllers
{
    public partial class OrganizationController
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