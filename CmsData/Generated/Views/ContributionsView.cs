using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "ContributionsView")]
    public partial class ContributionsView
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _FundId;

        private int _TypeId;

        private DateTime? _CDate;

        private decimal? _Amount;

        private int? _StatusId;

        private bool? _Pledged;

        private int? _Age;

        private int? _AgeRange;

        private string _Fund;

        private string _Status;

        private string _Type;

        private string _Name;

        public ContributionsView()
        {
        }

        [Column(Name = "FundId", Storage = "_FundId", DbType = "int NOT NULL")]
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

        [Column(Name = "TypeId", Storage = "_TypeId", DbType = "int NOT NULL")]
        public int TypeId
        {
            get => _TypeId;

            set
            {
                if (_TypeId != value)
                {
                    _TypeId = value;
                }
            }
        }

        [Column(Name = "CDate", Storage = "_CDate", DbType = "datetime")]
        public DateTime? CDate
        {
            get => _CDate;

            set
            {
                if (_CDate != value)
                {
                    _CDate = value;
                }
            }
        }

        [Column(Name = "Amount", Storage = "_Amount", DbType = "Decimal(11,2)")]
        public decimal? Amount
        {
            get => _Amount;

            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                }
            }
        }

        [Column(Name = "StatusId", Storage = "_StatusId", DbType = "int")]
        public int? StatusId
        {
            get => _StatusId;

            set
            {
                if (_StatusId != value)
                {
                    _StatusId = value;
                }
            }
        }

        [Column(Name = "Pledged", Storage = "_Pledged", DbType = "bit")]
        public bool? Pledged
        {
            get => _Pledged;

            set
            {
                if (_Pledged != value)
                {
                    _Pledged = value;
                }
            }
        }

        [Column(Name = "Age", Storage = "_Age", DbType = "int")]
        public int? Age
        {
            get => _Age;

            set
            {
                if (_Age != value)
                {
                    _Age = value;
                }
            }
        }

        [Column(Name = "AgeRange", Storage = "_AgeRange", DbType = "int")]
        public int? AgeRange
        {
            get => _AgeRange;

            set
            {
                if (_AgeRange != value)
                {
                    _AgeRange = value;
                }
            }
        }

        [Column(Name = "Fund", Storage = "_Fund", DbType = "nvarchar(256)")]
        public string Fund
        {
            get => _Fund;

            set
            {
                if (_Fund != value)
                {
                    _Fund = value;
                }
            }
        }

        [Column(Name = "Status", Storage = "_Status", DbType = "nvarchar(50)")]
        public string Status
        {
            get => _Status;

            set
            {
                if (_Status != value)
                {
                    _Status = value;
                }
            }
        }

        [Column(Name = "Type", Storage = "_Type", DbType = "nvarchar(50)")]
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(138)")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    _Name = value;
                }
            }
        }
    }
}
