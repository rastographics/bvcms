using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Preferences")]
    public partial class Preference : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _UserId;

        private string _PreferenceX;

        private string _ValueX;

        private EntityRef<User> _User;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnUserIdChanging(int value);
        partial void OnUserIdChanged();

        partial void OnPreferenceXChanging(string value);
        partial void OnPreferenceXChanged();

        partial void OnValueXChanging(string value);
        partial void OnValueXChanged();

        #endregion

        public Preference()
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

        [Column(Name = "Preference", UpdateCheck = UpdateCheck.Never, Storage = "_PreferenceX", DbType = "nvarchar(30) NOT NULL", IsPrimaryKey = true)]
        public string PreferenceX
        {
            get => _PreferenceX;

            set
            {
                if (_PreferenceX != value)
                {
                    OnPreferenceXChanging(value);
                    SendPropertyChanging();
                    _PreferenceX = value;
                    SendPropertyChanged("PreferenceX");
                    OnPreferenceXChanged();
                }
            }
        }

        [Column(Name = "Value", UpdateCheck = UpdateCheck.Never, Storage = "_ValueX", DbType = "nvarchar")]
        public string ValueX
        {
            get => _ValueX;

            set
            {
                if (_ValueX != value)
                {
                    OnValueXChanging(value);
                    SendPropertyChanging();
                    _ValueX = value;
                    SendPropertyChanged("ValueX");
                    OnValueXChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_UserPreferences_Users", Storage = "_User", ThisKey = "UserId", IsForeignKey = true)]
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
                        previousValue.Preferences.Remove(this);
                    }

                    _User.Entity = value;
                    if (value != null)
                    {
                        value.Preferences.Add(this);

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
