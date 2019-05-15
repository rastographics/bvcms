namespace TransactionGateway.ApiModels
{
    public class Money : BaseResponse
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
