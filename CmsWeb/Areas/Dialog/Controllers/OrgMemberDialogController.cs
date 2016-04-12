using System;
using System.Data.Linq;
using System.Web.Mvc;
using CmsWeb.Areas.Dialog.Models;
using CmsData;
using CmsWeb.Code;
using CmsWeb.Models.ExtraValues;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "OrgMemberDialog"), Route("{action}")]
    public class OrgMemberDialogController : CmsStaffController
    {
        private const string AutoOrgLeaderPromotion = "AutoOrgLeaderPromotion";
        private const string LeaderMemberType = "Leader";
        private const string AccessRole = "Access";
        private const string OrgLeadersOnlyRole = "OrgLeadersOnly";

        [HttpPost, Route("~/OrgMemberDialog/{group}/{oid}/{pid}")]
        public ActionResult Display(string group, int oid, int pid)
        {
            var m = new OrgMemberModel(group, oid, pid);
            return View("Display", m);
        }

        [HttpPost]
        public ActionResult Display(OrgMemberModel m)
        {
            return View("Display", m);
        }

        [Authorize(Roles = "Admin,ManageGroups")]
        [HttpPost, Route("SmallGroupChecked/{sgtagid:int}")]
        public ActionResult SmallGroupChecked(int sgtagid, bool ck, OrgMemberModel m)
        {
            return Content(m.SmallGroupChanged(sgtagid, ck));
        }

        [HttpPost]
        public ActionResult Edit(OrgMemberModel m)
        {
            return View(m);
        }

        [HttpPost]
        public ActionResult Update(OrgMemberModel m)
        {
            try
            {
                m.UpdateModel();
                CheckForPromotion(m);
            }
            catch (Exception)
            {
                ViewData["MemberTypes"] = CodeValueModel.ConvertToSelect(CodeValueModel.MemberTypeCodes(), "Id");
                return View("Edit", m);
            }
            return View("Display", m);
        }

        [HttpPost]
        public ActionResult Move(OrgMemberMoveModel m)
        {
            return View(m);
        }

        [HttpPost, Route("MoveResults")]
        public ActionResult MoveResults(OrgMemberMoveModel m)
        {
            return View("Move", m);
        }

        [HttpPost, Route("MoveSelect/{toid:int}")]
        public ActionResult MoveSelect(int toid, OrgMemberMoveModel m)
        {
            var ret = m.Move(toid);
            return Content(ret);
        }

        [HttpPost]
        public ActionResult AddQuestions(OrgMemberModel m)
        {
            m.AddQuestions();
            return Content("ok");
        }
        [HttpPost]
        public ActionResult EditQuestion(string id, string value)
        {
            var a = id.Split(',');
            var oid = a[0].ToInt();
            var pid = a[1].ToInt();
            var n = a[2].ToInt();
            var m = new OrgMemberModel(oid, pid);
            m.UpdateQuestion(n, value);

            DbUtil.LogActivity("OrgMem EditQuestion " + n, oid, pid);
            var c = Content(value);
            return c;
        }

        public string HelpLink()
        {
            return "";
        }

        [HttpPost]
        public ActionResult MissionSupport(MissionSupportModel m)
        {
            return View(m);
        }

        [HttpPost]
        public ActionResult AddMissionSupport(MissionSupportModel m)
        {
            m.PostContribution();
            return View("MissionSupportDone", m);
        }

        [HttpPost]
        public ActionResult AddTransaction(OrgMemberTransactionModel m)
        {
            return View(m);
        }

        [HttpPost]
        public ActionResult AddFeeAdjustment(OrgMemberTransactionModel m)
        {
            m.AdjustFee = true;
            return View(m);
        }

        [HttpPost]
        public ActionResult PostTransaction(OrgMemberTransactionModel m)
        {
            m.PostTransaction(ModelState);
            if (!ModelState.IsValid)
                return View("AddTransaction", m);
            return View("AddTransactionDone", m);
        }

        [HttpPost, Route("ShowDrop")]
        public ActionResult ShowDrop(OrgMemberModel m)
        {
            return View(m);
        }

        [HttpPost]
        public ActionResult Drop(OrgMemberModel m)
        {
            DbUtil.LogActivity(m.RemoveFromEnrollmentHistory
                ? $"removed enrollment history on {m.PeopleId} for {m.OrgId}"
                : $"dropped {m.PeopleId} for {m.OrgId}");
            m.Drop();
            return Content("Done");
        }
        [HttpPost, Route("ExtraValues/{oid}/{pid}")]
        public ActionResult ExtraValues(int oid, int pid)
        {
            var em = new ExtraValueModel(oid, pid, "OrgMember", "Adhoc");
            return View("Tabs/ExtraValue/Adhoc", em);
        }
        [HttpPost, Route("NewExtraValue/{oid}/{pid}")]
        public ActionResult NewExtraValue(int oid, int pid)
        {
            var m = new NewExtraValueModel(oid, pid, "OrgMember", "Adhoc");
            return View("Tabs/ExtraValue/NewAdhoc", m);
        }
        [HttpPost]
        public ActionResult AddExtraValue(NewExtraValueModel m)
        {
            try
            {
                m.AddAsNewAdhoc();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Tabs/ExtraValue/NewAdHoc", m);
            }
            var em = new ExtraValueModel(m.Id, m.Id2, "OrgMember", "Adhoc");
            return View("Tabs/ExtraValue/Adhoc", em);
        }
        [HttpPost, Route("DeleteExtraValue/{oid:int}/{pid:int}")]
        public ActionResult DeleteExtraValue(int oid, int pid, string name)
        {
            var m = new ExtraValueModel(oid, pid, "OrgMember", "Adhoc");
            m.Delete(name);
            return Content("deleted");
		}

        private void CheckForPromotion(OrgMemberModel m)
        {
            // first make sure person is a user.
            var user = DbUtil.Db.Users.SingleOrDefault(us => us.PeopleId == m.PeopleId);
            if (user != null)
            {
                // next check for leader org promotion.
                var autoLeaderOrgPromotionSetting = DbUtil.DbReadOnly.Settings.SingleOrDefault(x => x.Id == AutoOrgLeaderPromotion);
                if (autoLeaderOrgPromotionSetting != null)
                {
                    var checkForPromotion = false;
                    bool.TryParse(autoLeaderOrgPromotionSetting.SettingX, out checkForPromotion);
                    if (checkForPromotion)
                    {
                        // check for member type change to leader and doesn't have role.
                        if (m.MemberType.ToString() == LeaderMemberType && !user.InRole(OrgLeadersOnlyRole))
                        {
                            user.AddRoles(DbUtil.Db, !user.InRole(AccessRole) ? new[] { AccessRole, OrgLeadersOnlyRole } : new[] { OrgLeadersOnlyRole });
                            DbUtil.Db.SubmitChanges();
                        }
                        else if(m.MemberType.ToString() != LeaderMemberType && user.InRole(OrgLeadersOnlyRole))
                        {
                            // check to see if this user no longer has any Leader membership types
                            if (!DbUtil.Db.OrganizationMembers
                                .Any(x => x.MemberType.Code == LeaderMemberType && x.PeopleId == m.PeopleId))
                            {
                                // Get the roles list minus the org leaders only role
                                var roles = user.Roles.Where(x => x != OrgLeadersOnlyRole).ToArray();

                                user.SetRoles(DbUtil.Db, roles);
                                DbUtil.Db.SubmitChanges();
                            }
                        }
                    }
                }
            }
        }
    }
}
