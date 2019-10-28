using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PledgeFulfillment")]
    public partial class PledgeFulfillment
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _First;

        private string _Last;

        private string _Spouse;

        private string _MemberStatus;

        private DateTime? _PledgeDate;

        private DateTime? _LastDate;

        private decimal? _PledgeAmt;

        private decimal? _TotalGiven;

        private decimal? _Balance;

        private string _Address;

        private string _City;

        private string _State;

        private string _Zip;

        private int? _CreditGiverId;

        private int? _SpouseId;

        private int _FamilyId;

        public PledgeFulfillment()
        {
        }

        [Column(Name = "First", Storage = "_First", DbType = "nvarchar(25)")]
        public string First
        {
            get => _First;

            set
            {
                if (_First != value)
                {
                    _First = value;
                }
            }
        }

        [Column(Name = "Last", Storage = "_Last", DbType = "nvarchar(100) NOT NULL")]
        public string Last
        {
            get => _Last;

            set
            {
                if (_Last != value)
                {
                    _Last = value;
                }
            }
        }

        [Column(Name = "Spouse", Storage = "_Spouse", DbType = "nvarchar(25)")]
        public string Spouse
        {
            get => _Spouse;

            set
            {
                if (_Spouse != value)
                {
                    _Spouse = value;
                }
            }
        }

        [Column(Name = "MemberStatus", Storage = "_MemberStatus", DbType = "nvarchar(50)")]
        public string MemberStatus
        {
            get => _MemberStatus;

            set
            {
                if (_MemberStatus != value)
                {
                    _MemberStatus = value;
                }
            }
        }

        [Column(Name = "PledgeDate", Storage = "_PledgeDate", DbType = "datetime")]
        public DateTime? PledgeDate
        {
            get => _PledgeDate;

            set
            {
                if (_PledgeDate != value)
                {
                    _PledgeDate = value;
                }
            }
        }

        [Column(Name = "LastDate", Storage = "_LastDate", DbType = "datetime")]
        public DateTime? LastDate
        {
            get => _LastDate;

            set
            {
                if (_LastDate != value)
                {
                    _LastDate = value;
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

        [Column(Name = "TotalGiven", Storage = "_TotalGiven", DbType = "Decimal(38,2)")]
        public decimal? TotalGiven
        {
            get => _TotalGiven;

            set
            {
                if (_TotalGiven != value)
                {
                    _TotalGiven = value;
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

        [Column(Name = "Address", Storage = "_Address", DbType = "nvarchar(100)")]
        public string Address
        {
            get => _Address;

            set
            {
                if (_Address != value)
                {
                    _Address = value;
                }
            }
        }

        [Column(Name = "City", Storage = "_City", DbType = "nvarchar(30)")]
        public string City
        {
            get => _City;

            set
            {
                if (_City != value)
                {
                    _City = value;
                }
            }
        }

        [Column(Name = "State", Storage = "_State", DbType = "nvarchar(20)")]
        public string State
        {
            get => _State;

            set
            {
                if (_State != value)
                {
                    _State = value;
                }
            }
        }

        [Column(Name = "Zip", Storage = "_Zip", DbType = "nvarchar(15)")]
        public string Zip
        {
            get => _Zip;

            set
            {
                if (_Zip != value)
                {
                    _Zip = value;
                }
            }
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

        [Column(Name = "FamilyId", Storage = "_FamilyId", DbType = "int NOT NULL")]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    _FamilyId = value;
                }
            }
        }
    }
}
