using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Core
{
    internal class ChargeRequest : AcceptivaRequest
    {
        private const string URL = "https://sandbox.acceptivapro.com/api/api_request.php";
        private const string action = "charge";

        protected ChargeRequest(string apiKey, int paymentType, decimal amount,
            string orderId, string orderDescription, string peopleId)
            : base(URL, apiKey, action)
        {
            Data["params[0][items][0][id]"] = orderId.ToString();
            Data["params[0][items][0][desc]"] = orderDescription.ToString();
            Data["params[0][items][0][amt]"] = amount.ToString();
            Data["params[0][client_payer_id]"] = peopleId;
        }

        public new AcceptivaResponse<ChargeResponse> Execute()
        {
            var response = base.Execute();
            var chargeResponse = JsonConvert.DeserializeObject<List<AcceptivaResponse<ChargeResponse>>>(response);
            return chargeResponse[0];
        }
    }
}
