using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobilePostCreate
	{
		public string first = "";
		public string last = "";
		public string email = "";
		public string phone = "";
		public string dob = "";

		public void lowerEmail()
		{
			email = email.ToLower();
		}
	}
}