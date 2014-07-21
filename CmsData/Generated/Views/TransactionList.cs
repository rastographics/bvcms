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
	[Table(Name="TransactionList")]
	public partial class TransactionList
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _Id;
		
		private DateTime? _TransactionDate;
		
		private string _TransactionGateway;
		
		private int? _DatumId;
		
		private bool? _Testing;
		
		private decimal? _Amt;
		
		private string _ApprovalCode;
		
		private bool? _Approved;
		
		private string _TransactionId;
		
		private string _Message;
		
		private string _AuthCode;
		
		private decimal? _Amtdue;
		
		private string _Url;
		
		private string _Description;
		
		private string _Name;
		
		private string _Address;
		
		private string _City;
		
		private string _State;
		
		private string _Zip;
		
		private string _Phone;
		
		private string _Emails;
		
		private string _Participants;
		
		private int? _OrgId;
		
		private int? _OriginalId;
		
		private decimal? _Regfees;
		
		private decimal? _Donate;
		
		private string _Fund;
		
		private bool? _Financeonly;
		
		private bool? _Voided;
		
		private bool? _Credited;
		
		private bool? _Coupon;
		
		private bool? _Moneytran;
		
		private DateTime? _Settled;
		
		private DateTime? _Batch;
		
		private string _Batchref;
		
		private string _Batchtyp;
		
		private bool? _Fromsage;
		
		private int? _LoginPeopleId;
		
		private string _First;
		
		private string _MiddleInitial;
		
		private string _Last;
		
		private string _Suffix;
		
		private bool? _AdjustFee;
		
		private int _BalancesId;
		
		private decimal? _BegBal;
		
		private decimal? _Payment;
		
		private decimal? _TotDue;
		
		private int? _NumPeople;
		
		private bool? _CanVoid;
		
		private bool? _CanCredit;
		
		
		public TransactionList()
		{
		}

		
		
		[Column(Name="Id", Storage="_Id", DbType="int NOT NULL")]
		public int Id
		{
			get
			{
				return this._Id;
			}

			set
			{
				if (this._Id != value)
					this._Id = value;
			}

		}

		
		[Column(Name="TransactionDate", Storage="_TransactionDate", DbType="datetime")]
		public DateTime? TransactionDate
		{
			get
			{
				return this._TransactionDate;
			}

			set
			{
				if (this._TransactionDate != value)
					this._TransactionDate = value;
			}

		}

		
		[Column(Name="TransactionGateway", Storage="_TransactionGateway", DbType="nvarchar(50)")]
		public string TransactionGateway
		{
			get
			{
				return this._TransactionGateway;
			}

			set
			{
				if (this._TransactionGateway != value)
					this._TransactionGateway = value;
			}

		}

		
		[Column(Name="DatumId", Storage="_DatumId", DbType="int")]
		public int? DatumId
		{
			get
			{
				return this._DatumId;
			}

			set
			{
				if (this._DatumId != value)
					this._DatumId = value;
			}

		}

		
		[Column(Name="testing", Storage="_Testing", DbType="bit")]
		public bool? Testing
		{
			get
			{
				return this._Testing;
			}

			set
			{
				if (this._Testing != value)
					this._Testing = value;
			}

		}

		
		[Column(Name="amt", Storage="_Amt", DbType="money")]
		public decimal? Amt
		{
			get
			{
				return this._Amt;
			}

			set
			{
				if (this._Amt != value)
					this._Amt = value;
			}

		}

		
		[Column(Name="ApprovalCode", Storage="_ApprovalCode", DbType="nvarchar(150)")]
		public string ApprovalCode
		{
			get
			{
				return this._ApprovalCode;
			}

			set
			{
				if (this._ApprovalCode != value)
					this._ApprovalCode = value;
			}

		}

		
		[Column(Name="Approved", Storage="_Approved", DbType="bit")]
		public bool? Approved
		{
			get
			{
				return this._Approved;
			}

			set
			{
				if (this._Approved != value)
					this._Approved = value;
			}

		}

		
		[Column(Name="TransactionId", Storage="_TransactionId", DbType="nvarchar(50)")]
		public string TransactionId
		{
			get
			{
				return this._TransactionId;
			}

			set
			{
				if (this._TransactionId != value)
					this._TransactionId = value;
			}

		}

		
		[Column(Name="Message", Storage="_Message", DbType="nvarchar(150)")]
		public string Message
		{
			get
			{
				return this._Message;
			}

			set
			{
				if (this._Message != value)
					this._Message = value;
			}

		}

		
		[Column(Name="AuthCode", Storage="_AuthCode", DbType="nvarchar(150)")]
		public string AuthCode
		{
			get
			{
				return this._AuthCode;
			}

			set
			{
				if (this._AuthCode != value)
					this._AuthCode = value;
			}

		}

		
		[Column(Name="amtdue", Storage="_Amtdue", DbType="money")]
		public decimal? Amtdue
		{
			get
			{
				return this._Amtdue;
			}

			set
			{
				if (this._Amtdue != value)
					this._Amtdue = value;
			}

		}

		
		[Column(Name="URL", Storage="_Url", DbType="nvarchar(300)")]
		public string Url
		{
			get
			{
				return this._Url;
			}

			set
			{
				if (this._Url != value)
					this._Url = value;
			}

		}

		
		[Column(Name="Description", Storage="_Description", DbType="nvarchar(180)")]
		public string Description
		{
			get
			{
				return this._Description;
			}

			set
			{
				if (this._Description != value)
					this._Description = value;
			}

		}

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(100)")]
		public string Name
		{
			get
			{
				return this._Name;
			}

			set
			{
				if (this._Name != value)
					this._Name = value;
			}

		}

		
		[Column(Name="Address", Storage="_Address", DbType="nvarchar(50)")]
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

		
		[Column(Name="City", Storage="_City", DbType="nvarchar(50)")]
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

		
		[Column(Name="Phone", Storage="_Phone", DbType="nvarchar(20)")]
		public string Phone
		{
			get
			{
				return this._Phone;
			}

			set
			{
				if (this._Phone != value)
					this._Phone = value;
			}

		}

		
		[Column(Name="Emails", Storage="_Emails", DbType="nvarchar")]
		public string Emails
		{
			get
			{
				return this._Emails;
			}

			set
			{
				if (this._Emails != value)
					this._Emails = value;
			}

		}

		
		[Column(Name="Participants", Storage="_Participants", DbType="nvarchar")]
		public string Participants
		{
			get
			{
				return this._Participants;
			}

			set
			{
				if (this._Participants != value)
					this._Participants = value;
			}

		}

		
		[Column(Name="OrgId", Storage="_OrgId", DbType="int")]
		public int? OrgId
		{
			get
			{
				return this._OrgId;
			}

			set
			{
				if (this._OrgId != value)
					this._OrgId = value;
			}

		}

		
		[Column(Name="OriginalId", Storage="_OriginalId", DbType="int")]
		public int? OriginalId
		{
			get
			{
				return this._OriginalId;
			}

			set
			{
				if (this._OriginalId != value)
					this._OriginalId = value;
			}

		}

		
		[Column(Name="regfees", Storage="_Regfees", DbType="money")]
		public decimal? Regfees
		{
			get
			{
				return this._Regfees;
			}

			set
			{
				if (this._Regfees != value)
					this._Regfees = value;
			}

		}

		
		[Column(Name="donate", Storage="_Donate", DbType="money")]
		public decimal? Donate
		{
			get
			{
				return this._Donate;
			}

			set
			{
				if (this._Donate != value)
					this._Donate = value;
			}

		}

		
		[Column(Name="fund", Storage="_Fund", DbType="nvarchar(50)")]
		public string Fund
		{
			get
			{
				return this._Fund;
			}

			set
			{
				if (this._Fund != value)
					this._Fund = value;
			}

		}

		
		[Column(Name="financeonly", Storage="_Financeonly", DbType="bit")]
		public bool? Financeonly
		{
			get
			{
				return this._Financeonly;
			}

			set
			{
				if (this._Financeonly != value)
					this._Financeonly = value;
			}

		}

		
		[Column(Name="voided", Storage="_Voided", DbType="bit")]
		public bool? Voided
		{
			get
			{
				return this._Voided;
			}

			set
			{
				if (this._Voided != value)
					this._Voided = value;
			}

		}

		
		[Column(Name="credited", Storage="_Credited", DbType="bit")]
		public bool? Credited
		{
			get
			{
				return this._Credited;
			}

			set
			{
				if (this._Credited != value)
					this._Credited = value;
			}

		}

		
		[Column(Name="coupon", Storage="_Coupon", DbType="bit")]
		public bool? Coupon
		{
			get
			{
				return this._Coupon;
			}

			set
			{
				if (this._Coupon != value)
					this._Coupon = value;
			}

		}

		
		[Column(Name="moneytran", Storage="_Moneytran", DbType="bit")]
		public bool? Moneytran
		{
			get
			{
				return this._Moneytran;
			}

			set
			{
				if (this._Moneytran != value)
					this._Moneytran = value;
			}

		}

		
		[Column(Name="settled", Storage="_Settled", DbType="datetime")]
		public DateTime? Settled
		{
			get
			{
				return this._Settled;
			}

			set
			{
				if (this._Settled != value)
					this._Settled = value;
			}

		}

		
		[Column(Name="batch", Storage="_Batch", DbType="datetime")]
		public DateTime? Batch
		{
			get
			{
				return this._Batch;
			}

			set
			{
				if (this._Batch != value)
					this._Batch = value;
			}

		}

		
		[Column(Name="batchref", Storage="_Batchref", DbType="nvarchar(50)")]
		public string Batchref
		{
			get
			{
				return this._Batchref;
			}

			set
			{
				if (this._Batchref != value)
					this._Batchref = value;
			}

		}

		
		[Column(Name="batchtyp", Storage="_Batchtyp", DbType="nvarchar(50)")]
		public string Batchtyp
		{
			get
			{
				return this._Batchtyp;
			}

			set
			{
				if (this._Batchtyp != value)
					this._Batchtyp = value;
			}

		}

		
		[Column(Name="fromsage", Storage="_Fromsage", DbType="bit")]
		public bool? Fromsage
		{
			get
			{
				return this._Fromsage;
			}

			set
			{
				if (this._Fromsage != value)
					this._Fromsage = value;
			}

		}

		
		[Column(Name="LoginPeopleId", Storage="_LoginPeopleId", DbType="int")]
		public int? LoginPeopleId
		{
			get
			{
				return this._LoginPeopleId;
			}

			set
			{
				if (this._LoginPeopleId != value)
					this._LoginPeopleId = value;
			}

		}

		
		[Column(Name="First", Storage="_First", DbType="nvarchar(50)")]
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

		
		[Column(Name="MiddleInitial", Storage="_MiddleInitial", DbType="nvarchar(1)")]
		public string MiddleInitial
		{
			get
			{
				return this._MiddleInitial;
			}

			set
			{
				if (this._MiddleInitial != value)
					this._MiddleInitial = value;
			}

		}

		
		[Column(Name="Last", Storage="_Last", DbType="nvarchar(50)")]
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

		
		[Column(Name="Suffix", Storage="_Suffix", DbType="nvarchar(10)")]
		public string Suffix
		{
			get
			{
				return this._Suffix;
			}

			set
			{
				if (this._Suffix != value)
					this._Suffix = value;
			}

		}

		
		[Column(Name="AdjustFee", Storage="_AdjustFee", DbType="bit")]
		public bool? AdjustFee
		{
			get
			{
				return this._AdjustFee;
			}

			set
			{
				if (this._AdjustFee != value)
					this._AdjustFee = value;
			}

		}

		
		[Column(Name="BalancesId", Storage="_BalancesId", DbType="int NOT NULL")]
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
