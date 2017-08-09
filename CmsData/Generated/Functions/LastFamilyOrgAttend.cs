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
	[Table(Name="LastFamilyOrgAttends")]
	public partial class LastFamilyOrgAttend
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _FamilyId;
		
		private int _PeopleId;
		
		private DateTime? _Lastattend;
		
		
		public LastFamilyOrgAttend()
		{
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

		
		[Column(Name="lastattend", Storage="_Lastattend", DbType="datetime")]
		public DateTime? Lastattend
		{
			get
			{
				return this._Lastattend;
			}

			set
			{
				if (this._Lastattend != value)
					this._Lastattend = value;
			}

		}

		
    }

}
