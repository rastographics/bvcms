using CmsData.Finance.Acceptiva.Core;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Store
{
    internal class StorePayerRequest : AcceptivaRequest
    {
        private const string action = "store_payer_data";

        public StorePayerRequest(string apiKey, Payer payer, CreditCard creditCard, Ach ach)
            : base(apiKey, action)
        {
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
            Data["params[0][cc_num]"] = creditCard.CardNum;
            Data["params[0][cc_exp_mo]"] = creditCard.CardExpiration.Substring(0, 2);
            Data["params[0][cc_exp_yr]"] = creditCard.CardExpiration.Substring(2, 2);
            Data["params[0][ach_acct_num]"] = ach.AchAccNum;
            Data["params[0][ach_routing_num]"] = ach.AchRoutingNum;
        }

        public new AcceptivaResponse<PayerDataResponse> Execute()
        {
            var response = base.Execute();
            var storePayerResponse = JsonConvert.DeserializeObject<List<AcceptivaResponse<PayerDataResponse>>>(response);
            return storePayerResponse[0];
        }
    }
}
