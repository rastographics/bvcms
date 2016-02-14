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
	[Table(Name="InProgressRegistrations")]
	public partial class InProgressRegistration
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _Name;
		
		private string _OrganizationName;
		
		private DateTime? _Stamp;
		
		private int _PeopleId;
		
		private int _OrganizationId;
		
		private int _RegDataId;
		
		
		public InProgressRegistration()
		{
		}

		
		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(138)")]
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

		
		[Column(Name="Stamp", Storage="_Stamp", DbType="datetime")]
		public DateTime? Stamp
		{
			get
			{
				return this._Stamp;
			}

			set
			{
				if (this._Stamp != value)
					this._Stamp = value;
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

		
		[Column(Name="RegDataId", Storage="_RegDataId", DbType="int NOT NULL")]
		public int RegDataId
		{
			get
			{
				return this._RegDataId;
			}

			set
			{
				if (this._RegDataId != value)
					this._RegDataId = value;
			}

		}

		
    }

}
