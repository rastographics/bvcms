using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Controllers
{
    public partial class PersonController
    {
        [POST("Person2/MembershipDisplay/{id}")]
        public ActionResult MembershipDisplay(int id)
        {
            var m = new MemberInfo(id);
            return View("Profile/MembershipDisplay", m);
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
            return View("Profile/MembershipDisplay", m);
        }
        [POST("Person2/MembershipNotes/{id}")]
        public ActionResult MembershipNotes(int id)
        {
            var m = new PersonModel(id);
            return View("Profile/Notes", m);
        }
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
        [POST("Person2/Comments/{id}")]
        public ActionResult Comments(int id)
        {
            var m = new PersonModel(id);
            return View("Profile/Notes", m);
        }
    }
}
