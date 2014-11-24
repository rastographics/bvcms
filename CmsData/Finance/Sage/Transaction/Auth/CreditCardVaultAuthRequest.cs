using System;
using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Transaction.Auth
{
    internal class CreditCardVaultAuthRequest : TransactionProcessingRequest
    {
        private const string BASE_ADDRESS = "https://gateway.sagepayments.net/web_services/wsVault/wsVaultBankcard.asmx/";

        public CreditCardVaultAuthRequest(string id, string key, Guid vaultGuid, string nameOnCard, BillingAddress billingAddress, decimal amount)
            : base(BASE_ADDRESS, "VAULT_BANKCARD_AUTHONLY", id, key)
        {
            Data["GUID"] = vaultGuid.ToString().Replace("-", "");
            Data["T_CUSTOMER_NUMBER"] = string.Empty;
            Data["T_AMT"] = amount.ToString("n2");
            Data["C_NAME"] = nameOnCard;
            billingAddress.SetBillingAddressData(Data);
        }

        public CreditCardVaultAuthRequest(string id, string key, Guid vaultGuid, string nameOnCard, BillingAddress billingAddress, decimal amount, string orderNumber)
            : this(id, key, vaultGuid, nameOnCard, billingAddress, amount)
        {
            Data["T_ORDERNUM"] = orderNumber;
        }

        public CreditCardVaultAuthRequest(string id, string key, Guid vaultGuid, string nameOnCard, BillingAddress billingAddress, decimal amount, string orderNumber, string customerNumber)
            : this(id, key, vaultGuid, nameOnCard, billingAddress, amount, orderNumber)
        {
            Data["T_CUSTOMER_NUMBER"] = customerNumber;
        }
    }
}
