using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.BundleHeader")]
    public partial class BundleHeader : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _BundleHeaderId;

        private int _ChurchId;

        private int _CreatedBy;

        private DateTime _CreatedDate;

        private bool _RecordStatus;

        private int _BundleStatusId;

        private DateTime _ContributionDate;

        private int _BundleHeaderTypeId;

        private DateTime? _DepositDate;

        private decimal? _BundleTotal;

        private decimal? _TotalCash;

        private decimal? _TotalChecks;

        private decimal? _TotalEnvelopes;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private int? _FundId;

        private string _ReferenceId;

        private int? _ReferenceIdType;

        private EntitySet<BundleDetail> _BundleDetails;

        private EntityRef<ContributionFund> _Fund;

        private EntityRef<BundleHeaderType> _BundleHeaderType;

        private EntityRef<BundleStatusType> _BundleStatusType;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnBundleHeaderIdChanging(int value);
        partial void OnBundleHeaderIdChanged();

        partial void OnChurchIdChanging(int value);
        partial void OnChurchIdChanged();

        partial void OnCreatedByChanging(int value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        partial void OnRecordStatusChanging(bool value);
        partial void OnRecordStatusChanged();

        partial void OnBundleStatusIdChanging(int value);
        partial void OnBundleStatusIdChanged();

        partial void OnContributionDateChanging(DateTime value);
        partial void OnContributionDateChanged();

        partial void OnBundleHeaderTypeIdChanging(int value);
        partial void OnBundleHeaderTypeIdChanged();

        partial void OnDepositDateChanging(DateTime? value);
        partial void OnDepositDateChanged();

        partial void OnBundleTotalChanging(decimal? value);
        partial void OnBundleTotalChanged();

        partial void OnTotalCashChanging(decimal? value);
        partial void OnTotalCashChanged();

        partial void OnTotalChecksChanging(decimal? value);
        partial void OnTotalChecksChanged();

        partial void OnTotalEnvelopesChanging(decimal? value);
        partial void OnTotalEnvelopesChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        partial void OnFundIdChanging(int? value);
        partial void OnFundIdChanged();

        partial void OnReferenceIdChanging(string value);
        partial void OnReferenceIdChanged();

        partial void OnReferenceIdTypeChanging(int? value);
        partial void OnReferenceIdTypeChanged();

        #endregion

        public BundleHeader()
        {
            _BundleDetails = new EntitySet<BundleDetail>(new Action<BundleDetail>(attach_BundleDetails), new Action<BundleDetail>(detach_BundleDetails));

            _Fund = default(EntityRef<ContributionFund>);

            _BundleHeaderType = default(EntityRef<BundleHeaderType>);

            _BundleStatusType = default(EntityRef<BundleStatusType>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "BundleHeaderId", UpdateCheck = UpdateCheck.Never, Storage = "_BundleHeaderId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int BundleHeaderId
        {
            get => _BundleHeaderId;

            set
            {
                if (_BundleHeaderId != value)
                {
                    OnBundleHeaderIdChanging(value);
                    SendPropertyChanging();
                    _BundleHeaderId = value;
                    SendPropertyChanged("BundleHeaderId");
                    OnBundleHeaderIdChanged();
                }
            }
        }

        [Column(Name = "ChurchId", UpdateCheck = UpdateCheck.Never, Storage = "_ChurchId", DbType = "int NOT NULL")]
        public int ChurchId
        {
            get => _ChurchId;

            set
            {
                if (_ChurchId != value)
                {
                    OnChurchIdChanging(value);
                    SendPropertyChanging();
                    _ChurchId = value;
                    SendPropertyChanged("ChurchId");
                    OnChurchIdChanged();
                }
            }
        }

        [Column(Name = "CreatedBy", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int NOT NULL")]
        public int CreatedBy
        {
            get => _CreatedBy;

            set
            {
                if (_CreatedBy != value)
                {
                    OnCreatedByChanging(value);
                    SendPropertyChanging();
                    _CreatedBy = value;
                    SendPropertyChanged("CreatedBy");
                    OnCreatedByChanged();
                }
            }
        }

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime NOT NULL")]
        public DateTime CreatedDate
        {
            get => _CreatedDate;

            set
            {
                if (_CreatedDate != value)
                {
                    OnCreatedDateChanging(value);
                    SendPropertyChanging();
                    _CreatedDate = value;
                    SendPropertyChanged("CreatedDate");
                    OnCreatedDateChanged();
                }
            }
        }

        [Column(Name = "RecordStatus", UpdateCheck = UpdateCheck.Never, Storage = "_RecordStatus", DbType = "bit NOT NULL")]
        public bool RecordStatus
        {
            get => _RecordStatus;

            set
            {
                if (_RecordStatus != value)
                {
                    OnRecordStatusChanging(value);
                    SendPropertyChanging();
                    _RecordStatus = value;
                    SendPropertyChanged("RecordStatus");
                    OnRecordStatusChanged();
                }
            }
        }

        [Column(Name = "BundleStatusId", UpdateCheck = UpdateCheck.Never, Storage = "_BundleStatusId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int BundleStatusId
        {
            get => _BundleStatusId;

            set
            {
                if (_BundleStatusId != value)
                {
                    if (_BundleStatusType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnBundleStatusIdChanging(value);
                    SendPropertyChanging();
                    _BundleStatusId = value;
                    SendPropertyChanged("BundleStatusId");
                    OnBundleStatusIdChanged();
                }
            }
        }

        [Column(Name = "ContributionDate", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionDate", DbType = "datetime NOT NULL")]
        public DateTime ContributionDate
        {
            get => _ContributionDate;

            set
            {
                if (_ContributionDate != value)
                {
                    OnContributionDateChanging(value);
                    SendPropertyChanging();
                    _ContributionDate = value;
                    SendPropertyChanged("ContributionDate");
                    OnContributionDateChanged();
                }
            }
        }

        [Column(Name = "BundleHeaderTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_BundleHeaderTypeId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int BundleHeaderTypeId
        {
            get => _BundleHeaderTypeId;

            set
            {
                if (_BundleHeaderTypeId != value)
                {
                    if (_BundleHeaderType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnBundleHeaderTypeIdChanging(value);
                    SendPropertyChanging();
                    _BundleHeaderTypeId = value;
                    SendPropertyChanged("BundleHeaderTypeId");
                    OnBundleHeaderTypeIdChanged();
                }
            }
        }

        [Column(Name = "DepositDate", UpdateCheck = UpdateCheck.Never, Storage = "_DepositDate", DbType = "datetime")]
        public DateTime? DepositDate
        {
            get => _DepositDate;

            set
            {
                if (_DepositDate != value)
                {
                    OnDepositDateChanging(value);
                    SendPropertyChanging();
                    _DepositDate = value;
                    SendPropertyChanged("DepositDate");
                    OnDepositDateChanged();
                }
            }
        }

        [Column(Name = "BundleTotal", UpdateCheck = UpdateCheck.Never, Storage = "_BundleTotal", DbType = "Decimal(10,2)")]
        public decimal? BundleTotal
        {
            get => _BundleTotal;

            set
            {
                if (_BundleTotal != value)
                {
                    OnBundleTotalChanging(value);
                    SendPropertyChanging();
                    _BundleTotal = value;
                    SendPropertyChanged("BundleTotal");
                    OnBundleTotalChanged();
                }
            }
        }

        [Column(Name = "TotalCash", UpdateCheck = UpdateCheck.Never, Storage = "_TotalCash", DbType = "Decimal(10,2)")]
        public decimal? TotalCash
        {
            get => _TotalCash;

            set
            {
                if (_TotalCash != value)
                {
                    OnTotalCashChanging(value);
                    SendPropertyChanging();
                    _TotalCash = value;
                    SendPropertyChanged("TotalCash");
                    OnTotalCashChanged();
                }
            }
        }

        [Column(Name = "TotalChecks", UpdateCheck = UpdateCheck.Never, Storage = "_TotalChecks", DbType = "Decimal(10,2)")]
        public decimal? TotalChecks
        {
            get => _TotalChecks;

            set
            {
                if (_TotalChecks != value)
                {
                    OnTotalChecksChanging(value);
                    SendPropertyChanging();
                    _TotalChecks = value;
                    SendPropertyChanged("TotalChecks");
                    OnTotalChecksChanged();
                }
            }
        }

        [Column(Name = "TotalEnvelopes", UpdateCheck = UpdateCheck.Never, Storage = "_TotalEnvelopes", DbType = "Decimal(10,2)")]
        public decimal? TotalEnvelopes
        {
            get => _TotalEnvelopes;

            set
            {
                if (_TotalEnvelopes != value)
                {
                    OnTotalEnvelopesChanging(value);
                    SendPropertyChanging();
                    _TotalEnvelopes = value;
                    SendPropertyChanged("TotalEnvelopes");
                    OnTotalEnvelopesChanged();
                }
            }
        }

        [Column(Name = "ModifiedBy", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedBy", DbType = "int")]
        public int? ModifiedBy
        {
            get => _ModifiedBy;

            set
            {
                if (_ModifiedBy != value)
                {
                    OnModifiedByChanging(value);
                    SendPropertyChanging();
                    _ModifiedBy = value;
                    SendPropertyChanged("ModifiedBy");
                    OnModifiedByChanged();
                }
            }
        }

        [Column(Name = "ModifiedDate", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedDate", DbType = "datetime")]
        public DateTime? ModifiedDate
        {
            get => _ModifiedDate;

            set
            {
                if (_ModifiedDate != value)
                {
                    OnModifiedDateChanging(value);
                    SendPropertyChanging();
                    _ModifiedDate = value;
                    SendPropertyChanged("ModifiedDate");
                    OnModifiedDateChanged();
                }
            }
        }

        [Column(Name = "FundId", UpdateCheck = UpdateCheck.Never, Storage = "_FundId", DbType = "int")]
        [IsForeignKey]
        public int? FundId
        {
            get => _FundId;

            set
            {
                if (_FundId != value)
                {
                    if (_Fund.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnFundIdChanging(value);
                    SendPropertyChanging();
                    _FundId = value;
                    SendPropertyChanged("FundId");
                    OnFundIdChanged();
                }
            }
        }

        [Column(Name = "ReferenceId", UpdateCheck = UpdateCheck.Never, Storage = "_ReferenceId", DbType = "nvarchar(100)")]
        public string ReferenceId
        {
            get => _ReferenceId;

            set
            {
                if (_ReferenceId != value)
                {
                    OnReferenceIdChanging(value);
                    SendPropertyChanging();
                    _ReferenceId = value;
                    SendPropertyChanged("ReferenceId");
                    OnReferenceIdChanged();
                }
            }
        }

        [Column(Name = "ReferenceIdType", UpdateCheck = UpdateCheck.Never, Storage = "_ReferenceIdType", DbType = "int")]
        public int? ReferenceIdType
        {
            get => _ReferenceIdType;

            set
            {
                if (_ReferenceIdType != value)
                {
                    OnReferenceIdTypeChanging(value);
                    SendPropertyChanging();
                    _ReferenceIdType = value;
                    SendPropertyChanged("ReferenceIdType");
                    OnReferenceIdTypeChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "BUNDLE_DETAIL_BUNDLE_FK", Storage = "_BundleDetails", OtherKey = "BundleHeaderId")]
        public EntitySet<BundleDetail> BundleDetails
           {
               get => _BundleDetails;

            set => _BundleDetails.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "BundleHeaders__Fund", Storage = "_Fund", ThisKey = "FundId", IsForeignKey = true)]
        public ContributionFund Fund
        {
            get => _Fund.Entity;

            set
            {
                ContributionFund previousValue = _Fund.Entity;
                if (((previousValue != value)
                            || (_Fund.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Fund.Entity = null;
                        previousValue.BundleHeaders.Remove(this);
                    }

                    _Fund.Entity = value;
                    if (value != null)
                    {
                        value.BundleHeaders.Add(this);

                        _FundId = value.FundId;

                    }

                    else
                    {
                        _FundId = default(int?);

                    }

                    SendPropertyChanged("Fund");
                }
            }
        }

        [Association(Name = "FK_BUNDLE_HEADER_TBL_BundleHeaderTypes", Storage = "_BundleHeaderType", ThisKey = "BundleHeaderTypeId", IsForeignKey = true)]
        public BundleHeaderType BundleHeaderType
        {
            get => _BundleHeaderType.Entity;

            set
            {
                BundleHeaderType previousValue = _BundleHeaderType.Entity;
                if (((previousValue != value)
                            || (_BundleHeaderType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _BundleHeaderType.Entity = null;
                        previousValue.BundleHeaders.Remove(this);
                    }

                    _BundleHeaderType.Entity = value;
                    if (value != null)
                    {
                        value.BundleHeaders.Add(this);

                        _BundleHeaderTypeId = value.Id;

                    }

                    else
                    {
                        _BundleHeaderTypeId = default(int);

                    }

                    SendPropertyChanged("BundleHeaderType");
                }
            }
        }

        [Association(Name = "FK_BUNDLE_HEADER_TBL_BundleStatusTypes", Storage = "_BundleStatusType", ThisKey = "BundleStatusId", IsForeignKey = true)]
        public BundleStatusType BundleStatusType
        {
            get => _BundleStatusType.Entity;

            set
            {
                BundleStatusType previousValue = _BundleStatusType.Entity;
                if (((previousValue != value)
                            || (_BundleStatusType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _BundleStatusType.Entity = null;
                        previousValue.BundleHeaders.Remove(this);
                    }

                    _BundleStatusType.Entity = value;
                    if (value != null)
                    {
                        value.BundleHeaders.Add(this);

                        _BundleStatusId = value.Id;

                    }

                    else
                    {
                        _BundleStatusId = default(int);

                    }

                    SendPropertyChanged("BundleStatusType");
                }
            }
        }

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((PropertyChanging != null))
            {
                PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void attach_BundleDetails(BundleDetail entity)
        {
            SendPropertyChanging();
            entity.BundleHeader = this;
        }

        private void detach_BundleDetails(BundleDetail entity)
        {
            SendPropertyChanging();
            entity.BundleHeader = null;
        }
    }
}
