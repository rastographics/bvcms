using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ContributionsBasic")]
    public partial class ContributionsBasic
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _ContributionId;

        private int? _PeopleId;

        private int _FamilyId;

        private decimal? _ContributionAmount;

        private DateTime? _ContributionDate;

        private string _CheckNo;

        private int _ContributionTypeId;

        private int _FundId;

        private int _BundleHeaderTypeId;

        public ContributionsBasic()
        {
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

        [Column(Name = "BundleHeaderTypeId", Storage = "_BundleHeaderTypeId", DbType = "int NOT NULL")]
        public int BundleHeaderTypeId
        {
            get => _BundleHeaderTypeId;

            set
            {
                if (_BundleHeaderTypeId != value)
                {
                    _BundleHeaderTypeId = value;
                }
            }
        }
    }
}
