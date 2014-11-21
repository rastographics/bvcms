using UtilityExtensions;

namespace CmsData
{
    public partial class PaymentInfo
    {
        public void SetBillingAddress(string firstName, string middleName, string lastName, string suffix, string address, string city, string state, string zip, string phone)
        {
            FirstName = firstName.Truncate(50);
            MiddleInitial = middleName.Truncate(10);
            LastName = lastName.Truncate(50);
            Suffix = suffix.Truncate(10);
            Address = address.Truncate(50);
            City = city.Truncate(50);
            State = state.Truncate(10);
            Zip = zip.Truncate(15);
            Phone = phone.Truncate(25);
        }
    }
}