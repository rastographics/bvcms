using CmsWeb.Areas.Search.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "ContactSearch2"), Route("{action}/{id?}")]
    public class ContactSearchController : CmsStaffController
    {
        public ContactSearchController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/ContactSearch2")]
        public ActionResult Index()
        {
            Response.NoCache();
            var m = new ContactSearchModel();

            m.GetFromSession();
            return View(m);
        }

        [HttpPost]
        public ActionResult Results(ContactSearchModel m)
        {
            m.SaveToSession();
            return View(m);
        }

        [HttpPost]
        public ActionResult Clear()
        {
            var m = new ContactSearchModel();
            m.ClearSession();
            return Redirect("/ContactSearch2");
        }

        [HttpPost]
        public ActionResult ConvertToQuery(ContactSearchModel m)
        {
            var gid = m.ConvertToQuery();
            return Redirect($"/Query/{gid}");
        }

        [HttpGet]
        public ActionResult ContactTypeQuery(int id)
        {
            var gid = ContactSearchModel.ContactTypeQuery(id);
            return Redirect($"/Query/{gid}");
        }

        [HttpPost]
        public ActionResult ContactorSummary(ContactSearchModel m)
        {
            var q = m.ContactorSummary();
            return View(q);
        }

        [HttpPost]
        public ActionResult ContactSummary(ContactSearchModel m)
        {
            var q = m.ContactSummary();
            return View(q);
        }

        [HttpPost]
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
