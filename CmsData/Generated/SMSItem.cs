using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.SMSItems")]
    public partial class SMSItem : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _ListID;

        private int _PeopleID;

        private bool _Sent;

        private string _Number;

        private bool _NoNumber;

        private bool _NoOptIn;

        private string _ResultStatus;

        private string _ErrorMessage;

        private EntityRef<Person> _Person;

        private EntityRef<SMSList> _SMSList;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnListIDChanging(int value);
        partial void OnListIDChanged();

        partial void OnPeopleIDChanging(int value);
        partial void OnPeopleIDChanged();

        partial void OnSentChanging(bool value);
        partial void OnSentChanged();

        partial void OnNumberChanging(string value);
        partial void OnNumberChanged();

        partial void OnNoNumberChanging(bool value);
        partial void OnNoNumberChanged();

        partial void OnNoOptInChanging(bool value);
        partial void OnNoOptInChanged();

        partial void OnResultStatusChanging(string value);
        partial void OnResultStatusChanged();

        partial void OnErrorMessageChanging(string value);
        partial void OnErrorMessageChanged();

        #endregion

        public SMSItem()
        {
            _Person = default(EntityRef<Person>);

            _SMSList = default(EntityRef<SMSList>);

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

        [Column(Name = "ListID", UpdateCheck = UpdateCheck.Never, Storage = "_ListID", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int ListID
        {
            get => _ListID;

            set
            {
                if (_ListID != value)
                {
                    if (_SMSList.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnListIDChanging(value);
                    SendPropertyChanging();
                    _ListID = value;
                    SendPropertyChanged("ListID");
                    OnListIDChanged();
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

        [Column(Name = "Sent", UpdateCheck = UpdateCheck.Never, Storage = "_Sent", DbType = "bit NOT NULL")]
        public bool Sent
        {
            get => _Sent;

            set
            {
                if (_Sent != value)
                {
                    OnSentChanging(value);
                    SendPropertyChanging();
                    _Sent = value;
                    SendPropertyChanged("Sent");
                    OnSentChanged();
                }
            }
        }

        [Column(Name = "Number", UpdateCheck = UpdateCheck.Never, Storage = "_Number", DbType = "nvarchar(25) NOT NULL")]
        public string Number
        {
            get => _Number;

            set
            {
                if (_Number != value)
                {
                    OnNumberChanging(value);
                    SendPropertyChanging();
                    _Number = value;
                    SendPropertyChanged("Number");
                    OnNumberChanged();
                }
            }
        }

        [Column(Name = "NoNumber", UpdateCheck = UpdateCheck.Never, Storage = "_NoNumber", DbType = "bit NOT NULL")]
        public bool NoNumber
        {
            get => _NoNumber;

            set
            {
                if (_NoNumber != value)
                {
                    OnNoNumberChanging(value);
                    SendPropertyChanging();
                    _NoNumber = value;
                    SendPropertyChanged("NoNumber");
                    OnNoNumberChanged();
                }
            }
        }

        [Column(Name = "NoOptIn", UpdateCheck = UpdateCheck.Never, Storage = "_NoOptIn", DbType = "bit NOT NULL")]
        public bool NoOptIn
        {
            get => _NoOptIn;

            set
            {
                if (_NoOptIn != value)
                {
                    OnNoOptInChanging(value);
                    SendPropertyChanging();
                    _NoOptIn = value;
                    SendPropertyChanged("NoOptIn");
                    OnNoOptInChanged();
                }
            }
        }

        [Column(Name = "ResultStatus", UpdateCheck = UpdateCheck.WhenChanged, Storage = "_ResultStatus", DbType = "varchar(50)")]
        public string ResultStatus
        {
            get => _ResultStatus;

            set
            {
                if (_ResultStatus != value)
                {
                    OnResultStatusChanging(value);
                    SendPropertyChanging();
                    _ResultStatus = value;
                    SendPropertyChanged("ResultStatus");
                    OnResultStatusChanged();
                }
            }
        }

        [Column(Name = "ErrorMessage", UpdateCheck = UpdateCheck.WhenChanged, Storage = "_ErrorMessage", DbType = "varchar(300)")]
        public string ErrorMessage
        {
            get => _ErrorMessage;

            set
            {
                if (_ErrorMessage != value)
                {
                    OnErrorMessageChanging(value);
                    SendPropertyChanging();
                    _ErrorMessage = value;
                    SendPropertyChanged("ErrorMessage");
                    OnErrorMessageChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_SMSItems_People", Storage = "_Person", ThisKey = "PeopleID", IsForeignKey = true)]
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
                        previousValue.SMSItems.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.SMSItems.Add(this);

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

        [Association(Name = "FK_SMSItems_SMSList", Storage = "_SMSList", ThisKey = "ListID", IsForeignKey = true)]
        public SMSList SMSList
        {
            get => _SMSList.Entity;

            set
            {
                SMSList previousValue = _SMSList.Entity;
                if (((previousValue != value)
                            || (_SMSList.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _SMSList.Entity = null;
                        previousValue.SMSItems.Remove(this);
                    }

                    _SMSList.Entity = value;
                    if (value != null)
                    {
                        value.SMSItems.Add(this);

                        _ListID = value.Id;

                    }

                    else
                    {
                        _ListID = default(int);

                    }

                    SendPropertyChanged("SMSList");
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
