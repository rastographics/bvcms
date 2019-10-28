using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ChAiGiftData")]
    public partial class ChAiGiftDatum
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _PeopleId;

        private int _FamilyId;

        private int _ContributionId;

        private DateTime _CreatedDate;

        private DateTime? _ContributionDate;

        private decimal? _ContributionAmount;

        private string _Type;

        private string _Status;

        private string _BundleType;

        private string _FundName;

        private string _PaymentType;

        private string _CheckNo;

        public ChAiGiftDatum()
        {
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

        [Column(Name = "CreatedDate", Storage = "_CreatedDate", DbType = "datetime NOT NULL")]
        public DateTime CreatedDate
        {
            get => _CreatedDate;

            set
            {
                if (_CreatedDate != value)
                {
                    _CreatedDate = value;
                }
            }
        }

        [Column(Name = "ContributionDate", Storage = "_ContributionDate", DbType = "datetime")]
        public DateTime? ContributionDate
        {
            get => _ContributionDate;

            set
            {
                if (_ContributionDate != value)
                {
                    _ContributionDate = value;
                }
            }
        }

        [Column(Name = "ContributionAmount", Storage = "_ContributionAmount", DbType = "Decimal(11,2)")]
        public decimal? ContributionAmount
        {
            get => _ContributionAmount;

            set
            {
                if (_ContributionAmount != value)
                {
                    _ContributionAmount = value;
                }
            }
        }

        [Column(Name = "Type", Storage = "_Type", DbType = "nvarchar(50)")]
        public string Type
        {
            get => _Type;

            set
            {
                if (_Type != value)
                {
                    _Type = value;
                }
            }
        }

        [Column(Name = "Status", Storage = "_Status", DbType = "nvarchar(50)")]
        public string Status
        {
            get => _Status;

            set
            {
                if (_Status != value)
                {
                    _Status = value;
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

        [Column(Name = "PaymentType", Storage = "_PaymentType", DbType = "varchar(5) NOT NULL")]
        public string PaymentType
        {
            get => _PaymentType;

            set
            {
                if (_PaymentType != value)
                {
                    _PaymentType = value;
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
    }
}
