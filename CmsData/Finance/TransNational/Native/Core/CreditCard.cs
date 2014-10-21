using System.Collections.Specialized;

namespace CmsData.Finance.TransNational.Native.Core
{
    internal class CreditCard
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string CardNumber { get; set; }

        public string CardCode { get; set; }

        public string Expiration { get; set; }

        public BillingAddress BillingAddress { get; set; }

        internal void SetCreditCardData(NameValueCollection data)
        {
            data["firstname"] = FirstName;
            data["lastname"] = LastName;
            data["ccnumber"] = CardNumber;

            if (!string.IsNullOrEmpty(CardCode))
                data["cvv"] = CardCode;
            
            SetCreditCardExpirationData(data, Expiration);
            BillingAddress.SetBillingAddressData(data);
        }

        internal static void SetCreditCardExpirationData(NameValueCollection data, string expiration)
        {
            data["ccexp"] = expiration;
        }
    }
}
