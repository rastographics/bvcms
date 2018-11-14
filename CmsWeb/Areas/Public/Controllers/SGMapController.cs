using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System.Web.Mvc;

namespace CmsWeb.Areas.Public.Controllers
{
    public class SGMapController : CmsController
    {
        public SGMapController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index(int? id = null) // int id)
        {
            var m = new SGMapModel(id);
            return View(m);
        }
    }
}
