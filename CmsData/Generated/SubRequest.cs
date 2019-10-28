using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.SubRequest")]
    public partial class SubRequest : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _AttendId;

        private int _RequestorId;

        private DateTime _Requested;

        private int _SubstituteId;

        private DateTime? _Responded;

        private bool? _CanSub;

        private EntityRef<Attend> _Attend;

        private EntityRef<Person> _Requestor;

        private EntityRef<Person> _Substitute;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnAttendIdChanging(int value);
        partial void OnAttendIdChanged();

        partial void OnRequestorIdChanging(int value);
        partial void OnRequestorIdChanged();

        partial void OnRequestedChanging(DateTime value);
        partial void OnRequestedChanged();

        partial void OnSubstituteIdChanging(int value);
        partial void OnSubstituteIdChanged();

        partial void OnRespondedChanging(DateTime? value);
        partial void OnRespondedChanged();

        partial void OnCanSubChanging(bool? value);
        partial void OnCanSubChanged();

        #endregion

        public SubRequest()
        {
            _Attend = default(EntityRef<Attend>);

            _Requestor = default(EntityRef<Person>);

            _Substitute = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "AttendId", UpdateCheck = UpdateCheck.Never, Storage = "_AttendId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int AttendId
        {
            get => _AttendId;

            set
            {
                if (_AttendId != value)
                {
                    if (_Attend.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnAttendIdChanging(value);
                    SendPropertyChanging();
                    _AttendId = value;
                    SendPropertyChanged("AttendId");
                    OnAttendIdChanged();
                }
            }
        }

        [Column(Name = "RequestorId", UpdateCheck = UpdateCheck.Never, Storage = "_RequestorId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int RequestorId
        {
            get => _RequestorId;

            set
            {
                if (_RequestorId != value)
                {
                    if (_Requestor.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnRequestorIdChanging(value);
                    SendPropertyChanging();
                    _RequestorId = value;
                    SendPropertyChanged("RequestorId");
                    OnRequestorIdChanged();
                }
            }
        }

        [Column(Name = "Requested", UpdateCheck = UpdateCheck.Never, Storage = "_Requested", DbType = "datetime NOT NULL", IsPrimaryKey = true)]
        public DateTime Requested
        {
            get => _Requested;

            set
            {
                if (_Requested != value)
                {
                    OnRequestedChanging(value);
                    SendPropertyChanging();
                    _Requested = value;
                    SendPropertyChanged("Requested");
                    OnRequestedChanged();
                }
            }
        }

        [Column(Name = "SubstituteId", UpdateCheck = UpdateCheck.Never, Storage = "_SubstituteId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int SubstituteId
        {
            get => _SubstituteId;

            set
            {
                if (_SubstituteId != value)
                {
                    if (_Substitute.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnSubstituteIdChanging(value);
                    SendPropertyChanging();
                    _SubstituteId = value;
                    SendPropertyChanged("SubstituteId");
                    OnSubstituteIdChanged();
                }
            }
        }

        [Column(Name = "Responded", UpdateCheck = UpdateCheck.Never, Storage = "_Responded", DbType = "datetime")]
        public DateTime? Responded
        {
            get => _Responded;

            set
            {
                if (_Responded != value)
                {
                    OnRespondedChanging(value);
                    SendPropertyChanging();
                    _Responded = value;
                    SendPropertyChanged("Responded");
                    OnRespondedChanged();
                }
            }
        }

        [Column(Name = "CanSub", UpdateCheck = UpdateCheck.Never, Storage = "_CanSub", DbType = "bit")]
        public bool? CanSub
        {
            get => _CanSub;

            set
            {
                if (_CanSub != value)
                {
                    OnCanSubChanging(value);
                    SendPropertyChanging();
                    _CanSub = value;
                    SendPropertyChanged("CanSub");
                    OnCanSubChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "SubRequests__Attend", Storage = "_Attend", ThisKey = "AttendId", IsForeignKey = true)]
        public Attend Attend
        {
            get => _Attend.Entity;

            set
            {
                Attend previousValue = _Attend.Entity;
                if (((previousValue != value)
                            || (_Attend.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Attend.Entity = null;
                        previousValue.SubRequests.Remove(this);
                    }

                    _Attend.Entity = value;
                    if (value != null)
                    {
                        value.SubRequests.Add(this);

                        _AttendId = value.AttendId;

                    }

                    else
                    {
                        _AttendId = default(int);

                    }

                    SendPropertyChanged("Attend");
                }
            }
        }

        [Association(Name = "SubRequests__Requestor", Storage = "_Requestor", ThisKey = "RequestorId", IsForeignKey = true)]
        public Person Requestor
        {
            get => _Requestor.Entity;

            set
            {
                Person previousValue = _Requestor.Entity;
                if (((previousValue != value)
                            || (_Requestor.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Requestor.Entity = null;
                        previousValue.SubRequests.Remove(this);
                    }

                    _Requestor.Entity = value;
                    if (value != null)
                    {
                        value.SubRequests.Add(this);

                        _RequestorId = value.PeopleId;

                    }

                    else
                    {
                        _RequestorId = default(int);

                    }

                    SendPropertyChanged("Requestor");
                }
            }
        }

        [Association(Name = "SubResponses__Substitute", Storage = "_Substitute", ThisKey = "SubstituteId", IsForeignKey = true)]
        public Person Substitute
        {
            get => _Substitute.Entity;

            set
            {
                Person previousValue = _Substitute.Entity;
                if (((previousValue != value)
                            || (_Substitute.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Substitute.Entity = null;
                        previousValue.SubResponses.Remove(this);
                    }

                    _Substitute.Entity = value;
                    if (value != null)
                    {
                        value.SubResponses.Add(this);

                        _SubstituteId = value.PeopleId;

                    }

                    else
                    {
                        _SubstituteId = default(int);

                    }

                    SendPropertyChanged("Substitute");
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
