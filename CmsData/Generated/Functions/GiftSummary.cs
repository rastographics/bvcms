using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GiftSummary")]
    public partial class GiftSummary
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private decimal? _Total;

        private string _FundName;

        private string _FundDescription;

        public GiftSummary()
        {
        }

        [Column(Name = "Total", Storage = "_Total", DbType = "Decimal(38,2)")]
        public decimal? Total
        {
            get => _Total;

            set
            {
                if (_Total != value)
                {
                    _Total = value;
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
