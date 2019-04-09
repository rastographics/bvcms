using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Core
{
    internal class ChargeRequest : AcceptivaRequest
    {
        private const string URL = "https://sandbox.acceptivapro.com/api/api_request.php";
        private const string action = "charge";

        protected ChargeRequest(string apiKey, string merchAcctId, int paymentType, decimal amount, string orderId,
            string orderDescription, string peopleId, Payer payer)
            : base(URL, apiKey, action)
        {
            Data["params[0][items][0][id]"] = orderId.ToString();
            Data["params[0][items][0][desc]"] = orderDescription.ToString();
            Data["params[0][items][0][amt]"] = amount.ToString();
            Data["params[0][merch_acct_id_str]"] = merchAcctId;
            Data["params[0][client_payer_id]"] = peopleId;
            Data["params[0][client_trans_id]"] = orderId.ToString();
            Data["params[0][payer_email]"] = payer.Email;
            Data["params[0][payer_fname]"] = payer.FirstName;
            Data["params[0][payer_lname]"] = payer.LastName;
            Data["params[0][payer_address]"] = payer.Address;
            Data["params[0][payer_address2]"] = payer.Address2;
            Data["params[0][payer_city]"] = payer.City;
            Data["params[0][payer_state]"] = payer.State;
            Data["params[0][payer_country]"] = payer.Country;
            Data["params[0][payer_zip]"] = payer.Zip;
            Data["params[0][payer_phone]"] = payer.Phone;
        }

        public new AcceptivaResponse<ChargeResponse> Execute()
        {
            var response = base.Execute();
            var chargeResponse = JsonConvert.DeserializeObject<List<AcceptivaResponse<ChargeResponse>>>(response);
            return chargeResponse[0];
        }
    }
}
