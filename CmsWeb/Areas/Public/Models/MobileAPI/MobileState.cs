using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobileState
	{
		public string code = "";
		public string name = "";

		public MobileState populate(CmsData.StateLookup state)
		{
			code = state.StateCode;
			name = state.StateName;

			return this;
		}
	}
}