using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobilePostHeadcount
	{
		public int orgID = 0;
		public DateTime datetime = DateTime.Now;

		public int headcount = 0;

		public void changeHourOffset(int offset)
		{
			datetime = datetime.AddHours(offset);
		}
	}
}