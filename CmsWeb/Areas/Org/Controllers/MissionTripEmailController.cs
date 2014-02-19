using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsData.Codes;
using CmsWeb.Areas.Main.Models;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Areas.Org.Models;
using DocumentFormat.OpenXml.EMMA;
using Newtonsoft.Json;
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
            DbUtil.LogActivity("MissionTripEmail {0}".Fmt(pid));
            var m = new MissionTripEmailer {PeopleId = pid, OrgId = oid};
            return View(m);
        }

        [POST("MissionTripEmail/Send")]
        [ValidateInput(false)]
        public ActionResult Send(MissionTripEmailer m)
        {
            var s = m.Send();
            return Content("");
        }
        [POST("MissionTripEmail/TestSend")]
        [ValidateInput(false)]
        public ActionResult TestSend(MissionTripEmailer m)
        {
            var s = m.TestSend();
            return Content(s);
        }


        [POST("MissionTripEmail/Search/{id:int}")]
        public ActionResult SupportSearch(int id, string q)
        {
            var qq = MissionTripEmailer.Search(id, q).ToArray();
            return Content(JsonConvert.SerializeObject(qq));
        }

        [POST("MissionTripEmail/Supporters/{id:int}")]
        public ActionResult Supporters(int id)
        {
            var m = new MissionTripEmailer() {PeopleId = id};
            return View(m);
        }
        [POST("MissionTripEmail/SupportersEdit/{id:int}")]
        public ActionResult SupportersEdit(int id)
        {
            var m = new MissionTripEmailer() {PeopleId = id};
            return View(m);
        }
        [POST("MissionTripEmail/SupportersUpdate")]
        [ValidateInput(false)]
        public ActionResult SupportersUpdate(MissionTripEmailer m)
        {
            m.UpdateRecipients();
            return View("Supporters", m);
        }
        [POST("MissionTripEmail/RemoveSupporter/{id:int}/{supporterid:int}")]
        public ActionResult RemoveSupporter(int id, int supporterid)
        {
            var m = new MissionTripEmailer() {PeopleId = id};
            m.RemoveSupporter(supporterid);
            return View("SupportersEdit", m);
        }
        [POST("MissionTripEmail/AddSupporter/{id:int}/{supporter}")]
        public ActionResult AddSupporter(int id, string supporter)
        {
            int? supporterid = null;
            string email = null;
            if (supporter.AllDigits())
                supporterid = supporter.ToInt();
            else
                email = supporter;
            var m = new MissionTripEmailer() {PeopleId = id};
            ViewBag.newid = m.AddRecipient(supporterid, email);
            return View("Supporters", m);
        }
    }
}
