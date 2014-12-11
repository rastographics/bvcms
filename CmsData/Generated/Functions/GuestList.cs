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
	[Table(Name="GuestList")]
	public partial class GuestList
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private int _MeetingId;
		
		private int _AttendId;
		
		private DateTime _MeetingDate;
		
		private bool _Hidden;
		
		
		public GuestList()
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

		
		[Column(Name="AttendId", Storage="_AttendId", DbType="int NOT NULL")]
		public int AttendId
		{
			get
			{
				return this._AttendId;
			}

			set
			{
				if (this._AttendId != value)
					this._AttendId = value;
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

		
		[Column(Name="Hidden", Storage="_Hidden", DbType="bit NOT NULL")]
		public bool Hidden
		{
			get
			{
				return this._Hidden;
			}

			set
			{
				if (this._Hidden != value)
					this._Hidden = value;
			}

		}

		
    }

}
