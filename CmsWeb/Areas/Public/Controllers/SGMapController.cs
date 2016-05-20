using System.Web.Mvc;
using CmsWeb.Models;

namespace CmsWeb.Areas.Public.Controllers
{
    public class SGMapController : CmsController
    {
        public ActionResult Index(int? id = null) // int id)
        {
            var m = new SGMapModel(id);
            return View(m);
        }
    }
}
