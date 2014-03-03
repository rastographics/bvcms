using System.Linq;
using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
	public class QuickController : CmsController
	{
		public ActionResult Index(string q)
		{
			if (!q.HasValue())
				return Redirect("/");
			var m = new QuickSearchModel(q);
			if (m.people.Count == 1 && (q.AllDigits() || m.orgs.Count == 0))
			{
				var pid = m.people.Single().PeopleId;
				return Redirect("/Person2/" + pid);
			}
			if (m.orgs.Count == 1 && m.people.Count == 0)
			{
				var oid = m.orgs.Single().Id;
				return Redirect("/Organization/Index/" + oid);
			}
			return View(m);
		}
	}
}
