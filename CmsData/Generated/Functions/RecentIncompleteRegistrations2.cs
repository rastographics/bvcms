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
	[Table(Name="RecentIncompleteRegistrations2")]
	public partial class RecentIncompleteRegistrations2
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _PeopleId;
		
		private int? _OrgId;
		
		private int? _DatumId;
		
		private DateTime? _Stamp;
		
		private string _OrgName;
		
		private string _Name;
		
		
		public RecentIncompleteRegistrations2()
		{
		}

		
		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int")]
		public int? PeopleId
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

		
		[Column(Name="Stamp", Storage="_Stamp", DbType="datetime")]
		public DateTime? Stamp
		{
			get
			{
				return this._Stamp;
			}

			set
			{
				if (this._Stamp != value)
					this._Stamp = value;
			}

		}

		
		[Column(Name="OrgName", Storage="_OrgName", DbType="nvarchar(80)")]
		public string OrgName
		{
			get
			{
				return this._OrgName;
			}

			set
			{
				if (this._OrgName != value)
					this._OrgName = value;
			}

		}

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(80)")]
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

		
    }

}
