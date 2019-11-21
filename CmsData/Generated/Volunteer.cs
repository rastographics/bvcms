using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Volunteer")]
    public partial class Volunteer : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _PeopleId;

        private int? _StatusId;

        private DateTime? _ProcessedDate;

        private bool _Standard;

        private bool _Children;

        private bool _Leader;

        private string _Comments;

        private int? _MVRStatusId;

        private DateTime? _MVRProcessedDate;

        private EntitySet<VolunteerForm> _VolunteerForms;

        private EntitySet<VoluteerApprovalId> _VoluteerApprovalIds;

        private EntityRef<Person> _Person;

        private EntityRef<VolApplicationStatus> _VolApplicationStatus;

        private EntityRef<VolApplicationStatus> _StatusMvr;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        partial void OnStatusIdChanging(int? value);
        partial void OnStatusIdChanged();

        partial void OnProcessedDateChanging(DateTime? value);
        partial void OnProcessedDateChanged();

        partial void OnStandardChanging(bool value);
        partial void OnStandardChanged();

        partial void OnChildrenChanging(bool value);
        partial void OnChildrenChanged();

        partial void OnLeaderChanging(bool value);
        partial void OnLeaderChanged();

        partial void OnCommentsChanging(string value);
        partial void OnCommentsChanged();

        partial void OnMVRStatusIdChanging(int? value);
        partial void OnMVRStatusIdChanged();

        partial void OnMVRProcessedDateChanging(DateTime? value);
        partial void OnMVRProcessedDateChanged();

        #endregion

        public Volunteer()
        {
            _VolunteerForms = new EntitySet<VolunteerForm>(new Action<VolunteerForm>(attach_VolunteerForms), new Action<VolunteerForm>(detach_VolunteerForms));

            _VoluteerApprovalIds = new EntitySet<VoluteerApprovalId>(new Action<VoluteerApprovalId>(attach_VoluteerApprovalIds), new Action<VoluteerApprovalId>(detach_VoluteerApprovalIds));

            _Person = default(EntityRef<Person>);

            _VolApplicationStatus = default(EntityRef<VolApplicationStatus>);

            _StatusMvr = default(EntityRef<VolApplicationStatus>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", DbType = "int NOT NULL", IsPrimaryKey = true)]
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

        [Column(Name = "StatusId", UpdateCheck = UpdateCheck.Never, Storage = "_StatusId", DbType = "int")]
        [IsForeignKey]
        public int? StatusId
        {
            get => _StatusId;

            set
            {
                if (_StatusId != value)
                {
                    if (_VolApplicationStatus.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnStatusIdChanging(value);
                    SendPropertyChanging();
                    _StatusId = value;
                    SendPropertyChanged("StatusId");
                    OnStatusIdChanged();
                }
            }
        }

        [Column(Name = "ProcessedDate", UpdateCheck = UpdateCheck.Never, Storage = "_ProcessedDate", DbType = "datetime")]
        public DateTime? ProcessedDate
        {
            get => _ProcessedDate;

            set
            {
                if (_ProcessedDate != value)
                {
                    OnProcessedDateChanging(value);
                    SendPropertyChanging();
                    _ProcessedDate = value;
                    SendPropertyChanged("ProcessedDate");
                    OnProcessedDateChanged();
                }
            }
        }

        [Column(Name = "Standard", UpdateCheck = UpdateCheck.Never, Storage = "_Standard", DbType = "bit NOT NULL")]
        public bool Standard
        {
            get => _Standard;

            set
            {
                if (_Standard != value)
                {
                    OnStandardChanging(value);
                    SendPropertyChanging();
                    _Standard = value;
                    SendPropertyChanged("Standard");
                    OnStandardChanged();
                }
            }
        }

        [Column(Name = "Children", UpdateCheck = UpdateCheck.Never, Storage = "_Children", DbType = "bit NOT NULL")]
        public bool Children
        {
            get => _Children;

            set
            {
                if (_Children != value)
                {
                    OnChildrenChanging(value);
                    SendPropertyChanging();
                    _Children = value;
                    SendPropertyChanged("Children");
                    OnChildrenChanged();
                }
            }
        }

        [Column(Name = "Leader", UpdateCheck = UpdateCheck.Never, Storage = "_Leader", DbType = "bit NOT NULL")]
        public bool Leader
        {
            get => _Leader;

            set
            {
                if (_Leader != value)
                {
                    OnLeaderChanging(value);
                    SendPropertyChanging();
                    _Leader = value;
                    SendPropertyChanged("Leader");
                    OnLeaderChanged();
                }
            }
        }

        [Column(Name = "Comments", UpdateCheck = UpdateCheck.Never, Storage = "_Comments", DbType = "nvarchar")]
        public string Comments
        {
            get => _Comments;

            set
            {
                if (_Comments != value)
                {
                    OnCommentsChanging(value);
                    SendPropertyChanging();
                    _Comments = value;
                    SendPropertyChanged("Comments");
                    OnCommentsChanged();
                }
            }
        }

        [Column(Name = "MVRStatusId", UpdateCheck = UpdateCheck.Never, Storage = "_MVRStatusId", DbType = "int")]
        [IsForeignKey]
        public int? MVRStatusId
        {
            get => _MVRStatusId;

            set
            {
                if (_MVRStatusId != value)
                {
                    if (_StatusMvr.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnMVRStatusIdChanging(value);
                    SendPropertyChanging();
                    _MVRStatusId = value;
                    SendPropertyChanged("MVRStatusId");
                    OnMVRStatusIdChanged();
                }
            }
        }

        [Column(Name = "MVRProcessedDate", UpdateCheck = UpdateCheck.Never, Storage = "_MVRProcessedDate", DbType = "datetime")]
        public DateTime? MVRProcessedDate
        {
            get => _MVRProcessedDate;

            set
            {
                if (_MVRProcessedDate != value)
                {
                    OnMVRProcessedDateChanging(value);
                    SendPropertyChanging();
                    _MVRProcessedDate = value;
                    SendPropertyChanged("MVRProcessedDate");
                    OnMVRProcessedDateChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "FK_VolunteerForm_Volunteer1", Storage = "_VolunteerForms", OtherKey = "PeopleId")]
        public EntitySet<VolunteerForm> VolunteerForms
           {
               get => _VolunteerForms;

            set => _VolunteerForms.Assign(value);

           }

        [Association(Name = "FK_VoluteerApprovalIds_Volunteer", Storage = "_VoluteerApprovalIds", OtherKey = "PeopleId")]
        public EntitySet<VoluteerApprovalId> VoluteerApprovalIds
           {
               get => _VoluteerApprovalIds;

            set => _VoluteerApprovalIds.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_Volunteer_PEOPLE_TBL", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.Volunteers.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.Volunteers.Add(this);

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

        [Association(Name = "FK_Volunteer_VolApplicationStatus", Storage = "_VolApplicationStatus", ThisKey = "StatusId", IsForeignKey = true)]
        public VolApplicationStatus VolApplicationStatus
        {
            get => _VolApplicationStatus.Entity;

            set
            {
                VolApplicationStatus previousValue = _VolApplicationStatus.Entity;
                if (((previousValue != value)
                            || (_VolApplicationStatus.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _VolApplicationStatus.Entity = null;
                        previousValue.Volunteers.Remove(this);
                    }

                    _VolApplicationStatus.Entity = value;
                    if (value != null)
                    {
                        value.Volunteers.Add(this);

                        _StatusId = value.Id;

                    }

                    else
                    {
                        _StatusId = default(int?);

                    }

                    SendPropertyChanged("VolApplicationStatus");
                }
            }
        }

        [Association(Name = "StatusMvrId__StatusMvr", Storage = "_StatusMvr", ThisKey = "MVRStatusId", IsForeignKey = true)]
        public VolApplicationStatus StatusMvr
        {
            get => _StatusMvr.Entity;

            set
            {
                VolApplicationStatus previousValue = _StatusMvr.Entity;
                if (((previousValue != value)
                            || (_StatusMvr.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _StatusMvr.Entity = null;
                        previousValue.StatusMvrId.Remove(this);
                    }

                    _StatusMvr.Entity = value;
                    if (value != null)
                    {
                        value.StatusMvrId.Add(this);

                        _MVRStatusId = value.Id;

                    }

                    else
                    {
                        _MVRStatusId = default(int?);

                    }

                    SendPropertyChanged("StatusMvr");
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

        private void attach_VolunteerForms(VolunteerForm entity)
        {
            SendPropertyChanging();
            entity.Volunteer = this;
        }

        private void detach_VolunteerForms(VolunteerForm entity)
        {
            SendPropertyChanging();
            entity.Volunteer = null;
        }

        private void attach_VoluteerApprovalIds(VoluteerApprovalId entity)
        {
            SendPropertyChanging();
            entity.Volunteer = this;
        }

        private void detach_VoluteerApprovalIds(VoluteerApprovalId entity)
        {
            SendPropertyChanging();
            entity.Volunteer = null;
        }
    }
}
