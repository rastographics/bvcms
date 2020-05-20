using CmsData;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using Xunit;

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
                BankName = "",
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
                ScheduledGiftTypeId = ScheduledGiftTypeCode.Weekly,
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
    }
}
