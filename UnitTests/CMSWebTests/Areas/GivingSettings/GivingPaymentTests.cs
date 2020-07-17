using CmsWeb.Areas.Giving.Models;
using CMSWebTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace CMSWebTests.Areas.GivingSettings
{
    [Collection(Collections.Database)]
    public class GivingPaymentTests : ControllerTestBase
    {
        public GivingPaymentTests() : base()
        {
            MockAppSettings.Apply(
                ("PublicKey", "mytest"),
                ("PublicSalt", "66 82 79 78 66 82 79 78")
            );
        }

        [Fact]
        public void CreateGivingPaymentSchedule()
        {
            var person = CreatePerson();
            var emailStarting = RandomString();
            BillingInfo billingInfo = new BillingInfo()
            {
                firstName = person.FirstName,
                lastName = person.LastName,
                email = emailStarting.ToString() + "@myemail.com",
                phone = "2149123704",
                address = "33",
                address2 = "55",
                city = "Dallas",
                state = "Texas",
                zip = "99997-0008",
                country = "United States"
            };
            BankInfo bankInfo = new BankInfo()
            {
                accountName = "My Bank",
                accountNumber = "123456789",
                routingNumber = "111000614"
            };
            GivingPaymentViewModel viewModel = new GivingPaymentViewModel()
            {
                paymentTypeId = 1,
                isDefault = true,
                transactionTypeId = "authOnlyTransaction",
                incomingPeopleId = person.PeopleId,
                testing = true,
                billingInfo = billingInfo,
                bankInfo = bankInfo
            };

            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.Giving.Controllers.GivingPaymentController(requestManager);
            MockPaymentProcess.PaymentProcessNullCheck(db);
            controller.MethodsCreate(viewModel);

            var paymentMethod = (from pm in db.PaymentMethods where pm.PeopleId == person.PeopleId select pm).FirstOrDefault();
            paymentMethod.Decrypt();

            viewModel.paymentMethodId = paymentMethod.PaymentMethodId;
            viewModel.amount = 235;
            viewModel.fundId = (from c in db.ContributionFunds select c).FirstOrDefault().FundId;
            viewModel.scheduleTypeId = 1;
            viewModel.start = DateTime.Now;

            controller.SchedulesCreate(viewModel);
            var scheduledGift = (from s in db.ScheduledGifts where s.PaymentMethodId == paymentMethod.PaymentMethodId select s).FirstOrDefault();
            scheduledGift.PeopleId.ShouldBe(paymentMethod.PeopleId);

            var scheduledGiftAmount = (from sa in db.ScheduledGiftAmounts where sa.ScheduledGiftId == scheduledGift.ScheduledGiftId && sa.FundId == viewModel.fundId select sa).FirstOrDefault();
            scheduledGiftAmount.Amount.ShouldBe((int)viewModel.amount);
        }

        public override void Dispose()
        {
            base.Dispose();
            MockAppSettings.Remove("PublicKey", "PublicSalt");
        }
    }
}
