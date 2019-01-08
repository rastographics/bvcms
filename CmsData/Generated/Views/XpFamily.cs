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
	[Table(Name="XpFamily")]
	public partial class XpFamily
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _FamilyId;
		
		private DateTime? _AddressFromDate;
		
		private DateTime? _AddressToDate;
		
		private string _AddressLineOne;
		
		private string _AddressLineTwo;
		
		private string _CityName;
		
		private string _StateCode;
		
		private string _ZipCode;
		
		private string _CountryName;
		
		private string _ResCode;
		
		private string _HomePhone;
		
		private int? _HeadOfHouseholdId;
		
		private int? _HeadOfHouseholdSpouseId;
		
		private int? _CoupleFlag;
		
		private string _Comments;
		
		
		public XpFamily()
		{
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

		
		[Column(Name="AddressFromDate", Storage="_AddressFromDate", DbType="datetime")]
		public DateTime? AddressFromDate
		{
			get
			{
				return this._AddressFromDate;
			}

			set
			{
				if (this._AddressFromDate != value)
					this._AddressFromDate = value;
			}

		}

		
		[Column(Name="AddressToDate", Storage="_AddressToDate", DbType="datetime")]
		public DateTime? AddressToDate
		{
			get
			{
				return this._AddressToDate;
			}

			set
			{
				if (this._AddressToDate != value)
					this._AddressToDate = value;
			}

		}

		
		[Column(Name="AddressLineOne", Storage="_AddressLineOne", DbType="nvarchar(100)")]
		public string AddressLineOne
		{
			get
			{
				return this._AddressLineOne;
			}

			set
			{
				if (this._AddressLineOne != value)
					this._AddressLineOne = value;
			}

		}

		
		[Column(Name="AddressLineTwo", Storage="_AddressLineTwo", DbType="nvarchar(100)")]
		public string AddressLineTwo
		{
			get
			{
				return this._AddressLineTwo;
			}

			set
			{
				if (this._AddressLineTwo != value)
					this._AddressLineTwo = value;
			}

		}

		
		[Column(Name="CityName", Storage="_CityName", DbType="nvarchar(30)")]
		public string CityName
		{
			get
			{
				return this._CityName;
			}

			set
			{
				if (this._CityName != value)
					this._CityName = value;
			}

		}

		
		[Column(Name="StateCode", Storage="_StateCode", DbType="nvarchar(30)")]
		public string StateCode
		{
			get
			{
				return this._StateCode;
			}

			set
			{
				if (this._StateCode != value)
					this._StateCode = value;
			}

		}

		
		[Column(Name="ZipCode", Storage="_ZipCode", DbType="nvarchar(15)")]
		public string ZipCode
		{
			get
			{
				return this._ZipCode;
			}

			set
			{
				if (this._ZipCode != value)
					this._ZipCode = value;
			}

		}

		
		[Column(Name="CountryName", Storage="_CountryName", DbType="nvarchar(40)")]
		public string CountryName
		{
			get
			{
				return this._CountryName;
			}

			set
			{
				if (this._CountryName != value)
					this._CountryName = value;
			}

		}

		
		[Column(Name="ResCode", Storage="_ResCode", DbType="nvarchar(100)")]
		public string ResCode
		{
			get
			{
				return this._ResCode;
			}

			set
			{
				if (this._ResCode != value)
					this._ResCode = value;
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

		
		[Column(Name="HeadOfHouseholdSpouseId", Storage="_HeadOfHouseholdSpouseId", DbType="int")]
		public int? HeadOfHouseholdSpouseId
		{
			get
			{
				return this._HeadOfHouseholdSpouseId;
			}

			set
			{
				if (this._HeadOfHouseholdSpouseId != value)
					this._HeadOfHouseholdSpouseId = value;
			}

		}

		
		[Column(Name="CoupleFlag", Storage="_CoupleFlag", DbType="int")]
		public int? CoupleFlag
		{
			get
			{
				return this._CoupleFlag;
			}

			set
			{
				if (this._CoupleFlag != value)
					this._CoupleFlag = value;
			}

		}

		
		[Column(Name="Comments", Storage="_Comments", DbType="nvarchar(3000)")]
		public string Comments
		{
			get
			{
				return this._Comments;
			}

			set
			{
				if (this._Comments != value)
					this._Comments = value;
			}

		}

		
    }

}
