using CmsData.Finance.Acceptiva.Core;

namespace CmsData.Finance.Acceptiva.Store
{
    internal class StorePayer : StorePayerRequest
    {
        public StorePayer(bool isTesting, string apiKey, string ipAddress, Payer payer, string acceptivaPayerId, CreditCard creditCard, Ach ach)
            : base(isTesting, apiKey, ipAddress, payer, creditCard, ach)
        {
            Data["params[0][payer_id_str]"] = acceptivaPayerId;
        }
    }
}
