using System.Collections.Specialized;

namespace CmsData.Finance.TransNational.Core
{
    internal class BillingAddress : Address
    {
        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        internal void SetBillingAddressData(NameValueCollection data)
        {
            SetAddressData(data);
            
            if (!string.IsNullOrEmpty(Phone))
                data["phone"] = Phone;

            if (!string.IsNullOrEmpty(Fax))
                data["fax"] = Fax;

            data["email"] = Email;
        }
    }
}
