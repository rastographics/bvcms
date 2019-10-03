using CmsData;
using CmsWeb.Areas.People.Models;
using CmsWeb.Common.Status;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.UI;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    public class HomeController : CmsStaffController
    {
        public HomeController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index()
        {
            if (!Util2.OrgLeadersOnly && User.IsInRole("OrgLeadersOnly"))
            {
                Util2.OrgLeadersOnly = true;
                CurrentDatabase.SetOrgLeadersOnly();
            }
            var m = new HomeModel();
            return View(m);
        }

        [OutputCache(Duration = 60, Location = OutputCacheLocation.Any)]
        public async Task<ActionResult> Status()
        {
            var status = await StatusHelper.GetStatusAsync(HttpContext);

            return Json(status, JsonRequestBehavior.AllowGet);
        }

        [ValidateInput(false)]
        public ActionResult ShowError(string error, string url)
        {
            ViewData["error"] = Server.UrlDecode(error);
            ViewData["url"] = url;
            return View();
        }

        public ActionResult NewQuery()
        {
            var qb = CurrentDatabase.ScratchPadCondition();
            qb.Reset();
            qb.Save(CurrentDatabase);
            return Redirect("/Query");
        }

        public ActionResult NthTimeAttenders(int id)
        {
            var name = "VisitNumber-" + id;
            var q = CurrentDatabase.Queries.FirstOrDefault(qq => qq.Owner == "System" && qq.Name == name);
            if (q != null)
            {
                return Redirect("/Query/" + q.QueryId);
            }

            const CompareType comp = CompareType.Equal;
            var cc = CurrentDatabase.ScratchPadCondition();
            cc.Reset();
            Condition c;
            switch (id)
            {
                case 1:
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "1,True");
                    c.Quarters = "1";
                    c.Days = 7;
                    break;
                case 2:
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "1,True");
                    c.Quarters = "2";
                    c.Days = 7;
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "0,False");
                    c.Quarters = "1";
                    c.Days = 7;
                    break;
                case 3:
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "1,True");
                    c.Quarters = "3";
                    c.Days = 7;
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "0,False");
                    c.Quarters = "2";
                    c.Days = 7;
                    break;
            }
            cc.Description = name;
            cc.Save(CurrentDatabase, owner: "System");
            TempData["autorun"] = true;
            return Redirect("/Query/" + cc.Id);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ActiveRecords(DateTime? dt)
        {
            if (dt.HasValue)
            {
                TempData["ActiveRecords"] = CurrentDatabase.ActiveRecordsdt(dt.Value);
                TempData["ActiveRecords2"] = CurrentDatabase.ActiveRecords2dt(dt.Value);
            }
            else
            {
                TempData["ActiveRecords"] = CurrentDatabase.ActiveRecords();
                TempData["ActiveRecords2"] = CurrentDatabase.ActiveRecords2();
            }
            return View("Support2");
        }

        public ActionResult TargetPerson(bool id)
        {
            CurrentDatabase.SetUserPreference("TargetLinkPeople", id ? "false" : "true");
            CurrentDatabase.SubmitChanges();
            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.OriginalString);
            }

            return Redirect("/");
        }
        public ActionResult TargetOrg(bool id)
        {
            CurrentDatabase.SetUserPreference("TargetLinkOrg", id ? "false" : "true");
            CurrentDatabase.SubmitChanges();
            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.OriginalString);
            }

            return Redirect("/");
        }
        public ActionResult OnlineRegTypeSearchAdd(bool id)
        {
            Util2.SetSessionObj("OnlineRegTypeSearchAdd", id ? "false" : "true");
            CurrentDatabase.SubmitChanges();
            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.OriginalString);
            }

            return Redirect("/");
        }

        public ActionResult Names(string term)
        {
            var q = HomeModel.Names(term).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, Route("~/FastSearch")]
        public ActionResult FastSearch(string q)
        {
            var qq = HomeModel.FastSearch(q).ToArray();
            return Content(JsonConvert.SerializeObject(qq));
        }

        [HttpGet, Route("~/FastSearchPrefetch")]
        public ActionResult FastSearchPrefetch()
        {
            Response.NoCache();
            var qq = HomeModel.PrefetchSearch().ToArray();
            return Content(JsonConvert.SerializeObject(qq));
        }

        public ActionResult SwitchTag(string tag)
        {
            var m = new TagsModel { tag = tag };
            m.SetCurrentTag();
            if (Request.UrlReferrer != null)
            {
                return Redirect(Request.UrlReferrer.ToString());
            }

            return Redirect("/");
        }

        [HttpGet, Route("~/Preferences")]
        public ActionResult UserPreferences()
        {
            return View(CurrentDatabase.CurrentUser);
        }

        [HttpGet, Route("~/Home/Support2")]
        public ActionResult Support2(string helplink)
        {
            if (helplink.HasValue())
            {
                TempData["HelpLink"] = HttpUtility.UrlDecode(helplink);
            }

            return View();
        }
    }
}
