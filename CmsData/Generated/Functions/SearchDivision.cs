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
	[Table(Name="SearchDivisions")]
	public partial class SearchDivision
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _DivId;
		
		private string _Division;
		
		private string _Program;
		
		private string _Programs;
		
		private bool? _IsChecked;
		
		private bool? _IsMain;
		
		
		public SearchDivision()
		{
		}

		
		
		[Column(Name="DivId", Storage="_DivId", DbType="int NOT NULL")]
		public int DivId
		{
			get
			{
				return this._DivId;
			}

			set
			{
				if (this._DivId != value)
					this._DivId = value;
			}

		}

		
		[Column(Name="Division", Storage="_Division", DbType="nvarchar(50)")]
		public string Division
		{
			get
			{
				return this._Division;
			}

			set
			{
				if (this._Division != value)
					this._Division = value;
			}

		}

		
		[Column(Name="Program", Storage="_Program", DbType="nvarchar(50)")]
		public string Program
		{
			get
			{
				return this._Program;
			}

			set
			{
				if (this._Program != value)
					this._Program = value;
			}

		}

		
		[Column(Name="Programs", Storage="_Programs", DbType="nvarchar")]
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

		
		[Column(Name="IsChecked", Storage="_IsChecked", DbType="bit")]
		public bool? IsChecked
		{
			get
			{
				return this._IsChecked;
			}

			set
			{
				if (this._IsChecked != value)
					this._IsChecked = value;
			}

		}

		
		[Column(Name="IsMain", Storage="_IsMain", DbType="bit")]
		public bool? IsMain
		{
			get
			{
				return this._IsMain;
			}

			set
			{
				if (this._IsMain != value)
					this._IsMain = value;
			}

		}

		
    }

}
