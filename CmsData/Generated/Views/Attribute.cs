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
	[Table(Name="Attributes")]
	public partial class Attribute
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Field;
		
		private string _Name;
		
		private string _ValueX;
		
		
		public Attribute()
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

		
		[Column(Name="Field", Storage="_Field", DbType="nvarchar(150) NOT NULL")]
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

		
		[Column(Name="name", Storage="_Name", DbType="varchar(200)")]
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

		
		[Column(Name="value", Storage="_ValueX", DbType="varchar")]
		public string ValueX
		{
			get
			{
				return this._ValueX;
			}

			set
			{
				if (this._ValueX != value)
					this._ValueX = value;
			}

		}

		
    }

}
