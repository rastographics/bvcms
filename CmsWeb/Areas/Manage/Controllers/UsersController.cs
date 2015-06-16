using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
	[Authorize(Roles = "Admin")]
    [RouteArea("Manage", AreaPrefix= "Users"), Route("{action}")]
    public class UsersController : CmsController
    {
        [Route("~/Users")]
        public ActionResult Index(string id)
        {
	        var m = new UsersModel {name = id};
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(UsersModel m)
        {
            return View(m);
        }
    }
}
