using System.Collections.Specialized;

namespace CmsData.Finance.TransNational.Internal.Core
{
    internal class Address
    {
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Country { get; set; }

        internal void SetAddressData(NameValueCollection data)
        {
            data["address1"] = Address1;

            if (!string.IsNullOrEmpty(Address2))
                data["address2"] = Address2;
            
            data["city"] = City;
            data["state"] = State;
            data["zip"] = Zip;
            data["country"] = Country;
        }
    }
}
