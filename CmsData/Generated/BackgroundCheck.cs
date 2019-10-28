using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.BackgroundChecks")]
    public partial class BackgroundCheck : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private DateTime _Created;

        private DateTime _Updated;

        private int _UserID;

        private int _StatusID;

        private int _PeopleID;

        private string _ServiceCode;

        private int _ReportID;

        private string _ReportLink;

        private int _IssueCount;

        private string _ErrorMessages;

        private int _ReportTypeID;

        private int _ReportLabelID;

        private EntityRef<Person> _Person;

        private EntityRef<Person> _User;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnCreatedChanging(DateTime value);
        partial void OnCreatedChanged();

        partial void OnUpdatedChanging(DateTime value);
        partial void OnUpdatedChanged();

        partial void OnUserIDChanging(int value);
        partial void OnUserIDChanged();

        partial void OnStatusIDChanging(int value);
        partial void OnStatusIDChanged();

        partial void OnPeopleIDChanging(int value);
        partial void OnPeopleIDChanged();

        partial void OnServiceCodeChanging(string value);
        partial void OnServiceCodeChanged();

        partial void OnReportIDChanging(int value);
        partial void OnReportIDChanged();

        partial void OnReportLinkChanging(string value);
        partial void OnReportLinkChanged();

        partial void OnIssueCountChanging(int value);
        partial void OnIssueCountChanged();

        partial void OnErrorMessagesChanging(string value);
        partial void OnErrorMessagesChanged();

        partial void OnReportTypeIDChanging(int value);
        partial void OnReportTypeIDChanged();

        partial void OnReportLabelIDChanging(int value);
        partial void OnReportLabelIDChanged();

        #endregion

        public BackgroundCheck()
        {
            _Person = default(EntityRef<Person>);

            _User = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "ID", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    OnIdChanging(value);
                    SendPropertyChanging();
                    _Id = value;
                    SendPropertyChanged("Id");
                    OnIdChanged();
                }
            }
        }

        [Column(Name = "Created", UpdateCheck = UpdateCheck.Never, Storage = "_Created", DbType = "datetime NOT NULL")]
        public DateTime Created
        {
            get => _Created;

            set
            {
                if (_Created != value)
                {
                    OnCreatedChanging(value);
                    SendPropertyChanging();
                    _Created = value;
                    SendPropertyChanged("Created");
                    OnCreatedChanged();
                }
            }
        }

        [Column(Name = "Updated", UpdateCheck = UpdateCheck.Never, Storage = "_Updated", DbType = "datetime NOT NULL")]
        public DateTime Updated
        {
            get => _Updated;

            set
            {
                if (_Updated != value)
                {
                    OnUpdatedChanging(value);
                    SendPropertyChanging();
                    _Updated = value;
                    SendPropertyChanged("Updated");
                    OnUpdatedChanged();
                }
            }
        }

        [Column(Name = "UserID", UpdateCheck = UpdateCheck.Never, Storage = "_UserID", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int UserID
        {
            get => _UserID;

            set
            {
                if (_UserID != value)
                {
                    if (_User.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnUserIDChanging(value);
                    SendPropertyChanging();
                    _UserID = value;
                    SendPropertyChanged("UserID");
                    OnUserIDChanged();
                }
            }
        }

        [Column(Name = "StatusID", UpdateCheck = UpdateCheck.Never, Storage = "_StatusID", DbType = "int NOT NULL")]
        public int StatusID
        {
            get => _StatusID;

            set
            {
                if (_StatusID != value)
                {
                    OnStatusIDChanging(value);
                    SendPropertyChanging();
                    _StatusID = value;
                    SendPropertyChanged("StatusID");
                    OnStatusIDChanged();
                }
            }
        }

        [Column(Name = "PeopleID", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleID", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int PeopleID
        {
            get => _PeopleID;

            set
            {
                if (_PeopleID != value)
                {
                    if (_Person.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPeopleIDChanging(value);
                    SendPropertyChanging();
                    _PeopleID = value;
                    SendPropertyChanged("PeopleID");
                    OnPeopleIDChanged();
                }
            }
        }

        [Column(Name = "ServiceCode", UpdateCheck = UpdateCheck.Never, Storage = "_ServiceCode", DbType = "nvarchar(25) NOT NULL")]
        public string ServiceCode
        {
            get => _ServiceCode;

            set
            {
                if (_ServiceCode != value)
                {
                    OnServiceCodeChanging(value);
                    SendPropertyChanging();
                    _ServiceCode = value;
                    SendPropertyChanged("ServiceCode");
                    OnServiceCodeChanged();
                }
            }
        }

        [Column(Name = "ReportID", UpdateCheck = UpdateCheck.Never, Storage = "_ReportID", DbType = "int NOT NULL")]
        public int ReportID
        {
            get => _ReportID;

            set
            {
                if (_ReportID != value)
                {
                    OnReportIDChanging(value);
                    SendPropertyChanging();
                    _ReportID = value;
                    SendPropertyChanged("ReportID");
                    OnReportIDChanged();
                }
            }
        }

        [Column(Name = "ReportLink", UpdateCheck = UpdateCheck.Never, Storage = "_ReportLink", DbType = "nvarchar(255)")]
        public string ReportLink
        {
            get => _ReportLink;

            set
            {
                if (_ReportLink != value)
                {
                    OnReportLinkChanging(value);
                    SendPropertyChanging();
                    _ReportLink = value;
                    SendPropertyChanged("ReportLink");
                    OnReportLinkChanged();
                }
            }
        }

        [Column(Name = "IssueCount", UpdateCheck = UpdateCheck.Never, Storage = "_IssueCount", DbType = "int NOT NULL")]
        public int IssueCount
        {
            get => _IssueCount;

            set
            {
                if (_IssueCount != value)
                {
                    OnIssueCountChanging(value);
                    SendPropertyChanging();
                    _IssueCount = value;
                    SendPropertyChanged("IssueCount");
                    OnIssueCountChanged();
                }
            }
        }

        [Column(Name = "ErrorMessages", UpdateCheck = UpdateCheck.Never, Storage = "_ErrorMessages", DbType = "nvarchar")]
        public string ErrorMessages
        {
            get => _ErrorMessages;

            set
            {
                if (_ErrorMessages != value)
                {
                    OnErrorMessagesChanging(value);
                    SendPropertyChanging();
                    _ErrorMessages = value;
                    SendPropertyChanged("ErrorMessages");
                    OnErrorMessagesChanged();
                }
            }
        }

        [Column(Name = "ReportTypeID", UpdateCheck = UpdateCheck.Never, Storage = "_ReportTypeID", DbType = "int NOT NULL")]
        public int ReportTypeID
        {
            get => _ReportTypeID;

            set
            {
                if (_ReportTypeID != value)
                {
                    OnReportTypeIDChanging(value);
                    SendPropertyChanging();
                    _ReportTypeID = value;
                    SendPropertyChanged("ReportTypeID");
                    OnReportTypeIDChanged();
                }
            }
        }

        [Column(Name = "ReportLabelID", UpdateCheck = UpdateCheck.Never, Storage = "_ReportLabelID", DbType = "int NOT NULL")]
        public int ReportLabelID
        {
            get => _ReportLabelID;

            set
            {
                if (_ReportLabelID != value)
                {
                    OnReportLabelIDChanging(value);
                    SendPropertyChanging();
                    _ReportLabelID = value;
                    SendPropertyChanged("ReportLabelID");
                    OnReportLabelIDChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_BackgroundChecks_People", Storage = "_Person", ThisKey = "PeopleID", IsForeignKey = true)]
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
                        previousValue.BackgroundChecks.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.BackgroundChecks.Add(this);

                        _PeopleID = value.PeopleId;

                    }

                    else
                    {
                        _PeopleID = default(int);

                    }

                    SendPropertyChanged("Person");
                }
            }
        }

        [Association(Name = "People__User", Storage = "_User", ThisKey = "UserID", IsForeignKey = true)]
        public Person User
        {
            get => _User.Entity;

            set
            {
                Person previousValue = _User.Entity;
                if (((previousValue != value)
                            || (_User.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _User.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _User.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _UserID = value.PeopleId;

                    }

                    else
                    {
                        _UserID = default(int);

                    }

                    SendPropertyChanged("User");
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
    }
}
