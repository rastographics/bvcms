using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Attend")]
    public partial class Attend : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _PeopleId;

        private int _MeetingId;

        private int _OrganizationId;

        private DateTime _MeetingDate;

        private bool _AttendanceFlag;

        private int? _OtherOrgId;

        private int? _AttendanceTypeId;

        private int? _CreatedBy;

        private DateTime? _CreatedDate;

        private int _MemberTypeId;

        private int _AttendId;

        private int _OtherAttends;

        private bool? _BFCAttendance;

        private bool? _Registered;

        private int? _SeqNo;

        private int? _Commitment;

        private bool? _NoShow;

        private bool? _EffAttendFlag;

        private int _SubGroupID;

        private string _SubGroupName;

        private string _Pager;

        private EntitySet<SubRequest> _SubRequests;

        private EntityRef<MemberType> _MemberType;

        private EntityRef<AttendType> _AttendType;

        private EntityRef<Meeting> _Meeting;

        private EntityRef<Organization> _Organization;

        private EntityRef<Person> _Person;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnMeetingIdChanging(int value);
        partial void OnMeetingIdChanged();

        partial void OnOrganizationIdChanging(int value);
        partial void OnOrganizationIdChanged();

        partial void OnMeetingDateChanging(DateTime value);
        partial void OnMeetingDateChanged();

        partial void OnAttendanceFlagChanging(bool value);
        partial void OnAttendanceFlagChanged();

        partial void OnOtherOrgIdChanging(int? value);
        partial void OnOtherOrgIdChanged();

        partial void OnAttendanceTypeIdChanging(int? value);
        partial void OnAttendanceTypeIdChanged();

        partial void OnCreatedByChanging(int? value);
        partial void OnCreatedByChanged();

        partial void OnCreatedDateChanging(DateTime? value);
        partial void OnCreatedDateChanged();

        partial void OnMemberTypeIdChanging(int value);
        partial void OnMemberTypeIdChanged();

        partial void OnAttendIdChanging(int value);
        partial void OnAttendIdChanged();

        partial void OnOtherAttendsChanging(int value);
        partial void OnOtherAttendsChanged();

        partial void OnBFCAttendanceChanging(bool? value);
        partial void OnBFCAttendanceChanged();

        partial void OnRegisteredChanging(bool? value);
        partial void OnRegisteredChanged();

        partial void OnSeqNoChanging(int? value);
        partial void OnSeqNoChanged();

        partial void OnCommitmentChanging(int? value);
        partial void OnCommitmentChanged();

        partial void OnNoShowChanging(bool? value);
        partial void OnNoShowChanged();

        partial void OnEffAttendFlagChanging(bool? value);
        partial void OnEffAttendFlagChanged();

        partial void OnSubGroupIDChanging(int value);
        partial void OnSubGroupIDChanged();

        partial void OnSubGroupNameChanging(string value);
        partial void OnSubGroupNameChanged();

        partial void OnPagerChanging(string value);
        partial void OnPagerChanged();

        #endregion

        public Attend()
        {
            _SubRequests = new EntitySet<SubRequest>(new Action<SubRequest>(attach_SubRequests), new Action<SubRequest>(detach_SubRequests));

            _MemberType = default(EntityRef<MemberType>);

            _AttendType = default(EntityRef<AttendType>);

            _Meeting = default(EntityRef<Meeting>);

            _Organization = default(EntityRef<Organization>);

            _Person = default(EntityRef<Person>);

            OnCreated();
        }

        #region Columns

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

        [Column(Name = "MeetingId", UpdateCheck = UpdateCheck.Never, Storage = "_MeetingId", DbType = "int NOT NULL")]
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

        [Column(Name = "MeetingDate", UpdateCheck = UpdateCheck.Never, Storage = "_MeetingDate", DbType = "datetime NOT NULL")]
        public DateTime MeetingDate
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

        [Column(Name = "AttendanceFlag", UpdateCheck = UpdateCheck.Never, Storage = "_AttendanceFlag", DbType = "bit NOT NULL")]
        public bool AttendanceFlag
        {
            get => _AttendanceFlag;

            set
            {
                if (_AttendanceFlag != value)
                {
                    OnAttendanceFlagChanging(value);
                    SendPropertyChanging();
                    _AttendanceFlag = value;
                    SendPropertyChanged("AttendanceFlag");
                    OnAttendanceFlagChanged();
                }
            }
        }

        [Column(Name = "OtherOrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OtherOrgId", DbType = "int")]
        public int? OtherOrgId
        {
            get => _OtherOrgId;

            set
            {
                if (_OtherOrgId != value)
                {
                    OnOtherOrgIdChanging(value);
                    SendPropertyChanging();
                    _OtherOrgId = value;
                    SendPropertyChanged("OtherOrgId");
                    OnOtherOrgIdChanged();
                }
            }
        }

        [Column(Name = "AttendanceTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_AttendanceTypeId", DbType = "int")]
        [IsForeignKey]
        public int? AttendanceTypeId
        {
            get => _AttendanceTypeId;

            set
            {
                if (_AttendanceTypeId != value)
                {
                    if (_AttendType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnAttendanceTypeIdChanging(value);
                    SendPropertyChanging();
                    _AttendanceTypeId = value;
                    SendPropertyChanged("AttendanceTypeId");
                    OnAttendanceTypeIdChanged();
                }
            }
        }

        [Column(Name = "CreatedBy", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int")]
        public int? CreatedBy
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

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime? CreatedDate
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

        [Column(Name = "MemberTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_MemberTypeId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int MemberTypeId
        {
            get => _MemberTypeId;

            set
            {
                if (_MemberTypeId != value)
                {
                    if (_MemberType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnMemberTypeIdChanging(value);
                    SendPropertyChanging();
                    _MemberTypeId = value;
                    SendPropertyChanged("MemberTypeId");
                    OnMemberTypeIdChanged();
                }
            }
        }

        [Column(Name = "AttendId", UpdateCheck = UpdateCheck.Never, Storage = "_AttendId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int AttendId
        {
            get => _AttendId;

            set
            {
                if (_AttendId != value)
                {
                    OnAttendIdChanging(value);
                    SendPropertyChanging();
                    _AttendId = value;
                    SendPropertyChanged("AttendId");
                    OnAttendIdChanged();
                }
            }
        }

        [Column(Name = "OtherAttends", UpdateCheck = UpdateCheck.Never, Storage = "_OtherAttends", DbType = "int NOT NULL")]
        public int OtherAttends
        {
            get => _OtherAttends;

            set
            {
                if (_OtherAttends != value)
                {
                    OnOtherAttendsChanging(value);
                    SendPropertyChanging();
                    _OtherAttends = value;
                    SendPropertyChanged("OtherAttends");
                    OnOtherAttendsChanged();
                }
            }
        }

        [Column(Name = "BFCAttendance", UpdateCheck = UpdateCheck.Never, Storage = "_BFCAttendance", DbType = "bit")]
        public bool? BFCAttendance
        {
            get => _BFCAttendance;

            set
            {
                if (_BFCAttendance != value)
                {
                    OnBFCAttendanceChanging(value);
                    SendPropertyChanging();
                    _BFCAttendance = value;
                    SendPropertyChanged("BFCAttendance");
                    OnBFCAttendanceChanged();
                }
            }
        }

        [Column(Name = "Registered", UpdateCheck = UpdateCheck.Never, Storage = "_Registered", DbType = "bit")]
        public bool? Registered
        {
            get => _Registered;

            set
            {
                if (_Registered != value)
                {
                    OnRegisteredChanging(value);
                    SendPropertyChanging();
                    _Registered = value;
                    SendPropertyChanged("Registered");
                    OnRegisteredChanged();
                }
            }
        }

        [Column(Name = "SeqNo", UpdateCheck = UpdateCheck.Never, Storage = "_SeqNo", DbType = "int")]
        public int? SeqNo
        {
            get => _SeqNo;

            set
            {
                if (_SeqNo != value)
                {
                    OnSeqNoChanging(value);
                    SendPropertyChanging();
                    _SeqNo = value;
                    SendPropertyChanged("SeqNo");
                    OnSeqNoChanged();
                }
            }
        }

        [Column(Name = "Commitment", UpdateCheck = UpdateCheck.Never, Storage = "_Commitment", DbType = "int")]
        public int? Commitment
        {
            get => _Commitment;

            set
            {
                if (_Commitment != value)
                {
                    OnCommitmentChanging(value);
                    SendPropertyChanging();
                    _Commitment = value;
                    SendPropertyChanged("Commitment");
                    OnCommitmentChanged();
                }
            }
        }

        [Column(Name = "NoShow", UpdateCheck = UpdateCheck.Never, Storage = "_NoShow", DbType = "bit")]
        public bool? NoShow
        {
            get => _NoShow;

            set
            {
                if (_NoShow != value)
                {
                    OnNoShowChanging(value);
                    SendPropertyChanging();
                    _NoShow = value;
                    SendPropertyChanged("NoShow");
                    OnNoShowChanged();
                }
            }
        }

        [Column(Name = "EffAttendFlag", UpdateCheck = UpdateCheck.Never, Storage = "_EffAttendFlag", DbType = "bit", IsDbGenerated = true)]
        public bool? EffAttendFlag
        {
            get => _EffAttendFlag;

            set
            {
                if (_EffAttendFlag != value)
                {
                    OnEffAttendFlagChanging(value);
                    SendPropertyChanging();
                    _EffAttendFlag = value;
                    SendPropertyChanged("EffAttendFlag");
                    OnEffAttendFlagChanged();
                }
            }
        }

        [Column(Name = "SubGroupID", UpdateCheck = UpdateCheck.Never, Storage = "_SubGroupID", DbType = "int NOT NULL")]
        public int SubGroupID
        {
            get => _SubGroupID;

            set
            {
                if (_SubGroupID != value)
                {
                    OnSubGroupIDChanging(value);
                    SendPropertyChanging();
                    _SubGroupID = value;
                    SendPropertyChanged("SubGroupID");
                    OnSubGroupIDChanged();
                }
            }
        }

        [Column(Name = "SubGroupName", UpdateCheck = UpdateCheck.Never, Storage = "_SubGroupName", DbType = "nvarchar(200) NOT NULL")]
        public string SubGroupName
        {
            get => _SubGroupName;

            set
            {
                if (_SubGroupName != value)
                {
                    OnSubGroupNameChanging(value);
                    SendPropertyChanging();
                    _SubGroupName = value;
                    SendPropertyChanged("SubGroupName");
                    OnSubGroupNameChanged();
                }
            }
        }

        [Column(Name = "Pager", UpdateCheck = UpdateCheck.Never, Storage = "_Pager", DbType = "nvarchar(20) NOT NULL")]
        public string Pager
        {
            get => _Pager;

            set
            {
                if (_Pager != value)
                {
                    OnPagerChanging(value);
                    SendPropertyChanging();
                    _Pager = value;
                    SendPropertyChanged("Pager");
                    OnPagerChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "SubRequests__Attend", Storage = "_SubRequests", OtherKey = "AttendId")]
        public EntitySet<SubRequest> SubRequests
           {
               get => _SubRequests;

            set => _SubRequests.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Attend_MemberType", Storage = "_MemberType", ThisKey = "MemberTypeId", IsForeignKey = true)]
        public MemberType MemberType
        {
            get => _MemberType.Entity;

            set
            {
                MemberType previousValue = _MemberType.Entity;
                if (((previousValue != value)
                            || (_MemberType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _MemberType.Entity = null;
                        previousValue.Attends.Remove(this);
                    }

                    _MemberType.Entity = value;
                    if (value != null)
                    {
                        value.Attends.Add(this);

                        _MemberTypeId = value.Id;

                    }

                    else
                    {
                        _MemberTypeId = default(int);

                    }

                    SendPropertyChanged("MemberType");
                }
            }
        }

        [Association(Name = "FK_AttendWithAbsents_TBL_AttendType", Storage = "_AttendType", ThisKey = "AttendanceTypeId", IsForeignKey = true)]
        public AttendType AttendType
        {
            get => _AttendType.Entity;

            set
            {
                AttendType previousValue = _AttendType.Entity;
                if (((previousValue != value)
                            || (_AttendType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _AttendType.Entity = null;
                        previousValue.Attends.Remove(this);
                    }

                    _AttendType.Entity = value;
                    if (value != null)
                    {
                        value.Attends.Add(this);

                        _AttendanceTypeId = value.Id;

                    }

                    else
                    {
                        _AttendanceTypeId = default(int?);

                    }

                    SendPropertyChanged("AttendType");
                }
            }
        }

        [Association(Name = "FK_AttendWithAbsents_TBL_MEETINGS_TBL", Storage = "_Meeting", ThisKey = "MeetingId", IsForeignKey = true)]
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
                        previousValue.Attends.Remove(this);
                    }

                    _Meeting.Entity = value;
                    if (value != null)
                    {
                        value.Attends.Add(this);

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

        [Association(Name = "FK_AttendWithAbsents_TBL_ORGANIZATIONS_TBL", Storage = "_Organization", ThisKey = "OrganizationId", IsForeignKey = true)]
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
                        previousValue.Attends.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.Attends.Add(this);

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

        [Association(Name = "FK_AttendWithAbsents_TBL_PEOPLE_TBL", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.Attends.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.Attends.Add(this);

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

        private void attach_SubRequests(SubRequest entity)
        {
            SendPropertyChanging();
            entity.Attend = this;
        }

        private void detach_SubRequests(SubRequest entity)
        {
            SendPropertyChanging();
            entity.Attend = null;
        }
    }
}
