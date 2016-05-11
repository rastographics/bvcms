using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Dialog.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [Authorize(Roles="Edit")]
    [RouteArea("Dialog", AreaPrefix = "AddResource")]
    public class AddResourceController : CmsStaffController
    {
        [Route("~/AddResource")]
        public ActionResult Index(bool displayCopySettings = false)
        {
            /*var m = new NewOrganizationModel(DbUtil.Db.CurrentOrgId0, displayCopySettings);
            m.org.OrganizationName = "";
            m.org.Location = "";
            return View(m);*/
            return null;
        }

        [HttpPost, Route("Submit/{id:int}")]
        public ActionResult Submit(int id, NewOrganizationModel m)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            m.org.CreatedDate = Util.Now;
            m.org.CreatedBy = Util.UserId1;
            m.org.EntryPointId = org.EntryPointId;
            m.org.OrganizationTypeId = org.OrganizationTypeId;
            if (m.org.CampusId == 0)
                m.org.CampusId = null;
            if (!m.org.OrganizationName.HasValue())
                m.org.OrganizationName = $"New organization needs a name ({Util.UserFullName})";
            m.org.OrganizationStatusId = 30;
            m.org.DivisionId = org.DivisionId;

            DbUtil.Db.Organizations.InsertOnSubmit(m.org);
            DbUtil.Db.SubmitChanges();
            foreach (var div in org.DivOrgs)
                m.org.DivOrgs.Add(new DivOrg { Organization = m.org, DivId = div.DivId });
            if (m.copysettings)
            {
                foreach (var sc in org.OrgSchedules)
                    m.org.OrgSchedules.Add(new OrgSchedule
                    {
                        OrganizationId = m.org.OrganizationId,
                        AttendCreditId = sc.AttendCreditId,
                        SchedDay = sc.SchedDay,
                        SchedTime = sc.SchedTime,
                        Id = sc.Id
                    });
                m.org.CopySettings(DbUtil.Db, id);
            }
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity($"Add new org {m.org.OrganizationName}");
            return Redirect($"/Org/{m.org.OrganizationId}");
        }
    }
}
