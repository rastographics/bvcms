using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        #region Membership

        [POST("Person2/Membership/{id}")]
        public ActionResult Membership(int id)
        {
            var m = new MemberInfo(id);
            return View("Profile/Membership/Display", m);
        }

        [POST("Person2/MembershipEdit/{id}")]
        public ActionResult MembershipEdit(int id)
        {
            var m = new MemberInfo(id);
            return View("Profile/Membership/Edit", m);
        }

        [POST("Person2/MembershipUpdate/")]
        public ActionResult MembershipUpdate(MemberInfo m)
        {
            var ret = m.UpdateMember();
            if (ret != "ok")
                ViewBag.AutomationError = "<div class='alert'>{0}</div>".Fmt(ret);
            if (!ModelState.IsValid || ret != "ok")
                return View("Profile/Membership/Edit", m);

            DbUtil.LogActivity("Update Membership Info for: {0}".Fmt(m.person.Name));
            return View("Profile/Membership/Display", m);
        }

        #endregion
        #region Member Notes

        [POST("Person2/MemberNotes/{id:int}")]
        public ActionResult MemberNotes(int id)
        {
            var m = new MemberNotesModel(id);
            return View("Profile/MemberNotes/Display", m);
        }

        [POST("Person2/MemberNotesEdit/{id}")]
        public ActionResult MemberNotesEdit(int id)
        {
            var m = new MemberNotesModel(id);
            return View("Profile/MemberNotes/Edit", m);
        }

        [POST("Person2/MemberNotesUpdate")]
        public ActionResult MemberNotesUpdate(MemberNotesModel m)
        {
            m.UpdateMemberNotes();
            return View("Profile/MemberNotes/Display", m);
        }

        #endregion
        #region Member Documents

        [POST("Person2/MemberDocuments/{id}")]
        public ActionResult MemberDocuments(int id)
        {
            return View("Profile/Membership/Documents", id);
        }

        [POST("Person2/UploadDocument/{id:int}")]
        public ActionResult UploadDocument(int id, HttpPostedFileBase doc)
        {
            var person = DbUtil.Db.People.Single(pp => pp.PeopleId == id);
            DbUtil.LogActivity("Uploading Document for {0}".Fmt(person.Name));
            person.UploadDocument(DbUtil.Db, doc.InputStream, doc.FileName, doc.ContentType);
            return Redirect("/Person2/" + id);
        }
        [POST("Person2/MemberDocumentUpdateName")]
        public ActionResult MemberDocumentEditName(int pk, string name, string value)
        {
            MemberDocModel.UpdateName(pk, value);
            return new EmptyResult();
        }
        [POST("Person2/DeleteDocument/{id:int}/{docid:int}")]
        public ActionResult DeleteDocument(int id, int docid)
        {
            MemberDocModel.DeleteDocument(id, docid);
            return View("Profile/Membership/Documents", id);
        }

        #endregion
        #region Entry

        [POST("Person2/Entry/{id}")]
        public ActionResult Entry(int id)
        {
            return View("Profile/ExtraValues/Entry", id);
        }

        #endregion
        #region Comments

        [POST("Person2/Comments/{id}")]
        public ActionResult Comments(int id)
        {
            var m = new CommentsModel(id);
            return View("Profile/Comments/Display", m);
        }

        [POST("Person2/CommentsEdit/{id:int}")]
        public ActionResult CommentsEdit(int id)
        {
            var m = new CommentsModel(id);
            return View("Profile/Comments/Edit", m);
        }

        [POST("Person2/CommentsUpdate")]
        public ActionResult CommentsUpdate(CommentsModel m)
        {
            m.UpdateComments();
            return View("Profile/Comments/Display", m);
        }

        #endregion
    }
}
