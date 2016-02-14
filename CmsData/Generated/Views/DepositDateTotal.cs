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
	[Table(Name="DepositDateTotals")]
	public partial class DepositDateTotal
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private DateTime? _DepositDate;
		
		private decimal? _TotalHeader;
		
		private decimal? _TotalContributions;
		
		private int? _Count;
		
		
		public DepositDateTotal()
		{
		}

		
		
		[Column(Name="DepositDate", Storage="_DepositDate", DbType="datetime")]
		public DateTime? DepositDate
		{
			get
			{
				return this._DepositDate;
			}

			set
			{
				if (this._DepositDate != value)
					this._DepositDate = value;
			}

		}

		
		[Column(Name="TotalHeader", Storage="_TotalHeader", DbType="Decimal(38,2)")]
		public decimal? TotalHeader
		{
			get
			{
				return this._TotalHeader;
			}

			set
			{
				if (this._TotalHeader != value)
					this._TotalHeader = value;
			}

		}

		
		[Column(Name="TotalContributions", Storage="_TotalContributions", DbType="Decimal(38,2)")]
		public decimal? TotalContributions
		{
			get
			{
				return this._TotalContributions;
			}

			set
			{
				if (this._TotalContributions != value)
					this._TotalContributions = value;
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

		
    }

}
