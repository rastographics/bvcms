using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "BundleList")]
    public partial class BundleList
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _BundleHeaderId;

        private string _HeaderType;

        private DateTime? _DepositDate;

        private decimal? _TotalBundle;

        private int? _FundId;

        private string _Fund;

        private string _Status;

        private int? _Open;

        private DateTime? _PostingDate;

        private decimal? _TotalItems;

        private int? _ItemCount;

        private decimal? _TotalNonTaxDed;

        private int _BundleStatusId;

        public BundleList()
        {
        }

        [Column(Name = "BundleHeaderId", Storage = "_BundleHeaderId", DbType = "int NOT NULL")]
        public int BundleHeaderId
        {
            get => _BundleHeaderId;

            set
            {
                if (_BundleHeaderId != value)
                {
                    _BundleHeaderId = value;
                }
            }
        }

        [Column(Name = "HeaderType", Storage = "_HeaderType", DbType = "nvarchar(50)")]
        public string HeaderType
        {
            get => _HeaderType;

            set
            {
                if (_HeaderType != value)
                {
                    _HeaderType = value;
                }
            }
        }

        [Column(Name = "DepositDate", Storage = "_DepositDate", DbType = "datetime")]
        public DateTime? DepositDate
        {
            get => _DepositDate;

            set
            {
                if (_DepositDate != value)
                {
                    _DepositDate = value;
                }
            }
        }

        [Column(Name = "TotalBundle", Storage = "_TotalBundle", DbType = "Decimal(12,2)")]
        public decimal? TotalBundle
        {
            get => _TotalBundle;

            set
            {
                if (_TotalBundle != value)
                {
                    _TotalBundle = value;
                }
            }
        }

        [Column(Name = "FundId", Storage = "_FundId", DbType = "int")]
        public int? FundId
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

        [Column(Name = "open", Storage = "_Open", DbType = "int")]
        public int? Open
        {
            get => _Open;

            set
            {
                if (_Open != value)
                {
                    _Open = value;
                }
            }
        }

        [Column(Name = "PostingDate", Storage = "_PostingDate", DbType = "datetime")]
        public DateTime? PostingDate
        {
            get => _PostingDate;

            set
            {
                if (_PostingDate != value)
                {
                    _PostingDate = value;
                }
            }
        }

        [Column(Name = "TotalItems", Storage = "_TotalItems", DbType = "Decimal(38,2)")]
        public decimal? TotalItems
        {
            get => _TotalItems;

            set
            {
                if (_TotalItems != value)
                {
                    _TotalItems = value;
                }
            }
        }

        [Column(Name = "ItemCount", Storage = "_ItemCount", DbType = "int")]
        public int? ItemCount
        {
            get => _ItemCount;

            set
            {
                if (_ItemCount != value)
                {
                    _ItemCount = value;
                }
            }
        }

        [Column(Name = "TotalNonTaxDed", Storage = "_TotalNonTaxDed", DbType = "Decimal(38,2)")]
        public decimal? TotalNonTaxDed
        {
            get => _TotalNonTaxDed;

            set
            {
                if (_TotalNonTaxDed != value)
                {
                    _TotalNonTaxDed = value;
                }
            }
        }

        [Column(Name = "BundleStatusId", Storage = "_BundleStatusId", DbType = "int NOT NULL")]
        public int BundleStatusId
        {
            get => _BundleStatusId;

            set
            {
                if (_BundleStatusId != value)
                {
                    _BundleStatusId = value;
                }
            }
        }
    }
}
