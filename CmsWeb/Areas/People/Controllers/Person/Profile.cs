using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        //----------Membership---------------------------

        [POST("Person2/Membership/{id}")]
        public ActionResult Membership(int id)
        {
            var m = new MemberInfo(id);
            return View("Profile/Membership", m);
        }
        [POST("Person2/MembershipEdit/{id}")]
        public ActionResult MembershipEdit(int id)
        {
            var m = new MemberInfo(id);
            return View("Profile/MembershipEdit", m);
        }
        [POST("Person2/MembershipUpdate/")]
        public ActionResult MembershipUpdate(MemberInfo m)
        {
            var ret = m.UpdateMember();
            if (ret != "ok")
                ViewBag.AutomationError = "<div class='alert'>{0}</div>".Fmt(ret);
            if (!ModelState.IsValid || ret != "ok")
                return View("Profile/MembershipEdit", m);

            DbUtil.LogActivity("Update Membership Info for: {0}".Fmt(m.person.Name));
            return View("Profile/Membership", m);
        }

        //----------Member Notes---------------------------

        [POST("Person2/MemberNotes/{id:int}")]
        public ActionResult MemberNotes(int id)
        {
            var m = new MemberNotesModel(id);
            return View("Profile/MemberNotes", m);
        }
        [POST("Person2/MemberNotesEdit/{id}")]
        public ActionResult MemberNotesEdit(int id)
        {
            var m = new MemberNotesModel(id);
            return View("Profile/MemberNotesEdit", m);
        }
        [POST("Person2/MemberNotesUpdate")]
        public ActionResult MemberNotesUpdate(MemberNotesModel m)
        {
            m.UpdateMemberNotes();
            return View("Profile/MemberNotes", m);
        }

        //----------Member Documents---------------------------

        [POST("Person2/MemberDocuments/{id}")]
        public ActionResult MemberDocuments(int id)
        {
            var m = new PersonModel(id);
            return View("Profile/MemberDocuments", m);
        }

        //----------Extra Values---------------------------

        [POST("Person2/ExtraValuesStandard/{id}")]
        public ActionResult ExtraValuesStandard(int id)
        {
            var m = new PersonModel(id);
            return View("Profile/ExtraValuesStandard", m);
        }

        [POST("Person2/ExtraValuesAdhoc/{id}")]
        public ActionResult ExtraValuesAdhoc(int id)
        {
            var m = new PersonModel(id);
            return View("Profile/ExtraValuesAdhoc", m);
        }

        //----------COMMENTS---------------------------

        [POST("Person2/Comments/{id}")]
        public ActionResult Comments(int id)
        {
            var m = new CommentsModel(id);
            return View("Profile/Comments", m);
        }
        [POST("Person2/CommentsEdit/{id:int}")]
        public ActionResult CommentsEdit(int id)
        {
            var m = new CommentsModel(id);
            return View("Profile/CommentsEdit", m);
        }
        [POST("Person2/CommentsUpdate")]
        public ActionResult CommentsUpdate(CommentsModel m)
        {
            m.UpdateComments();
            return View("Profile/Comments", m);
        }
    }
}
