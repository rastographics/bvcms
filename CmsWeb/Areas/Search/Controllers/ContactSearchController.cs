using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    public class ContactSearchController : CmsStaffController
    {

        [HttpGet, Route("ContactSearch2")]
        public ActionResult Index()
        {
            if (!ViewExtensions2.UseNewLook())
                return Redirect("/ContactSearch");
            Response.NoCache();
            var m = new ContactSearchModel();
            m.Pager.Set("/ContactSearch2/Results");

            m.GetFromSession();
            return View(m);
        }
        [HttpPost, Route("ContactSearch2/Results/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Results(int? page, int? size, string sort, string dir, ContactSearchModel m)
        {
            m.Pager.Set("/ContactSearch2/Results", page, size, sort, dir);
            m.SaveToSession();
            return View(m);
        }
        [HttpPost, Route("ContactSearch2/Clear")]
        public ActionResult Clear()
        {
            var m = new ContactSearchModel();
            m.ClearSession();
            return Redirect("/ContactSearch2");
        }
        [HttpPost, Route("ContactSearch2/ConvertToQuery")]
        public ActionResult ConvertToQuery(ContactSearchModel m)
        {
            var gid = m.ConvertToQuery();
            return Redirect("/Query/{0}".Fmt(gid));
        }
        [HttpPost, Route("ContactSearch2/ContactTypeQuery/{id:int}")]
        public ActionResult ContactTypeQuery(int id)
        {
            var gid = ContactSearchModel.ContactTypeQuery(id);
            return Redirect("/Query/{0}".Fmt(gid));
        }
        [HttpPost, Route("ContactSearch2/ContactorSummary")]
        public ActionResult ContactorSummary(ContactSearchModel m)
        {
            var q = m.ContactorSummary();
            return View(q);
        }
        [HttpPost, Route("ContactSearch2/ContactSummary")]
        public ActionResult ContactSummary(ContactSearchModel m)
        {
            var q = m.ContactSummary();
            return View(q);
        }

        [HttpPost, Route("ContactSearch2/ContactTypeTotals")]
        public ActionResult ContactTypeTotals(ContactSearchModel m)
        {
            ViewBag.candelete = m.CanDeleteTotal();
            var q = m.ContactTypeTotals();
            return View(q);
        }

        [Authorize(Roles = "Developer")]
        public ActionResult DeleteContactsForType(int id)
        {
            ContactSearchModel.DeleteContactsForType(id);
            return Redirect("/ContactSearch/ContactTypeTotals");
        }
    }
}
