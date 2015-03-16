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
	[Table(Name="MeetingConflicts")]
	public partial class MeetingConflict
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private int? _OrgId1;
		
		private int? _OrgId2;
		
		private DateTime _MeetingDate;
		
		
		public MeetingConflict()
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

		
		[Column(Name="OrgId1", Storage="_OrgId1", DbType="int")]
		public int? OrgId1
		{
			get
			{
				return this._OrgId1;
			}

			set
			{
				if (this._OrgId1 != value)
					this._OrgId1 = value;
			}

		}

		
		[Column(Name="OrgId2", Storage="_OrgId2", DbType="int")]
		public int? OrgId2
		{
			get
			{
				return this._OrgId2;
			}

			set
			{
				if (this._OrgId2 != value)
					this._OrgId2 = value;
			}

		}

		
		[Column(Name="MeetingDate", Storage="_MeetingDate", DbType="datetime NOT NULL")]
		public DateTime MeetingDate
		{
			get
			{
				return this._MeetingDate;
			}

			set
			{
				if (this._MeetingDate != value)
					this._MeetingDate = value;
			}

		}

		
    }

}
