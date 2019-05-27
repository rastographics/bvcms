namespace CmsData.Finance.Acceptiva.Core.Helpers
{
    internal enum TransactionStatus
    {
        DataValidatedNoProcessing = 0,
        DataValidatedProcessing = 11,
        Processing = 12,
        Authtorized = 13,
        MarkedForVoid = 41,
        VoidProcessing = 42,
        Voided = 43,
        CCSettled = 52,
        eCheckSettled = 61,
        eCheckReturned = 63,
        MarkedForRefund = 71,
        RefundProcessing = 72,
        Refunded = 73,
        TokenSaved = 81
    }
}
