using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.CheckInAPI
{
	public class CheckInInformation
	{
		public List<CheckInSettingsEntry> settings;
		public List<CheckInCampus> campuses;

		public CheckInInformation(List<CheckInSettingsEntry> settings, List<CheckInCampus> campuses)
		{
			this.settings = settings;
			this.campuses = campuses;
		}
	}
}