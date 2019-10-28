using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "PledgeReport")]
    public partial class PledgeReport
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _FundId;

        private string _FundName;

        private decimal? _Plg;

        private decimal? _ToPledge;

        private decimal? _NotToPledge;

        private decimal? _ToFund;

        public PledgeReport()
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

        [Column(Name = "FundName", Storage = "_FundName", DbType = "nvarchar(256) NOT NULL")]
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

        [Column(Name = "ToPledge", Storage = "_ToPledge", DbType = "Decimal(38,2)")]
        public decimal? ToPledge
        {
            get => _ToPledge;

            set
            {
                if (_ToPledge != value)
                {
                    _ToPledge = value;
                }
            }
        }

        [Column(Name = "NotToPledge", Storage = "_NotToPledge", DbType = "Decimal(38,2)")]
        public decimal? NotToPledge
        {
            get => _NotToPledge;

            set
            {
                if (_NotToPledge != value)
                {
                    _NotToPledge = value;
                }
            }
        }

        [Column(Name = "ToFund", Storage = "_ToFund", DbType = "Decimal(38,2)")]
        public decimal? ToFund
        {
            get => _ToFund;

            set
            {
                if (_ToFund != value)
                {
                    _ToFund = value;
                }
            }
        }
    }
}
