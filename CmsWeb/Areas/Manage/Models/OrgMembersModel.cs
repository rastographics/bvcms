using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using MoreLinq;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class OrgMembersModel
    {
        private IQueryable<OrganizationMember> _members;

        public OrgMembersModel()
        {
            MembersOnly = true;
            MoveRegistrationData = true;
        }

        public bool MembersOnly { get; set; }
        public bool SmallGroupsToo { get; set; }
        public int TargetId { get; set; }
        public int SourceId { get; set; }
        public int ProgId { get; set; }
        public int DivId { get; set; }
        public bool EmailAllNotices { get; set; }
        public bool MoveRegistrationData { get; set; }
        public string Grades { get; set; }
        public string SmallGroup { get; set; }
        public string Sort { get; set; }
        public string Dir { get; set; }
        public IList<string> List { get; set; } = new List<string>();

        public void FetchSavedIds()
        {
            var pref = DbUtil.Db.UserPreference("OrgMembersModelIds", "0.0.0");
            var a = pref.Split('.').Select(s => s.ToInt()).ToArray();
            var prog = DbUtil.Db.Programs.SingleOrDefault(p => p.Id == a[0]);
            if (prog != null)
                ProgId = a[0];

            var div = DbUtil.Db.Divisions.SingleOrDefault(d => d.Id == a[1] && d.ProgId == ProgId);
            if (div != null)
                DivId = a[1];

            var source = DbUtil.Db.Organizations.Where(o => o.OrganizationId == a[2]).Select(o => o.OrganizationId).SingleOrDefault();
            SourceId = a[2];
        }

        public void ValidateIds()
        {
            var q = from prog in DbUtil.Db.Programs
                    where prog.Id == ProgId
                    let div = DbUtil.Db.Divisions.SingleOrDefault(d => d.Id == DivId && d.ProgId == ProgId)
                    let org = DbUtil.Db.Organizations.SingleOrDefault(o => o.OrganizationId == SourceId && o.DivOrgs.Any(d => d.DivId == DivId))
                    let org2 = DbUtil.Db.Organizations.SingleOrDefault(o => o.OrganizationId == SourceId && o.DivOrgs.Any(d => d.DivId == DivId))
                    select new {div, noorg = org == null, noorg2 = org2 == null};
            var i = q.SingleOrDefault();
            if (i == null)
                ProgId = DivId = SourceId = TargetId = 0;
            else
            {
                if (i.div == null)
                    DivId = SourceId = TargetId = 0;
                else
                {
                    if (i.noorg)
                        SourceId = 0;
                    if (i.noorg2)
                        TargetId = 0;
                }
            }
        }

        public IEnumerable<SelectListItem> Programs()
        {
            var q = from c in DbUtil.Db.Programs
                    orderby c.Name
                    select new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "(not specified)"
            });
            return list;
        }

        public IEnumerable<SelectListItem> Divisions()
        {
            var q = from d in DbUtil.Db.Divisions
                    where d.ProgId == ProgId
                    orderby d.Name
                    select new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "(not specified)"
            });
            return list;
        }

        public IEnumerable<SelectListItem> Organizations()
        {
            var roles = DbUtil.Db.CurrentRoles();
            var q = from o in DbUtil.Db.Organizations
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    where o.DivOrgs.Any(di => di.DivId == DivId)
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    orderby o.OrganizationName
                    let sctime = o.OrgSchedules.Count() == 1 ? " " + DbUtil.Db.GetScheduleDesc(o.OrgSchedules.First().MeetingTime) : ""
                    select new SelectListItem
                    {
                        Value = o.OrganizationId.ToString(),
                        Text = o.OrganizationName + sctime
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem {Value = "0", Text = "(not specified)"});
            return list;
        }

        public IEnumerable<SelectListItem> Organizations2()
        {
            var member = MemberTypeCode.Member;
            var roles = DbUtil.Db.CurrentRoles();
            var q = from o in DbUtil.Db.Organizations
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    where o.DivOrgs.Any(di => di.DivId == DivId)
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    orderby o.OrganizationName
                    let sctime = o.OrgSchedules.Count() == 1 ? " " + DbUtil.Db.GetScheduleDesc(o.OrgSchedules.First().MeetingTime) : ""
                    let cmales = o.OrganizationMembers.Count(m => m.Person.GenderId == 1 && m.MemberTypeId == member)
                    let cfemales = o.OrganizationMembers.Count(m => m.Person.GenderId == 2 && m.MemberTypeId == member)
                    select new SelectListItem
                    {
                        Value = o.OrganizationId.ToString(),
                        Text = o.OrganizationName + sctime + " (" + cmales + "+" + cfemales + "=" + (cmales + cfemales) + ")"
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem {Value = "0", Text = "(not specified)"});
            return list;
        }

        private IQueryable<OrganizationMember> GetMembers()
        {
            if (_members == null)
            {
                var glist = new int[] {};
                if (Grades.HasValue())
                    glist = (from g in (Grades ?? "").Split(',')
                             select g.ToInt()).ToArray();
                var q = from om in DbUtil.Db.OrganizationMembers
                        where om.Organization.DivOrgs.Any(di => di.DivId == DivId)
                        where SourceId == 0 || om.OrganizationId == SourceId
                        where glist.Length == 0 || glist.Contains(om.Person.Grade.Value)
                        where !SmallGroup.HasValue() || om.OrgMemMemTags.Any(mm => mm.MemberTag.Name == SmallGroup)
                        where !MembersOnly || om.MemberTypeId == MemberTypeCode.Member
                        select om;
                _members = q;
            }
            return _members;
        }

        public IEnumerable<MemberInfo> Members()
        {
            var q = ApplySort();
            var q2 = from om in q
                     select new MemberInfo
                     {
                         PeopleId = om.PeopleId,
                         Age = om.Person.Age,
                         DOB = Util.FormatBirthday(om.Person.BirthYear, om.Person.BirthMonth, om.Person.BirthDay),
                         Gender = om.Person.Gender.Code,
                         Grade = om.Person.Grade,
                         OrgId = om.OrganizationId,
                         Request = om.Request,
                         Name = om.Person.Name,
                         isChecked = om.OrganizationId == TargetId,
                         MemberStatus = om.MemberType.Description,
                         OrgName = om.Organization.OrganizationName
                     };
            return q2;
        }

        public int Count()
        {
            return GetMembers().Count();
        }

        public IEnumerable<OrganizationMember> ApplySort()
        {
            var q = GetMembers();

            if (Dir == "asc")
                switch (Sort)
                {
                    default:
                    case "Name":
                        q = from om in q
                            orderby om.Person.Name2
                            select om;
                        break;
                    case "Date of Birth":
                        q = from om in q
                            orderby om.Person.BirthYear, om.Person.BirthMonth, om.Person.BirthDay
                            select om;
                        break;
                    case "Organization":
                        q = from om in q
                            orderby om.Organization.OrganizationName, om.Person.Name2
                            select om;
                        break;
                    case "Grade":
                        q = from om in q
                            orderby om.Person.Grade, om.Organization.OrganizationName, om.Person.Name2
                            select om;
                        break;
                    case "Gender":
                        q = from om in q
                            orderby om.Person.Gender.Code, om.Organization.OrganizationName, om.Person.Name2
                            select om;
                        break;
                    case "Mixed":
                        q = from om in q
                            orderby om.Person.HashNum
                            select om;
                        break;
                }
            else
                switch (Sort)
                {
                    default:
                    case "Name":
                        q = from om in q
                            orderby om.Person.Name2 descending
                            select om;
                        break;
                    case "Date of Birth":
                        q = from om in q
                            orderby om.Person.BirthYear descending, om.Person.BirthMonth descending, om.Person.BirthDay descending
                            select om;
                        break;
                    case "Organization":
                        q = from om in q
                            orderby om.Organization.OrganizationName descending, om.Person.Name2
                            select om;
                        break;
                    case "Grade":
                        q = from om in q
                            orderby om.Person.Grade descending, om.Organization.OrganizationName, om.Person.Name2
                            select om;
                        break;
                    case "Gender":
                        q = from om in q
                            orderby om.Person.Gender.Code descending, om.Organization.OrganizationName, om.Person.Name2
                            select om;
                        break;
                    case "Mixed":
                        q = from om in q
                            orderby om.Person.HashNum
                            select om;
                        break;
                }
            return q;
        }

        public void Move()
        {
            foreach (var i in List)
            {
                if (!i.HasValue())
                    continue;
                var a = i.Split('.');
                if (a.Length != 2)
                    continue;
                var pid = a[0].ToInt();
                var oid = a[1].ToInt();
                if (oid == TargetId)
                    continue;
                OrganizationMember.MoveToOrg(DbUtil.Db, pid, oid, TargetId, MoveRegistrationData);
            }
            DbUtil.Db.UpdateMainFellowship(TargetId);
        }

        public int MovedCount()
        {
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.Organization.DivOrgs.Any(di => di.DivId == DivId)
                    where om.Moved == true
                    select om;
            return q.Count();
        }

        public void ResetMoved()
        {
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.Organization.DivOrgs.Any(di => di.DivId == DivId)
                    where om.Moved == true
                    select om;
            DbUtil.Db.ExecuteCommand(@"
UPDATE dbo.OrganizationMembers
SET Moved = NULL
WHERE EXISTS(SELECT NULL FROM dbo.DivOrg WHERE OrgId = OrganizationId AND DivId = {0})", DivId);
        }

        public int AllCount()
        {
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.Organization.DivOrgs.Any(di => di.DivId == DivId)
                    where om.Moved == true || EmailAllNotices
                    select om;
            return q.Count();
        }

        public void SendMovedNotices()
        {
            var Db = DbUtil.Db;

            var q = from om in Db.OrganizationMembers
                    where om.Organization.DivOrgs.Any(di => di.DivId == DivId)
                    where om.Moved == true || EmailAllNotices
                    select new
                    {
                        om,
                        om.Person,
                        om.Person.FromEmail,
                        om.Person.EmailAddress,
                        om.RegisterEmail,
                        om.Person.Name,
                        om.PeopleId,
                        om.Organization.OrganizationName,
                        om.Organization.Location,
                        om.Organization.LeaderName,
                        om.Organization.PhoneNumber
                    };
            var content = DbUtil.Db.ContentOfTypeHtml("OrgMembersModel_SendMovedNotices");
            if (content == null)
            {
                content = new Content()
                {
                    Name = "OrgMembersModel_SendMovedNotices",
                    Body = Resource1.OrgMembersModel_SendMovedNotices,
                    Title = "Room Assignment for {name} in {org}"
                };
                DbUtil.Db.Contents.InsertOnSubmit(content);
                DbUtil.Db.SubmitChanges();
            }
            if (content.Title == "SendMovedNotices") // replace old Title with new, improved version
                content.Title = "Room Assignment for {name} in {org}"; // this will be the subject

            var sb = new StringBuilder("Org Assignment Notices sent to:\r\n<pre>\r\n");
            foreach (var i in q)
            {
                var msg = content.Body.Replace("{name}", i.Name)
                                 .Replace("{org}", i.OrganizationName)
                                 .Replace("{room}", i.Location)
                                 .Replace("{leader}", i.LeaderName)
                                 .Replace("{phone}", DbUtil.Db.Setting("ChurchPhone", "ChurchPhone"))
                                 .Replace("{church}", DbUtil.Db.Setting("NameOfChurch", "NameOfChurch"));

                var subj = content.Title // the title of the content is the subject
                                  .Replace("{name}", i.Name)
                                  .Replace("{org}", i.OrganizationName)
                                  .Replace("{room}", i.Location);

                if (i.om.Moved == true || EmailAllNotices)
                {
                    if (i.RegisterEmail.HasValue())
                    {
                        Db.Email(Db.CurrentUser.Person.FromEmail,
                            i.om.Person, Util.ToMailAddressList(i.RegisterEmail),
                            subj, msg, false);
                        sb.Append($"\"{i.Name}\" [{i.FromEmail}]R ({i.PeopleId}): {i.Location}\r\n");
                        i.om.Moved = false;
                    }

                    var flist = (from fm in i.om.Person.Family.People
                                 where (fm.EmailAddress ?? "") != ""
                                 where fm.EmailAddress != i.RegisterEmail
                                 where fm.PositionInFamilyId == PositionInFamily.PrimaryAdult
                                 select fm).ToList();
                    Db.Email(Db.CurrentUser.Person.FromEmail, flist, subj, msg);
                    foreach (var m in flist)
                    {
                        sb.Append($"{m}P ({i.PeopleId}): {i.Location}\r\n");
                        i.om.Moved = false;
                    }
                }
            }
            sb.Append("</pre>\n");

            var q0 = from o in Db.Organizations
                     where o.DivOrgs.Any(di => di.DivId == DivId)
                     where o.NotifyIds.Length > 0
                     where o.RegistrationTypeId > 0
                     select o;
            var onlineorg = q0.FirstOrDefault();

            if (onlineorg == null)
                Db.Email(Db.CurrentUser.Person.FromEmail,
                    Db.CurrentUserPerson,
                    "Org Assignment notices sent to:", sb.ToString());
            else
                Db.Email(Db.CurrentUser.Person.FromEmail,
                    Db.PeopleFromPidString(onlineorg.NotifyIds),
                    "Org Assignment notices sent to:", sb.ToString());
            Db.SubmitChanges();
        }

        public EpplusResult ToExcel(int oid)
        {
            var d = from om in DbUtil.Db.OrganizationMembers
                    where om.OrganizationId == oid
                    select new
                    {
                        om.PeopleId,
                        om.OrganizationId,
                        Groups = string.Join(",", om.OrgMemMemTags.Select(mt => mt.MemberTag.Name).ToArray())
                    };
            return d.ToDataTable().ToExcel("OrgMembers.xlsx");
        }

        public class MemberInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public int OrgId { get; set; }
            public string OrgName { get; set; }
            public string MemberStatus { get; set; }
            public int? Grade { get; set; }
            public int? Age { get; set; }
            public string DOB { get; set; }
            public string Gender { get; set; }
            public string Request { get; set; }
            public bool isChecked { get; set; }

            public string Checked
            {
                get { return isChecked ? "checked='checked'" : ""; }
            }
        }
    }
}