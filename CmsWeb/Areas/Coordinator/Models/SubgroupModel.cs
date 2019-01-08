using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsData.View;
using DocumentFormat.OpenXml.Spreadsheet;
using UtilityExtensions;

namespace CmsWeb.Areas.Coordinator.Models
{
    public class SubgroupModel
    {
        public int guestcount;
        public int workercount;
        private List<SelectListItem> mtypes;

        public SubgroupModel()
        {
        }

        public SubgroupModel(int id)
        {
            orgid = id;

            var org = DbUtil.Db.LoadOrganizationById(orgid);
            OrgName = org.OrganizationName;
            //isRecreationTeam = org.IsRecreationTeam;
        }

        public int orgid { get; set; }
        public int? groupid { get; set; }
        public string TimeSlot { get; set; }
        public string GroupName { get; set; }
        public string OrgName { get; set; }
        public int[] SelectedPeopleIds { get; set; }
        public string ingroup { get; set; }
        public string notgroup { get; set; }
        public bool notgroupactive { get; set; }
        public bool isguest { get; set; }
        public string sort { get; set; }
        public int tagfilter { get; set; }
        public bool isRecreationTeam { get; set; }
     //   public bool isAttendanceBySubgroups => DbUtil.Db.LoadOrganizationById(orgid).AttendanceBySubGroups.GetValueOrDefault();
        public int memtype { get; set; }
        public IList<int> List { get; set; } = new List<int>();

        public GroupDetails GetGroupDetails(int id)
        {
            var d = from e in DbUtil.Db.MemberTags
                where e.Id == id
                select new GroupDetails
                {
                    GroupId = e.Id,
                    Name = e.Name,
                    Capacity = e.CheckInCapacity,
                    CheckInOpen = e.CheckInOpen
                };

            return d.SingleOrDefault();
        }

        public List<GroupDetails> GetAllGroupDetails()
        {
            var d = (from e in DbUtil.Db.MemberTags
                where e.OrgId == orgid && e.CheckIn
                select new GroupDetails
                {
                    GroupId = e.Id,
                    Name = e.Name,
                    Capacity = e.CheckInCapacity,
                    CheckInOpen = e.CheckInOpen
                }).ToList();

            return d;
        }

        public IEnumerable<PersonInfo> GuestOrgMembers()
        {
            isguest = true;
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.OrganizationId == orgid
                    where om.MemberTypeId != MemberTypeCode.Prospect
                    where om.MemberTypeId != MemberTypeCode.InActive
                    where (om.Pending ?? false) == false
                    where tagfilter == 0 || DbUtil.Db.TagPeople.Any(tt => tt.PeopleId == om.PeopleId && tt.Id == tagfilter)
                    where om.MemberType.AttendType.Worker == false
                    select om;
            var q3 = FetchOrgMemberList(q);
            return q3;
        }

        public IEnumerable<PersonInfo> WorkerOrgMembers()
        {
            isguest = false;
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.OrganizationId == orgid
                    where om.MemberTypeId != MemberTypeCode.Prospect
                    where om.MemberTypeId != MemberTypeCode.InActive
                    where (om.Pending ?? false) == false
                    where tagfilter == 0 || DbUtil.Db.TagPeople.Any(tt => tt.PeopleId == om.PeopleId && tt.Id == tagfilter)
                    where om.MemberType.AttendType.Worker
                    select om;
            var q3 = FetchOrgMemberList(q);
            return q3;
        }

        public IEnumerable<PersonInfo> SelectedOrgMembers(int[] peopleIds)
        {
            var q = from om in DbUtil.Db.People
                where peopleIds.Contains(om.PeopleId)
                select new PersonInfo
                {
                    PeopleId = om.PeopleId,
                    Name = om.Name
                };

            return q;
        }

        public SelectList CheckInGroups()
        {
            var q = from g in DbUtil.Db.MemberTags
                where g.OrgId == orgid && g.CheckIn
                orderby g.Name
                select new
                {
                    value = g.Id,
                    text = g.Name
                };
            var list = q.ToList();
            list.Insert(0, new { value = 0, text = "(not specified)" });
            return new SelectList(list, "value", "text", groupid.ToString());
        }

        public SelectList SmallGroups()
        {
            var q = from m in DbUtil.Db.MemberTags
                where m.OrgId == orgid && m.CheckIn
                where m.OrgMemMemTags.Any()
                orderby m.Name
                select m.Name;
            var list = q.ToList();
            list.Insert(0, "Select Subgroup");
            return new SelectList(list);
        }

