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
	[Table(Name="SearchNoDiacritics")]
	public partial class SearchNoDiacritic
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private int _FamilyId;
		
		private string _EmailAddress;
		
		private string _EmailAddress2;
		
		private string _CellPhone;
		
		private string _HomePhone;
		
		private string _WorkPhone;
		
		private int? _BirthMonth;
		
		private int? _BirthDay;
		
		private int? _BirthYear;
		
		private string _LastName;
		
		private string _FirstName;
		
		private string _NickName;
		
		private string _MiddleName;
		
		private string _MaidenName;
		
		private string _FirstName2;
		
		
		public SearchNoDiacritic()
		{
		}

		
		
		[Column(Name="PeopleId", Storage="_PeopleId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsDbGenerated=true)]
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

		
		[Column(Name="FamilyId", Storage="_FamilyId", DbType="int NOT NULL")]
		public int FamilyId
		{
			get
			{
				return this._FamilyId;
			}

			set
			{
				if (this._FamilyId != value)
					this._FamilyId = value;
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

		
		[Column(Name="EmailAddress2", Storage="_EmailAddress2", DbType="nvarchar(60)")]
		public string EmailAddress2
		{
			get
			{
				return this._EmailAddress2;
			}

			set
			{
				if (this._EmailAddress2 != value)
					this._EmailAddress2 = value;
			}

		}

		
		[Column(Name="CellPhone", Storage="_CellPhone", DbType="nvarchar(20)")]
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

		
		[Column(Name="HomePhone", Storage="_HomePhone", DbType="nvarchar(20)")]
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

		
		[Column(Name="WorkPhone", Storage="_WorkPhone", DbType="nvarchar(20)")]
		public string WorkPhone
		{
			get
			{
				return this._WorkPhone;
			}

			set
			{
				if (this._WorkPhone != value)
					this._WorkPhone = value;
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

		
		[Column(Name="LastName", Storage="_LastName", DbType="varchar(30)")]
		public string LastName
		{
			get
			{
				return this._LastName;
			}

			set
			{
				if (this._LastName != value)
					this._LastName = value;
			}

		}

		
		[Column(Name="FirstName", Storage="_FirstName", DbType="varchar(30)")]
		public string FirstName
		{
			get
			{
				return this._FirstName;
			}

			set
			{
				if (this._FirstName != value)
					this._FirstName = value;
			}

		}

		
		[Column(Name="NickName", Storage="_NickName", DbType="varchar(30)")]
		public string NickName
		{
			get
			{
				return this._NickName;
			}

			set
			{
				if (this._NickName != value)
					this._NickName = value;
			}

		}

		
		[Column(Name="MiddleName", Storage="_MiddleName", DbType="varchar(30)")]
		public string MiddleName
		{
			get
			{
				return this._MiddleName;
			}

			set
			{
				if (this._MiddleName != value)
					this._MiddleName = value;
			}

		}

		
		[Column(Name="MaidenName", Storage="_MaidenName", DbType="varchar(30)")]
		public string MaidenName
		{
			get
			{
				return this._MaidenName;
			}

			set
			{
				if (this._MaidenName != value)
					this._MaidenName = value;
			}

		}

		
		[Column(Name="FirstName2", Storage="_FirstName2", DbType="varchar(30)")]
		public string FirstName2
		{
			get
			{
				return this._FirstName2;
			}

			set
			{
				if (this._FirstName2 != value)
					this._FirstName2 = value;
			}

		}

		
    }

}
