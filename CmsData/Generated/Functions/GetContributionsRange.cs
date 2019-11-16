using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetContributionsRange")]
    public partial class GetContributionsRange
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Range;

        private int? _DonorCount;

        private int? _Count;

        private decimal? _Total;

        public GetContributionsRange()
        {
        }

        [Column(Name = "Range", Storage = "_Range", DbType = "int NOT NULL")]
        public int Range
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
    }
}
