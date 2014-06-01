using System;
using System.IO;
using System.Web;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using CmsData.Properties;
using UtilityExtensions;

namespace CmsData
{
    public partial class ManagedGiving
    {
        public DateTime FindNextDate(DateTime ndt)
        {
            if (ndt.Date == DateTime.Today)
                ndt = ndt.AddDays(1).Date;
            if (StartWhen.HasValue && ndt.Date < StartWhen)
                ndt = StartWhen.Value;

            if (SemiEvery == "S")
            {
                var dt1 = new DateTime(ndt.Year, ndt.Month, Day1.Value);
                var dt2 = new DateTime(ndt.Year, ndt.Month,
                        Math.Min(DateTime.DaysInMonth(ndt.Year, ndt.Month), Day2.Value));
                if (ndt <= dt1)
                    return dt1;
                if (ndt <= dt2)
                    return dt2;
                return dt1.AddMonths(1);
            }
            else
            {
                var dt = StartWhen.Value;
                var n = 1;
                if (Period == "W")
                    while (ndt > dt)
                        dt = StartWhen.Value.AddDays(EveryN.Value * 7 * n++);
                else if (Period == "M")
                    while (ndt > dt)
                        dt = StartWhen.Value.AddMonths(EveryN.Value * n++);
                return dt;
            }
        }
        public int DoGiving(CMSDataContext Db)
        {
            var gateway = Db.Setting("TransactionGateway", "");
            AuthorizeNet anet = null;
            SagePayments sage = null;
            if (gateway == "AuthorizeNet")
                anet = new AuthorizeNet(Db, testing: false);
            else if (gateway == "Sage")
                sage = new SagePayments(Db, testing: false);
            else
                return 0;

            TransactionResponse ret = null;
            var total = (from a in Db.RecurringAmounts
                         where a.PeopleId == PeopleId
                         where a.ContributionFund.FundStatusId == 1
                         select a.Amt).Sum();

            if (!total.HasValue || total == 0)
                return 0;

            var preferredtype = (from pi in Db.PaymentInfos
                                 where pi.PeopleId == PeopleId
                                 select pi.PreferredGivingType).Single();

            var t = new Transaction
            {
                TransactionDate = DateTime.Now,
                TransactionId = "started",
                First = Person.FirstName,
                MiddleInitial = Person.MiddleName.Truncate(1) ?? "",
                Last = Person.LastName,
                Suffix = Person.SuffixCode,
                Amt = total,
                Description = "Recurring Giving",
                Testing = false,
                TransactionGateway = gateway,
                Financeonly = true
            };
            Db.Transactions.InsertOnSubmit(t);
            Db.SubmitChanges();

            if (gateway == "AuthorizeNet")
                ret = anet.createCustomerProfileTransactionRequest(PeopleId, total ?? 0, "Recurring Giving", t.Id);
            else
                ret = sage.createVaultTransactionRequest(PeopleId, total ?? 0, "Recurring Giving", t.Id, preferredtype);
            t.TransactionPeople.Add(new TransactionPerson { PeopleId = PeopleId, Amt = total });

            t.Message = ret.Message;
            t.AuthCode = ret.AuthCode;
            t.Approved = ret.Approved;
            t.TransactionId = ret.TransactionId;
            var systemEmail = Db.Setting("SystemEmailAddress", "mailer@bvcms.com");

            var contributionemail = (from ex in Person.PeopleExtras
                                     where ex.Field == "ContributionEmail"
                                     select ex.Data).SingleOrDefault();
            if (contributionemail.HasValue())
                contributionemail = contributionemail.Trim();
            if (!Util.ValidEmail(contributionemail))
                contributionemail = Person.FromEmail;
            var gift = Db.Setting("NameForPayment", "gift");
            var church = Db.Setting("NameOfChurch", Db.CmsHost);
            if (ret.Approved)
            {
                var q = from a in Db.RecurringAmounts
                        where a.PeopleId == PeopleId
                        select a;

                foreach (var a in q)
                {
                    if (a.ContributionFund.FundStatusId == 1 && a.Amt > 0)
                        Person.PostUnattendedContribution(Db, a.Amt ?? 0, a.FundId, "Recurring Giving", tranid: t.Id);
                }
                var tot = q.Where(aa => aa.ContributionFund.FundStatusId == 1).Sum(aa => aa.Amt);
                NextDate = FindNextDate(DateTime.Today.AddDays(1));
                Db.SubmitChanges();
                if (tot > 0)
                {
                    Util.SendMsg(systemEmail, Db.CmsHost, Util.TryGetMailAddress(contributionemail),
                                 "Recurring {0} for {1}".Fmt(gift, church),
                                 "Your payment of ${0:N2} was processed this morning.".Fmt(tot),
                                 Util.ToMailAddressList(contributionemail), 0, null);
                }
            }
            else
            {
                Db.SubmitChanges();
                var failedGivingMessage = Db.ContentHtml("FailedGivingMessage", Resources.ManagedGiving_FailedGivingMessage);
                var adminEmail = Db.Setting("AdminMail", systemEmail);
                Util.SendMsg(systemEmail, Db.CmsHost, Util.TryGetMailAddress(contributionemail),
                        "Recurring {0} failed for {1}".Fmt(gift, church),
                        failedGivingMessage.Replace("{first}", Person.PreferredName),
                        Util.ToMailAddressList(contributionemail), 0, null);
                foreach (var p in Db.FinancePeople())
                    Util.SendMsg(systemEmail, Db.CmsHost, Util.TryGetMailAddress(adminEmail),
                        "Recurring Giving Failed on " + Db.CmsHost,
                        "<a href='{0}Transactions/{2}'>message: {1}, tranid:{2}</a>".Fmt(Db.CmsHost, ret.Message, t.Id),
                        Util.ToMailAddressList(p.EmailAddress), 0, null);
            }
            return 1;
        }
        public static int DoAllGiving(CMSDataContext Db)
        {
            var gateway = Db.Setting("TransactionGateway", "");
            int count = 0;
            if (gateway.HasValue())
            {
                var q = from rg in Db.ManagedGivings
                        where rg.NextDate < DateTime.Today
                        select rg;
                foreach (var rg in q)
                    rg.NextDate = rg.FindNextDate(DateTime.Today);

                var rgq = from rg in Db.ManagedGivings
                          where rg.NextDate == DateTime.Today
                          select new
                          {
                              rg,
                              rg.Person,
                              rg.Person.RecurringAmounts,
                          };
                foreach (var i in rgq)
                    count += i.rg.DoGiving(Db);
            }
            return count;
        }
    }
}