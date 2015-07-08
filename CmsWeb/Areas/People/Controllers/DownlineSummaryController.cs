using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "DownlineSummary")]
    public class DownlineSummaryController : CmsStaffController
    {
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