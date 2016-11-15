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
	[Table(Name="MoveSchedule")]
	public partial class MoveSchedule
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Name;
		
		private DateTime? _BirthDate;
		
		private int _FromOrgId;
		
		private string _FromOrg;
		
		private DateTime? _LastSunday;
		
		private int? _MosMax;
		
		private int? _ToOrgId;
		
		private string _ToOrg;
		
		private string _ToLocation;
		
		
		public MoveSchedule()
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

		
		[Column(Name="BirthDate", Storage="_BirthDate", DbType="date")]
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

		
		[Column(Name="FromOrgId", Storage="_FromOrgId", DbType="int NOT NULL")]
		public int FromOrgId
		{
			get
			{
				return this._FromOrgId;
			}

			set
			{
				if (this._FromOrgId != value)
					this._FromOrgId = value;
			}

		}

		
		[Column(Name="FromOrg", Storage="_FromOrg", DbType="nvarchar(100)")]
		public string FromOrg
		{
			get
			{
				return this._FromOrg;
			}

			set
			{
				if (this._FromOrg != value)
					this._FromOrg = value;
			}

		}

		
		[Column(Name="LastSunday", Storage="_LastSunday", DbType="date")]
		public DateTime? LastSunday
		{
			get
			{
				return this._LastSunday;
			}

			set
			{
				if (this._LastSunday != value)
					this._LastSunday = value;
			}

		}

		
		[Column(Name="MosMax", Storage="_MosMax", DbType="int")]
		public int? MosMax
		{
			get
			{
				return this._MosMax;
			}

			set
			{
				if (this._MosMax != value)
					this._MosMax = value;
			}

		}

		
		[Column(Name="ToOrgId", Storage="_ToOrgId", DbType="int")]
		public int? ToOrgId
		{
			get
			{
				return this._ToOrgId;
			}

			set
			{
				if (this._ToOrgId != value)
					this._ToOrgId = value;
			}

		}

		
		[Column(Name="ToOrg", Storage="_ToOrg", DbType="nvarchar(100)")]
		public string ToOrg
		{
			get
			{
				return this._ToOrg;
			}

			set
			{
				if (this._ToOrg != value)
					this._ToOrg = value;
			}

		}

		
		[Column(Name="ToLocation", Storage="_ToLocation", DbType="nvarchar(200)")]
		public string ToLocation
		{
			get
			{
				return this._ToLocation;
			}

			set
			{
				if (this._ToLocation != value)
					this._ToLocation = value;
			}

		}

		
    }

}
