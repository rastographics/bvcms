using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobilePostRollList
	{
		public int id = 0;
		public DateTime datetime = DateTime.Now;

		public void changeHourOffset(int offset)
		{
			datetime = datetime.AddHours(offset);
		}
	}
}