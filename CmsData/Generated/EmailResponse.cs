using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.EmailResponses")]
    public partial class EmailResponse : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _EmailQueueId;

        private int _PeopleId;

        private string _Type;

        private DateTime _Dt;

        private EntityRef<EmailQueue> _EmailQueue;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnEmailQueueIdChanging(int value);
        partial void OnEmailQueueIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnTypeChanging(string value);
        partial void OnTypeChanged();

        partial void OnDtChanging(DateTime value);
        partial void OnDtChanged();

        #endregion

        public EmailResponse()
        {
            _EmailQueue = default(EntityRef<EmailQueue>);

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

        [Column(Name = "EmailQueueId", UpdateCheck = UpdateCheck.Never, Storage = "_EmailQueueId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int EmailQueueId
        {
            get => _EmailQueueId;

            set
            {
                if (_EmailQueueId != value)
                {
                    if (_EmailQueue.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnEmailQueueIdChanging(value);
                    SendPropertyChanging();
                    _EmailQueueId = value;
                    SendPropertyChanged("EmailQueueId");
                    OnEmailQueueIdChanged();
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

        [Column(Name = "Type", UpdateCheck = UpdateCheck.Never, Storage = "_Type", DbType = "char(1) NOT NULL")]
        public string Type
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

        [Column(Name = "Dt", UpdateCheck = UpdateCheck.Never, Storage = "_Dt", DbType = "datetime NOT NULL")]
        public DateTime Dt
        {
            get => _Dt;

            set
            {
                if (_Dt != value)
                {
                    OnDtChanging(value);
                    SendPropertyChanging();
                    _Dt = value;
                    SendPropertyChanged("Dt");
                    OnDtChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_EmailResponses_EmailQueue", Storage = "_EmailQueue", ThisKey = "EmailQueueId", IsForeignKey = true)]
        public EmailQueue EmailQueue
        {
            get => _EmailQueue.Entity;

            set
            {
                EmailQueue previousValue = _EmailQueue.Entity;
                if (((previousValue != value)
                            || (_EmailQueue.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _EmailQueue.Entity = null;
                        previousValue.EmailResponses.Remove(this);
                    }

                    _EmailQueue.Entity = value;
                    if (value != null)
                    {
                        value.EmailResponses.Add(this);

                        _EmailQueueId = value.Id;

                    }

                    else
                    {
                        _EmailQueueId = default(int);

                    }

                    SendPropertyChanged("EmailQueue");
                }
            }
        }

        [Association(Name = "FK_EmailResponses_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.EmailResponses.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.EmailResponses.Add(this);

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
