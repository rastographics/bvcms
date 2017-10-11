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
	[Table(Name="ContributionSearch0")]
	public partial class ContributionSearch0
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _ContributionId;
		
		
		public ContributionSearch0()
		{
		}

		
		
		[Column(Name="ContributionId", Storage="_ContributionId", DbType="int")]
		public int? ContributionId
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
