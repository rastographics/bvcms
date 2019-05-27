namespace TransactionGateway.ApiModels
{
    public class Card: BaseResponse
    {
        public string Reference { get; set; }
        public string Brand { get; set; }
        public string logo { get; set; }
    }
}
