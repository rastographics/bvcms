using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PledgeBalances")]
    public partial class PledgeBalance
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _CreditGiverId;

        private int? _SpouseId;

        private decimal? _PledgeAmt;

        private decimal? _GivenAmt;

        private decimal? _Balance;

        public PledgeBalance()
        {
        }

        [Column(Name = "CreditGiverId", Storage = "_CreditGiverId", DbType = "int")]
        public int? CreditGiverId
        {
            get => _CreditGiverId;

            set
            {
                if (_CreditGiverId != value)
                {
                    _CreditGiverId = value;
                }
            }
        }

        [Column(Name = "SpouseId", Storage = "_SpouseId", DbType = "int")]
        public int? SpouseId
        {
            get => _SpouseId;

            set
            {
                if (_SpouseId != value)
                {
                    _SpouseId = value;
                }
            }
        }

        [Column(Name = "PledgeAmt", Storage = "_PledgeAmt", DbType = "Decimal(38,2)")]
        public decimal? PledgeAmt
        {
            get => _PledgeAmt;

            set
            {
                if (_PledgeAmt != value)
                {
                    _PledgeAmt = value;
                }
            }
        }

        [Column(Name = "GivenAmt", Storage = "_GivenAmt", DbType = "Decimal(38,2)")]
        public decimal? GivenAmt
        {
            get => _GivenAmt;

            set
            {
                if (_GivenAmt != value)
                {
                    _GivenAmt = value;
                }
            }
        }

        [Column(Name = "Balance", Storage = "_Balance", DbType = "Decimal(38,2)")]
        public decimal? Balance
        {
            get => _Balance;

            set
            {
                if (_Balance != value)
                {
                    _Balance = value;
                }
            }
        }
    }
}
