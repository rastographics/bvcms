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
	[Table(Name="VolunteerCalendar")]
	public partial class VolunteerCalendar
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Name;
		
		private bool? _Commits;
		
		private bool? _Conflicts;
		
		
		public VolunteerCalendar()
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

		
		[Column(Name="commits", Storage="_Commits", DbType="bit")]
		public bool? Commits
		{
			get
			{
				return this._Commits;
			}

			set
			{
				if (this._Commits != value)
					this._Commits = value;
			}

		}

		
		[Column(Name="conflicts", Storage="_Conflicts", DbType="bit")]
		public bool? Conflicts
		{
			get
			{
				return this._Conflicts;
			}

			set
			{
				if (this._Conflicts != value)
					this._Conflicts = value;
			}

		}

		
    }

}
