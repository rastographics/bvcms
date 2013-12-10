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
	[Table(Name="OrgVisitorsAsOfDate")]
	public partial class OrgVisitorsAsOfDate
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _VisitorType;
		
		private int _PeopleId;
		
		private int _FamilyId;
		
		private string _PreferredName;
		
		private string _LastName;
		
		private int? _BirthYear;
		
		private int? _BirthMonth;
		
		private int? _BirthDay;
		
		private string _PrimaryAddress;
		
		private string _PrimaryAddress2;
		
		private string _PrimaryCity;
		
		private string _PrimaryState;
		
		private string _PrimaryZip;
		
		private string _HomePhone;
		
		private string _CellPhone;
		
		private string _WorkPhone;
		
		private string _MemberStatus;
		
		private string _Email;
		
		private string _BFTeacher;
		
		private int? _BFTeacherId;
		
		private int? _Age;
		
		private DateTime? _LastAttended;
		
		
		public OrgVisitorsAsOfDate()
		{
		}

		
		
		[Column(Name="VisitorType", Storage="_VisitorType", DbType="varchar(2) NOT NULL")]
		public string VisitorType
		{
			get
			{
				return this._VisitorType;
			}

			set
			{
				if (this._VisitorType != value)
					this._VisitorType = value;
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

		
		[Column(Name="FamilyId", Storage="_FamilyId", DbType="int NOT NULL")]
		public int FamilyId
		{
			get
			{
				return this._FamilyId;
			}

			set
			{
				if (this._FamilyId != value)
					this._FamilyId = value;
			}

		}

		
		[Column(Name="PreferredName", Storage="_PreferredName", DbType="nvarchar(25)")]
		public string PreferredName
		{
			get
			{
				return this._PreferredName;
			}

			set
			{
				if (this._PreferredName != value)
					this._PreferredName = value;
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

		
		[Column(Name="BirthYear", Storage="_BirthYear", DbType="int")]
		public int? BirthYear
		{
			get
			{
				return this._BirthYear;
			}

			set
			{
				if (this._BirthYear != value)
					this._BirthYear = value;
			}

		}

		
		[Column(Name="BirthMonth", Storage="_BirthMonth", DbType="int")]
		public int? BirthMonth
		{
			get
			{
				return this._BirthMonth;
			}

			set
			{
				if (this._BirthMonth != value)
					this._BirthMonth = value;
			}

		}

		
		[Column(Name="BirthDay", Storage="_BirthDay", DbType="int")]
		public int? BirthDay
		{
			get
			{
				return this._BirthDay;
			}

			set
			{
				if (this._BirthDay != value)
					this._BirthDay = value;
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

		
		[Column(Name="PrimaryAddress2", Storage="_PrimaryAddress2", DbType="nvarchar(100)")]
		public string PrimaryAddress2
		{
			get
			{
				return this._PrimaryAddress2;
			}

			set
			{
				if (this._PrimaryAddress2 != value)
					this._PrimaryAddress2 = value;
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

		
		[Column(Name="WorkPhone", Storage="_WorkPhone", DbType="nvarchar(20)")]
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

		
		[Column(Name="BFTeacher", Storage="_BFTeacher", DbType="nvarchar(139)")]
		public string BFTeacher
		{
			get
			{
				return this._BFTeacher;
			}

			set
			{
				if (this._BFTeacher != value)
					this._BFTeacher = value;
			}

		}

		
		[Column(Name="BFTeacherId", Storage="_BFTeacherId", DbType="int")]
		public int? BFTeacherId
		{
			get
			{
				return this._BFTeacherId;
			}

			set
			{
				if (this._BFTeacherId != value)
					this._BFTeacherId = value;
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

		
    }

}
