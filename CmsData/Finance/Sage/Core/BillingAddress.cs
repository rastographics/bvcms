using System.Collections.Specialized;

namespace CmsData.Finance.Sage.Core
{
    internal class BillingAddress : Address
    {
        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        internal void SetBillingAddressData(NameValueCollection data)
        {
            SetAddressData(data);
            data["C_TELEPHONE"] = Phone;
            data["C_FAX"] = Fax;
            data["C_EMAIL"] = Email;
        }
    }
}
