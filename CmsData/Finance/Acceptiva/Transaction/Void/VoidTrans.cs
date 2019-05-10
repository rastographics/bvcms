using CmsData.Finance.Acceptiva.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Transaction.Void
{
    internal class VoidTrans: AcceptivaRequest
    {
        private const string action = "void_trans";

        public VoidTrans(string apiKey, string reference)
            : base(apiKey, action)
        {
            Data["params[0][trans_id_str]"] = reference;
        }

        public new AcceptivaResponse<VoidResponse> Execute()
        {
            var response = base.Execute();
            var chargeResponse = JsonConvert.DeserializeObject<List<AcceptivaResponse<VoidResponse>>>(response);
            return chargeResponse[0];
        }
    }
}
