using System;
using System.Linq;
using System.Web.Mvc;
using CmsWeb.Areas.Search.Models;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaPrefix = "SavedQuery"), Route("{action}/{id?}")]
    public class SavedQueryController : CmsStaffController
    {
        [HttpGet, Route("~/SavedQueryList")]
        public ActionResult Index()
        {
            var m = new SavedQueryModel { OnlyMine = DbUtil.Db.UserPreference("SavedQueryOnlyMine", "false").ToBool() };
            m.Pager.Set("/SavedQuery/Results", 1, null, "Last Run", "desc");
            return View(m);
        }
        [HttpPost, Route("Results/{page?}/{size?}/{sort=Last Run}/{dir=desc}")]
        public ActionResult Results(int? page, int? size, string sort, string dir, SavedQueryModel m)
        {
            m.Pager.Set("/SavedQuery/Results", page, size, sort, dir);
            return View(m);
        }
        [HttpPost]
        public ActionResult Edit(Guid id)
        {
            var m = new SavedQueryInfo(id);
            if (m.Name.Equals(Util.ScratchPad2))
                m.Name = "copy of scratchpad";
            return View(m);
        }
        [HttpPost]
        public ActionResult Update(SavedQueryInfo m)
        {
            if (m.Name.Equal(Util.ScratchPad2))
                m.Ispublic = false;
            m.CanDelete = true; // must be true since they can edit if they got here
            m.UpdateModel();
            return View("Row", m);
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            var q = DbUtil.Db.LoadQueryById2(id);
            DbUtil.Db.Queries.DeleteOnSubmit(q);
            DbUtil.Db.SubmitChanges();
            return Content("ok");
        }
    }
}
