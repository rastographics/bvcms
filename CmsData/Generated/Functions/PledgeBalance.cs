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
	[Table(Name="PledgeBalances")]
	public partial class PledgeBalance
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _CreditGiverId;
		
		private int? _SpouseId;
		
		private decimal? _PledgeAmt;
		
		private decimal? _GivenAmt;
		
		private decimal? _Balance;
		
		
		public PledgeBalance()
		{
		}

		
		
		[Column(Name="CreditGiverId", Storage="_CreditGiverId", DbType="int")]
		public int? CreditGiverId
		{
			get
			{
				return this._CreditGiverId;
			}

			set
			{
				if (this._CreditGiverId != value)
					this._CreditGiverId = value;
			}

		}

		
		[Column(Name="SpouseId", Storage="_SpouseId", DbType="int")]
		public int? SpouseId
		{
			get
			{
				return this._SpouseId;
			}

			set
			{
				if (this._SpouseId != value)
					this._SpouseId = value;
			}

		}

		
		[Column(Name="PledgeAmt", Storage="_PledgeAmt", DbType="Decimal(38,2)")]
		public decimal? PledgeAmt
		{
			get
			{
				return this._PledgeAmt;
			}

			set
			{
				if (this._PledgeAmt != value)
					this._PledgeAmt = value;
			}

		}

		
		[Column(Name="GivenAmt", Storage="_GivenAmt", DbType="Decimal(38,2)")]
		public decimal? GivenAmt
		{
			get
			{
				return this._GivenAmt;
			}

			set
			{
				if (this._GivenAmt != value)
					this._GivenAmt = value;
			}

		}

		
		[Column(Name="Balance", Storage="_Balance", DbType="Decimal(38,2)")]
		public decimal? Balance
		{
			get
			{
				return this._Balance;
			}

			set
			{
				if (this._Balance != value)
					this._Balance = value;
			}

		}

		
    }

}
