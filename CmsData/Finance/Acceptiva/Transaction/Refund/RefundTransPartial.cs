using CmsData.Finance.Acceptiva.Core;
using CmsData.Finance.Acceptiva.Core.Void;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Transaction.Refund
{
    internal class RefundTransPartial: AcceptivaRequest
    {
        private const string action = "refund_trans_partial";

        public RefundTransPartial(string apiKey, string reference, int tranId, decimal amount)
            : base(apiKey, action)
        {
            Data["params[0][trans_id_str]"] = reference;
            Data["params[0][params[0][items][0][id_str]]"] = tranId.ToString();
            Data["params[0][params[0][items][0][amt]]"] = amount.ToString();
        }

        public new AcceptivaResponse<VoidResponse> Execute()
        {
            var response = base.Execute();
            var chargeResponse = JsonConvert.DeserializeObject<List<AcceptivaResponse<VoidResponse>>>(response);
            return chargeResponse[0];
        }
    }
}
