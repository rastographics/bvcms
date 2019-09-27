using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Lifecycle;
using Newtonsoft.Json;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix = "")]
    public class MissionTripEmailController : CmsStaffController
    {
        public MissionTripEmailController(IRequestManager requestManager) : base(requestManager)
        {
            NoCheckRole = true;
        }

        [HttpGet, Route("MissionTripEmail2/{oid}/{pid}")]
        public ActionResult Index(int oid, int pid)
        {
            if (Util.UserPeopleId != pid)
            {
                return Content("not authorized");
            }
            DbUtil.LogActivity($"MissionTripEmail {pid}");
            var m = new MissionTripEmailer {PeopleId = pid, OrgId = oid};
            return View(m);
        }

        [HttpGet, Route("MissionTripEmail2/EmailBody/{oid}/{pid}")]
        public ActionResult EmailBody(int oid, int pid)
        {
            if (Util.UserPeopleId != pid)
            {
                return Content("not authorized");
            }
            var m = new MissionTripEmailer {PeopleId = pid, OrgId = oid};
            return View(m);
        }

        [HttpGet, Route("MissionTripEmail2/Sent")]
        public ActionResult Sent()
        {
            return View();
        }

        [HttpPost, Route("MissionTripEmail2/Send")]
        [ValidateInput(false)]
        public ActionResult Send(MissionTripEmailer m)
        {
            var s = m.Send();
            if (s == null)
                return Content("/MissionTripEmail2/Sent");
            return Content(s);
        }

        [HttpPost, Route("MissionTripEmail2/TestSend")]
        [ValidateInput(false)]
        public ActionResult TestSend(MissionTripEmailer m)
        {
            var s = m.TestSend();
            return Content(s);
        }

        [HttpPost, Route("MissionTripEmail2/Search/{id:int}")]
        public ActionResult SupportSearch(int id, string q)
        {
            var qq = MissionTripEmailer.Search(id, q).ToArray();
            return Content(JsonConvert.SerializeObject(qq));
        }

        [HttpPost, Route("MissionTripEmail2/Supporters/{id:int}")]
        public ActionResult Supporters(int id)
        {
            var m = new MissionTripEmailer {PeopleId = id};
            return View(m);
        }

        [HttpPost, Route("MissionTripEmail2/SupportersEdit/{id:int}")]
        public ActionResult SupportersEdit(int id)
        {
            var m = new MissionTripEmailer {PeopleId = id};
            return View(m);
        }

        [HttpPost, Route("MissionTripEmail2/SupportersUpdate")]
        [ValidateInput(false)]
        public ActionResult SupportersUpdate(MissionTripEmailer m)
        {
            m.UpdateRecipients();
            return View("Supporters", m);
        }

        [HttpPost, Route("MissionTripEmail2/RemoveSupporter/{id:int}/{supporterid:int}")]
        public ActionResult RemoveSupporter(int id, int supporterid)
        {
            var m = new MissionTripEmailer {PeopleId = id};
            m.RemoveSupporter(supporterid);
            return View("SupportersEdit", m);
        }

        [HttpPost, Route("MissionTripEmail2/AddSupporter/{id:int}/{supporter}")]
        public ActionResult AddSupporter(int id, string supporter)
        {
            int? supporterid = null;
            string email = null;
            if (supporter.AllDigits())
                supporterid = supporter.ToInt();
            else
                email = supporter;
            var m = new MissionTripEmailer {PeopleId = id};
            ViewBag.newid = m.AddRecipient(supporterid, email);
            return View("Supporters", m);
        }
    }
}
