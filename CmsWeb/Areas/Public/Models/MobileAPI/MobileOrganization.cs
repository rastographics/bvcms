using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobileOrganization
	{
		public int id = 0;

		public string name { get; set; }

		public DateTime datetime { get; set; }

		public MobileOrganization populate(OrganizationInfo oi)
		{
			id = oi.id;
			name = oi.name;

			datetime = createOrgDateTime(oi.time, oi.day);
			return this;
		}

		public DateTime createOrgDateTime(DateTime time, int day)
		{
			return DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).AddDays(day).AddHours(time.Hour).AddMinutes(time.Minute);
		}
	}

	public class OrganizationInfo
	{
		public int id { get; set; }
		public string name { get; set; }
		public DateTime time { get; set; }
		public int day { get; set; }
	}
}