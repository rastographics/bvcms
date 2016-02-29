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
	[Table(Name="OrgDayStats")]
	public partial class OrgDayStat
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _OrganizationId;
		
		private string _OrganizationName;
		
		private string _Leader;
		
		private string _Location;
		
		private DateTime? _MeetingTime;
		
		private int? _Attends;
		
		private int? _Guests;
		
		private int? _Members;
		
		private int? _NewMembers;
		
		private int? _Dropped;
		
		private int? _Members7;
		
		
		public OrgDayStat()
		{
		}

		
		
		[Column(Name="OrganizationId", Storage="_OrganizationId", DbType="int")]
		public int? OrganizationId
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

		
		[Column(Name="OrganizationName", Storage="_OrganizationName", DbType="varchar(200)")]
		public string OrganizationName
		{
			get
			{
				return this._OrganizationName;
			}

			set
			{
				if (this._OrganizationName != value)
					this._OrganizationName = value;
			}

		}

		
		[Column(Name="Leader", Storage="_Leader", DbType="nvarchar(80)")]
		public string Leader
		{
			get
			{
				return this._Leader;
			}

			set
			{
				if (this._Leader != value)
					this._Leader = value;
			}

		}

		
		[Column(Name="Location", Storage="_Location", DbType="nvarchar(80)")]
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

		
		[Column(Name="MeetingTime", Storage="_MeetingTime", DbType="datetime")]
		public DateTime? MeetingTime
		{
			get
			{
				return this._MeetingTime;
			}

			set
			{
				if (this._MeetingTime != value)
					this._MeetingTime = value;
			}

		}

		
		[Column(Name="Attends", Storage="_Attends", DbType="int")]
		public int? Attends
		{
			get
			{
				return this._Attends;
			}

			set
			{
				if (this._Attends != value)
					this._Attends = value;
			}

		}

		
		[Column(Name="Guests", Storage="_Guests", DbType="int")]
		public int? Guests
		{
			get
			{
				return this._Guests;
			}

			set
			{
				if (this._Guests != value)
					this._Guests = value;
			}

		}

		
		[Column(Name="Members", Storage="_Members", DbType="int")]
		public int? Members
		{
			get
			{
				return this._Members;
			}

			set
			{
				if (this._Members != value)
					this._Members = value;
			}

		}

		
		[Column(Name="NewMembers", Storage="_NewMembers", DbType="int")]
		public int? NewMembers
		{
			get
			{
				return this._NewMembers;
			}

			set
			{
				if (this._NewMembers != value)
					this._NewMembers = value;
			}

		}

		
		[Column(Name="Dropped", Storage="_Dropped", DbType="int")]
		public int? Dropped
		{
			get
			{
				return this._Dropped;
			}

			set
			{
				if (this._Dropped != value)
					this._Dropped = value;
			}

		}

		
		[Column(Name="Members7", Storage="_Members7", DbType="int")]
		public int? Members7
		{
			get
			{
				return this._Members7;
			}

			set
			{
				if (this._Members7 != value)
					this._Members7 = value;
			}

		}

		
    }

}
