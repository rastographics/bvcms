using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
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
            var db = DbUtil.Db;
            db.CurrentOrg = new CurrentOrg {Id = id, GroupSelect = GroupSelectCode.Member};

            var m = new OrganizationModel(id);
            if (peopleid.HasValue)
                m.NameFilter = peopleid.ToString();

            if (m.Org == null)
                return Content("organization not found");

            if (Util2.OrgLeadersOnly)
            {
                var oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
                if (!oids.Contains(m.Org.OrganizationId))
                    return NotAllowed("You must be a leader of this organization", m.Org.OrganizationName);
                var sgleader = DbUtil.Db.SmallGroupLeader(id, Util.UserPeopleId);
                if (sgleader.HasValue())
                {
                    db.CurrentOrg.SgFilter = sgleader;
                    m.SgFilter = sgleader;
                }
            }
            if (m.Org.LimitToRole.HasValue())
                if (!User.IsInRole(m.Org.LimitToRole))
                    return NotAllowed("no privilege to view ", m.Org.OrganizationName);

            DbUtil.LogOrgActivity($"Viewing Org({m.Org.OrganizationName})", id, m.Org.OrganizationName);

            ViewBag.OrganizationContext = true;
            ViewBag.orgname = m.Org.FullName;
            ViewBag.model = m;
            ViewBag.selectmode = 0;
            InitExportToolbar(id);
            m.GroupSelect = "10";
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
            var org = DbUtil.Db.LoadOrganizationById(id);
            if (org == null)
                return Content("error, bad orgid");
            if (id == 1)
                return Content("Cannot delete first org");
            if (!org.PurgeOrg(DbUtil.Db))
                return Content("error, not deleted");
            var currorg = Util2.CurrentOrganization;
            currorg.Id = 0;
            currorg.SgFilter = null;
            DbUtil.LogActivity($"Delete Org {Session["ActiveOrganization"]}");
            Session.Remove("ActiveOrganization");
            return Content("ok");
        }

        private void InitExportToolbar(int oid)
        {
            ViewBag.oid = oid;
            var qid = DbUtil.Db.QueryInCurrentOrg().QueryId;
            ViewBag.queryid = qid;
            ViewBag.TagAction = "/Org/TagAll/" + qid;
            ViewBag.UnTagAction = "/Org/UnTagAll/" + qid;
            ViewBag.AddContact = "/Org/AddContact/" + qid;
            ViewBag.AddTasks = "/Org/AddTasks/" + qid;
            ViewBag.OrganizationContext = true;
            if (!DbUtil.Db.Organizations.Any(oo => oo.ParentOrgId == oid))
                return;
            ViewBag.ParentOrgContext = true;
            ViewBag.leadersqid = DbUtil.Db.QueryLeadersUnderCurrentOrg().QueryId;
            ViewBag.membersqid = DbUtil.Db.QueryMembersUnderCurrentOrg().QueryId;
        }

        [HttpPost]
        public ActionResult Settings(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView(m);
        }

        [HttpPost]
        public ActionResult Registrations(int id)
        {
            var m = new OrganizationModel(id);
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

        [HttpPost]
        public ActionResult CommunityGroup(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView(m);
        }

        [HttpPost]
        public ActionResult AddContactReceived(int id)
        {
            var o = DbUtil.Db.LoadOrganizationById(id);
            DbUtil.LogPersonActivity($"Adding contact to organization: {o.FullName}", id, o.FullName);
            var c = new Contact
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                ContactDate = Util.Now.Date,
                OrganizationId = o.OrganizationId
            };

            var defaultRole = DbUtil.Db.Setting("Contacts-DefaultRole", null);
            if (!string.IsNullOrEmpty(defaultRole) && DbUtil.Db.Roles.Any(x => x.RoleName == defaultRole))
                c.LimitToRole = defaultRole;

            DbUtil.Db.Contacts.InsertOnSubmit(c);
            DbUtil.Db.SubmitChanges();

            c.contactsMakers.Add(new Contactor { PeopleId = Util.UserPeopleId.Value });
            DbUtil.Db.SubmitChanges();

            TempData["ContactEdit"] = true;
            return Content($"/Contact2/{c.ContactId}");
        }
    }
}
