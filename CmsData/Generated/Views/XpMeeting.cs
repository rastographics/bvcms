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
	[Table(Name="XpMeeting")]
	public partial class XpMeeting
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _MeetingId;
		
		private int _OrganizationId;
		
		private bool _GroupMeetingFlag;
		
		private string _Description;
		
		private string _Location;
		
		private int _NumPresent;
		
		private int _NumMembers;
		
		private int _NumVstMembers;
		
		private int _NumRepeatVst;
		
		private int _NumNewVisit;
		
		private DateTime? _MeetingDate;
		
		private int? _NumOutTown;
		
		private int? _NumOtherAttends;
		
		private int? _HeadCount;
		
		
		public XpMeeting()
		{
		}

		
		
		[Column(Name="MeetingId", Storage="_MeetingId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsDbGenerated=true)]
		public int MeetingId
		{
			get
			{
				return this._MeetingId;
			}

			set
			{
				if (this._MeetingId != value)
					this._MeetingId = value;
			}

		}

		
		[Column(Name="OrganizationId", Storage="_OrganizationId", DbType="int NOT NULL")]
		public int OrganizationId
		{
			get
			{
				return this._OrganizationId;
			}

			set
			{
				if (this._OrganizationId != value)
					this._OrganizationId = value;
			}

		}

		
		[Column(Name="GroupMeetingFlag", Storage="_GroupMeetingFlag", DbType="bit NOT NULL")]
		public bool GroupMeetingFlag
		{
			get
			{
				return this._GroupMeetingFlag;
			}

			set
			{
				if (this._GroupMeetingFlag != value)
					this._GroupMeetingFlag = value;
			}

		}

		
		[Column(Name="Description", Storage="_Description", DbType="nvarchar(100)")]
		public string Description
		{
			get
			{
				return this._Description;
			}

			set
			{
				if (this._Description != value)
					this._Description = value;
			}

		}

		
		[Column(Name="Location", Storage="_Location", DbType="nvarchar(200)")]
		public string Location
		{
			get
			{
				return this._Location;
			}

			set
			{
				if (this._Location != value)
					this._Location = value;
			}

		}

		
		[Column(Name="NumPresent", Storage="_NumPresent", DbType="int NOT NULL")]
		public int NumPresent
		{
			get
			{
				return this._NumPresent;
			}

			set
			{
				if (this._NumPresent != value)
					this._NumPresent = value;
			}

		}

		
		[Column(Name="NumMembers", Storage="_NumMembers", DbType="int NOT NULL")]
		public int NumMembers
		{
			get
			{
				return this._NumMembers;
			}

			set
			{
				if (this._NumMembers != value)
					this._NumMembers = value;
			}

		}

		
		[Column(Name="NumVstMembers", Storage="_NumVstMembers", DbType="int NOT NULL")]
		public int NumVstMembers
		{
			get
			{
				return this._NumVstMembers;
			}

			set
			{
				if (this._NumVstMembers != value)
					this._NumVstMembers = value;
			}

		}

		
		[Column(Name="NumRepeatVst", Storage="_NumRepeatVst", DbType="int NOT NULL")]
		public int NumRepeatVst
		{
			get
			{
				return this._NumRepeatVst;
			}

			set
			{
				if (this._NumRepeatVst != value)
					this._NumRepeatVst = value;
			}

		}

		
		[Column(Name="NumNewVisit", Storage="_NumNewVisit", DbType="int NOT NULL")]
		public int NumNewVisit
		{
			get
			{
				return this._NumNewVisit;
			}

			set
			{
				if (this._NumNewVisit != value)
					this._NumNewVisit = value;
			}

		}

		
		[Column(Name="MeetingDate", Storage="_MeetingDate", DbType="datetime")]
		public DateTime? MeetingDate
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

		
		[Column(Name="NumOutTown", Storage="_NumOutTown", DbType="int")]
		public int? NumOutTown
		{
			get
			{
				return this._NumOutTown;
			}

			set
			{
				if (this._NumOutTown != value)
					this._NumOutTown = value;
			}

		}

		
		[Column(Name="NumOtherAttends", Storage="_NumOtherAttends", DbType="int")]
		public int? NumOtherAttends
		{
			get
			{
				return this._NumOtherAttends;
			}

			set
			{
				if (this._NumOtherAttends != value)
					this._NumOtherAttends = value;
			}

		}

		
		[Column(Name="HeadCount", Storage="_HeadCount", DbType="int")]
		public int? HeadCount
		{
			get
			{
				return this._HeadCount;
			}

			set
			{
				if (this._HeadCount != value)
					this._HeadCount = value;
			}

		}

		
    }

}
