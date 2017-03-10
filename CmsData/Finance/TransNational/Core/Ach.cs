using System.Collections.Specialized;

namespace CmsData.Finance.TransNational.Core
{
    internal class Ach
    {
        public string NameOnAccount { get; set; }

        public string AccountNumber { get; set; }

        public string RoutingNumber { get; set; }

        public string Type { get; set; }

        public BillingAddress BillingAddress { get; set; }

        internal void SetAchData(NameValueCollection data)
        {
            data["checkname"] = NameOnAccount;
            data["checkaccount"] = AccountNumber;
            data["checkaba"] = RoutingNumber;
            data["account_holder_type"] = "personal";
            data["account_type"] = Type;

            BillingAddress?.SetBillingAddressData(data);
        }
    }
}
