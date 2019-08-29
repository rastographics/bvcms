using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData.View
{
    [Table(Name = "PledgesSummary")]
    public partial class PledgesSummary
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

        private int? _FundId;

        private string _FundName;

        private decimal? _AmountPledged;

        private decimal? _AmountContributed;

        private decimal? _Balance;

        public PledgesSummary()
        {
        }

        [Column(Name = "FundId", Storage = "_FundId", DbType = "int")]
        public int? FundId
        {
            get
            {
                return this._FundId;
            }

            set
            {
                if (this._FundId != value)
                    this._FundId = value;
            }
        }

        [Column(Name = "FundName", Storage = "_FundName", DbType = "nvarchar(max)")]
        public string FundName
        {
            get
            {
                return this._FundName;
            }

            set
            {
                if (this._FundName != value)
                    this._FundName = value;
            }
        }

        [Column(Name = "AmountPledged", Storage = "_AmountPledged", DbType = "Decimal(38,2)")]
        public decimal? PledgeAmt
        {
            get
            {
                return this._AmountPledged;
            }

            set
            {
                if (this._AmountPledged != value)
                    this._AmountPledged = value;
            }
        }

        [Column(Name = "AmountContributed", Storage = "_AmountContributed", DbType = "Decimal(38,2)")]
        public decimal? AmountContributed
        {
            get
            {
                return this._AmountContributed;
            }

            set
            {
                if (this._AmountContributed != value)
                    this._AmountContributed = value;
            }
        }

        [Column(Name = "Balance", Storage = "_Balance", DbType = "Decimal(38,2)")]
        public decimal? Balance
        {
            get
            {
                return this._Balance;
            }

            set
            {
                if (this._Balance != value)
                    this._Balance = value;
            }
        }
    }
}
