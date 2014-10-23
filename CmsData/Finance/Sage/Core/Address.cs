using System.Collections.Specialized;

namespace CmsData.Finance.Sage.Core
{
    internal class Address
    {
        public string Address1 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Country { get; set; }

        internal void SetAddressData(NameValueCollection data)
        {
            data["C_ADDRESS"] = Address1;
            data["C_CITY"] = City;
            data["C_STATE"] = State;
            data["C_ZIP"] = Zip;
            data["C_COUNTRY"] = Country;
        }
    }
}
