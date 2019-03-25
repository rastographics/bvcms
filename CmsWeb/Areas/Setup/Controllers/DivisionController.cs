using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Dapper;
using System;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [RouteArea("Setup", AreaPrefix = "Division"), Route("{action}/{id?}")]
    public class DivisionController : CmsStaffController
    {
        public DivisionController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Authorize(Roles = "Admin")]
        [Route("~/Divisions")]
        public ActionResult Index()
        {
            var m = new DivisionModel();
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(DivisionModel m)
        {
            return View(m);
        }
        [HttpPost]
        public ActionResult Create(DivisionModel m)
        {
            var d = new Division { Name = "New Division" };
            d.ProgId = m.TagProgramId;
            d.ProgDivs.Add(new ProgDiv { ProgId = m.TagProgramId.Value });
            CurrentDatabase.Divisions.InsertOnSubmit(d);
            CurrentDatabase.SubmitChanges();
            CurrentDatabase.Refresh(RefreshMode.OverwriteCurrentValues, d);
            var di = m.DivisionItem(d.Id).Single();
            return View("Row", di);
        }


        public ActionResult Edit(string id, string value)
        {
            if (!id.HasValue())
            {
                return new EmptyResult();
            }

            var iid = id.Substring(1).ToInt();
            var div = CurrentDatabase.Divisions.SingleOrDefault(p => p.Id == iid);
            if (div != null)
            {
                switch (id.Substring(0, 1))
                {
                    case "n":
                        div.Name = value;
                        CurrentDatabase.SubmitChanges();
                        return Content(value);
                    case "p":
                        div.ProgId = value.ToInt();
                        CurrentDatabase.SubmitChanges();
                        return Content(div.Program.Name);
                    case "r":
                        div.ReportLine = value.ToInt2();
                        CurrentDatabase.SubmitChanges();
                        return Content(value);
                    case "z":
                        div.NoDisplayZero = value == "yes";
                        CurrentDatabase.SubmitChanges();
                        return Content(value);
                }
            }

            return new EmptyResult();
        }
        [HttpPost]
        public EmptyResult Delete(int id)
        {
            var div = CurrentDatabase.Divisions.SingleOrDefault(m => m.Id == id);
            if (div == null)
            {
                return new EmptyResult();
            }

            CurrentDatabase.ProgDivs.DeleteAllOnSubmit(CurrentDatabase.ProgDivs.Where(di => di.DivId == id));
            CurrentDatabase.DivOrgs.DeleteAllOnSubmit(CurrentDatabase.DivOrgs.Where(di => di.DivId == id));

            foreach (var o in div.Organizations)
            {
                o.DivisionId = null;
            }

            CurrentDatabase.Divisions.DeleteOnSubmit(div);
            CurrentDatabase.SubmitChanges();

            return new EmptyResult();
        }
        [Serializable]
        public class ToggleTagReturn
        {
            public string value;
            public string ChangeMain;
        }
        [HttpPost]
        public ActionResult ToggleProg(int id, DivisionModel m)
        {
            var division = CurrentDatabase.Divisions.Single(d => d.Id == id);
            bool t = division.ToggleTag(CurrentDatabase, m.TagProgramId.Value);
            CurrentDatabase.SubmitChanges();
            CurrentDatabase.Refresh(RefreshMode.OverwriteCurrentValues, division);
            var di = m.DivisionItem(id).Single();
            return View("Row", di);
        }
        [HttpPost]
        public ActionResult MainProg(int id, DivisionModel m)
        {
            var division = CurrentDatabase.Divisions.Single(d => d.Id == id);
            division.ProgId = m.TagProgramId;
            CurrentDatabase.SubmitChanges();
            CurrentDatabase.Refresh(RefreshMode.OverwriteCurrentValues, division);
            var di = m.DivisionItem(id).Single();
            return View("Row", di);
        }

        [HttpGet, Route("~/DivisionCodes")]
        public ActionResult Codes()
        {
            var sql = @"
SELECT 
	pd.ProgId
	, p.Name Prog
	, pd.DivId
	, d.Name Div
FROM dbo.Program p
JOIN dbo.ProgDiv pd ON pd.ProgId = p.Id
JOIN dbo.Division d ON d.Id = pd.DivId
ORDER BY p.Name, d.Name
";
            var q = CurrentDatabase.Connection.Query(sql);
            var sb = new StringBuilder();
            var currprog = "";
            foreach (var r in q)
            {
                if (r.Prog != currprog)
                {
                    sb.AppendLine($"\"{r.ProgId}[{r.Prog}]\",");
                    currprog = r.Prog;
                }
                sb.AppendLine($"\t\"{r.DivId}[{r.Div}]\",");
            }
            return Content(sb.ToString(), "text/plain");
        }
    }
}
