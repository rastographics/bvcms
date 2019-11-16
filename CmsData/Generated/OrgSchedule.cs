using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.OrgSchedule")]
    public partial class OrgSchedule : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _OrganizationId;

        private int _Id;

        private int? _ScheduleId;

        private DateTime? _SchedTime;

        private int? _SchedDay;

        private DateTime? _MeetingTime;

        private int? _AttendCreditId;

        private DateTime? _NextMeetingDate;

        private EntityRef<Organization> _Organization;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnOrganizationIdChanging(int value);
        partial void OnOrganizationIdChanged();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnScheduleIdChanging(int? value);
        partial void OnScheduleIdChanged();

        partial void OnSchedTimeChanging(DateTime? value);
        partial void OnSchedTimeChanged();

        partial void OnSchedDayChanging(int? value);
        partial void OnSchedDayChanged();

        partial void OnMeetingTimeChanging(DateTime? value);
        partial void OnMeetingTimeChanged();

        partial void OnAttendCreditIdChanging(int? value);
        partial void OnAttendCreditIdChanged();

        partial void OnNextMeetingDateChanging(DateTime? value);
        partial void OnNextMeetingDateChanged();

        #endregion

        public OrgSchedule()
        {
            _Organization = default(EntityRef<Organization>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "OrganizationId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    if (_Organization.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOrganizationIdChanging(value);
                    SendPropertyChanging();
                    _OrganizationId = value;
                    SendPropertyChanged("OrganizationId");
                    OnOrganizationIdChanged();
                }
            }
        }

        [Column(Name = "Id", UpdateCheck = UpdateCheck.Never, Storage = "_Id", DbType = "int NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "ScheduleId", UpdateCheck = UpdateCheck.Never, Storage = "_ScheduleId", DbType = "int")]
        public int? ScheduleId
        {
            get => _ScheduleId;

            set
            {
                if (_ScheduleId != value)
                {
                    OnScheduleIdChanging(value);
                    SendPropertyChanging();
                    _ScheduleId = value;
                    SendPropertyChanged("ScheduleId");
                    OnScheduleIdChanged();
                }
            }
        }

        [Column(Name = "SchedTime", UpdateCheck = UpdateCheck.Never, Storage = "_SchedTime", DbType = "datetime")]
        public DateTime? SchedTime
        {
            get => _SchedTime;

            set
            {
                if (_SchedTime != value)
                {
                    OnSchedTimeChanging(value);
                    SendPropertyChanging();
                    _SchedTime = value;
                    SendPropertyChanged("SchedTime");
                    OnSchedTimeChanged();
                }
            }
        }

        [Column(Name = "SchedDay", UpdateCheck = UpdateCheck.Never, Storage = "_SchedDay", DbType = "int")]
        public int? SchedDay
        {
            get => _SchedDay;

            set
            {
                if (_SchedDay != value)
                {
                    OnSchedDayChanging(value);
                    SendPropertyChanging();
                    _SchedDay = value;
                    SendPropertyChanged("SchedDay");
                    OnSchedDayChanged();
                }
            }
        }

        [Column(Name = "MeetingTime", UpdateCheck = UpdateCheck.Never, Storage = "_MeetingTime", DbType = "datetime")]
        public DateTime? MeetingTime
        {
            get => _MeetingTime;

            set
            {
                if (_MeetingTime != value)
                {
                    OnMeetingTimeChanging(value);
                    SendPropertyChanging();
                    _MeetingTime = value;
                    SendPropertyChanged("MeetingTime");
                    OnMeetingTimeChanged();
                }
            }
        }

        [Column(Name = "AttendCreditId", UpdateCheck = UpdateCheck.Never, Storage = "_AttendCreditId", DbType = "int")]
        public int? AttendCreditId
        {
            get => _AttendCreditId;

            set
            {
                if (_AttendCreditId != value)
                {
                    OnAttendCreditIdChanging(value);
                    SendPropertyChanging();
                    _AttendCreditId = value;
                    SendPropertyChanged("AttendCreditId");
                    OnAttendCreditIdChanged();
                }
            }
        }

        [Column(Name = "NextMeetingDate", UpdateCheck = UpdateCheck.Never, Storage = "_NextMeetingDate", DbType = "datetime", IsDbGenerated = true)]
        public DateTime? NextMeetingDate
        {
            get => _NextMeetingDate;

            set
            {
                if (_NextMeetingDate != value)
                {
                    OnNextMeetingDateChanging(value);
                    SendPropertyChanging();
                    _NextMeetingDate = value;
                    SendPropertyChanged("NextMeetingDate");
                    OnNextMeetingDateChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_OrgSchedule_Organizations", Storage = "_Organization", ThisKey = "OrganizationId", IsForeignKey = true)]
        public Organization Organization
        {
            get => _Organization.Entity;

            set
            {
                Organization previousValue = _Organization.Entity;
                if (((previousValue != value)
                            || (_Organization.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Organization.Entity = null;
                        previousValue.OrgSchedules.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.OrgSchedules.Add(this);

                        _OrganizationId = value.OrganizationId;

                    }

                    else
                    {
                        _OrganizationId = default(int);

                    }

                    SendPropertyChanged("Organization");
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
