using System.Collections.Generic;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using CmsData.API;

namespace CmsWeb.Areas.Public.Controllers
{
    public class APITestController : Controller
    {
        public ActionResult Index()
        {
			//var plan = new TestPlan { Sections = ApiTestInfo.testplan() };
			var plan = ApiTestInfo.testplan();
//		    var sw = new StringWriter();
//			var xs = new XmlSerializer(typeof(TestPlan));
//		    var ns = new XmlSerializerNamespaces();
//		    ns.Add("", "");
//		    xs.Serialize(sw, plan, ns);
//		    return Content(sw.ToString(), "text/xml");
            return View(plan);
        }
        [ValidateInput(false)]
        public ActionResult Init(string script, string uname, string pword)
        {
            Session["APIuname"] = uname;
            Session["APIpword"] = pword;
            Session["APIinit"] = script;
            var valid = CMSMembershipProvider.provider.ValidateUser(uname, pword);
            if (valid)
            {
                var roles = CMSRoleProvider.provider;
                if (!roles.IsUserInRole(uname, "Developer"))
                    valid = false;
            }
            DbUtil.LogActivity("APITest");
            if (!valid)
                return Content("Not a Valid Developer");
            return Content("Authentication Initialized");
        }
		[ValidateInput(false)]
		public ActionResult Test(string script, Dictionary<string, string> args)
		{
			if (args == null)
				args = new Dictionary<string, string>();
			args.Add("uname", (string)Session["APIuname"]);
			args.Add("pword", (string)Session["APIpword"]);
			var init = (string)Session["APIinit"];
			return Content(APIFunctions.TestAPI(init, script, args));
		}
        [Authorize(Roles = "Finance")]
        public ActionResult TurnFinanceOn()
        {
            Session.Remove("testnofinance");
            return Redirect("/Person2/Current");
        }
        [Authorize(Roles = "Finance")]
        public ActionResult TurnFinanceOff()
        {
            Session["testnofinance"] = "true";
            return Redirect("/Person2/Current");
        }
    }
}