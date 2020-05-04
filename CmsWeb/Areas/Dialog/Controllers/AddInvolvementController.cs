using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [Authorize(Roles = "Edit")]
    [RouteArea("Dialog", AreaPrefix = "AddInvolvement")]
    public class AddInvolvementController : CmsStaffController
    {
        public AddInvolvementController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/AddInvolvement")]
        public ActionResult Index(bool displayCopySettings = false)
        {
            var m = new NewInvolvementModel(CurrentDatabase, CurrentDatabase.CurrentSessionOrgId, displayCopySettings);
            m.org.InvolvementName = "";
            m.org.Location = "";
            return View(m);
        }

        [HttpPost, Route("Submit/{id:int}")]
        public ActionResult Submit(int id, NewInvolvementModel m)
        {
            var org = CurrentDatabase.LoadInvolvementById(id);
            m.org.CreatedDate = Util.Now;
            m.org.CreatedBy = CurrentDatabase.UserId1;
            m.org.EntryPointId = org.EntryPointId;
            m.org.InvolvementTypeId = org.InvolvementTypeId;
            if (m.org.CampusId == 0)
            {
                m.org.CampusId = null;
            }

            if (!m.org.InvolvementName.HasValue())
            {
                m.org.InvolvementName = $"New Involvement needs a name ({Util.UserFullName})";
            }

            m.org.InvolvementStatusId = 30;
            m.org.DivisionId = org.DivisionId;

            CurrentDatabase.Involvements.InsertOnSubmit(m.org);
            CurrentDatabase.SubmitChanges();
            foreach (var div in org.DivOrgs)
            {
                m.org.DivOrgs.Add(new DivOrg { Involvement = m.org, DivId = div.DivId });
            }

            if (m.copysettings)
            {
                foreach (var sc in org.OrgSchedules)
                {
                    m.org.OrgSchedules.Add(new OrgSchedule
                    {
                        InvolvementId = m.org.InvolvementId,
                        AttendCreditId = sc.AttendCreditId,
                        SchedDay = sc.SchedDay,
                        SchedTime = sc.SchedTime,
                        Id = sc.Id
                    });
                }

                m.org.CopySettings(CurrentDatabase, id);
            }
            else
            {
                Settings os = new Settings()
                {
                    ShowDOBOnFind = true,
                    ShowPhoneOnFind = true
                };
                m.org.RegSettingXml = Util.Serialize(os);
            }

            CurrentDatabase.SubmitChanges();
            DbUtil.LogActivity($"Add new org {m.org.InvolvementName}");
            return Redirect($"/Org/{m.org.InvolvementId}");
        }
    }
}
