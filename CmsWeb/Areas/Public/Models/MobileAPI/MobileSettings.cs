using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobileSettings
	{
		public int peopleID;
		public int userID;

		public int givingEnabled = 1;
		public int givingAllowCC = 0;
		public int givingOrgID = 0;

		public List<MobileRole> roles;
	}
}