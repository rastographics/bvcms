namespace TransactionGateway.ApiModels
{
    public class Fund : BaseResponse
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public bool taxDeductible { get; set; }
    }
}
