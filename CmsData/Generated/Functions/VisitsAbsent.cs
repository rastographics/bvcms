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
	[Table(Name="VisitsAbsents")]
	public partial class VisitsAbsent
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Name;
		
		private string _Status;
		
		private string _MemberType;
		
		private bool _AttendanceFlag;
		
		private int _MemberTypeId;
		
		private DateTime? _LastAttended;
		
		private decimal? _AttendPct;
		
		private string _AttendStr;
		
		private DateTime? _Birthday;
		
		private string _EmailAddress;
		
		private string _HomePhone;
		
		private string _CellPhone;
		
		private string _PrimaryAddress;
		
		private string _PrimaryCity;
		
		private string _PrimaryState;
		
		private string _PrimaryZip;
		
		
		public VisitsAbsent()
		{
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

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(139)")]
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

		
		[Column(Name="status", Storage="_Status", DbType="nvarchar(100)")]
		public string Status
		{
			get
			{
				return this._Status;
			}

			set
			{
				if (this._Status != value)
					this._Status = value;
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

		
		[Column(Name="AttendanceFlag", Storage="_AttendanceFlag", DbType="bit NOT NULL")]
		public bool AttendanceFlag
		{
			get
			{
				return this._AttendanceFlag;
			}

			set
			{
				if (this._AttendanceFlag != value)
					this._AttendanceFlag = value;
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

		
		[Column(Name="LastAttended", Storage="_LastAttended", DbType="datetime")]
		public DateTime? LastAttended
		{
			get
			{
				return this._LastAttended;
			}

			set
			{
				if (this._LastAttended != value)
					this._LastAttended = value;
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

		
		[Column(Name="Birthday", Storage="_Birthday", DbType="datetime")]
		public DateTime? Birthday
		{
			get
			{
				return this._Birthday;
			}

			set
			{
				if (this._Birthday != value)
					this._Birthday = value;
			}

		}

		
		[Column(Name="EmailAddress", Storage="_EmailAddress", DbType="nvarchar(150)")]
		public string EmailAddress
		{
			get
			{
				return this._EmailAddress;
			}

			set
			{
				if (this._EmailAddress != value)
					this._EmailAddress = value;
			}

		}

		
		[Column(Name="HomePhone", Storage="_HomePhone", DbType="nvarchar(20)")]
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

		
		[Column(Name="CellPhone", Storage="_CellPhone", DbType="nvarchar(20)")]
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

		
		[Column(Name="PrimaryAddress", Storage="_PrimaryAddress", DbType="nvarchar(100)")]
		public string PrimaryAddress
		{
			get
			{
				return this._PrimaryAddress;
			}

			set
			{
				if (this._PrimaryAddress != value)
					this._PrimaryAddress = value;
			}

		}

		
		[Column(Name="PrimaryCity", Storage="_PrimaryCity", DbType="nvarchar(30)")]
		public string PrimaryCity
		{
			get
			{
				return this._PrimaryCity;
			}

			set
			{
				if (this._PrimaryCity != value)
					this._PrimaryCity = value;
			}

		}

		
		[Column(Name="PrimaryState", Storage="_PrimaryState", DbType="nvarchar(20)")]
		public string PrimaryState
		{
			get
			{
				return this._PrimaryState;
			}

			set
			{
				if (this._PrimaryState != value)
					this._PrimaryState = value;
			}

		}

		
		[Column(Name="PrimaryZip", Storage="_PrimaryZip", DbType="nvarchar(15)")]
		public string PrimaryZip
		{
			get
			{
				return this._PrimaryZip;
			}

			set
			{
				if (this._PrimaryZip != value)
					this._PrimaryZip = value;
			}

		}

		
    }

}
