
using CmsData.Finance.TransNational.Native.Core;

namespace CmsData.Finance.TransNational.Native.Vault
{
    internal class CreateCreditCardVaultRequest : VaultRequest
    {
        public CreateCreditCardVaultRequest(string userName, string password, CreditCard creditCard) 
            : base(userName, password)
        {
            Data["customer_vault"] = "add_customer";
            Data["method"] = "creditcard";
            creditCard.SetCreditCardData(Data);
        }
    }
}
