using System.Collections.Generic;
using System.Web.Mvc;
using CmsWeb.Areas.Public.Models;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.Public.Controllers
{
	public class SmallGroupFinderController : Controller
	{
		public SmallGroupFinderModel sgfm;

		public ActionResult Index(string id)
		{
			var check = (from e in DbUtil.Db.Contents
							 where e.Name == "SGF-" + id + ".xml"
							 select e).SingleOrDefault();

			if (check == null)
				return new HttpNotFoundResult("Page not found!");

			SmallGroupFinderModel sgfm = new SmallGroupFinderModel();
			sgfm.load(id);

			if (Request.Form.Count == 0)
			{
				//sgfm.setDefaultSearch();
			}
			else
			{
				Dictionary<string, string> post = new Dictionary<string, string>();

				foreach (string item in Request.Form)
				{
					if (item.StartsWith("SGF:"))
						post.Add(item, Request[item]);
				}

				sgfm.setSearch(post);
			}

			if (sgfm.hasShell())
				// Process shell here
				return Content(sgfm.createFromShell());
			else
				return View(sgfm);
		}
	}
}