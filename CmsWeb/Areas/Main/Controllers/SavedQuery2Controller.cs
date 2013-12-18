using System;
using System.Linq;
using System.Web.Mvc;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Areas.Main.Controllers
{
    public class SavedQuery2Controller : CmsStaffController
    {
        public ActionResult Index()
        {
            if (Fingerprint.UseNewLook())
                return Redirect("/SavedQueryList");
            if(!Fingerprint.TestSb2())
                return Redirect("/SavedQuery");
            var m = new SavedQuery2Model();
            return View(m);
        }
        [HttpPost]
        public ActionResult Edit(string id, string value)
        {
            var a = id.SplitStr(".", 2);
            var iid = Guid.Parse(a[1]);
            var c = DbUtil.Db.Queries.SingleOrDefault(cc => cc.QueryId == iid);
            switch (a[0])
            {
                case "d":
                    c.Name = value;
                    break;
                case "o":
                    c.Owner = value;
                    break;
                case "p":
                    c.Ispublic = value == "yes";
                    break;
            }
            DbUtil.Db.SubmitChanges();
            return Content(value);
        }
        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            var c = DbUtil.Db.Queries.Single(cc => cc.QueryId == id);
            DbUtil.Db.Queries.DeleteOnSubmit(c);
            DbUtil.Db.SubmitChanges();
            return new EmptyResult();
        }
        [HttpPost]
        public ActionResult Results()
        {
            var m = new SavedQuery2Model();
            UpdateModel(m.Pager);
            UpdateModel(m);
            var onlyMine = DbUtil.Db.UserPreference("savedSearchOnlyMine", "true").ToBool();
            if (m.onlyMine != onlyMine)
                DbUtil.Db.SetUserPreference("savedSearchOnlyMine", m.onlyMine);
            return View(m);
        }

    }
}
