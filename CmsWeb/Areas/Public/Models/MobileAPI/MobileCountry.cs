using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.MobileAPI
{
	public class MobileCountry
	{
		public int id = 0;

		public string code = "";
		public string description = "";

		public MobileCountry populate(CmsData.Country country)
		{
			id = country.Id;
			code = country.Code;
			description = country.Description;

			return this;
		}
	}
}