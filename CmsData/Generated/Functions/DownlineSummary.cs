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
	[Table(Name="DownlineSummary")]
	public partial class DownlineSummary
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Rank;
		
		private int? _PeopleId;
		
		private string _LeaderName;
		
		private int? _Cnt;
		
		private int? _Levels;
		
		private int? _MaxRows;
		
		
		public DownlineSummary()
		{
		}

		
		
		[Column(Name="Rank", Storage="_Rank", DbType="int")]
		public int? Rank
		{
			get
			{
				return this._Rank;
			}

			set
			{
				if (this._Rank != value)
					this._Rank = value;
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

		
		[Column(Name="LeaderName", Storage="_LeaderName", DbType="varchar(100)")]
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

		
		[Column(Name="Cnt", Storage="_Cnt", DbType="int")]
		public int? Cnt
		{
			get
			{
				return this._Cnt;
			}

			set
			{
				if (this._Cnt != value)
					this._Cnt = value;
			}

		}

		
		[Column(Name="Levels", Storage="_Levels", DbType="int")]
		public int? Levels
		{
			get
			{
				return this._Levels;
			}

			set
			{
				if (this._Levels != value)
					this._Levels = value;
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
