using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "")]
    public class ContactController : CmsStaffController
    {
        [HttpGet, Route("Contact2/{id}")]
        public ActionResult Index(int id)
        {
            if (!ViewExtensions2.UseNewLook())
                return Redirect("/Contact/Index/" + id);
            var m = new ContactModel(id);
            if (m.contact == null)
                return Content("contact is private or does not exist");

            var edit = (bool?)TempData["ContactEdit"] == true;
            ViewBag.edit = edit;
            return View(m);
        }

        [HttpPost, Route("Contact2/RemoveContactee/{cid:int}/{pid:int}")]
        public ActionResult RemoveContactee(int cid, int pid)
        {
            var m = new ContacteesModel(cid);
            m.RemoveContactee(pid);
            return Content("ok");

        }
        [HttpPost, Route("Contact2/RemoveContactor/{cid:int}/{pid:int}")]
        public ActionResult RemoveContactor(int cid, int pid)
        {
            var m = new ContactorsModel(cid);
            if (m.Contact != null)
                m.RemoveContactor(pid);
            return Content("ok");
        }

        [HttpPost, Route("Contact2/Contactees/{cid:int}")]
        public ActionResult Contactees(int cid)
        {
            var m = new ContactModel(cid);
            return View(m.MinisteredTo);
        }
        [HttpPost, Route("Contact2/Contactors/{cid:int}")]
        public ActionResult Contactors(int cid)
        {
            var m = new ContactModel(cid);
            return View(m.Ministers);
        }
        [HttpPost, Route("Contact2/ContactEdit/{cid:int}")]
        public ActionResult ContactEdit(int cid)
        {
            var m = new ContactModel(cid);
            if (!m.CanViewComments)
                return View("ContactDisplay", m);
            return View(m);
        }
        [HttpPost, Route("Contact2/ContactDisplay/{cid:int}")]
        public ActionResult ContactDisplay(int cid)
        {
            var m = new ContactModel(cid);
            return View(m);
        }
        [HttpPost, Route("Contact2/ContactUpdate/{cid:int}")]
        public ActionResult ContactUpdate(int cid, ContactModel c)
        {
            if (!ModelState.IsValid)
                return View("ContactEdit", c);
            c.UpdateContact();
            return View("ContactDisplay", c);
        }
        [HttpPost, Route("Contact2/ContactDelete/{cid:int}")]
        public ActionResult ContactDelete(int cid)
        {
            ContactModel.DeleteContact(cid);
            return Redirect("/ContactSearch2");
        }
        [HttpPost, Route("Contact2/NewTeamContact/{cid:int}")]
        public ActionResult NewTeamContact(int cid)
        {
            var m = new ContactModel(cid);
            var nid = m.AddNewTeamContact();
            return Redirect("/Contact2/" + nid);
        }
        [HttpPost, Route("Contact2/AddTask/{cid:int}/{pid:int}")]
        public ActionResult AddTask(int cid, int pid)
        {
            var m = new ContacteesModel(cid);
            var tid = m.AddTask(pid);
            return Redirect("/Task/List/" + tid);
        }
    }
}
