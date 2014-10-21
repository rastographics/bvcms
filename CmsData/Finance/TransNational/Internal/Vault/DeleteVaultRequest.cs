
namespace CmsData.Finance.TransNational.Internal.Vault
{
    internal class DeleteVaultRequest : VaultRequest
    {
        public DeleteVaultRequest(string userName, string password, string vaultId) 
            : base(userName, password)
        {
            Data["customer_vault"] = "delete_customer";
            Data["customer_vault_id"] = vaultId;
        }
    }
}
