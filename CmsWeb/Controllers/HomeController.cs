using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.People.Models;
using Newtonsoft.Json;
using UtilityExtensions;
using CmsWeb.Models;

namespace CmsWeb.Controllers
{
    public class HomeController : CmsStaffController
    {
        public ActionResult Index()
        {
            if (!Util2.OrgMembersOnly && User.IsInRole("OrgMembersOnly"))
            {
                Util2.OrgMembersOnly = true;
                DbUtil.Db.SetOrgMembersOnly();
            }
            else if (!Util2.OrgLeadersOnly && User.IsInRole("OrgLeadersOnly"))
            {
                Util2.OrgLeadersOnly = true;
                DbUtil.Db.SetOrgLeadersOnly();
            }
            var m = new HomeModel();
            return View(m);
        }
        [GET("Person/TinyImage/{id}")]
        [GET("Person2/TinyImage/{id}")]
        [GET("TinyImage/{id}")]
        public ActionResult TinyImage(int id)
        {
            return new PictureResult(id, portrait: true, tiny: true);
        }
        [GET("Person/Image/{id:int}/{w:int?}/{h:int?}")]
        [GET("Person2/Image/{id:int}/{w:int?}/{h:int?}")]
        [GET("Image/{id:int}/{w:int?}/{h:int?}")]
        public ActionResult Image(int id, int? w, int? h)
        {
            return new PictureResult(id);
        }
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Support2(string helplink)
        {
            if(helplink.HasValue())
                TempData["HelpLink"] = HttpUtility.UrlDecode(helplink);
            return View();
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
            var qb = DbUtil.Db.ScratchPadCondition();
            qb.Reset(DbUtil.Db);
            qb.Save(DbUtil.Db);
            return Redirect("/QueryBuilder2/Main");
        }
        public ActionResult Test()
        {
            string test = null;
            var x = test.Replace('3', '4');
            return Content("done");
        }
        public ActionResult RecordTest(int id, string v)
        {
            var o = DbUtil.Db.LoadOrganizationById(id);
            o.AddEditExtra(DbUtil.Db, "tested", v);
            DbUtil.Db.SubmitChanges();
            return Content(v);
        }
        public ActionResult NthTimeAttenders(int id)
        {
            var name = "VisitNumber-" + id;
            const CompareType comp = CompareType.Equal;
            var cc = DbUtil.Db.ScratchPadCondition();
            cc.Reset(DbUtil.Db);
            Condition c;
            switch (id)
            {
                case 1:
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "1,T");
                    c.Quarters = "1";
                    c.Days = 7;
                    break;
                case 2:
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "1,T");
                    c.Quarters = "2";
                    c.Days = 7;
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "0,F");
                    c.Quarters = "1";
                    c.Days = 7;
                    break;
                case 3:
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "1,T");
                    c.Quarters = "3";
                    c.Days = 7;
                    c = cc.AddNewClause(QueryType.RecentVisitNumber, comp, "0,F");
                    c.Quarters = "2";
                    c.Days = 7;
                    break;
            }
            cc.Save(DbUtil.Db);
            TempData["autorun"] = true;
            return Redirect("/QueryBuilder2/Main/" + cc.Id);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ActiveRecords()
        {
            TempData["ActiveRecords"] = DbUtil.Db.ActiveRecords();
            return View("Support2");
        }
        public ActionResult TargetPerson(bool id)
        {
            DbUtil.Db.SetUserPreference("TargetLinkPeople", id ? "false" : "true");
            DbUtil.Db.SubmitChanges();
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.OriginalString);
            return Redirect("/");
        }

        public ActionResult ToggleSupport()
        {
            var usesupport = DbUtil.Db.UserPreference("UseNewSupport", "false").ToBool();
            DbUtil.Db.SetUserPreference("UseNewSupport", usesupport ? "false" : "true");
            DbUtil.Db.SubmitChanges();
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.OriginalString);
            return Redirect("/");
        }
        public ActionResult Names(string term)
        {
            var q = HomeModel.Names(term).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }
        [POST("FastSearch")]
        public ActionResult FastSearch(string q)
        {
            var qq = HomeModel.FastSearch(q).ToArray();
            return Content(JsonConvert.SerializeObject(qq));
        }
        [GET("FastSearchPrefetch")]
        public ActionResult FastSearchPrefetch()
        {
            Response.NoCache();
            var qq = HomeModel.PrefetchSearch().ToArray();
            return Content(JsonConvert.SerializeObject(qq));
        }

        [POST("HideTip")]
        public ActionResult HideTip(string tip)
        {
            DbUtil.Db.SetUserPreference("hide-tip-" + tip, "true");
            return new EmptyResult();
        }
        [GET("ResetTips")]
        public ActionResult ResetTips()
        {
            DbUtil.Db.ExecuteCommand("DELETE dbo.Preferences WHERE Preference LIKE 'hide-tip-%' AND UserId = {0}", Util.UserId);
            var d = Session["preferences"] as Dictionary<string, string>;
            var keys = d.Keys.Where(kk => kk.StartsWith("hide-tip-")).ToList();
            foreach (var k in keys)
                d.Remove(k);

            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
            return Redirect("/");
        }

        public ActionResult TestTypeahead()
        {
            return View();
        }

        public ActionResult TestRegs()
        {
            foreach (var o in DbUtil.Db.Organizations)
            {
                try
                {
                    var rs = new Settings(o.RegSetting, DbUtil.Db, o.OrganizationId);
                }
                catch (Exception ex)
                {
                    return Content("bad org <a href=\"{0}{1}\">{2}</a>\n{3}".Fmt(Util.ServerLink("/RegSetting/Index/"), o.OrganizationId, o.OrganizationName, ex.Message));
                }
            }
            return Content("ok");
        }
        public ActionResult SwitchTag(string tag)
        {
            var m = new TagsModel { tag = tag };
            m.SetCurrentTag();
            if (Request.UrlReferrer != null)
                return Redirect(Request.UrlReferrer.ToString());
            return Redirect("/");
        }
    }
}