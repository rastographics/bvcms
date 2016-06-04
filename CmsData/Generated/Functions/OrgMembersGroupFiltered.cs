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
	[Table(Name="OrgMembersGroupFiltered")]
	public partial class OrgMembersGroupFiltered
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int? _PeopleId;
		
		private int? _OrganizationId;
		
		private int? _Age;
		
		private int? _Grade;
		
		private int? _MemberTypeId;
		
		private string _MemberType;
		
		private int? _BirthYear;
		
		private int? _BirthMonth;
		
		private int? _BirthDay;
		
		private string _OrganizationName;
		
		private string _Name2;
		
		private string _Name;
		
		private string _Gender;
		
		private int? _HashNum;
		
		private string _Request;
		
		private string _Groups;
		
		
		public OrgMembersGroupFiltered()
		{
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

		
		[Column(Name="Age", Storage="_Age", DbType="int")]
		public int? Age
		{
			get
			{
				return this._Age;
			}

			set
			{
				if (this._Age != value)
					this._Age = value;
			}

		}

		
		[Column(Name="Grade", Storage="_Grade", DbType="int")]
		public int? Grade
		{
			get
			{
				return this._Grade;
			}

			set
			{
				if (this._Grade != value)
					this._Grade = value;
			}

		}

		
		[Column(Name="MemberTypeId", Storage="_MemberTypeId", DbType="int")]
		public int? MemberTypeId
		{
			get
			{
				return this._MemberTypeId;
			}

			set
			{
				if (this._MemberTypeId != value)
					this._MemberTypeId = value;
			}

		}

		
		[Column(Name="MemberType", Storage="_MemberType", DbType="varchar(100)")]
		public string MemberType
		{
			get
			{
				return this._MemberType;
			}

			set
			{
				if (this._MemberType != value)
					this._MemberType = value;
			}

		}

		
		[Column(Name="BirthYear", Storage="_BirthYear", DbType="int")]
		public int? BirthYear
		{
			get
			{
				return this._BirthYear;
			}

			set
			{
				if (this._BirthYear != value)
					this._BirthYear = value;
			}

		}

		
		[Column(Name="BirthMonth", Storage="_BirthMonth", DbType="int")]
		public int? BirthMonth
		{
			get
			{
				return this._BirthMonth;
			}

			set
			{
				if (this._BirthMonth != value)
					this._BirthMonth = value;
			}

		}

		
		[Column(Name="BirthDay", Storage="_BirthDay", DbType="int")]
		public int? BirthDay
		{
			get
			{
				return this._BirthDay;
			}

			set
			{
				if (this._BirthDay != value)
					this._BirthDay = value;
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

		
		[Column(Name="Name2", Storage="_Name2", DbType="nvarchar(200)")]
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

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(200)")]
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

		
		[Column(Name="Gender", Storage="_Gender", DbType="varchar(10)")]
		public string Gender
		{
			get
			{
				return this._Gender;
			}

			set
			{
				if (this._Gender != value)
					this._Gender = value;
			}

		}

		
		[Column(Name="HashNum", Storage="_HashNum", DbType="int")]
		public int? HashNum
		{
			get
			{
				return this._HashNum;
			}

			set
			{
				if (this._HashNum != value)
					this._HashNum = value;
			}

		}

		
		[Column(Name="Request", Storage="_Request", DbType="nvarchar(140)")]
		public string Request
		{
			get
			{
				return this._Request;
			}

			set
			{
				if (this._Request != value)
					this._Request = value;
			}

		}

		
		[Column(Name="Groups", Storage="_Groups", DbType="nvarchar")]
		public string Groups
		{
			get
			{
				return this._Groups;
			}

			set
			{
				if (this._Groups != value)
					this._Groups = value;
			}

		}

		
    }

}
