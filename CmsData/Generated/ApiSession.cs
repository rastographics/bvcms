using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.ApiSession")]
    public partial class ApiSession : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _UserId;

        private Guid _SessionToken;

        private int? _Pin;

        private DateTime _LastAccessedDate;

        private DateTime _CreatedDate;

        private EntityRef<User> _User;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnUserIdChanging(int value);
        partial void OnUserIdChanged();

        partial void OnSessionTokenChanging(Guid value);
        partial void OnSessionTokenChanged();

        partial void OnPinChanging(int? value);
        partial void OnPinChanged();

        partial void OnLastAccessedDateChanging(DateTime value);
        partial void OnLastAccessedDateChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        #endregion

        public ApiSession()
        {
            _User = default(EntityRef<User>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "UserId", UpdateCheck = UpdateCheck.Never, Storage = "_UserId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int UserId
        {
            get => _UserId;

            set
            {
                if (_UserId != value)
                {
                    if (_User.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnUserIdChanging(value);
                    SendPropertyChanging();
                    _UserId = value;
                    SendPropertyChanged("UserId");
                    OnUserIdChanged();
                }
            }
        }

        [Column(Name = "SessionToken", UpdateCheck = UpdateCheck.Never, Storage = "_SessionToken", DbType = "uniqueidentifier NOT NULL")]
        public Guid SessionToken
        {
            get => _SessionToken;

            set
            {
                if (_SessionToken != value)
                {
                    OnSessionTokenChanging(value);
                    SendPropertyChanging();
                    _SessionToken = value;
                    SendPropertyChanged("SessionToken");
                    OnSessionTokenChanged();
                }
            }
        }

        [Column(Name = "PIN", UpdateCheck = UpdateCheck.Never, Storage = "_Pin", DbType = "int")]
        public int? Pin
        {
            get => _Pin;

            set
            {
                if (_Pin != value)
                {
                    OnPinChanging(value);
                    SendPropertyChanging();
                    _Pin = value;
                    SendPropertyChanged("Pin");
                    OnPinChanged();
                }
            }
        }

        [Column(Name = "LastAccessedDate", UpdateCheck = UpdateCheck.Never, Storage = "_LastAccessedDate", DbType = "datetime NOT NULL")]
        public DateTime LastAccessedDate
        {
            get => _LastAccessedDate;

            set
            {
                if (_LastAccessedDate != value)
                {
                    OnLastAccessedDateChanging(value);
                    SendPropertyChanging();
                    _LastAccessedDate = value;
                    SendPropertyChanged("LastAccessedDate");
                    OnLastAccessedDateChanged();
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

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Users_ApiSession", Storage = "_User", ThisKey = "UserId", IsForeignKey = true)]
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
                        previousValue.ApiSessions.Remove(this);
                    }

                    _User.Entity = value;
                    if (value != null)
                    {
                        value.ApiSessions.Add(this);

                        _UserId = value.UserId;

                    }

                    else
                    {
                        _UserId = default(int);

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
