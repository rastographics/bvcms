using System.Collections.Generic;
using System.Web.Mvc;
using CmsWeb.Areas.Public.Models;
using System.Linq;
using CmsData;
using System.Web;

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
				var search = new Dictionary<string, SearchItem>();

				var encoded = Request.Form.ToString();

                var loadAllValues = DbUtil.Db.Setting("SGF-LoadAllExtraValues", "false").ToLower() == "true";

                foreach (string item in encoded.Split('&'))
				{
					if (item.StartsWith("SGF") || loadAllValues)
					{
						var parts = item.Split('=');

						if (parts.Count() == 2)
						{
							parts[0] = HttpUtility.UrlDecode(parts[0]);
							parts[1] = HttpUtility.UrlDecode(parts[1]);

							if (search.ContainsKey(parts[0]))
							{
								search[parts[0]].values.Add(parts[1]);
								search[parts[0]].parse = true;
							}
							else
							{
								search.Add(parts[0], new SearchItem() { name = parts[0], values = { parts[1] } });
							}
						}
					}
				}

				sgfm.setSearch(search);
			}

			if (sgfm.hasShell())
				// Process shell here
				return Content(sgfm.createFromShell());
			else
				return View(sgfm);
		}
	}
}