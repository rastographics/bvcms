using CmsData.Finance.Acceptiva.Core;

namespace CmsData.Finance.Acceptiva.Charge
{
    internal class CreditCardCharge : ChargeRequest
    {
        private const int paymentTypeCC = 1;

        public CreditCardCharge(string apiKey, string cc_id, CreditCard creditCard,
            Payer payer, decimal amt, string tranId, string tranDesc, string peopleId)
            :base(apiKey, cc_id, paymentTypeCC, amt, tranId, tranId, peopleId, payer)
        {
            string expirateMonth = creditCard.CardExpiration.Substring(0,2);
            string expirateYear = creditCard.CardExpiration.Substring(2,2);
            Data["params[0][cc_num]"] = creditCard.CardNum;
            Data["params[0][cc_exp_mo]"] = expirateMonth;
            Data["params[0][cc_exp_yr]"] = expirateYear;
            Data["params[0][cc_cvv]"] = creditCard.CardCvv;
        }
    }
}
