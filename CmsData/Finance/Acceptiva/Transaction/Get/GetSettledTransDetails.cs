using CmsData.Finance.Acceptiva.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Transaction.Get
{
    internal class GetSettledTransDetails: AcceptivaRequest
    {
        private const string action = "get_trans_details";

        public GetSettledTransDetails(bool isTesting, string apiKey, DateTime dateStart, DateTime dateEnd)
            : base(isTesting, apiKey, action)
        {
            Data["params[0][filters][0]"] = $"trans_date>{dateStart.ToString("yyyy-MM-dd")}";
            Data["params[0][filters][1]"] = $"trans_date<{dateEnd.ToString("yyyy-MM-dd")}";
            Data["params[0][filters][1]"] = $"trans_status=52,61,73,74";
        }

        public new List<AcceptivaResponse<TransactionResponse>> Execute()
        {
            var response = base.Execute();
            return JsonConvert.DeserializeObject<List<AcceptivaResponse<TransactionResponse>>>(response);
        }
    }
}
