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
	[Table(Name="RecentIncompleteRegistrations")]
	public partial class RecentIncompleteRegistration
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _PeopleId;
		
		private int? _OrgId;
		
		private int? _DatumId;
		
		private DateTime? _Stamp;
		
		
		public RecentIncompleteRegistration()
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

		
    }

}
