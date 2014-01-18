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
	[Table(Name="MembersWhoAttendedOrgs")]
	public partial class MembersWhoAttendedOrg
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Name2;
		
		private int? _Age;
		
		private int? _BibleFellowshipClassId;
		
		private string _OrganizationName;
		
		private string _LeaderName;
		
		
		public MembersWhoAttendedOrg()
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

		
		[Column(Name="Name2", Storage="_Name2", DbType="varchar(90)")]
		public string Name2
		{
			get
			{
				return this._Name2;
			}

			set
			{
				if (this._Name2 != value)
					this._Name2 = value;
			}

		}

		
		[Column(Name="Age", Storage="_Age", DbType="int")]
		public int? Age
		{
			get
			{
				return this._Age;
			}

			set
			{
				if (this._Age != value)
					this._Age = value;
			}

		}

		
		[Column(Name="BibleFellowshipClassId", Storage="_BibleFellowshipClassId", DbType="int")]
		public int? BibleFellowshipClassId
		{
			get
			{
				return this._BibleFellowshipClassId;
			}

			set
			{
				if (this._BibleFellowshipClassId != value)
					this._BibleFellowshipClassId = value;
			}

		}

		
		[Column(Name="OrganizationName", Storage="_OrganizationName", DbType="varchar(100)")]
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

		
		[Column(Name="LeaderName", Storage="_LeaderName", DbType="varchar(90)")]
		public string LeaderName
		{
			get
			{
				return this._LeaderName;
			}

			set
			{
				if (this._LeaderName != value)
					this._LeaderName = value;
			}

		}

		
    }

}
