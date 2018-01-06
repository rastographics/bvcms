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
	[Table(Name="GiftSummary")]
	public partial class GiftSummary
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private decimal? _Total;
		
		private string _FundName;
		
		
		public GiftSummary()
		{
		}

		
		
		[Column(Name="Total", Storage="_Total", DbType="Decimal(38,2)")]
		public decimal? Total
		{
			get
			{
				return this._Total;
			}

			set
			{
				if (this._Total != value)
					this._Total = value;
			}

		}

		
		[Column(Name="FundName", Storage="_FundName", DbType="nvarchar(256) NOT NULL")]
		public string FundName
		{
			get
			{
				return this._FundName;
			}

			set
			{
				if (this._FundName != value)
					this._FundName = value;
			}

		}

		
    }

}
