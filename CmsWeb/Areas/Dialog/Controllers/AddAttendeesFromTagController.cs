using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "AddAttendeesFromTag"), Route("{action}/{id?}")]
    public class AddAttendeesFromTagController : CmsStaffController
    {
        public AddAttendeesFromTagController(IRequestManager requestManager) : base(requestManager)
        {
        }

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

            if (!ModelState.IsValid) // show validation errors
            {
                return View("Index", model);
            }

            model.UpdateLongRunningOp(CurrentDatabase, AddAttendeesFromTag.Op);
            if (model.ShowCount(CurrentDatabase))
            {
                return View("Index", model); // let them confirm by seeing the count and the tagname
            }

            if (!model.Started.HasValue)
            {
                DbUtil.LogActivity($"Add attendees from tag for {Session["ActiveOrganization"]}");
                model.Process(CurrentDatabase);
            }

            return View(model);
        }
    }
}
