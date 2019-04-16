using CmsData.Finance.Acceptiva.Core;

namespace CmsData.Finance.Acceptiva.Charge
{
    internal class AchCharge : ChargeRequest
    {
        private const int paymentTypeAch = 2;

        public AchCharge(string apiKey, string ach_id, Ach ach,
            Payer payer, decimal amt, string tranId, string tranDesc, string peopleId)
            : base(apiKey, ach_id, paymentTypeAch, amt, tranId, tranId, peopleId, payer)
        {
            Data["params[0][ach_acct_num]"] = ach.AchAccNum;
            Data["params[0][ach_routing_num]"] = ach.AchRoutingNum;            
        }
    }
}
