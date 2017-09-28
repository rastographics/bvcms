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
	[Table(Name="ChAiIndividualData")]
	public partial class ChAiIndividualDatum
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private int _FamilyId;
		
		private DateTime? _CreatedDate;
		
		private int? _HeadOfHouseholdId;
		
		private DateTime? _FamilyCreatedDate;
		
		private string _MemberStatus;
		
		private string _Salutation;
		
		private string _FirstName;
		
		private string _LastName;
		
		private string _FamilyPosition;
		
		private string _MaritalStatus;
		
		private string _Gender;
		
		private string _Address1;
		
		private string _Address2;
		
		private string _City;
		
		private string _State;
		
		private string _Zip;
		
		private string _HomePhone;
		
		private string _EmailAddress;
		
		private string _BirthDate;
		
		private DateTime? _LastModified;
		
		
		public ChAiIndividualDatum()
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

		
		[Column(Name="CreatedDate", Storage="_CreatedDate", DbType="datetime")]
		public DateTime? CreatedDate
		{
			get
			{
				return this._CreatedDate;
			}

			set
			{
				if (this._CreatedDate != value)
					this._CreatedDate = value;
			}

		}

		
		[Column(Name="HeadOfHouseholdId", Storage="_HeadOfHouseholdId", DbType="int")]
		public int? HeadOfHouseholdId
		{
			get
			{
				return this._HeadOfHouseholdId;
			}

			set
			{
				if (this._HeadOfHouseholdId != value)
					this._HeadOfHouseholdId = value;
			}

		}

		
		[Column(Name="FamilyCreatedDate", Storage="_FamilyCreatedDate", DbType="datetime")]
		public DateTime? FamilyCreatedDate
		{
			get
			{
				return this._FamilyCreatedDate;
			}

			set
			{
				if (this._FamilyCreatedDate != value)
					this._FamilyCreatedDate = value;
			}

		}

		
		[Column(Name="MemberStatus", Storage="_MemberStatus", DbType="nvarchar(50)")]
		public string MemberStatus
		{
			get
			{
				return this._MemberStatus;
			}

			set
			{
				if (this._MemberStatus != value)
					this._MemberStatus = value;
			}

		}

		
		[Column(Name="Salutation", Storage="_Salutation", DbType="nvarchar(10)")]
		public string Salutation
		{
			get
			{
				return this._Salutation;
			}

			set
			{
				if (this._Salutation != value)
					this._Salutation = value;
			}

		}

		
		[Column(Name="FirstName", Storage="_FirstName", DbType="nvarchar(25)")]
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

		
		[Column(Name="LastName", Storage="_LastName", DbType="nvarchar(100) NOT NULL")]
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

		
		[Column(Name="FamilyPosition", Storage="_FamilyPosition", DbType="nvarchar(100)")]
		public string FamilyPosition
		{
			get
			{
				return this._FamilyPosition;
			}

			set
			{
				if (this._FamilyPosition != value)
					this._FamilyPosition = value;
			}

		}

		
		[Column(Name="MaritalStatus", Storage="_MaritalStatus", DbType="nvarchar(100)")]
		public string MaritalStatus
		{
			get
			{
				return this._MaritalStatus;
			}

			set
			{
				if (this._MaritalStatus != value)
					this._MaritalStatus = value;
			}

		}

		
		[Column(Name="Gender", Storage="_Gender", DbType="nvarchar(100)")]
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

		
		[Column(Name="Address1", Storage="_Address1", DbType="nvarchar(100)")]
		public string Address1
		{
			get
			{
				return this._Address1;
			}

			set
			{
				if (this._Address1 != value)
					this._Address1 = value;
			}

		}

		
		[Column(Name="Address2", Storage="_Address2", DbType="nvarchar(100)")]
		public string Address2
		{
			get
			{
				return this._Address2;
			}

			set
			{
				if (this._Address2 != value)
					this._Address2 = value;
			}

		}

		
		[Column(Name="City", Storage="_City", DbType="nvarchar(30)")]
		public string City
		{
			get
			{
				return this._City;
			}

			set
			{
				if (this._City != value)
					this._City = value;
			}

		}

		
		[Column(Name="State", Storage="_State", DbType="nvarchar(20)")]
		public string State
		{
			get
			{
				return this._State;
			}

			set
			{
				if (this._State != value)
					this._State = value;
			}

		}

		
		[Column(Name="Zip", Storage="_Zip", DbType="nvarchar(5)")]
		public string Zip
		{
			get
			{
				return this._Zip;
			}

			set
			{
				if (this._Zip != value)
					this._Zip = value;
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

		
		[Column(Name="BirthDate", Storage="_BirthDate", DbType="varchar(20)")]
		public string BirthDate
		{
			get
			{
				return this._BirthDate;
			}

			set
			{
				if (this._BirthDate != value)
					this._BirthDate = value;
			}

		}

		
		[Column(Name="LastModified", Storage="_LastModified", DbType="datetime")]
		public DateTime? LastModified
		{
			get
			{
				return this._LastModified;
			}

			set
			{
				if (this._LastModified != value)
					this._LastModified = value;
			}

		}

		
    }

}
