using System.Collections.Generic;

namespace TransactionGateway.ApiModels
{
    public class PaymentList : BaseResponse
    {
        public List<Payment> Items { get; set; }
    }
}
