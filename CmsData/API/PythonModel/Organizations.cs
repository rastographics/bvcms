using System;
using System.Collections.Generic;
using System.Linq;
using CmsData.API;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public int CurrentOrgId { get; set; }

        public void AddMembersToOrg(object query, object orgId)
        {
            var q = db.PeopleQuery2(query);
            var dt = DateTime.Now;
            db.LogActivity($"PythonModel.AddMembersToOrg(query,{orgId})");
            foreach (var p in q)
            {
                var db2 = NewDataContext();
                OrganizationMember.InsertOrgMembers(db2, orgId.ToInt(), p.PeopleId, MemberTypeCode.Member, dt, null, false);
                db2.Dispose();
            }
        }

        public void AddMemberToOrg(object pid, object orgId)
        {
            db.LogActivity($"PythonModel.AddMemberToOrg({pid},{orgId})");
            AddMembersToOrg(pid.ToInt(), orgId.ToInt());
        }

        public void AddSubGroup(object pid, object orgId, string group)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            if (om == null)
                throw new Exception($"no orgmember {pid}:");
            var db2 = NewDataContext();
            om.AddToGroup(db2, group);
            db2.Dispose();
        }

        public APIOrganization.Organization GetOrganization(object orgId)
        {
            var api = new APIOrganization(db);
            return api.GetOrganization(orgId.ToInt());
        }

        public bool InOrg(object pid, object orgId)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            return om != null;
        }

        public bool InSubGroup(object pid, object orgId, string group)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            if (om == null)
                return false;

            return om.IsInGroup(group);
        }

        public void JoinOrg(int orgid, object person)
        {
            var db2 = NewDataContext();

            int? pid = null;
            if (person is int)
                pid = person.ToInt2();
            else if (person is Person)
                pid = ((Person) person).PeopleId;
            db.LogActivity($"PythonModel.JoinOrg({pid},{orgid})");
            if (pid == null)
                return;
            OrganizationMember.InsertOrgMembers(db2, orgid, pid.Value, 220, DateTime.Now, null, false);

            db2.Dispose();
        }

        public void MoveToOrg(int pid, int fromOrg, int toOrg)
        {
            var db2 = NewDataContext();
            if (fromOrg == toOrg)
                return;
            db.LogActivity($"PythonModel.MoveToOrg({pid},{fromOrg},{toOrg})");
            var om = db2.OrganizationMembers.Single(m => m.PeopleId == pid && m.OrganizationId == fromOrg);
            var tom = db2.OrganizationMembers.SingleOrDefault(m => m.PeopleId == pid && m.OrganizationId == toOrg);
            if (tom == null)
            {
                tom = OrganizationMember.InsertOrgMembers(db2,
                    toOrg, pid, MemberTypeCode.Member, DateTime.Now, null, om.Pending ?? false);
                if (tom == null)
                    return;
            }
            tom.UserData = om.UserData;
            tom.MemberTypeId = om.MemberTypeId;
            tom.ShirtSize = om.ShirtSize;
            if (om.OrganizationId != tom.OrganizationId)
                tom.Moved = true;
            om.Drop(db2);
            db2.SubmitChanges();
        }

        public List<int> OrganizationIds(int progid, int divid, bool? includeInactive)
        {
            var q = from o in db.Organizations
                    where progid == 0 || o.DivOrgs.Any(dd => dd.Division.ProgDivs.Any(pp => pp.ProgId == progid))
                    where divid == 0 || o.DivOrgs.Select(dd => dd.DivId).Contains(divid)
                    where includeInactive == true || o.OrganizationStatusId == OrgStatusCode.Active
                    select o.OrganizationId;
            return q.ToList();
        }

        public void RemoveSubGroup(object pid, object orgId, string group)
        {
            var om = (from mm in db.OrganizationMembers
                      where mm.PeopleId == pid.ToInt()
                      where mm.OrganizationId == orgId.ToInt()
                      select mm).SingleOrDefault();
            if (om == null)
                throw new Exception($"no orgmember {pid}:");
            var db2 = NewDataContext();
            om.RemoveFromGroup(db2, group);
            db2.Dispose();
        }

        public void UpdateMainFellowship(int orgId)
        {
            var db2 = NewDataContext();
            db2.UpdateMainFellowship(orgId);
        }

        public void DropOrgMember(int pid, int orgId)
        {
            var db2 = NewDataContext();
            db.LogActivity($"PythonModel.DropOrgMember({pid},{orgId})");
            var om = db2.OrganizationMembers.Single(m => m.PeopleId == pid && m.OrganizationId == orgId);
            om.Drop(db2);
        }
    }
}