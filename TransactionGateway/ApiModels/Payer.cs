namespace TransactionGateway.ApiModels
{
    public class Payer : BaseResponse
    {
        public string Key { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string emailAddress { get; set; }
        public string mobileNumber { get; set; }
        public string payerType { get; set; }
        public string role { get; set; }
    }
}
