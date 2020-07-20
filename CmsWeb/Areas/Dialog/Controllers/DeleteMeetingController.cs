using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "DeleteMeeting"), Route("{action}/{id?}")]
    public class DeleteMeetingController : CmsStaffController
    {
        public DeleteMeetingController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpPost, Route("~/DeleteMeeting/{id:int}")]
        public ActionResult Index(int id)
        {
            var model = new DeleteMeeting(id, CurrentDatabase);
            var org = CurrentDatabase.LoadOrganizationById(model.OrgId);
            if (!User.IsInRole("Developer"))
            {
                if (org.RegistrationTypeId == RegistrationTypeCode.Ticketing)
                    return Message("You cannot delete a meeting for a Ticketing Org, only a developer can do that");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(DeleteMeeting model)
        {
            model.UpdateLongRunningOp(CurrentDatabase, DeleteMeeting.Op);
            if (!model.Started.HasValue)
            {
                DbUtil.LogActivity($"Delete Meeting {model.MeetingId}", orgid: model.OrgId, userId: CurrentDatabase.UserPeopleId);
                model.Process(CurrentDatabase);
            }
            return View(model);
        }
    }
}
