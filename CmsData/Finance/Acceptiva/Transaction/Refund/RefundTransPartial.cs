using CmsData.Finance.Acceptiva.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Transaction.Refund
{
    internal class RefundTransPartial: AcceptivaRequest
    {
        private const string action = "refund_trans_partial";

        public RefundTransPartial(bool isTesting, string apiKey, string reference, string idString, decimal amount)
            : base(isTesting, apiKey, action)
        {
            Data["params[0][trans_id_str]"] = reference;
            Data["params[0][items][0][id_str]"] = idString;
            Data["params[0][items][0][amt]"] = amount.ToString();
        }

        public new AcceptivaResponse<TransactionResponse> Execute()
        {
            var response = base.Execute();
            var chargeResponse = JsonConvert.DeserializeObject<List<AcceptivaResponse<TransactionResponse>>>(response);
            return chargeResponse[0];
        }
    }
}
