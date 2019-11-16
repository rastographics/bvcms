using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GetTotalContributions")]
    public partial class GetTotalContribution
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _PeopleId;

        private string _Name;

        private string _SpouseName;

        private int _FundId;

        private string _FundDescription;

        private int? _Cnt;

        private decimal? _Amt;

        private decimal? _Plg;

        public GetTotalContribution()
        {
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

        [Column(Name = "Name", Storage = "_Name", DbType = "nvarchar(126)")]
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

        [Column(Name = "SpouseName", Storage = "_SpouseName", DbType = "nvarchar(138)")]
        public string SpouseName
        {
            get => _SpouseName;

            set
            {
                if (_SpouseName != value)
                {
                    _SpouseName = value;
                }
            }
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

        [Column(Name = "FundDescription", Storage = "_FundDescription", DbType = "nvarchar(256) NOT NULL")]
        public string FundDescription
        {
            get => _FundDescription;

            set
            {
                if (_FundDescription != value)
                {
                    _FundDescription = value;
                }
            }
        }

        [Column(Name = "Cnt", Storage = "_Cnt", DbType = "int")]
        public int? Cnt
        {
            get => _Cnt;

            set
            {
                if (_Cnt != value)
                {
                    _Cnt = value;
                }
            }
        }

        [Column(Name = "Amt", Storage = "_Amt", DbType = "Decimal(38,2)")]
        public decimal? Amt
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

        [Column(Name = "Plg", Storage = "_Plg", DbType = "Decimal(38,2)")]
        public decimal? Plg
        {
            get => _Plg;

            set
            {
                if (_Plg != value)
                {
                    _Plg = value;
                }
            }
        }
    }
}
