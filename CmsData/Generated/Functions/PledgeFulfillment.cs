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
	[Table(Name="PledgeFulfillment")]
	public partial class PledgeFulfillment
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _First;
		
		private string _Last;
		
		private string _Spouse;
		
		private string _MemberStatus;
		
		private DateTime? _PledgeDate;
		
		private DateTime? _LastDate;
		
		private decimal? _PledgeAmt;
		
		private decimal? _TotalGiven;
		
		private decimal? _Balance;
		
		private string _Address;
		
		private string _City;
		
		private string _State;
		
		private string _Zip;
		
		private int? _CreditGiverId;
		
		private int? _SpouseId;
		
		
		public PledgeFulfillment()
		{
		}

		
		
		[Column(Name="First", Storage="_First", DbType="nvarchar(25)")]
		public string First
		{
			get
			{
				return this._First;
			}

			set
			{
				if (this._First != value)
					this._First = value;
			}

		}

		
		[Column(Name="Last", Storage="_Last", DbType="nvarchar(100) NOT NULL")]
		public string Last
		{
			get
			{
				return this._Last;
			}

			set
			{
				if (this._Last != value)
					this._Last = value;
			}

		}

		
		[Column(Name="Spouse", Storage="_Spouse", DbType="nvarchar(25)")]
		public string Spouse
		{
			get
			{
				return this._Spouse;
			}

			set
			{
				if (this._Spouse != value)
					this._Spouse = value;
			}

		}

		
		[Column(Name="MemberStatus", Storage="_MemberStatus", DbType="nvarchar(50)")]
		public string MemberStatus
		{
			get
			{
				return this._MemberStatus;
			}

			set
			{
				if (this._MemberStatus != value)
					this._MemberStatus = value;
			}

		}

		
		[Column(Name="PledgeDate", Storage="_PledgeDate", DbType="datetime")]
		public DateTime? PledgeDate
		{
			get
			{
				return this._PledgeDate;
			}

			set
			{
				if (this._PledgeDate != value)
					this._PledgeDate = value;
			}

		}

		
		[Column(Name="LastDate", Storage="_LastDate", DbType="datetime")]
		public DateTime? LastDate
		{
			get
			{
				return this._LastDate;
			}

			set
			{
				if (this._LastDate != value)
					this._LastDate = value;
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

		
		[Column(Name="TotalGiven", Storage="_TotalGiven", DbType="Decimal(38,2)")]
		public decimal? TotalGiven
		{
			get
			{
				return this._TotalGiven;
			}

			set
			{
				if (this._TotalGiven != value)
					this._TotalGiven = value;
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

		
		[Column(Name="Address", Storage="_Address", DbType="nvarchar(100)")]
		public string Address
		{
			get
			{
				return this._Address;
			}

			set
			{
				if (this._Address != value)
					this._Address = value;
			}

		}

		
		[Column(Name="City", Storage="_City", DbType="nvarchar(30)")]
		public string City
		{
			get
			{
				return this._City;
			}

			set
			{
				if (this._City != value)
					this._City = value;
			}

		}

		
		[Column(Name="State", Storage="_State", DbType="nvarchar(20)")]
		public string State
		{
			get
			{
				return this._State;
			}

			set
			{
				if (this._State != value)
					this._State = value;
			}

		}

		
		[Column(Name="Zip", Storage="_Zip", DbType="nvarchar(15)")]
		public string Zip
		{
			get
			{
				return this._Zip;
			}

			set
			{
				if (this._Zip != value)
					this._Zip = value;
			}

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

		
    }

}
