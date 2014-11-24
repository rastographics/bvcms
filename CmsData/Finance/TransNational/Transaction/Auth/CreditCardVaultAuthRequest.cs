namespace CmsData.Finance.TransNational.Transaction.Auth
{
    internal class CreditCardVaultAuthRequest : TransactRequest
    {
        public CreditCardVaultAuthRequest(string userName, string password, string vaultId, decimal amount) 
            : base(userName, password)
        {
            Data["type"] = "auth";
            Data["payment"] = "creditcard";
            Data["customer_vault_id"] = vaultId;
            Data["amount"] = amount.ToString("0.00");
        }

        public CreditCardVaultAuthRequest(string userName, string password, string vaultId, decimal amount, string orderId)
            : this(userName, password, vaultId, amount)
        {
            Data["orderid"] = orderId;
        }

        public CreditCardVaultAuthRequest(string userName, string password, string vaultId, decimal amount, string orderId, string orderDescription)
            : this(userName, password, vaultId, amount, orderId)
        {
            Data["orderdescription"] = orderDescription;
        }

        public CreditCardVaultAuthRequest(string userName, string password, string vaultId, decimal amount, string orderId, string orderDescription, string poNumber)
            : this(userName, password, vaultId, amount, orderId, orderDescription)
        {
            Data["ponumber"] = poNumber;
        }
    }
}
