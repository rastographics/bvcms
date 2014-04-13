using System.Web.Mvc;
using CmsData.Classes.QuickBooks;
using DevDefined.OAuth.Framework;

namespace CmsWeb.Areas.Finance.Controllers
{
	[Authorize(Roles = "Finance")]
    [RouteArea("Finance", AreaPrefix= "QuickBooks"), Route("{action}/{id?}")]
	public class QuickBooksController : Controller
	{
        [Route("~/QuickBooks")]
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult RequestOAuthToken()
		{
            QuickBooksHelper qbh = new QuickBooksHelper(Request);
            string authLink = qbh.RequestOAuthToken();

            Session["QBToken"] = qbh.GetCurrentToken();

            return Redirect(authLink);
		}

		public ActionResult RequestAccessToken()
		{
            QuickBooksHelper qbh = new QuickBooksHelper(Request);
            qbh.SetCurrentToken( (IToken)Session["QBToken"] );
            qbh.RequestAccessToken(Request["realmId"], Request["oauth_verifier"], Request["dataSource"]);

            // david: Change response based on results
            return View("Index");
		}

		public ActionResult Disconnect()
		{
            QuickBooksHelper qbh = new QuickBooksHelper(Request);
            bool complete = qbh.Disconnect();

            // TODO: Change response based on results
			return View("Index");
		}
	}
}