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
	[Table(Name="MeetingsDataForDateRange")]
	public partial class MeetingsDataForDateRange
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _Organization;
		
		private string _LeaderName;
		
		private DateTime? _Dt;
		
		private int? _Cnt;
		
		
		public MeetingsDataForDateRange()
		{
		}

		
		
		[Column(Name="Organization", Storage="_Organization", DbType="nvarchar(100) NOT NULL")]
		public string Organization
		{
			get
			{
				return this._Organization;
			}

			set
			{
				if (this._Organization != value)
					this._Organization = value;
			}

		}

		
		[Column(Name="LeaderName", Storage="_LeaderName", DbType="nvarchar(50)")]
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

		
		[Column(Name="dt", Storage="_Dt", DbType="datetime")]
		public DateTime? Dt
		{
			get
			{
				return this._Dt;
			}

			set
			{
				if (this._Dt != value)
					this._Dt = value;
			}

		}

		
		[Column(Name="cnt", Storage="_Cnt", DbType="int")]
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

		
    }

}
