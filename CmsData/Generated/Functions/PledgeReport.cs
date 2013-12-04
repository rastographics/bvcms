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
	[Table(Name="PledgeReport")]
	public partial class PledgeReport
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _FundId;
		
		private string _FundName;
		
		private decimal? _Plg;
		
		private decimal? _ToPledge;
		
		private decimal? _NotToPledge;
		
		private decimal? _ToFund;
		
		
		public PledgeReport()
		{
		}

		
		
		[Column(Name="FundId", Storage="_FundId", DbType="int NOT NULL")]
		public int FundId
		{
			get
			{
				return this._FundId;
			}

			set
			{
				if (this._FundId != value)
					this._FundId = value;
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

		
		[Column(Name="Plg", Storage="_Plg", DbType="Decimal(38,2)")]
		public decimal? Plg
		{
			get
			{
				return this._Plg;
			}

			set
			{
				if (this._Plg != value)
					this._Plg = value;
			}

		}

		
		[Column(Name="ToPledge", Storage="_ToPledge", DbType="Decimal(38,2)")]
		public decimal? ToPledge
		{
			get
			{
				return this._ToPledge;
			}

			set
			{
				if (this._ToPledge != value)
					this._ToPledge = value;
			}

		}

		
		[Column(Name="NotToPledge", Storage="_NotToPledge", DbType="Decimal(38,2)")]
		public decimal? NotToPledge
		{
			get
			{
				return this._NotToPledge;
			}

			set
			{
				if (this._NotToPledge != value)
					this._NotToPledge = value;
			}

		}

		
		[Column(Name="ToFund", Storage="_ToFund", DbType="Decimal(38,2)")]
		public decimal? ToFund
		{
			get
			{
				return this._ToFund;
			}

			set
			{
				if (this._ToFund != value)
					this._ToFund = value;
			}

		}

		
    }

}
