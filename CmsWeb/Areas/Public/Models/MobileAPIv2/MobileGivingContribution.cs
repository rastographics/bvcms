using System;
using CmsData.Codes;

namespace CmsWeb.Areas.Public.Models.MobileAPIv2
{
	public class MobileGivingContribution
	{
		public int id = 0;
		public int typeID = 0;

		public decimal amount = 0;

		public DateTime date = DateTime.Today;

		public string checkNumber = "";

		public int fundID = 0;
		public string fundName = "";
		public bool fundTaxDeductible = true;
		public bool fundPledge = false;

		public int getType()
		{
			if( fundTaxDeductible == false ) return ContributionTypeCode.NonTaxDed;
			if( fundPledge ) return ContributionTypeCode.Pledge;

			return typeID;
		}
	}
}