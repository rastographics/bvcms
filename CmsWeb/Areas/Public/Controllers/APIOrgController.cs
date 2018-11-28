using CmsData;
using CmsData.API;
using CmsWeb.Lifecycle;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
    public class APIOrgController : CmsController
    {
        public APIOrgController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        public ActionResult OrganizationsForDiv(int id)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($"<Organizations error=\"{ret.Substring(1)}\" />");
            }

            var api = new APIOrganization(CurrentDatabase);
            DbUtil.LogActivity("APIOrg Organizations for Div " + id);
            return Content(api.OrganizationsForDiv(id), "text/xml");
        }

        [HttpGet]
        public ActionResult OrgMembers2(int id)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($"<OrgMembers error=\"{ret.Substring(1)}\" />");
            }

            var api = new APIOrganization(CurrentDatabase);
            DbUtil.LogActivity("APIOrg OrgMembers2 " + id);
            //            return Content(api.OrgMembersPython(id), "text/xml");
            return Content(api.OrgMembers2(id, null), "text/xml");
        }

        [HttpGet]
        public ActionResult OrgMembers(int id, string search)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($"<OrgMembers error=\"{ret.Substring(1)}\" />");
            }

            var api = new APIOrganization(CurrentDatabase);
            DbUtil.LogActivity("APIOrg OrgMembers " + id);
            return Content(api.OrgMembers(id, search), "text/xml");
        }

        [HttpGet]
        public ActionResult ExtraValues(int id, string fields)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($"<ExtraValues error=\"{ret.Substring(1)}\" />");
            }

            DbUtil.LogActivity($"APIOrg ExtraValues {id}, {fields}");
            return Content(new APIOrganization(CurrentDatabase)
                .ExtraValues(id, fields), "text/xml");
        }

        [HttpPost]
        public ActionResult AddEditExtraValue(int orgid, string field, string value)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content(ret.Substring(1));
            }

            DbUtil.LogActivity($"APIOrg AddEditExtraValue {orgid}, {field}, {value}");
            return Content(new APIOrganization(CurrentDatabase)
                .AddEditExtraValue(orgid, field, value));
        }

        [HttpPost]
        public ActionResult DeleteExtraValue(int orgid, string field)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content(ret.Substring(1));
            }

            DbUtil.LogActivity($"APIOrg DeleteExtraValue {orgid}, {field}");
            return Content(new APIOrganization(CurrentDatabase)
                .DeleteExtraValue(orgid, field));
        }

        [HttpPost]
        public ActionResult UpdateOrgMember(int OrgId, int PeopleId, string type, string enrolled, string inactive, bool? pending)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content(ret.Substring(1));
            }

            new APIOrganization(CurrentDatabase)
                .UpdateOrgMember(OrgId, PeopleId, type, enrolled.ToDate(), inactive, pending);
            DbUtil.LogActivity($"APIOrg UpdateOrgMember {OrgId}, {PeopleId}");
            return Content("ok");
        }

        [HttpPost]
        public ActionResult NewOrganization(int divId, string name, string location, int? parentOrgId, int? campusId, int? orgtype, int? leadertype, int? securitytype, string securityrole)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($@"<NewOrganization status=""error"">{ret.Substring(1)}</NewOrganization>");
            }

            DbUtil.LogActivity("APIOrganization NewOrganization");
            return Content(new APIOrganization(CurrentDatabase).NewOrganization(divId, name, location, parentOrgId, campusId, orgtype, leadertype, securitytype, securityrole), "text/xml");
        }

        [HttpPost]
        public ActionResult UpdateOrganization(int orgId, string name, string campusid, string active, string location, string description, int? orgtype, int? leadertype, int? securitytype, string securityrole, int? parentorg)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content(ret.Substring(1));
            }

            new APIOrganization(CurrentDatabase)
                .UpdateOrganization(orgId, name, campusid, active, location, description, orgtype, leadertype, securitytype, securityrole, parentorg);
            DbUtil.LogActivity($"APIOrg UpdateOrganization {orgId}");
            return Content("ok");
        }

        [HttpPost]
        public ActionResult AddDivToOrg(int orgId, int divid)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($@"<AddDivToOrg status=""error"">{ret.Substring(1)}</AddDivToOrg>");
            }

            DbUtil.LogActivity("APIOrganization AddDivToOrg");
            return Content(new APIOrganization(CurrentDatabase).AddDivToOrg(orgId, divid));
        }

        [HttpPost]
        public ActionResult RemoveDivFromOrg(int orgId, int divid)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($@"<RemoveDivFromOrg status=""error"">{ret.Substring(1)}</RemoveDivFromOrg>");
            }

            DbUtil.LogActivity("APIOrganization RemoveDivFromOrg");
            return Content(new APIOrganization(CurrentDatabase).RemoveDivFromOrg(orgId, divid));
        }

        [HttpPost]
        public ActionResult AddOrgMember(int OrgId, int PeopleId, string MemberType, bool? pending)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($@"<AddOrgMember status=""error"">{ret.Substring(1)}</AddOrgMember>");
            }

            DbUtil.LogActivity("APIOrganization AddOrgMember");
            return Content(new APIOrganization(CurrentDatabase).AddOrgMember(OrgId, PeopleId, MemberType, pending), "text/xml");
        }

        [HttpPost]
        public ActionResult DropOrgMember(int OrgId, int PeopleId, string MemberType)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($@"<DropOrgMember status=""error"">{ret.Substring(1)}</DropOrgMember>");
            }

            DbUtil.LogActivity("APIOrganization DropOrgMember");
            return Content(new APIOrganization(CurrentDatabase).DropOrgMember(OrgId, PeopleId), "text/xml");
        }

        [HttpPost]
        public ActionResult CreateProgramDivision(string program, string division)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($@"<CreateProgramDivision status=""error"">{ret.Substring(1)}</CreateProgramDivision>");
            }

            DbUtil.LogActivity("APIOrganization CreateProgramDivision");
            return Content(new APIOrganization(CurrentDatabase).CreateProgramDivision(program, division), "text/xml");
        }

        public ActionResult ParentOrgs(int id, string extravalue1, string extravalue2)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($@"<ParentOrgs status=""error"">{ret.Substring(1)}</ParentOrgs>");
            }

            DbUtil.LogActivity("APIOrganization ParentOrgs");
            return Content(new APIOrganization(CurrentDatabase).ParentOrgs(id, extravalue1, extravalue2), "text/xml");
        }

        public ActionResult ChildOrgs(int id, string extravalue1, string extravalue2)
        {
            var ret = AuthenticateDeveloper();
            if (ret.StartsWith("!"))
            {
                return Content($@"<ChildOrgs status=""error"">{ret.Substring(1)}</ChildOrgs>");
            }

            DbUtil.LogActivity("APIOrganization ChildOrgs");
            return Content(new APIOrganization(CurrentDatabase).ChildOrgs(id, extravalue1, extravalue2), "text/xml");
        }

        public ActionResult EmailReminders(int id)
        {
            var ret = AuthenticateDeveloper();
            return Content($@"<EmailReminders status=""error"">No Longer supported</EmailReminders>");
        }
    }
}
