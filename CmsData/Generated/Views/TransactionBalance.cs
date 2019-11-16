using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "TransactionBalances")]
    public partial class TransactionBalance
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _BalancesId;

        private decimal? _BegBal;

        private decimal? _Payment;

        private decimal? _TotDue;

        private int? _NumPeople;

        private string _People;

        private bool? _CanVoid;

        private bool? _CanCredit;

        private bool? _IsAdjustment;

        public TransactionBalance()
        {
        }

        [Column(Name = "BalancesId", Storage = "_BalancesId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
        public int BalancesId
        {
            get => _BalancesId;

            set
            {
                if (_BalancesId != value)
                {
                    _BalancesId = value;
                }
            }
        }

        [Column(Name = "BegBal", Storage = "_BegBal", DbType = "money")]
        public decimal? BegBal
        {
            get => _BegBal;

            set
            {
                if (_BegBal != value)
                {
                    _BegBal = value;
                }
            }
        }

        [Column(Name = "Payment", Storage = "_Payment", DbType = "money")]
        public decimal? Payment
        {
            get => _Payment;

            set
            {
                if (_Payment != value)
                {
                    _Payment = value;
                }
            }
        }

        [Column(Name = "TotDue", Storage = "_TotDue", DbType = "money")]
        public decimal? TotDue
        {
            get => _TotDue;

            set
            {
                if (_TotDue != value)
                {
                    _TotDue = value;
                }
            }
        }

        [Column(Name = "NumPeople", Storage = "_NumPeople", DbType = "int")]
        public int? NumPeople
        {
            get => _NumPeople;

            set
            {
                if (_NumPeople != value)
                {
                    _NumPeople = value;
                }
            }
        }

        [Column(Name = "People", Storage = "_People", DbType = "nvarchar")]
        public string People
        {
            get => _People;

            set
            {
                if (_People != value)
                {
                    _People = value;
                }
            }
        }

        [Column(Name = "CanVoid", Storage = "_CanVoid", DbType = "bit")]
        public bool? CanVoid
        {
            get => _CanVoid;

            set
            {
                if (_CanVoid != value)
                {
                    _CanVoid = value;
                }
            }
        }

        [Column(Name = "CanCredit", Storage = "_CanCredit", DbType = "bit")]
        public bool? CanCredit
        {
            get => _CanCredit;

            set
            {
                if (_CanCredit != value)
                {
                    _CanCredit = value;
                }
            }
        }

        [Column(Name = "IsAdjustment", Storage = "_IsAdjustment", DbType = "bit")]
        public bool? IsAdjustment
        {
            get => _IsAdjustment;

            set
            {
                if (_IsAdjustment != value)
                {
                    _IsAdjustment = value;
                }
            }
        }
    }
}
