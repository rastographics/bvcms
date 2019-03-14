using CmsData;

namespace TransactionGateway
{
    public interface IRGateway
    {
        Transaction CreateTransaction(string paymentToken);
    }
}
