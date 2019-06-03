using CmsData.Finance.Acceptiva.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Transaction.Void
{
    internal class VoidTrans: AcceptivaRequest
    {
        private const string action = "void_trans";

        public VoidTrans(bool isTesting, string apiKey, string reference)
            : base(isTesting, apiKey, action)
        {
            Data["params[0][trans_id_str]"] = reference;
        }

        public new AcceptivaResponse<TransactionResponse> Execute()
        {
            var response = base.Execute();
            var chargeResponse = JsonConvert.DeserializeObject<List<AcceptivaResponse<TransactionResponse>>>(response);
            return chargeResponse[0];
        }
    }
}
