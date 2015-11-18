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
	[Table(Name="VolunteerTimes")]
	public partial class VolunteerTime
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private string _Time;
		
		private int? _DayOfWeek;
		
		
		public VolunteerTime()
		{
		}

		
		
		[Column(Name="OrganizationId", Storage="_OrganizationId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsDbGenerated=true)]
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

		
		[Column(Name="Time", Storage="_Time", DbType="varchar(50)")]
		public string Time
		{
			get
			{
				return this._Time;
			}

			set
			{
				if (this._Time != value)
					this._Time = value;
			}

		}

		
		[Column(Name="DayOfWeek", Storage="_DayOfWeek", DbType="int")]
		public int? DayOfWeek
		{
			get
			{
				return this._DayOfWeek;
			}

			set
			{
				if (this._DayOfWeek != value)
					this._DayOfWeek = value;
			}

		}

		
    }

}
