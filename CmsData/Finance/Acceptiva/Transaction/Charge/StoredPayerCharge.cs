using CmsData.Finance.Acceptiva.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Transaction.Charge
{
    internal class StoredPayerCharge : AcceptivaRequest
    {
        private const string action = "charge";

        public StoredPayerCharge(string apiKey, string merch_acct_id, string acceptivaPayerId, decimal amt, string tranId, string tranDesc, int paymentType, string lname, string fname)
            :base(apiKey, action)
        {
            Data["params[0][items][0][id]"] = tranId.ToString();
            Data["params[0][items][0][desc]"] = tranDesc.ToString();
            Data["params[0][items][0][amt]"] = amt.ToString();
            Data["params[0][payment_type]"] = paymentType.ToString();
            Data["params[0][merch_acct_id_str]"] = merch_acct_id;
            Data["params[0][payer_id_str]"] = acceptivaPayerId;
            Data["params[0][payer_lname]"] = lname;
            Data["params[0][payer_fname]"] = fname;
            Data["params[0][store_payer_data]"] = "false";
        }

        public new AcceptivaResponse<ChargeResponse> Execute()
        {
            var response = base.Execute();
            var chargeResponse = JsonConvert.DeserializeObject<List<AcceptivaResponse<ChargeResponse>>>(response);
            return chargeResponse[0];
        }
    }
}
