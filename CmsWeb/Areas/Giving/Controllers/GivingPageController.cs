using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;
using CmsWeb.Areas.Giving.Models;
using CmsData.Codes;
using CmsData.Classes.Giving;

namespace CmsWeb.Areas.Giving.Controllers
{
    [RouteArea("Giving", AreaPrefix = "Give"), Route("{action}/{id?}")]
    public class GivingPageController : CmsStaffController
    {
        public GivingPageController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        [Route("~/Give/{id}")]
        public ActionResult Index(string id, string type = null, int fund = 0, string amount = null)
        {
            var page = CurrentDatabase.GivingPages.Where(p => p.PageUrl == id).SingleOrDefault();
            ViewBag.Id = id;
            ViewBag.Type = type;
            ViewBag.Fund = fund;
            ViewBag.Amount = amount;
            return View(page);
        }
        
    }
}
