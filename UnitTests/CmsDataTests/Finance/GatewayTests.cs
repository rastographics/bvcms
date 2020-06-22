using System;
using System.Linq;
using System.Linq.Dynamic;
using CmsData;
using CmsData.Finance;
using CmsWeb.Areas.Giving.Models;
using CmsWeb.Areas.Giving.Controllers;
using SharedTestFixtures;
using Xunit;
using Shouldly;

namespace CmsData.Finance.Tests
{
    [Collection(Collections.Database)]
    public class GatewayTests : DatabaseTestBase
    {
        public GatewayTests() : base()
        {
            MockAppSettings.Apply(
                ("PublicKey", "mytest"),
                ("PublicSalt", "66 82 79 78 66 82 79 78")
            );
        }

        [Fact]
        public void AuthCreditCardCreatePaymentMethod()
        {
            var person = CreatePerson();
            // Add/remove reference to CMSWeb in solution explorer to update metadata
            GivingPaymentViewModel viewModel = new GivingPaymentViewModel()
            {
                paymentTypeId = 2,
                isDefault = true,
                name = "My Visa",
                firstName = "Jason",
                lastName = "Rice",
                cardNumber = "4111111111111111",
                cvv = "999",
                expiresMonth = "05",
                expiresYear = "2099",
                address = "33",
                address2 = "55",
                city = "Dallas",
                state = "Texas",
                country = "United States",
                zip = "99997-0008",
                phone = "2149123704",
                transactionTypeId = "authOnlyTransaction",
                incomingPeopleId = person.PeopleId,
                testing = true
            };

            var givingPaymentModel = new GivingPaymentModel(db);

            var paymentProcess = (from p in db.PaymentProcess where p.ProcessName == "Recurring Giving" select p).FirstOrDefault();
            var paymentProcessNull = false;
            if (paymentProcess.GatewayAccountId == null)
            {
                paymentProcess.GatewayAccountId = 2;
                db.SubmitChanges();
                paymentProcessNull = true;
            }

            givingPaymentModel.CreateMethod(viewModel);

            var paymentMethod = (from pm in db.PaymentMethods
                                 where pm.PeopleId == person.PeopleId
                                 select pm).FirstOrDefault();
            paymentMethod.Decrypt();
            paymentMethod.NameOnAccount.ShouldBe("Jason Rice");

            if (paymentProcessNull == true)
            {
                var paymentProcess2 = (from p in db.PaymentProcess where p.ProcessName == "Recurring Giving" select p).FirstOrDefault();
                paymentProcess2.GatewayAccountId = null;
                db.SubmitChanges();
            }
        }

        [Fact]
        public void AuthBankCreatePaymentMethod()
        {
            var person = CreatePerson();
            // Add/remove reference to CMSWeb in solution explorer to update metadata
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

            var givingPaymentModel = new GivingPaymentModel(db);

            var paymentProcess = (from p in db.PaymentProcess where p.ProcessName == "Recurring Giving" select p).FirstOrDefault();
            var paymentProcessNull = false;
            if (paymentProcess.GatewayAccountId == null)
            {
                paymentProcess.GatewayAccountId = 2;
                db.SubmitChanges();
                paymentProcessNull = true;
            }

            givingPaymentModel.CreateMethod(viewModel);

            var paymentMethod = (from pm in db.PaymentMethods
                                 where pm.PeopleId == person.PeopleId
                                 select pm).FirstOrDefault();
            paymentMethod.Decrypt();
            paymentMethod.NameOnAccount.ShouldBe("Jason Rice");
            if(paymentProcessNull == true)
            {
                var paymentProcess2 = (from p in db.PaymentProcess where p.ProcessName == "Recurring Giving" select p).FirstOrDefault();
                paymentProcess2.GatewayAccountId = null;
                db.SubmitChanges();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            MockAppSettings.Remove("PublicKey", "PublicSalt");
        }
    }
}
