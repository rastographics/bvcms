using CmsData.Finance.Acceptiva.Core;

namespace CmsData.Finance.Acceptiva.Store
{
    internal class StoreNewPayer : StorePayerRequest
    {
        public StoreNewPayer(bool isTesting, string apiKey, string ipAddres, Payer payer, string peopleId, CreditCard creditCard, Ach ach)
            :base(isTesting, apiKey, ipAddres, payer, creditCard, ach)
        {
            Data["params[0][client_payer_id]"] = peopleId;
        }
    }
}
