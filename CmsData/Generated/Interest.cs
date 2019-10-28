using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "CMS_VOLUNTEER.INTEREST_TBL")]
    public partial class Interest : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _InterestId;

        private int? _PositionId;

        private int? _InterestPeopleId;

        private int? _CreatedBy;

        private DateTime? _CreatedDate;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private string _RecordStatus;

        private int? _ChurchId;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnInterestIdChanging(int value);
        partial void OnInterestIdChanged();

        partial void OnPositionIdChanging(int? value);
        partial void OnPositionIdChanged();

        partial void OnInterestPeopleIdChanging(int? value);
        partial void OnInterestPeopleIdChanged();

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

        #endregion

        public Interest()
        {
            OnCreated();
        }

        #region Columns

        [Column(Name = "INTEREST_ID", UpdateCheck = UpdateCheck.Never, Storage = "_InterestId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int InterestId
        {
            get => _InterestId;

            set
            {
                if (_InterestId != value)
                {
                    OnInterestIdChanging(value);
                    SendPropertyChanging();
                    _InterestId = value;
                    SendPropertyChanged("InterestId");
                    OnInterestIdChanged();
                }
            }
        }

        [Column(Name = "POSITION_ID", UpdateCheck = UpdateCheck.Never, Storage = "_PositionId", DbType = "int")]
        public int? PositionId
        {
            get => _PositionId;

            set
            {
                if (_PositionId != value)
                {
                    OnPositionIdChanging(value);
                    SendPropertyChanging();
                    _PositionId = value;
                    SendPropertyChanged("PositionId");
                    OnPositionIdChanged();
                }
            }
        }

        [Column(Name = "INTEREST_PEOPLE_ID", UpdateCheck = UpdateCheck.Never, Storage = "_InterestPeopleId", DbType = "int")]
        public int? InterestPeopleId
        {
            get => _InterestPeopleId;

            set
            {
                if (_InterestPeopleId != value)
                {
                    OnInterestPeopleIdChanging(value);
                    SendPropertyChanging();
                    _InterestPeopleId = value;
                    SendPropertyChanged("InterestPeopleId");
                    OnInterestPeopleIdChanged();
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
