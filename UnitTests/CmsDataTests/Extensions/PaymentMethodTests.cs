using Xunit;
using System;
using System.Linq;
using SharedTestFixtures;
using Shouldly;
using CmsData.Codes;

namespace CmsData.Tests
{
    [Collection(Collections.Database)]
    public class PaymentMethodTests : DatabaseTestBase
    {
        public PaymentMethodTests() : base()
        {
            MockAppSettings.Apply(
                ("PublicKey", "mytest"),
                ("PublicSalt", "66 82 79 78 66 82 79 78")
            );
        }

        [Fact]
        public void EncryptDecryptTest()
        {
            var person = CreatePerson();
            var pm1 = new PaymentMethod
            {
                ExpiresMonth = 10,
                ExpiresYear = DateTime.Now.Year + 1,
                GatewayAccountId = 1,
                IsDefault = true,
                Last4 = "1234",
                MaskedDisplay = "•••• •••• •••• 1234",
                Name = "Visa Card",
                NameOnAccount = person.Name,
                PaymentMethodTypeId = PaymentMethodTypeCode.Visa,
                VaultId = $"A{RandomNumber(1000000, 99999999)}",
                CustomerId = $"A{RandomNumber(1000000, 99999999)}"
            };
            person.PaymentMethods.Add(pm1);
            pm1.Encrypt();
            db.SubmitChanges();
            db = db.Copy();

            var pm2 = db.PaymentMethods.Where(p => p.PeopleId == person.PeopleId).First();
            pm2.Decrypt();
            pm1.Last4.ShouldNotBe(pm2.Last4);
            pm1.MaskedDisplay.ShouldNotBe(pm2.MaskedDisplay);
            pm1.NameOnAccount.ShouldNotBe(pm2.NameOnAccount);
            pm1.VaultId.ShouldNotBe(pm2.VaultId);
            pm1.CustomerId.ShouldNotBe(pm2.CustomerId);

            pm1.Decrypt();
            pm1.ExpiresMonth.ShouldBe(pm2.ExpiresMonth);
            pm1.ExpiresYear.ShouldBe(pm2.ExpiresYear);
            pm1.IsDefault.ShouldBe(pm2.IsDefault);
            pm1.Last4.ShouldBe(pm2.Last4);
            pm1.MaskedDisplay.ShouldBe(pm2.MaskedDisplay);
            pm1.Name.ShouldBe(pm2.Name);
            pm1.NameOnAccount.ShouldBe(pm2.NameOnAccount);
            pm1.VaultId.ShouldBe(pm2.VaultId);
            pm1.CustomerId.ShouldBe(pm2.CustomerId);
        }

        public override void Dispose()
        {
            base.Dispose();
            MockAppSettings.Remove("PublicKey", "PublicSalt");
        }
    }
}
