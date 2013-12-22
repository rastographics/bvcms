using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Main.Controllers
{
    public class SavedQueryController : CmsStaffController
    {
        public ActionResult Index()
        {
            if (ViewExtensions2.UseNewLook())
                return Redirect("/SavedQueryList");
            if(ViewExtensions2.TestSb2())
                return Redirect("/SavedQuery2");
            var m = new SavedQueryModel();
            return View(m);
        }
        [HttpPost]
        public ActionResult Edit(string id, string value)
        {
            var a = id.Split('.');
            var iid = a[1].ToInt();
            var c = DbUtil.Db.QueryBuilderClauses.SingleOrDefault(cc => cc.QueryId == iid);
            switch (a[0])
            {
                case "d":
                    c.Description = value;
                    break;
                case "o":
                    c.SavedBy = value;
                    break;
                case "p":
                    c.IsPublic = value == "yes";
                    break;
            }
            DbUtil.Db.SubmitChanges();
            return Content(value);
        }
        [HttpPost]
        public ActionResult Delete(string id)
        {
            var a = id.Split('.');
            var iid = a[1].ToInt();
            var c = DbUtil.Db.QueryBuilderClauses.SingleOrDefault(cc => cc.QueryId == iid);
            DbUtil.Db.DeleteQueryBuilderClauseOnSubmit(c);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }
        [HttpPost]
        public ActionResult Results()
        {
            var m = new SavedQueryModel();
            UpdateModel(m.Pager);
            UpdateModel(m);
            var onlyMine = DbUtil.Db.UserPreference("savedSearchOnlyMine", "true").ToBool();
            if (m.onlyMine != onlyMine)
                DbUtil.Db.SetUserPreference("savedSearchOnlyMine", m.onlyMine);
            return View(m);
        }

    }
}
