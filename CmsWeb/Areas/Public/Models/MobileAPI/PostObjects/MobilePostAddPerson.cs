using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobilePostAddPerson
	{
		public int familyID = 0;

		public string firstName = "";
		public string goesBy = "";
		public string lastName = "";

		public DateTime birthday;

		public string mobilePhone = "";
		public string homePhone = "";
		public string eMail = "";
		public string address = "";
		public string zipcode = "";

		public int genderID = 0;
		public int maritalStatusID = 0;
	}
}