using CmsWeb.Lifecycle;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;
using CmsWeb.Areas.Giving.Models;
using CmsData.Codes;
using CmsData.Classes.Giving;
using System.Text.RegularExpressions;

namespace CmsWeb.Areas.Giving.Controllers
{
    [RouteArea("Giving", AreaPrefix = "Give"), Route("{action}/{id?}")]
    public class GivingPageController : CmsStaffController
    {
        public GivingPageController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        [Route("~/Give")]
        public ActionResult DefaultPageIndex()
        {
            var peopleId = CurrentDatabase.CurrentUser.PeopleId;
            var person = CurrentDatabase.People.Where(p => p.PeopleId == CurrentDatabase.CurrentUser.PeopleId).SingleOrDefault();
            if(person.CampusId != null)
            {
                foreach(var item in CurrentDatabase.GivingPages)
                {
                    if(item.CampusId == person.CampusId && item.MainCampusPageFlag == true)
                    {
                        return Redirect("/Give/" + item.PageUrl);
                    }
                }
                var givingPage = CurrentDatabase.GivingPages.Where(p => p.DefaultPage == true).SingleOrDefault();
                return Redirect("/Give/" + givingPage.PageUrl);
            }
            else
            {
                var givingPage = CurrentDatabase.GivingPages.Where(p => p.DefaultPage == true).SingleOrDefault();
                return Redirect("/Give/" + givingPage.PageUrl);
            }
        }

        [HttpGet]
        [Route("~/Give/{id}")]
        public ActionResult Index(string id, string type = null, int fund = 0, string amount = null)
        {
            var givingPage = CurrentDatabase.GivingPages.Where(p => p.PageUrl == id).SingleOrDefault();
            if (givingPage == null)
            {
                // no giving page at this url
                return Redirect("/Give/");
            }
            else
            {
                if (!givingPage.Enabled)
                {
                    if (givingPage.DisabledRedirect != null)
                    {
                        return Redirect(givingPage.DisabledRedirect);
                    }
                    else
                    {
                        // no where to redirect to
                        return Redirect("/Give/");
                    }
                }
            }
            var model = new GivingPageModel(CurrentDatabase);
            var page = model.GetGivingPages(givingPage.GivingPageId).SingleOrDefault();
            var shell = givingPage.SkinFile?.Body;
            if (shell.HasValue())
            {
                var re = new Regex(@"(.*<!--FORM START-->\s*).*(<!--FORM END-->.*)", RegexOptions.Singleline);
                var t = re.Match(shell).Groups[1].Value.Replace("<!--FORM CSS-->", ViewExtensions2.Bootstrap3Css());
                t = t.Replace("<html>\r\n<head>\r\n\t<title></title>\r\n</head>\r\n<body>&nbsp;</body>\r\n", "");
                ViewBag.hasshell = true;
                ViewBag.top = t;
                var b = re.Match(shell).Groups[2].Value;
                b = b.Replace("</html>", "");
                ViewBag.bottom = b;
            }
            else
            {
                ViewBag.hasshell = false;
            }

            ViewBag.Type = type;
            ViewBag.Fund = fund;
            ViewBag.Amount = amount;
            return View(page);
        }
        
    }
}
