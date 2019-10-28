using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "CMS_VOLUNTEER.APPLICATION_PREREQ_TBL")]
    public partial class ApplicationPrereq : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _ApplicationPrereqId;

        private int? _ApplicationId;

        private int? _PrerequisiteId;

        private int? _CheckedBy;

        private DateTime? _CheckedDate;

        private DateTime? _DateSatisfied;

        private int? _CreatedBy;

        private DateTime? _CreatedDate;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private string _RecordStatus;

        private int? _ChurchId;

        private int? _Status;

        private string _Comments;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnApplicationPrereqIdChanging(int value);
        partial void OnApplicationPrereqIdChanged();

        partial void OnApplicationIdChanging(int? value);
        partial void OnApplicationIdChanged();

        partial void OnPrerequisiteIdChanging(int? value);
        partial void OnPrerequisiteIdChanged();

        partial void OnCheckedByChanging(int? value);
        partial void OnCheckedByChanged();

        partial void OnCheckedDateChanging(DateTime? value);
        partial void OnCheckedDateChanged();

        partial void OnDateSatisfiedChanging(DateTime? value);
        partial void OnDateSatisfiedChanged();

        partial void OnCreatedByChanging(int? value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime? value);
        partial void OnCreatedDateChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        partial void OnRecordStatusChanging(string value);
        partial void OnRecordStatusChanged();

        partial void OnChurchIdChanging(int? value);
        partial void OnChurchIdChanged();

        partial void OnStatusChanging(int? value);
        partial void OnStatusChanged();

        partial void OnCommentsChanging(string value);
        partial void OnCommentsChanged();

        #endregion

        public ApplicationPrereq()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "APPLICATION_PREREQ_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ApplicationPrereqId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int ApplicationPrereqId
        {
            get => _ApplicationPrereqId;

            set
            {
                if (_ApplicationPrereqId != value)
                {
                    OnApplicationPrereqIdChanging(value);
                    SendPropertyChanging();
                    _ApplicationPrereqId = value;
                    SendPropertyChanged("ApplicationPrereqId");
                    OnApplicationPrereqIdChanged();
                }
            }
        }

        [Column(Name = "APPLICATION_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ApplicationId", DbType = "int")]
        public int? ApplicationId
        {
            get => _ApplicationId;

            set
            {
                if (_ApplicationId != value)
                {
                    OnApplicationIdChanging(value);
                    SendPropertyChanging();
                    _ApplicationId = value;
                    SendPropertyChanged("ApplicationId");
                    OnApplicationIdChanged();
                }
            }
        }

        [Column(Name = "PREREQUISITE_ID", UpdateCheck = UpdateCheck.Never, Storage = "_PrerequisiteId", DbType = "int")]
        public int? PrerequisiteId
        {
            get => _PrerequisiteId;

            set
            {
                if (_PrerequisiteId != value)
                {
                    OnPrerequisiteIdChanging(value);
                    SendPropertyChanging();
                    _PrerequisiteId = value;
                    SendPropertyChanged("PrerequisiteId");
                    OnPrerequisiteIdChanged();
                }
            }
        }

        [Column(Name = "CHECKED_BY", UpdateCheck = UpdateCheck.Never, Storage = "_CheckedBy", DbType = "int")]
        public int? CheckedBy
        {
            get => _CheckedBy;

            set
            {
                if (_CheckedBy != value)
                {
                    OnCheckedByChanging(value);
                    SendPropertyChanging();
                    _CheckedBy = value;
                    SendPropertyChanged("CheckedBy");
                    OnCheckedByChanged();
                }
            }
        }

        [Column(Name = "CHECKED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_CheckedDate", DbType = "datetime")]
        public DateTime? CheckedDate
        {
            get => _CheckedDate;

            set
            {
                if (_CheckedDate != value)
                {
                    OnCheckedDateChanging(value);
                    SendPropertyChanging();
                    _CheckedDate = value;
                    SendPropertyChanged("CheckedDate");
                    OnCheckedDateChanged();
                }
            }
        }

        [Column(Name = "DATE_SATISFIED", UpdateCheck = UpdateCheck.Never, Storage = "_DateSatisfied", DbType = "datetime")]
        public DateTime? DateSatisfied
        {
            get => _DateSatisfied;

            set
            {
                if (_DateSatisfied != value)
                {
                    OnDateSatisfiedChanging(value);
                    SendPropertyChanging();
                    _DateSatisfied = value;
                    SendPropertyChanged("DateSatisfied");
                    OnDateSatisfiedChanged();
                }
            }
        }

        [Column(Name = "CREATED_BY", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int")]
        public int? CreatedBy
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

        [Column(Name = "CREATED_DATE", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime? CreatedDate
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

        [Column(Name = "RECORD_STATUS", UpdateCheck = UpdateCheck.Never, Storage = "_RecordStatus", DbType = "varchar(1)")]
        public string RecordStatus
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

        [Column(Name = "CHURCH_ID", UpdateCheck = UpdateCheck.Never, Storage = "_ChurchId", DbType = "int")]
        public int? ChurchId
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

        [Column(Name = "STATUS", UpdateCheck = UpdateCheck.Never, Storage = "_Status", DbType = "int")]
        public int? Status
        {
            get => _Status;

            set
            {
                if (_Status != value)
                {
                    OnStatusChanging(value);
                    SendPropertyChanging();
                    _Status = value;
                    SendPropertyChanged("Status");
                    OnStatusChanged();
                }
            }
        }

        [Column(Name = "COMMENTS", UpdateCheck = UpdateCheck.Never, Storage = "_Comments", DbType = "varchar(4000)")]
        public string Comments
        {
            get => _Comments;

            set
            {
                if (_Comments != value)
                {
                    OnCommentsChanging(value);
                    SendPropertyChanging();
                    _Comments = value;
                    SendPropertyChanged("Comments");
                    OnCommentsChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

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
    }
}
