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
		
		private int? _ContacteeId;
		
		private int? _ContactorId;
		
		private int? _TaskAboutId;
		
		private int? _TaskDelegatedId;
		
		private DateTime? _LastAttendDt;
		
		private DateTime? _ContactReceived;
		
		private DateTime? _ContactMade;
		
		private DateTime? _TaskAboutDt;
		
		private DateTime? _TaskDelegatedDt;
		
		
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

		
		[Column(Name="ContacteeId", Storage="_ContacteeId", DbType="int")]
		public int? ContacteeId
		{
			get
			{
				return this._ContacteeId;
			}

			set
			{
				if (this._ContacteeId != value)
					this._ContacteeId = value;
			}

		}

		
		[Column(Name="ContactorId", Storage="_ContactorId", DbType="int")]
		public int? ContactorId
		{
			get
			{
				return this._ContactorId;
			}

			set
			{
				if (this._ContactorId != value)
					this._ContactorId = value;
			}

		}

		
		[Column(Name="TaskAboutId", Storage="_TaskAboutId", DbType="int")]
		public int? TaskAboutId
		{
			get
			{
				return this._TaskAboutId;
			}

			set
			{
				if (this._TaskAboutId != value)
					this._TaskAboutId = value;
			}

		}

		
		[Column(Name="TaskDelegatedId", Storage="_TaskDelegatedId", DbType="int")]
		public int? TaskDelegatedId
		{
			get
			{
				return this._TaskDelegatedId;
			}

			set
			{
				if (this._TaskDelegatedId != value)
					this._TaskDelegatedId = value;
			}

		}

		
		[Column(Name="LastAttendDt", Storage="_LastAttendDt", DbType="datetime")]
		public DateTime? LastAttendDt
		{
			get
			{
				return this._LastAttendDt;
			}

			set
			{
				if (this._LastAttendDt != value)
					this._LastAttendDt = value;
			}

		}

		
		[Column(Name="ContactReceived", Storage="_ContactReceived", DbType="datetime")]
		public DateTime? ContactReceived
		{
			get
			{
				return this._ContactReceived;
			}

			set
			{
				if (this._ContactReceived != value)
					this._ContactReceived = value;
			}

		}

		
		[Column(Name="ContactMade", Storage="_ContactMade", DbType="datetime")]
		public DateTime? ContactMade
		{
			get
			{
				return this._ContactMade;
			}

			set
			{
				if (this._ContactMade != value)
					this._ContactMade = value;
			}

		}

		
		[Column(Name="TaskAboutDt", Storage="_TaskAboutDt", DbType="datetime")]
		public DateTime? TaskAboutDt
		{
			get
			{
				return this._TaskAboutDt;
			}

			set
			{
				if (this._TaskAboutDt != value)
					this._TaskAboutDt = value;
			}

		}

		
		[Column(Name="TaskDelegatedDt", Storage="_TaskDelegatedDt", DbType="datetime")]
		public DateTime? TaskDelegatedDt
		{
			get
			{
				return this._TaskDelegatedDt;
			}

			set
			{
				if (this._TaskDelegatedDt != value)
					this._TaskDelegatedDt = value;
			}

		}

		
    }

}
