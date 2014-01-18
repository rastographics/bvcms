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
	[Table(Name="WeeklyAttendsForOrgs")]
	public partial class WeeklyAttendsForOrg
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private bool _Attended;
		
		private DateTime _Sunday;
		
		
		public WeeklyAttendsForOrg()
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

		
		[Column(Name="Attended", Storage="_Attended", DbType="bit NOT NULL")]
		public bool Attended
		{
			get
			{
				return this._Attended;
			}

			set
			{
				if (this._Attended != value)
					this._Attended = value;
			}

		}

		
		[Column(Name="Sunday", Storage="_Sunday", DbType="datetime NOT NULL")]
		public DateTime Sunday
		{
			get
			{
				return this._Sunday;
			}

			set
			{
				if (this._Sunday != value)
					this._Sunday = value;
			}

		}

		
    }

}
