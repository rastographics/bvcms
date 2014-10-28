using System;
using System.Linq;
using CmsData.Finance;
using CmsData.Properties;
using UtilityExtensions;

namespace CmsData
{
    public partial class ManagedGiving
    {
        public DateTime FindNextDate(DateTime ndt)
        {
            if (ndt.Date == Util.Now.Date)
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
        public int DoGiving(CMSDataContext db)
        {
            var gw = db.Gateway();

            TransactionResponse ret = null;
            var total = (from a in db.RecurringAmounts
                         where a.PeopleId == PeopleId
                         where a.ContributionFund.FundStatusId == 1
                         select a.Amt).Sum();

            if (!total.HasValue || total == 0)
                return 0;

            var preferredtype = (from pi in db.PaymentInfos
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
                TransactionGateway = gw.GatewayType,
                Financeonly = true
            };
            db.Transactions.InsertOnSubmit(t);
            db.SubmitChanges();

            ret = gw.PayWithVault(PeopleId, total ?? 0, "Recurring Giving", t.Id, preferredtype);

            t.Message = ret.Message;
            t.AuthCode = ret.AuthCode;
            t.Approved = ret.Approved;
            t.TransactionId = ret.TransactionId;
            var systemEmail = db.Setting("SystemEmailAddress", "mailer@bvcms.com");

            var contributionemail = (from ex in Person.PeopleExtras
                                     where ex.Field == "ContributionEmail"
                                     select ex.Data).SingleOrDefault();
            if (contributionemail.HasValue())
                contributionemail = contributionemail.Trim();
            if (!Util.ValidEmail(contributionemail))
                contributionemail = Person.FromEmail;
            var gift = db.Setting("NameForPayment", "gift");
            var church = db.Setting("NameOfChurch", db.CmsHost);
            if (ret.Approved)
            {
                var q = from a in db.RecurringAmounts
                        where a.PeopleId == PeopleId
                        select a;

                foreach (var a in q)
                {
                    if (a.ContributionFund.FundStatusId == 1 && a.Amt > 0)
                        Person.PostUnattendedContribution(db, a.Amt ?? 0, a.FundId, "Recurring Giving", tranid: t.Id);
                }
                var tot = q.Where(aa => aa.ContributionFund.FundStatusId == 1).Sum(aa => aa.Amt);
                t.TransactionPeople.Add(new TransactionPerson
                {
                    PeopleId = Person.PeopleId,
                    Amt = tot,
                });
                NextDate = FindNextDate(Util.Now.Date.AddDays(1));
                db.SubmitChanges();
                if (tot > 0)
                {
                    Util.SendMsg(systemEmail, db.CmsHost, Util.TryGetMailAddress(contributionemail),
                                 "Recurring {0} for {1}".Fmt(gift, church),
                                 "Your payment of ${0:N2} was processed this morning.".Fmt(tot),
                                 Util.ToMailAddressList(contributionemail), 0, null);
                }
            }
            else
            {
                db.SubmitChanges();
                var failedGivingMessage = db.ContentHtml("FailedGivingMessage", Resources.ManagedGiving_FailedGivingMessage);
                var adminEmail = db.Setting("AdminMail", systemEmail);
                Util.SendMsg(systemEmail, db.CmsHost, Util.TryGetMailAddress(contributionemail),
                        "Recurring {0} failed for {1}".Fmt(gift, church),
                        failedGivingMessage.Replace("{first}", Person.PreferredName),
                        Util.ToMailAddressList(contributionemail), 0, null);
                foreach (var p in db.FinancePeople())
                    Util.SendMsg(systemEmail, db.CmsHost, Util.TryGetMailAddress(adminEmail),
                        "Recurring Giving Failed on " + db.CmsHost,
                        "<a href='{0}Transactions/{2}'>message: {1}, tranid:{2}</a>".Fmt(db.CmsHost, ret.Message, t.Id),
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
                        where rg.NextDate < Util.Now.Date
                        //where rg.PeopleId == 819918
                        select rg;
                foreach (var rg in q)
                    rg.NextDate = rg.FindNextDate(Util.Now.Date);

                var rgq = from rg in Db.ManagedGivings
                          where rg.NextDate == Util.Now.Date
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