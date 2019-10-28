using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "Contributors")]
    public partial class Contributor
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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

        private string _SpouseTitle;

        private int? _SpouseId;

        private int? _SpouseContributionOptionsId;

        private decimal _SpouseAmount;

        private int? _CampusId;

        private string _HouseName;

        private bool _ElectronicStatement;

        private string _MailingAddress;

        private string _CoupleName;

        public Contributor()
        {
        }

        [Column(Name = "PrimaryAddress", Storage = "_PrimaryAddress", DbType = "nvarchar(100)")]
        public string PrimaryAddress
        {
            get => _PrimaryAddress;

            set
            {
                if (_PrimaryAddress != value)
                {
                    _PrimaryAddress = value;
                }
            }
        }

        [Column(Name = "PrimaryAddress2", Storage = "_PrimaryAddress2", DbType = "nvarchar(100)")]
        public string PrimaryAddress2
        {
            get => _PrimaryAddress2;

            set
            {
                if (_PrimaryAddress2 != value)
                {
                    _PrimaryAddress2 = value;
                }
            }
        }

        [Column(Name = "PrimaryCity", Storage = "_PrimaryCity", DbType = "nvarchar(30)")]
        public string PrimaryCity
        {
            get => _PrimaryCity;

            set
            {
                if (_PrimaryCity != value)
                {
                    _PrimaryCity = value;
                }
            }
        }

        [Column(Name = "PrimaryState", Storage = "_PrimaryState", DbType = "nvarchar(20)")]
        public string PrimaryState
        {
            get => _PrimaryState;

            set
            {
                if (_PrimaryState != value)
                {
                    _PrimaryState = value;
                }
            }
        }

        [Column(Name = "PrimaryZip", Storage = "_PrimaryZip", DbType = "nvarchar(15)")]
        public string PrimaryZip
        {
            get => _PrimaryZip;

            set
            {
                if (_PrimaryZip != value)
                {
                    _PrimaryZip = value;
                }
            }
        }

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int NOT NULL")]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    _FamilyId = value;
                }
            }
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int NOT NULL")]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    _PeopleId = value;
                }
            }
        }

        [Column(Name = "LastName", Storage = "_LastName", DbType = "nvarchar(100) NOT NULL")]
        public string LastName
        {
            get => _LastName;

            set
            {
                if (_LastName != value)
                {
                    _LastName = value;
                }
            }
        }

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(126)")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    _Name = value;
                }
            }
        }

        [Column(Name = "Title", Storage = "_Title", DbType = "nvarchar(10)")]
        public string Title
        {
            get => _Title;

            set
            {
                if (_Title != value)
                {
                    _Title = value;
                }
            }
        }

        [Column(Name = "Suffix", Storage = "_Suffix", DbType = "nvarchar(10)")]
        public string Suffix
        {
            get => _Suffix;

            set
            {
                if (_Suffix != value)
                {
                    _Suffix = value;
                }
            }
        }

        [Column(Name = "ContributionOptionsId", Storage = "_ContributionOptionsId", DbType = "int")]
        public int? ContributionOptionsId
        {
            get => _ContributionOptionsId;

            set
            {
                if (_ContributionOptionsId != value)
                {
                    _ContributionOptionsId = value;
                }
            }
        }

        [Column(Name = "DeceasedDate", Storage = "_DeceasedDate", DbType = "datetime")]
        public DateTime? DeceasedDate
        {
            get => _DeceasedDate;

            set
            {
                if (_DeceasedDate != value)
                {
                    _DeceasedDate = value;
                }
            }
        }

        [Column(Name = "Age", Storage = "_Age", DbType = "int")]
        public int? Age
        {
            get => _Age;

            set
            {
                if (_Age != value)
                {
                    _Age = value;
                }
            }
        }

        [Column(Name = "PositionInFamilyId", Storage = "_PositionInFamilyId", DbType = "int NOT NULL")]
        public int PositionInFamilyId
        {
            get => _PositionInFamilyId;

            set
            {
                if (_PositionInFamilyId != value)
                {
                    _PositionInFamilyId = value;
                }
            }
        }

        [Column(Name = "hohFlag", Storage = "_HohFlag", DbType = "int NOT NULL")]
        public int HohFlag
        {
            get => _HohFlag;

            set
            {
                if (_HohFlag != value)
                {
                    _HohFlag = value;
                }
            }
        }

        [Column(Name = "Amount", Storage = "_Amount", DbType = "Decimal(38,2) NOT NULL")]
        public decimal Amount
        {
            get => _Amount;

            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                }
            }
        }

        [Column(Name = "GiftCount", Storage = "_GiftCount", DbType = "int NOT NULL")]
        public int GiftCount
        {
            get => _GiftCount;

            set
            {
                if (_GiftCount != value)
                {
                    _GiftCount = value;
                }
            }
        }

        [Column(Name = "GiftInKind", Storage = "_GiftInKind", DbType = "bit")]
        public bool? GiftInKind
        {
            get => _GiftInKind;

            set
            {
                if (_GiftInKind != value)
                {
                    _GiftInKind = value;
                }
            }
        }

        [Column(Name = "SpouseName", Storage = "_SpouseName", DbType = "nvarchar(138)")]
        public string SpouseName
        {
            get => _SpouseName;

            set
            {
                if (_SpouseName != value)
                {
                    _SpouseName = value;
                }
            }
        }

        [Column(Name = "SpouseTitle", Storage = "_SpouseTitle", DbType = "nvarchar(10)")]
        public string SpouseTitle
        {
            get => _SpouseTitle;

            set
            {
                if (_SpouseTitle != value)
                {
                    _SpouseTitle = value;
                }
            }
        }

        [Column(Name = "SpouseId", Storage = "_SpouseId", DbType = "int")]
        public int? SpouseId
        {
            get => _SpouseId;

            set
            {
                if (_SpouseId != value)
                {
                    _SpouseId = value;
                }
            }
        }

        [Column(Name = "SpouseContributionOptionsId", Storage = "_SpouseContributionOptionsId", DbType = "int")]
        public int? SpouseContributionOptionsId
        {
            get => _SpouseContributionOptionsId;

            set
            {
                if (_SpouseContributionOptionsId != value)
                {
                    _SpouseContributionOptionsId = value;
                }
            }
        }

        [Column(Name = "SpouseAmount", Storage = "_SpouseAmount", DbType = "Decimal(38,2) NOT NULL")]
        public decimal SpouseAmount
        {
            get => _SpouseAmount;

            set
            {
                if (_SpouseAmount != value)
                {
                    _SpouseAmount = value;
                }
            }
        }

        [Column(Name = "CampusId", Storage = "_CampusId", DbType = "int")]
        public int? CampusId
        {
            get => _CampusId;

            set
            {
                if (_CampusId != value)
                {
                    _CampusId = value;
                }
            }
        }

        [Column(Name = "HouseName", Storage = "_HouseName", DbType = "nvarchar(100)")]
        public string HouseName
        {
            get => _HouseName;

            set
            {
                if (_HouseName != value)
                {
                    _HouseName = value;
                }
            }
        }

        [Column(Name = "ElectronicStatement", Storage = "_ElectronicStatement", DbType = "bit NOT NULL")]
        public bool ElectronicStatement
        {
            get => _ElectronicStatement;

            set
            {
                if (_ElectronicStatement != value)
                {
                    _ElectronicStatement = value;
                }
            }
        }

        [Column(Name = "MailingAddress", Storage = "_MailingAddress", DbType = "nvarchar")]
        public string MailingAddress
        {
            get => _MailingAddress;

            set
            {
                if (_MailingAddress != value)
                {
                    _MailingAddress = value;
                }
            }
        }

        [Column(Name = "CoupleName", Storage = "_CoupleName", DbType = "nvarchar")]
        public string CoupleName
        {
            get => _CoupleName;

            set
            {
                if (_CoupleName != value)
                {
                    _CoupleName = value;
                }
            }
        }
    }
}
