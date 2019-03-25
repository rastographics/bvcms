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
	[Table(Name="Donors")]
	public partial class Donor
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private string _PrimaryAddress;
		
		private string _PrimaryAddress2;
		
		private string _PrimaryCity;
		
		private string _PrimaryState;
		
		private string _PrimaryZip;
		
		private int _FamilyId;
		
		private int _PeopleId;
		
		private string _LastName;
		
		private string _Name;
		
		private string _Title;
		
		private string _Suffix;
		
		private int? _ContributionOptionsId;
		
		private DateTime? _DeceasedDate;
		
		private int? _Age;
		
		private int _PositionInFamilyId;
		
		private int _HohFlag;
		
		private decimal _Amount;
		
		private int _GiftCount;
		
		private bool? _GiftInKind;
		
		private string _SpouseName;

        private string _SpouseNameWithoutNickname;
		
		private string _SpouseTitle;
		
		private int? _SpouseId;
		
		private int? _SpouseContributionOptionsId;
		
		private decimal _SpouseAmount;
		
		private int? _CampusId;
		
		private string _HouseName;
		
		private bool _ElectronicStatement;
		
		private string _MailingAddress;
		
		private string _CoupleName;
		
		
		public Donor()
		{
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

		
		[Column(Name="PrimaryAddress2", Storage="_PrimaryAddress2", DbType="nvarchar(100)")]
		public string PrimaryAddress2
		{
			get
			{
				return this._PrimaryAddress2;
			}

			set
			{
				if (this._PrimaryAddress2 != value)
					this._PrimaryAddress2 = value;
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

		
		[Column(Name="Name", Storage="_Name", DbType="nvarchar(126) NOT NULL")]
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

		
		[Column(Name="Title", Storage="_Title", DbType="nvarchar(10)")]
		public string Title
		{
			get
			{
				return this._Title;
			}

			set
			{
				if (this._Title != value)
					this._Title = value;
			}

		}

		
		[Column(Name="Suffix", Storage="_Suffix", DbType="nvarchar(10)")]
		public string Suffix
		{
			get
			{
				return this._Suffix;
			}

			set
			{
				if (this._Suffix != value)
					this._Suffix = value;
			}

		}

		
		[Column(Name="ContributionOptionsId", Storage="_ContributionOptionsId", DbType="int")]
		public int? ContributionOptionsId
		{
			get
			{
				return this._ContributionOptionsId;
			}

			set
			{
				if (this._ContributionOptionsId != value)
					this._ContributionOptionsId = value;
			}

		}

		
		[Column(Name="DeceasedDate", Storage="_DeceasedDate", DbType="datetime")]
		public DateTime? DeceasedDate
		{
			get
			{
				return this._DeceasedDate;
			}

			set
			{
				if (this._DeceasedDate != value)
					this._DeceasedDate = value;
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

		
		[Column(Name="hohFlag", Storage="_HohFlag", DbType="int NOT NULL")]
		public int HohFlag
		{
			get
			{
				return this._HohFlag;
			}

			set
			{
				if (this._HohFlag != value)
					this._HohFlag = value;
			}

		}

		
		[Column(Name="Amount", Storage="_Amount", DbType="Decimal(38,2) NOT NULL")]
		public decimal Amount
		{
			get
			{
				return this._Amount;
			}

			set
			{
				if (this._Amount != value)
					this._Amount = value;
			}

		}

		
		[Column(Name="GiftCount", Storage="_GiftCount", DbType="int NOT NULL")]
		public int GiftCount
		{
			get
			{
				return this._GiftCount;
			}

			set
			{
				if (this._GiftCount != value)
					this._GiftCount = value;
			}

		}

		
		[Column(Name="GiftInKind", Storage="_GiftInKind", DbType="bit")]
		public bool? GiftInKind
		{
			get
			{
				return this._GiftInKind;
			}

			set
			{
				if (this._GiftInKind != value)
					this._GiftInKind = value;
			}

		}

		
		[Column(Name="SpouseName", Storage="_SpouseName", DbType="nvarchar(138)")]
		public string SpouseName
		{
			get
			{
				return this._SpouseName;
			}

			set
			{
				if (this._SpouseName != value)
					this._SpouseName = value;
			}

		}


        [Column(Name = "SpouseNameWithoutNickname", Storage = "_SpouseNameWithoutNickname", DbType = "nvarchar(138)")]
        public string SpouseNameWithoutNickname
        {
            get
            {
                return this._SpouseNameWithoutNickname;
            }

            set
            {
                if (this._SpouseNameWithoutNickname != value)
                    this._SpouseNameWithoutNickname = value;
            }

        }



        [Column(Name="SpouseTitle", Storage="_SpouseTitle", DbType="nvarchar(10)")]
		public string SpouseTitle
		{
			get
			{
				return this._SpouseTitle;
			}

			set
			{
				if (this._SpouseTitle != value)
					this._SpouseTitle = value;
			}

		}

		
		[Column(Name="SpouseId", Storage="_SpouseId", DbType="int")]
		public int? SpouseId
		{
			get
			{
				return this._SpouseId;
			}

			set
			{
				if (this._SpouseId != value)
					this._SpouseId = value;
			}

		}

		
		[Column(Name="SpouseContributionOptionsId", Storage="_SpouseContributionOptionsId", DbType="int")]
		public int? SpouseContributionOptionsId
		{
			get
			{
				return this._SpouseContributionOptionsId;
			}

			set
			{
				if (this._SpouseContributionOptionsId != value)
					this._SpouseContributionOptionsId = value;
			}

		}

		
		[Column(Name="SpouseAmount", Storage="_SpouseAmount", DbType="Decimal(38,2) NOT NULL")]
		public decimal SpouseAmount
		{
			get
			{
				return this._SpouseAmount;
			}

			set
			{
				if (this._SpouseAmount != value)
					this._SpouseAmount = value;
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

		
		[Column(Name="HouseName", Storage="_HouseName", DbType="nvarchar(100)")]
		public string HouseName
		{
			get
			{
				return this._HouseName;
			}

			set
			{
				if (this._HouseName != value)
					this._HouseName = value;
			}

		}

		
		[Column(Name="ElectronicStatement", Storage="_ElectronicStatement", DbType="bit NOT NULL")]
		public bool ElectronicStatement
		{
			get
			{
				return this._ElectronicStatement;
			}

			set
			{
				if (this._ElectronicStatement != value)
					this._ElectronicStatement = value;
			}

		}

		
		[Column(Name="MailingAddress", Storage="_MailingAddress", DbType="nvarchar")]
		public string MailingAddress
		{
			get
			{
				return this._MailingAddress;
			}

			set
			{
				if (this._MailingAddress != value)
					this._MailingAddress = value;
			}

		}

		
		[Column(Name="CoupleName", Storage="_CoupleName", DbType="nvarchar")]
		public string CoupleName
		{
			get
			{
				return this._CoupleName;
			}

			set
			{
				if (this._CoupleName != value)
					this._CoupleName = value;
			}

		}

		
    }

}
