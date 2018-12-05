using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.Code;
using UtilityExtensions;
using System.Text;

namespace CmsWeb.Models
{
    public class DepositsModel
    {
        public DateTime Dt1 { get; set; }
        public DepositsModel() { }
        public DepositsModel(DateTime dt1)
        {
            Dt1 = dt1;
        }

        public IEnumerable<DepositInfo> GetDeposits()
        {
            var model = new BundleModel();
            return model.FetchDepositBundles(Dt1);
        }        
    }
}
