using CmsData;
using CmsData.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsWeb.Areas.Giving.Models
{
    public class MigrateGivingModel
    {
        public int Count { get; set; }

        public static bool Migrate(CMSDataContext db, out string error)
        {
            var errors = new StringBuilder();
            var result = true;
            var managedGivings = (from mg in db.ManagedGivings
                                  join ra in db.RecurringAmounts on mg.PeopleId equals ra.PeopleId into Amounts
                                  from last in mg.Person.Contributions
                                    .Where(c => Amounts.Any(a => a.FundId == c.FundId))
                                    .OrderByDescending(c => c.ContributionDate)
                                    .Take(1)
                                  select new ManageGivingConversionModel
                                  {
                                      Person = mg.Person,
                                      PeopleId = mg.PeopleId,
                                      PaymentInfo = mg.Person.PaymentInfos.FirstOrDefault(),
                                      Day1 = mg.Day1,
                                      Day2 = mg.Day2,
                                      EveryN = mg.EveryN,
                                      NextDate = mg.NextDate,
                                      Period = mg.Period,
                                      SemiEvery = mg.SemiEvery,
                                      StartDate = mg.StartWhen ?? DateTime.Now,
                                      LastDate = last.ContributionDate,
                                      Amounts = Amounts
                                  }).ToList();
            foreach (var managedGiving in managedGivings)
            {
                try
                {
                    if (managedGiving.PaymentInfo != null)
                    {
                        var paymentMethod = PaymentInfoToPaymentMethod(managedGiving.PaymentInfo, managedGiving.Person);
                        var scheduledGift = new ScheduledGift
                        {
                            IsEnabled = true,
                            LastProcessed = managedGiving.LastDate,
                            PaymentMethod = paymentMethod,
                            PeopleId = managedGiving.PeopleId,
                            StartDate = managedGiving.StartDate,
                            Day1 = managedGiving.Day1,
                            Day2 = managedGiving.Day2,
                            Interval = managedGiving.EveryN.GetValueOrDefault(1),
                            ScheduledGiftTypeId = GetScheduledGiftTypeId(managedGiving)
                        };
                        scheduledGift.ScheduledGiftAmounts.AddRange(RecurringToScheduledGifts(managedGiving.Amounts));
                        db.SubmitChanges();
                    }
                }
                catch (Exception e)
                {
                    errors.AppendLine($"{e.Message} - {managedGiving.Person.Name} ({managedGiving.PeopleId})");
                }
            }
            if (errors.Length == 0)
            {
                //TODO: Change database settings to use scheduledgiving
                //db.SetSetting("UseGivingSchedules", "true");
            }
            error = errors.ToString();
            return result;
        }

        private static PaymentMethod PaymentInfoToPaymentMethod(PaymentInfo info, Person person)
        {
            PaymentMethod bank = null;
            PaymentMethod card = null;
            if (info.Expires.HasValue()) // Card
            {
                card = CreateCardPaymentMethod(info, person);
            }
            if (info.MaskedAccount.HasValue()) // ACH
            {
                bank = CreateBankPaymentMethod(info, person);
            }
            return new[] { bank, card }.FirstOrDefault(p => p?.IsDefault == true);
        }

        private static PaymentMethod CreateBankPaymentMethod(PaymentInfo info, Person person)
        {
            PaymentMethod bank;
            var last4 = info.MaskedAccount.Split('X').Last();
            bank = new PaymentMethod
            {
                GatewayAccountId = info.GatewayAccountId,
                IsDefault = info.PreferredGivingType == "B",
                MaskedDisplay = info.MaskedAccount?.Replace('X', '•'),
                Name = $"Account ending in {last4}",
                NameOnAccount = $"{info.FirstName} {info.LastName}".Trim(),
                PaymentMethodTypeId = PaymentMethodTypeCode.Bank,
                PeopleId = info.PeopleId,
                //TODO: Add back when CustomerId is implemented
                //CustomerId = info.AuNetCustId,
                VaultId = GetBankVaultId(info)
            };
            bank.Encrypt();
            person.PaymentMethods.Add(bank);
            return bank;
        }

        private static PaymentMethod CreateCardPaymentMethod(PaymentInfo info, Person person)
        {
            PaymentMethod card;
            var last4 = info.MaskedCard.Split(' ').Last();
            var month = info.Expires.GetDigits().Substring(0, 2).ToInt();
            var year = info.Expires.GetDigits().Substring(2, 2).ToInt();
            card = new PaymentMethod
            {
                ExpiresMonth = month,
                ExpiresYear = year,
                GatewayAccountId = info.GatewayAccountId,
                IsDefault = info.PreferredGivingType == "C",
                Last4 = last4,
                MaskedDisplay = info.MaskedCard?.Replace('X', '•'),
                Name = $"Card ending in {last4}",
                NameOnAccount = $"{info.FirstName} {info.LastName}".Trim(),
                PaymentMethodTypeId = PaymentMethodTypeCode.Other,
                PeopleId = info.PeopleId,
                //TODO: Add back when CustomerId is implemented
                //CustomerId = info.AuNetCustId,
                VaultId = GetCardVaultId(info)
            };
            card.Encrypt();
            person.PaymentMethods.Add(card);
            return card;
        }

        private static string GetBankVaultId(PaymentInfo paymentInfo)
        {
            return Util.PickFirst(
                paymentInfo.TbnBankVaultId?.ToString(),
                paymentInfo.AuNetCustPayBankId?.ToString(),
                paymentInfo.SageBankGuid?.ToString(),
                paymentInfo.AcceptivaPayerId
                );
        }

        private static string GetCardVaultId(PaymentInfo paymentInfo)
        {
            return Util.PickFirst(
                paymentInfo.TbnCardVaultId?.ToString(),
                paymentInfo.AuNetCustPayId?.ToString(),
                paymentInfo.SageCardGuid?.ToString(),
                paymentInfo.AcceptivaPayerId,
                paymentInfo.BluePayCardVaultId
                );
        }

        private static IEnumerable<ScheduledGiftAmount> RecurringToScheduledGifts(IEnumerable<RecurringAmount> list)
        {
            foreach(var item in list)
            {
                if (item.Amt.HasValue)
                {
                    yield return new ScheduledGiftAmount
                    {
                        Amount = item.Amt.Value,
                        FundId = item.FundId,
                    };
                }
            }
        }

        private static int GetScheduledGiftTypeId(ManageGivingConversionModel managedGiving)
        {
            var semiEvery = managedGiving.SemiEvery;
            var period = managedGiving.Period;
            var everyN = managedGiving.EveryN;
            var day1 = managedGiving.Day1;
            var day2 = managedGiving.Day2;

            var unrecognized = new Exception($"Unrecognized giving schedule {semiEvery}:{everyN}:{period} ({day1}, {day2})");
            if (!everyN.HasValue || everyN == 0)
            {
                everyN = 1;
            }
            switch (period)
            {
                case "M":
                    if (everyN < 3) return semiEvery == "E" ? ScheduledGiftTypeCode.Monthly : ScheduledGiftTypeCode.SemiMonthly;
                    else if (everyN == 3) return ScheduledGiftTypeCode.Quarterly;
                    else if (everyN == 12) return ScheduledGiftTypeCode.Annually;
                    else throw unrecognized;
                case "W":
                    if (everyN < 3) return semiEvery == "E" || everyN == 1 ? ScheduledGiftTypeCode.Weekly : ScheduledGiftTypeCode.BiWeekly;
                    else if (everyN == 5 || everyN == 4) return ScheduledGiftTypeCode.Monthly;
                    else if (everyN == 12) return ScheduledGiftTypeCode.Quarterly;
                    else if (everyN == 52) return ScheduledGiftTypeCode.Annually;
                    else throw unrecognized;
                default:
                    throw unrecognized;
            }
        }
    }
}
