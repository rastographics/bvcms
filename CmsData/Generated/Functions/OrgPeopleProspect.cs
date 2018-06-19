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
	[Table(Name="OrgPeopleProspects")]
	public partial class OrgPeopleProspect
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Tab;
		
		private string _GroupCode;
		
		private decimal? _AttPct;
		
		private DateTime? _LastAttended;
		
		private DateTime? _Joined;
		
		private DateTime? _Dropped;
		
		private DateTime? _InactiveDate;
		
		private string _MemberCode;
		
		private string _MemberType;
		
		private bool _Hidden;
		
		private string _Groups;
		
		private int? _Grade;
		
		
		public OrgPeopleProspect()
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

		
		[Column(Name="Hidden", Storage="_Hidden", DbType="bit NOT NULL")]
		public bool Hidden
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

		
    }

}
