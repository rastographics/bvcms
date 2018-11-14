using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "OrgPrevMemberDialog"), Route("{action}")]
    public class OrgPrevMemberDialogController : CmsStaffController
    {
        public OrgPrevMemberDialogController(IRequestManager requestManager) : base(requestManager)
        {
        }

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
    }
}
