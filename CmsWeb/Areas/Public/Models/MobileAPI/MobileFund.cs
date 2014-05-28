using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;

namespace CmsWeb.MobileAPI
{
	public class MobileFund
	{
		public int id;
		public string name;

		public MobileFund populate(ContributionFund cf)
		{
			id = cf.FundId;
			name = cf.FundName;

			return this;
		}
	}
}