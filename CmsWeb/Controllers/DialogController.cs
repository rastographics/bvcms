using System;
using System.Web.Mvc;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Code;

namespace CmsWeb.Controllers
{
    public class DialogController : Controller
    {
        public class Options
        {
            public bool useMailFlags { get; set; }
        }

        public class NewMeetingInfo
        {
            public CodeInfo Schedule { get; set; }
            public CodeInfo AttendCredit { get; set; }
            public DateTime? MeetingDate { get; set; }
            public bool ByGroup { get; set; }
            public string GroupFilterPrefix { get; set; }
            public string HighlightGroup { get; set; }
            public bool UseAltNames { get; set; }
        }
        public ActionResult ChooseFormat(string id)
        {
            var m = new Options() {useMailFlags = id == "useMailFlags"};
            return View(m);
        }
        public ActionResult NewMeeting(int id, bool forMeeting)
        {
            var oi = new OrganizationModel(id);
            var m = new NewMeetingInfo()
            {
                Schedule = new CodeInfo(0, forMeeting ? oi.SchedulesPrev() : oi.SchedulesNext()),
                AttendCredit = new CodeInfo(0, oi.AttendCreditList())
            };
            return View(m);
        }

        public ActionResult TagAll()
        {
            return View();
        }

        public ActionResult DeleteStandardExtra()
        {
            return View();
        }

        public ActionResult GetExtraValue()
        {
            return View();
        }

    }
}
