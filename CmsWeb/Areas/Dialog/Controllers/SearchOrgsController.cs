using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    // todo: use bootstrap
    [RouteArea("Dialog", AreaPrefix = "SearchOrgs"), Route("{action}/{id?}")]
    public class SearchOrgsController : CmsStaffController
    {
        public SearchOrgsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/SearchOrgs/{id:int}")]
        public ActionResult Index(int id, bool? singlemode)
        {
            Response.NoCache();
            var list = Session["orgPickList"] as List<int>;
            var m = new SearchOrgsModel
            {
                id = id,
                singlemode = singlemode ?? false,
                cklist = list,
            };
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(SearchOrgsModel m)
        {
            m.cklist = Session["orgPickList"] as List<int>;
            return View("Index", m);
        }
        [HttpPost]
        public ActionResult SaveOrgIds(int id, string oids)
        {
            Session["orgPickList"] = oids.Split(',').Select(oo => oo.ToInt()).ToList();
            return new EmptyResult();
        }
    }
}
