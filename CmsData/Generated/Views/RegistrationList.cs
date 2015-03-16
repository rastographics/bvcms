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
	[Table(Name="RegistrationList")]
	public partial class RegistrationList
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _Id;
		
		private DateTime? _Stamp;
		
		private int? _OrganizationId;
		
		private string _OrganizationName;
		
		private int? _PeopleId;
		
		private string _Name;
		
		private string _Dob;
		
		private string _First;
		
		private string _Last;
		
		private int? _Cnt;
		
		private bool? _Mobile;
		
		private bool? _Completed;
		
		private bool? _Abandoned;
		
		private int? _UserPeopleId;
		
		private bool? _Expired;
		
		private DateTime? _RegStart;
		
		private DateTime? _RegEnd;
		
		
		public RegistrationList()
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

		
		[Column(Name="OrganizationId", Storage="_OrganizationId", DbType="int")]
		public int? OrganizationId
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

		
		[Column(Name="OrganizationName", Storage="_OrganizationName", DbType="nvarchar(100)")]
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

		
		[Column(Name="dob", Storage="_Dob", DbType="varchar(50)")]
		public string Dob
		{
			get
			{
				return this._Dob;
			}

			set
			{
				if (this._Dob != value)
					this._Dob = value;
			}

		}

		
		[Column(Name="first", Storage="_First", DbType="varchar(50)")]
		public string First
		{
			get
			{
				return this._First;
			}

			set
			{
				if (this._First != value)
					this._First = value;
			}

		}

		
		[Column(Name="last", Storage="_Last", DbType="varchar(50)")]
		public string Last
		{
			get
			{
				return this._Last;
			}

			set
			{
				if (this._Last != value)
					this._Last = value;
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

		
		[Column(Name="mobile", Storage="_Mobile", DbType="bit")]
		public bool? Mobile
		{
			get
			{
				return this._Mobile;
			}

			set
			{
				if (this._Mobile != value)
					this._Mobile = value;
			}

		}

		
		[Column(Name="completed", Storage="_Completed", DbType="bit")]
		public bool? Completed
		{
			get
			{
				return this._Completed;
			}

			set
			{
				if (this._Completed != value)
					this._Completed = value;
			}

		}

		
		[Column(Name="abandoned", Storage="_Abandoned", DbType="bit")]
		public bool? Abandoned
		{
			get
			{
				return this._Abandoned;
			}

			set
			{
				if (this._Abandoned != value)
					this._Abandoned = value;
			}

		}

		
		[Column(Name="UserPeopleId", Storage="_UserPeopleId", DbType="int")]
		public int? UserPeopleId
		{
			get
			{
				return this._UserPeopleId;
			}

			set
			{
				if (this._UserPeopleId != value)
					this._UserPeopleId = value;
			}

		}

		
		[Column(Name="expired", Storage="_Expired", DbType="bit")]
		public bool? Expired
		{
			get
			{
				return this._Expired;
			}

			set
			{
				if (this._Expired != value)
					this._Expired = value;
			}

		}

		
		[Column(Name="RegStart", Storage="_RegStart", DbType="datetime")]
		public DateTime? RegStart
		{
			get
			{
				return this._RegStart;
			}

			set
			{
				if (this._RegStart != value)
					this._RegStart = value;
			}

		}

		
		[Column(Name="RegEnd", Storage="_RegEnd", DbType="datetime")]
		public DateTime? RegEnd
		{
			get
			{
				return this._RegEnd;
			}

			set
			{
				if (this._RegEnd != value)
					this._RegEnd = value;
			}

		}

		
    }

}
