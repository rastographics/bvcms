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
	[Table(Name="CurrOrgMembers")]
	public partial class CurrOrgMember
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private string _OrganizationName;
		
		private string _FirstName;
		
		private string _LastName;
		
		private string _Gender;
		
		private int? _Grade;
		
		private string _ShirtSize;
		
		private string _Request;
		
		private decimal _Amount;
		
		private decimal _AmountPaid;
		
		private int _HasBalance;
		
		private string _Groups;
		
		private string _Email;
		
		private string _HomePhone;
		
		private string _CellPhone;
		
		private string _WorkPhone;
		
		private int? _Age;
		
		private DateTime? _BirthDate;
		
		private DateTime? _JoinDate;
		
		private string _MemberStatus;
		
		private string _SchoolOther;
		
		private DateTime? _LastAttend;
		
		private decimal? _AttendPct;
		
		private string _AttendStr;
		
		private string _MemberType;
		
		private string _UserData;
		
		private DateTime? _InactiveDate;
		
		private string _Medical;
		
		private int _PeopleId;
		
		private DateTime? _EnrollDate;
		
		private int? _Tickets;
		
		
		public CurrOrgMember()
		{
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

		
		[Column(Name="OrganizationName", Storage="_OrganizationName", DbType="nvarchar(100) NOT NULL")]
		public string OrganizationName
		{
			get
			{
				return this._OrganizationName;
			}

			set
			{
				if (this._OrganizationName != value)
					this._OrganizationName = value;
			}

		}

		
		[Column(Name="FirstName", Storage="_FirstName", DbType="nvarchar(50)")]
		public string FirstName
		{
			get
			{
				return this._FirstName;
			}

			set
			{
				if (this._FirstName != value)
					this._FirstName = value;
			}

		}

		
		[Column(Name="LastName", Storage="_LastName", DbType="nvarchar(100) NOT NULL")]
		public string LastName
		{
			get
			{
				return this._LastName;
			}

			set
			{
				if (this._LastName != value)
					this._LastName = value;
			}

		}

		
		[Column(Name="Gender", Storage="_Gender", DbType="nvarchar(20)")]
		public string Gender
		{
			get
			{
				return this._Gender;
			}

			set
			{
				if (this._Gender != value)
					this._Gender = value;
			}

		}

		
		[Column(Name="Grade", Storage="_Grade", DbType="int")]
		public int? Grade
		{
			get
			{
				return this._Grade;
			}

			set
			{
				if (this._Grade != value)
					this._Grade = value;
			}

		}

		
		[Column(Name="ShirtSize", Storage="_ShirtSize", DbType="nvarchar(50)")]
		public string ShirtSize
		{
			get
			{
				return this._ShirtSize;
			}

			set
			{
				if (this._ShirtSize != value)
					this._ShirtSize = value;
			}

		}

		
		[Column(Name="Request", Storage="_Request", DbType="nvarchar(140)")]
		public string Request
		{
			get
			{
				return this._Request;
			}

			set
			{
				if (this._Request != value)
					this._Request = value;
			}

		}

		
		[Column(Name="Amount", Storage="_Amount", DbType="money NOT NULL")]
		public decimal Amount
		{
			get
			{
				return this._Amount;
			}

			set
			{
				if (this._Amount != value)
					this._Amount = value;
			}

		}

		
		[Column(Name="AmountPaid", Storage="_AmountPaid", DbType="money NOT NULL")]
		public decimal AmountPaid
		{
			get
			{
				return this._AmountPaid;
			}

			set
			{
				if (this._AmountPaid != value)
					this._AmountPaid = value;
			}

		}

		
		[Column(Name="HasBalance", Storage="_HasBalance", DbType="int NOT NULL")]
		public int HasBalance
		{
			get
			{
				return this._HasBalance;
			}

			set
			{
				if (this._HasBalance != value)
					this._HasBalance = value;
			}

		}

		
		[Column(Name="Groups", Storage="_Groups", DbType="nvarchar")]
		public string Groups
		{
			get
			{
				return this._Groups;
			}

			set
			{
				if (this._Groups != value)
					this._Groups = value;
			}

		}

		
		[Column(Name="Email", Storage="_Email", DbType="nvarchar(150)")]
		public string Email
		{
			get
			{
				return this._Email;
			}

			set
			{
				if (this._Email != value)
					this._Email = value;
			}

		}

		
		[Column(Name="HomePhone", Storage="_HomePhone", DbType="nvarchar(50)")]
		public string HomePhone
		{
			get
			{
				return this._HomePhone;
			}

			set
			{
				if (this._HomePhone != value)
					this._HomePhone = value;
			}

		}

		
		[Column(Name="CellPhone", Storage="_CellPhone", DbType="nvarchar(50)")]
		public string CellPhone
		{
			get
			{
				return this._CellPhone;
			}

			set
			{
				if (this._CellPhone != value)
					this._CellPhone = value;
			}

		}

		
		[Column(Name="WorkPhone", Storage="_WorkPhone", DbType="nvarchar(50)")]
		public string WorkPhone
		{
			get
			{
				return this._WorkPhone;
			}

			set
			{
				if (this._WorkPhone != value)
					this._WorkPhone = value;
			}

		}

		
		[Column(Name="Age", Storage="_Age", DbType="int")]
		public int? Age
		{
			get
			{
				return this._Age;
			}

			set
			{
				if (this._Age != value)
					this._Age = value;
			}

		}

		
		[Column(Name="BirthDate", Storage="_BirthDate", DbType="datetime")]
		public DateTime? BirthDate
		{
			get
			{
				return this._BirthDate;
			}

			set
			{
				if (this._BirthDate != value)
					this._BirthDate = value;
			}

		}

		
		[Column(Name="JoinDate", Storage="_JoinDate", DbType="datetime")]
		public DateTime? JoinDate
		{
			get
			{
				return this._JoinDate;
			}

			set
			{
				if (this._JoinDate != value)
					this._JoinDate = value;
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

		
		[Column(Name="SchoolOther", Storage="_SchoolOther", DbType="nvarchar(200)")]
		public string SchoolOther
		{
			get
			{
				return this._SchoolOther;
			}

			set
			{
				if (this._SchoolOther != value)
					this._SchoolOther = value;
			}

		}

		
		[Column(Name="LastAttend", Storage="_LastAttend", DbType="datetime")]
		public DateTime? LastAttend
		{
			get
			{
				return this._LastAttend;
			}

			set
			{
				if (this._LastAttend != value)
					this._LastAttend = value;
			}

		}

		
		[Column(Name="AttendPct", Storage="_AttendPct", DbType="real")]
		public decimal? AttendPct
		{
			get
			{
				return this._AttendPct;
			}

			set
			{
				if (this._AttendPct != value)
					this._AttendPct = value;
			}

		}

		
		[Column(Name="AttendStr", Storage="_AttendStr", DbType="nvarchar(200)")]
		public string AttendStr
		{
			get
			{
				return this._AttendStr;
			}

			set
			{
				if (this._AttendStr != value)
					this._AttendStr = value;
			}

		}

		
		[Column(Name="MemberType", Storage="_MemberType", DbType="nvarchar(100)")]
		public string MemberType
		{
			get
			{
				return this._MemberType;
			}

			set
			{
				if (this._MemberType != value)
					this._MemberType = value;
			}

		}

		
		[Column(Name="UserData", Storage="_UserData", DbType="nvarchar")]
		public string UserData
		{
			get
			{
				return this._UserData;
			}

			set
			{
				if (this._UserData != value)
					this._UserData = value;
			}

		}

		
		[Column(Name="InactiveDate", Storage="_InactiveDate", DbType="datetime")]
		public DateTime? InactiveDate
		{
			get
			{
				return this._InactiveDate;
			}

			set
			{
				if (this._InactiveDate != value)
					this._InactiveDate = value;
			}

		}

		
		[Column(Name="Medical", Storage="_Medical", DbType="nvarchar(1000)")]
		public string Medical
		{
			get
			{
				return this._Medical;
			}

			set
			{
				if (this._Medical != value)
					this._Medical = value;
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

		
		[Column(Name="EnrollDate", Storage="_EnrollDate", DbType="datetime")]
		public DateTime? EnrollDate
		{
			get
			{
				return this._EnrollDate;
			}

			set
			{
				if (this._EnrollDate != value)
					this._EnrollDate = value;
			}

		}

		
		[Column(Name="Tickets", Storage="_Tickets", DbType="int")]
		public int? Tickets
		{
			get
			{
				return this._Tickets;
			}

			set
			{
				if (this._Tickets != value)
					this._Tickets = value;
			}

		}

		
    }

}
