using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PledgesSummary")]
    public partial class PledgesSummary
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _FundId;

        private string _FundName;

        private decimal _AmountPledged;

        private decimal _AmountContributed;

        private decimal _Balance;

        public PledgesSummary()
        {
        }

        [Column(Name = "FundId", Storage = "_FundId", DbType = "int")]
        public int FundId
        {
            get => _FundId;

            set
            {
                if (_FundId != value)
                {
                    _FundId = value;
                }
            }
        }

        [Column(Name = "FundName", Storage = "_FundName", DbType = "nvarchar(max)")]
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

        [Column(Name = "AmountPledged", Storage = "_AmountPledged", DbType = "Decimal(38,2)")]
        public decimal AmountPledged
        {
            get => _AmountPledged;

            set
            {
                if (_AmountPledged != value)
                {
                    _AmountPledged = value;
                }
            }
        }

        [Column(Name = "AmountContributed", Storage = "_AmountContributed", DbType = "Decimal(38,2)")]
        public decimal AmountContributed
        {
            get => _AmountContributed;

            set
            {
                if (_AmountContributed != value)
                {
                    _AmountContributed = value;
                }
            }
        }

        [Column(Name = "Balance", Storage = "_Balance", DbType = "Decimal(38,2)")]
        public decimal Balance
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
