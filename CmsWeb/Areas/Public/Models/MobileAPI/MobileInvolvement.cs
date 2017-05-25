using System;

namespace CmsWeb.MobileAPI
{
	public class MobileInvolvement
	{
		public string name = "";
		public string leader = "";
		public string type = "";

		public string division = "";
		public string program = "";
		public string group = "";

		public DateTime? enrolledDate;

		public int attendancePercent = 0;
	}
}