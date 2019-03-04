using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CmsWeb.Areas.Public.Models.MobileAPIv2;

namespace CmsWeb.MobileAPI
{
	[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
	[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
	public class MobileRollList
	{
		public List<MobileAttendee> attendees;
		public List<MobileMeetingCategory> categories;

		public string categoriesEnabled = "false";
		public string category = "";
		public int headcount = 0;

		public string headcountEnabled = "true";
		public int meetingID = 0;
	}
}