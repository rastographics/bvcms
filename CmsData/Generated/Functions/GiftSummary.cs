using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "GiftSummary")]
    public partial class GiftSummary
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);


        private decimal? _Total;

        private string _FundName;

        private string _FundDescription;


        public GiftSummary()
        {
        }



        [Column(Name = "Total", Storage = "_Total", DbType = "Decimal(38,2)")]
        public decimal? Total
        {
            get
            {
                return this._Total;
            }

            set
            {
                if (this._Total != value)
                {
                    this._Total = value;
                }
            }

        }


        [Column(Name = "FundName", Storage = "_FundName", DbType = "nvarchar(256) NOT NULL")]
        public string FundName
        {
            get
            {
                return this._FundName;
            }

            set
            {
                if (this._FundName != value)
                {
                    this._FundName = value;
                }
            }

        }

        [Column(Name = "FundDescription", Storage = "_FundDescription", DbType = "nvarchar(256)")]
        public string FundDescription
        {
            get
            {
                return this._FundDescription;
            }

            set
            {
                if (this._FundDescription != value)
                {
                    this._FundDescription = value;
                }
            }
        }

    }

}
