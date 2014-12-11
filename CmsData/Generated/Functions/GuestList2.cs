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
	[Table(Name="GuestList2")]
	public partial class GuestList2
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private DateTime _LastAttendDt;
		
		private bool _Hidden;
		
		private int? _MemberTypeId;
		
		
		public GuestList2()
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

		
		[Column(Name="LastAttendDt", Storage="_LastAttendDt", DbType="datetime NOT NULL")]
		public DateTime LastAttendDt
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

		
		[Column(Name="MemberTypeId", Storage="_MemberTypeId", DbType="int")]
		public int? MemberTypeId
		{
			get
			{
				return this._MemberTypeId;
			}

			set
			{
				if (this._MemberTypeId != value)
					this._MemberTypeId = value;
			}

		}

		
    }

}
