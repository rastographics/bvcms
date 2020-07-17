using SharedTestFixtures;
using Shouldly;
using System.Linq;
using Xunit;
using CMSWebTests.Support;
using CmsWeb.Lifecycle;
using CmsWeb.Membership;
using CmsWeb.Areas.Giving.Models;
using System;
using System.Collections.Generic;
using CmsData;

namespace CMSWebTests.Areas.Finance
{
    [Collection(Collections.Database)]
    public class GatewayTests : ControllerTestBase
    {
        private Person person = new Person();

        public GatewayTests() : base()
        {
            MockAppSettings.Apply(
                ("PublicKey", "mytest"),
                ("PublicSalt", "66 82 79 78 66 82 79 78")
            );
            person = CreatePerson();
        }

        [Fact]
        public void AuthCreditCardCreatePaymentMethod()
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

            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.Giving.Controllers.GivingPaymentController(requestManager);
            MockPaymentProcess.PaymentProcessNullCheck(db);
            controller.MethodsCreate(viewModel);

            var paymentMethod = (from pm in db.PaymentMethods
                                 where pm.PeopleId == person.PeopleId
                                 select pm).FirstOrDefault();
            paymentMethod.Decrypt();
            paymentMethod.NameOnAccount.ShouldBe(person.Name);
        }

        [Fact]
        public void ProcessCreditCardPayment()
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
                givingPageId = 1 // need to create method to create new giving page
            };

            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.Giving.Controllers.GivingPaymentController(requestManager);
            MockPaymentProcess.PaymentProcessNullCheck(db);
            var results = controller.ProcessOneTimeGift(viewModel);

            decimal totalAmount = 0;
            if (myGifts.Count == 1)
            {
                totalAmount = myGifts[0].amount;
            }
            else
            {
                foreach (var item in myGifts)
                {
                    totalAmount += item.amount;
                }
            }

            decimal storedAmount = 0;
            var contributions = (from c in db.Contributions where c.PeopleId == person.PeopleId select c).ToList();
            if (contributions.Count == 1)
            {
                storedAmount = (decimal)contributions[0].ContributionAmount;
            }
            else
            {
                foreach (var item in contributions)
                {
                    storedAmount += (decimal)item.ContributionAmount;
                }
            }

            storedAmount.ShouldBe(totalAmount);
        }

        [Fact]
        public void AuthBankCreatePaymentMethod()
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

            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.Giving.Controllers.GivingPaymentController(requestManager);
            MockPaymentProcess.PaymentProcessNullCheck(db);
            controller.MethodsCreate(viewModel);

            var paymentMethod = (from pm in db.PaymentMethods
                                 where pm.PeopleId == person.PeopleId
                                 select pm).FirstOrDefault();
            paymentMethod.Decrypt();
            paymentMethod.NameOnAccount.ShouldBe(person.Name);
        }

        [Fact]
        public void ProcessBankAccountPayment()
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
                givingPageId = 1 // need to create method to create new giving page
            };

            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.Giving.Controllers.GivingPaymentController(requestManager);
            MockPaymentProcess.PaymentProcessNullCheck(db);
            var results = controller.ProcessOneTimeGift(viewModel);

            decimal totalAmount = 0;
            if (myGifts.Count == 1)
            {
                totalAmount = myGifts[0].amount;
            }
            else
            {
                foreach (var item in myGifts)
                {
                    totalAmount += item.amount;
                }
            }

            decimal storedAmount = 0;
            var contributions = (from c in db.Contributions where c.PeopleId == person.PeopleId select c).ToList();
            if (contributions.Count == 1)
            {
                storedAmount = (decimal)contributions[0].ContributionAmount;
            }
            else
            {
                foreach (var item in contributions)
                {
                    storedAmount += (decimal)item.ContributionAmount;
                }
            }

            storedAmount.ShouldBe(totalAmount);
        }

        public override void Dispose()
        {
            base.Dispose();
            MockAppSettings.Remove("PublicKey", "PublicSalt");
        }
    }
}
