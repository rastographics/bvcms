using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Areas.Org.Models.Org;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult CommunityGroupEdit(int id)
        {
            var m = new SettingsCommunityGroupModel(id);
            return PartialView("CommunityGroup/CommunityGroupEdit", m);
        }

        [HttpPost]
        public ActionResult CommunityGroupUpdate(SettingsCommunityGroupModel m)
        {
            if (!ModelState.IsValid)
                return PartialView("CommunityGroup/CommunityGroupEdit", m);
            DbUtil.LogActivity($"Update Community Group Settings {m.Org.OrganizationName}");
            try
            {
                m.Update();
                if (!m.Org.NotifyIds.HasValue())
                    ModelState.AddModelError("Form", needNotify);
                return PartialView("CommunityGroup/CommunityGroup", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return PartialView("CommunityGroup/CommunityGroupEdit", m);
            }
        }
    }
}