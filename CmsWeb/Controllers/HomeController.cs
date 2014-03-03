using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.People.Models;
using CmsWeb.Code;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using iTextSharp.tool.xml.html;
using Newtonsoft.Json;
using UtilityExtensions;
using CmsWeb.Models;
using Novacode;

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
            return Redirect(ViewExtensions2.UseNewLook()
                ? "/Query"
                : "/QueryBuilder2/Main");
        }
        public ActionResult Test()
        {
            var q = from p in DbUtil.Db.People
                    where p.LastName == "Carroll"
                    select p;
            byte[] contents = null;
            using (var ms = new MemoryStream())
            {
                var dd = DocX.Create(Server.MapPath("/ttt.docx"));
                dd.MarginLeft = 18;
                dd.MarginRight = 18;
                dd.MarginTop = 48;
                dd.MarginBottom = 30;
                dd.PageHeight = 1056;
                dd.PageWidth = 816;
                var col = 0;
                var row = 0;
                Table t = null;
                foreach (var p in q)
                {
                    if (t == null || col == 0 && row == 0)
                    {
                        t = dd.InsertTable(10, 5);
                        foreach (var rr in t.Rows)
                            for (var i = 0; i < 5; i++)
                            {
                                rr.Cells[i].VerticalAlignment = VerticalAlignment.Center;
                                rr.Height = 96.0;
                                rr.Cells[i].Width = i % 2 == 0
                                    ? 252.4667
                                    : 11.533;
                            }
                    }
                    var c = t.Rows[row].Cells[col];
                    c.Paragraphs[0].InsertText(p.Name);
                    c.InsertParagraph(p.PrimaryAddress);
                    if (p.PrimaryAddress2.HasValue())
                        c.InsertParagraph(p.PrimaryAddress2);
                    c.InsertParagraph(p.CityStateZip);

                    col += 2;
                    if (col == 6)
                    {
                        row++;
                        col = 0;
                        if (row == 10)
                            row = 0;
                    }
                }
                dd.SaveAs(ms);
                Response.ContentType = "application/msword";
                Response.AddHeader("content-disposition", "inline; filename=minutes.docx");
                Response.AddHeader("content-length", ms.Length.ToString());
                Response.BinaryWrite(ms.ToArray());
                Response.End();
            }
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
            var q = DbUtil.Db.Queries.FirstOrDefault(qq => qq.Owner == "System" && qq.Name == name);
            if (q != null)
                return Redirect(
                    ViewExtensions2.UseNewLook()
                        ? "/Query/" + q.QueryId
                        : "/QueryBuilder2/Main/" + q.QueryId);

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
            cc.Description = name;
            cc.Save(DbUtil.Db, owner: "System");
            TempData["autorun"] = true;
            return Redirect(
                ViewExtensions2.UseNewLook()
                    ? "/Query/" + cc.Id
                    : "/QueryBuilder2/Main/" + cc.Id);
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

        [GET("Preferences")]
        public ActionResult UserPreferences()
        {
            return View(DbUtil.Db.CurrentUser);
        }
        [GET("Home/Support2")]
        public ActionResult Support2(string helplink)
        {
            if (helplink.HasValue())
                TempData["HelpLink"] = HttpUtility.UrlDecode(helplink);
            return View();
        }
    }

    public class Home2Controller : CmsController
    {
        [GET("Home/MyDataSupport")]
        public ActionResult MyDataSupport()
        {
            return View("../Home/MyDataSupport");
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
    }
}