
using CmsData.Finance.TransNational.Core;

namespace CmsData.Finance.TransNational.Vault
{
    internal class UpdateCreditCardVaultRequest : VaultRequest
    {
        private UpdateCreditCardVaultRequest(string userName, string password, string vaultId)
            : base(userName, password)
        {
            Data["customer_vault"] = "update_customer";
            Data["customer_vault_id"] = vaultId;
            Data["method"] = "creditcard";
        }

        public UpdateCreditCardVaultRequest(string userName, string password, string vaultId, string expiration, string firstName, string lastName, BillingAddress billingAddress)
            : this(userName, password, vaultId)
        {
            Data["firstname"] = firstName;
            Data["lastname"] = lastName;
            Data["ccexp"] = expiration;
            if (billingAddress != null)
                billingAddress.SetAddressData(Data);
        }

        public UpdateCreditCardVaultRequest(string userName, string password, string vaultId, CreditCard creditCard) 
            : this(userName, password, vaultId)
        {
            creditCard.SetCreditCardData(Data);
        }
    }
}
