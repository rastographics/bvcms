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
	[Table(Name="PeopleBasicModifed")]
	public partial class PeopleBasicModifed
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _PeopleId;
		
		private int _FamilyId;
		
		private int _PositionInFamilyId;
		
		private int? _CampusId;
		
		private string _FirstName;
		
		private string _LastName;
		
		private int _GenderId;
		
		private DateTime? _BirthDate;
		
		private string _EmailAddress;
		
		private int _MaritalStatusId;
		
		private string _PrimaryAddress;
		
		private string _PrimaryCity;
		
		private string _PrimaryState;
		
		private string _PrimaryZip;
		
		private string _HomePhone;
		
		private string _WorkPhone;
		
		private string _CellPhone;
		
		private DateTime? _CreatedDate;
		
		private DateTime? _ModifiedDate;
		
		private bool? _IsDeceased;
		
		
		public PeopleBasicModifed()
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

		
		[Column(Name="PositionInFamilyId", Storage="_PositionInFamilyId", DbType="int NOT NULL")]
		public int PositionInFamilyId
		{
			get
			{
				return this._PositionInFamilyId;
			}

			set
			{
				if (this._PositionInFamilyId != value)
					this._PositionInFamilyId = value;
			}

		}

		
		[Column(Name="CampusId", Storage="_CampusId", DbType="int")]
		public int? CampusId
		{
			get
			{
				return this._CampusId;
			}

			set
			{
				if (this._CampusId != value)
					this._CampusId = value;
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

		
		[Column(Name="GenderId", Storage="_GenderId", DbType="int NOT NULL")]
		public int GenderId
		{
			get
			{
				return this._GenderId;
			}

			set
			{
				if (this._GenderId != value)
					this._GenderId = value;
			}

		}

		
		[Column(Name="BirthDate", Storage="_BirthDate", DbType="datetime")]
		public DateTime? BirthDate
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

		
		[Column(Name="MaritalStatusId", Storage="_MaritalStatusId", DbType="int NOT NULL")]
		public int MaritalStatusId
		{
			get
			{
				return this._MaritalStatusId;
			}

			set
			{
				if (this._MaritalStatusId != value)
					this._MaritalStatusId = value;
			}

		}

		
		[Column(Name="PrimaryAddress", Storage="_PrimaryAddress", DbType="nvarchar(100)")]
		public string PrimaryAddress
		{
			get
			{
				return this._PrimaryAddress;
			}

			set
			{
				if (this._PrimaryAddress != value)
					this._PrimaryAddress = value;
			}

		}

		
		[Column(Name="PrimaryCity", Storage="_PrimaryCity", DbType="nvarchar(30)")]
		public string PrimaryCity
		{
			get
			{
				return this._PrimaryCity;
			}

			set
			{
				if (this._PrimaryCity != value)
					this._PrimaryCity = value;
			}

		}

		
		[Column(Name="PrimaryState", Storage="_PrimaryState", DbType="nvarchar(20)")]
		public string PrimaryState
		{
			get
			{
				return this._PrimaryState;
			}

			set
			{
				if (this._PrimaryState != value)
					this._PrimaryState = value;
			}

		}

		
		[Column(Name="PrimaryZip", Storage="_PrimaryZip", DbType="nvarchar(15)")]
		public string PrimaryZip
		{
			get
			{
				return this._PrimaryZip;
			}

			set
			{
				if (this._PrimaryZip != value)
					this._PrimaryZip = value;
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

		
		[Column(Name="ModifiedDate", Storage="_ModifiedDate", DbType="datetime")]
		public DateTime? ModifiedDate
		{
			get
			{
				return this._ModifiedDate;
			}

			set
			{
				if (this._ModifiedDate != value)
					this._ModifiedDate = value;
			}

		}

		
		[Column(Name="IsDeceased", Storage="_IsDeceased", DbType="bit")]
		public bool? IsDeceased
		{
			get
			{
				return this._IsDeceased;
			}

			set
			{
				if (this._IsDeceased != value)
					this._IsDeceased = value;
			}

		}

		
    }

}
