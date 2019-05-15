using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System;
using System.Web.Mvc;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "AddToOrgFromTag"), Route("{action}/{id?}")]
    public class AddToOrgFromTagController : CmsStaffController
    {
        public AddToOrgFromTagController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("~/AddToOrgFromTag/{qid:guid}")]
        public ActionResult Index(Guid qid)
        {
            LongRunningOperation.RemoveExisting(CurrentDatabase, qid);
            var model = new AddToOrgFromTag(qid, CurrentDatabase);
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(AddToOrgFromTag model)
        {
            var op = new AddToOrgFromTag(model.QueryId, CurrentDatabase);
            op.Tag = model.Tag;
            op.Count = model.Count;
            op.Started = model.Started;
            op.Completed = model.Completed;

            op.Validate(ModelState);

            if (!ModelState.IsValid) // show validation errors
            {
                return View("Index", op);
            }

            op.UpdateLongRunningOp(CurrentDatabase, AddToOrgFromTag.Op);
            if (op.ShowCount(CurrentDatabase))
            {
                return View("Index", op); // let them confirm by seeing the count and the tagname
            }

            if (!op.Started.HasValue)
            {
                DbUtil.LogActivity($"Add to org from tag for {Session["ActiveOrganization"]}");
                op.Process(CurrentDatabase);
            }

            return View(op);
        }
    }
}
