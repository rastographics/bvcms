using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ManagedGivingList")]
    public partial class ManagedGivingList
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private string _Name2;

        private int _PeopleId;

        private DateTime? _StartWhen;

        private DateTime? _NextDate;

        private string _SemiEvery;

        private int? _Day1;

        private int? _Day2;

        private int? _EveryN;

        private string _Period;

        private DateTime? _StopWhen;

        private int? _StopAfter;

        private string _Type;

        private decimal? _ActiveAmt;

        private decimal? _InactiveAmt;

        public ManagedGivingList()
        {
        }

        [Column(Name = "Name2", Storage = "_Name2", DbType = "nvarchar(139)")]
        public string Name2
        {
            get => _Name2;

            set
            {
                if (_Name2 != value)
                {
                    _Name2 = value;
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

        [Column(Name = "StartWhen", Storage = "_StartWhen", DbType = "datetime")]
        public DateTime? StartWhen
        {
            get => _StartWhen;

            set
            {
                if (_StartWhen != value)
                {
                    _StartWhen = value;
                }
            }
        }

        [Column(Name = "NextDate", Storage = "_NextDate", DbType = "datetime")]
        public DateTime? NextDate
        {
            get => _NextDate;

            set
            {
                if (_NextDate != value)
                {
                    _NextDate = value;
                }
            }
        }

        [Column(Name = "SemiEvery", Storage = "_SemiEvery", DbType = "nvarchar(2)")]
        public string SemiEvery
        {
            get => _SemiEvery;

            set
            {
                if (_SemiEvery != value)
                {
                    _SemiEvery = value;
                }
            }
        }

        [Column(Name = "Day1", Storage = "_Day1", DbType = "int")]
        public int? Day1
        {
            get => _Day1;

            set
            {
                if (_Day1 != value)
                {
                    _Day1 = value;
                }
            }
        }

        [Column(Name = "Day2", Storage = "_Day2", DbType = "int")]
        public int? Day2
        {
            get => _Day2;

            set
            {
                if (_Day2 != value)
                {
                    _Day2 = value;
                }
            }
        }

        [Column(Name = "EveryN", Storage = "_EveryN", DbType = "int")]
        public int? EveryN
        {
            get => _EveryN;

            set
            {
                if (_EveryN != value)
                {
                    _EveryN = value;
                }
            }
        }

        [Column(Name = "Period", Storage = "_Period", DbType = "nvarchar(2)")]
        public string Period
        {
            get => _Period;

            set
            {
                if (_Period != value)
                {
                    _Period = value;
                }
            }
        }

        [Column(Name = "StopWhen", Storage = "_StopWhen", DbType = "datetime")]
        public DateTime? StopWhen
        {
            get => _StopWhen;

            set
            {
                if (_StopWhen != value)
                {
                    _StopWhen = value;
                }
            }
        }

        [Column(Name = "StopAfter", Storage = "_StopAfter", DbType = "int")]
        public int? StopAfter
        {
            get => _StopAfter;

            set
            {
                if (_StopAfter != value)
                {
                    _StopAfter = value;
                }
            }
        }

        [Column(Name = "type", Storage = "_Type", DbType = "nvarchar(2)")]
        public string Type
        {
            get => _Type;

            set
            {
                if (_Type != value)
                {
                    _Type = value;
                }
            }
        }

        [Column(Name = "ActiveAmt", Storage = "_ActiveAmt", DbType = "money")]
        public decimal? ActiveAmt
        {
            get => _ActiveAmt;

            set
            {
                if (_ActiveAmt != value)
                {
                    _ActiveAmt = value;
                }
            }
        }

        [Column(Name = "InactiveAmt", Storage = "_InactiveAmt", DbType = "money")]
        public decimal? InactiveAmt
        {
            get => _InactiveAmt;

            set
            {
                if (_InactiveAmt != value)
                {
                    _InactiveAmt = value;
                }
            }
        }
    }
}
