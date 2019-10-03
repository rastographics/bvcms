using CmsWeb.Areas.Org.Models;
using CmsWeb.Lifecycle;
using System;
using System.Web.Mvc;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix = "OrgGroups"), Route("{action}/{id?}")]
    public class OrgGroupsController : CmsStaffController
    {
        public OrgGroupsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/OrgGroups/{id:int}")]
        public ActionResult Index(int id)
        {
            var m = new OrgGroupsModel(CurrentDatabase, id);
            return View(m);
        }

        [HttpPost]
        public ActionResult Filter(OrgGroupsModel m)
        {
            return View("Rows", m);
        }

        [HttpPost]
        public ActionResult AssignSelectedToTargetGroup(OrgGroupsModel model)
        {
            model.AssignSelectedToTargetGroup();
            return View("Rows", model);
        }

        [HttpPost]
        [Route("{orgId:int}/SubGroups/{groupId:int}/ToggleCheckin")]
        public ActionResult ToggleCheckin(int orgId, int groupId)
        {
            try
            {
                OrgGroupsModel.ToggleCheckin(CurrentDatabase, orgId, groupId);
            }
            catch(ArgumentException ex)
            {
                return Content(ex.Message);
            }
            return Redirect("/OrgGroups/Management/" + orgId);
        }

        [HttpPost]
        public ActionResult MakeLeaderOfTargetGroup(OrgGroupsModel m)
        {
            m.MakeLeaderOfTargetGroup();
            return View("Rows", m);
        }

        [HttpPost]
        public ActionResult RemoveAsLeaderOfTargetGroup(OrgGroupsModel m)
        {
            m.RemoveAsLeaderOfTargetGroup();
            return View("Rows", m);
        }

        [HttpPost]
        public ActionResult RemoveSelectedFromTargetGroup(OrgGroupsModel m)
        {
            m.RemoveSelectedFromTargetGroup();
            return View("Rows", m);
        }

        [HttpPost]
        public ActionResult MakeNewGroup(OrgGroupsModel m)
        {
            try
            {
                var group = m.MakeNewGroup();
                ViewData["newgid"] = group.Id;
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
            return Redirect("/OrgGroups/Management/" + m.orgid);
        }


        [HttpPost]
        public ActionResult RenameGroup(OrgGroupsModel m)
        {
            try
            {
                m.RenameGroup();
            }
            catch (ArgumentException e)
            {
                return Content(e.Message);
            }
            return Redirect("/OrgGroups/Management/" + m.orgid);
        }

        [HttpPost]
        public ActionResult EditGroup(OrgGroupsModel m)
        {
            try
            {
                m.EditGroup();
            }
            catch (ArgumentException e)
            {
                return Content(e.Message);
            }
            return Redirect("/OrgGroups/Management/" + m.orgid);
        }

        [HttpPost]
        public ActionResult DeleteGroup(OrgGroupsModel m)
        {
            m.DeleteGroup();
            ViewData["groupid"] = m.groupid.ToString();
            return Redirect("/OrgGroups/Management/" + m.orgid);
        }

        [HttpPost]
        public ActionResult DeleteGroups(int id, int[] groups)
        {
            OrgGroupsModel.DeleteGroups(CurrentDatabase, groups);
            return Redirect("/OrgGroups/Management/" + id);
        }

        public ActionResult UpdateScore(string id, int value)
        {
            OrgGroupsModel.UpdateScore(CurrentDatabase, id, value);
            return Content(value.ToString());
        }

        public ActionResult UploadScores(string data, int orgID)
        {
            OrgGroupsModel.UploadScores(CurrentDatabase, data, orgID);
            return Content("OK");
        }

        public ActionResult SwapPlayers(string pOne, string pTwo)
        {
            OrgGroupsModel.SwapPlayers(CurrentDatabase, pOne, pTwo);
            return Content("Complete");
        }

        public ActionResult Management(int id)
        {
            var m = new OrgGroupsModel(CurrentDatabase, id);
            return View(m);
        }

        public ActionResult CreateTeams(int id)
        {
            var m = new OrgGroupsModel(CurrentDatabase, id);
            m.createTeamGroups();

            return Content("Complete");
        }
    }
}
