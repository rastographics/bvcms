
using CmsData.Finance.TransNational.Internal.Core;

namespace CmsData.Finance.TransNational.Internal.Vault
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

        public UpdateCreditCardVaultRequest(string userName, string password, string vaultId, string expiration)
            : this(userName, password, vaultId)
        {
            CreditCard.SetCreditCardExpirationData(Data, expiration);
        }

        public UpdateCreditCardVaultRequest(string userName, string password, string vaultId, CreditCard creditCard) 
            : this(userName, password, vaultId)
        {
            creditCard.SetCreditCardData(Data);
        }
    }
}
