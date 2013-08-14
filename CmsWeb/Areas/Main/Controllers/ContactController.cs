using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Models.ContactPage;
using iTextSharp.text.pdf.fonts.cmaps;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaUrl = "Contact")]
    public class ContactController : CmsStaffController
    {
        [GET("Contact/{id}")]
        public ActionResult Index(int id)
        {
            var m = new ContactModel(id);
            return View(m);
        }

        [POST("Contact/RemoveContactee/{cid:int}/{pid:int}")]
        public ActionResult RemoveContactee(int cid, int pid)
        {
            var m = new ContacteesModel(cid);
            m.RemoveContactee(pid);
            return Content("ok");

        }
        [POST("Contact/RemoveContactor/{cid:int}/{pid:int}")]
        public ActionResult RemoveContactor(int cid, int pid)
        {
            var m = new ContactorsModel(cid);
            m.RemoveContactor(pid);
            return Content("ok");
        }

        [POST("Contact/Contactees/{cid:int}")]
        public ActionResult Contactees(int cid)
        {
            var m = new ContacteesModel(cid);
            return View(m);
        }
        [POST("Contact/Contactors/{cid:int}")]
        public ActionResult Contactors(int cid)
        {
            var m = new ContactorsModel(cid);
            return View(m);
        }
        [POST("Contact/ContactEdit/{cid:int}")]
        public ActionResult ContactEdit(int cid)
        {
            var m = new ContactModel(cid);
            return View(m);
        }
        [POST("Contact/ContactDisplay/{cid:int}")]
        public ActionResult ContactDisplay(int cid)
        {
            var m = new ContactModel(cid);
            return View(m);
        }
        [POST("Contact/ContactUpdate/{cid:int}")]
        public ActionResult ContactUpdate(int cid, ContactModel c)
        {
            var m = new ContactModel(cid);
            m.UpdateContact(c.contact);
            return View("ContactDisplay", m);
        }
        [POST("Contact/ContactDelete/{cid:int}")]
        public ActionResult ContactDelete(int cid)
        {
            var m = new ContactModel(cid);
            m.DeleteContact();
            return Redirect("/ContactSearch");
        }
        [POST("Contact/NewTeamContact/{cid:int}")]
        public ActionResult NewTeamContact(int cid)
        {
            var m = new ContactModel(cid);
            var nid = m.AddNewTeamContact();
            return Redirect("/Contact/" + nid);
        }
        [POST("Contact/AddTask/{cid:int}/{pid:int}")]
        public ActionResult AddTask(int cid, int pid)
        {
            var m = new ContacteesModel(cid);
            var tid = m.AddTask(pid);
            return Redirect("/Task/List/" + tid);
        }
    }
}
