using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix = "Org"), Route("{action}/{id?}")]
    [ValidateInput(false)]
    [SessionExpire]
    public partial class OrgController : CmsStaffController
    {
        private const string needNotify = "WARNING: please add the notify persons on messages tab.";

        [HttpGet, Route("~/Org/{id:int}")]
        public ActionResult Index(int id, int? peopleid = null)
        {
            if (id == 0)
            {
                var recent = Util2.MostRecentOrgs;
                id = recent.Any() ? recent[0].Id : 1;
                return Redirect($"/Org/{id}");
            }
            var m = OrganizationModel.Create(CurrentDatabase, CurrentUser);
            m.OrgId = id;
            if (peopleid.HasValue)
                m.NameFilter = peopleid.ToString();

            if (m.Org == null)
                return Content("organization not found");

            if (Util2.OrgLeadersOnly)
            {
                var oids = CurrentDatabase.GetLeaderOrgIds(Util.UserPeopleId);
                if (!oids.Contains(m.Org.OrganizationId))
                    return NotAllowed("You must be a leader of this organization", m.Org.OrganizationName);
                var sgleader = CurrentDatabase.SmallGroupLeader(id, Util.UserPeopleId);
                if (sgleader.HasValue())
                    m.SgFilter = sgleader;
            }
            if (m.Org.LimitToRole.HasValue())
                if (!User.IsInRole(m.Org.LimitToRole))
                    return NotAllowed("no privilege to view ", m.Org.OrganizationName);

            DbUtil.LogOrgActivity($"Viewing Org({m.Org.OrganizationName})", id, m.Org.OrganizationName);

            m.OrgMain.Divisions = GetOrgDivisions(id);

            ViewBag.OrganizationContext = true;
            ViewBag.orgname = m.Org.FullName;
            ViewBag.model = m;
            ViewBag.selectmode = 0;
            InitExportToolbar(m);
            Session["ActiveOrganization"] = m.Org.OrganizationName;
            return View(m);
        }

        private ActionResult NotAllowed(string error, string name)
        {
            DbUtil.LogActivity($"Trying to view Organization ({name})");
            return Content($"<h3 style='color:red'>{error}</h3>\n<a href='{"javascript: history.go(-1)"}'>{"Go Back"}</a>");
        }

        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var org = CurrentDatabase.LoadOrganizationById(id);
            if (org == null)
                return Content("error, bad orgid");
            if (id == 1)
                return Content("Cannot delete first org");
            var err = org.PurgeOrg(CurrentDatabase);
            if(err.HasValue())
                return Content($"error, {err}");
            DbUtil.LogActivity($"Delete Org {Session["ActiveOrganization"]}");
            Session.Remove("ActiveOrganization");
            return Content("ok");
        }

        private void InitExportToolbar(OrganizationModel m)
        {
            ViewBag.oid = m.Id;
            ViewBag.queryid = m.QueryId;
            ViewBag.TagAction = "/Org/TagAll/" + m.QueryId;
            ViewBag.UnTagAction = "/Org/UnTagAll/" + m.QueryId;
            ViewBag.AddContact = "/Org/AddContact/" + m.QueryId;
            ViewBag.AddTasks = "/Org/AddTasks/" + m.QueryId;
            ViewBag.OrganizationContext = true;
            if (!CurrentDatabase.Organizations.Any(oo => oo.ParentOrgId == m.Id))
                return;

            ViewBag.ParentOrgContext = true;
            ViewBag.leadersqid = CurrentDatabase.QueryLeadersUnderCurrentOrg().QueryId;
            ViewBag.membersqid = CurrentDatabase.QueryMembersUnderCurrentOrg().QueryId;
        }

        [HttpPost]
        public ActionResult Settings(int id)
        {
            var m = OrganizationModel.Create(CurrentDatabase, CurrentUser);
            m.OrgId = id;
            return PartialView(m);
        }

        [HttpPost]
        public ActionResult Registrations(int id)
        {
            var m = OrganizationModel.Create(CurrentDatabase, CurrentUser);
            m.OrgId = id;
            return PartialView(m);
        }

        [HttpPost]
        public ActionResult ContactsReceived(int id)
        {
            var m = new ContactsReceivedModel
            {
                OrganizationId = id
            };

            return PartialView("Contacts", m);
        }

        /*[HttpPost]
        public ActionResult Resources(int id)
        {
            var resources = new List<CmsData.Resource.Resource>();
            resources.Add(new CmsData.Resource.Resource
            {
                Name = "South America Mission Goals",
                UpdatedTime = DateTime.Now.AddDays(-22)
            });
            resources.Add(new CmsData.Resource.Resource
            {
                Name = "Trip Budget",
                UpdatedTime = DateTime.Now.AddDays(-12)
            });
            
            return PartialView(resources);
        }*/

        [HttpPost]
        public ActionResult CommunityGroup(int id)
        {
            var m = OrganizationModel.Create(CurrentDatabase, CurrentUser);
            m.OrgId = id;
            return PartialView(m);
        }

        [HttpPost]
        public ActionResult AddContactReceived(int id)
        {
            var o = CurrentDatabase.LoadOrganizationById(id);
            DbUtil.LogPersonActivity($"Adding contact to organization: {o.FullName}", id, o.FullName);
            var c = new Contact
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                ContactDate = Util.Now.Date,
                OrganizationId = o.OrganizationId
            };

            CurrentDatabase.Contacts.InsertOnSubmit(c);
            CurrentDatabase.SubmitChanges();

            c.contactsMakers.Add(new Contactor { PeopleId = Util.UserPeopleId.Value });
            CurrentDatabase.SubmitChanges();

            var defaultRole = CurrentDatabase.Setting("Contacts-DefaultRole", null);
            if (!string.IsNullOrEmpty(defaultRole) && CurrentDatabase.Roles.Any(x => x.RoleName == defaultRole))
                TempData["SetRole"] = defaultRole;

            TempData["ContactEdit"] = true;
            return Content($"/Contact2/{c.ContactId}");
        }
    }
}
