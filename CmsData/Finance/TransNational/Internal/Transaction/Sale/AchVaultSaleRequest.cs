
namespace CmsData.Finance.TransNational.Internal.Transaction.Sale
{
    internal class AchVaultSaleRequest : TransactRequest
    {
        public AchVaultSaleRequest(string userName, string password, string vaultId, decimal amount) 
            : base(userName, password)
        {
            Data["type"] = "sale";
            Data["payment"] = "check";
            Data["customer_vault_id"] = vaultId;
            Data["amount"] = amount.ToString("0.00");
        }

        public AchVaultSaleRequest(string userName, string password, string vaultId, decimal amount, string orderId)
            : this(userName, password, vaultId, amount)
        {
            Data["orderid"] = orderId;
        }

        public AchVaultSaleRequest(string userName, string password, string vaultId, decimal amount, string orderId, string orderDescription)
            : this(userName, password, vaultId, amount, orderId)
        {
            Data["orderdescription"] = orderDescription;
        }
    }
}
