using CmsData;

namespace TransactionGateway
{
    public interface IRGateway
    {
        Transaction ConfirmTransaction(Transaction transaction, string paymentToken);
    }
}
