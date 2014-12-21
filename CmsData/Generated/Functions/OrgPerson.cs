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
	[Table(Name="OrgPeople")]
	public partial class OrgPerson
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Tab;
		
		private string _GroupCode;
		
		private string _Name;
		
		private string _Name2;
		
		private int? _Age;
		
		private int? _BirthDay;
		
		private int? _BirthMonth;
		
		private int? _BirthYear;
		
		private string _Address;
		
		private string _Address2;
		
		private string _City;
		
		private string _St;
		
		private string _Zip;
		
		private string _EmailAddress;
		
		private string _HomePhone;
		
		private string _CellPhone;
		
		private string _WorkPhone;
		
		private string _MemberStatus;
		
		private int? _LeaderId;
		
		private string _LeaderName;
		
		private bool? _HasTag;
		
		private decimal? _AttPct;
		
		private DateTime? _LastAttended;
		
		private DateTime? _Joined;
		
		private DateTime? _Dropped;
		
		private DateTime? _InactiveDate;
		
		private string _MemberCode;
		
		private string _MemberType;
		
		private bool? _Hidden;
		
		private string _Groups;
		
		
		public OrgPerson()
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

		
		[Column(Name="Tab", Storage="_Tab", DbType="varchar(9) NOT NULL")]
		public string Tab
		{
			get
			{
				return this._Tab;
			}

			set
			{
				if (this._Tab != value)
					this._Tab = value;
			}

		}

		
		[Column(Name="GroupCode", Storage="_GroupCode", DbType="varchar(2) NOT NULL")]
		public string GroupCode
		{
			get
			{
				return this._GroupCode;
			}

			set
			{
				if (this._GroupCode != value)
					this._GroupCode = value;
			}

		}

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(138)")]
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

		
		[Column(Name="Name2", Storage="_Name2", DbType="nvarchar(139)")]
		public string Name2
		{
			get
			{
				return this._Name2;
			}

			set
			{
				if (this._Name2 != value)
					this._Name2 = value;
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

		
		[Column(Name="Address2", Storage="_Address2", DbType="nvarchar(100)")]
		public string Address2
		{
			get
			{
				return this._Address2;
			}

			set
			{
				if (this._Address2 != value)
					this._Address2 = value;
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

		
		[Column(Name="ST", Storage="_St", DbType="nvarchar(20)")]
		public string St
		{
			get
			{
				return this._St;
			}

			set
			{
				if (this._St != value)
					this._St = value;
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

		
		[Column(Name="HasTag", Storage="_HasTag", DbType="bit")]
		public bool? HasTag
		{
			get
			{
				return this._HasTag;
			}

			set
			{
				if (this._HasTag != value)
					this._HasTag = value;
			}

		}

		
		[Column(Name="AttPct", Storage="_AttPct", DbType="real")]
		public decimal? AttPct
		{
			get
			{
				return this._AttPct;
			}

			set
			{
				if (this._AttPct != value)
					this._AttPct = value;
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

		
		[Column(Name="Joined", Storage="_Joined", DbType="datetime")]
		public DateTime? Joined
		{
			get
			{
				return this._Joined;
			}

			set
			{
				if (this._Joined != value)
					this._Joined = value;
			}

		}

		
		[Column(Name="Dropped", Storage="_Dropped", DbType="datetime")]
		public DateTime? Dropped
		{
			get
			{
				return this._Dropped;
			}

			set
			{
				if (this._Dropped != value)
					this._Dropped = value;
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

		
		[Column(Name="MemberCode", Storage="_MemberCode", DbType="nvarchar(20)")]
		public string MemberCode
		{
			get
			{
				return this._MemberCode;
			}

			set
			{
				if (this._MemberCode != value)
					this._MemberCode = value;
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

		
		[Column(Name="Hidden", Storage="_Hidden", DbType="bit")]
		public bool? Hidden
		{
			get
			{
				return this._Hidden;
			}

			set
			{
				if (this._Hidden != value)
					this._Hidden = value;
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

		
    }

}
