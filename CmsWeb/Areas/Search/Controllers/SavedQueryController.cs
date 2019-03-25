using CmsData;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Lifecycle;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "SavedQuery"), Route("{action}/{id?}")]
    public class SavedQueryController : CmsStaffController
    {
        public SavedQueryController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet, Route("~/SavedQueryList")]
        public ActionResult Index()
        {
            var m = new SavedQueryModel(CurrentDatabase)
            {
                OnlyMine = CurrentDatabase.UserPreference("SavedQueryOnlyMine", "false").ToBool()
            };
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(SavedQueryModel m)
        {
            m.Db = CurrentDatabase;
            return View(m);
        }
        [HttpPost]
        public ActionResult Edit(Guid id)
        {
            var m = new SavedQueryInfo(id, CurrentDatabase);
            if (m.Name.Equals(Util.ScratchPad2))
            {
                m.Name = "copy of scratchpad";
            }

            return View(m);
        }
        [HttpPost]
        public ActionResult Update(SavedQueryInfo m)
        {
            m.Db = CurrentDatabase;
            if (m.Name.Equal(Util.ScratchPad2))
            {
                m.Ispublic = false;
            }

            m.CanDelete = true; // must be true since they can edit if they got here
            m.UpdateModel();
            return View("Row", m);
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            var q = CurrentDatabase.LoadQueryById2(id);
            CurrentDatabase.Queries.DeleteOnSubmit(q);
            CurrentDatabase.SubmitChanges();
            return Content("ok");
        }

        [HttpPost]
        public ActionResult Code(SavedQueryModel m)
        {
            m.Db = CurrentDatabase;
            var qlist = from q in m.DefineModelList()
                        select q.QueryId;
            var list = string.Join(",", qlist);
            TempData["codelist"] = list;
            return Content("ok");
        }
        [HttpGet]
        public ActionResult Code()
        {
            var list = TempData["codelist"] as string;
            if (list == null)
            {
                return Content("no data");
            }

            var guids = list.Split(',').ToList().ConvertAll(vv => new Guid(vv));
            return View(new QueryCodeModel(CurrentDatabase, CodeSql.Queries, guids));
        }
        [HttpPost]
        public ActionResult PythonCode(SavedQueryModel m)
        {
            m.Db = CurrentDatabase;
            var qlist = from q in m.DefineModelList()
                        select q.QueryId;
            var list = string.Join(",", qlist);
            TempData["codelist"] = list;
            return Content("ok");
        }
        [HttpGet]
        public ActionResult PythonCode()
        {
            var list = TempData["codelist"] as string;
            if (list == null)
            {
                return Content("no data");
            }

            var guids = list.Split(',').ToList().ConvertAll(vv => new Guid(vv));
            Response.ContentType = "text/plain";
            return View(new QueryCodeModel(CurrentDatabase, CodeSql.Queries, guids));
        }
    }
}
