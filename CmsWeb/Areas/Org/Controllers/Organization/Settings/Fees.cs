using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrganizationController
    {
        [HttpPost]
        public ActionResult Fees(int id)
        {
            return PartialView("Settings/Fees", getRegSettings(id));
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult FeesEdit(int id)
        {
            return PartialView("Settings/FeesEdit", getRegSettings(id));
        }
        [HttpPost]
        public ActionResult FeesUpdate(int id)
        {
            var m = getRegSettings(id);
            m.OrgFees.list.Clear();
            try
            {
                DbUtil.LogActivity("Update Fees {0}".Fmt(m.org.OrganizationName));
                UpdateModel(m);
                var os = new Settings(m.ToString(), DbUtil.Db, id);
                m.org.RegSetting = os.ToString();
                DbUtil.Db.SubmitChanges();
                if (!m.org.NotifyIds.HasValue())
                    ModelState.AddModelError("Form", needNotify);
                return PartialView("Settings/Fees", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return PartialView("Settings/FeesEdit", m);
            }
        }
    }
}