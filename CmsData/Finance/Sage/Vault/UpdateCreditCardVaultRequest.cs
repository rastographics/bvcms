
using System;
using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Vault
{
    internal class UpdateCreditCardVaultRequest : VaultRequest
    {
        private UpdateCreditCardVaultRequest(string id, string key, string operation, Guid vaultGuid, string expiration) 
            : base(id, key, operation)
        {
            Data["GUID"] = vaultGuid.ToString().Replace("-", "");
            Data["EXPIRATION_DATE"] = expiration;
        }

        public UpdateCreditCardVaultRequest(string id, string key, Guid vaultGuid, string expiration)
            : this(id, key, "UPDATE_CREDIT_CARD_EXPIRATION_DATE", vaultGuid, expiration) { }
        

        public UpdateCreditCardVaultRequest(string id, string key, Guid vaultGuid, string expiration, string cardNumber)
            : this(id, key, "UPDATE_CREDIT_CARD_DATA", vaultGuid, expiration)
        {
            Data["CARDNUMBER"] = cardNumber;
        }
    }
}
