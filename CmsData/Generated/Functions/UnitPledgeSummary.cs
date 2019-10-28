using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "UnitPledgeSummary")]
    public partial class UnitPledgeSummary
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _FundName;

        private decimal? _Given;

        private decimal? _Pledged;

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
