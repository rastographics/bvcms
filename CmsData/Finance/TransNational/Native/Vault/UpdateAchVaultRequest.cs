
using CmsData.Finance.TransNational.Native.Core;

namespace CmsData.Finance.TransNational.Native.Vault
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
    }
}
