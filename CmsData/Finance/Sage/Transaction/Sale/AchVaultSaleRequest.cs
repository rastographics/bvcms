using System;
using CmsData.Finance.Sage.Core;

namespace CmsData.Finance.Sage.Transaction.Sale
{
    internal class AchVaultSaleRequest : TransactionProcessingRequest
    {
        private const string BASE_ADDRESS = "https://gateway.sagepayments.net/web_services/wsVault/wsVaultVirtualCheck.asmx/";

        public AchVaultSaleRequest(string id, string key, string originatorId, Guid vaultGuid, string firstName, string middleInitial, string lastName, string suffix, BillingAddress billingAddress, decimal amount)
            : base(BASE_ADDRESS, "VIRTUAL_CHECK_PPD_SALE", id, key)
        {
            Data["C_ORIGINATOR_ID"] = originatorId;
            Data["GUID"] = vaultGuid.ToString().Replace("-", "");
            Data["C_FIRST_NAME"] = firstName;
            Data["C_MIDDLE_INITIAL"] = middleInitial;
            Data["C_LAST_NAME"] = lastName;
            Data["C_SUFFIX"] = suffix;
            Data["T_AMT"] = amount.ToString("n2");
            billingAddress.SetBillingAddressData(Data);
        }

        public AchVaultSaleRequest(string id, string key, string originatorId, Guid vaultGuid, string firstName, string middleInitial, string lastName, string suffix, BillingAddress billingAddress, decimal amount, string orderNumber)
            : this(id, key, originatorId, vaultGuid, firstName, middleInitial, lastName, suffix, billingAddress, amount)
        {
            Data["T_ORDERNUM"] = orderNumber;
        }
    }
}
