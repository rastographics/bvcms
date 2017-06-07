using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Dialog.Models;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix="AddAttendeesFromTag"), Route("{action}/{id?}")]
    public class AddAttendeesFromTagController : CmsStaffController
    {
        [HttpPost, Route("~/AddAttendeesFromTag/{id:int}")]
        public ActionResult Index(int id)
        {
            var model = new AddAttendeesFromTag(id);
            return View(model);
        }
        [HttpPost]
        public ActionResult Process(AddAttendeesFromTag model)
        {
            model.Validate(ModelState);

            if(!ModelState.IsValid) // show validation errors
                return View("Index", model);

            model.UpdateLongRunningOp(DbUtil.Db, AddAttendeesFromTag.Op);
            if(model.ShowCount(DbUtil.Db))
                return View("Index", model); // let them confirm by seeing the count and the tagname

            if (!model.Started.HasValue)
            {
                DbUtil.LogActivity($"Add attendees from tag for {Session["ActiveOrganization"]}");
                model.Process(DbUtil.Db);
            }

            return View(model);
        }
    }
}
