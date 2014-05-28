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
	[Table(Name="CheckinByDate")]
	public partial class CheckinByDate
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _PeopleId;
		
		private string _Name;
		
		private int? _OrgId;
		
		private string _OrgName;
		
		private string _Time;
		
		private bool? _Present;
		
		
		public CheckinByDate()
		{
		}

		
		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int")]
		public int? PeopleId
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

		
		[Column(Name="Name", Storage="_Name", DbType="varchar(100)")]
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

		
		[Column(Name="OrgId", Storage="_OrgId", DbType="int")]
		public int? OrgId
		{
			get
			{
				return this._OrgId;
			}

			set
			{
				if (this._OrgId != value)
					this._OrgId = value;
			}

		}

		
		[Column(Name="OrgName", Storage="_OrgName", DbType="varchar(100)")]
		public string OrgName
		{
			get
			{
				return this._OrgName;
			}

			set
			{
				if (this._OrgName != value)
					this._OrgName = value;
			}

		}

		
		[Column(Name="time", Storage="_Time", DbType="time")]
		public string Time
		{
			get
			{
				return this._Time;
			}

			set
			{
				if (this._Time != value)
					this._Time = value;
			}

		}

		
		[Column(Name="present", Storage="_Present", DbType="bit")]
		public bool? Present
		{
			get
			{
				return this._Present;
			}

			set
			{
				if (this._Present != value)
					this._Present = value;
			}

		}

		
    }

}
