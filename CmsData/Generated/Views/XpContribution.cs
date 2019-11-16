using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "XpContribution")]
    public partial class XpContribution
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private int _ContributionId;

        private int? _PeopleId;

        private string _BundleType;

        private DateTime? _DepositDate;

        private string _Fund;

        private string _Type;

        private DateTime? _DateX;

        private decimal? _Amount;

        private string _Description;

        private string _Status;

        private bool? _Pledge;

        private string _CheckNo;

        private string _Campus;

        public XpContribution()
        {
        }

        [Column(Name = "ContributionId", Storage = "_ContributionId", DbType = "int NOT NULL")]
        public int ContributionId
        {
            get => _ContributionId;

            set
            {
                if (_ContributionId != value)
                {
                    _ContributionId = value;
                }
            }
        }

        [Column(Name = "PeopleId", Storage = "_PeopleId", DbType = "int")]
        public int? PeopleId
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

        [Column(Name = "BundleType", Storage = "_BundleType", DbType = "nvarchar(50)")]
        public string BundleType
        {
            get => _BundleType;

            set
            {
                if (_BundleType != value)
                {
                    _BundleType = value;
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

        [Column(Name = "Date", Storage = "_DateX", DbType = "datetime")]
        public DateTime? DateX
        {
            get => _DateX;

            set
            {
                if (_DateX != value)
                {
                    _DateX = value;
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

        [Column(Name = "Description", Storage = "_Description", DbType = "nvarchar(256)")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    _Description = value;
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

        [Column(Name = "Pledge", Storage = "_Pledge", DbType = "bit")]
        public bool? Pledge
        {
            get => _Pledge;

            set
            {
                if (_Pledge != value)
                {
                    _Pledge = value;
                }
            }
        }

        [Column(Name = "CheckNo", Storage = "_CheckNo", DbType = "nvarchar(20)")]
        public string CheckNo
        {
            get => _CheckNo;

            set
            {
                if (_CheckNo != value)
                {
                    _CheckNo = value;
                }
            }
        }

        [Column(Name = "Campus", Storage = "_Campus", DbType = "nvarchar(100)")]
        public string Campus
        {
            get => _Campus;

            set
            {
                if (_Campus != value)
                {
                    _Campus = value;
                }
            }
        }
    }
}
