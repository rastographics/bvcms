using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GivingCurrentPercentOfFormer")]
    public partial class GivingCurrentPercentOfFormer
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _Pid;

        private decimal? _C1amt;

        private decimal? _C2amt;

        private decimal? _Pct;

        public GivingCurrentPercentOfFormer()
        {
        }

        [Column(Name = "pid", Storage = "_Pid", DbType = "int NOT NULL")]
        public int Pid
        {
            get => _Pid;

            set
            {
                if (_Pid != value)
                {
                    _Pid = value;
                }
            }
        }

        [Column(Name = "c1amt", Storage = "_C1amt", DbType = "Decimal(38,2)")]
        public decimal? C1amt
        {
            get => _C1amt;

            set
            {
                if (_C1amt != value)
                {
                    _C1amt = value;
                }
            }
        }

        [Column(Name = "c2amt", Storage = "_C2amt", DbType = "Decimal(38,2)")]
        public decimal? C2amt
        {
            get => _C2amt;

            set
            {
                if (_C2amt != value)
                {
                    _C2amt = value;
                }
            }
        }

        [Column(Name = "pct", Storage = "_Pct", DbType = "Decimal(38,6)")]
        public decimal? Pct
        {
            get => _Pct;

            set
            {
                if (_Pct != value)
                {
                    _Pct = value;
                }
            }
        }
    }
}
