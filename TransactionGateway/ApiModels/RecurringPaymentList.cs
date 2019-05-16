using System.Collections.Generic;

namespace TransactionGateway.ApiModels
{
    public class RecurringPaymentList : BaseResponse
    {
        public List<RecurringPayment> Items { get; set; }
    }
}
