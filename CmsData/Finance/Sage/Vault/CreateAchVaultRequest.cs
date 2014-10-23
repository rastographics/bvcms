
namespace CmsData.Finance.Sage.Vault
{
    internal class CreateAchVaultRequest : VaultRequest
    {
        public CreateAchVaultRequest(string id, string key, string accountNumber, string routingNumber)
            : base(id, key, "INSERT_VIRTUAL_CHECK_DATA")
        {
            Data["ACCOUNT_NUMBER"] = accountNumber;
            Data["ROUTING_NUMBER"] = routingNumber;
            Data["C_ACCT_TYPE"] = "DDA";
        }
    }
}
