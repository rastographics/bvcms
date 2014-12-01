using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "RegistrationSearch"), Route("{action}/{id?}")]
    public class RegistrationSearchController : CmsStaffController
    {
        [HttpGet, Route("~/RegistrationSearch")]
        public ActionResult Index()
        {
            Response.NoCache();
            var m = new RegistrationSearchModel();
            m.GetFromSession();
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(RegistrationSearchModel m, PagerModel2 pager)
        {
            m.Pager = pager;
            m.SaveToSession();
            return View(m);
        }
        [HttpPost]
        public ActionResult Clear()
        {
            var m = new RegistrationSearchModel();
            m.ClearSession();
            return Redirect("/RegistrationSearch");
        }
    }
}
