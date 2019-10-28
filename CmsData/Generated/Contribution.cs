using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Contribution")]
    public partial class Contribution : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ContributionId;

        private int _CreatedBy;

        private DateTime _CreatedDate;

        private int _FundId;

        private int _ContributionTypeId;

        private int? _PeopleId;

        private DateTime? _ContributionDate;

        private decimal? _ContributionAmount;

        private string _ContributionDesc;

        private int? _ContributionStatusId;

        private bool? _PledgeFlag;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private DateTime? _PostingDate;

        private string _BankAccount;

        private int? _ExtraDataId;

        private string _CheckNo;

        private int? _QBSyncID;

        private int? _TranId;

        private int? _Source;

        private int? _CampusId;

        private int _ImageID;

        private string _MetaInfo;

        private int _Origin;

        private EntitySet<BundleDetail> _BundleDetails;

        private EntitySet<ContributionTag> _ContributionTags;

        private EntityRef<ContributionFund> _ContributionFund;

        private EntityRef<ContributionStatus> _ContributionStatus;

        private EntityRef<ContributionType> _ContributionType;

        private EntityRef<ExtraDatum> _ExtraDatum;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnContributionIdChanging(int value);
        partial void OnContributionIdChanged();

        partial void OnCreatedByChanging(int value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        partial void OnFundIdChanging(int value);
        partial void OnFundIdChanged();

        partial void OnContributionTypeIdChanging(int value);
        partial void OnContributionTypeIdChanged();

        partial void OnPeopleIdChanging(int? value);
        partial void OnPeopleIdChanged();

        partial void OnContributionDateChanging(DateTime? value);
        partial void OnContributionDateChanged();

        partial void OnContributionAmountChanging(decimal? value);
        partial void OnContributionAmountChanged();

        partial void OnContributionDescChanging(string value);
        partial void OnContributionDescChanged();

        partial void OnContributionStatusIdChanging(int? value);
        partial void OnContributionStatusIdChanged();

        partial void OnPledgeFlagChanging(bool? value);
        partial void OnPledgeFlagChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        partial void OnPostingDateChanging(DateTime? value);
        partial void OnPostingDateChanged();

        partial void OnBankAccountChanging(string value);
        partial void OnBankAccountChanged();

        partial void OnExtraDataIdChanging(int? value);
        partial void OnExtraDataIdChanged();

        partial void OnCheckNoChanging(string value);
        partial void OnCheckNoChanged();

        partial void OnQBSyncIDChanging(int? value);
        partial void OnQBSyncIDChanged();

        partial void OnTranIdChanging(int? value);
        partial void OnTranIdChanged();

        partial void OnSourceChanging(int? value);
        partial void OnSourceChanged();

        partial void OnCampusIdChanging(int? value);
        partial void OnCampusIdChanged();

        partial void OnImageIDChanging(int value);
        partial void OnImageIDChanged();

        partial void OnMetaInfoChanging(string value);
        partial void OnMetaInfoChanged();

        partial void OnOriginChanging(int value);
        partial void OnOriginChanged();

        #endregion

        public Contribution()
        {
            _BundleDetails = new EntitySet<BundleDetail>(new Action<BundleDetail>(attach_BundleDetails), new Action<BundleDetail>(detach_BundleDetails));

            _ContributionTags = new EntitySet<ContributionTag>(new Action<ContributionTag>(attach_ContributionTags), new Action<ContributionTag>(detach_ContributionTags));

            _ContributionFund = default(EntityRef<ContributionFund>);

            _ContributionStatus = default(EntityRef<ContributionStatus>);

            _ContributionType = default(EntityRef<ContributionType>);

            _ExtraDatum = default(EntityRef<ExtraDatum>);

            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ContributionId", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ContributionId
        {
            get => _ContributionId;

            set
            {
                if (_ContributionId != value)
                {
                    OnContributionIdChanging(value);
                    SendPropertyChanging();
                    _ContributionId = value;
                    SendPropertyChanged("ContributionId");
                    OnContributionIdChanged();
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

        [Column(Name = "FundId", UpdateCheck = UpdateCheck.Never, Storage = "_FundId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int FundId
        {
            get => _FundId;

            set
            {
                if (_FundId != value)
                {
                    if (_ContributionFund.HasLoadedOrAssignedValue)
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

        [Column(Name = "ContributionTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionTypeId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int ContributionTypeId
        {
            get => _ContributionTypeId;

            set
            {
                if (_ContributionTypeId != value)
                {
                    if (_ContributionType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnContributionTypeIdChanging(value);
                    SendPropertyChanging();
                    _ContributionTypeId = value;
                    SendPropertyChanged("ContributionTypeId");
                    OnContributionTypeIdChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        [IsForeignKey]
        public int? PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "ContributionDate", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionDate", DbType = "datetime")]
        public DateTime? ContributionDate
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

        [Column(Name = "ContributionAmount", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionAmount", DbType = "Decimal(11,2)")]
        public decimal? ContributionAmount
        {
            get => _ContributionAmount;

            set
            {
                if (_ContributionAmount != value)
                {
                    OnContributionAmountChanging(value);
                    SendPropertyChanging();
                    _ContributionAmount = value;
                    SendPropertyChanged("ContributionAmount");
                    OnContributionAmountChanged();
                }
            }
        }

        [Column(Name = "ContributionDesc", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionDesc", DbType = "nvarchar(256)")]
        public string ContributionDesc
        {
            get => _ContributionDesc;

            set
            {
                if (_ContributionDesc != value)
                {
                    OnContributionDescChanging(value);
                    SendPropertyChanging();
                    _ContributionDesc = value;
                    SendPropertyChanged("ContributionDesc");
                    OnContributionDescChanged();
                }
            }
        }

        [Column(Name = "ContributionStatusId", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionStatusId", DbType = "int")]
        [IsForeignKey]
        public int? ContributionStatusId
        {
            get => _ContributionStatusId;

            set
            {
                if (_ContributionStatusId != value)
                {
                    if (_ContributionStatus.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnContributionStatusIdChanging(value);
                    SendPropertyChanging();
                    _ContributionStatusId = value;
                    SendPropertyChanged("ContributionStatusId");
                    OnContributionStatusIdChanged();
                }
            }
        }

        [Column(Name = "PledgeFlag", UpdateCheck = UpdateCheck.Never, Storage = "_PledgeFlag", DbType = "bit")]
        public bool? PledgeFlag
        {
            get => _PledgeFlag;

            set
            {
                if (_PledgeFlag != value)
                {
                    OnPledgeFlagChanging(value);
                    SendPropertyChanging();
                    _PledgeFlag = value;
                    SendPropertyChanged("PledgeFlag");
                    OnPledgeFlagChanged();
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

        [Column(Name = "PostingDate", UpdateCheck = UpdateCheck.Never, Storage = "_PostingDate", DbType = "datetime")]
        public DateTime? PostingDate
        {
            get => _PostingDate;

            set
            {
                if (_PostingDate != value)
                {
                    OnPostingDateChanging(value);
                    SendPropertyChanging();
                    _PostingDate = value;
                    SendPropertyChanged("PostingDate");
                    OnPostingDateChanged();
                }
            }
        }

        [Column(Name = "BankAccount", UpdateCheck = UpdateCheck.Never, Storage = "_BankAccount", DbType = "nvarchar(250)")]
        public string BankAccount
        {
            get => _BankAccount;

            set
            {
                if (_BankAccount != value)
                {
                    OnBankAccountChanging(value);
                    SendPropertyChanging();
                    _BankAccount = value;
                    SendPropertyChanged("BankAccount");
                    OnBankAccountChanged();
                }
            }
        }

        [Column(Name = "ExtraDataId", UpdateCheck = UpdateCheck.Never, Storage = "_ExtraDataId", DbType = "int")]
        [IsForeignKey]
        public int? ExtraDataId
        {
            get => _ExtraDataId;

            set
            {
                if (_ExtraDataId != value)
                {
                    if (_ExtraDatum.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnExtraDataIdChanging(value);
                    SendPropertyChanging();
                    _ExtraDataId = value;
                    SendPropertyChanged("ExtraDataId");
                    OnExtraDataIdChanged();
                }
            }
        }

        [Column(Name = "CheckNo", UpdateCheck = UpdateCheck.Never, Storage = "_CheckNo", DbType = "nvarchar(20)")]
        public string CheckNo
        {
            get => _CheckNo;

            set
            {
                if (_CheckNo != value)
                {
                    OnCheckNoChanging(value);
                    SendPropertyChanging();
                    _CheckNo = value;
                    SendPropertyChanged("CheckNo");
                    OnCheckNoChanged();
                }
            }
        }

        [Column(Name = "QBSyncID", UpdateCheck = UpdateCheck.Never, Storage = "_QBSyncID", DbType = "int")]
        public int? QBSyncID
        {
            get => _QBSyncID;

            set
            {
                if (_QBSyncID != value)
                {
                    OnQBSyncIDChanging(value);
                    SendPropertyChanging();
                    _QBSyncID = value;
                    SendPropertyChanged("QBSyncID");
                    OnQBSyncIDChanged();
                }
            }
        }

        [Column(Name = "TranId", UpdateCheck = UpdateCheck.Never, Storage = "_TranId", DbType = "int")]
        public int? TranId
        {
            get => _TranId;

            set
            {
                if (_TranId != value)
                {
                    OnTranIdChanging(value);
                    SendPropertyChanging();
                    _TranId = value;
                    SendPropertyChanged("TranId");
                    OnTranIdChanged();
                }
            }
        }

        [Column(Name = "Source", UpdateCheck = UpdateCheck.Never, Storage = "_Source", DbType = "int")]
        public int? Source
        {
            get => _Source;

            set
            {
                if (_Source != value)
                {
                    OnSourceChanging(value);
                    SendPropertyChanging();
                    _Source = value;
                    SendPropertyChanged("Source");
                    OnSourceChanged();
                }
            }
        }

        [Column(Name = "CampusId", UpdateCheck = UpdateCheck.Never, Storage = "_CampusId", DbType = "int")]
        public int? CampusId
        {
            get => _CampusId;

            set
            {
                if (_CampusId != value)
                {
                    OnCampusIdChanging(value);
                    SendPropertyChanging();
                    _CampusId = value;
                    SendPropertyChanged("CampusId");
                    OnCampusIdChanged();
                }
            }
        }

        [Column(Name = "ImageID", UpdateCheck = UpdateCheck.Never, Storage = "_ImageID", DbType = "int NOT NULL")]
        public int ImageID
        {
            get => _ImageID;

            set
            {
                if (_ImageID != value)
                {
                    OnImageIDChanging(value);
                    SendPropertyChanging();
                    _ImageID = value;
                    SendPropertyChanged("ImageID");
                    OnImageIDChanged();
                }
            }
        }

        [Column(Name = "MetaInfo", UpdateCheck = UpdateCheck.Never, Storage = "_MetaInfo", DbType = "varchar(100)")]
        public string MetaInfo
        {
            get => _MetaInfo;

            set
            {
                if (_MetaInfo != value)
                {
                    OnMetaInfoChanging(value);
                    SendPropertyChanging();
                    _MetaInfo = value;
                    SendPropertyChanged("MetaInfo");
                    OnMetaInfoChanged();
                }
            }
        }

        [Column(Name = "Origin", UpdateCheck = UpdateCheck.Never, Storage = "_Origin", DbType = "int NOT NULL")]
        public int Origin
        {
            get => _Origin;

            set
            {
                if (_Origin != value)
                {
                    OnOriginChanging(value);
                    SendPropertyChanging();
                    _Origin = value;
                    SendPropertyChanged("Origin");
                    OnOriginChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "BUNDLE_DETAIL_CONTR_FK", Storage = "_BundleDetails", OtherKey = "ContributionId")]
        public EntitySet<BundleDetail> BundleDetails
           {
               get => _BundleDetails;

            set => _BundleDetails.Assign(value);

           }

        [Association(Name = "FK_ContributionTag_Contribution", Storage = "_ContributionTags", OtherKey = "ContributionId")]
        public EntitySet<ContributionTag> ContributionTags
           {
               get => _ContributionTags;

            set => _ContributionTags.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Contribution_ContributionFund", Storage = "_ContributionFund", ThisKey = "FundId", IsForeignKey = true)]
        public ContributionFund ContributionFund
        {
            get => _ContributionFund.Entity;

            set
            {
                ContributionFund previousValue = _ContributionFund.Entity;
                if (((previousValue != value)
                            || (_ContributionFund.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ContributionFund.Entity = null;
                        previousValue.Contributions.Remove(this);
                    }

                    _ContributionFund.Entity = value;
                    if (value != null)
                    {
                        value.Contributions.Add(this);

                        _FundId = value.FundId;

                    }

                    else
                    {
                        _FundId = default(int);

                    }

                    SendPropertyChanged("ContributionFund");
                }
            }
        }

        [Association(Name = "FK_Contribution_ContributionStatus", Storage = "_ContributionStatus", ThisKey = "ContributionStatusId", IsForeignKey = true)]
        public ContributionStatus ContributionStatus
        {
            get => _ContributionStatus.Entity;

            set
            {
                ContributionStatus previousValue = _ContributionStatus.Entity;
                if (((previousValue != value)
                            || (_ContributionStatus.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ContributionStatus.Entity = null;
                        previousValue.Contributions.Remove(this);
                    }

                    _ContributionStatus.Entity = value;
                    if (value != null)
                    {
                        value.Contributions.Add(this);

                        _ContributionStatusId = value.Id;

                    }

                    else
                    {
                        _ContributionStatusId = default(int?);

                    }

                    SendPropertyChanged("ContributionStatus");
                }
            }
        }

        [Association(Name = "FK_Contribution_ContributionType", Storage = "_ContributionType", ThisKey = "ContributionTypeId", IsForeignKey = true)]
        public ContributionType ContributionType
        {
            get => _ContributionType.Entity;

            set
            {
                ContributionType previousValue = _ContributionType.Entity;
                if (((previousValue != value)
                            || (_ContributionType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ContributionType.Entity = null;
                        previousValue.Contributions.Remove(this);
                    }

                    _ContributionType.Entity = value;
                    if (value != null)
                    {
                        value.Contributions.Add(this);

                        _ContributionTypeId = value.Id;

                    }

                    else
                    {
                        _ContributionTypeId = default(int);

                    }

                    SendPropertyChanged("ContributionType");
                }
            }
        }

        [Association(Name = "FK_Contribution_ExtraData", Storage = "_ExtraDatum", ThisKey = "ExtraDataId", IsForeignKey = true)]
        public ExtraDatum ExtraDatum
        {
            get => _ExtraDatum.Entity;

            set
            {
                ExtraDatum previousValue = _ExtraDatum.Entity;
                if (((previousValue != value)
                            || (_ExtraDatum.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ExtraDatum.Entity = null;
                        previousValue.Contributions.Remove(this);
                    }

                    _ExtraDatum.Entity = value;
                    if (value != null)
                    {
                        value.Contributions.Add(this);

                        _ExtraDataId = value.Id;

                    }

                    else
                    {
                        _ExtraDataId = default(int?);

                    }

                    SendPropertyChanged("ExtraDatum");
                }
            }
        }

        [Association(Name = "FK_Contribution_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
        public Person Person
        {
            get => _Person.Entity;

            set
            {
                Person previousValue = _Person.Entity;
                if (((previousValue != value)
                            || (_Person.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Person.Entity = null;
                        previousValue.Contributions.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.Contributions.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int?);

                    }

                    SendPropertyChanged("Person");
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
            entity.Contribution = this;
        }

        private void detach_BundleDetails(BundleDetail entity)
        {
            SendPropertyChanging();
            entity.Contribution = null;
        }

        private void attach_ContributionTags(ContributionTag entity)
        {
            SendPropertyChanging();
            entity.Contribution = this;
        }

        private void detach_ContributionTags(ContributionTag entity)
        {
            SendPropertyChanging();
            entity.Contribution = null;
        }
    }
}
