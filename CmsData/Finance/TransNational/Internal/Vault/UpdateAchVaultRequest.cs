
using CmsData.Finance.TransNational.Internal.Core;

namespace CmsData.Finance.TransNational.Internal.Vault
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
