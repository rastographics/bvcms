
using CmsData.Finance.TransNational.Internal.Core;

namespace CmsData.Finance.TransNational.Internal.Vault
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
