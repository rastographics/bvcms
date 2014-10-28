using System.Collections.Specialized;

namespace CmsData.Finance.Sage.Core
{
    internal class CreditCard
    {
        public string NameOnCard { get; set; }

        public string CardNumber { get; set; }

        public string CardCode { get; set; }

        public string Expiration { get; set; }

        public BillingAddress BillingAddress { get; set; }

        internal void SetCreditCardData(NameValueCollection data)
        {
            data["C_NAME"] = NameOnCard;
            data["C_CARDNUMBER"] = CardNumber;
            data["C_CVV"] = CardCode;
            data["C_EXP"] = Expiration;

            if (BillingAddress != null)
                BillingAddress.SetBillingAddressData(data);
        }
    }
}
