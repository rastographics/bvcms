using UtilityExtensions;

namespace CmsData
{
    public partial class PaymentInfo
    {
        public void SetBillingAddress(string firstName, string middleName, string lastName, string suffix, string address, string address2, string city, string state, string country, string zip, string phone)
        {
            FirstName = firstName.Truncate(50);
            MiddleInitial = middleName.Truncate(10);
            LastName = lastName.Truncate(50);
            Suffix = suffix.Truncate(10);
            Address = address.Truncate(50);
            Address2 = address2.Truncate(50);
            City = city.Truncate(50);
            State = state.Truncate(10);
            Country = Country.Truncate(50);
            Zip = zip.Truncate(15);
            Phone = phone.Truncate(25);
        }
    }
}