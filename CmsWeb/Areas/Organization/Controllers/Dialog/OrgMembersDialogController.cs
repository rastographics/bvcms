using System.Web.Mvc;
using Areas.Org.Dialog.Models;
using CmsData;

namespace CmsWeb.Areas.Org.Dialog.Controllers
{
    // todo: use bootstrap
    [RouteArea("Organization", AreaPrefix= "OrgMembersDialog"), Route("{action}")]
	public class OrgMembersDialogController : CmsStaffController
	{
        [Route("~/OrgMembersDialog")]
		public ActionResult Index()
		{
            var co = DbUtil.Db.CurrentOrg;
            var m = new OrgMembersUpdate();
			return View(m);
		}
		[HttpPost]
		public ActionResult Update(OrgMembersUpdate m)
		{
            m.Update();
		    return View("Updated", m);
		}
		[HttpPost]
		public ActionResult ShowDrop(OrgMembersUpdate m)
		{
            return View(m);
		}
		[HttpPost]
		public ActionResult Drop(OrgMembersUpdate m)
		{
            m.Drop();
		    return View("Dropped", m);
		}
	}
}
