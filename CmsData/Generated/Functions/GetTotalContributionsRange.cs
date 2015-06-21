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
	[Table(Name="GetTotalContributionsRange")]
	public partial class GetTotalContributionsRange
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private decimal? _Total;
		
		private int? _Count;
		
		private int _Range;
		
		
		public GetTotalContributionsRange()
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

		
		[Column(Name="Count", Storage="_Count", DbType="int")]
		public int? Count
		{
			get
			{
				return this._Count;
			}

			set
			{
				if (this._Count != value)
					this._Count = value;
			}

		}

		
		[Column(Name="RANGE", Storage="_Range", DbType="int NOT NULL")]
		public int Range
		{
			get
			{
				return this._Range;
			}

			set
			{
				if (this._Range != value)
					this._Range = value;
			}

		}

		
    }

}
