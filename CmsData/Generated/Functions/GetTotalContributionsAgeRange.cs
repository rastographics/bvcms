using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetTotalContributionsAgeRange")]
    public partial class GetTotalContributionsAgeRange
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private decimal? _Total;

        private int? _Count;

        private int? _DonorCount;

        private string _Range;

        public GetTotalContributionsAgeRange()
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

        [Column(Name = "Count", Storage = "_Count", DbType = "int")]
        public int? Count
        {
            get => _Count;

            set
            {
                if (_Count != value)
                {
                    _Count = value;
                }
            }
        }

        [Column(Name = "DonorCount", Storage = "_DonorCount", DbType = "int")]
        public int? DonorCount
        {
            get => _DonorCount;

            set
            {
                if (_DonorCount != value)
                {
                    _DonorCount = value;
                }
            }
        }

        [Column(Name = "Range", Storage = "_Range", DbType = "nvarchar(4000)")]
        public string Range
        {
            get => _Range;

            set
            {
                if (_Range != value)
                {
                    _Range = value;
                }
            }
        }
    }
}
