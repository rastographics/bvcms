using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobilePostAttend
	{
		public int orgID = 0;
		public DateTime datetime = DateTime.Now;

		public int peopleID = 0;
		public bool present = false;
		
	}
}