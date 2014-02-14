using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsData.Codes;
using CmsWeb.Areas.Main.Models;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;
using CmsData;
using Elmah;
using System.Threading;
using Dapper;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaUrl = "MissionTripEmail")]
    public class MissionTripEmailController : Controller
    {
        [GET("MissionTripEmail/{oid}/{pid}")]
        public ActionResult Index(int oid, int pid)
        {
            DbUtil.LogActivity("Emailing people");

            var m = new MissionTripEmailer(oid, pid);

            return View(m);
        }

        [POST("MissionTripEmail/Send")]
        [ValidateInput(false)]
        public ActionResult QueueEmails(MissionTripEmailer m)
        {
            if (!m.Subject.HasValue() || !m.Body.HasValue())
                return Json(new { id = 0, content = "<h2>Both Subject and Body need some text</h2>" });
            if (!User.IsInRole("Admin") && m.Body.Contains("{createaccount}"))
                return Json(new { id = 0, content = "<h2>Only Admin can use {createaccount}</h2>" });

            if (Util.SessionTimedOut())
            {
                Session["massemailer"] = m;
                return Content("timeout");
            }

            DbUtil.LogActivity("Emailing people");

            string host = Util.Host;
            // save these from HttpContext to set again inside thread local storage
            var useremail = Util.UserEmail;
            var isinroleemailtest = User.IsInRole("EmailTest");

                    DbUtil.Db.SendPeopleEmail(0);
            return Content("");
        }
        [POST("MissionTripEmail/Send")]
        [ValidateInput(false)]
        public ActionResult TestEmail(MissionTripEmailer m)
        {
            if (Util.SessionTimedOut())
            {
                Session["massemailer"] = m;
                return Content("timeout");
            }
            var p = DbUtil.Db.LoadPersonById(Util.UserPeopleId.Value);
            try
            {
                DbUtil.Db.Email("", p, null, m.Subject, m.Body, false);
            }
            catch (Exception ex)
            {
                return Content("<h2>Error Email Sent</h2>" + ex.Message);
            }
            return Content("<h2>Test Email Sent</h2>");
        }
    }
}
