using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Text;
using System.Linq;
using System.Xml.Serialization;
using CmsData.Codes;
using CmsData.OnlineRegSummaryText;
using CmsData.Registration;
using HandlebarsDotNet;
using UtilityExtensions;

namespace CmsData.API
{
    public class APIOrganization
    {
        private CMSDataContext Db;

        public APIOrganization(CMSDataContext Db)
        {
            this.Db = Db;
        }
        public string OrganizationsForDiv(int divid)
        {
            var q = from o in Db.Organizations
                    where o.DivOrgs.Any(dd => dd.DivId == divid)
                    where o.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Active
                    let leader = Db.People.SingleOrDefault(ll => ll.PeopleId == o.LeaderId)
                    select new
                    {
                        o.OrganizationId,
                        o.OrganizationName,
                        o.Location,
                        o.Description,
                        o.CampusId,
                        o.LeaderName,
                        o.LeaderId,
                        Email = leader != null ? leader.EmailAddress : "",
                        IsParent = o.ChildOrgs.Count() > 0,
                        NumMembers = o.OrganizationMembers.Count(om => om.Pending != true && om.MemberTypeId != CmsData.Codes.MemberTypeCode.InActive)
                    };
            var w = new APIWriter();

            w.Start("Organizations");
            foreach (var o in q)
            {
                w.Start("Organization");
                w.Attr("Id", o.OrganizationId);
                w.Attr("Name", o.OrganizationName);
                w.Attr("NumMembers", o.NumMembers);
                if (o.IsParent)
                    w.Attr("IsParent", o.IsParent);
                w.Attr("Location", o.Location);
                w.Attr("Description", o.Description);
                w.Attr("CampusId", o.CampusId);
                w.Attr("Leader", o.LeaderName);
                w.Attr("LeaderId", o.LeaderId);
                w.Attr("Email", o.Email);
                w.End();
            }
            w.End();
            return w.ToString();
        }
        public class OrgMemberInfo
        {
            public OrganizationMember member { get; set; }
            public Person person { get; set; }
            public IEnumerable<string> tags { get; set; }
        }
        public List<OrgMemberInfo> OrgMembersData(int orgid)
        {
            // load data, do in memory joins
            var qm = (from m in Db.OrganizationMembers
                      where m.OrganizationId == orgid
                      where m.MemberTypeId != Codes.MemberTypeCode.InActive
                      select new { m, m.Person }).ToList();
            var mt = (from m in Db.OrgMemMemTags
                      where m.OrganizationMember.OrganizationId == orgid
                      where m.OrganizationMember.MemberTypeId != Codes.MemberTypeCode.InActive
                      select new { m.OrganizationMember.PeopleId, m.MemberTag.Name }).ToList();
            var q = from i in qm
                    select new OrgMemberInfo
                    {
                        member = i.m,
                        person = i.Person,
                        tags = from t in mt
                               where t.PeopleId == i.m.PeopleId
                               select t.Name,
                    };
            return q.ToList();
        }
        //        public string OrgMembersPython(int orgid)
        //        {
        //            var list = OrgMembersData(orgid);
        //            var script = Db.Content("API-OrgMembers");
        //            if (script == null)
        //            {
        //                script = new Content();
        //                script.Body = @"
        //from System import *
        //from System.Text import *
        //
        //class OrgMembers(object):
        //
        //	def Run(self, m, w, q):
        //		w.Start('OrgMembers')
        //		for i in q:
        //			w.Start('Member')
        //			w.Attr('PeopleId', i.member.PeopleId)
        //			w.Attr('Name', i.member.Person.Name)
        //			w.Attr('PreferredName', i.member.Person.PreferredName)
        //			w.Attr('LastName', i.member.Person.LastName)
        //			w.Attr('Email', i.member.Person.EmailAddress)
        //			w.Attr('Enrolled', i.member.EnrollmentDate)
        //			w.Attr('MemberType', i.member.MemberType.Description)
        //			for t in i.tags:
        //				w.Add('Group', t)
        //			w.End()
        //		w.End()
        //		return w.ToString()
        //";
        //            }
        //            if (script == null)
        //                return "<login error=\"no API-OrgMembers script\" />";
        //            var engine = Python.CreateEngine();
        //            var sc = engine.CreateScriptSourceFromString(script.Body);
        //            try
        //            {
        //                var code = sc.Compile();
        //                var scope = engine.CreateScope();
        //                code.Execute(scope);
        //
        //                dynamic LoginInfo = scope.GetVariable("OrgMembers");
        //                dynamic m = LoginInfo();
        //                var w = new APIWriter();
        //                return m.Run(this, w, list);
        //            }
        //            catch (Exception ex,
        //            {
        //                return $"<login error=\"API-OrgMembers script error: {ex.Message}\" />";
        //            }
        //        }

