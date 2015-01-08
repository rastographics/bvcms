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
	[Table(Name="TransactionBalances")]
	public partial class TransactionBalance
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _BalancesId;
		
		private decimal? _BegBal;
		
		private decimal? _Payment;
		
		private decimal? _TotDue;
		
		private int? _NumPeople;
		
		private string _People;
		
		private bool? _CanVoid;
		
		private bool? _CanCredit;
		
		
		public TransactionBalance()
		{
		}

		
		
		[Column(Name="BalancesId", Storage="_BalancesId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsDbGenerated=true)]
		public int BalancesId
		{
			get
			{
				return this._BalancesId;
			}

			set
			{
				if (this._BalancesId != value)
					this._BalancesId = value;
			}

		}

		
		[Column(Name="BegBal", Storage="_BegBal", DbType="money")]
		public decimal? BegBal
		{
			get
			{
				return this._BegBal;
			}

			set
			{
				if (this._BegBal != value)
					this._BegBal = value;
			}

		}

		
		[Column(Name="Payment", Storage="_Payment", DbType="money")]
		public decimal? Payment
		{
			get
			{
				return this._Payment;
			}

			set
			{
				if (this._Payment != value)
					this._Payment = value;
			}

		}

		
		[Column(Name="TotDue", Storage="_TotDue", DbType="money")]
		public decimal? TotDue
		{
			get
			{
				return this._TotDue;
			}

			set
			{
				if (this._TotDue != value)
					this._TotDue = value;
			}

		}

		
		[Column(Name="NumPeople", Storage="_NumPeople", DbType="int")]
		public int? NumPeople
		{
			get
			{
				return this._NumPeople;
			}

			set
			{
				if (this._NumPeople != value)
					this._NumPeople = value;
			}

		}

		
		[Column(Name="People", Storage="_People", DbType="nvarchar")]
		public string People
		{
			get
			{
				return this._People;
			}

			set
			{
				if (this._People != value)
					this._People = value;
			}

		}

		
		[Column(Name="CanVoid", Storage="_CanVoid", DbType="bit")]
		public bool? CanVoid
		{
			get
			{
				return this._CanVoid;
			}

			set
			{
				if (this._CanVoid != value)
					this._CanVoid = value;
			}

		}

		
		[Column(Name="CanCredit", Storage="_CanCredit", DbType="bit")]
		public bool? CanCredit
		{
			get
			{
				return this._CanCredit;
			}

			set
			{
				if (this._CanCredit != value)
					this._CanCredit = value;
			}

		}

		
    }

}
