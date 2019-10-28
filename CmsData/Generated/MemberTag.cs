using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.MemberTags")]
    public partial class MemberTag : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private string _Name;

        private int? _OrgId;

        private string _VolFrequency;

        private DateTime? _VolStartDate;

        private DateTime? _VolEndDate;

        private int? _NoCancelWeeks;

        private bool _CheckIn;

        private bool _CheckInOpen;

        private int _CheckInCapacity;

        private bool _CheckInOpenDefault;

        private int _CheckInCapacityDefault;

        private int? _ScheduleId;

        private EntitySet<OrgMemMemTag> _OrgMemMemTags;

        private EntityRef<Organization> _Organization;

        private EntityRef<OrgSchedule> _Schedule;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        partial void OnOrgIdChanging(int? value);
        partial void OnOrgIdChanged();

        partial void OnVolFrequencyChanging(string value);
        partial void OnVolFrequencyChanged();

        partial void OnVolStartDateChanging(DateTime? value);
        partial void OnVolStartDateChanged();

        partial void OnVolEndDateChanging(DateTime? value);
        partial void OnVolEndDateChanged();

        partial void OnNoCancelWeeksChanging(int? value);
        partial void OnNoCancelWeeksChanged();

        partial void OnCheckInChanging(bool value);
        partial void OnCheckInChanged();

        partial void OnCheckInOpenChanging(bool value);
        partial void OnCheckInOpenChanged();

        partial void OnCheckInCapacityChanging(int value);
        partial void OnCheckInCapacityChanged();

        partial void OnCheckInOpenDefaultChanging(bool value);
        partial void OnCheckInOpenDefaultChanged();

        partial void OnCheckInCapacityDefaultChanging(int value);
        partial void OnCheckInCapacityDefaultChanged();

        partial void OnScheduleIdChanging(int? value);
        partial void OnScheduleIdChanged();

        #endregion

        public MemberTag()
        {
            _OrgMemMemTags = new EntitySet<OrgMemMemTag>(new Action<OrgMemMemTag>(attach_OrgMemMemTags), new Action<OrgMemMemTag>(detach_OrgMemMemTags));

            _Organization = default(EntityRef<Organization>);

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

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(200)")]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    OnNameChanging(value);
                    SendPropertyChanging();
                    _Name = value;
                    SendPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }

        [Column(Name = "OrgId", UpdateCheck = UpdateCheck.Never, Storage = "_OrgId", DbType = "int")]
        [IsForeignKey]
        public int? OrgId
        {
            get => _OrgId;

            set
            {
                if (_OrgId != value)
                {
                    if (_Organization.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOrgIdChanging(value);
                    SendPropertyChanging();
                    _OrgId = value;
                    SendPropertyChanged("OrgId");
                    OnOrgIdChanged();
                }
            }
        }

        [Column(Name = "VolFrequency", UpdateCheck = UpdateCheck.Never, Storage = "_VolFrequency", DbType = "nvarchar(2)")]
        public string VolFrequency
        {
            get => _VolFrequency;

            set
            {
                if (_VolFrequency != value)
                {
                    OnVolFrequencyChanging(value);
                    SendPropertyChanging();
                    _VolFrequency = value;
                    SendPropertyChanged("VolFrequency");
                    OnVolFrequencyChanged();
                }
            }
        }

        [Column(Name = "VolStartDate", UpdateCheck = UpdateCheck.Never, Storage = "_VolStartDate", DbType = "datetime")]
        public DateTime? VolStartDate
        {
            get => _VolStartDate;

            set
            {
                if (_VolStartDate != value)
                {
                    OnVolStartDateChanging(value);
                    SendPropertyChanging();
                    _VolStartDate = value;
                    SendPropertyChanged("VolStartDate");
                    OnVolStartDateChanged();
                }
            }
        }

        [Column(Name = "VolEndDate", UpdateCheck = UpdateCheck.Never, Storage = "_VolEndDate", DbType = "datetime")]
        public DateTime? VolEndDate
        {
            get => _VolEndDate;

            set
            {
                if (_VolEndDate != value)
                {
                    OnVolEndDateChanging(value);
                    SendPropertyChanging();
                    _VolEndDate = value;
                    SendPropertyChanged("VolEndDate");
                    OnVolEndDateChanged();
                }
            }
        }

        [Column(Name = "NoCancelWeeks", UpdateCheck = UpdateCheck.Never, Storage = "_NoCancelWeeks", DbType = "int")]
        public int? NoCancelWeeks
        {
            get => _NoCancelWeeks;

            set
            {
                if (_NoCancelWeeks != value)
                {
                    OnNoCancelWeeksChanging(value);
                    SendPropertyChanging();
                    _NoCancelWeeks = value;
                    SendPropertyChanged("NoCancelWeeks");
                    OnNoCancelWeeksChanged();
                }
            }
        }

        [Column(Name = "CheckIn", UpdateCheck = UpdateCheck.Never, Storage = "_CheckIn", DbType = "bit NOT NULL")]
        public bool CheckIn
        {
            get => _CheckIn;

            set
            {
                if (_CheckIn != value)
                {
                    OnCheckInChanging(value);
                    SendPropertyChanging();
                    _CheckIn = value;
                    SendPropertyChanged("CheckIn");
                    OnCheckInChanged();
                }
            }
        }

        [Column(Name = "CheckInOpen", UpdateCheck = UpdateCheck.Never, Storage = "_CheckInOpen", DbType = "bit NOT NULL")]
        public bool CheckInOpen
        {
            get => _CheckInOpen;

            set
            {
                if (_CheckInOpen != value)
                {
                    OnCheckInOpenChanging(value);
                    SendPropertyChanging();
                    _CheckInOpen = value;
                    SendPropertyChanged("CheckInOpen");
                    OnCheckInOpenChanged();
                }
            }
        }

        [Column(Name = "CheckInCapacity", UpdateCheck = UpdateCheck.Never, Storage = "_CheckInCapacity", DbType = "int NOT NULL")]
        public int CheckInCapacity
        {
            get => _CheckInCapacity;

            set
            {
                if (_CheckInCapacity != value)
                {
                    OnCheckInCapacityChanging(value);
                    SendPropertyChanging();
                    _CheckInCapacity = value;
                    SendPropertyChanged("CheckInCapacity");
                    OnCheckInCapacityChanged();
                }
            }
        }

        [Column(Name = "CheckInOpenDefault", UpdateCheck = UpdateCheck.Never, Storage = "_CheckInOpenDefault", DbType = "bit NOT NULL")]
        public bool CheckInOpenDefault
        {
            get => _CheckInOpenDefault;

            set
            {
                if (_CheckInOpenDefault != value)
                {
                    OnCheckInOpenDefaultChanging(value);
                    SendPropertyChanging();
                    _CheckInOpenDefault = value;
                    SendPropertyChanged("CheckInOpenDefault");
                    OnCheckInOpenDefaultChanged();
                }
            }
        }

        [Column(Name = "CheckInCapacityDefault", UpdateCheck = UpdateCheck.Never, Storage = "_CheckInCapacityDefault", DbType = "int NOT NULL")]
        public int CheckInCapacityDefault
        {
            get => _CheckInCapacityDefault;

            set
            {
                if (_CheckInCapacityDefault != value)
                {
                    OnCheckInCapacityDefaultChanging(value);
                    SendPropertyChanging();
                    _CheckInCapacityDefault = value;
                    SendPropertyChanged("CheckInCapacityDefault");
                    OnCheckInCapacityDefaultChanged();
                }
            }
        }

        [Column(Name = "ScheduleId", UpdateCheck = UpdateCheck.Never, Storage = "_ScheduleId", DbType = "int")]
        [IsForeignKey]
        public int? ScheduleId
        {
            get => _ScheduleId;

            set
            {
                if (_ScheduleId != value)
                {
                    if (_Organization.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnScheduleIdChanging(value);
                    SendPropertyChanging();
                    _ScheduleId = value;
                    SendPropertyChanged("ScheduleId");
                    OnScheduleIdChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_OrgMemMemTags_MemberTags", Storage = "_OrgMemMemTags", OtherKey = "MemberTagId")]
        public EntitySet<OrgMemMemTag> OrgMemMemTags
           {
               get => _OrgMemMemTags;

            set => _OrgMemMemTags.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_MemberTags_Organizations", Storage = "_Organization", ThisKey = "OrgId", IsForeignKey = true)]
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
                        previousValue.MemberTags.Remove(this);
                    }

                    _Organization.Entity = value;
                    if (value != null)
                    {
                        value.MemberTags.Add(this);

                        _OrgId = value.OrganizationId;

                    }

                    else
                    {
                        _OrgId = default(int?);

                    }

                    SendPropertyChanged("Organization");
                }
            }
        }

        [Association(Name = "FK_MemberTags_OrgSchedule", Storage = "_Schedule", ThisKey = "OrgId,ScheduleId", IsForeignKey = true)]
        public OrgSchedule Schedule
        {
            get => _Schedule.Entity;

            set
            {
                OrgSchedule previousValue = _Schedule.Entity;
                if (((previousValue != value)
                            || (_Schedule.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();

                    _Schedule.Entity = value;
                    if (value != null)
                    {
                        _ScheduleId = value.ScheduleId;
                    }

                    else
                    {
                        _ScheduleId = default(int?);

                    }

                    SendPropertyChanged("Schedule");
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

        private void attach_OrgMemMemTags(OrgMemMemTag entity)
        {
            SendPropertyChanging();
            entity.MemberTag = this;
        }

        private void detach_OrgMemMemTags(OrgMemMemTag entity)
        {
            SendPropertyChanging();
            entity.MemberTag = null;
        }
    }
}
