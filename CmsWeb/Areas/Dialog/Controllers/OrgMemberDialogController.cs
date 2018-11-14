using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Code;
using CmsWeb.Lifecycle;
using CmsWeb.Models.ExtraValues;
using System;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Dialog.Controllers
{
    [RouteArea("Dialog", AreaPrefix = "OrgMemberDialog"), Route("{action}")]
    public class OrgMemberDialogController : CmsStaffController
    {
        public OrgMemberDialogController(IRequestManager requestManager) : base(requestManager)
        {
        }

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
            var typ = a[3];
            var m = new OrgMemberModel(oid, pid);
            m.UpdateQuestion(n, typ, value);

            DbUtil.LogActivity("OrgMem EditQuestion " + n, oid, pid);
            var c = Content(value);
            return c;
        }
        [Authorize(Roles = "Developer")]
        [HttpPost, Route("DeleteQuestion/{id}")]
        public ActionResult DeleteQuestion(string id)
        {
            var a = id.Split(',');
            var oid = a[0].ToInt();
            var pid = a[1].ToInt();
            var n = a[2].ToInt();
            var typ = a[3];
            var m = new OrgMemberModel(oid, pid);
            m.DeleteQuestion(n, typ);
            return Content("ok");
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
            {
                return View("AddTransaction", m);
            }

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
        [HttpPost, Route("AddNewSmallGroup/{id:int}")]
        public ActionResult AddNewSmallGroup(int id, OrgMemberModel m)
        {
            m.AddNewSmallGroup(id);
            ModelState.Clear();
            return View("Tabs/Groups", m);
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
    }
}
