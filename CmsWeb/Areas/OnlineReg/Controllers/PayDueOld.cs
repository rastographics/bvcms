using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Controllers
{
    public partial class OnlineRegController
    {
        public ActionResult PayDue(string q)
        {
            if (!q.HasValue())
                return Content("unknown");
            var id = Util.Decrypt(q);
            var ed = DbUtil.Db.ExtraDatas.SingleOrDefault(e => e.Id == id.ToInt());
            if (ed == null)
                return Content("no outstanding transaction");
            PaymentModel pm = null;
            try
            {
                var s = ed.Data.Replace("CMSWeb.Models", "CmsWeb.Models");
                var ti = Util.DeSerialize<TransactionInfo>(s);
                pm = new PaymentModel
                {
                    NameOnAccount = ti.Name,
                    Address = ti.Address,
                    Amount = ti.AmountDue,
                    City = ti.City,
                    Email = ti.Email,
                    Phone = ti.Phone.FmtFone(),
                    State = ti.State,
                    PostalCode = ti.Zip,
                    testing = ti.testing,
                    PostbackURL = DbUtil.Db.ServerLink("/OnlineReg/ConfirmServiceU/" + id),
                    Misc2 = ti.Header,
                    Misc1 = ti.Name,
                    _URL = ti.URL,
                    _timeout = new PaymentForm().TimeOut,
                    _datumid = ed.Id,
                };

                SetHeaders(ti.orgid);
                return View("PayAmtDue/ServiceU", pm);
            }
            catch (Exception ex)
            {
                throw new Exception("Problem in PayDue: " + ed.Data, ex);
            }
        }
        public ActionResult Confirm2(int? id, string TransactionID, decimal Amount)
        {
            var Db = DbUtil.Db;
            if (!id.HasValue)
                return View("Unknown");
            if (!TransactionID.HasValue())
                return Content("error no transaction");

            var ed = Db.ExtraDatas.SingleOrDefault(e => e.Id == id);
            if (ed == null)
                return Content("no pending transaction found");

            var ti = Util.DeSerialize<TransactionInfo>(ed.Data.Replace("CMSWeb.Models", "CmsWeb.Models"));
            var org = Db.LoadOrganizationById(ti.orgid);
            if (ti.AmountDue == Amount)
            {
                ti.AmountDue = 0;
                Db.ExtraDatas.DeleteOnSubmit(ed);
            }
            else
            {
                ti.AmountDue -= Amount;
                var s = Util.Serialize<TransactionInfo>(ti);
                ed.Data = s;
                ed.Stamp = Util.Now;
            }
            var amt = Amount;
            var ar = new List<Person>();
            foreach (var pi in ti.people)
            {
                var p = Db.LoadPersonById(pi.pid);
                ar.Add(p);

                if (p != null)
                {
                    var om = Db.OrganizationMembers.SingleOrDefault(m => m.OrganizationId == ti.orgid && m.PeopleId == pi.pid);

                    if (om != null)
                    {
                        var due = (om.Amount - om.TotalPaid(DbUtil.Db)) ?? 0;
                        var pay = amt;
                        if (pay > due)
                            pay = due;
                        //om.AmountPaid += pay;

                        string tstamp = Util.Now.ToString("MMM d yyyy h:mm tt");
                        om.AddToMemberData(tstamp);
                        var tran = "{0:C} ({1} {2})".Fmt(
                            pay, TransactionID, ti.testing == true ? " test" : "");
                        om.AddToMemberData(tran);

                        var reg = p.RecRegs.Single();
                        reg.AddToComments("-------------");
                        reg.AddToComments(tran);
                        reg.AddToComments(Util.Now.ToString("MMM d yyyy h:mm tt"));
                        reg.AddToComments("{0} - {1}".Fmt(org.DivisionName, org.OrganizationName));

                        amt -= pay;
                    }
                }
                else
                    Db.Email(Db.StaffEmailForOrg(org.OrganizationId),
                        Db.PeopleFromPidString(org.NotifyIds), 
                        "missing person on payment due", 
                        "Cannot find {0} ({1}), payment due completed of {2:c} but no record".Fmt(pi.name, pi.pid, pi.amt));
            }
            DbUtil.Db.SubmitChanges();
            var names = string.Join(", ", ti.people.Select(i => i.name).ToArray());
            Db.Email(Db.StaffEmailForOrg(org.OrganizationId), ar,
                "Payment confirmation", "Thank you for paying {0:c} for {1}.<br/>Your balance is {2:c}<br/>{3}".Fmt(Amount, ti.Header, ti.AmountDue, names));
            Db.Email(ar.First().FromEmail,
                Db.PeopleFromPidString(org.NotifyIds), 
                "payment received for " + ti.Header, 
                "{0} paid {1:c} for {2}, balance of {3:c}\n({4})".Fmt(ti.Name, Amount, ti.Header, ti.AmountDue, names));

            ViewBag.Amount = Amount.ToString("c");
            SetHeaders(ti.orgid);
            return View("PayAmtDue/OldConfirm", ti);
        }
    }
}
