using System;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Dialog.Models;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix="OrgDrop"), Route("{action}/{id?}")]
    public class OrgDropController : CmsStaffController
    {
        [HttpPost, Route("~/OrgDrop/{id:int}")]
        public ActionResult Index(int id)
        {
            if (id != DbUtil.Db.CurrentOrgId0)
                throw new Exception($"Current org has changed from {id} to {DbUtil.Db.CurrentOrgId0}, aborting");
            var model = new OrgDrop(id);
            model.RemoveExistingLop(DbUtil.Db, id, OrgDrop.Op);
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(OrgDrop model)
        {
            model.UpdateLongRunningOp(DbUtil.Db, OrgDrop.Op);
            if (!model.Started.HasValue)
                model.Process(DbUtil.Db);
            return View(model);
        }

        [HttpPost]
        public ActionResult DropSingleMember(int orgId, int peopleId)
        {
            var model = new OrgDrop();
            model.DropSingleMember(orgId, peopleId);
            return Content("ok");
        }
    }
}
