using System.Collections.Generic;

namespace CmsWeb.MobileAPI
{
	public class MobileGivingHistory
	{
		public int thisYear = 0;
		public int yearToDateTotal = 0;

		public int lastYear = 0;
		public int lastYearTotal = 0;

		public List<MobileGivingEntry> entries = new List<MobileGivingEntry>();

		public void setLastYearTotal( int lastYear, int lastYearTotal )
		{
			this.lastYear = lastYear;
			this.lastYearTotal = lastYearTotal;
		}

		public void updateEntries( int thisYear, List<MobileGivingEntry> entries )
		{
			this.thisYear = thisYear;
			this.entries.AddRange( entries );

			foreach( MobileGivingEntry entry in entries ) {
				yearToDateTotal += entry.amount;
			}
		}
	}
}