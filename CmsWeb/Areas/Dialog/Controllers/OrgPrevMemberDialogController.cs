using System;
using System.Data.Linq;
using System.Web.Mvc;
using CmsWeb.Areas.Dialog.Models;
using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using CmsWeb.Code;
using CmsWeb.Models.ExtraValues;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "OrgPrevMemberDialog"), Route("{action}")]
    public class OrgPrevMemberDialogController : CmsStaffController
    {
        [HttpPost, Route("~/OrgPrevMemberDialog/{oid}/{pid}")]
        public ActionResult Display(int oid, int pid)
        {
            var m = new OrgPrevMemberModel(oid, pid);
            return View("Display", m);
        }

        [HttpPost]
        public ActionResult Display(OrgPrevMemberModel m)
        {
            return View("Display", m);
        }
        [HttpPost, Route("ExtraValues/{oid}/{pid}")]
        public ActionResult ExtraValues(int oid, int pid)
        {
            var em = new ExtraValueModel(oid, pid, "OrgMember", "Adhoc");
            return View("Tabs/ExtraValue/Adhoc", em);
        }
    }
}
