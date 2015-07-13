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
	[Table(Name="DownlineLevels")]
	public partial class DownlineLevel
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _Level;
		
		private string _OrganizationName;
		
		private string _Leader;
		
		private int? _OrgId;
		
		private int? _LeaderId;
		
		private int? _Cnt;
		
		private DateTime? _StartDt;
		
		private DateTime? _EndDt;
		
		private int? _MaxRows;
		
		
		public DownlineLevel()
		{
		}

		
		
		[Column(Name="Level", Storage="_Level", DbType="int")]
		public int? Level
		{
			get
			{
				return this._Level;
			}

			set
			{
				if (this._Level != value)
					this._Level = value;
			}

		}

		
		[Column(Name="OrganizationName", Storage="_OrganizationName", DbType="nvarchar(100) NOT NULL")]
		public string OrganizationName
		{
			get
			{
				return this._OrganizationName;
			}

			set
			{
				if (this._OrganizationName != value)
					this._OrganizationName = value;
			}

		}

		
		[Column(Name="Leader", Storage="_Leader", DbType="nvarchar(139)")]
		public string Leader
		{
			get
			{
				return this._Leader;
			}

			set
			{
				if (this._Leader != value)
					this._Leader = value;
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

		
		[Column(Name="EndDt", Storage="_EndDt", DbType="datetime")]
		public DateTime? EndDt
		{
			get
			{
				return this._EndDt;
			}

			set
			{
				if (this._EndDt != value)
					this._EndDt = value;
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
