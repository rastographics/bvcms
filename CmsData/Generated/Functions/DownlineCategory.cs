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
	[Table(Name="DownlineCategories")]
	public partial class DownlineCategory
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _Rownum;
		
		private int? _Id;
		
		private string _Name;
		
		private bool? _Mainfellowship;
		
		private string _Programs;
		
		private string _Divisions;
		
		
		public DownlineCategory()
		{
		}

		
		
		[Column(Name="rownum", Storage="_Rownum", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsDbGenerated=true)]
		public int Rownum
		{
			get
			{
				return this._Rownum;
			}

			set
			{
				if (this._Rownum != value)
					this._Rownum = value;
			}

		}

		
		[Column(Name="id", Storage="_Id", DbType="int")]
		public int? Id
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

		
		[Column(Name="name", Storage="_Name", DbType="varchar(50)")]
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

		
		[Column(Name="mainfellowship", Storage="_Mainfellowship", DbType="bit")]
		public bool? Mainfellowship
		{
			get
			{
				return this._Mainfellowship;
			}

			set
			{
				if (this._Mainfellowship != value)
					this._Mainfellowship = value;
			}

		}

		
		[Column(Name="programs", Storage="_Programs", DbType="varchar(100)")]
		public string Programs
		{
			get
			{
				return this._Programs;
			}

			set
			{
				if (this._Programs != value)
					this._Programs = value;
			}

		}

		
		[Column(Name="divisions", Storage="_Divisions", DbType="varchar(100)")]
		public string Divisions
		{
			get
			{
				return this._Divisions;
			}

			set
			{
				if (this._Divisions != value)
					this._Divisions = value;
			}

		}

		
    }

}
