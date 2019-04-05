using CmsWeb.Areas.Org.Models;
using CmsWeb.Lifecycle;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController : CmsStaffController
    {
        [HttpPost]
        [Authorize(Roles = "Edit")]
        [ValidateInput(false)]
        public ActionResult NewExtraValue(int id, string field, string value, bool multiline)
        {
            var m = OrganizationModel.Create(CurrentDatabase, CurrentUser);
            try
            {
                m.Org.AddEditExtra(CurrentDatabase, field, value, multiline);
                CurrentDatabase.SubmitChanges();
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
            var e = CurrentDatabase.OrganizationExtras.Single(ee => ee.OrganizationId == id && ee.Field == field);
            CurrentDatabase.OrganizationExtras.DeleteOnSubmit(e);
            CurrentDatabase.SubmitChanges();
            var m = OrganizationModel.Create(CurrentDatabase, CurrentUser);
            return PartialView("Settings/ExtrasGrid", m.Org);
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ContentResult EditExtra(string id, string value)
        {
            var a = id.SplitStr("-", 2);
            var b = a[1].SplitStr(".", 2);
            var e = CurrentDatabase.OrganizationExtras.Single(ee => ee.OrganizationId == b[1].ToInt() && ee.Field == b[0]);
            e.Data = value;
            CurrentDatabase.SubmitChanges();
            return Content(value);
        }
    }
}
