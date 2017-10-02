using System.Collections.Generic;

namespace CmsWeb.MobileAPI
{
	public class MobileSettings
	{
		public int peopleID = 0;
		public int userID = 0;
		public string userName = "";

		public int campusID = 0;
		public string campusName = "";

		public List<string> roles = new List<string>();
	}
}