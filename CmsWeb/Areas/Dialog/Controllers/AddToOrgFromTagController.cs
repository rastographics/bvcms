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
        public AddToOrgFromTagController(RequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("~/AddToOrgFromTag/{qid:guid}")]
        public ActionResult Index(Guid qid)
        {
            LongRunningOperation.RemoveExisting(DbUtil.Db, qid);
            var model = new AddToOrgFromTag(qid);
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(AddToOrgFromTag model)
        {
            model.Validate(ModelState);

            if (!ModelState.IsValid) // show validation errors
            {
                return View("Index", model);
            }

            model.UpdateLongRunningOp(DbUtil.Db, AddToOrgFromTag.Op);
            if (model.ShowCount(DbUtil.Db))
            {
                return View("Index", model); // let them confirm by seeing the count and the tagname
            }

            if (!model.Started.HasValue)
            {
                DbUtil.LogActivity($"Add to org from tag for {Session["ActiveOrganization"]}");
                model.Process(DbUtil.Db);
            }

            return View(model);
        }
    }
}
