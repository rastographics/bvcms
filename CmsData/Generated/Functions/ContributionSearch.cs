using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData.View
{
	[Table(Name="ContributionSearch")]
	public partial class ContributionSearch
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _ContributionId;
		
		
		public ContributionSearch()
		{
		}

		
		
		[Column(Name="ContributionId", Storage="_ContributionId", DbType="int NOT NULL")]
		public int ContributionId
		{
			get
			{
				return this._ContributionId;
			}

			set
			{
				if (this._ContributionId != value)
					this._ContributionId = value;
			}

		}

		
    }

}
