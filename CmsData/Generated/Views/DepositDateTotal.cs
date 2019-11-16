using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "DepositDateTotals")]
    public partial class DepositDateTotal
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private DateTime? _DepositDate;

        private decimal? _TotalHeader;

        private decimal? _TotalContributions;

        private int? _Count;

        public DepositDateTotal()
        {
        }

        [Column(Name = "DepositDate", Storage = "_DepositDate", DbType = "datetime")]
        public DateTime? DepositDate
        {
            get => _DepositDate;

            set
            {
                if (_DepositDate != value)
                {
                    _DepositDate = value;
                }
            }
        }

        [Column(Name = "TotalHeader", Storage = "_TotalHeader", DbType = "Decimal(38,2)")]
        public decimal? TotalHeader
        {
            get => _TotalHeader;

            set
            {
                if (_TotalHeader != value)
                {
                    _TotalHeader = value;
                }
            }
        }

        [Column(Name = "TotalContributions", Storage = "_TotalContributions", DbType = "Decimal(38,2)")]
        public decimal? TotalContributions
        {
            get => _TotalContributions;

            set
            {
                if (_TotalContributions != value)
                {
                    _TotalContributions = value;
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
    }
}
