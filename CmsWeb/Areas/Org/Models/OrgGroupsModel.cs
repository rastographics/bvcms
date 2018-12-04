using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgGroupsModel
    {
        public int count;
        private List<SelectListItem> mtypes;

        public OrgGroupsModel()
        {
        }

        public OrgGroupsModel(int id)
        {
            orgid = id;

            var org = DbUtil.Db.LoadOrganizationById(orgid);
            isRecreationTeam = org.IsRecreationTeam;
        }

        public int orgid { get; set; }
        public int? groupid { get; set; }
        public int? ScheduleId { get; set; }
        public bool CheckInOpenDefault { get; set; }
        public int CheckInCapacityDefault { get; set; }
        public string GroupName { get; set; }
        public string ingroup { get; set; }
        public string notgroup { get; set; }
        public bool notgroupactive { get; set; }
        public string sort { get; set; }
        public int tagfilter { get; set; }
        public bool isRecreationTeam { get; set; }
        public bool isAttendanceBySubgroups => DbUtil.Db.LoadOrganizationById(orgid).AttendanceBySubGroups.GetValueOrDefault();
        public string OrgName => DbUtil.Db.LoadOrganizationById(orgid).OrganizationName;
        public int memtype { get; set; }
        public IList<int> List { get; set; } = new List<int>();
        public string AllowCheckin { get; set; }

        public GroupDetails GetGroupDetails(int id)
        {
            var d = from e in DbUtil.Db.OrgMemMemTags
                    from om in DbUtil.Db.OrganizationMembers.DefaultIfEmpty()
                    where e.MemberTagId == id
                    where om.PeopleId == e.PeopleId
                    where om.OrganizationId == e.OrgId
                    group new { e, om } by e.MemberTagId
                    into grp
                    select new GroupDetails
                    {
                        members = grp.Count(m => m.e.MemberTagId > 0),
                        total = grp.Sum(t => t.om.Score),
                        average = grp.Average(a => a.om.Score)
                    };

            return d.SingleOrDefault();
        }

        private IEnumerable<OrgSchedule> _orgSchedules;
        public IEnumerable<OrgSchedule> GetOrgSchedules()
        {
            return _orgSchedules ?? (_orgSchedules =
                (from schedule in DbUtil.Db.OrgSchedules
                   where schedule.OrganizationId == orgid
                   orderby schedule.SchedDay, schedule.SchedTime
                   select schedule).ToList());
        }

        public IEnumerable<MemberTag> GroupsList()
        {
            return from g in DbUtil.Db.MemberTags
                   where g.OrgId == orgid
                   orderby g.Name
                   select g;
        }

        public SelectList Groups()
        {
            var q = from g in DbUtil.Db.MemberTags
                    where g.OrgId == orgid
                    orderby g.Name
                    select new GroupListItem
                    {
                        value = g.Id,
                        name = g.Name,
                        schedule = g.Schedule
                    };
            var list = q.ToList();
            list.Insert(0, new GroupListItem {value = 0, name = "(not specified)"});
            return new SelectList(list, "value", "text", groupid.ToString());
        }

        private List<SelectListItem> MemberTypes()
        {
            if (mtypes == null)
            {
                var q = from om in DbUtil.Db.OrganizationMembers
                        where om.OrganizationId == orgid
                        where (om.Pending ?? false) == false
                        where om.MemberTypeId != MemberTypeCode.InActive
                        where om.MemberTypeId != MemberTypeCode.Prospect
                        group om by om.MemberType
                        into g
                        orderby g.Key.Description
                        select new SelectListItem
                        {
                            Value = g.Key.Id.ToString(),
                            Text = g.Key.Description
                        };
                mtypes = q.ToList();
            }
            return mtypes;
        }

        public IEnumerable<SelectListItem> MemberTypeCodesWithNotSpecified()
        {
            var mt = MemberTypes().ToList();
            mt.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return mt;
        }

        public IEnumerable<PersonInfo> FetchOrgMemberList()
        {
            if (ingroup == null)
            {
                ingroup = string.Empty;
            }

            var q = OrgMembers();
            if (memtype != 0)
            {
                q = q.Where(om => om.MemberTypeId == memtype);
            }

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
            {
                if (notgroup.HasValue())
                {
                    q = q.Where(om => !om.OrgMemMemTags.Any(omt => omt.MemberTag.Name.StartsWith(notgroup)));
                }
                else
                {
                    q = q.Where(om => !om.OrgMemMemTags.Any());
                }
            }

            count = q.Count();
            if (!sort.HasValue())
            {
                sort = "Name";
            }

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

        public IEnumerable<SelectListItem> Tags()
        {
            var cv = new CodeValueModel();
            var tg = CodeValueModel.ConvertToSelect(cv.UserTags(Util.UserPeopleId), "Id").ToList();
            tg.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return tg;
        }

        public void createTeamGroups()
        {
            var c = from e in DbUtil.Db.OrganizationMembers
                    where e.Score == 0
                    where e.OrganizationId == orgid
                    select e;

            foreach (var coach in c)
            {
                var name = "TM: " + coach.Person.Name;

                var group = DbUtil.Db.MemberTags.SingleOrDefault(g => g.Name == name && g.OrgId == orgid);
                if (group != null)
                {
                    continue;
                }

                group = new MemberTag
                {
                    Name = name,
                    OrgId = orgid
                };

                DbUtil.Db.MemberTags.InsertOnSubmit(group);
            }

            DbUtil.Db.SubmitChanges();

            // Refresh the list
            var teamList = (from e in DbUtil.Db.MemberTags
                            where e.OrgId == orgid
                            where e.Name.StartsWith("TM:")
                            select e).ToList();


            var p = (from e in DbUtil.Db.OrganizationMembers
                     where e.Score != 0
                     where e.OrganizationId == orgid
                     select e).ToList();

            var teams = teamList.Count();
            var players = p.Count();
            var perTeam = Math.Floor((double)players / teams);
            var passes = Math.Floor(perTeam / 2);

            for (var iX = 0; iX < passes; iX++)
            {
                foreach (var team in teamList)
                {
                    var tagTop = new OrgMemMemTag();
                    var tagBot = new OrgMemMemTag();

                    var top = p.OrderByDescending(t => t.Score).ThenByDescending(t => t.PeopleId).Take(1).SingleOrDefault();
                    var bot = p.OrderBy(t => t.Score).ThenBy(t => t.PeopleId).Take(1).SingleOrDefault();

                    tagTop.MemberTagId = team.Id;
                    tagTop.OrgId = orgid;
                    tagTop.PeopleId = top.PeopleId;

                    tagBot.MemberTagId = team.Id;
                    tagBot.OrgId = orgid;
                    tagBot.PeopleId = bot.PeopleId;

                    DbUtil.Db.OrgMemMemTags.InsertOnSubmit(tagTop);
                    DbUtil.Db.OrgMemMemTags.InsertOnSubmit(tagBot);

                    p.Remove(top);
                    p.Remove(bot);
                }
            }

            if (p.Any())
            {
                foreach (var team in teamList)
                {
                    var tagBot = new OrgMemMemTag();

                    var bot = p.OrderBy(t => t.Score).ThenBy(t => t.PeopleId).Take(1).SingleOrDefault();
                    if (bot == null)
                    {
                        break;
                    }

                    tagBot.MemberTagId = team.Id;
                    tagBot.OrgId = orgid;
                    tagBot.PeopleId = bot.PeopleId;

                    DbUtil.Db.OrgMemMemTags.InsertOnSubmit(tagBot);

                    p.Remove(bot);
                }
            }

            if (p.Any())
            {
                foreach (var team in teamList)
                {
                    var tagBot = new OrgMemMemTag();

                    var bot = p.OrderBy(t => t.Score).ThenBy(t => t.PeopleId).Take(1).SingleOrDefault();
                    if (bot == null)
                    {
                        break;
                    }

                    tagBot.MemberTagId = team.Id;
                    tagBot.OrgId = orgid;
                    tagBot.PeopleId = bot.PeopleId;

                    DbUtil.Db.OrgMemMemTags.InsertOnSubmit(tagBot);

                    p.Remove(bot);
                }
            }

            DbUtil.Db.SubmitChanges();
        }

        public class GroupDetails
        {
            public int members { get; set; }
            public int total { get; set; }
            public double average { get; set; }
        }

        public class GroupInfo
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public string IsLeader { get; internal set; }
        }

        public class PersonInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
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
    }
}
