
using CmsData.Finance.TransNational.Core;

namespace CmsData.Finance.TransNational.Vault
{
    internal class UpdateAchVaultRequest : VaultRequest
    {
        public UpdateAchVaultRequest(string userName, string password, string vaultId, Ach ach) 
            : base(userName, password)
        {
            Data["customer_vault"] = "update_customer";
            Data["customer_vault_id"] = vaultId;
            Data["method"] = "check";
            ach.SetAchData(Data);
        }

        public UpdateAchVaultRequest(string userName, string password, string vaultId, string nameOnAccount, BillingAddress billingAddress) 
            : base(userName, password)
        {
            Data["customer_vault"] = "update_customer";
            Data["customer_vault_id"] = vaultId;
            Data["method"] = "check";
            Data["checkname"] = nameOnAccount;
            billingAddress.SetBillingAddressData(Data);
        }
    }
}