        public string OrgMembers2(int orgid, string search)
        {
            search = search ?? "";
            var nosearch = !search.HasValue();
            var qm = from m in Db.OrganizationMembers
                     where m.OrganizationId == orgid
                     where nosearch || m.Person.Name2.StartsWith(search)
                     select new
                     {
                         m.PeopleId,
                         m.Person.Name,
                         First = m.Person.PreferredName,
                         Last = m.Person.LastName,
                         m.Person.EmailAddress,
                         m.EnrollmentDate,
                         MemberType = m.MemberType.Description,
                         IsLeaderType = (m.MemberType.AttendanceTypeId ?? 0) == CmsData.Codes.AttendTypeCode.Leader,
                     };
            var mt = from m in Db.OrgMemMemTags
                     where m.OrganizationMember.OrganizationId == orgid
                     where m.OrganizationMember.MemberTypeId != Codes.MemberTypeCode.InActive
                     select new
                     {
                         m.OrganizationMember.PeopleId,
                         m.MemberTag.Name
                     };
            var mtags = mt.ToList();

            var w = new APIWriter();
            w.Start("OrgMembers");
            foreach (var m in qm.ToList())
            {
                w.Start("Member");
                w.Attr("PeopleId", m.PeopleId);
                w.Attr("Name", m.Name);
                w.Attr("PreferredName", m.First);
                w.Attr("LastName", m.Last);
                w.Attr("Email", m.EmailAddress);
                w.Attr("Enrolled", m.EnrollmentDate);
                w.Attr("MemberType", m.MemberType);
                if (m.IsLeaderType)
                    w.Attr("IsLeader", m.IsLeaderType);
                var qt = from t in mtags
                         where t.PeopleId == m.PeopleId
                         select t.Name;
                foreach (var group in qt)
                    w.Add("Group", group);
                w.End();
            }
            w.End();
            return w.ToString();
        }
        public string OrgMembers(int orgid, string search)
        {
            search = search ?? "";
            var nosearch = !search.HasValue();
            var qm = from m in Db.OrganizationMembers
                     where m.OrganizationId == orgid
                     where nosearch || m.Person.Name2.StartsWith(search)
                     select new
                     {
                         m.PeopleId,
                         First = m.Person.PreferredName,
                         Last = m.Person.LastName,
                         m.Person.EmailAddress,
                         m.EnrollmentDate,
                         MemberType = m.MemberType.Description,
                         IsLeaderType = (m.MemberType.AttendanceTypeId ?? 0) == CmsData.Codes.AttendTypeCode.Leader,
                     };
            var mt = from m in Db.OrgMemMemTags
                     where m.OrganizationMember.OrganizationId == orgid
                     where m.OrganizationMember.MemberTypeId != Codes.MemberTypeCode.InActive
                     select new
                     {
                         m.OrganizationMember.PeopleId,
                         m.MemberTag.Name
                     };
            var mtags = mt.ToList();

            var w = new APIWriter();
            w.Start("OrgMembers");
            foreach (var m in qm.ToList())
            {
                w.Start("Member");
                w.Attr("PreferredName", m.First);
                w.Attr("LastName", m.Last);
                w.Attr("Email", m.EmailAddress);
                w.Attr("Enrolled", m.EnrollmentDate);
                w.Attr("MemberType", m.MemberType);
                if (m.IsLeaderType)
                    w.Attr("IsLeader", m.IsLeaderType);
                var qt = from t in mtags
                         where t.PeopleId == m.PeopleId
                         select t.Name;
                foreach (var group in qt)
                    w.Add("Group", group);
                w.End();
            }
            w.End();
            return w.ToString();
        }
        public string UpdateOrgMember(int orgid, int peopleid, string MemberType, DateTime? EnrollDate, string InactiveDate, bool? pending)
        {
            try
            {
                var om = Db.OrganizationMembers.Single(mm =>
                    mm.OrganizationId == orgid
                    && mm.PeopleId == peopleid);
                if (MemberType.HasValue())
                {
                    var mt = CmsData.Organization.FetchOrCreateMemberType(Db, MemberType);
                    om.MemberTypeId = mt.Id;
                }
                if (EnrollDate.HasValue)
                    om.EnrollmentDate = EnrollDate;
                if (pending.HasValue)
                    om.Pending = pending;

                var d = InactiveDate.ToDate();
                if (d.HasValue)
                    om.InactiveDate = d;
                else if (InactiveDate == "null")
                    om.InactiveDate = null;

                Db.SubmitChanges();

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string DeleteExtraValue(int orgid, string field)
        {
            try
            {
                var q = from v in Db.OrganizationExtras
                        where v.Field == field
                        where v.OrganizationId == orgid
                        select v;
                Db.OrganizationExtras.DeleteAllOnSubmit(q);
                Db.SubmitChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string ExtraValues(int orgid, string fields)
        {
            try
            {
                var a = (fields ?? "").Split(',');
                var nofields = !fields.HasValue();
                var q = from v in Db.OrganizationExtras
                        where nofields || a.Contains(v.Field)
                        where v.OrganizationId == orgid
                        select v;
                var w = new APIWriter();
                w.Start("ExtraOrgValues");
                w.Attr("Id", orgid);
                foreach (var v in q)
                    w.Add(v.Field, v.Data);
                w.End();
                return w.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string AddEditExtraValue(int orgid, string field, string value)
        {
            try
            {
                var q = from v in Db.OrganizationExtras
                        where v.Field == field
                        where v.OrganizationId == orgid
                        select v;
                var ev = q.SingleOrDefault();
                if (ev == null)
                {
                    ev = new OrganizationExtra
                    {
                        OrganizationId = orgid,
                        Field = field,
                    };
                    Db.OrganizationExtras.InsertOnSubmit(ev);
                }
                ev.Data = value;
                Db.SubmitChanges();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string NewOrganization(int divId, string name, string location, int? parentOrgId, int? campusId, int? orgtype, int? leadertype, int? securitytype, string securityrole)
        {
            try
            {
                var d = Db.Divisions.Single(dd => dd.Id == divId);
                if (d == null)
                    throw new Exception("no division " + divId);
                var o = CmsData.Organization.CreateOrganization(Db, d, name);
                o.ParentOrgId = parentOrgId;
                o.Location = location;
                o.CampusId = campusId;
                o.LimitToRole = securityrole;
                o.LeaderMemberTypeId = leadertype;
                o.OrganizationTypeId = orgtype;
                o.SecurityTypeId = securitytype ?? 0;
                Db.SubmitChanges();
                return $@"<NewOrganization id=""{o.OrganizationId}"" status=""ok""></NewOrganization>";
            }
            catch (Exception ex)
            {
                return $@"<NewOrganization status=""error"">{HttpUtility.HtmlEncode(ex.Message)}</NewOrganization>";
            }
        }

        public string AddDivToOrg(int orgid, int divid)
        {
            var div = Db.DivOrgs.SingleOrDefault(dd => dd.DivId == divid && dd.OrgId == orgid);
            if (div == null)
                Db.DivOrgs.InsertOnSubmit(new DivOrg { DivId = divid, OrgId = orgid });
            var o = Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == orgid && oo.DivisionId == null);
            if (o != null)
                o.DivisionId = divid;
            Db.SubmitChanges();
            return "ok";
        }

        public string RemoveDivFromOrg(int orgid, int divid)
        {
            var div = Db.DivOrgs.SingleOrDefault(dd => dd.DivId == divid && dd.OrgId == orgid);
            if (div != null)
                Db.DivOrgs.DeleteOnSubmit(div);
            var o = Db.Organizations.SingleOrDefault(oo => oo.OrganizationId == orgid && oo.DivisionId == divid);
            if (o != null)
                o.DivisionId = null;
            Db.SubmitChanges();
            return "ok";
        }

        public string UpdateOrganization(int orgid, string name, string campusid, string active, string location, string description, int? orgtype, int? leadertype, int? securitytype, string securityrole, int? parentorg)
        {
            try
            {
                var o = Db.Organizations.Single(oo => oo.OrganizationId == orgid);
                if (name.HasValue())
                    o.OrganizationName = name;
                o.CampusId = campusid.ToInt2();
                if (active.HasValue())
                    o.OrganizationStatusId = active.ToBool() ? Codes.OrgStatusCode.Active : Codes.OrgStatusCode.Inactive;
                o.Location = location;
                o.Description = description;

                if (securityrole != null)
                    o.LimitToRole = securityrole;
                if (leadertype.HasValue)
                    o.LeaderMemberTypeId = leadertype;
                if (orgtype.HasValue)
                    o.OrganizationTypeId = orgtype;
                if (securitytype.HasValue)
                    o.SecurityTypeId = securitytype.Value;
                if (parentorg.HasValue)
                    o.ParentOrgId = parentorg == 0 ? null : parentorg;

                Db.SubmitChanges();

                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string AddOrgMember(int OrgId, int PeopleId, string MemberType, bool? pending)
        {
            try
            {
                if (!MemberType.HasValue())
                    MemberType = "Member";
                var mt = CmsData.Organization.FetchOrCreateMemberType(Db, MemberType);
                OrganizationMember.InsertOrgMembers(Db, OrgId, PeopleId, mt.Id, DateTime.Now, null, pending ?? false);
                return @"<AddOrgMember status=""ok"" />";
            }
            catch (Exception ex)
            {
                return $@"<AddOrgMember status=""error"">{HttpUtility.HtmlEncode(ex.Message)}</AddOrgMember>";
            }
        }
        public string DropOrgMember(int OrgId, int PeopleId)
        {
            try
            {
                var om = Db.OrganizationMembers.SingleOrDefault(mm => mm.OrganizationId == OrgId && mm.PeopleId == PeopleId);
                if (om == null)
                    throw new Exception("no orgmember");
                om.Drop(Db, DateTime.Now);
                Db.SubmitChanges();
                return @"<DropOrgMember status=""ok"" />";
            }
            catch (Exception ex)
            {
                return $@"<DropOrgMember status=""error"">{HttpUtility.HtmlEncode(ex.Message)}</DropOrgMember>";
            }
        }
        public string CreateProgramDivision(string program, string division)
        {
            try
            {
                var p = CmsData.Organization.FetchOrCreateProgram(Db, program);
                var d = CmsData.Organization.FetchOrCreateDivision(Db, p, division);
                return $@"<CreateProgramDivsion status=""ok"" progid=""{p.Id}"" divid=""{d.Id}"" />";
            }
            catch (Exception ex)
            {
                return
                    $@"<CreateProgramDivision status=""error"">{HttpUtility.HtmlEncode(ex.Message)}</CreateProgramDivision>";
            }
        }

        [Serializable]
        public class Member
        {
            [XmlAttribute]
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string email { get; set; }
        }
        [Serializable]
        public class Organization
        {
            [XmlAttribute]
            public int id { get; set; }
            public string name { get; set; }
            public string location { get; set; }
            public string description { get; set; }
            public string extravalue1 { get; set; }
            public string extravalue2 { get; set; }
            public List<Member> members { get; set; }
        }
        [Serializable]
        public class Organizations
        {
            [XmlAttribute]
            public string status { get; set; }
            public List<Organization> List { get; set; }
        }

        public Organization GetOrganization(int id)
        {
            var org = (from o in Db.Organizations
                       where o.OrganizationId == id
                       select new Organization
                       {
                           id = o.OrganizationId,
                           name = o.OrganizationName,
                           location = o.Location,
                           description = o.Description,
                       }).SingleOrDefault();
            return org;
        }
        public string ParentOrgs(int id, string extravalue1, string extravalue2)
        {
            try
            {
                var q = from o in Db.Organizations
                        where o.ChildOrgs.Any()
                        where o.DivisionId == id
                        select new Organization
                        {
                            id = o.OrganizationId,
                            name = o.OrganizationName,
                            location = o.Location,
                            description = o.Description,
                            extravalue1 = (from ev in o.OrganizationExtras
                                           where ev.Field == extravalue1
                                           select ev.Data).SingleOrDefault(),
                            extravalue2 = (from ev in o.OrganizationExtras
                                           where ev.Field == extravalue2
                                           select ev.Data).SingleOrDefault(),
                            members = (from m in o.OrganizationMembers
                                       where m.Pending != true
                                       where m.MemberTypeId != Codes.MemberTypeCode.InActive
                                       where m.MemberType.AttendanceTypeId == Codes.AttendTypeCode.Leader
                                       select new Member
                                       {
                                           id = m.PeopleId,
                                           name = m.Person.Name,
                                           email = m.Person.EmailAddress,
                                           type = m.MemberType.Description
                                       }).ToList()
                        };
                return SerializeOrgs(q, "ParentOrgs", "ParentOrg", "Leaders");
            }
            catch (Exception ex)
            {
                return $@"<ParentOrgs status=""error"">{HttpUtility.HtmlEncode(ex.Message)}</ParentOrgs>";
            }

        }
        public string ChildOrgs(int id, string extravalue1, string extravalue2)
        {
            try
            {
                var q = from o in Db.Organizations
                        where o.ParentOrgId == id
                        select new Organization
                        {
                            id = o.OrganizationId,
                            name = o.OrganizationName,
                            location = o.Location,
                            description = o.Description,
                            extravalue1 = (from ev in o.OrganizationExtras
                                           where ev.Field == extravalue1
                                           select ev.Data).SingleOrDefault(),
                            extravalue2 = (from ev in o.OrganizationExtras
                                           where ev.Field == extravalue2
                                           select ev.Data).SingleOrDefault(),
                            members = (from m in o.OrganizationMembers
                                       where m.Pending != true
                                       where m.MemberTypeId != Codes.MemberTypeCode.InActive
                                       select new Member
                                       {
                                           id = m.PeopleId,
                                           name = m.Person.Name,
                                           email = m.Person.EmailAddress,
                                           type = m.MemberType.Description
                                       }).ToList()
                        };
                return SerializeOrgs(q, "ChildOrgs", "ChildOrg", "Members");
            }
            catch (Exception ex)
            {
                return $@"<ChildOrgs status=""error"">{HttpUtility.HtmlEncode(ex.Message)}</ChildOrgs>";
            }

        }
        public string ChildOrgMembers(int id)
        {
            try
            {
                var q = from o in Db.Organizations
                        where o.ParentOrgId == id
                        select new Organization
                        {
                            id = o.OrganizationId,
                            name = o.OrganizationName,
                            location = o.Location,
                            description = o.Description,
                            members = (from m in o.OrganizationMembers
                                       where m.Pending != true
                                       where m.MemberTypeId != Codes.MemberTypeCode.InActive
                                       select new Member
                                       {
                                           id = m.PeopleId,
                                           name = m.Person.Name,
                                           email = m.Person.EmailAddress,
                                           type = m.MemberType.Description
                                       }).ToList()
                        };
                return SerializeOrgs(q, "ChildOrgs", "ChildOrg", "Members");
            }
            catch (Exception ex)
            {
                return $@"<ChildOrgs status=""error"">{HttpUtility.HtmlEncode(ex.Message)}</ChildOrgs>";
            }

        }

        private static string SerializeOrgs(IQueryable<Organization> q, string root, string OrgElement, string MembersElement)
        {
            var sw = new StringWriter();
            var a = new Organizations { status = "ok", List = q.ToList() };

            var ao = new XmlAttributeOverrides();
            ao.Add(typeof(Organizations), new XmlAttributes
            {
                XmlRoot = new XmlRootAttribute(root)
            });
            ao.Add(typeof(Organizations), "List", new XmlAttributes
            {
                XmlElements = { new XmlElementAttribute(OrgElement) }
            });
            ao.Add(typeof(Organization), "members", new XmlAttributes
            {
                XmlArray = new XmlArrayAttribute(MembersElement)
            });

            var xs = new XmlSerializer(typeof(Organizations), ao);
            xs.Serialize(sw, a);
            return sw.ToString();
        }
        public static string MessageReplacements(CMSDataContext db, Person p, string divisionName, int orgId, string organizationName, string location, string message)
        {
            message = message.Replace("{first}", p.PreferredName, ignoreCase: true);
            message = message.Replace("{name}", p.Name, ignoreCase: true);
            message = message.Replace("{division}", divisionName, ignoreCase: true);
            message = message.Replace("{org}", organizationName, ignoreCase: true);
            message = message.Replace("{location}", location, ignoreCase: true);
            message = message.Replace("{cmshost}", db.CmsHost, ignoreCase: true);
            message = message.Replace("{orgbarcode}", $"{{orgbarcode:{orgId}}}");
            return message;
        }

        public class Committment
        {
            public DateTime MeetingDate { get; set; }
            public string Description { get; set; }
            public string Date => MeetingDate.ToLongDateString();
            public string Time => MeetingDate.ToLongTimeString();
        }
        public void SendVolunteerReminders(int id, bool sendall)
        {
            var org = Db.LoadOrganizationById(id);
            Db.SetCurrentOrgId(id);
            var setting = Db.CreateRegistrationSettings(id);
            setting.org = org;
            var currmembers = (from om in org.OrganizationMembers
                               where (om.Pending ?? false) == false
                               where om.MemberTypeId != CmsData.Codes.MemberTypeCode.InActive
                               where org.Attends.Any(a => (a.MeetingDate <= DateTime.Today.AddDays(7) || sendall)
                                   && a.MeetingDate >= DateTime.Today
                                   && AttendCommitmentCode.committed.Contains(a.Commitment ?? 0)
                                   && a.PeopleId == om.PeopleId)
                               select om).ToList();

            var notify = Db.StaffPeopleForOrg(org.OrganizationId);
            var from = Db.StaffEmailForOrg(org.OrganizationId);

            foreach (var om in currmembers)
            {
                var q = (from a in org.Attends
                         where a.PeopleId == om.PeopleId
                         where AttendCommitmentCode.committed.Contains(a.Commitment ?? 0)
                         where a.MeetingDate >= DateTime.Today
                         orderby a.MeetingDate
                         select new Committment()
                         {
                             MeetingDate = a.MeetingDate,
                             Description = setting.TimeSlots.FindDescription(a.MeetingDate)
                         }).ToList();
                if (!q.Any())
                    continue;
                var template = Handlebars.Compile(@"
<blockquote>
    <table>
        <tr>
            <td> Date </td>
            <td> Time </td>
            <td> Description </td>
        </tr>
{{#each this}}
            <tr>
                <td>{{Date}}</td>
                <td>{{Time}}</td>
                <td>{{Description}}</td>
            </tr>
{{/each}}
    </table>
</blockquote>");
                var details = template(q);

                var oname = org.OrganizationName;

                var subject = Util.PickFirst(setting.ReminderSubject, "no subject");
                var message = Util.PickFirst(setting.ReminderBody, "no body");

                string loc = org.Location;
                message = MessageReplacements(Db, om.Person, null, org.OrganizationId, oname, loc, message);

                message = message.Replace("{phone}", org.PhoneNumber.FmtFone7());
                message = message.Replace("{details}", details);

                Db.Email(from, om.Person, subject, message);
            }
            var sb = new StringBuilder(@"
<blockquote>
    <table>
        <tr>
            <td> Date </td>
            <td> Time </td>
        </tr>");
            foreach (var om in currmembers)
            {
                var q = (from a in org.Attends
                         where a.PeopleId == om.PeopleId
                         where AttendCommitmentCode.committed.Contains(a.Commitment ?? 0)
                         where a.MeetingDate >= DateTime.Today
                         where a.MeetingDate <= DateTime.Today.AddDays(7)
                         orderby a.MeetingDate
                         select a.MeetingDate).ToList();
                if (!q.Any())
                    continue;
                foreach (var d in q)
                    sb.AppendFormat("\n<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>", om.Person.Name, d.ToLongDateString(), d.ToLongTimeString());
            }
            sb.Append(@"
    </table>
</blockquote>
");
            foreach (var n in notify)
            {
                var organizationName = org.OrganizationName;

                var message = Util.PickFirst(setting.ReminderBody, "no body");

                string location = org.Location;
                message = MessageReplacements(Db, n, null, org.OrganizationId, organizationName, location, message);

                message = message.Replace("{phone}", org.PhoneNumber.FmtFone7());
                message = message.Replace("{details}", sb.ToString());

                Db.Email(from, n, "Reminder Notices sent for " + organizationName, message);
            }
        }

        public void SendEventReminders(Guid id)
        {
            var org = Db.LoadOrganizationById(Db.CurrentSessionOrgId);
            var setting = Db.CreateRegistrationSettings(org.OrganizationId);

            const string noSubject = "no subject";
            const string noBody = "no body";

            var subject = Util.PickFirst(setting.ReminderSubject, noSubject);
            var message = Util.PickFirst(setting.ReminderBody, noBody);
            if (subject == noSubject || message == noBody)
                throw new Exception("no subject or body");

            var q = from p in Db.PeopleQuery(id)
                join om in Db.OrganizationMembers on 
                    new {p.PeopleId, org.OrganizationId} 
                    equals new {om.PeopleId, om.OrganizationId} into j
                from om in j
                select om;

            var notify = Db.StaffPeopleForOrg(org.OrganizationId).FirstOrDefault();
            if (notify == null)
                throw new Exception("no notify person");

            var needdetails = message.Contains("{details}");
            foreach (var om in q)
            {
                var details = needdetails ? SummaryInfo.GetResults(Db, om.PeopleId, om.OrganizationId) : "";
                var organizationName = org.OrganizationName;

                subject = Util.PickFirst(setting.ReminderSubject, noSubject);
                message = Util.PickFirst(setting.ReminderBody, noBody);

                string location = org.Location;
                message = MessageReplacements(Db, om.Person, null, org.OrganizationId, organizationName, location, message);

                message = message.Replace("{phone}", org.PhoneNumber.FmtFone7());
                if(details.HasValue())
                    message = message.Replace("{details}", details);

                Db.Email(notify.FromEmail, om.Person, subject, message);
            }
        }
        public void SendEventReminders(int id)
        {
            var org = Db.LoadOrganizationById(id);
            Db.SetCurrentOrgId(id);
            var setting = Db.CreateRegistrationSettings(id);

            const string noSubject = "no subject";
            const string noBody = "no body";

            var subject = Util.PickFirst(setting.ReminderSubject, noSubject);
            var message = Util.PickFirst(setting.ReminderBody, noBody);
            if (subject == noSubject || message == noBody)
                throw new Exception("no subject or body");

            var currmembers = from om in org.OrganizationMembers
                              where (om.Pending ?? false) == false
                              where om.MemberTypeId != CmsData.Codes.MemberTypeCode.InActive
                              select om;
            var notify = Db.StaffPeopleForOrg(org.OrganizationId).FirstOrDefault();
            if (notify == null)
                throw new Exception("no notify person");

            foreach (var om in currmembers)
            {
                var details = SummaryInfo.GetResults(Db, om.PeopleId, om.OrganizationId);
                var organizationName = org.OrganizationName;

                subject = Util.PickFirst(setting.ReminderSubject, noSubject);
                message = Util.PickFirst(setting.ReminderBody, noBody);

                string location = org.Location;
                message = MessageReplacements(Db, om.Person, null, org.OrganizationId, organizationName, location, message);

                message = message.Replace("{phone}", org.PhoneNumber.FmtFone7());
                message = message.Replace("{details}", details);

                Db.Email(notify.FromEmail, om.Person, subject, message);
            }
        }

//        private string PrepareSummaryText2(OrganizationMember om)
//        {
//            var template = Handlebars.Compile(@"
//<table>
//    <tr><td>Org:</td><td>{{Orgname}}</td></tr>
//    <tr><td>First:</td><td>{{First}}</td></tr>
//    <tr><td>Last:</td><td>{{Last}}</td></tr>
//{{#each AskItems}}
//    {{#each Rows}}
//        <tr><td>{{Label}}</td><td>{{Description}}</td></tr>
//    {{/each}}
//{{/each}}
//</table>
//");
//            return template(new SummaryInfo(Db, om));
//        }
//
//        private class SummaryInfo
//        {
//            public string Orgname { get; set; }
//            public string First { get; set; }
//            public string Last { get; set; }
//            private readonly OrganizationMember om;
//            private readonly Settings setting;
//
//            public SummaryInfo(CMSDataContext db, OrganizationMember om)
//            {
//                this.om = om;
//                First = om.Person.PreferredName;
//                Last = om.Person.LastName;
//                Orgname = om.Organization.OrganizationName;
//                setting = db.CreateRegistrationSettings(om.OrganizationId);
//            }
//            public IEnumerable<AskItem> AskItems
//            {
//                get
//                {
//                    var types = new[] { "AskDropdown", "AskCheckboxes" };
//                    return from ask in setting.AskItems
//                           where types.Contains(ask.Type)
//                           select new AskItem(ask, om);
//                }
//            }
//
//            public class AskItem
//            {
//                private readonly Ask ask;
//                private readonly OrganizationMember om;
//
//                public AskItem(Ask ask, OrganizationMember om)
//                {
//                    this.ask = ask;
//                    this.om = om;
//                }
//
//                public class Row
//                {
//                    public string Label { get; set; }
//                    public string Description { get; set; }
//                }
//
//                public IEnumerable<Row> Rows
//                {
//                    get
//                    {
//                        if (ask.Type == "AskCheckboxes")
//                        {
//                            var label = ((AskCheckboxes)ask).Label;
//                            var option = ((AskCheckboxes)ask).list.Where(mm => om.OrgMemMemTags.Any(mt => mt.MemberTag.Name == mm.SmallGroup)).ToList();
//                            foreach (var m in option)
//                            {
//                                yield return new Row() { Label = label, Description = m.Description };
//                                label = string.Empty;
//                            }
//                        }
//                        else
//                        {
//                            var option = ((AskDropdown)ask).list.Where(mm => om.OrgMemMemTags.Any(mt => mt.MemberTag.Name == mm.SmallGroup)).ToList();
//                            if (option.Any())
//                                yield return new Row()
//                                {
//                                    Label = Util.PickFirst(((AskDropdown)ask).Label, "Options"),
//                                    Description = option.First().Description
//                                };
//                        }
//                    }
//                }
//            }
//        }
    }
}
