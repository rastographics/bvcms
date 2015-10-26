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
	[Table(Name="LastAttends")]
	public partial class LastAttend
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private string _Name2;
		
		private int _OrganizationId;
		
		private string _OrganizationName;
		
		private string _LastAttendX;
		
		private string _HomePhone;
		
		private string _CellPhone;
		
		private string _EmailAddress;
		
		private bool? _HasHomePhone;
		
		private bool? _HasCellPhone;
		
		private bool? _HasEmail;
		
		
		public LastAttend()
		{
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

		
		[Column(Name="Name2", Storage="_Name2", DbType="nvarchar(139)")]
		public string Name2
		{
			get
			{
				return this._Name2;
			}

			set
			{
				if (this._Name2 != value)
					this._Name2 = value;
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

		
		[Column(Name="LastAttend", Storage="_LastAttendX", DbType="nvarchar(4000)")]
		public string LastAttendX
		{
			get
			{
				return this._LastAttendX;
			}

			set
			{
				if (this._LastAttendX != value)
					this._LastAttendX = value;
			}

		}

		
		[Column(Name="HomePhone", Storage="_HomePhone", DbType="nvarchar(32)")]
		public string HomePhone
		{
			get
			{
				return this._HomePhone;
			}

			set
			{
				if (this._HomePhone != value)
					this._HomePhone = value;
			}

		}

		
		[Column(Name="CellPhone", Storage="_CellPhone", DbType="nvarchar(32)")]
		public string CellPhone
		{
			get
			{
				return this._CellPhone;
			}

			set
			{
				if (this._CellPhone != value)
					this._CellPhone = value;
			}

		}

		
		[Column(Name="EmailAddress", Storage="_EmailAddress", DbType="nvarchar(150)")]
		public string EmailAddress
		{
			get
			{
				return this._EmailAddress;
			}

			set
			{
				if (this._EmailAddress != value)
					this._EmailAddress = value;
			}

		}

		
		[Column(Name="HasHomePhone", Storage="_HasHomePhone", DbType="bit")]
		public bool? HasHomePhone
		{
			get
			{
				return this._HasHomePhone;
			}

			set
			{
				if (this._HasHomePhone != value)
					this._HasHomePhone = value;
			}

		}

		
		[Column(Name="HasCellPhone", Storage="_HasCellPhone", DbType="bit")]
		public bool? HasCellPhone
		{
			get
			{
				return this._HasCellPhone;
			}

			set
			{
				if (this._HasCellPhone != value)
					this._HasCellPhone = value;
			}

		}

		
		[Column(Name="HasEmail", Storage="_HasEmail", DbType="bit")]
		public bool? HasEmail
		{
			get
			{
				return this._HasEmail;
			}

			set
			{
				if (this._HasEmail != value)
					this._HasEmail = value;
			}

		}

		
    }

}
