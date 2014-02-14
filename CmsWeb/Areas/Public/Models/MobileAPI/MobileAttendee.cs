using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsWeb.Areas.Reports.Models;

namespace CmsWeb.MobileAPI
{
	public class MobileAttendee
	{
		public int id = 0;

		public string name = "";

		public bool member = false;
		public bool attended = false;

		public MobileAttendee populate(RollsheetModel.AttendInfo p)
		{
			id = p.PeopleId;
			name = p.Name;
			member = p.Member;
			attended = p.Attended;

			return this;
		}
	}
}