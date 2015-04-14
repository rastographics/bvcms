using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsCheckin
{
	public class CheckInSettingsEntry
	{
		public int id = 0;
		public string name = "";
		public string settings = "";

		public string Name
		{
			get { return name; }
		}

		public CheckInSettingsEntry()
		{
		}

		public CheckInSettingsEntry(string name, string settings)
		{
			this.name = name;
			this.settings = settings;
		}
	}
}