        public IEnumerable<PersonInfo> FetchOrgMemberList(IQueryable<OrganizationMember> q)
        {
            if (ingroup == null)
                ingroup = string.Empty;
            if (memtype != 0)
                q = q.Where(om => om.MemberTypeId == memtype);
            if (ingroup.HasValue())
            {
                var groups = ingroup.Split(',');
                for (var i = 0; i < groups.Length; i++)
                {
                    var group = groups[i];
                    q = q.Where(om => om.OrgMemMemTags.Any(omt => omt.MemberTag.Name.StartsWith(group)));
                }
            }
            if (notgroupactive)
                if (notgroup.HasValue())
                    q = q.Where(om => !om.OrgMemMemTags.Any(omt => omt.MemberTag.Name.StartsWith(notgroup)));
                else
                    q = q.Where(om => !om.OrgMemMemTags.Any());

            guestcount = q.Count();

            if (!sort.HasValue())
                sort = "Name";
            switch (sort)
            {
                case "Request":
                    q = from m in q
                        let ck = m.OrgMemMemTags.Any(g => g.MemberTagId == groupid.ToInt())
                        orderby !ck, m.Request == null ? 2 : 1, m.Request
                        select m;
                    break;
                case "Score":
                    q = from m in q
                        let ck = m.OrgMemMemTags.Any(g => g.MemberTagId == groupid.ToInt())
                        orderby !ck, m.Score descending
                        select m;
                    break;
                case "Name":
                    q = from m in q
                        let ck = m.OrgMemMemTags.Any(g => g.MemberTagId == groupid.ToInt())
                        orderby !ck, m.Person.Name2
                        select m;
                    break;
                case "MemberType":
                    q = from m in q
                        orderby m.MemberType.Description
                        select m;
                    break;
                case "Groups":
                    q = from m in q
                        let ck = m.OrgMemMemTags.Any(g => g.MemberTagId == groupid.ToInt())
                        let grp = (from g in m.OrgMemMemTags
                                   where g.MemberTag.Name.StartsWith(ingroup)
                                   orderby g.MemberTag.Name
                                   select g.MemberTag.Name).FirstOrDefault()
                        orderby !ck, grp, m.Person.Name2
                        select m;
                    break;
            }
            var q2 = from m in q
                     let p = m.Person
                     let ck = m.OrgMemMemTags.Any(g => g.MemberTagId == groupid.ToInt())
                     select new PersonInfo
                     {
                         PeopleId = m.PeopleId,
                         Name = p.Name,
                         LastName = p.LastName,
                         JoinDate = p.JoinDate,
                         BirthYear = p.BirthYear,
                         BirthMon = p.BirthMonth,
                         BirthDay = p.BirthDay,
                         Address = p.PrimaryAddress,
                         Address2 = p.PrimaryAddress2,
                         CityStateZip = p.CityStateZip5,
                         HomePhone = p.HomePhone.FmtFone(),
                         CellPhone = p.CellPhone.FmtFone(),
                         WorkPhone = p.WorkPhone.FmtFone(),
                         Email = p.EmailAddress,
                         MemberStatus = m.MemberType.Description,
                         Age = p.Age,
                         ischecked = ck,
                         Gender = p.Gender.Description,
                         Request = m.Request,
                         Score = m.Score,
                         Groups = from mt in m.OrgMemMemTags
                                  let ck2 = mt.MemberTag.Name.StartsWith(ingroup)
                                  orderby ck2 descending, mt.MemberTag.Name
                                  select new GroupInfo
                                  {
                                      Name = mt.MemberTag.Name,
                                      Count = mt.MemberTag.OrgMemMemTags.Count(),
                                      IsLeader = mt.IsLeader.GetValueOrDefault() ? "- Leader" : ""
                                  }
                     };
            return q2;
        }

        public IQueryable<OrganizationMember> OrgMembers()
        {
            var q = from om in DbUtil.Db.OrganizationMembers
                where om.OrganizationId == orgid
                where om.MemberTypeId != MemberTypeCode.Prospect
                where om.MemberTypeId != MemberTypeCode.InActive
                where (om.Pending ?? false) == false
                where tagfilter == 0 || DbUtil.Db.TagPeople.Any(tt => tt.PeopleId == om.PeopleId && tt.Id == tagfilter)
                //where om.OrgMemMemTags.Any(g => g.MemberTagId == sg) || (sg ?? 0) == 0
                select om;
            return q;
        }

        public class PersonInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
            public int AttendType { get; set; }
            public DateTime? JoinDate { get; set; }
            public string Email { get; set; }
            public string BirthDate => Person.FormatBirthday(BirthYear, BirthMon, BirthDay, PeopleId);
            public int? BirthYear { get; set; }
            public int? BirthMon { get; set; }
            public int? BirthDay { get; set; }
            public string Address { get; set; }
            public string Address2 { get; set; }
            public string CityStateZip { get; set; }
            public string HomePhone { get; set; }
            public string CellPhone { get; set; }
            public string WorkPhone { get; set; }
            public int? Age { get; set; }
            public string MemberStatus { get; set; }
            public string Gender { get; set; }
            public string Request { get; set; }
            public int Score { get; set; }
            public IEnumerable<GroupInfo> Groups { get; set; }

            public HtmlString GroupsDisplay
            {
                get
                {
                    var s = string.Join(",~", Groups.Select(g => $"{g.Name}({g.Count}) {g.IsLeader }").ToArray());
                    s = s.Replace(" ", "&nbsp;").Replace(",~", "<br />\n");
                    return new HtmlString(s);
                }
            }

            public bool ischecked { get; set; }
            public string ToolTip => $"{Name} ({PeopleId})|Cell Phone: {CellPhone}|Work Phone: {WorkPhone}|Home Phone: {HomePhone}|BirthDate: {BirthDate:d}|Join Date: {JoinDate:d}|Status: {MemberStatus}|Email: {Email}";

            public HtmlString IsInGroup()
            {
                var s = ischecked ? "class='info'" : "";
                return new HtmlString(s);
            }
        }

        public class GroupDetails
        {
            public int GroupId { get; set; }
            public string Name { get; set; }
            public int Capacity { get; set; }
            public bool CheckInOpen { get; set; }
        }

        public class GroupInfo
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public string IsLeader { get; internal set; }
        }

    }
}
