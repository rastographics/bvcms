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
	[Table(Name="AllStatusFlags")]
	public partial class AllStatusFlag
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Flag;
		
		private string _Name;
		
		private string _Role;
		
		
		public AllStatusFlag()
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

		
		[Column(Name="Flag", Storage="_Flag", DbType="nvarchar(200) NOT NULL")]
		public string Flag
		{
			get
			{
				return this._Flag;
			}

			set
			{
				if (this._Flag != value)
					this._Flag = value;
			}

		}

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(4000)")]
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

		
		[Column(Name="Role", Storage="_Role", DbType="nvarchar(50)")]
		public string Role
		{
			get
			{
				return this._Role;
			}

			set
			{
				if (this._Role != value)
					this._Role = value;
			}

		}

		
    }

}
