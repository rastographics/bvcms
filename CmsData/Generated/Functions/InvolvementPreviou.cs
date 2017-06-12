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
	[Table(Name="InvolvementPrevious")]
	public partial class InvolvementPreviou
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private string _OrgType;
		
		private string _OrgCode;
		
		private int _OrgTypeSort;
		
		private string _Name;
		
		private string _DivisionName;
		
		private string _ProgramName;
		
		private string _LeaderName;
		
		private string _Location;
		
		private DateTime? _MeetingTime;
		
		private string _MemberType;
		
		private DateTime? _EnrollDate;
		
		private DateTime _TransactionDate;
		
		private DateTime? _FirstTransactionDate;
		
		private decimal? _AttendancePercentage;
		
		private bool? _HasDirectory;
		
		private bool? _IsLeaderAttendanceType;
		
		private int _PeopleId;
		
		private int? _LeaderId;
		
		private bool? _Pending;
		
		private string _LimitToRole;
		
		private int _SecurityTypeId;
		
		private int _TransactionTypeId;
		
		private bool _TransactionStatus;
		
		
		public InvolvementPreviou()
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

		
		[Column(Name="OrgType", Storage="_OrgType", DbType="nvarchar(50) NOT NULL")]
		public string OrgType
		{
			get
			{
				return this._OrgType;
			}

			set
			{
				if (this._OrgType != value)
					this._OrgType = value;
			}

		}

		
		[Column(Name="OrgCode", Storage="_OrgCode", DbType="nvarchar(20)")]
		public string OrgCode
		{
			get
			{
				return this._OrgCode;
			}

			set
			{
				if (this._OrgCode != value)
					this._OrgCode = value;
			}

		}

		
		[Column(Name="OrgTypeSort", Storage="_OrgTypeSort", DbType="int NOT NULL")]
		public int OrgTypeSort
		{
			get
			{
				return this._OrgTypeSort;
			}

			set
			{
				if (this._OrgTypeSort != value)
					this._OrgTypeSort = value;
			}

		}

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(100) NOT NULL")]
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

		
		[Column(Name="DivisionName", Storage="_DivisionName", DbType="nvarchar(50)")]
		public string DivisionName
		{
			get
			{
				return this._DivisionName;
			}

			set
			{
				if (this._DivisionName != value)
					this._DivisionName = value;
			}

		}

		
		[Column(Name="ProgramName", Storage="_ProgramName", DbType="nvarchar(50)")]
		public string ProgramName
		{
			get
			{
				return this._ProgramName;
			}

			set
			{
				if (this._ProgramName != value)
					this._ProgramName = value;
			}

		}

		
		[Column(Name="LeaderName", Storage="_LeaderName", DbType="nvarchar(50)")]
		public string LeaderName
		{
			get
			{
				return this._LeaderName;
			}

			set
			{
				if (this._LeaderName != value)
					this._LeaderName = value;
			}

		}

		
		[Column(Name="Location", Storage="_Location", DbType="nvarchar(200)")]
		public string Location
		{
			get
			{
				return this._Location;
			}

			set
			{
				if (this._Location != value)
					this._Location = value;
			}

		}

		
		[Column(Name="MeetingTime", Storage="_MeetingTime", DbType="datetime")]
		public DateTime? MeetingTime
		{
			get
			{
				return this._MeetingTime;
			}

			set
			{
				if (this._MeetingTime != value)
					this._MeetingTime = value;
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

		
		[Column(Name="FirstTransactionDate", Storage="_FirstTransactionDate", DbType="datetime")]
		public DateTime? FirstTransactionDate
		{
			get
			{
				return this._FirstTransactionDate;
			}

			set
			{
				if (this._FirstTransactionDate != value)
					this._FirstTransactionDate = value;
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

		
		[Column(Name="HasDirectory", Storage="_HasDirectory", DbType="bit")]
		public bool? HasDirectory
		{
			get
			{
				return this._HasDirectory;
			}

			set
			{
				if (this._HasDirectory != value)
					this._HasDirectory = value;
			}

		}

		
		[Column(Name="IsLeaderAttendanceType", Storage="_IsLeaderAttendanceType", DbType="bit")]
		public bool? IsLeaderAttendanceType
		{
			get
			{
				return this._IsLeaderAttendanceType;
			}

			set
			{
				if (this._IsLeaderAttendanceType != value)
					this._IsLeaderAttendanceType = value;
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

		
		[Column(Name="LeaderId", Storage="_LeaderId", DbType="int")]
		public int? LeaderId
		{
			get
			{
				return this._LeaderId;
			}

			set
			{
				if (this._LeaderId != value)
					this._LeaderId = value;
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

		
		[Column(Name="LimitToRole", Storage="_LimitToRole", DbType="nvarchar(20)")]
		public string LimitToRole
		{
			get
			{
				return this._LimitToRole;
			}

			set
			{
				if (this._LimitToRole != value)
					this._LimitToRole = value;
			}

		}

		
		[Column(Name="SecurityTypeId", Storage="_SecurityTypeId", DbType="int NOT NULL")]
		public int SecurityTypeId
		{
			get
			{
				return this._SecurityTypeId;
			}

			set
			{
				if (this._SecurityTypeId != value)
					this._SecurityTypeId = value;
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

		
    }

}
