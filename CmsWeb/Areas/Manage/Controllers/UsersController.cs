using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System.Web.Mvc;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Manage", AreaPrefix = "Users"), Route("{action}")]
    public class UsersController : CmsController
    {
        public UsersController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Users")]
        public ActionResult Index(string id)
        {
            var m = new UsersModel { name = id };
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(UsersModel m)
        {
            return View(m);
        }
    }
}
