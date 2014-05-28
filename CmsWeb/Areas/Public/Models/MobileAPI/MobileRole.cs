using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;

namespace CmsWeb.MobileAPI
{
	public class MobileRole
	{
		public int id;
		public string name;

		public MobileRole populate( Role role )
		{
			id = role.RoleId;
			name = role.RoleName;

			return this;
		}
	}
}