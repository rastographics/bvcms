using Shouldly;
using System.Collections.Specialized;
using Xunit;

namespace CmsWeb.Areas.OnlineReg.Models.Tests
{
    public class PaymentFormTests
    {
        [Theory]
        [InlineData("Name", "Joe Smith", "Joe Smith")]
        [InlineData("Account", "1010101010", "••••••1010")]
        [InlineData("CreditCard", "4012888888881881", "••••••••••••1881")]
        [InlineData("Routing", "2222333344", "••••••••••")]
        [InlineData("CVV", "321", "•••")]
        [InlineData("CVV", "4567", "••••")]
        [InlineData("Password", "abcdefgh", "••••••••••")]
        [InlineData("ConfirmPassword", "abcdefgh", "••••••••••")]
        public void RemoveSensitiveInformationTest(string key, string value, string result)
        {
            var form = new NameValueCollection(1);
            form.Add(key, value);

            var coll = PaymentForm.RemoveSensitiveInformation(form);

            coll[key].ShouldBe(result);
        }
    }
}
