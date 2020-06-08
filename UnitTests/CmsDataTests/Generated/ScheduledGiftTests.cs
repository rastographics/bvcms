using CmsData;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using UtilityExtensions;
using Xunit;
using static CmsData.Codes.ScheduledGiftTypeCode;

namespace CmsDataTests.Generated
{
    [Collection(Collections.Database)]
    public class ScheduledGiftTests : DatabaseTestBase
    {
        [Fact]
        public void ScheduledGiftCreateTest()
        {
            var person = CreatePerson();
            var paymentMethod = new PaymentMethod
            {
                Person = person,
                GatewayAccountId = 1,
                ExpiresMonth = 10,
                ExpiresYear = DateTime.Now.Year + 1,
                Last4 = "1234",
                MaskedDisplay = "•••• •••• •••• 1234",
                Name = "Visa Card",
                NameOnAccount = person.Name,
                PaymentMethodTypeId = PaymentMethodTypeCode.Visa,
                VaultId = RandomString()
            };

            db.SubmitChanges();

            var gift = new ScheduledGift
            {
                PaymentMethodId = paymentMethod.PaymentMethodId,
                ScheduledGiftTypeId = Weekly,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
            };
            gift.ScheduledGiftAmounts.Add(new ScheduledGiftAmount
            {
                FundId = 1,
                Amount = 123.45m,
            });
            person.ScheduledGifts.Add(gift);
            db.SubmitChanges();

            db = db.Copy();
            db.ScheduledGifts.Where(g => g.PeopleId == person.PeopleId).Count().ShouldBe(1);
            db.ScheduledGiftAmounts.Where(g => g.ScheduledGift.PeopleId == person.PeopleId).Count().ShouldBe(1);
        }

        [Theory]
        [InlineData(Weekly, "2019-05-10", "2020-05-10", null, "2020-05-17")] //no end date
        [InlineData(Weekly, "2019-05-31", "2020-05-31", null, "2020-06-07")] //next month
        [InlineData(Weekly, "2019-05-10", "2020-05-10", "2020-05-12", null)] //end before next date
        [InlineData(Weekly, "2019-05-10", "2020-05-10", "2020-05-17", "2020-05-17")] //end on next date
        [InlineData(Weekly, "2019-05-10", "2020-05-10", "2020-12-31", "2020-05-17")] //end after next date

        [InlineData(BiWeekly, "2019-05-10", "2020-05-10", null, "2020-05-24")]
        [InlineData(BiWeekly, "2019-05-30", "2020-05-30", null, "2020-06-13")]
        [InlineData(BiWeekly, "2019-05-10", "2020-05-10", "2020-05-21", null)]
        [InlineData(BiWeekly, "2019-05-10", "2020-05-10", "2020-05-24", "2020-05-24")]
        [InlineData(BiWeekly, "2019-05-10", "2020-05-10", "2020-12-31", "2020-05-24")]

        [InlineData(SemiMonthly, "2019-05-10", "2020-05-10", null, "2020-05-15")]
        [InlineData(SemiMonthly, "2019-05-01", "2020-05-15", null, "2020-06-01")]
        [InlineData(SemiMonthly, "2019-05-15", "2020-05-15", "2020-05-20", null)]
        [InlineData(SemiMonthly, "2019-05-01", "2020-05-01", "2020-05-15", "2020-05-15")]
        [InlineData(SemiMonthly, "2019-05-01", "2020-05-01", "2020-12-31", "2020-05-15")]

        [InlineData(Monthly, "2019-05-10", "2020-05-10", null, "2020-06-10")]
        [InlineData(Monthly, "2019-05-31", "2020-05-31", null, "2020-06-30")]
        [InlineData(Monthly, "2019-05-15", "2020-05-15", "2020-05-30", null)]
        [InlineData(Monthly, "2019-05-01", "2020-05-01", "2020-06-01", "2020-06-01")]
        [InlineData(Monthly, "2019-05-15", "2020-05-15", "2020-12-31", "2020-06-15")]
        [InlineData(Monthly, "2019-01-31", "2020-01-31", null, "2020-02-29")] //leap year

        [InlineData(Quarterly, "2019-05-10", "2020-05-10", null, "2020-08-10")]
        [InlineData(Quarterly, "2019-06-30", "2020-06-30", null, "2020-09-30")]
        [InlineData(Quarterly, "2019-05-15", "2020-05-15", "2020-05-30", null)]
        [InlineData(Quarterly, "2019-05-01", "2020-05-01", "2020-08-01", "2020-08-01")]
        [InlineData(Quarterly, "2019-05-31", "2020-05-31", "2020-12-31", "2020-08-31")]
        [InlineData(Quarterly, "2020-02-29", "2021-02-28", null, "2021-05-29")] //leap year
        [InlineData(Quarterly, "2019-01-31", "2020-01-31", null, "2020-04-30")] //regular quarter

        [InlineData(Annually, "2019-05-10", "2020-05-10", null, "2021-05-10")]
        [InlineData(Annually, "2019-06-30", "2020-06-30", null, "2021-06-30")]
        [InlineData(Annually, "2019-05-15", "2020-05-15", "2020-05-30", null)]
        [InlineData(Annually, "2019-05-01", "2020-05-01", "2021-05-01", "2021-05-01")]
        [InlineData(Annually, "2019-05-31", "2020-05-31", "2021-06-31", "2021-05-31")]
        [InlineData(Annually, "2019-02-28", "2020-02-28", null, "2021-02-28")] //leap year
        [InlineData(Annually, "2020-02-29", "2021-02-28", null, "2022-02-28")] //leap year
        public void ScheduledGiftNextOccurrenceTest(int scheduleType, string startDate, string lastDate, string endDate, string nextDate)
        {
            var schedule = new ScheduledGift
            {
                Day1 = startDate.ToDate().Value.Day,
                ScheduledGiftTypeId = scheduleType,
                StartDate = startDate.ToDate().Value,
                LastProcessed = lastDate.ToDate(),
                EndDate = endDate.ToDate(),
            };

            schedule.Next().ShouldBe(nextDate.ToDate());
        }
    }
}
