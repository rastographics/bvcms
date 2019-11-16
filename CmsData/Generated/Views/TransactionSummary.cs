using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "TransactionSummary")]
    public partial class TransactionSummary
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int? _RegId;

        private int _OrganizationId;

        private int _PeopleId;

        private DateTime? _TranDate;

        private decimal? _IndAmt;

        private decimal? _TotalAmt;

        private decimal? _TotalFee;

        private decimal _TotPaid;

        private decimal _TotCoupon;

        private decimal? _TotDue;

        private decimal? _IndPaid;

        private decimal? _IndDue;

        private double _IndPctC;

        private int? _NumPeople;

        private bool? _Isdeposit;

        private bool? _Iscoupon;

        private decimal _Donation;

        private bool _IsDonor;

        public TransactionSummary()
        {
        }

        [Column(Name = "RegId", Storage = "_RegId", DbType = "int")]
        public int? RegId
        {
            get => _RegId;

            set
            {
                if (_RegId != value)
                {
                    _RegId = value;
                }
            }
        }

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", DbType = "int NOT NULL")]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
                }
            }
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int NOT NULL")]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    _PeopleId = value;
                }
            }
        }

        [Column(Name = "TranDate", Storage = "_TranDate", DbType = "datetime")]
        public DateTime? TranDate
        {
            get => _TranDate;

            set
            {
                if (_TranDate != value)
                {
                    _TranDate = value;
                }
            }
        }

        [Column(Name = "IndAmt", Storage = "_IndAmt", DbType = "money")]
        public decimal? IndAmt
        {
            get => _IndAmt;

            set
            {
                if (_IndAmt != value)
                {
                    _IndAmt = value;
                }
            }
        }

        [Column(Name = "TotalAmt", Storage = "_TotalAmt", DbType = "money")]
        public decimal? TotalAmt
        {
            get => _TotalAmt;

            set
            {
                if (_TotalAmt != value)
                {
                    _TotalAmt = value;
                }
            }
        }

        [Column(Name = "TotalFee", Storage = "_TotalFee", DbType = "money")]
        public decimal? TotalFee
        {
            get => _TotalFee;

            set
            {
                if (_TotalFee != value)
                {
                    _TotalFee = value;
                }
            }
        }

        [Column(Name = "TotPaid", Storage = "_TotPaid", DbType = "money NOT NULL")]
        public decimal TotPaid
        {
            get => _TotPaid;

            set
            {
                if (_TotPaid != value)
                {
                    _TotPaid = value;
                }
            }
        }

        [Column(Name = "TotCoupon", Storage = "_TotCoupon", DbType = "money NOT NULL")]
        public decimal TotCoupon
        {
            get => _TotCoupon;

            set
            {
                if (_TotCoupon != value)
                {
                    _TotCoupon = value;
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

        [Column(Name = "IndPaid", Storage = "_IndPaid", DbType = "money")]
        public decimal? IndPaid
        {
            get => _IndPaid;

            set
            {
                if (_IndPaid != value)
                {
                    _IndPaid = value;
                }
            }
        }

        [Column(Name = "IndDue", Storage = "_IndDue", DbType = "money")]
        public decimal? IndDue
        {
            get => _IndDue;

            set
            {
                if (_IndDue != value)
                {
                    _IndDue = value;
                }
            }
        }

        [Column(Name = "IndPctC", Storage = "_IndPctC", DbType = "float NOT NULL")]
        public double IndPctC
        {
            get => _IndPctC;

            set
            {
                if (_IndPctC != value)
                {
                    _IndPctC = value;
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

        [Column(Name = "isdeposit", Storage = "_Isdeposit", DbType = "bit")]
        public bool? Isdeposit
        {
            get => _Isdeposit;

            set
            {
                if (_Isdeposit != value)
                {
                    _Isdeposit = value;
                }
            }
        }

        [Column(Name = "iscoupon", Storage = "_Iscoupon", DbType = "bit")]
        public bool? Iscoupon
        {
            get => _Iscoupon;

            set
            {
                if (_Iscoupon != value)
                {
                    _Iscoupon = value;
                }
            }
        }

        [Column(Name = "Donation", Storage = "_Donation", DbType = "money NOT NULL")]
        public decimal Donation
        {
            get => _Donation;

            set
            {
                if (_Donation != value)
                {
                    _Donation = value;
                }
            }
        }

        [Column(Name = "IsDonor", Storage = "_IsDonor", DbType = "bit NOT NULL")]
        public bool IsDonor
        {
            get => _IsDonor;

            set
            {
                if (_IsDonor != value)
                {
                    _IsDonor = value;
                }
            }
        }
    }
}
