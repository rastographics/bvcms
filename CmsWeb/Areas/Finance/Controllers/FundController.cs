using CmsData;
using CmsWeb.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Finance.Controllers
{
    [Authorize(Roles = "Admin,Finance,FinanceViewOnly")]
    [RouteArea("Finance", AreaPrefix = "Fund"), Route("{action}/{id?}")]
    public class FundController : CmsStaffController
    {
        public FundController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/Funds")]
        public ActionResult Index(string sort, int? status)
        {
            var m = from f in CurrentDatabase.ContributionFunds
                    where f.FundStatusId == (status ?? 1)
                    select f;
            ViewBag.status = status ?? 1;
            switch (sort ?? "FundId")
            {
                case "FundId":
                    m = from f in m
                        orderby f.FundStatusId, f.FundId
                        select f;
                    break;
                case "Created":
                    m = from f in m
                        orderby f.CreatedDate descending
                        select f;
                    break;
                case "Sort":
                    m = from f in m
                        orderby f.OnlineSort != null ? 1 : 2, f.OnlineSort, f.FundId
                        select f;
                    break;
                case "Name":
                    m = from f in m
                        orderby f.FundName
                        select f;
                    break;
                case "StatusId":
                    m = from f in m
                        orderby f.FundStatusId, f.OnlineSort != null ? 1 : 2, f.OnlineSort, f.FundId
                        select f;
                    break;
                case "PledgeFlag":
                    m = from f in m
                        orderby f.FundPledgeFlag, f.FundStatusId, f.OnlineSort != null ? 1 : 2, f.OnlineSort, f.FundId
                        select f;
                    break;
                case "NonTaxDed":
                    m = from f in m
                        orderby f.NonTaxDeductible, f.FundPledgeFlag, f.FundStatusId, f.OnlineSort != null ? 1 : 2, f.OnlineSort, f.FundId
                        select f;
                    break;
            }
            return View(m);
        }

        [HttpPost]
        public ActionResult Create(string fundid)
        {
            var id = fundid.ToInt();
            if (id == 0)
            {
                return Json(new { error = "expected an integer (account number)" });
            }

            var f = CurrentDatabase.ContributionFunds.SingleOrDefault(ff => ff.FundId == id);
            if (f != null)
            {
                return Json(new { error = $"fund already exists: {f.FundName} ({fundid})" });
            }

            try
            {
                f = new ContributionFund
                {
                    FundName = "new fund",
                    FundId = id,
                    CreatedBy = Util.UserId1,
                    CreatedDate = Util.Now,
                    FundStatusId = 1,
                    FundTypeId = 1,
                    FundPledgeFlag = false,
                    NonTaxDeductible = false
                };
                CurrentDatabase.ContributionFunds.InsertOnSubmit(f);
                CurrentDatabase.SubmitChanges();
                return Json(new { edit = "/Fund/Edit/" + id });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public ActionResult Edit(int id)
        {
            var fund = CurrentDatabase.ContributionFunds.SingleOrDefault(f => f.FundId == id);
            if (fund == null)
            {
                RedirectToAction("Index");
            }

            return View(fund);
        }

        public ActionResult Delete(int id)
        {
            var f = CurrentDatabase.ContributionFunds.SingleOrDefault(fu => fu.FundId == id);
            if (f != null)
            {
                CurrentDatabase.ContributionFunds.DeleteOnSubmit(f);
            }

            CurrentDatabase.SubmitChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Update(int fundId)
        {
            var fund = CurrentDatabase.ContributionFunds.SingleOrDefault(f => f.FundId == fundId);
            if (fund != null)
            {
                UpdateModel(fund);
            }

            if (ModelState.IsValid)
            {
                CurrentDatabase.SubmitChanges();
                TempData["SuccessMessage"] = "Fund was successfully saved.";
                return RedirectToAction("Index");
            }
            return View("Edit", fund);
        }

        [HttpPost]
        public ContentResult EditOrder(string id, int? value)
        {
            var iid = id.Substring(1).ToInt();
            var fund = CurrentDatabase.ContributionFunds.SingleOrDefault(m => m.FundId == iid);
            fund.OnlineSort = value;
            CurrentDatabase.SubmitChanges();
            return Content(value.ToString());
        }

        [HttpPost]
        public ContentResult EditStatus(string id, int value)
        {
            var iid = id.Substring(1).ToInt();
            var fund = CurrentDatabase.ContributionFunds.SingleOrDefault(m => m.FundId == iid);
            fund.FundStatusId = value;
            CurrentDatabase.SubmitChanges();
            return Content(value == 1 ? "Open" : "Closed");
        }

        public static List<SelectListItem> GetFundStatusList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "Open", Value = "1" });
            list.Add(new SelectListItem { Text = "Closed", Value = "2" });
            return list;
        }

        public static List<SelectListItem> GetFundTypeList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "1", Value = "1" });
            list.Add(new SelectListItem { Text = "2", Value = "2" });
            list.Add(new SelectListItem { Text = "3", Value = "3" });
            return list;
        }

        public static List<SelectListItem> GetRolesList()
        {
            //todo: static?
            var roles = DbUtil.Db.Roles.OrderBy(r => r.RoleName).Select(r => new SelectListItem { Value = r.RoleId.ToString(), Text = r.RoleName }).ToList();
            roles.Insert(0, new SelectListItem { Value = "-1", Text = "(not assigned)" });
            roles.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)", Selected = true });

            return roles;
        }
    }
}
