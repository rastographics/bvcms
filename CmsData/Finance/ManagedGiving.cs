using System;
using System.Linq;
using CmsData.Codes;
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
            var q = (from a in db.RecurringAmounts
                     where a.PeopleId == PeopleId
                     where a.ContributionFund.FundStatusId == 1
                     where a.ContributionFund.OnlineSort != null
                     where a.Amt > 0
                     select new GivingConfirmation.FundItem()
                     {
                         Amt = a.Amt.Value,
                         Desc = a.ContributionFund.FundName,
                         Fundid = a.FundId
                     }).ToList();
            var total = q.Sum(vv => vv.Amt);

            if (total == 0)
                return 0;

            var paymentInfo = db.PaymentInfos.Single(x => x.PeopleId == PeopleId);
            var preferredType = paymentInfo.PreferredGivingType ?? paymentInfo.PreferredPaymentType;

            var gw = GetGateway(db, paymentInfo);

            var orgid = (from o in db.Organizations
                         where o.RegistrationTypeId == RegistrationTypeCode.ManageGiving
                         select o.OrganizationId).FirstOrDefault();

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
                LastFourACH = preferredType == PaymentType.Ach ? paymentInfo.MaskedAccount.Last(4) : null,
                OrgId = orgid,
                LoginPeopleId = Person.PeopleId,
            };

            var vaultid = gw.VaultId(PeopleId);
            if (!vaultid.HasValue())
            {
                t.Message = "Missing VaultId";
                t.Approved = false;
            }
            db.Transactions.InsertOnSubmit(t);
            db.SubmitChanges();
            if (!vaultid.HasValue())
                return 0;

            var ret = gw.PayWithVault(PeopleId, total, "Recurring Giving", t.Id, preferredType);

            t.Message = ret.Message;
            t.AuthCode = ret.AuthCode;
            t.Approved = ret.Approved;
            t.TransactionId = ret.TransactionId;

            var gift = db.Setting("NameForPayment", "gift");
            var church = db.Setting("NameOfChurch", db.CmsHost);
            var notify = db.RecurringGivingNotifyPersons();
            var staff = notify[0];
            var from = Util.TryGetMailAddress(staff.EmailAddress);
            if (ret.Approved)
            {
                NextDate = FindNextDate(Util.Now.Date.AddDays(1));
                db.SubmitChanges();
                var msg = db.Content("RecurringGiftNotice") ?? new Content
                {
                    Title = $"Recurring {gift} for {{church}}",
                    Body = $"Your gift of {total:C} was processed this morning."
                };
                var body = GivingConfirmation.PostAndBuild(db, staff, Person, msg.Body, orgid, q, t, "Recurring Giving");
                var subject = msg.Title.Replace("{church}", church);
                var m = new EmailReplacements(db, body, from);
                body = m.DoReplacements(db, Person);
                db.EmailFinanceInformation(from, Person, null, subject, body);
            }
            else
            {
                t.TransactionPeople.Add(new TransactionPerson
                {
                    PeopleId = Person.PeopleId,
                    Amt = t.Amt,
                    OrgId = orgid,
                });
                db.SubmitChanges();
                var msg = db.Content("RecurringGiftFailedNotice") ?? new Content
                {
                    Title = $"Recurring {gift} for {{church}} did not succeed",
                    Body = @"Your payment of {total} failed to process this morning.<br>
The message was '{message}'.
Please contact the Finance office at the church."
                };
                var subject = msg.Title.Replace("{church}", church);
                var body = msg.Body.Replace("{total}", $"${total:N2}")
                    .Replace("{message}", ret.Message);
                var m = new EmailReplacements(db, body, from);
                body = m.DoReplacements(db, Person);

                db.Email(from, Person, null, subject, body, false);
                foreach (var p in db.RecurringGivingNotifyPersons())
                    db.EmailFinanceInformation(from, p, null,
                        $"Recurring Giving Failed on {db.CmsHost}",
                        $"<a href='{db.CmsHost}/Transactions/{t.Id}'>message: {ret.Message}, tranid:{t.Id}</a>");
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
