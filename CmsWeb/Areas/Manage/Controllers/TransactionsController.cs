using CmsData;
using CmsData.Finance;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Edit, ManageTransactions")]
    [RouteArea("Manage", AreaPrefix = "Transactions"), Route("{action}/{id?}")]
    public class TransactionsController : CmsStaffController
    {
        public TransactionsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        [Route("~/Transactions")]
        [Route("~/Transactions/{id:int}")]
        public ActionResult Index(int? id, int? goerid = null, int? senderid = null, string reference = "", string desc = "")
        {
            var m = new TransactionsModel(id, reference, desc) { GoerId = goerid, SenderId = senderid };
            return View(m);
        }

        [HttpPost]
        public ActionResult Report(TransactionsModel m)
        {
            return View(m);
        }

        [HttpPost]
        public ActionResult ReportByDescription(TransactionsModel m)
        {
            return View(m);
        }

        [HttpPost]
        public ActionResult ReportByBatchDescription(TransactionsModel m)
        {
            return View(m);
        }

        [HttpPost]
        public ActionResult TransactionsWithExtraDonationAmount(TransactionsModel m)
        {
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
            var count = ManagedGiving.DoAllGiving(CurrentDatabase);
            return Content(count.ToString());
        }

        [HttpPost]
        [Authorize(Roles = "Developer")]
        public ActionResult SetParent(int id, int parid, TransactionsModel m)
        {
            var t = CurrentDatabase.Transactions.SingleOrDefault(tt => tt.Id == id);
            if (t == null)
            {
                return Content("notran");
            }

            t.OriginalId = parid;
            CurrentDatabase.SubmitChanges();
            return View("List", m);
        }

        [HttpPost]
        [Authorize(Roles = "ManageTransactions,Finance")]
        public ActionResult CreditVoid(int id, string type, decimal? amt, TransactionsModel m)
        {
            var t = CurrentDatabase.Transactions.SingleOrDefault(tt => tt.Id == id);
            if (t == null)
            {
                return Content("notran");
            }

            var qq = from tt in CurrentDatabase.Transactions
                     where tt.OriginalId == id || tt.Id == id
                     orderby tt.Id descending
                     select tt;
            var t0 = qq.First();
            var gw = CurrentDatabase.Gateway(t.Testing ?? false);
            TransactionResponse resp = null;
            var re = t.TransactionId;
            if (re.Contains("(testing"))
            {
                re = re.Substring(0, re.IndexOf("(testing)"));
            }

            if (type == "Void")
            {
                resp = gw.VoidCreditCardTransaction(re);
                if (!resp.Approved)
                {
                    resp = gw.VoidCheckTransaction(re);
                }

                t.Voided = resp.Approved;
                amt = t.Amt;
            }
            else
            {
                if (t.PaymentType == PaymentType.Ach)
                {
                    resp = gw.RefundCheck(re, amt ?? 0);
                }
                else if (t.PaymentType == PaymentType.CreditCard)
                {
                    resp = gw.RefundCreditCard(re, amt ?? 0, t.LastFourCC);
                }

                t.Credited = resp.Approved;
            }

            if (!resp.Approved)
            {
                DbUtil.LogActivity("error: " + resp.Message);
                return Content("error: " + resp.Message);
            }

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

            CurrentDatabase.Transactions.InsertOnSubmit(transaction);
            CurrentDatabase.SubmitChanges();
            DbUtil.LogActivity("CreditVoid for " + t.TransactionId);

            CurrentDatabase.SendEmail(Util.TryGetMailAddress(CurrentDatabase.StaffEmailForOrg(transaction.OrgId ?? 0)),
                "Void/Credit Transaction Type: " + type,
                $@"<table>
<tr><td>Name</td><td>{Transaction.FullName(t)}</td></tr>
<tr><td>Email</td><td>{t.Emails}</td></tr>
<tr><td>Address</td><td>{t.Address}</td></tr>
<tr><td>Phone</td><td>{t.Phone}</td></tr>
<tr><th colspan=""2"">Transaction Info</th></tr>
<tr><td>Description</td><td>{t.Description}</td></tr>
<tr><td>Amount</td><td>{-amt:N2}</td></tr>
<tr><td>Date</td><td>{t.TransactionDate.FormatDateTm()}</td></tr>
<tr><td>TranIds</td><td>Org: {t.Id} {t.TransactionId}, Curr: {transaction.Id} {transaction.TransactionId}</td></tr>
<tr><td>User</td><td>{Util.UserFullName}</td></tr>
</table>", Util.EmailAddressListFromString(CurrentDatabase.StaffEmailForOrg(transaction.OrgId ?? 0)));

            return View("List", m);
        }

        [HttpPost]
        [Authorize(Roles = "ManageTransactions,Finance")]
        public ActionResult DeleteManual(int id, TransactionsModel m)
        {
            CurrentDatabase.ExecuteCommand(@"
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
            var gsa = CurrentDatabase.GoerSenderAmounts.Single(tt => tt.Id == id);
            CurrentDatabase.GoerSenderAmounts.DeleteOnSubmit(gsa);
            CurrentDatabase.SubmitChanges();
            return View("GoerSupporters", m);
        }
        [Authorize(Roles = "ManageTransactions,Finance,Developer")]
        [HttpPost, Route("AssignGoer/{id:int}/{gid:int}")]
        public ActionResult AssignGoer(int id, int? gid, TransactionsModel m)
        {
            if (gid == 0)
            {
                gid = null;
            }

            var gsa = CurrentDatabase.GoerSenderAmounts.Single(tt => tt.Id == id);
            gsa.GoerId = gid;
            CurrentDatabase.SubmitChanges();
            return View("GoerSupporters", m);
        }
        [HttpPost]
        [Authorize(Roles = "ManageTransactions,Finance")]
        public ActionResult Adjust(int id, decimal amt, string desc, TransactionsModel m)
        {
            var qq = from tt in CurrentDatabase.Transactions
                     where tt.OriginalId == id || tt.Id == id
                     orderby tt.Id descending
                     select tt;
            var t = qq.FirstOrDefault();

            if (t == null)
            {
                return Content("notran");
            }

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

            CurrentDatabase.Transactions.InsertOnSubmit(t2);
            CurrentDatabase.SubmitChanges();
            CurrentDatabase.SendEmail(Util.TryGetMailAddress(CurrentDatabase.StaffEmailForOrg(t2.OrgId ?? 0)),
                "Adjustment Transaction",
                $@"<table>
<tr><td>Name</td><td>{Transaction.FullName(t)}</td></tr>
<tr><td>Email</td><td>{t.Emails}</td></tr>
<tr><td>Address</td><td>{t.Address}</td></tr>
<tr><td>Phone</td><td>{t.Phone}</td></tr>
<tr><th colspan=""2"">Transaction Info</th></tr>
<tr><td>Description</td><td>{t.Description}</td></tr>
<tr><td>Amount</td><td>{t.Amt:N2}</td></tr>
<tr><td>Date</td><td>{t.TransactionDate.FormatDateTm()}</td></tr>
<tr><td>TranIds</td><td>Org: {t.Id} {t.TransactionId}, Curr: {t2.Id} {t2.TransactionId}</td></tr>
</table>", Util.EmailAddressListFromString(CurrentDatabase.StaffEmailForOrg(t2.OrgId ?? 0)));
            DbUtil.LogActivity("Adjust for " + t.TransactionId);
            return View("List", m);
        }
    }
}
