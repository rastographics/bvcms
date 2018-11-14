using CmsData;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Lifecycle;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix = "OrgGroups"), Route("{action}/{id?}")]
    public class OrgGroupsController : CmsStaffController
    {
        public OrgGroupsController(RequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/OrgGroups/{id:int}")]
        public ActionResult Index(int id)
        {
            var m = new OrgGroupsModel(id);
            return View(m);
        }
        [HttpPost]
        public ActionResult Filter(OrgGroupsModel m)
        {
            return View("Rows", m);
        }
        [HttpPost]
        public ActionResult AssignSelectedToTargetGroup(OrgGroupsModel m)
        {
            var a = m.List.ToArray();
            var sgname = CurrentDatabase.MemberTags.Single(mt => mt.Id == m.groupid).Name;
            var q2 = from om in m.OrgMembers()
                     where om.OrgMemMemTags.All(mt => mt.MemberTag.Name != sgname)
                     where a.Contains(om.PeopleId)
                     select om;
            foreach (var om in q2)
            {
                om.AddToGroup(CurrentDatabase, sgname);
            }

            CurrentDatabase.SubmitChanges();
            return View("Rows", m);
        }

        [HttpPost]
        public ActionResult MakeLeaderOfTargetGroup(OrgGroupsModel m)
        {
            var a = m.List.ToArray();
            var q2 = from om in m.OrgMembers()
                     where a.Contains(om.PeopleId)
                     select om;
            if (m.groupid != null)
            {
                foreach (var om in q2)
                {
                    om.MakeLeaderOfGroup(CurrentDatabase, m.groupid.GetValueOrDefault());
                }
            }

            CurrentDatabase.SubmitChanges();
            return View("Rows", m);
        }

        [HttpPost]
        public ActionResult RemoveAsLeaderOfTargetGroup(OrgGroupsModel m)
        {
            var a = m.List.ToArray();
            var q2 = from om in m.OrgMembers()
                     where a.Contains(om.PeopleId)
                     select om;
            if (m.groupid != null)
            {
                foreach (var om in q2)
                {
                    om.RemoveAsLeaderOfGroup(CurrentDatabase, m.groupid.GetValueOrDefault());
                }
            }
            CurrentDatabase.SubmitChanges();
            return View("Rows", m);
        }

        [HttpPost]
        public ActionResult RemoveSelectedFromTargetGroup(OrgGroupsModel m)
        {
            var a = m.List.ToArray();
            var sgname = CurrentDatabase.MemberTags.Single(mt => mt.Id == m.groupid).Name;
            var q1 = from omt in CurrentDatabase.OrgMemMemTags
                     where omt.OrgId == m.orgid
                     where omt.MemberTag.Name == sgname
                     where a.Contains(omt.PeopleId)
                     select omt;
            CurrentDatabase.OrgMemMemTags.DeleteAllOnSubmit(q1);
            CurrentDatabase.SubmitChanges();
            return View("Rows", m);
        }
        [HttpPost]
        public ActionResult MakeNewGroup(OrgGroupsModel m)
        {
            if (!m.GroupName.HasValue())
            {
                return Content("error: no group name");
            }

            //var Db = Db;
            var group = CurrentDatabase.MemberTags.SingleOrDefault(g =>
                g.Name == m.GroupName && g.OrgId == m.orgid);
            if (group == null)
            {
                group = new MemberTag
                {
                    Name = m.GroupName,
                    OrgId = m.orgid
                };
                CurrentDatabase.MemberTags.InsertOnSubmit(group);
                CurrentDatabase.SubmitChanges();
            }
            m.groupid = group.Id;
            ViewData["newgid"] = group.Id;
            return Redirect("/OrgGroups/Management/" + m.orgid);
        }
        [HttpPost]
        public ActionResult RenameGroup(OrgGroupsModel m)
        {
            if (!m.GroupName.HasValue() || m.groupid == 0)
            {
                return Content("error: no group name");
            }

            var group = CurrentDatabase.MemberTags.SingleOrDefault(d => d.Id == m.groupid);
            if (group != null)
            {
                group.Name = m.GroupName;
            }

            CurrentDatabase.SubmitChanges();
            m.GroupName = null;
            return Redirect("/OrgGroups/Management/" + m.orgid);
        }
        [HttpPost]
        public ActionResult DeleteGroup(OrgGroupsModel m)
        {
            var group = CurrentDatabase.MemberTags.SingleOrDefault(g => g.Id == m.groupid);
            if (group != null)
            {
                CurrentDatabase.OrgMemMemTags.DeleteAllOnSubmit(group.OrgMemMemTags);
                CurrentDatabase.MemberTags.DeleteOnSubmit(group);
                CurrentDatabase.SubmitChanges();
                m.groupid = (from v in m.Groups()
                             where v.Value != "0"
                             select v.Value).FirstOrDefault().ToInt();
                ViewData["groupid"] = m.groupid.ToString();
            }
            return Redirect("/OrgGroups/Management/" + m.orgid);
        }

        [HttpPost]
        public ActionResult DeleteGroups(int id, int[] groups)
        {
            var groupList = CurrentDatabase.MemberTags.Where(t => groups.Contains(t.Id));

            foreach (var group in groupList)
            {
                CurrentDatabase.OrgMemMemTags.DeleteAllOnSubmit(group.OrgMemMemTags);
                CurrentDatabase.MemberTags.DeleteOnSubmit(group);
            }

            CurrentDatabase.SubmitChanges();

            return Redirect("/OrgGroups/Management/" + id);
        }

        public ActionResult UpdateScore(string id, int value)
        {
            string[] split = id.Split('-');
            int orgID = split[0].ToInt();
            int peopleID = split[1].ToInt();

            var member = (from e in CurrentDatabase.OrganizationMembers
                          where e.OrganizationId == orgID
                          where e.PeopleId == peopleID
                          select e).SingleOrDefault();

            if (member != null)
            {
                member.Score = value;
            }

            CurrentDatabase.SubmitChanges();

            return Content(value.ToString());
        }

        public ActionResult UploadScores(string data, int orgID)
        {
            var csv = new CsvReader(new StringReader(data), false, '\t');
            var list = csv.ToList();

            foreach (var score in list)
            {
                var peopleID = score[0].ToInt();

                var player = (from e in CurrentDatabase.OrganizationMembers
                              where e.OrganizationId == orgID
                              where e.PeopleId == peopleID
                              select e).SingleOrDefault();

                if (player != null)
                {
                    player.Score = score[1].ToInt();
                }

                CurrentDatabase.SubmitChanges();
            }

            return Content("OK");
        }

        public ActionResult SwapPlayers(string pOne, string pTwo)
        {
            string[] splitOne = pOne.Split('-');
            int orgIDOne = splitOne[0].ToInt();
            int peopleIDOne = splitOne[1].ToInt();

            string[] splitTwo = pTwo.Split('-');
            int orgIDTwo = splitTwo[0].ToInt();
            int peopleIDTwo = splitTwo[1].ToInt();

            var playerOne = (from e in CurrentDatabase.OrganizationMembers
                             where e.OrganizationId == orgIDOne
                             where e.PeopleId == peopleIDOne
                             select e).SingleOrDefault();

            var playerTwo = (from e in CurrentDatabase.OrganizationMembers
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

                        CurrentDatabase.OrgMemMemTags.DeleteOnSubmit(pTwoTag);
                        CurrentDatabase.OrgMemMemTags.InsertOnSubmit(pOneNew);
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

                    CurrentDatabase.OrgMemMemTags.DeleteOnSubmit(pOneTag);
                    CurrentDatabase.OrgMemMemTags.InsertOnSubmit(pTwoNew);
                }
            }

            CurrentDatabase.SubmitChanges();
            return Content("Complete");
        }

        public ActionResult Management(int id)
        {
            var m = new OrgGroupsModel(id);
            return View(m);
        }

        public ActionResult CreateTeams(int id)
        {
            var m = new OrgGroupsModel(id);
            m.createTeamGroups();

            return Content("Complete");
        }
    }
}
