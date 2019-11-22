using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetTotalContributionsRange")]
    public partial class GetTotalContributionsRange
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private decimal? _Total;

        private int? _Count;

        private int _Range;

        public GetTotalContributionsRange()
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

        [Column(Name = "RANGE", Storage = "_Range", DbType = "int NOT NULL")]
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
    }
}
