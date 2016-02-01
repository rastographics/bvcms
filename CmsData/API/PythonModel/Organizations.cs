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

        public void AddMembersToOrg(object savedQuery, object orgId)
        {
            var q = db.PeopleQuery2(savedQuery);
            var dt = DateTime.Now;
            foreach (var p in q)
            {
                var db2 = NewDataContext();
                OrganizationMember.InsertOrgMembers(db2, orgId.ToInt(), p.PeopleId, MemberTypeCode.Member, dt, null, false);
                db2.Dispose();
            }
        }

        public void AddMemberToOrg(object pid, object orgId)
        {
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

        public void JoinOrg(int orgId, Person p)
        {
            var db2 = NewDataContext();
            OrganizationMember.InsertOrgMembers(db2, orgId, p.PeopleId, 220, DateTime.Now, null, false);
            db2.Dispose();
        }

        public List<int> OrganizationIds(int progid, int divid)
        {
            var q = from o in db.Organizations
                    where progid == 0 || o.DivOrgs.Any(dd => dd.Division.ProgDivs.Any(pp => pp.ProgId == progid))
                    where divid == 0 || o.DivOrgs.Select(dd => dd.DivId).Contains(divid)
                    select o.OrganizationId;
            return q.ToList();
        }

        public Guid OrgMembersQuery(int progid, int divid, int orgid, string memberTypes)
        {
            var c = db.ScratchPadCondition();
            c.Reset();
            var mtlist = memberTypes.Split(',');
            var mts = string.Join(";", from mt in db.MemberTypes
                                       where mtlist.Contains(mt.Description)
                                       select $"{mt.Id},{mt.Code}");
            var clause = c.AddNewClause(QueryType.MemberTypeCodes, CompareType.OneOf, mts);
            clause.Program = progid.ToString();
            clause.Division = divid.ToString();
            clause.Organization = orgid.ToString();
            c.Save(db);
            return c.Id;
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
    }
}