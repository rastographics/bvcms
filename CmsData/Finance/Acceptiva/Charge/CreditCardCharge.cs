using CmsData.Finance.Acceptiva.Core;

namespace CmsData.Finance.Acceptiva.Charge
{
    internal class CreditCardCharge : ChargeRequest
    {
        private const int paymentTypeCC = 1;

        public CreditCardCharge(string apiKey, string cc_id, CreditCard creditCard,
            decimal amt, string tranId, string tranDesc, string peopleId)
            :base(apiKey, paymentTypeCC, amt, tranId, tranId, peopleId)
        {
            string[] expirateDate = creditCard.CardExpiration.Split('/');
            Data["params[0][cc_num]"] = creditCard.CardNum;
            Data["params[0][cc_exp_mo]"] = expirateDate[0];
            Data["params[0][cc_exp_yr]"] = expirateDate[1];
            Data["params[0][cc_cvv]"] = creditCard.CardNum;

        }
    }
}
