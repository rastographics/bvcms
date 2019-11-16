using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Meetings")]
    public partial class Meeting : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _MeetingId;

        private int _CreatedBy;

        private DateTime _CreatedDate;

        private int _OrganizationId;

        private int _NumPresent;

        private int _NumMembers;

        private int _NumVstMembers;

        private int _NumRepeatVst;

        private int _NumNewVisit;

        private string _Location;

        private DateTime? _MeetingDate;

        private bool _GroupMeetingFlag;

        private string _Description;

        private int? _NumOutTown;

        private int? _NumOtherAttends;

        private int? _AttendCreditId;

        private int? _ScheduleId;

        private bool? _NoAutoAbsents;

        private int? _HeadCount;

        private int? _MaxCount;

        private EntitySet<Attend> _Attends;

        private EntitySet<MeetingExtra> _MeetingExtras;

        private EntitySet<VolRequest> _VolRequests;

        private EntityRef<AttendCredit> _AttendCredit;

        private EntityRef<Organization> _Organization;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnMeetingIdChanging(int value);
        partial void OnMeetingIdChanged();

        partial void OnCreatedByChanging(int value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime value);
        partial void OnCreatedDateChanged();

        partial void OnOrganizationIdChanging(int value);
        partial void OnOrganizationIdChanged();

        partial void OnNumPresentChanging(int value);
        partial void OnNumPresentChanged();

        partial void OnNumMembersChanging(int value);
        partial void OnNumMembersChanged();

        partial void OnNumVstMembersChanging(int value);
        partial void OnNumVstMembersChanged();

        partial void OnNumRepeatVstChanging(int value);
        partial void OnNumRepeatVstChanged();

        partial void OnNumNewVisitChanging(int value);
        partial void OnNumNewVisitChanged();

        partial void OnLocationChanging(string value);
        partial void OnLocationChanged();

        partial void OnMeetingDateChanging(DateTime? value);
        partial void OnMeetingDateChanged();

        partial void OnGroupMeetingFlagChanging(bool value);
        partial void OnGroupMeetingFlagChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnNumOutTownChanging(int? value);
        partial void OnNumOutTownChanged();

        partial void OnNumOtherAttendsChanging(int? value);
        partial void OnNumOtherAttendsChanged();

        partial void OnAttendCreditIdChanging(int? value);
        partial void OnAttendCreditIdChanged();

        partial void OnScheduleIdChanging(int? value);
        partial void OnScheduleIdChanged();

        partial void OnNoAutoAbsentsChanging(bool? value);
        partial void OnNoAutoAbsentsChanged();

        partial void OnHeadCountChanging(int? value);
        partial void OnHeadCountChanged();

        partial void OnMaxCountChanging(int? value);
        partial void OnMaxCountChanged();

        #endregion

        public Meeting()
        {
            _Attends = new EntitySet<Attend>(new Action<Attend>(attach_Attends), new Action<Attend>(detach_Attends));

            _MeetingExtras = new EntitySet<MeetingExtra>(new Action<MeetingExtra>(attach_MeetingExtras), new Action<MeetingExtra>(detach_MeetingExtras));

            _VolRequests = new EntitySet<VolRequest>(new Action<VolRequest>(attach_VolRequests), new Action<VolRequest>(detach_VolRequests));

            _AttendCredit = default(EntityRef<AttendCredit>);

            _Organization = default(EntityRef<Organization>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "MeetingId", UpdateCheck = UpdateCheck.Never, Storage = "_MeetingId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int MeetingId
        {
            get => _MeetingId;

            set
            {
                if (_MeetingId != value)
                {
                    OnMeetingIdChanging(value);
                    SendPropertyChanging();
                    _MeetingId = value;
                    SendPropertyChanged("MeetingId");
                    OnMeetingIdChanged();
                }
            }
        }

        [Column(Name = "CreatedBy", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int NOT NULL")]
        public int CreatedBy
        {
            get => _CreatedBy;

            set
            {
                if (_CreatedBy != value)
                {
                    OnCreatedByChanging(value);
                    SendPropertyChanging();
                    _CreatedBy = value;
                    SendPropertyChanged("CreatedBy");
                    OnCreatedByChanged();
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

        [Column(Name = "OrganizationId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationId", DbType = "int NOT NULL")]
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

        [Column(Name = "NumPresent", UpdateCheck = UpdateCheck.Never, Storage = "_NumPresent", DbType = "int NOT NULL")]
        public int NumPresent
        {
            get => _NumPresent;

            set
            {
                if (_NumPresent != value)
                {
                    OnNumPresentChanging(value);
                    SendPropertyChanging();
                    _NumPresent = value;
                    SendPropertyChanged("NumPresent");
                    OnNumPresentChanged();
                }
            }
        }

        [Column(Name = "NumMembers", UpdateCheck = UpdateCheck.Never, Storage = "_NumMembers", DbType = "int NOT NULL")]
        public int NumMembers
        {
            get => _NumMembers;

            set
            {
                if (_NumMembers != value)
                {
                    OnNumMembersChanging(value);
                    SendPropertyChanging();
                    _NumMembers = value;
                    SendPropertyChanged("NumMembers");
                    OnNumMembersChanged();
                }
            }
        }

        [Column(Name = "NumVstMembers", UpdateCheck = UpdateCheck.Never, Storage = "_NumVstMembers", DbType = "int NOT NULL")]
        public int NumVstMembers
        {
            get => _NumVstMembers;

            set
            {
                if (_NumVstMembers != value)
                {
                    OnNumVstMembersChanging(value);
                    SendPropertyChanging();
                    _NumVstMembers = value;
                    SendPropertyChanged("NumVstMembers");
                    OnNumVstMembersChanged();
                }
            }
        }

        [Column(Name = "NumRepeatVst", UpdateCheck = UpdateCheck.Never, Storage = "_NumRepeatVst", DbType = "int NOT NULL")]
        public int NumRepeatVst
        {
            get => _NumRepeatVst;

            set
            {
                if (_NumRepeatVst != value)
                {
                    OnNumRepeatVstChanging(value);
                    SendPropertyChanging();
                    _NumRepeatVst = value;
                    SendPropertyChanged("NumRepeatVst");
                    OnNumRepeatVstChanged();
                }
            }
        }

        [Column(Name = "NumNewVisit", UpdateCheck = UpdateCheck.Never, Storage = "_NumNewVisit", DbType = "int NOT NULL")]
        public int NumNewVisit
        {
            get => _NumNewVisit;

            set
            {
                if (_NumNewVisit != value)
                {
                    OnNumNewVisitChanging(value);
                    SendPropertyChanging();
                    _NumNewVisit = value;
                    SendPropertyChanged("NumNewVisit");
                    OnNumNewVisitChanged();
                }
            }
        }

        [Column(Name = "Location", UpdateCheck = UpdateCheck.Never, Storage = "_Location", DbType = "nvarchar(200)")]
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

        [Column(Name = "MeetingDate", UpdateCheck = UpdateCheck.Never, Storage = "_MeetingDate", DbType = "datetime")]
        public DateTime? MeetingDate
        {
            get => _MeetingDate;

            set
            {
                if (_MeetingDate != value)
                {
                    OnMeetingDateChanging(value);
                    SendPropertyChanging();
                    _MeetingDate = value;
                    SendPropertyChanged("MeetingDate");
                    OnMeetingDateChanged();
                }
            }
        }

        [Column(Name = "GroupMeetingFlag", UpdateCheck = UpdateCheck.Never, Storage = "_GroupMeetingFlag", DbType = "bit NOT NULL")]
        public bool GroupMeetingFlag
        {
            get => _GroupMeetingFlag;

            set
            {
                if (_GroupMeetingFlag != value)
                {
                    OnGroupMeetingFlagChanging(value);
                    SendPropertyChanging();
                    _GroupMeetingFlag = value;
                    SendPropertyChanged("GroupMeetingFlag");
                    OnGroupMeetingFlagChanged();
                }
            }
        }

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar(100)")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    OnDescriptionChanging(value);
                    SendPropertyChanging();
                    _Description = value;
                    SendPropertyChanged("Description");
                    OnDescriptionChanged();
                }
            }
        }

        [Column(Name = "NumOutTown", UpdateCheck = UpdateCheck.Never, Storage = "_NumOutTown", DbType = "int")]
        public int? NumOutTown
        {
            get => _NumOutTown;

            set
            {
                if (_NumOutTown != value)
                {
                    OnNumOutTownChanging(value);
                    SendPropertyChanging();
                    _NumOutTown = value;
                    SendPropertyChanged("NumOutTown");
                    OnNumOutTownChanged();
                }
            }
        }

        [Column(Name = "NumOtherAttends", UpdateCheck = UpdateCheck.Never, Storage = "_NumOtherAttends", DbType = "int")]
        public int? NumOtherAttends
        {
            get => _NumOtherAttends;

            set
            {
                if (_NumOtherAttends != value)
                {
                    OnNumOtherAttendsChanging(value);
                    SendPropertyChanging();
                    _NumOtherAttends = value;
                    SendPropertyChanged("NumOtherAttends");
                    OnNumOtherAttendsChanged();
                }
            }
        }

        [Column(Name = "AttendCreditId", UpdateCheck = UpdateCheck.Never, Storage = "_AttendCreditId", DbType = "int")]
        [IsForeignKey]
        public int? AttendCreditId
        {
            get => _AttendCreditId;

            set
            {
                if (_AttendCreditId != value)
                {
                    if (_AttendCredit.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnAttendCreditIdChanging(value);
                    SendPropertyChanging();
                    _AttendCreditId = value;
                    SendPropertyChanged("AttendCreditId");
                    OnAttendCreditIdChanged();
                }
            }
        }

        [Column(Name = "ScheduleId", UpdateCheck = UpdateCheck.Never, Storage = "_ScheduleId", DbType = "int", IsDbGenerated = true)]
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

        [Column(Name = "NoAutoAbsents", UpdateCheck = UpdateCheck.Never, Storage = "_NoAutoAbsents", DbType = "bit")]
        public bool? NoAutoAbsents
        {
            get => _NoAutoAbsents;

            set
            {
                if (_NoAutoAbsents != value)
                {
                    OnNoAutoAbsentsChanging(value);
                    SendPropertyChanging();
                    _NoAutoAbsents = value;
                    SendPropertyChanged("NoAutoAbsents");
                    OnNoAutoAbsentsChanged();
                }
            }
        }

        [Column(Name = "HeadCount", UpdateCheck = UpdateCheck.Never, Storage = "_HeadCount", DbType = "int")]
        public int? HeadCount
        {
            get => _HeadCount;

            set
            {
                if (_HeadCount != value)
                {
                    OnHeadCountChanging(value);
                    SendPropertyChanging();
                    _HeadCount = value;
                    SendPropertyChanged("HeadCount");
                    OnHeadCountChanged();
                }
            }
        }

        [Column(Name = "MaxCount", UpdateCheck = UpdateCheck.Never, Storage = "_MaxCount", DbType = "int", IsDbGenerated = true)]
        public int? MaxCount
        {
            get => _MaxCount;

            set
            {
                if (_MaxCount != value)
                {
                    OnMaxCountChanging(value);
                    SendPropertyChanging();
                    _MaxCount = value;
                    SendPropertyChanged("MaxCount");
                    OnMaxCountChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_AttendWithAbsents_TBL_MEETINGS_TBL", Storage = "_Attends", OtherKey = "MeetingId")]
        public EntitySet<Attend> Attends
           {
               get => _Attends;

            set => _Attends.Assign(value);

           }

        [Association(Name = "FK_MeetingExtra_Meetings", Storage = "_MeetingExtras", OtherKey = "MeetingId")]
        public EntitySet<MeetingExtra> MeetingExtras
           {
               get => _MeetingExtras;

            set => _MeetingExtras.Assign(value);

           }

        [Association(Name = "VolRequests__Meeting", Storage = "_VolRequests", OtherKey = "MeetingId")]
        public EntitySet<VolRequest> VolRequests
           {
               get => _VolRequests;

            set => _VolRequests.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Meetings_AttendCredit", Storage = "_AttendCredit", ThisKey = "AttendCreditId", IsForeignKey = true)]
        public AttendCredit AttendCredit
        {
            get => _AttendCredit.Entity;

            set
            {
                AttendCredit previousValue = _AttendCredit.Entity;
                if (((previousValue != value)
                            || (_AttendCredit.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _AttendCredit.Entity = null;
                        previousValue.Meetings.Remove(this);
                    }

                    _AttendCredit.Entity = value;
                    if (value != null)
                    {
                        value.Meetings.Add(this);

                        _AttendCreditId = value.Id;

                    }

                    else
                    {
                        _AttendCreditId = default(int?);

                    }

                    SendPropertyChanged("AttendCredit");
                }
            }
        }

        [Association(Name = "FK_MEETINGS_TBL_ORGANIZATIONS_TBL", Storage = "_Organization", ThisKey = "OrganizationId", IsForeignKey = true)]
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
                        previousValue.Meetings.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.Meetings.Add(this);

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

        private void attach_Attends(Attend entity)
        {
            SendPropertyChanging();
            entity.Meeting = this;
        }

        private void detach_Attends(Attend entity)
        {
            SendPropertyChanging();
            entity.Meeting = null;
        }

        private void attach_MeetingExtras(MeetingExtra entity)
        {
            SendPropertyChanging();
            entity.Meeting = this;
        }

        private void detach_MeetingExtras(MeetingExtra entity)
        {
            SendPropertyChanging();
            entity.Meeting = null;
        }

        private void attach_VolRequests(VolRequest entity)
        {
            SendPropertyChanging();
            entity.Meeting = this;
        }

        private void detach_VolRequests(VolRequest entity)
        {
            SendPropertyChanging();
            entity.Meeting = null;
        }
    }
}
