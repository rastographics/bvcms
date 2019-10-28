using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MobileAppDevices")]
    public partial class MobileAppDevice : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private DateTime _Created;

        private DateTime _LastSeen;

        private int _DeviceTypeID;

        private string _InstanceID;

        private string _NotificationID;

        private int? _UserID;

        private int? _PeopleID;

        private string _Authentication;

        private string _Code;

        private DateTime _CodeExpires;

        private string _CodeEmail;

        private EntityRef<Person> _Person;

        private EntityRef<User> _User;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnCreatedChanging(DateTime value);
        partial void OnCreatedChanged();

        partial void OnLastSeenChanging(DateTime value);
        partial void OnLastSeenChanged();

        partial void OnDeviceTypeIDChanging(int value);
        partial void OnDeviceTypeIDChanged();

        partial void OnInstanceIDChanging(string value);
        partial void OnInstanceIDChanged();

        partial void OnNotificationIDChanging(string value);
        partial void OnNotificationIDChanged();

        partial void OnUserIDChanging(int? value);
        partial void OnUserIDChanged();

        partial void OnPeopleIDChanging(int? value);
        partial void OnPeopleIDChanged();

        partial void OnAuthenticationChanging(string value);
        partial void OnAuthenticationChanged();

        partial void OnCodeChanging(string value);
        partial void OnCodeChanged();

        partial void OnCodeExpiresChanging(DateTime value);
        partial void OnCodeExpiresChanged();

        partial void OnCodeEmailChanging(string value);
        partial void OnCodeEmailChanged();

        #endregion

        public MobileAppDevice()
        {
            _Person = default(EntityRef<Person>);

            _User = default(EntityRef<User>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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

        [Column(Name = "created", UpdateCheck = UpdateCheck.Never, Storage = "_Created", DbType = "datetime NOT NULL")]
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

        [Column(Name = "lastSeen", UpdateCheck = UpdateCheck.Never, Storage = "_LastSeen", DbType = "datetime NOT NULL")]
        public DateTime LastSeen
        {
            get => _LastSeen;

            set
            {
                if (_LastSeen != value)
                {
                    OnLastSeenChanging(value);
                    SendPropertyChanging();
                    _LastSeen = value;
                    SendPropertyChanged("LastSeen");
                    OnLastSeenChanged();
                }
            }
        }

        [Column(Name = "deviceTypeID", UpdateCheck = UpdateCheck.Never, Storage = "_DeviceTypeID", DbType = "int NOT NULL")]
        public int DeviceTypeID
        {
            get => _DeviceTypeID;

            set
            {
                if (_DeviceTypeID != value)
                {
                    OnDeviceTypeIDChanging(value);
                    SendPropertyChanging();
                    _DeviceTypeID = value;
                    SendPropertyChanged("DeviceTypeID");
                    OnDeviceTypeIDChanged();
                }
            }
        }

        [Column(Name = "instanceID", UpdateCheck = UpdateCheck.Never, Storage = "_InstanceID", DbType = "varchar(255) NOT NULL")]
        public string InstanceID
        {
            get => _InstanceID;

            set
            {
                if (_InstanceID != value)
                {
                    OnInstanceIDChanging(value);
                    SendPropertyChanging();
                    _InstanceID = value;
                    SendPropertyChanged("InstanceID");
                    OnInstanceIDChanged();
                }
            }
        }

        [Column(Name = "notificationID", UpdateCheck = UpdateCheck.Never, Storage = "_NotificationID", DbType = "varchar NOT NULL")]
        public string NotificationID
        {
            get => _NotificationID;

            set
            {
                if (_NotificationID != value)
                {
                    OnNotificationIDChanging(value);
                    SendPropertyChanging();
                    _NotificationID = value;
                    SendPropertyChanged("NotificationID");
                    OnNotificationIDChanged();
                }
            }
        }

        [Column(Name = "userID", UpdateCheck = UpdateCheck.Never, Storage = "_UserID", DbType = "int")]
        [IsForeignKey]
        public int? UserID
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

        [Column(Name = "peopleID", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleID", DbType = "int")]
        [IsForeignKey]
        public int? PeopleID
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

        [Column(Name = "authentication", UpdateCheck = UpdateCheck.Never, Storage = "_Authentication", DbType = "varchar(64) NOT NULL")]
        public string Authentication
        {
            get => _Authentication;

            set
            {
                if (_Authentication != value)
                {
                    OnAuthenticationChanging(value);
                    SendPropertyChanging();
                    _Authentication = value;
                    SendPropertyChanged("Authentication");
                    OnAuthenticationChanged();
                }
            }
        }

        [Column(Name = "code", UpdateCheck = UpdateCheck.Never, Storage = "_Code", DbType = "varchar(64) NOT NULL")]
        public string Code
        {
            get => _Code;

            set
            {
                if (_Code != value)
                {
                    OnCodeChanging(value);
                    SendPropertyChanging();
                    _Code = value;
                    SendPropertyChanged("Code");
                    OnCodeChanged();
                }
            }
        }

        [Column(Name = "codeExpires", UpdateCheck = UpdateCheck.Never, Storage = "_CodeExpires", DbType = "datetime NOT NULL")]
        public DateTime CodeExpires
        {
            get => _CodeExpires;

            set
            {
                if (_CodeExpires != value)
                {
                    OnCodeExpiresChanging(value);
                    SendPropertyChanging();
                    _CodeExpires = value;
                    SendPropertyChanged("CodeExpires");
                    OnCodeExpiresChanged();
                }
            }
        }

        [Column(Name = "codeEmail", UpdateCheck = UpdateCheck.Never, Storage = "_CodeEmail", DbType = "varchar(255) NOT NULL")]
        public string CodeEmail
        {
            get => _CodeEmail;

            set
            {
                if (_CodeEmail != value)
                {
                    OnCodeEmailChanging(value);
                    SendPropertyChanging();
                    _CodeEmail = value;
                    SendPropertyChanged("CodeEmail");
                    OnCodeEmailChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_MobileAppDevices_People", Storage = "_Person", ThisKey = "PeopleID", IsForeignKey = true)]
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
                        previousValue.MobileAppDevices.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.MobileAppDevices.Add(this);

                        _PeopleID = value.PeopleId;

                    }

                    else
                    {
                        _PeopleID = default(int?);

                    }

                    SendPropertyChanged("Person");
                }
            }
        }

        [Association(Name = "FK_MobileAppDevices_Users", Storage = "_User", ThisKey = "UserID", IsForeignKey = true)]
        public User User
        {
            get => _User.Entity;

            set
            {
                User previousValue = _User.Entity;
                if (((previousValue != value)
                            || (_User.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _User.Entity = null;
                        previousValue.MobileAppDevices.Remove(this);
                    }

                    _User.Entity = value;
                    if (value != null)
                    {
                        value.MobileAppDevices.Add(this);

                        _UserID = value.UserId;

                    }

                    else
                    {
                        _UserID = default(int?);

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
