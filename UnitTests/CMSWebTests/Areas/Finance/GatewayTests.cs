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

namespace CMSWebTests.Areas.Finance
{
    [Collection(Collections.Database)]
    public class GatewayTests : ControllerTestBase
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

        //[Fact]
        //public void ProcessCreditCardPayment()
        //{
        //    var person = CreatePerson();
        //    //var fund = 
        //    SelectedFund selectedFund1 = new SelectedFund()
        //    {
        //        fundId = 1,
        //        fundName = "General Operation"
        //    };
        //    SelectedFund selectedFund2 = new SelectedFund()
        //    {
        //        fundId = 2,
        //        fundName = "Pledge"
        //    };
        //    Gift gift1 = new Gift()
        //    {
        //        amount = 1,
        //        fund = selectedFund1
        //    };
        //    Gift gift2 = new Gift()
        //    {
        //        amount = 2,
        //        fund = selectedFund1
        //    };
        //    Gift gift3 = new Gift()
        //    {
        //        amount = 3,
        //        fund = selectedFund2
        //    };
        //    var myGifts = new List<Gift>();
        //    myGifts.Add(gift1);
        //    myGifts.Add(gift2);
        //    myGifts.Add(gift3);
        //    BillingInfo billingInfo = new BillingInfo()
        //    {
        //        firstName = person.FirstName,
        //        lastName = person.LastName,
        //        email = "abc@myemail.com",
        //        phone = "2149123704",
        //        address = "33",
        //        address2 = "55",
        //        city = "Dallas",
        //        state = "Texas",
        //        zip = "99997-0008",
        //        country = "United States"
        //    };
        //    CardInfo cardInfo = new CardInfo()
        //    {
        //        cardNumber = "4111111111111111",
        //        nameOnCard = person.Name,
        //        expDateMonth = "05",
        //        expDateYear = DateTime.Now.AddYears(2).Year.ToString(),
        //        cardCode = "999"
        //    };
        //    GivingPaymentViewModel viewModel = new GivingPaymentViewModel()
        //    {
        //        paymentTypeId = 2,
        //        incomingPeopleId = person.PeopleId,
        //        testing = true,
        //        gifts = myGifts,
        //        billingInfo = billingInfo,
        //        cardInfo = cardInfo
        //    };

        //    var requestManager = FakeRequestManager.Create();
        //    var controller = new CmsWeb.Areas.Giving.Controllers.GivingPaymentController(requestManager);
        //    MockPaymentProcess.PaymentProcessNullCheck(db);
        //    var results = controller.ProcessOneTimeGift(viewModel);

        //}

        [Fact]
        public void AuthBankCreatePaymentMethod()
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

            var paymentMethod = (from pm in db.PaymentMethods
                                 where pm.PeopleId == person.PeopleId
                                 select pm).FirstOrDefault();
            paymentMethod.Decrypt();
            paymentMethod.NameOnAccount.ShouldBe(person.Name);
        }

        public override void Dispose()
        {
            base.Dispose();
            MockAppSettings.Remove("PublicKey", "PublicSalt");
        }
    }
}
