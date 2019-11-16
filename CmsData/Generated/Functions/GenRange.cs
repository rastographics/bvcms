using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GenRanges")]
    public partial class GenRange
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _Amt;

        private int? _MinAmt;

        private int? _MaxAmt;

        public GenRange()
        {
        }

        [Column(Name = "amt", Storage = "_Amt", DbType = "int")]
        public int? Amt
        {
            get => _Amt;

            set
            {
                if (_Amt != value)
                {
                    _Amt = value;
                }
            }
        }

        [Column(Name = "MinAmt", Storage = "_MinAmt", DbType = "int")]
        public int? MinAmt
        {
            get => _MinAmt;

            set
            {
                if (_MinAmt != value)
                {
                    _MinAmt = value;
                }
            }
        }

        [Column(Name = "MaxAmt", Storage = "_MaxAmt", DbType = "int")]
        public int? MaxAmt
        {
            get => _MaxAmt;

            set
            {
                if (_MaxAmt != value)
                {
                    _MaxAmt = value;
                }
            }
        }
    }
}
