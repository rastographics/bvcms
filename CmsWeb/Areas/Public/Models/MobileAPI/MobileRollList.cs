using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobileRollList
	{
		public int meetingID = 0;

		public string headcountEnabled = "true";
		public int headcount = 0;

		public List<MobileAttendee> attendees;
	}
}