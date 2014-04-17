using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix= "SearchDivisions"), Route("{action}/{id?}")]
    public class SearchDivisionsController : CmsStaffController
    {
        [HttpGet, Route("~/SearchDivisions/{id:int}")]
        public ActionResult Index(int id, bool? singlemode, bool? ordered)
        {
            Response.NoCache();
            var m = new SearchDivisionsModel
            {
                id = id,
                singlemode = singlemode ?? false,
                ordered = ordered ?? false,
            };
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(SearchDivisionsModel m)
        {
            return View(m);
        }
        [HttpPost]
        public ActionResult MoveToTop(SearchDivisionsModel m)
        {
            DbUtil.Db.SetMainDivision(m.id, m.topid);
            return View("Results", m);
        }
        [HttpPost]
        public ActionResult AddRemoveDiv(int id, int divid, bool ischecked)
        {
            var d = DbUtil.Db.DivOrgs.SingleOrDefault(dd => dd.DivId == divid && dd.OrgId == id);
            if (ischecked && d == null)
            {
                d = new DivOrg { OrgId = id, DivId = divid };
                DbUtil.Db.DivOrgs.InsertOnSubmit(d);
                DbUtil.Db.SubmitChanges();
            }
            if (!ischecked && d != null)
            {
                DbUtil.Db.DivOrgs.DeleteOnSubmit(d);
                DbUtil.Db.SubmitChanges();
            }
            return new EmptyResult();
        }
    }
}
