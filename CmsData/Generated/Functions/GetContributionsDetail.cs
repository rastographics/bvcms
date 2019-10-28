using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetContributionsDetails")]
    public partial class GetContributionsDetail
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _FamilyId;

        private int? _PeopleId;

        private DateTime? _DateX;

        private int? _CreditGiverId;

        private int? _CreditGiverId2;

        private int? _SpouseId;

        private string _HeadName;

        private string _SpouseName;

        private decimal? _Amount;

        private decimal? _PledgeAmount;

        private int? _BundleHeaderId;

        private string _ContributionDesc;

        private string _CheckNo;

        private int _FundId;

        private string _FundName;

        private int _OpenPledgeFund;

        private string _BundleType;

        private string _BundleStatus;

        private int _ContributionId;

        private int _ContributionStatusId;

        private int _ContributionTypeId;

        public GetContributionsDetail()
        {
        }

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int")]
        public int? FamilyId
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

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
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

        [Column(Name = "Date", Storage = "_DateX", DbType = "datetime")]
        public DateTime? DateX
        {
            get => _DateX;

            set
            {
                if (_DateX != value)
                {
                    _DateX = value;
                }
            }
        }

        [Column(Name = "CreditGiverId", Storage = "_CreditGiverId", DbType = "int")]
        public int? CreditGiverId
        {
            get => _CreditGiverId;

            set
            {
                if (_CreditGiverId != value)
                {
                    _CreditGiverId = value;
                }
            }
        }

        [Column(Name = "CreditGiverId2", Storage = "_CreditGiverId2", DbType = "int")]
        public int? CreditGiverId2
        {
            get => _CreditGiverId2;

            set
            {
                if (_CreditGiverId2 != value)
                {
                    _CreditGiverId2 = value;
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

        [Column(Name = "HeadName", Storage = "_HeadName", DbType = "nvarchar(139)")]
        public string HeadName
        {
            get => _HeadName;

            set
            {
                if (_HeadName != value)
                {
                    _HeadName = value;
                }
            }
        }

        [Column(Name = "SpouseName", Storage = "_SpouseName", DbType = "nvarchar(139)")]
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

        [Column(Name = "Amount", Storage = "_Amount", DbType = "Decimal(11,2)")]
        public decimal? Amount
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

        [Column(Name = "PledgeAmount", Storage = "_PledgeAmount", DbType = "Decimal(11,2)")]
        public decimal? PledgeAmount
        {
            get => _PledgeAmount;

            set
            {
                if (_PledgeAmount != value)
                {
                    _PledgeAmount = value;
                }
            }
        }

        [Column(Name = "BundleHeaderId", Storage = "_BundleHeaderId", DbType = "int")]
        public int? BundleHeaderId
        {
            get => _BundleHeaderId;

            set
            {
                if (_BundleHeaderId != value)
                {
                    _BundleHeaderId = value;
                }
            }
        }

        [Column(Name = "ContributionDesc", Storage = "_ContributionDesc", DbType = "nvarchar(256)")]
        public string ContributionDesc
        {
            get => _ContributionDesc;

            set
            {
                if (_ContributionDesc != value)
                {
                    _ContributionDesc = value;
                }
            }
        }

        [Column(Name = "CheckNo", Storage = "_CheckNo", DbType = "nvarchar(20)")]
        public string CheckNo
        {
            get => _CheckNo;

            set
            {
                if (_CheckNo != value)
                {
                    _CheckNo = value;
                }
            }
        }

        [Column(Name = "FundId", Storage = "_FundId", DbType = "int NOT NULL")]
        public int FundId
        {
            get => _FundId;

            set
            {
                if (_FundId != value)
                {
                    _FundId = value;
                }
            }
        }

        [Column(Name = "FundName", Storage = "_FundName", DbType = "nvarchar(256) NOT NULL")]
        public string FundName
        {
            get => _FundName;

            set
            {
                if (_FundName != value)
                {
                    _FundName = value;
                }
            }
        }

        [Column(Name = "OpenPledgeFund", Storage = "_OpenPledgeFund", DbType = "int NOT NULL")]
        public int OpenPledgeFund
        {
            get => _OpenPledgeFund;

            set
            {
                if (_OpenPledgeFund != value)
                {
                    _OpenPledgeFund = value;
                }
            }
        }

        [Column(Name = "BundleType", Storage = "_BundleType", DbType = "nvarchar(50)")]
        public string BundleType
        {
            get => _BundleType;

            set
            {
                if (_BundleType != value)
                {
                    _BundleType = value;
                }
            }
        }

        [Column(Name = "BundleStatus", Storage = "_BundleStatus", DbType = "nvarchar(50)")]
        public string BundleStatus
        {
            get => _BundleStatus;

            set
            {
                if (_BundleStatus != value)
                {
                    _BundleStatus = value;
                }
            }
        }

        [Column(Name = "ContributionId", Storage = "_ContributionId", DbType = "int NOT NULL")]
        public int ContributionId
        {
            get => _ContributionId;

            set
            {
                if (_ContributionId != value)
                {
                    _ContributionId = value;
                }
            }
        }

        [Column(Name = "ContributionStatusId", Storage = "_ContributionStatusId", DbType = "int NOT NULL")]
        public int ContributionStatusId
        {
            get => _ContributionStatusId;

            set
            {
                if (_ContributionStatusId != value)
                {
                    _ContributionStatusId = value;
                }
            }
        }

        [Column(Name = "ContributionTypeId", Storage = "_ContributionTypeId", DbType = "int NOT NULL")]
        public int ContributionTypeId
        {
            get => _ContributionTypeId;

            set
            {
                if (_ContributionTypeId != value)
                {
                    _ContributionTypeId = value;
                }
            }
        }
    }
}
