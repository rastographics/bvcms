using CmsWeb.Areas.People.Models;
using CmsWeb.Lifecycle;
using CmsWeb.Models.ExtraValues;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "Contact2"), Route("{action}/{cid:int}")]
    public class ContactController : CmsStaffController
    {
        public ContactController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/Contact2/{cid}")]
        public ActionResult Index(int cid, bool edit = false)
        {
            var m = new ContactModel(cid);
            if (m.contact == null)
            {
                return Content("contact is private or does not exist");
            }

            if (edit)
            {
                TempData["ContactEdit"] = true;
                return Redirect($"/Contact2/{cid}");
            }
            else
            {
                if (TempData.ContainsKey("SetRole"))
                {
                    m.LimitToRole = TempData["SetRole"].ToString();
                }

                var showEdit = (bool?)TempData["ContactEdit"] == true;
                ViewBag.edit = showEdit;
                return View(m);
            }
        }

        [HttpPost, Route("RemoveContactee/{cid:int}/{pid:int}")]
        public ActionResult RemoveContactee(int cid, int pid)
        {
            var m = new ContacteesModel(cid);
            m.RemoveContactee(pid);
            return Content("ok");

        }

        [HttpPost, Route("RemoveContactor/{cid:int}/{pid:int}")]
        public ActionResult RemoveContactor(int cid, int pid)
        {
            var m = new ContactorsModel(cid);
            if (m.Contact != null)
            {
                m.RemoveContactor(pid);
            }

            return Content("ok");
        }

        [HttpPost]
        public ActionResult Contactees(int cid)
        {
            var m = new ContactModel(cid);
            return View(m.MinisteredTo);
        }
        [HttpPost]
        public ActionResult Contactors(int cid)
        {
            var m = new ContactModel(cid);
            return View(m.Ministers);
        }
        [HttpPost]
        public ActionResult ContactEdit(int cid)
        {
            var m = new ContactModel(cid);
            if (!m.CanViewComments)
            {
                return View("ContactDisplay", m);
            }

            return View(m);
        }
        [HttpPost]
        public ActionResult ContactDisplay(int cid)
        {
            var m = new ContactModel(cid);
            return View(m);
        }
        [HttpGet]
        public ActionResult ConvertContacteesToQuery(int cid)
        {
            Response.NoCache();
            var m = new ContacteesModel(cid);
            var gid = m.ConvertToQuery();
            return Redirect($"/Query/{gid}");
        }
        [HttpGet]
        public ActionResult ConvertContactorsToQuery(int cid)
        {
            Response.NoCache();
            var m = new ContactorsModel(cid);
            var gid = m.ConvertToQuery();
            return Redirect($"/Query/{gid}");
        }
        [HttpPost]
        public ActionResult ContactUpdate(int cid, ContactModel c)
        {
            c.SetLocationOnContact();
            if (!ModelState.IsValid)
            {
                return View("ContactEdit", c);
            }

            c.UpdateContact();
            return View("ContactDisplay", c);
        }
        [HttpPost]
        public ActionResult ContactDelete(int cid)
        {
            ContactModel.DeleteContact(cid);
            return Redirect("/ContactSearch2");
        }
        [HttpPost]
        public ActionResult NewTeamContact(int cid)
        {
            var m = new ContactModel(cid);
            var nid = m.AddNewTeamContact();
            return Redirect("/Contact2/" + nid);
        }
        [HttpPost, Route("AddTask/{cid:int}/{pid:int}")]
        public ActionResult AddTask(int cid, int pid)
        {
            var m = new ContacteesModel(cid);
            var tid = m.AddTask(pid);
            return Redirect("/Task/" + tid);
        }
        [HttpPost]
        public ActionResult ExtraValues(int cid, string ministry, string contactType, string contactReason)
        {
            var m = new ContactModel(cid);
            m.SetLocationOnContact(ministry, contactType, contactReason);
            var evmodel = new ExtraValueModel(cid, "Contact", m.Location);
            return View("/Views/ExtraValue/Location.cshtml", evmodel);
        }
    }
}
