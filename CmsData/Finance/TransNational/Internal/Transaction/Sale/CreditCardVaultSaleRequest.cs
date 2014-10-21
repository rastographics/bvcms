namespace CmsData.Finance.TransNational.Internal.Transaction.Sale
{
    internal class CreditCardVaultSaleRequest : TransactRequest
    {
        public CreditCardVaultSaleRequest(string userName, string password, string vaultId, decimal amount) 
            : base(userName, password)
        {
            Data["type"] = "sale";
            Data["payment"] = "creditcard";
            Data["customer_vault_id"] = vaultId;
            Data["amount"] = amount.ToString("0.00");
        }

        public CreditCardVaultSaleRequest(string userName, string password, string vaultId, decimal amount, string orderId)
            : this(userName, password, vaultId, amount)
        {
            Data["orderid"] = orderId;
        }

        public CreditCardVaultSaleRequest(string userName, string password, string vaultId, decimal amount, string orderId, string orderDescription)
            : this(userName, password, vaultId, amount, orderId)
        {
            Data["orderdescription"] = orderDescription;
        }

        public CreditCardVaultSaleRequest(string userName, string password, string vaultId, decimal amount, string orderId, string orderDescription, string poNumber)
            : this(userName, password, vaultId, amount, orderId, orderDescription)
        {
            Data["ponumber"] = poNumber;
        }
    }
}
