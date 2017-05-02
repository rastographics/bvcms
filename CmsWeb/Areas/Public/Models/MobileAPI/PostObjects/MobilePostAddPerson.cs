using System;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsWeb.MobileAPI
{
    public class MobilePostAddPerson
	{
		public int familyID = 0;

		public string firstName = "";
		public string goesBy = "";
		public string lastName = "";
		public string altName = "";

		public int genderID = 0;
		public DateTime? birthday;

		public string eMail = "";
		public string cellPhone = "";
		public string homePhone = "";

		public int maritalStatusID = MaritalStatusCode.Unknown;

		public string address = "";
		public string address2 = "";
		public string city = "";
		public string state = "";
		public string zipcode = "";

		public string country = "";

		public int visitMeeting = 0;

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
		}

        public int getAge()
        {
            if (birthday == null) return -1;

            DateTime today = DateTime.Now;

            int age = today.Year - birthday.Value.Year;

            if (today.Month < birthday.Value.Month || (today.Month == birthday.Value.Month && today.Day < birthday.Value.Day))
                age--;

            return age;
        }
    }
}