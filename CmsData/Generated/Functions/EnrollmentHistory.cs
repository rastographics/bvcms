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
	[Table(Name="EnrollmentHistory")]
	public partial class EnrollmentHistory
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _TransactionId;
		
		private bool _TransactionStatus;
		
		private int? _CreatedBy;
		
		private DateTime? _CreatedDate;
		
		private DateTime _TransactionDate;
		
		private int _TransactionTypeId;
		
		private int _OrganizationId;
		
		private string _OrganizationName;
		
		private int _PeopleId;
		
		private int _MemberTypeId;
		
		private DateTime? _EnrollmentDate;
		
		private decimal? _AttendancePercentage;
		
		private DateTime? _NextTranChangeDate;
		
		private int? _EnrollmentTransactionId;
		
		private bool? _Pending;
		
		private DateTime? _InactiveDate;
		
		private string _UserData;
		
		private string _Request;
		
		private string _ShirtSize;
		
		private int? _Grade;
		
		private int? _Tickets;
		
		private string _RegisterEmail;
		
		private int? _TranId;
		
		private int _Score;
		
		private string _SmallGroups;
		
		private bool? _SkipInsertTriggerProcessing;
		
		private int? _PrevTranType;
		
		private int _Isgood;
		
		private int _Id;
		
		private string _Code;
		
		private string _Description;
		
		private int? _AttendanceTypeId;
		
		private bool? _Hardwired;
		
		private string _MemberType;
		
		
		public EnrollmentHistory()
		{
		}

		
		
		[Column(Name="TransactionId", Storage="_TransactionId", DbType="int NOT NULL")]
		public int TransactionId
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

		
		[Column(Name="TransactionStatus", Storage="_TransactionStatus", DbType="bit NOT NULL")]
		public bool TransactionStatus
		{
			get
			{
				return this._TransactionStatus;
			}

			set
			{
				if (this._TransactionStatus != value)
					this._TransactionStatus = value;
			}

		}

		
		[Column(Name="CreatedBy", Storage="_CreatedBy", DbType="int")]
		public int? CreatedBy
		{
			get
			{
				return this._CreatedBy;
			}

			set
			{
				if (this._CreatedBy != value)
					this._CreatedBy = value;
			}

		}

		
		[Column(Name="CreatedDate", Storage="_CreatedDate", DbType="datetime")]
		public DateTime? CreatedDate
		{
			get
			{
				return this._CreatedDate;
			}

			set
			{
				if (this._CreatedDate != value)
					this._CreatedDate = value;
			}

		}

		
		[Column(Name="TransactionDate", Storage="_TransactionDate", DbType="datetime NOT NULL")]
		public DateTime TransactionDate
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

		
		[Column(Name="TransactionTypeId", Storage="_TransactionTypeId", DbType="int NOT NULL")]
		public int TransactionTypeId
		{
			get
			{
				return this._TransactionTypeId;
			}

			set
			{
				if (this._TransactionTypeId != value)
					this._TransactionTypeId = value;
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

		
		[Column(Name="MemberTypeId", Storage="_MemberTypeId", DbType="int NOT NULL")]
		public int MemberTypeId
		{
			get
			{
				return this._MemberTypeId;
			}

			set
			{
				if (this._MemberTypeId != value)
					this._MemberTypeId = value;
			}

		}

		
		[Column(Name="EnrollmentDate", Storage="_EnrollmentDate", DbType="datetime")]
		public DateTime? EnrollmentDate
		{
			get
			{
				return this._EnrollmentDate;
			}

			set
			{
				if (this._EnrollmentDate != value)
					this._EnrollmentDate = value;
			}

		}

		
		[Column(Name="AttendancePercentage", Storage="_AttendancePercentage", DbType="real")]
		public decimal? AttendancePercentage
		{
			get
			{
				return this._AttendancePercentage;
			}

			set
			{
				if (this._AttendancePercentage != value)
					this._AttendancePercentage = value;
			}

		}

		
		[Column(Name="NextTranChangeDate", Storage="_NextTranChangeDate", DbType="datetime")]
		public DateTime? NextTranChangeDate
		{
			get
			{
				return this._NextTranChangeDate;
			}

			set
			{
				if (this._NextTranChangeDate != value)
					this._NextTranChangeDate = value;
			}

		}

		
		[Column(Name="EnrollmentTransactionId", Storage="_EnrollmentTransactionId", DbType="int")]
		public int? EnrollmentTransactionId
		{
			get
			{
				return this._EnrollmentTransactionId;
			}

			set
			{
				if (this._EnrollmentTransactionId != value)
					this._EnrollmentTransactionId = value;
			}

		}

		
		[Column(Name="Pending", Storage="_Pending", DbType="bit")]
		public bool? Pending
		{
			get
			{
				return this._Pending;
			}

			set
			{
				if (this._Pending != value)
					this._Pending = value;
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

		
		[Column(Name="RegisterEmail", Storage="_RegisterEmail", DbType="nvarchar(80)")]
		public string RegisterEmail
		{
			get
			{
				return this._RegisterEmail;
			}

			set
			{
				if (this._RegisterEmail != value)
					this._RegisterEmail = value;
			}

		}

		
		[Column(Name="TranId", Storage="_TranId", DbType="int")]
		public int? TranId
		{
			get
			{
				return this._TranId;
			}

			set
			{
				if (this._TranId != value)
					this._TranId = value;
			}

		}

		
		[Column(Name="Score", Storage="_Score", DbType="int NOT NULL")]
		public int Score
		{
			get
			{
				return this._Score;
			}

			set
			{
				if (this._Score != value)
					this._Score = value;
			}

		}

		
		[Column(Name="SmallGroups", Storage="_SmallGroups", DbType="nvarchar(2000)")]
		public string SmallGroups
		{
			get
			{
				return this._SmallGroups;
			}

			set
			{
				if (this._SmallGroups != value)
					this._SmallGroups = value;
			}

		}

		
		[Column(Name="SkipInsertTriggerProcessing", Storage="_SkipInsertTriggerProcessing", DbType="bit")]
		public bool? SkipInsertTriggerProcessing
		{
			get
			{
				return this._SkipInsertTriggerProcessing;
			}

			set
			{
				if (this._SkipInsertTriggerProcessing != value)
					this._SkipInsertTriggerProcessing = value;
			}

		}

		
		[Column(Name="PrevTranType", Storage="_PrevTranType", DbType="int")]
		public int? PrevTranType
		{
			get
			{
				return this._PrevTranType;
			}

			set
			{
				if (this._PrevTranType != value)
					this._PrevTranType = value;
			}

		}

		
		[Column(Name="isgood", Storage="_Isgood", DbType="int NOT NULL")]
		public int Isgood
		{
			get
			{
				return this._Isgood;
			}

			set
			{
				if (this._Isgood != value)
					this._Isgood = value;
			}

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

		
		[Column(Name="Code", Storage="_Code", DbType="nvarchar(20)")]
		public string Code
		{
			get
			{
				return this._Code;
			}

			set
			{
				if (this._Code != value)
					this._Code = value;
			}

		}

		
		[Column(Name="Description", Storage="_Description", DbType="nvarchar(100)")]
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

		
		[Column(Name="AttendanceTypeId", Storage="_AttendanceTypeId", DbType="int")]
		public int? AttendanceTypeId
		{
			get
			{
				return this._AttendanceTypeId;
			}

			set
			{
				if (this._AttendanceTypeId != value)
					this._AttendanceTypeId = value;
			}

		}

		
		[Column(Name="Hardwired", Storage="_Hardwired", DbType="bit")]
		public bool? Hardwired
		{
			get
			{
				return this._Hardwired;
			}

			set
			{
				if (this._Hardwired != value)
					this._Hardwired = value;
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

		
    }

}
