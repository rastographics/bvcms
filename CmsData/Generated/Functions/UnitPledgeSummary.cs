using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "UnitPledgeSummary")]
    public partial class UnitPledgeSummary
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs => new PropertyChangingEventArgs("");

        private string _FundName;

        private decimal? _Given;

        private decimal? _PriorYearsTotal;

        private decimal? _CurrentYearTotal;

        private decimal? _Pledged;

        private DateTime? _PledgeDate;

        private string _FundDescription;

        public UnitPledgeSummary()
        {
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

        [Column(Name = "Given", Storage = "_Given", DbType = "Decimal(38,2)")]
        public decimal? Given
        {
            get => _Given;

            set
            {
                if (_Given != value)
                {
                    _Given = value;
                }
            }
        }

        [Column(Name = "CurrentYearTotal", Storage = "_CurrentYearTotal", DbType = "Decimal(38,2)")]
        public decimal? CurrentYearTotal
        {
            get => _CurrentYearTotal;

            set
            {
                if (_CurrentYearTotal != value)
                {
                    _CurrentYearTotal = value;
                }
            }
        }

        [Column(Name = "PriorYearsTotal", Storage = "_PriorYearsTotal", DbType = "Decimal(38,2)")]
        public decimal? PriorYearsTotal
        {
            get => _PriorYearsTotal;

            set
            {
                if (_PriorYearsTotal != value)
                {
                    _PriorYearsTotal = value;
                }
            }
        }

        [Column(Name = "Pledged", Storage = "_Pledged", DbType = "Decimal(38,2)")]
        public decimal? Pledged
        {
            get => _Pledged;

            set
            {
                if (_Pledged != value)
                {
                    _Pledged = value;
                }
            }
        }

        [Column(Name = "PledgeDate", UpdateCheck = UpdateCheck.Never, Storage = "_PledgeDate", DbType = "datetime")]
        public DateTime? PledgeDate
        {
            get => _PledgeDate;

            set
            {
                if (_PledgeDate != value)
                {
                    _PledgeDate = value;
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

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public decimal Balance { get; set; }
    }
}
