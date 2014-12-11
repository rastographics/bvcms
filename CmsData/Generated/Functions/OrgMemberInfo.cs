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
	[Table(Name="OrgMemberInfo")]
	public partial class OrgMemberInfo
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private int? _LastMeetingId;
		
		private DateTime? _LastAttendedDt;
		
		
		public OrgMemberInfo()
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

		
		[Column(Name="LastMeetingId", Storage="_LastMeetingId", DbType="int")]
		public int? LastMeetingId
		{
			get
			{
				return this._LastMeetingId;
			}

			set
			{
				if (this._LastMeetingId != value)
					this._LastMeetingId = value;
			}

		}

		
		[Column(Name="LastAttendedDt", Storage="_LastAttendedDt", DbType="datetime")]
		public DateTime? LastAttendedDt
		{
			get
			{
				return this._LastAttendedDt;
			}

			set
			{
				if (this._LastAttendedDt != value)
					this._LastAttendedDt = value;
			}

		}

		
    }

}
