using CmsWeb.Areas.Giving.Models;
using CMSWebTests.Support;
using FluentAssertions;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            GivingPaymentViewModel viewModel = new GivingPaymentViewModel()
            {
                paymentTypeId = 1,
                isDefault = true,
                name = "My Bank",
                firstName = "Jason",
                lastName = "Rice",
                bankAccount = "123456789",
                bankRouting = "111000614",
                transactionTypeId = "authOnlyTransaction",
                incomingPeopleId = person.PeopleId,
                testing = true
            };

            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.Giving.Controllers.GivingPaymentController(requestManager);
            var paymentProcessActionTaken = MockPaymentProcess.PaymentProcessNullCheck(db);
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

            if (paymentProcessActionTaken == "changed")
                MockPaymentProcess.ChangePaymentProcessToNull(db);
        }

        public override void Dispose()
        {
            base.Dispose();
            MockAppSettings.Remove("PublicKey", "PublicSalt");
        }
    }
}
