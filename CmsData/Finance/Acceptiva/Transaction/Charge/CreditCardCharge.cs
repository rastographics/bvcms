using CmsData.Finance.Acceptiva.Core;

namespace CmsData.Finance.Acceptiva.Transaction.Charge
{
    internal class CreditCardCharge : ChargeRequest
    {
        private const int paymentTypeCC = 1;

        public CreditCardCharge(bool isTesting, string apiKey, string cc_id, CreditCard creditCard,
            Payer payer, decimal amt, string tranId, string tranDesc)
            :base(isTesting, apiKey, cc_id, paymentTypeCC, amt, tranId, tranDesc, payer)
        {
            Data["params[0][cc_num]"] = creditCard.CardNum;
            Data["params[0][cc_exp_mo]"] = creditCard.CardExpiration.Substring(0, 2);
            Data["params[0][cc_exp_yr]"] = creditCard.CardExpiration.Substring(2, 2);
            Data["params[0][cc_cvv]"] = creditCard.CardCvv;
        }
    }
}
