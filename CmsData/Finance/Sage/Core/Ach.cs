using System.Collections.Specialized;

namespace CmsData.Finance.Sage.Core
{
    internal class Ach
    {
        public string FirstName { get; set; }

        public string MiddleInitial { get; set; }

        public string LastName { get; set; }

        public string Suffix { get; set; }

        public string AccountNumber { get; set; }

        public string RoutingNumber { get; set; }

        public BillingAddress BillingAddress { get; set; }

        internal void SetAchData(NameValueCollection data)
        {
            data["C_FIRST_NAME"] = FirstName;
            data["C_MIDDLE_INITIAL"] = MiddleInitial;
            data["C_LAST_NAME"] = LastName;
            data["C_SUFFIX"] = Suffix;
            data["C_ACCT"] = AccountNumber;
            data["C_RTE"] = RoutingNumber;
            data["C_ACCT_TYPE"] = "DDA";

            if (BillingAddress != null)
                BillingAddress.SetBillingAddressData(data);
        }
    }
}
