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
	[Table(Name="XpOrgSchedule")]
	public partial class XpOrgSchedule
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _Id;
		
		private int _OrganizationId;
		
		private string _SchedTime;
		
		private int? _SchedDay;
		
		
		public XpOrgSchedule()
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

		
		[Column(Name="OrganizationId", Storage="_OrganizationId", DbType="int NOT NULL")]
		public int OrganizationId
		{
			get
			{
				return this._OrganizationId;
			}

			set
			{
				if (this._OrganizationId != value)
					this._OrganizationId = value;
			}

		}

		
		[Column(Name="SchedTime", Storage="_SchedTime", DbType="nvarchar(4000)")]
		public string SchedTime
		{
			get
			{
				return this._SchedTime;
			}

			set
			{
				if (this._SchedTime != value)
					this._SchedTime = value;
			}

		}

		
		[Column(Name="SchedDay", Storage="_SchedDay", DbType="int")]
		public int? SchedDay
		{
			get
			{
				return this._SchedDay;
			}

			set
			{
				if (this._SchedDay != value)
					this._SchedDay = value;
			}

		}

		
    }

}
