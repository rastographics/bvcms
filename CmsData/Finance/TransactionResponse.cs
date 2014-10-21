namespace CmsData
{
    public class TransactionResponse
    {
        public bool Approved { get; set; }
        public string Message { get; set; }
        public string AuthCode { get; set; }
        public string TransactionId { get; set; }
    }
}