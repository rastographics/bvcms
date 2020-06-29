using SharedTestFixtures;
using Shouldly;
using System.Linq;
using Xunit;
using CMSWebTests.Support;
using CmsWeb.Lifecycle;
using CmsWeb.Membership;
using CmsWeb.Areas.Giving.Models;

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
            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.Giving.Controllers.GivingPaymentController(requestManager);

            //var givingPaymentModel = new GivingPaymentModel(db);

            var paymentProcessActionTaken = MockPaymentProcess.PaymentProcessNullCheck(db);

            //givingPaymentModel.CreateMethod(viewModel);
            controller.MethodsCreate(viewModel);

            var paymentMethod = (from pm in db.PaymentMethods
                                 where pm.PeopleId == person.PeopleId
                                 select pm).FirstOrDefault();
            paymentMethod.Decrypt();
            paymentMethod.NameOnAccount.ShouldBe("Jason Rice");

            if (paymentProcessActionTaken == "changed")
            {
                MockPaymentProcess.ChangePaymentProcessToNull(db);
            }
        }

        [Fact]
        public void AuthBankCreatePaymentMethod()
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

            //var givingPaymentModel = new GivingPaymentModel(db);

            var paymentProcessActionTaken = MockPaymentProcess.PaymentProcessNullCheck(db);

            controller.MethodsCreate(viewModel);
            //givingPaymentModel.CreateMethod(viewModel);

            var paymentMethod = (from pm in db.PaymentMethods
                                 where pm.PeopleId == person.PeopleId
                                 select pm).FirstOrDefault();
            paymentMethod.Decrypt();
            paymentMethod.NameOnAccount.ShouldBe("Jason Rice");

            if (paymentProcessActionTaken == "changed")
            {
                MockPaymentProcess.ChangePaymentProcessToNull(db);
            }
        }

        private IRequestManager SetupRequestManager()
        {
            var username = RandomString();
            var password = RandomString();
            var user = CreateUser(username, password);
            var requestManager = FakeRequestManager.Create();
            var membershipProvider = new MockCMSMembershipProvider { ValidUser = true };
            var roleProvider = new MockCMSRoleProvider();
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);
            requestManager.CurrentHttpContext.Request.Headers["Authorization"] = BasicAuthenticationString(username, password);
            return requestManager;
        }

        public override void Dispose()
        {
            base.Dispose();
            MockAppSettings.Remove("PublicKey", "PublicSalt");
        }
    }
}
