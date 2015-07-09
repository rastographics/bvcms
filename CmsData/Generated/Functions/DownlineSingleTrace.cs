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
	[Table(Name="DownlineSingleTrace")]
	public partial class DownlineSingleTrace
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Generation;
		
		private int? _LeaderId;
		
		private string _LeaderName;
		
		private int? _DiscipleId;
		
		private string _DiscipleName;
		
		private string _OrgName;
		
		private DateTime? _StartDt;
		
		private string _Trace;
		
		
		public DownlineSingleTrace()
		{
		}

		
		
		[Column(Name="Generation", Storage="_Generation", DbType="int")]
		public int? Generation
		{
			get
			{
				return this._Generation;
			}

			set
			{
				if (this._Generation != value)
					this._Generation = value;
			}

		}

		
		[Column(Name="LeaderId", Storage="_LeaderId", DbType="int")]
		public int? LeaderId
		{
			get
			{
				return this._LeaderId;
			}

			set
			{
				if (this._LeaderId != value)
					this._LeaderId = value;
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

		
		[Column(Name="DiscipleId", Storage="_DiscipleId", DbType="int")]
		public int? DiscipleId
		{
			get
			{
				return this._DiscipleId;
			}

			set
			{
				if (this._DiscipleId != value)
					this._DiscipleId = value;
			}

		}

		
		[Column(Name="DiscipleName", Storage="_DiscipleName", DbType="varchar(100)")]
		public string DiscipleName
		{
			get
			{
				return this._DiscipleName;
			}

			set
			{
				if (this._DiscipleName != value)
					this._DiscipleName = value;
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

		
		[Column(Name="StartDt", Storage="_StartDt", DbType="datetime")]
		public DateTime? StartDt
		{
			get
			{
				return this._StartDt;
			}

			set
			{
				if (this._StartDt != value)
					this._StartDt = value;
			}

		}

		
		[Column(Name="Trace", Storage="_Trace", DbType="varchar(400)")]
		public string Trace
		{
			get
			{
				return this._Trace;
			}

			set
			{
				if (this._Trace != value)
					this._Trace = value;
			}

		}

		
    }

}
