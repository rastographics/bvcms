using System.Collections.Generic;

namespace CmsWeb.Areas.Public.Models.CheckInAPIv2
{
	public class AttendanceBundle
	{
		public int labelSize = 0;

		public int securityLabels = 0;
		public bool guestLabels = true;
		public bool locationLabels = true;
		public int nameTagAge = 13;

		public List<Attendance> attendances = new List<Attendance>();
	}
}