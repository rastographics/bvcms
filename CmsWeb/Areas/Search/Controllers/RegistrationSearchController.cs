using CmsWeb.Areas.Search.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "RegistrationSearch"), Route("{action}/{id?}")]
    public class RegistrationSearchController : CmsStaffController
    {
        public RegistrationSearchController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/RegistrationSearch")]
        public ActionResult Index()
        {
            Response.NoCache();
            var m = new RegistrationSearchModel();

            //m.GetFromSession();
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(RegistrationSearchModel m)
        {
            //m.SaveToSession();
            return View(m);
        }
        [HttpPost]
        public ActionResult Clear()
        {
            //var m = new RegistrationSearchModel();
            //m.ClearSession();
            return Redirect("/RegistrationSearch");
        }
        [HttpGet, Route("~/IncompleteRegistrationsOrg/{id:int}")]
        public ActionResult IncompleteRegistrationsOrg(int id, int? days)
        {
            var m = new IncompleteRegistrations(id, days);

            return View("IncompleteRegistrations", m);
        }
        [HttpPost, Route("~/IncompleteRegistrations")]
        public ActionResult IncompleteRegistrations(OrgSearchModel orgsearch, int? days)
        {
            var m = new IncompleteRegistrations(orgsearch, days);

            return View("IncompleteRegistrations", m);
        }
        [HttpPost, Route("~/IncompleteRegistrations/Results")]
        public ActionResult IncompleteResults(IncompleteRegistrations m)
        {
            //m.SaveToSession();
            return View(m);
        }
    }
}
