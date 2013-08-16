using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CmsData;
using CmsData.API;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Models;
using ContributionSearchModel = CmsWeb.Models.ContributionSearchModel;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Finance")]
    public class ContributionsController : CmsStaffController
    {
        public ActionResult Index(int? id, int? year)
        {
        	var m = new ContributionSearchModel(id, year);
            return View(m);
        }
        [HttpPost]
		public ActionResult Results(ContributionSearchModel m)
        {
			return View(m);
		}
        [HttpPost]
		public ActionResult Export(ContributionSearchModel m)
        {
            return new ExcelResult { Data = m.ContributionsListAll(), FileName = "contributions.xls" };
        }
        [HttpPost]
		public ActionResult Return(int id, ContributionSearchModel m)
        {
            m.Return(id);
            return RedirectToAction("Index", new {id = m.SearchInfo.PeopleId, m.SearchInfo.Year});
        }
        [HttpPost]
		public ActionResult Reverse(int id, ContributionSearchModel m)
        {
            m.Reverse(id);
            return RedirectToAction("Index", new {id = m.SearchInfo.PeopleId, m.SearchInfo.Year});
        }
    }
}
