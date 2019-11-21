using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MobileAppPushRegistrations")]
    public partial class MobileAppPushRegistration : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _Type;

        private int _PeopleId;

        private string _RegistrationId;

        private int _Priority;

        private bool _Enabled;

        private int _Rebranded;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnTypeChanging(int value);
        partial void OnTypeChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnRegistrationIdChanging(string value);
        partial void OnRegistrationIdChanged();

        partial void OnPriorityChanging(int value);
        partial void OnPriorityChanged();

        partial void OnEnabledChanging(bool value);
        partial void OnEnabledChanged();

        partial void OnRebrandedChanging(int value);
        partial void OnRebrandedChanged();

        #endregion

        public MobileAppPushRegistration()
        {
            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
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

        [Column(Name = "Type", UpdateCheck = UpdateCheck.Never, Storage = "_Type", DbType = "int NOT NULL")]
        public int Type
        {
            get => _Type;

            set
            {
                if (_Type != value)
                {
                    OnTypeChanging(value);
                    SendPropertyChanging();
                    _Type = value;
                    SendPropertyChanged("Type");
                    OnTypeChanged();
                }
            }
        }

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int PeopleId
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

        [Column(Name = "RegistrationId", UpdateCheck = UpdateCheck.Never, Storage = "_RegistrationId", DbType = "varchar NOT NULL")]
        public string RegistrationId
        {
            get => _RegistrationId;

            set
            {
                if (_RegistrationId != value)
                {
                    OnRegistrationIdChanging(value);
                    SendPropertyChanging();
                    _RegistrationId = value;
                    SendPropertyChanged("RegistrationId");
                    OnRegistrationIdChanged();
                }
            }
        }

        [Column(Name = "Priority", UpdateCheck = UpdateCheck.Never, Storage = "_Priority", DbType = "int NOT NULL")]
        public int Priority
        {
            get => _Priority;

            set
            {
                if (_Priority != value)
                {
                    OnPriorityChanging(value);
                    SendPropertyChanging();
                    _Priority = value;
                    SendPropertyChanged("Priority");
                    OnPriorityChanged();
                }
            }
        }

        [Column(Name = "Enabled", UpdateCheck = UpdateCheck.Never, Storage = "_Enabled", DbType = "bit NOT NULL")]
        public bool Enabled
        {
            get => _Enabled;

            set
            {
                if (_Enabled != value)
                {
                    OnEnabledChanging(value);
                    SendPropertyChanging();
                    _Enabled = value;
                    SendPropertyChanged("Enabled");
                    OnEnabledChanged();
                }
            }
        }

        [Column(Name = "rebranded", UpdateCheck = UpdateCheck.Never, Storage = "_Rebranded", DbType = "int NOT NULL")]
        public int Rebranded
        {
            get => _Rebranded;

            set
            {
                if (_Rebranded != value)
                {
                    OnRebrandedChanging(value);
                    SendPropertyChanging();
                    _Rebranded = value;
                    SendPropertyChanged("Rebranded");
                    OnRebrandedChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_MobileAppPushRegistrations_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.MobileAppPushRegistrations.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.MobileAppPushRegistrations.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int);

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
    }
}
