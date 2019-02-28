using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(query);
                var dt = Util.Now;
                foreach (var p in q)
                {
                    OrganizationMember.InsertOrgMembers(db2, orgId.ToInt(), p.PeopleId, MemberTypeCode.Member, dt, null, false);
                    db2.SubmitChanges();
                }
            }
            db.LogActivity($"PythonModel.AddMembersToOrg({query},{orgId})");
        }

        public void AddMemberToOrg(object pid, object orgId)
        {
            AddMembersToOrg(pid.ToString(), orgId.ToInt());
        }

        public int AddOrganization(string name, string program, string division)
        {
            using (var db2 = NewDataContext())
            {
                var prog = Organization.FetchOrCreateProgram(db2, program);
                var div = Organization.FetchOrCreateDivision(db2, prog, division);
                var neworg = new Organization
                {
                    OrganizationName = name,
                    CreatedDate = Util.Now,
                    CreatedBy = Util.UserId1,
                    OrganizationStatusId = 30,
                    DivisionId = div.Id,
                };
                db2.Organizations.InsertOnSubmit(neworg);
                db2.SubmitChanges();
                neworg.DivOrgs.Add(new DivOrg() {Organization = neworg, DivId = div.Id});
                db2.SubmitChanges();
                db2.LogActivity($"Python AddOrganization({neworg.OrganizationName}, {program}, {division})");
                return neworg.OrganizationId;
            }
        }

        public int AddOrganization(string name, int? templateid = null, bool copysettings = true)
        {
            using (var db2 = NewDataContext())
            {
                var org = db2.LoadOrganizationById(templateid);
                if (org == null)
                {
                    org = new Organization() {DivisionId = 1};
                    copysettings = false;
                }
                var neworg = new Organization
                {
                    OrganizationName = name,
                    CreatedDate = Util.Now,
                    CreatedBy = Util.UserId1,
                    EntryPointId = org.EntryPointId,
                    OrganizationTypeId = org.OrganizationTypeId,
                    CampusId = org.CampusId,
                    OrganizationStatusId = 30,
                    DivisionId = org.DivisionId,
                };
                db2.Organizations.InsertOnSubmit(neworg);
                db2.SubmitChanges();
                foreach (var div in org.DivOrgs)
                    neworg.DivOrgs.Add(new DivOrg {Organization = neworg, DivId = div.DivId});
                if (copysettings && templateid > 0)
                {
                    foreach (var sc in org.OrgSchedules)
                        neworg.OrgSchedules.Add(new OrgSchedule
                              {
                                  OrganizationId = neworg.OrganizationId,
                                  AttendCreditId = sc.AttendCreditId,
                                  SchedDay = sc.SchedDay,
                                  SchedTime = sc.SchedTime,
                                  Id = sc.Id
                              });
                    neworg.CopySettings(db2, templateid.Value);
                }
                db2.SubmitChanges();
                db2.LogActivity($"Python NewOrganization{neworg.OrganizationName} ({neworg.OrganizationId})");
                return neworg.OrganizationId;
            }
        }

        public void AddSubGroup(object pid, object orgId, string group)
        {
            using (var db2 = NewDataContext())
            {
                var om = (from mm in db2.OrganizationMembers
                    where mm.PeopleId == pid.ToInt()
                    where mm.OrganizationId == orgId.ToInt()
                    select mm).SingleOrDefault();
                if (om == null)
                    throw new Exception($"no orgmember {pid}:");
                om.AddToGroup(db2, group);
            }
        }

        public void AddSubGroupFromQuery(object query, object orgId, string group)
        {
            db.LogActivity($"PythonModel.AddSubGroupFromQuery(query,{orgId})");
            using (var db2 = NewDataContext())
            {
                var q = db2.PeopleQuery2(query);
                foreach (var p in q)
                {
                    var om = (from mm in db2.OrganizationMembers
                        where mm.PeopleId == p.PeopleId
                        where mm.OrganizationId == orgId.ToInt()
                        select mm).SingleOrDefault();
                    om?.AddToGroup(db2, group);
                }
            }
        }

        public void DeleteOrg(string name, string program, string division)
        {
            if (!HttpContextFactory.Current.User.IsInRole("developer"))
                db.LogActivity($"Python DeleteOrg({name}, {division}) denied");

            var p = db.Programs.SingleOrDefault(pp => pp.Name == program);
            if (p == null)
                return;
            var d = db.Divisions.Single(pp => pp.Name == division && pp.ProgDivs.Any(pd => pd.ProgId == p.Id));
            if (d == null)
                return;
            var q = from oo in db.Organizations
                    where oo.OrganizationName == name
                    where oo.DivOrgs.Any(dd => dd.DivId == d.Id)
                    select oo;
            if (!q.Any())
                return;
            foreach (var org in q)
                org.PurgeOrg(db);
            HttpContextFactory.Current.Session.Remove("ActiveOrganization");
            db.LogActivity($"Python DeleteOrg {name}, {program}, {division}");
        }

        public void DropOrgMember(object pid, object orgId)
        {
            db.LogActivity($"PythonModel.DropOrgMember({pid},{orgId})");
            using (var db2 = NewDataContext())
            {
                var om = db2.OrganizationMembers.Single(m => m.PeopleId == pid.ToInt() && m.OrganizationId == orgId.ToInt());
                om.Drop(db2);
                db2.SubmitChanges();
            }
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
            return om?.IsInGroup(group) ?? false;
        }

        public void JoinOrg(object orgid, object person)
        {
            int? pid = null;
            if (person is int)
                pid = person.ToInt2();
            else if (person is Person)
                pid = ((Person) person).PeopleId;
            if (pid == null)
                return;
            db.LogActivity($"PythonModel.JoinOrg({pid},{orgid})");
            using (var db2 = NewDataContext())
                OrganizationMember.InsertOrgMembers(db2, orgid.ToInt(), pid.Value, 220, Util.Now, null, false);
        }

        public void MoveToOrg(object pid, object fromOrg, object toOrg, bool? moveregdata = true, int toMemberTypeId = -1)
        {
            db.LogActivity($"PythonModel.MoveToOrg({pid},{fromOrg},{toOrg})");
            using (var db2 = NewDataContext())
                OrganizationMember.MoveToOrg(db2, pid.ToInt(), fromOrg.ToInt(), toOrg.ToInt(), moveregdata, toMemberTypeId);
        }

        public List<int> OrganizationIds(int progid, int divid, bool includeInactive = false)
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
            using (var db2 = NewDataContext())
                om.RemoveFromGroup(db2, group);
        }

        public void SetMemberType(object pid, object oid, string type)
        {
            db.LogActivity($"PythonModel.SetMemberType({pid},{oid})");
            using (var db2 = NewDataContext())
            {
                var om = db2.OrganizationMembers.Single(m => m.PeopleId == pid.ToInt() && m.OrganizationId == oid.ToInt());
                var mt = Organization.FetchOrCreateMemberType(db2, type);
                om.MemberTypeId = mt.Id;
            }
        }

        public void UpdateMainFellowship(int orgId)
        {
            using (var db2 = NewDataContext())
                db2.UpdateMainFellowship(orgId);
        }
    }
}
