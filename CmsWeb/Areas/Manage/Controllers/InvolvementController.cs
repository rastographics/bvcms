using System;
using System.Linq;
using CmsWeb.Lifecycle;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Manage.Models.Involvement;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,Design")]
    [ValidateInput(false)]
    [RouteArea("Manage", AreaPrefix = "Involvement"), Route("{action}/{id?}")]
    public class InvolvementController : CmsStaffController
    {
        public InvolvementController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index()
        {
            var involvementTypes = CurrentDatabase.ExecuteQuery<InvolvementTabModel.OrgType>("select * from lookup.OrganizationType");

            var model = new CustomizeInvolvementModel(involvementTypes);

            model.Current.ReadXml(CurrentDatabase.Contents.SingleOrDefault(c => c.Name == model.Current.Name)?.Body ?? Resource1.InvolvementTableCurrent);
            model.Pending.ReadXml(CurrentDatabase.Contents.SingleOrDefault(c => c.Name == model.Pending.Name)?.Body ?? Resource1.InvolvementTablePending);
            model.Previous.ReadXml(CurrentDatabase.Contents.SingleOrDefault(c => c.Name == model.Previous.Name)?.Body ?? Resource1.InvolvementTablePrevious);

            return View(model);
        }

        public JsonResult Update(InvolvementTabModel model)
        {
            var involvementTabContent = CurrentDatabase.Contents.SingleOrDefault(c => c.Name == model.Name);
            if (involvementTabContent == null)
            {
                involvementTabContent = new Content
                {
                    Title = "Edit Text Content",
                    Name = model.Name,
                    TypeID = ContentTypeCode.TypeText,
                    DateCreated = DateTime.Now
                };
                CurrentDatabase.Contents.InsertOnSubmit(involvementTabContent);
            }

            involvementTabContent.Body = model.BuildXml();

            CurrentDatabase.SubmitChanges();

            return Json(new {success = true});
        }
    }
}
