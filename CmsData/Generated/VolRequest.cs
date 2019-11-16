using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.VolRequest")]
    public partial class VolRequest : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _MeetingId;

        private int _RequestorId;

        private DateTime _Requested;

        private int _VolunteerId;

        private DateTime? _Responded;

        private bool? _CanVol;

        private EntityRef<Meeting> _Meeting;

        private EntityRef<Person> _Requestor;

        private EntityRef<Person> _Volunteer;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnMeetingIdChanging(int value);
        partial void OnMeetingIdChanged();

        partial void OnRequestorIdChanging(int value);
        partial void OnRequestorIdChanged();

        partial void OnRequestedChanging(DateTime value);
        partial void OnRequestedChanged();

        partial void OnVolunteerIdChanging(int value);
        partial void OnVolunteerIdChanged();

        partial void OnRespondedChanging(DateTime? value);
        partial void OnRespondedChanged();

        partial void OnCanVolChanging(bool? value);
        partial void OnCanVolChanged();

        #endregion

        public VolRequest()
        {
            _Meeting = default(EntityRef<Meeting>);

            _Requestor = default(EntityRef<Person>);

            _Volunteer = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "MeetingId", UpdateCheck = UpdateCheck.Never, Storage = "_MeetingId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int MeetingId
        {
            get => _MeetingId;

            set
            {
                if (_MeetingId != value)
                {
                    if (_Meeting.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnMeetingIdChanging(value);
                    SendPropertyChanging();
                    _MeetingId = value;
                    SendPropertyChanged("MeetingId");
                    OnMeetingIdChanged();
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

        [Column(Name = "VolunteerId", UpdateCheck = UpdateCheck.Never, Storage = "_VolunteerId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int VolunteerId
        {
            get => _VolunteerId;

            set
            {
                if (_VolunteerId != value)
                {
                    if (_Volunteer.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnVolunteerIdChanging(value);
                    SendPropertyChanging();
                    _VolunteerId = value;
                    SendPropertyChanged("VolunteerId");
                    OnVolunteerIdChanged();
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

        [Column(Name = "CanVol", UpdateCheck = UpdateCheck.Never, Storage = "_CanVol", DbType = "bit")]
        public bool? CanVol
        {
            get => _CanVol;

            set
            {
                if (_CanVol != value)
                {
                    OnCanVolChanging(value);
                    SendPropertyChanging();
                    _CanVol = value;
                    SendPropertyChanged("CanVol");
                    OnCanVolChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "VolRequests__Meeting", Storage = "_Meeting", ThisKey = "MeetingId", IsForeignKey = true)]
        public Meeting Meeting
        {
            get => _Meeting.Entity;

            set
            {
                Meeting previousValue = _Meeting.Entity;
                if (((previousValue != value)
                            || (_Meeting.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Meeting.Entity = null;
                        previousValue.VolRequests.Remove(this);
                    }

                    _Meeting.Entity = value;
                    if (value != null)
                    {
                        value.VolRequests.Add(this);

                        _MeetingId = value.MeetingId;

                    }

                    else
                    {
                        _MeetingId = default(int);

                    }

                    SendPropertyChanged("Meeting");
                }
            }
        }

        [Association(Name = "VolRequests__Requestor", Storage = "_Requestor", ThisKey = "RequestorId", IsForeignKey = true)]
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
                        previousValue.VolRequests.Remove(this);
                    }

                    _Requestor.Entity = value;
                    if (value != null)
                    {
                        value.VolRequests.Add(this);

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

        [Association(Name = "VolResponses__Volunteer", Storage = "_Volunteer", ThisKey = "VolunteerId", IsForeignKey = true)]
        public Person Volunteer
        {
            get => _Volunteer.Entity;

            set
            {
                Person previousValue = _Volunteer.Entity;
                if (((previousValue != value)
                            || (_Volunteer.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Volunteer.Entity = null;
                        previousValue.VolResponses.Remove(this);
                    }

                    _Volunteer.Entity = value;
                    if (value != null)
                    {
                        value.VolResponses.Add(this);

                        _VolunteerId = value.PeopleId;

                    }

                    else
                    {
                        _VolunteerId = default(int);

                    }

                    SendPropertyChanged("Volunteer");
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
