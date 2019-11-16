using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "CMS_MAILINGS.DISTRIBUTION_LISTS_TBL")]
    public partial class DistributionList : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _DistributionListId;

        private int _CreatedBy;

        private DateTime _CreatedDate;

        private bool _RecordStatus;

        private string _DistributionListName;

        private string _DistributionListPurpose;

        private DateTime? _DateDeactivated;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private int? _MinistryId;

        private int? _UserId;

        private EntitySet<DistListMember> _DistListMembers;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnDistributionListIdChanging(int value);
        partial void OnDistributionListIdChanged();

        partial void OnCreatedByChanging(int value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        partial void OnRecordStatusChanging(bool value);
        partial void OnRecordStatusChanged();

        partial void OnDistributionListNameChanging(string value);
        partial void OnDistributionListNameChanged();

        partial void OnDistributionListPurposeChanging(string value);
        partial void OnDistributionListPurposeChanged();

        partial void OnDateDeactivatedChanging(DateTime? value);
        partial void OnDateDeactivatedChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        partial void OnMinistryIdChanging(int? value);
        partial void OnMinistryIdChanged();

        partial void OnUserIdChanging(int? value);
        partial void OnUserIdChanged();

        #endregion

        public DistributionList()
        {
            _DistListMembers = new EntitySet<DistListMember>(new Action<DistListMember>(attach_DistListMembers), new Action<DistListMember>(detach_DistListMembers));

            OnCreated();
        }

        #region Columns

        [Column(Name = "DISTRIBUTION_LIST_ID", UpdateCheck = UpdateCheck.Never, Storage = "_DistributionListId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        public int DistributionListId
        {
            get => _DistributionListId;

            set
            {
                if (_DistributionListId != value)
                {
                    OnDistributionListIdChanging(value);
                    SendPropertyChanging();
                    _DistributionListId = value;
                    SendPropertyChanged("DistributionListId");
                    OnDistributionListIdChanged();
                }
            }
        }

        [Column(Name = "CREATED_BY", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int NOT NULL")]
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

        [Column(Name = "CREATED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime NOT NULL")]
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

        [Column(Name = "RECORD_STATUS", UpdateCheck = UpdateCheck.Never, Storage = "_RecordStatus", DbType = "bit NOT NULL")]
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

        [Column(Name = "DISTRIBUTION_LIST_NAME", UpdateCheck = UpdateCheck.Never, Storage = "_DistributionListName", DbType = "varchar(25)")]
        public string DistributionListName
        {
            get => _DistributionListName;

            set
            {
                if (_DistributionListName != value)
                {
                    OnDistributionListNameChanging(value);
                    SendPropertyChanging();
                    _DistributionListName = value;
                    SendPropertyChanged("DistributionListName");
                    OnDistributionListNameChanged();
                }
            }
        }

        [Column(Name = "DISTRIBUTION_LIST_PURPOSE", UpdateCheck = UpdateCheck.Never, Storage = "_DistributionListPurpose", DbType = "varchar(256)")]
        public string DistributionListPurpose
        {
            get => _DistributionListPurpose;

            set
            {
                if (_DistributionListPurpose != value)
                {
                    OnDistributionListPurposeChanging(value);
                    SendPropertyChanging();
                    _DistributionListPurpose = value;
                    SendPropertyChanged("DistributionListPurpose");
                    OnDistributionListPurposeChanged();
                }
            }
        }

        [Column(Name = "DATE_DEACTIVATED", UpdateCheck = UpdateCheck.Never, Storage = "_DateDeactivated", DbType = "datetime")]
        public DateTime? DateDeactivated
        {
            get => _DateDeactivated;

            set
            {
                if (_DateDeactivated != value)
                {
                    OnDateDeactivatedChanging(value);
                    SendPropertyChanging();
                    _DateDeactivated = value;
                    SendPropertyChanged("DateDeactivated");
                    OnDateDeactivatedChanged();
                }
            }
        }

        [Column(Name = "MODIFIED_BY", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedBy", DbType = "int")]
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

        [Column(Name = "MODIFIED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedDate", DbType = "datetime")]
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

        [Column(Name = "MINISTRY_ID", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryId", DbType = "int")]
        public int? MinistryId
        {
            get => _MinistryId;

            set
            {
                if (_MinistryId != value)
                {
                    OnMinistryIdChanging(value);
                    SendPropertyChanging();
                    _MinistryId = value;
                    SendPropertyChanged("MinistryId");
                    OnMinistryIdChanged();
                }
            }
        }

        [Column(Name = "USER_ID", UpdateCheck = UpdateCheck.Never, Storage = "_UserId", DbType = "int")]
        public int? UserId
        {
            get => _UserId;

            set
            {
                if (_UserId != value)
                {
                    OnUserIdChanging(value);
                    SendPropertyChanging();
                    _UserId = value;
                    SendPropertyChanged("UserId");
                    OnUserIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "DIST_LIST_MEMBERS_DIST_LIST_FK", Storage = "_DistListMembers", OtherKey = "DistributionListId")]
        public EntitySet<DistListMember> DistListMembers
           {
               get => _DistListMembers;

            set => _DistListMembers.Assign(value);

           }

        #endregion

        #region Foreign Keys

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

        private void attach_DistListMembers(DistListMember entity)
        {
            SendPropertyChanging();
            entity.DistributionList = this;
        }

        private void detach_DistListMembers(DistListMember entity)
        {
            SendPropertyChanging();
            entity.DistributionList = null;
        }
    }
}
