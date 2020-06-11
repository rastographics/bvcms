using System;
using System.Linq.Dynamic;
using CmsData;
using CmsData.Finance;
using CmsWeb.Areas.Giving.Models;
using SharedTestFixtures;
using Xunit;

namespace CmsData.Finance.Tests
{
    [Collection("Gateway Tests")]
    public class GatewayTests : DatabaseTestBase
    {
        [Fact]
        public void AuthCreditCardCreatePaymentMethod()
        {
            //(int ? paymentTypeId = null, bool isDefault = true, string name = "", string firstName = "", string lastName = "", string bankAccount = "", string bankRouting = "",
            //string cardNumber = "", string cvv = "", string expiresMonth = null, string expiresYear = null, string address = "", string address2 = "", string city = "",
            //string state = "", string country = "", string zip = "", string phone = "", string transactionTypeId = "", string emailAddress = "")

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
                //testing = true
            };

            //var paymentMethod = db.PaymentMethods.Where(pm => pm.)

            //MethodsCreate(1,true,"My Bank","Jason", "Rice","123456789", "111000614","","","","","","","","","","","","authOnlyTransaction");
            //MethodsCreate(2, true, "My Visa", "Jason", "Rice","","", "4111111111111111", "999", "05", "2021","33", "my address 2", "Dallas", "Texas", "United States", "99997-0008", "2149123704", "authOnlyTransaction");

            //TODO: Finish this test
            //int peopleId = 1;
            //decimal amt = 0;        
            //string cardnumber = "4111111111111111";
            //string expires = "0220";
            //string description = "desc";
            //int tranid = 123;
            //string cardcode = "999";
            //string email = "dahnabez@gmail.com";
            //string first = "Oskar";
            //string last = "Baez";
            //string addr = "addr";
            //string addr2 = "addr2";
            //string city = "mycity";
            //string state = "mystate";
            //string country = "United States";
            //string zip = "03510";
            //string phone = "5547946830";

            //var db = CMSDataContext.Create(DatabaseFixture.Host);
            //var gateway = db.Gateway(true);

            //var response = gateway.PayWithCreditCard(peopleId, amt, cardnumber, expires, description, tranid, cardcode, email, first, last, addr, addr2, city, state, country, zip, phone);
            //bool actual = response.Approved;
            //bool expected = true;
            //Assert.Equal(expected, actual);
        }
    }
}
