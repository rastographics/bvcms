using CmsWeb.Areas.FundReporting.Services;
using System.Web.Mvc;

namespace CmsWeb.Areas.FundReporting.Controllers
{
    [Authorize(Roles = "FundManager")]
    [RouteArea("FundManager", AreaPrefix = "FundManager")]
    public class FundReportingController : CmsStaffController
    {
        private readonly FundReportingService _fundReportingService;

        public FundReportingController()
        {
            _fundReportingService = new FundReportingService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("/report/{reportName}")]
        public ActionResult Run(string reportName)
        {
            return View();
        }

        [HttpPost]
        [Route("/report/{reportName}/{outputType}")]
        public ActionResult Run(string reportName, string outputType = "html")
        {
            switch (outputType)
            {
                case "html":
                    {
                        return View();
                    }
                case "xml":
                    {
                        return Content("");
                    }
                case "json":
                    {
                        return Content("");
                    }
                case "csv":
                    {
                        return Content("");
                    }
                case "xls":
                    {
                        return Content("");
                    }
                case "pdf":
                    {
                        return Content("");
                    }
                default:
                    {
                        return Content("");
                    }
            }
        }
    }
}
