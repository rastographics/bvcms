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
	[Table(Name="RecentRegistrations")]
	public partial class RecentRegistration
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private DateTime? _Dt1;
		
		private DateTime? _Dt2;
		
		private int? _Cnt;
		
		private int _OrganizationId;
		
		private string _OrganizationName;
		
		
		public RecentRegistration()
		{
		}

		
		
		[Column(Name="dt1", Storage="_Dt1", DbType="datetime")]
		public DateTime? Dt1
		{
			get
			{
				return this._Dt1;
			}

			set
			{
				if (this._Dt1 != value)
					this._Dt1 = value;
			}

		}

		
		[Column(Name="dt2", Storage="_Dt2", DbType="datetime")]
		public DateTime? Dt2
		{
			get
			{
				return this._Dt2;
			}

			set
			{
				if (this._Dt2 != value)
					this._Dt2 = value;
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

		
    }

}
