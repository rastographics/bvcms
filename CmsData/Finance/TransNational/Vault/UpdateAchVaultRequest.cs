using CmsData.Finance.TransNational.Core;

namespace CmsData.Finance.TransNational.Vault
{
    internal class UpdateAchVaultRequest : VaultRequest
    {
        private UpdateAchVaultRequest(string userName, string password, string vaultId)
            : base(userName, password)
        {
            Data["customer_vault"] = "update_customer";
            Data["customer_vault_id"] = vaultId;
            Data["sec_code"] = "WEB";
            Data["method"] = "check";
        }

        public UpdateAchVaultRequest(string userName, string password, string vaultId, Ach ach) 
            : this(userName, password, vaultId)
        {
            ach.SetAchData(Data);
        }

        public UpdateAchVaultRequest(string userName, string password, string vaultId, string nameOnAccount, BillingAddress billingAddress) 
            : this(userName, password, vaultId)
        {
            Data["checkname"] = nameOnAccount;
            billingAddress.SetBillingAddressData(Data);
        }
    }
}
