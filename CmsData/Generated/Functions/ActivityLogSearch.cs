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
	[Table(Name="ActivityLogSearch")]
	public partial class ActivityLogSearch
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _Machine;
		
		private DateTime? _DateX;
		
		private int? _UserId;
		
		private string _UserName;
		
		private string _Activity;
		
		private string _OrgName;
		
		private int? _OrgId;
		
		private int? _PeopleId;
		
		private string _PersonName;
		
		private int? _MaxRows;
		
		
		public ActivityLogSearch()
		{
		}

		
		
		[Column(Name="Machine", Storage="_Machine", DbType="varchar(50)")]
		public string Machine
		{
			get
			{
				return this._Machine;
			}

			set
			{
				if (this._Machine != value)
					this._Machine = value;
			}

		}

		
		[Column(Name="date", Storage="_DateX", DbType="datetime")]
		public DateTime? DateX
		{
			get
			{
				return this._DateX;
			}

			set
			{
				if (this._DateX != value)
					this._DateX = value;
			}

		}

		
		[Column(Name="UserId", Storage="_UserId", DbType="int")]
		public int? UserId
		{
			get
			{
				return this._UserId;
			}

			set
			{
				if (this._UserId != value)
					this._UserId = value;
			}

		}

		
		[Column(Name="UserName", Storage="_UserName", DbType="nvarchar(50)")]
		public string UserName
		{
			get
			{
				return this._UserName;
			}

			set
			{
				if (this._UserName != value)
					this._UserName = value;
			}

		}

		
		[Column(Name="Activity", Storage="_Activity", DbType="nvarchar(200)")]
		public string Activity
		{
			get
			{
				return this._Activity;
			}

			set
			{
				if (this._Activity != value)
					this._Activity = value;
			}

		}

		
		[Column(Name="OrgName", Storage="_OrgName", DbType="nvarchar(100)")]
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

		
		[Column(Name="PersonName", Storage="_PersonName", DbType="nvarchar(139)")]
		public string PersonName
		{
			get
			{
				return this._PersonName;
			}

			set
			{
				if (this._PersonName != value)
					this._PersonName = value;
			}

		}

		
		[Column(Name="MaxRows", Storage="_MaxRows", DbType="int")]
		public int? MaxRows
		{
			get
			{
				return this._MaxRows;
			}

			set
			{
				if (this._MaxRows != value)
					this._MaxRows = value;
			}

		}

		
    }

}
