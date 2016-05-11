using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsData.Resource;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [RouteArea("Manage", AreaPrefix = "Resources"), Route("{action}/{id?}")]
    [ValidateInput(false)]
    public class ResourceController : CmsStaffController
    {
        [HttpGet]
        [Route("~/Resources")]
        public ActionResult Index()
        {
            var resources = new List<Resource>();
            resources.Add(new Resource
            {
                Name = "South America Mission Goals",
                Type = ResourceType.Pdf,
                OrgId = 1,
                UpdatedTime = DateTime.Now.AddDays(-22)
            });
            resources.Add(new Resource
            {
                Name = "Trip Budget",
                Type = ResourceType.Spreadsheet,
                UpdatedTime = DateTime.Now.AddDays(-12)
            });

            return View(resources);
        }
    }
}
