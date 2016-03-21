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
	[Table(Name="AttendCredits")]
	public partial class AttendCredit
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private int _PeopleId;
		
		private bool? _Attended;
		
		private DateTime? _WeekDate;
		
		private int? _OtherAttends;
		
		
		public AttendCredit()
		{
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

		
		[Column(Name="Attended", Storage="_Attended", DbType="bit")]
		public bool? Attended
		{
			get
			{
				return this._Attended;
			}

			set
			{
				if (this._Attended != value)
					this._Attended = value;
			}

		}

		
		[Column(Name="WeekDate", Storage="_WeekDate", DbType="datetime")]
		public DateTime? WeekDate
		{
			get
			{
				return this._WeekDate;
			}

			set
			{
				if (this._WeekDate != value)
					this._WeekDate = value;
			}

		}

		
		[Column(Name="OtherAttends", Storage="_OtherAttends", DbType="int")]
		public int? OtherAttends
		{
			get
			{
				return this._OtherAttends;
			}

			set
			{
				if (this._OtherAttends != value)
					this._OtherAttends = value;
			}

		}

		
    }

}
