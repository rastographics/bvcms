using System;
using System.Collections.Generic;
using System.Linq;
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
        [ValidateInput(false)]
        public ActionResult ShowError(string error, string url)
        {
            ViewData["error"] = Server.UrlDecode(error);
            ViewData["url"] = url;
            return View();
        }
        public ActionResult NewQuery()
        {
            var qb = DbUtil.Db.QueryBuilderScratchPad();
            qb.CleanSlate(DbUtil.Db);
            return Redirect("/QueryBuilder/Main");
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
            var qb = DbUtil.Db.QueryBuilderClauses.FirstOrDefault(c => c.IsPublic && c.Description == name && c.SavedBy == "public");
            if (qb == null)
            {
                qb = DbUtil.Db.QueryBuilderScratchPad();
                qb.CleanSlate(DbUtil.Db);

                var comp = CompareType.Equal;
                QueryBuilderClause clause = null;
                switch (id)
                {
                    case 1:
                        clause = qb.AddNewClause(QueryType.RecentVisitNumber, comp, "1,T");
                        clause.Quarters = "1";
                        clause.Days = 7;
                        break;
                    case 2:
                        clause = qb.AddNewClause(QueryType.RecentVisitNumber, comp, "1,T");
                        clause.Quarters = "2";
                        clause.Days = 7;
                        clause = qb.AddNewClause(QueryType.RecentVisitNumber, comp, "0,F");
                        clause.Quarters = "1";
                        clause.Days = 7;
                        break;
                    case 3:
                        clause = qb.AddNewClause(QueryType.RecentVisitNumber, comp, "1,T");
                        clause.Quarters = "3";
                        clause.Days = 7;
                        clause = qb.AddNewClause(QueryType.RecentVisitNumber, comp, "0,F");
                        clause.Quarters = "2";
                        clause.Days = 7;
                        break;
                }
                qb = qb.SaveTo(DbUtil.Db, name, "public", true);
            }
            TempData["autorun"] = true;
            return Redirect("/QueryBuilder/Main/{0}".Fmt(qb.QueryId));
        }
        [Authorize(Roles = "Admin")]
        public ActionResult ActiveRecords()
        {
            TempData["ActiveRecords"] = DbUtil.Db.ActiveRecords();
            return View("About");
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