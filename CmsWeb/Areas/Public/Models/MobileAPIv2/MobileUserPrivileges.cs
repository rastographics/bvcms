using System.Collections.Generic;

namespace CmsWeb.Areas.Public.Models.MobileAPIv2
{
	public class MobileUserPrivileges
	{
		public List<string> roles = new List<string>();
		public List<string> flags = new List<string>();
	}
}