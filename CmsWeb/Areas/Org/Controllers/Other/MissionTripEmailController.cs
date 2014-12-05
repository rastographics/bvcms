using System.Linq;
using System.Web.Mvc;
using CmsWeb.Areas.Org.Models;
using Newtonsoft.Json;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix = "")]
    public class MissionTripEmailController : Controller
    {
        [HttpGet, Route("MissionTripEmail/{oid}/{pid}")]
        public ActionResult Index(int oid, int pid)
        {
            if (Util.UserPeopleId != pid && !User.IsInRole("MissionGiving"))
                return Content("not authorized");
            DbUtil.LogActivity("MissionTripEmail {0}".Fmt(pid));
            var m = new MissionTripEmailer {PeopleId = pid, OrgId = oid};
            return View(m);
        }
        [HttpGet, Route("MissionTripEmail/Sent")]
        public ActionResult Sent()
        {
            return View();
        }

        [HttpPost, Route("MissionTripEmail/Send")]
        [ValidateInput(false)]
        public ActionResult Send(MissionTripEmailer m)
        {
            var s = m.Send();
            if (s == null)
                return Content("/MissionTripEmail/Sent");
            return Content(s);
        }
        [HttpPost, Route("MissionTripEmail/TestSend")]
        [ValidateInput(false)]
        public ActionResult TestSend(MissionTripEmailer m)
        {
            var s = m.TestSend();
            return Content(s);
        }


        [HttpPost, Route("MissionTripEmail/Search/{id:int}")]
        public ActionResult SupportSearch(int id, string q)
        {
            var qq = MissionTripEmailer.Search(id, q).ToArray();
            return Content(JsonConvert.SerializeObject(qq));
        }

        [HttpPost, Route("MissionTripEmail/Supporters/{id:int}")]
        public ActionResult Supporters(int id)
        {
            var m = new MissionTripEmailer() {PeopleId = id};
            return View(m);
        }
        [HttpPost, Route("MissionTripEmail/SupportersEdit/{id:int}")]
        public ActionResult SupportersEdit(int id)
        {
            var m = new MissionTripEmailer() {PeopleId = id};
            return View(m);
        }
        [HttpPost, Route("MissionTripEmail/SupportersUpdate")]
        [ValidateInput(false)]
        public ActionResult SupportersUpdate(MissionTripEmailer m)
        {
            m.UpdateRecipients();
            return View("Supporters", m);
        }
        [HttpPost, Route("MissionTripEmail/RemoveSupporter/{id:int}/{supporterid:int}")]
        public ActionResult RemoveSupporter(int id, int supporterid)
        {
            var m = new MissionTripEmailer() {PeopleId = id};
            m.RemoveSupporter(supporterid);
            return View("SupportersEdit", m);
        }
        [HttpPost, Route("MissionTripEmail/AddSupporter/{id:int}/{supporter}")]
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
