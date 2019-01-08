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
	[Table(Name="XpAttendance")]
	public partial class XpAttendance
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private int _MeetingId;
		
		private int _OrganizationId;
		
		private DateTime _MeetingDate;
		
		private bool _AttendanceFlag;
		
		private string _AttendanceType;
		
		private string _MemberType;
		
		
		public XpAttendance()
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

		
		[Column(Name="MeetingId", Storage="_MeetingId", DbType="int NOT NULL")]
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

		
		[Column(Name="AttendanceFlag", Storage="_AttendanceFlag", DbType="bit NOT NULL")]
		public bool AttendanceFlag
		{
			get
			{
				return this._AttendanceFlag;
			}

			set
			{
				if (this._AttendanceFlag != value)
					this._AttendanceFlag = value;
			}

		}

		
		[Column(Name="AttendanceType", Storage="_AttendanceType", DbType="nvarchar(100)")]
		public string AttendanceType
		{
			get
			{
				return this._AttendanceType;
			}

			set
			{
				if (this._AttendanceType != value)
					this._AttendanceType = value;
			}

		}

		
		[Column(Name="MemberType", Storage="_MemberType", DbType="nvarchar(100)")]
		public string MemberType
		{
			get
			{
				return this._MemberType;
			}

			set
			{
				if (this._MemberType != value)
					this._MemberType = value;
			}

		}

		
    }

}
