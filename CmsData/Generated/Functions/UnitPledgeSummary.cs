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
	[Table(Name="UnitPledgeSummary")]
	public partial class UnitPledgeSummary
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _FundName;
		
		private decimal? _Given;
		
		private decimal? _Pledged;
		
		
		public UnitPledgeSummary()
		{
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

		
		[Column(Name="Given", Storage="_Given", DbType="Decimal(38,2)")]
		public decimal? Given
		{
			get
			{
				return this._Given;
			}

			set
			{
				if (this._Given != value)
					this._Given = value;
			}

		}

		
		[Column(Name="Pledged", Storage="_Pledged", DbType="Decimal(38,2)")]
		public decimal? Pledged
		{
			get
			{
				return this._Pledged;
			}

			set
			{
				if (this._Pledged != value)
					this._Pledged = value;
			}

		}

		
    }

}
