using System;
using UtilityExtensions;

namespace CmsWeb.CheckInAPI
{
    public class CheckInAddEditPerson
    {
        public bool edit = false;

        public int campus = 0;
        public int id = 0;
        public int familyID = 0;

        public string firstName = "";
        public string goesBy = "";
        public string altName = "";
        public string lastName = "";

        public int genderID = 0;

        public DateTime? birthday;
        public bool birthdaySet = false;
        public bool birthdayClear = false;

        public string father = "";
        public string mother = "";

        public string eMail = "";
        public string cellPhone = "";
        public string homePhone = "";

        public int maritalStatusID = 0;

        public string address = "";
        public string address2 = "";
        public string city = "";
        public string state = "";
        public string zipcode = "";

        public string country = "";

        public string church = "";

        public string allergies = "";

        public string emergencyName = "";
        public string emergencyPhone = "";

        public void clean()
        {
            firstName = firstName.Trim();
            goesBy = goesBy.Trim();
            lastName = lastName.Trim();

            cellPhone = cellPhone.GetDigits();
            homePhone = homePhone.GetDigits();
            eMail = eMail.Trim();

            address = address.Trim();
            address2 = address2.Trim();
            city = city.Trim();
            state = state.Trim();
            zipcode = zipcode.Trim();

            allergies = allergies.Trim();

            emergencyName = emergencyName.Trim();
            emergencyPhone = emergencyPhone.Trim();
        }

        public int getAge()
        {
            if( birthday == null ) return -1;

            DateTime today = DateTime.Now;

            int age = today.Year - birthday.Value.Year;

            if( today.Month < birthday.Value.Month || (today.Month == birthday.Value.Month && today.Day < birthday.Value.Day) )
                age--;

            return age;
        }
    }
}