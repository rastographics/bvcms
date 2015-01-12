using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobileFloor
	{
		public int id = 0;
		public string name = "";
		public string image = "";

		public List<MobileRoom> rooms;
	}
}