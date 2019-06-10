using CmsData.Finance.Acceptiva.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Transaction.Settlement
{
    internal class SettleTransaction : AcceptivaRequest
    {
        private const string action = "settle_trans";

        public SettleTransaction(bool isTesting, string apiKey, string transId)
            : base(isTesting, apiKey, action)
        {
            Data["params[0][trans_id_str]"] = transId;
        }

        public new AcceptivaResponse<TransactionResponse> Execute()
        {
            var response = base.Execute();
            var chargeResponse = JsonConvert.DeserializeObject<List<AcceptivaResponse<TransactionResponse>>>(response);
            return chargeResponse[0];
        }
    }
}
