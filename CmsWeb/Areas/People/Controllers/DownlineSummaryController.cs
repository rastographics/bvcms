using CmsWeb.Areas.People.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "DownlineSummary")]
    public class DownlineSummaryController : CmsStaffController
    {
        public DownlineSummaryController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/DownlineSummary/{category:int}")]
        public ActionResult Index(int category)
        {
            var m = new DownlineSummaryModel
            {
                CategoryId = category
            };
            return View(m);
        }
        [HttpPost, Route("~/DownlineSummary/Results")]
        public ActionResult Results(DownlineSummaryModel m)
        {
            return View(m);
        }

    }
}
