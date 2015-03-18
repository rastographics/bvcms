using System.Web.Mvc;
using CmsWeb.Areas.Dialog.Models;
using CmsData;
using CmsData.Codes;

namespace CmsWeb.Areas.Dialog.Controllers
{
    // todo: use bootstrap
    [RouteArea("Dialog", AreaPrefix= "OrgMembersUpdate"), Route("{action}")]
	public class OrgMembersUpdateController : CmsStaffController
	{
        [Route("~/OrgMembersUpdate")]
		public ActionResult Index()
		{
            if(DbUtil.Db.CurrentOrg == null)
                DbUtil.Db.CurrentOrg = new CurrentOrg() { GroupSelect = GroupSelectCode.Member, Id = Util2.CurrentOrgId };
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
		public ActionResult SmallGroups(OrgMembersUpdate m)
		{
            return View(m);
		}
		[HttpPost]
		public ActionResult Drop(OrgMembersUpdate m)
		{
            m.Drop();
		    return View("Dropped", m);
		}
        [HttpPost, Route("~/OrgMembersUpdate/AddSmallGroup/{id:int}")]
        public ActionResult AddSmallGroup(int id)
        {
            var m = new OrgMembersUpdate();
            m.AddSmallGroup(id);
            return Content("ok");
        }
        [HttpPost, Route("~/OrgMembersUpdate/RemoveSmallGroup/{id:int}")]
        public ActionResult RemoveSmallGroup(int id)
        {
            var m = new OrgMembersUpdate();
            m.RemoveSmallGroup(id);
            return Content("ok");
        }
	}
}
