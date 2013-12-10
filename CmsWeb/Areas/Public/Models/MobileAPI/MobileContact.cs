using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobileContact
	{
		public int type { get; set; }
		public string label { get; set; }
		public string info { get; set; }

		public MobileContact( int newType, string newLabel, string newInfo )
		{
			type = newType;
			label = newLabel;
			info = newInfo;
		}
	}
}