using System;

namespace CmsData.Finance.Sage.Vault
{
    internal class UpdateAchVaultRequest : VaultRequest
    {
        public UpdateAchVaultRequest(string id, string key, Guid vaultGuid, string accountNumber, string routingNumber)
            : base(id, key, "UPDATE_VIRTUAL_CHECK_DATA")
        {
            Data["GUID"] = vaultGuid.ToString().Replace("-", "");
            Data["ACCOUNT_NUMBER"] = accountNumber;
            Data["ROUTING_NUMBER"] = routingNumber;
            Data["C_ACCT_TYPE"] = "DDA";
        }
    }
}
