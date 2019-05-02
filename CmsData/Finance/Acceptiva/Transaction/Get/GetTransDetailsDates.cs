using CmsData.Finance.Acceptiva.Core;
using CmsData.Finance.Acceptiva.Transaction.Void;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CmsData.Finance.Acceptiva.Transaction.Get
{
    internal class GetTransDetailsDates: AcceptivaRequest
    {
        private const string action = "get_trans_details";

        public GetTransDetailsDates(string apiKey, DateTime dateStart, DateTime dateEnd)
            : base(apiKey, action)
        {
            Data["params[0][filters][0]"] = $"trans_date>{dateStart.ToString("yyyy-MM-dd")}";
            Data["params[0][filters][1]"] = $"trans_date<{dateEnd.ToString("yyyy-MM-dd")}";
        }

        public new List<AcceptivaResponse<VoidResponse>> Execute()
        {
            var response = base.Execute();
            return JsonConvert.DeserializeObject<List<AcceptivaResponse<VoidResponse>>>(response);
        }
    }
}
