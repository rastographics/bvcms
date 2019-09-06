using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Models;
using UtilityExtensions;
using LumenWorks.Framework.IO.Csv;

namespace CmsWeb.Areas.Org.Models
{
    public class OrgGroupsModel : IDbBinder
    {
        public CMSDataContext CurrentDatabase { get; set; }
        internal CMSDataContext Db => CurrentDatabase;
        public int count;
        private List<SelectListItem> mtypes;

        public OrgGroupsModel()
        {
        }

        public OrgGroupsModel(CMSDataContext db, int id)
        {
            CurrentDatabase = db;
            orgid = id;
            var org = Db.LoadOrganizationById(orgid);
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
        public bool isAttendanceBySubgroups => Db.LoadOrganizationById(orgid).AttendanceBySubGroups.GetValueOrDefault();
        public string OrgName => Db.LoadOrganizationById(orgid).OrganizationName;
        public int memtype { get; set; }
        public IList<int> List { get; set; } = new List<int>();
        public string AllowCheckin { get; set; }

        public GroupDetails GetGroupDetails(int id)
        {
            var d = from e in Db.OrgMemMemTags
                    from om in Db.OrganizationMembers.DefaultIfEmpty()
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
                (from schedule in Db.OrgSchedules
                   where schedule.OrganizationId == orgid
                   orderby schedule.SchedDay, schedule.SchedTime
                   select schedule).ToList());
        }

        public IEnumerable<MemberTag> GroupsList()
        {
            return from g in Db.MemberTags
                   where g.OrgId == orgid
                   orderby g.Name
                   select g;
        }

        public SelectList Groups()
        {
            var q = from g in Db.MemberTags
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
                var q = from om in Db.OrganizationMembers
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
            var q = from om in Db.OrganizationMembers
                    where om.OrganizationId == orgid
                    where om.MemberTypeId != MemberTypeCode.Prospect
                    where om.MemberTypeId != MemberTypeCode.InActive
                    where (om.Pending ?? false) == false
                    where tagfilter == 0 || Db.TagPeople.Any(tt => tt.PeopleId == om.PeopleId && tt.Id == tagfilter)
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
            var c = from e in Db.OrganizationMembers
                    where e.Score == 0
                    where e.OrganizationId == orgid
                    select e;

            foreach (var coach in c)
            {
                var name = "TM: " + coach.Person.Name;

                var group = Db.MemberTags.SingleOrDefault(g => g.Name == name && g.OrgId == orgid);
                if (group != null)
                {
                    continue;
                }

                group = new MemberTag
                {
                    Name = name,
                    OrgId = orgid
                };

                Db.MemberTags.InsertOnSubmit(group);
            }

            Db.SubmitChanges();

            // Refresh the list
            var teamList = (from e in Db.MemberTags
                            where e.OrgId == orgid
                            where e.Name.StartsWith("TM:")
                            select e).ToList();


            var p = (from e in Db.OrganizationMembers
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

                    Db.OrgMemMemTags.InsertOnSubmit(tagTop);
                    Db.OrgMemMemTags.InsertOnSubmit(tagBot);

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

                    Db.OrgMemMemTags.InsertOnSubmit(tagBot);

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

                    Db.OrgMemMemTags.InsertOnSubmit(tagBot);

                    p.Remove(bot);
                }
            }

            Db.SubmitChanges();
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

        public MemberTag MakeNewGroup()
        {
            var group = new MemberTag
            {
                Name = GroupName,
                OrgId = orgid,
                CheckIn = AllowCheckin.ToBool(),
                ScheduleId = ScheduleId,
                CheckInOpenDefault = CheckInOpenDefault,
                CheckInCapacityDefault = CheckInCapacityDefault,
            };
            Db.MemberTags.InsertOnSubmit(group);
            Db.SubmitChanges();
            groupid = group.Id;
            return group;
        }
        public void EditGroup()
        {
            if (!GroupName.HasValue() || groupid == 0)
            {
                throw new ArgumentException("error: no group name");
            }
            var group = Db.MemberTags.SingleOrDefault(d => d.Id == groupid);
            if (group != null)
            {
                group.Name = GroupName;
                group.CheckIn = AllowCheckin == "true";
                group.CheckInCapacityDefault = CheckInCapacityDefault;
                group.CheckInOpenDefault = CheckInOpenDefault;
                group.ScheduleId = ScheduleId;
            }
            Db.SubmitChanges();
        }

        public void RenameGroup()
        {
            if (!GroupName.HasValue() || groupid == 0)
            {
                throw new ArgumentException("error: no group name");
            }
            var group = Db.MemberTags.SingleOrDefault(d => d.Id == groupid);
            if (group != null)
            {
                group.Name = GroupName;
            }
            Db.SubmitChanges();
            GroupName = null;
        }
        public void AssignSelectedToTargetGroup()
        {
            var people = List.ToArray();
            var memberTag = Db.MemberTags.Single(t => t.Id == groupid && t.OrgId == orgid);
            var orgmembersToAdd = from om in OrgMembers()
                                  where om.OrgMemMemTags.All(m => m.MemberTag.Id != groupid)
                                  where people.Contains(om.PeopleId)
                                  select om;
            foreach (var orgmember in orgmembersToAdd)
            {
                memberTag.OrgMemMemTags.Add(new OrgMemMemTag
                {
                    PeopleId = orgmember.PeopleId,
                    OrgId = orgmember.OrganizationId
                });
            }
            Db.SubmitChanges();
        }
        public void MakeLeaderOfTargetGroup()
        {
            var a = List.ToArray();
            var q2 = from om in OrgMembers()
                     where a.Contains(om.PeopleId)
                     select om;
            if (groupid != null)
            {
                foreach (var om in q2)
                {
                    om.MakeLeaderOfGroup(Db, groupid.GetValueOrDefault());
                }
            }
            Db.SubmitChanges();
        }

