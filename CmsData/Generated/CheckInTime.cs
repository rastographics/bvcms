using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.CheckInTimes")]
    public partial class CheckInTime : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int? _PeopleId;

        private DateTime? _CheckInTimeX;

        private int? _GuestOfId;

        private string _Location;

        private int _GuestOfPersonID;

        private int? _AccessTypeID;

        private EntitySet<CheckInActivity> _CheckInActivities;

        private EntitySet<CheckInTime> _Guests;

        private EntityRef<Person> _Person;

        private EntityRef<CheckInTime> _GuestOf;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnPeopleIdChanging(int? value);
        partial void OnPeopleIdChanged();

        partial void OnCheckInTimeXChanging(DateTime? value);
        partial void OnCheckInTimeXChanged();

        partial void OnGuestOfIdChanging(int? value);
        partial void OnGuestOfIdChanged();

        partial void OnLocationChanging(string value);
        partial void OnLocationChanged();

        partial void OnGuestOfPersonIDChanging(int value);
        partial void OnGuestOfPersonIDChanged();

        partial void OnAccessTypeIDChanging(int? value);
        partial void OnAccessTypeIDChanged();

        #endregion

        public CheckInTime()
        {
            _CheckInActivities = new EntitySet<CheckInActivity>(new Action<CheckInActivity>(attach_CheckInActivities), new Action<CheckInActivity>(detach_CheckInActivities));

            _Guests = new EntitySet<CheckInTime>(new Action<CheckInTime>(attach_Guests), new Action<CheckInTime>(detach_Guests));

            _Person = default(EntityRef<Person>);

            _GuestOf = default(EntityRef<CheckInTime>);

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

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int")]
        [IsForeignKey]
        public int? PeopleId
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

        [Column(Name = "CheckInTime", UpdateCheck = UpdateCheck.Never, Storage = "_CheckInTimeX", DbType = "datetime")]
        public DateTime? CheckInTimeX
        {
            get => _CheckInTimeX;

            set
            {
                if (_CheckInTimeX != value)
                {
                    OnCheckInTimeXChanging(value);
                    SendPropertyChanging();
                    _CheckInTimeX = value;
                    SendPropertyChanged("CheckInTimeX");
                    OnCheckInTimeXChanged();
                }
            }
        }

        [Column(Name = "GuestOfId", UpdateCheck = UpdateCheck.Never, Storage = "_GuestOfId", DbType = "int")]
        [IsForeignKey]
        public int? GuestOfId
        {
            get => _GuestOfId;

            set
            {
                if (_GuestOfId != value)
                {
                    if (_GuestOf.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnGuestOfIdChanging(value);
                    SendPropertyChanging();
                    _GuestOfId = value;
                    SendPropertyChanged("GuestOfId");
                    OnGuestOfIdChanged();
                }
            }
        }

        [Column(Name = "location", UpdateCheck = UpdateCheck.Never, Storage = "_Location", DbType = "nvarchar(50)")]
        public string Location
        {
            get => _Location;

            set
            {
                if (_Location != value)
                {
                    OnLocationChanging(value);
                    SendPropertyChanging();
                    _Location = value;
                    SendPropertyChanged("Location");
                    OnLocationChanged();
                }
            }
        }

        [Column(Name = "GuestOfPersonID", UpdateCheck = UpdateCheck.Never, Storage = "_GuestOfPersonID", DbType = "int NOT NULL")]
        public int GuestOfPersonID
        {
            get => _GuestOfPersonID;

            set
            {
                if (_GuestOfPersonID != value)
                {
                    OnGuestOfPersonIDChanging(value);
                    SendPropertyChanging();
                    _GuestOfPersonID = value;
                    SendPropertyChanged("GuestOfPersonID");
                    OnGuestOfPersonIDChanged();
                }
            }
        }

        [Column(Name = "AccessTypeID", UpdateCheck = UpdateCheck.Never, Storage = "_AccessTypeID", DbType = "int")]
        public int? AccessTypeID
        {
            get => _AccessTypeID;

            set
            {
                if (_AccessTypeID != value)
                {
                    OnAccessTypeIDChanging(value);
                    SendPropertyChanging();
                    _AccessTypeID = value;
                    SendPropertyChanged("AccessTypeID");
                    OnAccessTypeIDChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_CheckInActivity_CheckInTimes", Storage = "_CheckInActivities", OtherKey = "Id")]
        public EntitySet<CheckInActivity> CheckInActivities
           {
               get => _CheckInActivities;

            set => _CheckInActivities.Assign(value);

           }

        [Association(Name = "Guests__GuestOf", Storage = "_Guests", OtherKey = "GuestOfId")]
        public EntitySet<CheckInTime> Guests
           {
               get => _Guests;

            set => _Guests.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_CheckInTimes_People", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.CheckInTimes.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.CheckInTimes.Add(this);

                        _PeopleId = value.PeopleId;

                    }

                    else
                    {
                        _PeopleId = default(int?);

                    }

                    SendPropertyChanged("Person");
                }
            }
        }

        [Association(Name = "Guests__GuestOf", Storage = "_GuestOf", ThisKey = "GuestOfId", IsForeignKey = true)]
        public CheckInTime GuestOf
        {
            get => _GuestOf.Entity;

            set
            {
                CheckInTime previousValue = _GuestOf.Entity;
                if (((previousValue != value)
                            || (_GuestOf.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _GuestOf.Entity = null;
                        previousValue.Guests.Remove(this);
                    }

                    _GuestOf.Entity = value;
                    if (value != null)
                    {
                        value.Guests.Add(this);

                        _GuestOfId = value.Id;

                    }

                    else
                    {
                        _GuestOfId = default(int?);

                    }

                    SendPropertyChanged("GuestOf");
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

        private void attach_CheckInActivities(CheckInActivity entity)
        {
            SendPropertyChanging();
            entity.CheckInTime = this;
        }

        private void detach_CheckInActivities(CheckInActivity entity)
        {
            SendPropertyChanging();
            entity.CheckInTime = null;
        }

        private void attach_Guests(CheckInTime entity)
        {
            SendPropertyChanging();
            entity.GuestOf = this;
        }

        private void detach_Guests(CheckInTime entity)
        {
            SendPropertyChanging();
            entity.GuestOf = null;
        }
    }
}
