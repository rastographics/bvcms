using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Ministries")]
    public partial class Ministry : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _MinistryId;

        private string _MinistryName;

        private int? _CreatedBy;

        private DateTime? _CreatedDate;

        private int? _ModifiedBy;

        private DateTime? _ModifiedDate;

        private bool? _RecordStatus;

        private int? _DepartmentId;

        private string _MinistryDescription;

        private int? _MinistryContactId;

        private int? _ChurchId;

        private EntitySet<Contact> _Contacts;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnMinistryIdChanging(int value);
        partial void OnMinistryIdChanged();

        partial void OnMinistryNameChanging(string value);
        partial void OnMinistryNameChanged();

        partial void OnCreatedByChanging(int? value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime? value);
        partial void OnCreatedDateChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedDateChanging(DateTime? value);
        partial void OnModifiedDateChanged();

        partial void OnRecordStatusChanging(bool? value);
        partial void OnRecordStatusChanged();

        partial void OnDepartmentIdChanging(int? value);
        partial void OnDepartmentIdChanged();

        partial void OnMinistryDescriptionChanging(string value);
        partial void OnMinistryDescriptionChanged();

        partial void OnMinistryContactIdChanging(int? value);
        partial void OnMinistryContactIdChanged();

        partial void OnChurchIdChanging(int? value);
        partial void OnChurchIdChanged();

        #endregion

        public Ministry()
        {
            _Contacts = new EntitySet<Contact>(new Action<Contact>(attach_Contacts), new Action<Contact>(detach_Contacts));

            OnCreated();
        }

        #region Columns

        [Column(Name = "MinistryId", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int MinistryId
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

        [Column(Name = "MinistryName", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryName", DbType = "nvarchar(50)")]
        public string MinistryName
        {
            get => _MinistryName;

            set
            {
                if (_MinistryName != value)
                {
                    OnMinistryNameChanging(value);
                    SendPropertyChanging();
                    _MinistryName = value;
                    SendPropertyChanged("MinistryName");
                    OnMinistryNameChanged();
                }
            }
        }

        [Column(Name = "CreatedBy", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int")]
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

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime")]
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

        [Column(Name = "RecordStatus", UpdateCheck = UpdateCheck.Never, Storage = "_RecordStatus", DbType = "bit")]
        public bool? RecordStatus
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

        [Column(Name = "DepartmentId", UpdateCheck = UpdateCheck.Never, Storage = "_DepartmentId", DbType = "int")]
        public int? DepartmentId
        {
            get => _DepartmentId;

            set
            {
                if (_DepartmentId != value)
                {
                    OnDepartmentIdChanging(value);
                    SendPropertyChanging();
                    _DepartmentId = value;
                    SendPropertyChanged("DepartmentId");
                    OnDepartmentIdChanged();
                }
            }
        }

        [Column(Name = "MinistryDescription", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryDescription", DbType = "nvarchar(512)")]
        public string MinistryDescription
        {
            get => _MinistryDescription;

            set
            {
                if (_MinistryDescription != value)
                {
                    OnMinistryDescriptionChanging(value);
                    SendPropertyChanging();
                    _MinistryDescription = value;
                    SendPropertyChanged("MinistryDescription");
                    OnMinistryDescriptionChanged();
                }
            }
        }

        [Column(Name = "MinistryContactId", UpdateCheck = UpdateCheck.Never, Storage = "_MinistryContactId", DbType = "int")]
        public int? MinistryContactId
        {
            get => _MinistryContactId;

            set
            {
                if (_MinistryContactId != value)
                {
                    OnMinistryContactIdChanging(value);
                    SendPropertyChanging();
                    _MinistryContactId = value;
                    SendPropertyChanged("MinistryContactId");
                    OnMinistryContactIdChanged();
                }
            }
        }

        [Column(Name = "ChurchId", UpdateCheck = UpdateCheck.Never, Storage = "_ChurchId", DbType = "int")]
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

        [Association(Name = "FK_Contacts_Ministries", Storage = "_Contacts", OtherKey = "MinistryId")]
        public EntitySet<Contact> Contacts
           {
               get => _Contacts;

            set => _Contacts.Assign(value);

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

        private void attach_Contacts(Contact entity)
        {
            SendPropertyChanging();
            entity.Ministry = this;
        }

        private void detach_Contacts(Contact entity)
        {
            SendPropertyChanging();
            entity.Ministry = null;
        }
    }
}
