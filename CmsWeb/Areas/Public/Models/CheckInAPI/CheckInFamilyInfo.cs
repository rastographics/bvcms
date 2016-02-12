using CmsData;

namespace CmsWeb.Areas.Public.Models.CheckInAPI
{
    public class CheckInFamilyInfo
    {
        public string last = "";

        public string homePhone = "";

        public string address = "";
        public string address2 = "";
        public string city = "";
        public string state = "";
        public string zip = "";
        public string country = "";

        public CheckInFamilyInfo(Family family)
        {
            last = family.HeadOfHousehold.LastName;

            homePhone = family.HomePhone;

            address = family.AddressLineOne;
            address2 = family.AddressLineTwo;
            city = family.CityName;
            state = family.StateCode;
            zip = family.ZipCode;
            country = family.CountryName;
        }
    }
}