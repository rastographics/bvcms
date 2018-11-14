using CmsWeb.Areas.People.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "DownlineLevels")]
    public class DownlineLevelsController : CmsStaffController
    {
        public DownlineLevelsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/DownlineLevels/{category:int}/{peopleid:int}")]
        public ActionResult Index(int category, int peopleid)
        {
            var m = new DownlineLevelsModel
            {
                CategoryId = category,
                DownlineId = peopleid
            };
            return View(m);
        }
        [HttpPost, Route("~/DownlineLevels/Results")]
        public ActionResult Results(DownlineLevelsModel m)
        {
            return View(m);
        }
    }
}
