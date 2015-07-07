using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "Downline")]
    public class DownlineController : CmsStaffController
    {
        [HttpGet, Route("~/Downline/{category}/{peopleid}")]
        public ActionResult Index(int category, int peopleid)
        {
            var m = new DownlineModel
            {
                CategoryId = category,
                DownlineId = peopleid
            };
            return View(m);
        }
        [HttpPost, Route("~/Downline/Results")]
        public ActionResult Results(DownlineModel m)
        {
            return View(m);
        }

        [HttpGet, Route("~/Downline/Trace/{category}/{peopleid}")]
        public ActionResult Trace(int category, int peopleid, string trace)
        {
            var m = new DownlineModel
            {
                CategoryId = category,
                DownlineId = peopleid,
                Trace = trace
            };
            return View(m);
        }
    }
}