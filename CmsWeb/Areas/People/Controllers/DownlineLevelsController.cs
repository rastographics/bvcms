using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models;

namespace CmsWeb.Areas.People.Controllers
{
    [RouteArea("People", AreaPrefix = "DownlineLevels")]
    public class DownlineLevelsController : CmsStaffController
    {
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