        public void RemoveAsLeaderOfTargetGroup()
        {
            var a = List.ToArray();
            var q2 = from om in OrgMembers()
                     where a.Contains(om.PeopleId)
                     select om;
            if (groupid != null)
            {
                foreach (var om in q2)
                {
                    om.RemoveAsLeaderOfGroup(Db, groupid.GetValueOrDefault());
                }
            }
            Db.SubmitChanges();
        }
        public void RemoveSelectedFromTargetGroup()
        {
            var a = List.ToArray();
            var sgname = Db.MemberTags.Single(mt => mt.Id == groupid).Name;
            var q1 = from omt in Db.OrgMemMemTags
                     where omt.OrgId == orgid
                     where omt.MemberTag.Name == sgname
                     where a.Contains(omt.PeopleId)
                     select omt;
            Db.OrgMemMemTags.DeleteAllOnSubmit(q1);
            Db.SubmitChanges();
        }
        public void DeleteGroup()
        {
            var group = Db.MemberTags.SingleOrDefault(g => g.Id == groupid);
            if (group != null)
            {
                Db.OrgMemMemTags.DeleteAllOnSubmit(group.OrgMemMemTags);
                Db.MemberTags.DeleteOnSubmit(group);
                Db.SubmitChanges();
                groupid = (from v in Groups()
                             where v.Value != "0"
                             select v.Value).FirstOrDefault().ToInt();
            }
        }
        public static void DeleteGroups(CMSDataContext Db, int[] groups)
        {
            var groupList = Db.MemberTags.Where(t => groups.Contains(t.Id));
            foreach (var group in groupList)
            {
                Db.OrgMemMemTags.DeleteAllOnSubmit(group.OrgMemMemTags);
                Db.MemberTags.DeleteOnSubmit(group);
            }
            Db.SubmitChanges();
        }
        public static void UploadScores(CMSDataContext Db, string data, int orgID)
        {
            var csv = new CsvReader(new StringReader(data), false, '\t');
            var list = csv.ToList();

            foreach (var score in list)
            {
                var peopleID = score[0].ToInt();

                var player = (from e in Db.OrganizationMembers
                              where e.OrganizationId == orgID
                              where e.PeopleId == peopleID
                              select e).SingleOrDefault();

                if (player != null)
                {
                    player.Score = score[1].ToInt();
                }
                Db.SubmitChanges();
            }
        }
        public static void SwapPlayers(CMSDataContext Db, string pOne, string pTwo)
        {
            string[] splitOne = pOne.Split('-');
            int orgIDOne = splitOne[0].ToInt();
            int peopleIDOne = splitOne[1].ToInt();

            string[] splitTwo = pTwo.Split('-');
            int orgIDTwo = splitTwo[0].ToInt();
            int peopleIDTwo = splitTwo[1].ToInt();

            var playerOne = (from e in Db.OrganizationMembers
                             where e.OrganizationId == orgIDOne
                             where e.PeopleId == peopleIDOne
                             select e).SingleOrDefault();

            var playerTwo = (from e in Db.OrganizationMembers
                             where e.OrganizationId == orgIDTwo
                             where e.PeopleId == peopleIDTwo
                             select e).SingleOrDefault();


            if (playerOne != null)
            {
                var pOneTag = playerOne.OrgMemMemTags.FirstOrDefault(t1 => t1.MemberTag.Name.StartsWith("TM:"));
                if (playerTwo != null)
                {
                    var pTwoTag = playerTwo.OrgMemMemTags.FirstOrDefault(t2 => t2.MemberTag.Name.StartsWith("TM:"));

                    if (pTwoTag != null)
                    {
                        var pOneNew = new OrgMemMemTag
                        {
                            PeopleId = peopleIDOne,
                            OrgId = pTwoTag.OrgId,
                            MemberTagId = pTwoTag.MemberTagId
                        };

                        Db.OrgMemMemTags.DeleteOnSubmit(pTwoTag);
                        Db.OrgMemMemTags.InsertOnSubmit(pOneNew);
                    }
                }

                if (pOneTag != null)
                {
                    var pTwoNew = new OrgMemMemTag
                    {
                        PeopleId = peopleIDTwo,
                        OrgId = pOneTag.OrgId,
                        MemberTagId = pOneTag.MemberTagId
                    };

                    Db.OrgMemMemTags.DeleteOnSubmit(pOneTag);
                    Db.OrgMemMemTags.InsertOnSubmit(pTwoNew);
                }
            }
            Db.SubmitChanges();
        }
        public static void UpdateScore(CMSDataContext Db, string id, int value)
        {
            string[] split = id.Split('-');
            int orgID = split[0].ToInt();
            int peopleID = split[1].ToInt();

            var member = (from e in Db.OrganizationMembers
                          where e.OrganizationId == orgID
                          where e.PeopleId == peopleID
                          select e).SingleOrDefault();

            if (member != null)
            {
                member.Score = value;
            }
            Db.SubmitChanges();
        }
        internal static void ToggleCheckin(CMSDataContext Db, int orgId, int groupId)
        {
            var group = Db.MemberTags.SingleOrDefault(g => g.Id == groupId && g.OrgId == orgId);
            if (group is null)
            {
                throw new ArgumentException("error: no matching group found");
            }
            group.CheckIn = !group.CheckIn;
            Db.SubmitChanges();
        }
    }
}
