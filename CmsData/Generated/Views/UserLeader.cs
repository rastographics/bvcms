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
	[Table(Name="UserLeaders")]
	public partial class UserLeader
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Name;
		
		private string _Username;
		
		private int? _UserId;
		
		private string _Access;
		
		private string _OrgLeaderOnly;
		
		
		public UserLeader()
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

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(138)")]
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

		
		[Column(Name="Username", Storage="_Username", DbType="nvarchar(50)")]
		public string Username
		{
			get
			{
				return this._Username;
			}

			set
			{
				if (this._Username != value)
					this._Username = value;
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

		
		[Column(Name="Access", Storage="_Access", DbType="varchar(6)")]
		public string Access
		{
			get
			{
				return this._Access;
			}

			set
			{
				if (this._Access != value)
					this._Access = value;
			}

		}

		
		[Column(Name="OrgLeaderOnly", Storage="_OrgLeaderOnly", DbType="varchar(14)")]
		public string OrgLeaderOnly
		{
			get
			{
				return this._OrgLeaderOnly;
			}

			set
			{
				if (this._OrgLeaderOnly != value)
					this._OrgLeaderOnly = value;
			}

		}

		
    }

}
