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
	[Table(Name="TransactionSummary")]
	public partial class TransactionSummary
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _RegId;
		
		private int _OrganizationId;
		
		private int _PeopleId;
		
		private DateTime? _TranDate;
		
		private decimal? _IndAmt;
		
		private decimal? _TotalAmt;
		
		private decimal? _TotalFee;
		
		private decimal _TotPaid;
		
		private decimal _TotCoupon;
		
		private decimal? _TotDue;
		
		private decimal? _IndPaid;
		
		private decimal? _IndDue;
		
		private double _IndPctC;
		
		private int? _NumPeople;
		
		private bool? _Isdeposit;
		
		private bool? _Iscoupon;
		
		private decimal _Donation;
		
		private bool _IsDonor;
		
		
		public TransactionSummary()
		{
		}

		
		
		[Column(Name="RegId", Storage="_RegId", DbType="int")]
		public int? RegId
		{
			get
			{
				return this._RegId;
			}

			set
			{
				if (this._RegId != value)
					this._RegId = value;
			}

		}

		
		[Column(Name="OrganizationId", Storage="_OrganizationId", DbType="int NOT NULL")]
		public int OrganizationId
		{
			get
			{
				return this._OrganizationId;
			}

			set
			{
				if (this._OrganizationId != value)
					this._OrganizationId = value;
			}

		}

		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int NOT NULL")]
		public int PeopleId
		{
			get
			{
				return this._PeopleId;
			}

			set
			{
				if (this._PeopleId != value)
					this._PeopleId = value;
			}

		}

		
		[Column(Name="TranDate", Storage="_TranDate", DbType="datetime")]
		public DateTime? TranDate
		{
			get
			{
				return this._TranDate;
			}

			set
			{
				if (this._TranDate != value)
					this._TranDate = value;
			}

		}

		
		[Column(Name="IndAmt", Storage="_IndAmt", DbType="money")]
		public decimal? IndAmt
		{
			get
			{
				return this._IndAmt;
			}

			set
			{
				if (this._IndAmt != value)
					this._IndAmt = value;
			}

		}

		
		[Column(Name="TotalAmt", Storage="_TotalAmt", DbType="money")]
		public decimal? TotalAmt
		{
			get
			{
				return this._TotalAmt;
			}

			set
			{
				if (this._TotalAmt != value)
					this._TotalAmt = value;
			}

		}

		
		[Column(Name="TotalFee", Storage="_TotalFee", DbType="money")]
		public decimal? TotalFee
		{
			get
			{
				return this._TotalFee;
			}

			set
			{
				if (this._TotalFee != value)
					this._TotalFee = value;
			}

		}

		
		[Column(Name="TotPaid", Storage="_TotPaid", DbType="money NOT NULL")]
		public decimal TotPaid
		{
			get
			{
				return this._TotPaid;
			}

			set
			{
				if (this._TotPaid != value)
					this._TotPaid = value;
			}

		}

		
		[Column(Name="TotCoupon", Storage="_TotCoupon", DbType="money NOT NULL")]
		public decimal TotCoupon
		{
			get
			{
				return this._TotCoupon;
			}

			set
			{
				if (this._TotCoupon != value)
					this._TotCoupon = value;
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

		
		[Column(Name="IndPaid", Storage="_IndPaid", DbType="money")]
		public decimal? IndPaid
		{
			get
			{
				return this._IndPaid;
			}

			set
			{
				if (this._IndPaid != value)
					this._IndPaid = value;
			}

		}

		
		[Column(Name="IndDue", Storage="_IndDue", DbType="money")]
		public decimal? IndDue
		{
			get
			{
				return this._IndDue;
			}

			set
			{
				if (this._IndDue != value)
					this._IndDue = value;
			}

		}

		
		[Column(Name="IndPctC", Storage="_IndPctC", DbType="float NOT NULL")]
		public double IndPctC
		{
			get
			{
				return this._IndPctC;
			}

			set
			{
				if (this._IndPctC != value)
					this._IndPctC = value;
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

		
		[Column(Name="isdeposit", Storage="_Isdeposit", DbType="bit")]
		public bool? Isdeposit
		{
			get
			{
				return this._Isdeposit;
			}

			set
			{
				if (this._Isdeposit != value)
					this._Isdeposit = value;
			}

		}

		
		[Column(Name="iscoupon", Storage="_Iscoupon", DbType="bit")]
		public bool? Iscoupon
		{
			get
			{
				return this._Iscoupon;
			}

			set
			{
				if (this._Iscoupon != value)
					this._Iscoupon = value;
			}

		}

		
		[Column(Name="Donation", Storage="_Donation", DbType="money NOT NULL")]
		public decimal Donation
		{
			get
			{
				return this._Donation;
			}

			set
			{
				if (this._Donation != value)
					this._Donation = value;
			}

		}

		
		[Column(Name="IsDonor", Storage="_IsDonor", DbType="bit NOT NULL")]
		public bool IsDonor
		{
			get
			{
				return this._IsDonor;
			}

			set
			{
				if (this._IsDonor != value)
					this._IsDonor = value;
			}

		}

		
    }

}
