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
        public ActionResult OnlineRegFees(int id)
        {
            return PartialView("Settings/OnlineReg/Fees", GetRegSettings(id));
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult OnlineRegFeesEdit(int id)
        {
            return PartialView("Settings/OnlineReg/FeesEdit", GetRegSettings(id));
        }
        [HttpPost]
        public ActionResult OnlineRegFeesUpdate(int id)
        {
            var m = GetRegSettings(id);
            m.OrgFees.list.Clear();
            try
            {
                DbUtil.LogActivity("Update OnlineRegFees {0}".Fmt(m.org.OrganizationName));
                UpdateModel(m);
                var os = new Settings(m.ToString(), DbUtil.Db, id);
                m.org.RegSetting = os.ToString();
                DbUtil.Db.SubmitChanges();
                if (!m.org.NotifyIds.HasValue())
                    ModelState.AddModelError("Form", needNotify);
                return PartialView("Settings/OnlineReg/Fees", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return PartialView("Settings/OnlineReg/FeesEdit", m);
            }
        }
    }
}