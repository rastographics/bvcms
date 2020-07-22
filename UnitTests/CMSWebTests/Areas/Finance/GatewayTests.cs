using SharedTestFixtures;
using Shouldly;
using System.Linq;
using Xunit;
using CMSWebTests.Support;
using CmsWeb.Areas.Giving.Models;
using System;
using System.Collections.Generic;
using CmsData;
using CmsWeb.Areas.Giving.Controllers;
using SharedTestFixtures.Network;

namespace CMSWebTests.Areas.Finance
{
    [Collection(Collections.Database)]
    public class GatewayTests : ControllerTestBase
    {
        private Person person = new Person();
        private GivingPage newGivingPage = new GivingPage();

        public GatewayTests() : base()
        {
            MockAppSettings.Apply(
                ("PublicKey", "mytest"),
                ("PublicSalt", "66 82 79 78 66 82 79 78"),
                ("sysfromemail", "info@touchpointsoftware.com")
            );
            var contributionFund = MockFunds.CreateContributionFund(db, null);
            var pageName = "Giving Page Test, " + DateTime.Now.ToString();
            newGivingPage = MockGivingPage.CreateGivingPage(db, pageName, contributionFund.FundId, 1);
        }

        [Fact]
        public void TestAuthAndProcessMethods()
        {
            var requestManager = FakeRequestManager.Create();
            var controller = new GivingPaymentController(requestManager);
            MockPaymentProcess.PaymentProcessNullCheck(db);

            person = CreatePerson();
            AuthCreditCardCreatePaymentMethod(controller);
            var paymentMethod1 = (from pm in db.PaymentMethods where pm.PeopleId == person.PeopleId select pm).FirstOrDefault();
            paymentMethod1.Decrypt();
            paymentMethod1.NameOnAccount.ShouldBe(person.Name);

            person = CreatePerson();
            AuthBankCreatePaymentMethod(controller);
            var paymentMethod2 = (from pm in db.PaymentMethods where pm.PeopleId == person.PeopleId select pm).FirstOrDefault();
            paymentMethod2.Decrypt();
            paymentMethod2.NameOnAccount.ShouldBe(person.Name);

            person = CreatePerson();
            ProcessCreditCardPayment(controller);
            decimal totalAmount1 = 60;
            decimal storedAmount1 = 0;
            var contributions1 = (from c in db.Contributions where c.PeopleId == person.PeopleId select c).ToList();
            if (contributions1.Count == 1)
            {
                storedAmount1 = (decimal)contributions1[0].ContributionAmount;
            }
            else
            {
                foreach (var item in contributions1)
                {
                    storedAmount1 += (decimal)item.ContributionAmount;
                }
            }
            storedAmount1.ShouldBe(totalAmount1);

            person = CreatePerson();
            ProcessBankAccountPayment(controller);
            decimal totalAmount2 = 60;
            decimal storedAmount2 = 0;
            var contributions2 = (from c in db.Contributions where c.PeopleId == person.PeopleId select c).ToList();
            if (contributions2.Count == 1)
            {
                storedAmount2 = (decimal)contributions2[0].ContributionAmount;
            }
            else
            {
                foreach (var item in contributions2)
                {
                    storedAmount2 += (decimal)item.ContributionAmount;
                }
            }
            storedAmount2.ShouldBe(totalAmount2);
        }

        private void AuthCreditCardCreatePaymentMethod(GivingPaymentController controller)
        {
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
            CardInfo cardInfo = new CardInfo()
            {
                cardNumber = "4111111111111111",
                cardCode = "999",
                expDateMonth = "05",
                expDateYear = DateTime.Now.AddYears(2).Year.ToString(),
                nameOnCard = person.Name,
                accountName = "My Visa"
            };
            GivingPaymentViewModel viewModel = new GivingPaymentViewModel()
            {
                paymentTypeId = 2,
                isDefault = true,
                transactionTypeId = "authOnlyTransaction",
                incomingPeopleId = person.PeopleId,
                testing = true,
                billingInfo = billingInfo,
                cardInfo = cardInfo
            };
            controller.MethodsCreate(viewModel);
        }

        private void AuthBankCreatePaymentMethod(GivingPaymentController controller)
        {
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
            controller.MethodsCreate(viewModel);
        }

        private void ProcessCreditCardPayment(GivingPaymentController controller)
        {
            SelectedFund selectedFund1 = new SelectedFund()
            {
                fundId = 1,
                fundName = "General Operation"
            };
            SelectedFund selectedFund2 = new SelectedFund()
            {
                fundId = 2,
                fundName = "Pledge"
            };

            Gift gift1 = new Gift()
            {
                amount = 10,
                fund = selectedFund1
            };
            Gift gift2 = new Gift()
            {
                amount = 20,
                fund = selectedFund1
            };
            Gift gift3 = new Gift()
            {
                amount = 30,
                fund = selectedFund2
            };

            var myGifts = new List<Gift>();
            myGifts.Add(gift1);
            myGifts.Add(gift2);
            myGifts.Add(gift3);

            BillingInfo billingInfo = new BillingInfo()
            {
                firstName = person.FirstName,
                lastName = person.LastName,
                email = "abc@myemail.com",
                phone = "2149123704",
                address = "33",
                address2 = "55",
                city = "Dallas",
                state = "Texas",
                zip = "99997-0008",
                country = "United States"
            };
            CardInfo cardInfo = new CardInfo()
            {
                cardNumber = "4111111111111111",
                nameOnCard = person.Name,
                expDateMonth = "05",
                expDateYear = DateTime.Now.AddYears(2).Year.ToString(),
                cardCode = "999"
            };
            GivingPaymentViewModel viewModel = new GivingPaymentViewModel()
            {
                paymentTypeId = 2,
                incomingPeopleId = person.PeopleId,
                testing = true,
                gifts = myGifts,
                billingInfo = billingInfo,
                cardInfo = cardInfo,
                givingPageId = newGivingPage.GivingPageId
            };
            controller.ProcessOneTimeGift(viewModel);
        }

        private void ProcessBankAccountPayment(GivingPaymentController controller)
        {
            SelectedFund selectedFund1 = new SelectedFund()
            {
                fundId = 1,
                fundName = "General Operation"
            };
            SelectedFund selectedFund2 = new SelectedFund()
            {
                fundId = 2,
                fundName = "Pledge"
            };

            Gift gift1 = new Gift()
            {
                amount = 10,
                fund = selectedFund1
            };
            Gift gift2 = new Gift()
            {
                amount = 20,
                fund = selectedFund1
            };
            Gift gift3 = new Gift()
            {
                amount = 30,
                fund = selectedFund2
            };

            var myGifts = new List<Gift>();
            myGifts.Add(gift1);
            myGifts.Add(gift2);
            myGifts.Add(gift3);

            BillingInfo billingInfo = new BillingInfo()
            {
                firstName = person.FirstName,
                lastName = person.LastName,
                email = "abc@myemail.com",
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
                routingNumber = "111000614",
                nameOnAccount = person.Name
            };
            GivingPaymentViewModel viewModel = new GivingPaymentViewModel()
            {
                paymentTypeId = 1,
                incomingPeopleId = person.PeopleId,
                testing = true,
                gifts = myGifts,
                billingInfo = billingInfo,
                bankInfo = bankInfo,
                givingPageId = newGivingPage.GivingPageId
            };
            controller.ProcessOneTimeGift(viewModel);
        }

        public override void Dispose()
        {
            base.Dispose();
            MockAppSettings.Remove("PublicKey", "PublicSalt");
        }
    }
}
