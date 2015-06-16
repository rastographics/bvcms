using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        // Membership ---------------------------------------------------

        [HttpPost]
        public ActionResult Membership(int id)
        {
            var m = new MemberInfo(id);
            return View("Profile/Membership/Display", m);
        }

        [HttpPost]
        public ActionResult MembershipEdit(int id)
        {
            var m = new MemberInfo(id);
            return View("Profile/Membership/Edit", m);
        }
        [HttpPost]
        public ActionResult JustAddedNotMember(int id)
        {
            var p = DbUtil.Db.LoadPersonById(id);
            p.MemberStatusId = MemberStatusCode.NotMember;
            DbUtil.Db.SubmitChanges();
            var m = new MemberInfo(id);
            return View("Profile/Membership/Display", m);
        }

        [HttpPost]
        public ActionResult MembershipUpdate(MemberInfo m)
        {
            var ret = m.UpdateMember();
            if (ret != "ok")
                ViewBag.AutomationError = "<div class='alert'>{0}</div>".Fmt(ret);
            if (!ModelState.IsValid || ret != "ok")
                return View("Profile/Membership/Edit", m);

            DbUtil.LogPersonActivity("Update Membership Info for: {0}".Fmt(m.person.Name), m.PeopleId, m.person.Name);
            return View("Profile/Membership/Display", m);
        }

        // Member Note ---------------------------------------------------

        [HttpPost]
        public ActionResult MemberNotes(int id)
        {
            var m = new MemberNotesModel(id);
            return View("Profile/MemberNotes/Display", m);
        }

        [HttpPost]
        public ActionResult MemberNotesEdit(int id)
        {
            var m = new MemberNotesModel(id);
            return View("Profile/MemberNotes/Edit", m);
        }

        [HttpPost]
        public ActionResult MemberNotesUpdate(MemberNotesModel m)
        {
            m.UpdateMemberNotes();
            return View("Profile/MemberNotes/Display", m);
        }

        // Member Documents ---------------------------------------------------

        [HttpPost]
        public ActionResult MemberDocuments(int id)
        {
            return View("Profile/Membership/Documents", id);
        }

        [HttpPost]
        public ActionResult UploadDocument(int id, HttpPostedFileBase doc)
        {
            if (doc == null) 
                return Redirect("/Person2/" + id);
            var person = DbUtil.Db.People.Single(pp => pp.PeopleId == id);
            DbUtil.LogPersonActivity("Uploading Document for {0}".Fmt(person.Name), id, person.Name);
            person.UploadDocument(DbUtil.Db, doc.InputStream, doc.FileName, doc.ContentType);
            return Redirect("/Person2/" + id);
        }
        [HttpPost]
        public ActionResult MemberDocumentUpdateName(int pk, string name, string value)
        {
            MemberDocModel.UpdateName(pk, value);
            return new EmptyResult();
        }
        [HttpPost, Route("DeleteDocument/{id:int}/{docid:int}")]
        public ActionResult DeleteDocument(int id, int docid)
        {
            MemberDocModel.DeleteDocument(id, docid);
            return View("Profile/Membership/Documents", id);
        }

        // Comments ---------------------------------------------------

        [HttpPost]
        public ActionResult Comments(int id)
        {
            var m = new CommentsModel(id);
            return View("Profile/Comments/Display", m);
        }

        [HttpPost]
        public ActionResult CommentsEdit(int id)
        {
            var m = new CommentsModel(id);
            return View("Profile/Comments/Edit", m);
        }

        [HttpPost]
        public ActionResult CommentsUpdate(CommentsModel m)
        {
            m.UpdateComments();
            return View("Profile/Comments/Display", m);
        }

    }
}
