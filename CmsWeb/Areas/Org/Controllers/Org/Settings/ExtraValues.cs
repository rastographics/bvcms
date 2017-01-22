using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        [HttpPost]
        [Authorize(Roles = "Edit")]
        [ValidateInput(false)]
        public ActionResult NewExtraValue(int id, string field, string value, bool multiline)
        {
            var m = new OrganizationModel();
            try
            {
                m.Org.AddEditExtra(DbUtil.Db, field, value, multiline);
                DbUtil.Db.SubmitChanges();
            }
            catch (Exception ex)
            {
                return Content("error: " + ex.Message);
            }
            return PartialView("Settings/ExtrasGrid", m.Org);
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult DeleteExtra(int id, string field)
        {
            var e = DbUtil.Db.OrganizationExtras.Single(ee => ee.OrganizationId == id && ee.Field == field);
            DbUtil.Db.OrganizationExtras.DeleteOnSubmit(e);
            DbUtil.Db.SubmitChanges();
            var m = new OrganizationModel();
            return PartialView("Settings/ExtrasGrid", m.Org);
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ContentResult EditExtra(string id, string value)
        {
            var a = id.SplitStr("-", 2);
            var b = a[1].SplitStr(".", 2);
            var e = DbUtil.Db.OrganizationExtras.Single(ee => ee.OrganizationId == b[1].ToInt() && ee.Field == b[0]);
            e.Data = value;
            DbUtil.Db.SubmitChanges();
            return Content(value);
        }
    }
}