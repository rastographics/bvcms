using System;
using System.Linq;
using CmsData.Finance;
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
                var dt1 = new DateTime(ndt.Year, ndt.Month, Day1 ?? 1);
                var dt2 = new DateTime(ndt.Year, ndt.Month,
                    Math.Min(DateTime.DaysInMonth(ndt.Year, ndt.Month), Day2 ?? 15));
                if (ndt <= dt1)
                    return dt1;
                if (ndt <= dt2)
                    return dt2;
                return dt1.AddMonths(1);
            }
            var startwhen = StartWhen ?? DateTime.Today;
            var everywhen = EveryN ?? 1;
            var dt = startwhen;
            var n = 1;
            switch (Period)
            {
                case "W":
                    while (ndt > dt)
                        dt = startwhen.AddDays(everywhen * 7 * n++);
                    break;
                case "M":
                    while (ndt > dt)
                        dt = startwhen.AddMonths(everywhen * n++);
                    break;
            }
            return dt;
        }
        public int DoGiving(CMSDataContext db)
        {
            var total = (from a in db.RecurringAmounts
                         where a.PeopleId == PeopleId
                         where a.ContributionFund.FundStatusId == 1
                         where a.ContributionFund.OnlineSort != null
                         select a.Amt).Sum();

            if (!total.HasValue || total == 0)
                return 0;

            var paymentInfo = db.PaymentInfos.Single(x => x.PeopleId == PeopleId);
            var preferredType = paymentInfo.PreferredGivingType;

            var gw = GetGateway(db, paymentInfo);

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
                Financeonly = true,
                PaymentType = preferredType,
                LastFourCC = preferredType == PaymentType.CreditCard ? paymentInfo.MaskedCard.Last(4) : null,
                LastFourACH = preferredType == PaymentType.Ach ? paymentInfo.MaskedAccount.Last(4) : null
            };

            db.Transactions.InsertOnSubmit(t);
            db.SubmitChanges();

            var ret = gw.PayWithVault(PeopleId, total ?? 0, "Recurring Giving", t.Id, preferredType);

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
            var q = from a in db.RecurringAmounts
                    where a.PeopleId == PeopleId
                    select a;
            var tot = q.Where(aa => aa.ContributionFund.FundStatusId == 1).Sum(aa => aa.Amt);
            t.TransactionPeople.Add(new TransactionPerson
            {
                PeopleId = Person.PeopleId,
                Amt = tot,
            });
            if (ret.Approved)
            {
                foreach (var a in q)
                {
                    if (a.ContributionFund.FundStatusId == 1 && a.ContributionFund.OnlineSort != null && a.Amt > 0)
                        Person.PostUnattendedContribution(db, a.Amt ?? 0, a.FundId, "Recurring Giving", tranid: t.Id);
                }

                NextDate = FindNextDate(Util.Now.Date.AddDays(1));
                db.SubmitChanges();
                if (tot > 0)
                {
                    var msg = db.Content("RecurringGiftNotice") ?? new Content
                              { Title = $"Recurring {gift} for {{church}}",
                                Body = "Your payment of {total} was processed this morning." };
                    var subject = msg.Title.Replace("{church}", church);
                    var body = msg.Body.Replace("{total}", $"${tot:N2}");
                    var from = Util.TryGetMailAddress(contributionemail);
                    var m = new EmailReplacements(db, body, from);
                    body = m.DoReplacements(db, Person);
                    Util.SendMsg(systemEmail, db.CmsHost, from, subject, body,
                                 Util.ToMailAddressList(contributionemail), 0, Person.PeopleId);
                }
            }
            else
            {
                db.SubmitChanges();
                var msg = db.Content("RecurringGiftFailedNotice") ?? new Content
                          { Title = $"Recurring {gift} for {{church}} did not succeed",
                            Body = @"Your payment of {total} failed to process this morning.<br>
The message was '{message}'.
Please contact the Finance office at the church." };
                var subject = msg.Title.Replace("{church}", church);
                var body = msg.Body.Replace("{total}", $"${tot:N2}")
                    .Replace("{message}", ret.Message);
                var from = Util.TryGetMailAddress(contributionemail);
                var m = new EmailReplacements(db, body, from);
                body = m.DoReplacements(db, Person);

                var adminEmail = db.Setting("AdminMail", systemEmail);
                Util.SendMsg(systemEmail, db.CmsHost, from, subject, body,
                        Util.ToMailAddressList(contributionemail), 0, Person.PeopleId);
                foreach (var p in db.RecurringGivingNotifyPersons())
                    Util.SendMsg(systemEmail, db.CmsHost, Util.TryGetMailAddress(adminEmail),
                        "Recurring Giving Failed on " + db.CmsHost,
                        $"<a href='{db.CmsHost}/Transactions/{t.Id}'>message: {ret.Message}, tranid:{t.Id}</a>",
                        Util.ToMailAddressList(p.EmailAddress), 0, Person.PeopleId);
            }
            return 1;
        }

        private IGateway GetGateway(CMSDataContext db, PaymentInfo pi)
        {
            var tempgateway = db.Setting("TemporaryGateway", "");

            if (!tempgateway.HasValue())
                return db.Gateway();

            var gateway = db.Setting("TransactionGateway", "");
            switch (gateway.ToLower()) // Check to see if standard gateway is set up
            {
                case "sage":
                    if ((pi.PreferredGivingType == "B" && pi.SageBankGuid.HasValue) ||
                        (pi.PreferredGivingType == "C" && pi.SageCardGuid.HasValue))
                        return db.Gateway();
                    break;
                case "transnational":
                    if ((pi.PreferredGivingType == "B" && pi.TbnBankVaultId.HasValue) ||
                        (pi.PreferredGivingType == "C" && pi.TbnCardVaultId.HasValue))
                        return db.Gateway();
                    break;
            }

            // fall back to temporary gateway because the user hasn't migrated their payments off of the temporary gateway yet
            return db.Gateway(usegateway: tempgateway);
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
