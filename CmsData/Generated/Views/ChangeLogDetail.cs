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
	[Table(Name="ChangeLogDetails")]
	public partial class ChangeLogDetail
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _Id;
		
		private int _PeopleId;
		
		private string _Section;
		
		private DateTime _Created;
		
		private int? _FamilyId;
		
		private int _UserPeopleId;
		
		private string _Field;
		
		private string _Before;
		
		private string _After;
		
		
		public ChangeLogDetail()
		{
		}

		
		
		[Column(Name="Id", Storage="_Id", DbType="int NOT NULL")]
		public int Id
		{
			get
			{
				return this._Id;
			}

			set
			{
				if (this._Id != value)
					this._Id = value;
			}

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

		
		[Column(Name="Section", Storage="_Section", DbType="nvarchar(50)")]
		public string Section
		{
			get
			{
				return this._Section;
			}

			set
			{
				if (this._Section != value)
					this._Section = value;
			}

		}

		
		[Column(Name="Created", Storage="_Created", DbType="datetime NOT NULL")]
		public DateTime Created
		{
			get
			{
				return this._Created;
			}

			set
			{
				if (this._Created != value)
					this._Created = value;
			}

		}

		
		[Column(Name="FamilyId", Storage="_FamilyId", DbType="int")]
		public int? FamilyId
		{
			get
			{
				return this._FamilyId;
			}

			set
			{
				if (this._FamilyId != value)
					this._FamilyId = value;
			}

		}

		
		[Column(Name="UserPeopleId", Storage="_UserPeopleId", DbType="int NOT NULL")]
		public int UserPeopleId
		{
			get
			{
				return this._UserPeopleId;
			}

			set
			{
				if (this._UserPeopleId != value)
					this._UserPeopleId = value;
			}

		}

		
		[Column(Name="Field", Storage="_Field", DbType="nvarchar(50) NOT NULL")]
		public string Field
		{
			get
			{
				return this._Field;
			}

			set
			{
				if (this._Field != value)
					this._Field = value;
			}

		}

		
		[Column(Name="Before", Storage="_Before", DbType="nvarchar")]
		public string Before
		{
			get
			{
				return this._Before;
			}

			set
			{
				if (this._Before != value)
					this._Before = value;
			}

		}

		
		[Column(Name="After", Storage="_After", DbType="nvarchar")]
		public string After
		{
			get
			{
				return this._After;
			}

			set
			{
				if (this._After != value)
					this._After = value;
			}

		}

		
    }

}
