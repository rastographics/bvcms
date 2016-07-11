using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Finance;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Edit, ManageTransactions")]
    [RouteArea("Manage", AreaPrefix = "Transactions"), Route("{action}/{id?}")]
    public class TransactionsController : CmsStaffController
    {
        [HttpGet]
        [Route("~/Transactions")]
        [Route("~/Transactions/{id:int}")]
        public ActionResult Index(int? id, int? goerid = null, int? senderid = null, string reference = "", string desc = "")
        {
            var m = new TransactionsModel(id, reference, desc) {GoerId = goerid, SenderId = senderid};
            return View(m);
        }

        [HttpPost]
        public ActionResult Report(DateTime? sdt, DateTime? edt)
        {
            var m = new TransactionsModel
            {
                startdt = sdt,
                enddt = edt,
                usebatchdates = true,
            };
            return View(m);
        }

        [HttpPost]
        public ActionResult ReportByDescription(DateTime? sdt, DateTime? edt)
        {
            var m = new TransactionsModel
            {
                startdt = sdt,
                enddt = edt,
                usebatchdates = true,
            };
            return View(m);
        }

        [HttpPost]
        public ActionResult ReportByBatchDescription(DateTime? sdt, DateTime? edt)
        {
            var m = new TransactionsModel
            {
                startdt = sdt,
                enddt = edt,
                usebatchdates = true,
            };
            return View(m);
        }

        [HttpPost]
        public ActionResult List(TransactionsModel m)
        {
            UpdateModel(m.Pager);
            return View(m);
        }

        [HttpPost]
        public ActionResult Export(TransactionsModel m)
        {
            return m.ToExcel();
        }

        [HttpGet]
        [Authorize(Roles = "Finance")]
        public ActionResult RunRecurringGiving()
        {
            var count = ManagedGiving.DoAllGiving(DbUtil.Db);
            return Content(count.ToString());
        }

        [HttpPost]
        [Authorize(Roles = "Developer")]
        public ActionResult SetParent(int id, int parid, TransactionsModel m)
        {
            var t = DbUtil.Db.Transactions.SingleOrDefault(tt => tt.Id == id);
            if (t == null)
                return Content("notran");
            t.OriginalId = parid;
            DbUtil.Db.SubmitChanges();
            return View("List", m);
        }

        [HttpPost]
        [Authorize(Roles = "ManageTransactions,Finance")]
        public ActionResult CreditVoid(int id, string type, decimal? amt, TransactionsModel m)
        {
            var t = DbUtil.Db.Transactions.SingleOrDefault(tt => tt.Id == id);
            if (t == null)
                return Content("notran");

            var qq = from tt in DbUtil.Db.Transactions
                where tt.OriginalId == id || tt.Id == id
                orderby tt.Id descending
                select tt;
            var t0 = qq.First();
            var gw = DbUtil.Db.Gateway(t.Testing ?? false);
            TransactionResponse resp = null;
            var re = t.TransactionId;
            if (re.Contains("(testing"))
                re = re.Substring(0, re.IndexOf("(testing)"));
            if (type == "Void")
            {
                resp = gw.VoidCreditCardTransaction(re);
                if (!resp.Approved)
                    resp = gw.VoidCheckTransaction(re);

                t.Voided = resp.Approved;
                amt = t.Amt;
            }
            else
            {
                if (t.PaymentType == PaymentType.Ach)
                    resp = gw.RefundCheck(re, amt ?? 0);
                else if (t.PaymentType == PaymentType.CreditCard)
                    resp = gw.RefundCreditCard(re, amt ?? 0);

                t.Credited = resp.Approved;
            }

            if (!resp.Approved)
                return Content("error: " + resp.Message);

            var transaction = new Transaction
            {
                TransactionId = resp.TransactionId + (t.Testing == true ? "(testing)" : ""),
                First = t.First,
                MiddleInitial = t.MiddleInitial,
                Last = t.Last,
                Suffix = t.Suffix,
                Amt = -amt,
                Amtdue = t0.Amtdue + amt,
                Approved = true,
                AuthCode = t.AuthCode,
                Message = t.Message,
                Donate = -t.Donate,
                Regfees = -t.Regfees,
                TransactionDate = DateTime.Now,
                TransactionGateway = t.TransactionGateway,
                Testing = t.Testing,
                Description = t.Description,
                OrgId = t.OrgId,
                OriginalId = t.OriginalId,
                Participants = t.Participants,
                Financeonly = t.Financeonly,
                PaymentType = t.PaymentType,
                LastFourCC = t.LastFourCC,
                LastFourACH = t.LastFourACH
            };

            DbUtil.Db.Transactions.InsertOnSubmit(transaction);
            DbUtil.Db.SubmitChanges();
            Util.SendMsg(DbUtil.Db.SysFromEmail, Util.Host,
                Util.TryGetMailAddress(DbUtil.Db.StaffEmailForOrg(transaction.OrgId ?? 0)),
                "Void/Credit Transaction Type: " + type,
                $@"<table>
<tr><td>Name</td><td>{Transaction.FullName(t)}</td></tr>
<tr><td>Email</td><td>{t.Emails}</td></tr>
<tr><td>Address</td><td>{t.Address}</td></tr>
<tr><td>Phone</td><td>{t.Phone}</td></tr>
<tr><th colspan=""2"">Transaction Info</th></tr>
<tr><td>Description</td><td>{t.Description}</td></tr>
<tr><td>Amount</td><td>{-amt:N2}</td></tr>
<tr><td>Date</td><td>{t.TransactionDate.Value.FormatDateTm()}</td></tr>
<tr><td>TranIds</td><td>Org: {t.Id} {t.TransactionId}, Curr: {transaction.Id} {transaction.TransactionId}</td></tr>
<tr><td>User</td><td>{Util.UserFullName}</td></tr>
</table>", Util.EmailAddressListFromString(DbUtil.Db.StaffEmailForOrg(transaction.OrgId ?? 0)),
                0, 0);
            DbUtil.LogActivity("CreditVoid for " + t.TransactionId);

            return View("List", m);
        }

        [HttpPost]
        [Authorize(Roles = "ManageTransactions,Finance")]
        public ActionResult DeleteManual(int id, TransactionsModel m)
        {
            DbUtil.Db.ExecuteCommand(@"
UPDATE dbo.OrganizationMembers SET TranId = NULL WHERE TranId = {0}
DELETE dbo.TransactionPeople WHERE Id = {0}
DELETE dbo.[Transaction] WHERE Id = {0}
", id);
            return View("List", m);
        }
        [HttpPost]
        [Authorize(Roles = "ManageTransactions,Finance")]
        public ActionResult DeleteGoerSenderAmount(int id, TransactionsModel m)
        {
            var gsa = DbUtil.Db.GoerSenderAmounts.Single(tt => tt.Id == id);
            DbUtil.Db.GoerSenderAmounts.DeleteOnSubmit(gsa);
            DbUtil.Db.SubmitChanges();
            return View("GoerSupporters", m);
        }
        [Authorize(Roles = "ManageTransactions,Finance,Developer")]
        [HttpPost, Route("AssignGoer/{id:int}/{gid:int}")]
        public ActionResult AssignGoer(int id, int? gid, TransactionsModel m)
        {
            if (gid == 0)
                gid = null;
            var gsa = DbUtil.Db.GoerSenderAmounts.Single(tt => tt.Id == id);
            gsa.GoerId = gid;
            DbUtil.Db.SubmitChanges();
            return View("GoerSupporters", m);
        }
        [HttpPost]
        [Authorize(Roles = "ManageTransactions,Finance")]
        public ActionResult Adjust(int id, decimal amt, string desc, TransactionsModel m)
        {
            var qq = from tt in DbUtil.Db.Transactions
                where tt.OriginalId == id || tt.Id == id
                orderby tt.Id descending
                select tt;
            var t = qq.FirstOrDefault();

            if (t == null)
                return Content("notran");

            var t2 = new Transaction
            {
                TransactionId = "Adjustment",
                First = t.First,
                MiddleInitial = t.MiddleInitial,
                Last = t.Last,
                Suffix = t.Suffix,
                Amt = amt,
                Emails = t.Emails,
                Amtdue = t.Amtdue - amt,
                Approved = true,
                AuthCode = "",
                Message = desc,
                Donate = -t.Donate,
                Regfees = -t.Regfees,
                TransactionDate = DateTime.Now,
                TransactionGateway = t.TransactionGateway,
                Testing = t.Testing,
                Description = t.Description,
                OrgId = t.OrgId,
                OriginalId = t.OriginalId,
                Participants = t.Participants,
                Financeonly = t.Financeonly,
                PaymentType = t.PaymentType,
                LastFourCC = t.LastFourCC,
                LastFourACH = t.LastFourACH
            };

            DbUtil.Db.Transactions.InsertOnSubmit(t2);
            DbUtil.Db.SubmitChanges();
            Util.SendMsg(DbUtil.Db.SysFromEmail, Util.Host,
                Util.TryGetMailAddress(DbUtil.Db.StaffEmailForOrg(t2.OrgId ?? 0)),
                "Adjustment Transaction",
                $@"<table>
<tr><td>Name</td><td>{Transaction.FullName(t)}</td></tr>
<tr><td>Email</td><td>{t.Emails}</td></tr>
<tr><td>Address</td><td>{t.Address}</td></tr>
<tr><td>Phone</td><td>{t.Phone}</td></tr>
<tr><th colspan=""2"">Transaction Info</th></tr>
<tr><td>Description</td><td>{t.Description}</td></tr>
<tr><td>Amount</td><td>{t.Amt:N2}</td></tr>
<tr><td>Date</td><td>{t.TransactionDate.Value.FormatDateTm()}</td></tr>
<tr><td>TranIds</td><td>Org: {t.Id} {t.TransactionId}, Curr: {t2.Id} {t2.TransactionId}</td></tr>
</table>", Util.EmailAddressListFromString(DbUtil.Db.StaffEmailForOrg(t2.OrgId ?? 0)),
                0, 0);
            DbUtil.LogActivity("Adjust for " + t.TransactionId);
            return View("List", m);
        }
    }
}
