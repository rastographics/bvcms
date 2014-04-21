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
		public ActionResult LastActivity(int? userid, string activity)
		{
			var dt = DateTime.Now;
			var cmd = new SqlCommand();
			cmd.CommandTimeout = 300;
			if (activity.HasValue())
			{
				if (userid.HasValue)
				{
					cmd.Parameters.AddWithValue("@p2", userid);
					cmd.Parameters.AddWithValue("@p3", "%" + activity + "%");
					cmd.CommandText =
						@"SELECT TOP 200 
Name, UserId, Activity, ActivityDate, Machine 
FROM dbo.ActivityAll 
where userid = @p2 and Activity like @p3
ORDER BY ActivityDate DESC";
				}
				else
				{
					cmd.Parameters.AddWithValue("@p1", "%" + activity + "%");
					cmd.CommandText =
						@"SELECT TOP 200 
Name, UserId, Activity, ActivityDate, Machine 
FROM dbo.ActivityAll
where Activity like @p1
ORDER BY ActivityDate DESC";
				}
			}
			else
			{
				if (userid.HasValue)
				{
					cmd.Parameters.AddWithValue("@p2", userid);
					cmd.CommandText =
						@"SELECT TOP 200 
Name, UserId, Activity, ActivityDate, Machine 
FROM dbo.ActivityAll
where userid = @p2
ORDER BY ActivityDate DESC";
				}
				else
					cmd.CommandText =
						@"SELECT TOP 200 
Name, UserId, Activity, ActivityDate, Machine 
FROM dbo.ActivityAll
ORDER BY ActivityDate DESC";
			}
			return View(cmd);
		}
    }
}
