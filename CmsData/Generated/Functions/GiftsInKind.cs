using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GiftsInKind")]
    public partial class GiftsInKind
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _ContributionId;

        private decimal? _ContributionAmount;

        private DateTime? _ContributionDate;

        private string _FundName;

        private string _CheckNo;

        private string _Name;

        private string _Description;

        private string _FundDescription;

        public GiftsInKind()
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(138)")]
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

        [Column(Name = "Description", Storage = "_Description", DbType = "nvarchar(256)")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    _Description = value;
                }
            }
        }

        [Column(Name = "FundDescription", Storage = "_FundDescription", DbType = "nvarchar(256)")]
        public string FundDescription
        {
            get => _FundDescription;

            set
            {
                if (_FundDescription != value)
                {
                    _FundDescription = value;
                }
            }
        }
    }
}
