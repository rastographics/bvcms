using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Task")]
    public partial class Task : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int _OwnerId;

        private int _ListId;

        private int? _CoOwnerId;

        private int? _CoListId;

        private int? _StatusId;

        private DateTime _CreatedOn;

        private int? _SourceContactId;

        private int? _CompletedContactId;

        private string _Notes;

        private int? _ModifiedBy;

        private DateTime? _ModifiedOn;

        private string _Project;

        private bool _Archive;

        private int? _Priority;

        private int? _WhoId;

        private DateTime? _Due;

        private string _Location;

        private string _Description;

        private DateTime? _CompletedOn;

        private bool? _ForceCompleteWContact;

        private int? _OrginatorId;

        private string _DeclineReason;

        private string _LimitToRole;

        private EntityRef<TaskList> _CoTaskList;

        private EntityRef<TaskStatus> _TaskStatus;

        private EntityRef<Person> _Owner;

        private EntityRef<TaskList> _TaskList;

        private EntityRef<Person> _AboutWho;

        private EntityRef<Contact> _SourceContact;

        private EntityRef<Contact> _CompletedContact;

        private EntityRef<Person> _CoOwner;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnOwnerIdChanging(int value);
        partial void OnOwnerIdChanged();

        partial void OnListIdChanging(int value);
        partial void OnListIdChanged();

        partial void OnCoOwnerIdChanging(int? value);
        partial void OnCoOwnerIdChanged();

        partial void OnCoListIdChanging(int? value);
        partial void OnCoListIdChanged();

        partial void OnStatusIdChanging(int? value);
        partial void OnStatusIdChanged();

        partial void OnCreatedOnChanging(DateTime value);
        partial void OnCreatedOnChanged();

        partial void OnSourceContactIdChanging(int? value);
        partial void OnSourceContactIdChanged();

        partial void OnCompletedContactIdChanging(int? value);
        partial void OnCompletedContactIdChanged();

        partial void OnNotesChanging(string value);
        partial void OnNotesChanged();

        partial void OnModifiedByChanging(int? value);
        partial void OnModifiedByChanged();

        partial void OnModifiedOnChanging(DateTime? value);
        partial void OnModifiedOnChanged();

        partial void OnProjectChanging(string value);
        partial void OnProjectChanged();

        partial void OnArchiveChanging(bool value);
        partial void OnArchiveChanged();

        partial void OnPriorityChanging(int? value);
        partial void OnPriorityChanged();

        partial void OnWhoIdChanging(int? value);
        partial void OnWhoIdChanged();

        partial void OnDueChanging(DateTime? value);
        partial void OnDueChanged();

        partial void OnLocationChanging(string value);
        partial void OnLocationChanged();

        partial void OnDescriptionChanging(string value);
        partial void OnDescriptionChanged();

        partial void OnCompletedOnChanging(DateTime? value);
        partial void OnCompletedOnChanged();

        partial void OnForceCompleteWContactChanging(bool? value);
        partial void OnForceCompleteWContactChanged();

        partial void OnOrginatorIdChanging(int? value);
        partial void OnOrginatorIdChanged();

        partial void OnDeclineReasonChanging(string value);
        partial void OnDeclineReasonChanged();

        partial void OnLimitToRoleChanging(string value);
        partial void OnLimitToRoleChanged();

        #endregion

        public Task()
        {
            _CoTaskList = default(EntityRef<TaskList>);

            _TaskStatus = default(EntityRef<TaskStatus>);

            _Owner = default(EntityRef<Person>);

            _TaskList = default(EntityRef<TaskList>);

            _AboutWho = default(EntityRef<Person>);

            _SourceContact = default(EntityRef<Contact>);

            _CompletedContact = default(EntityRef<Contact>);

            _CoOwner = default(EntityRef<Person>);

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

        [Column(Name = "OwnerId", UpdateCheck = UpdateCheck.Never, Storage = "_OwnerId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int OwnerId
        {
            get => _OwnerId;

            set
            {
                if (_OwnerId != value)
                {
                    if (_Owner.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOwnerIdChanging(value);
                    SendPropertyChanging();
                    _OwnerId = value;
                    SendPropertyChanged("OwnerId");
                    OnOwnerIdChanged();
                }
            }
        }

        [Column(Name = "ListId", UpdateCheck = UpdateCheck.Never, Storage = "_ListId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int ListId
        {
            get => _ListId;

            set
            {
                if (_ListId != value)
                {
                    if (_TaskList.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnListIdChanging(value);
                    SendPropertyChanging();
                    _ListId = value;
                    SendPropertyChanged("ListId");
                    OnListIdChanged();
                }
            }
        }

        [Column(Name = "CoOwnerId", UpdateCheck = UpdateCheck.Never, Storage = "_CoOwnerId", DbType = "int")]
        [IsForeignKey]
        public int? CoOwnerId
        {
            get => _CoOwnerId;

            set
            {
                if (_CoOwnerId != value)
                {
                    if (_CoOwner.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnCoOwnerIdChanging(value);
                    SendPropertyChanging();
                    _CoOwnerId = value;
                    SendPropertyChanged("CoOwnerId");
                    OnCoOwnerIdChanged();
                }
            }
        }

        [Column(Name = "CoListId", UpdateCheck = UpdateCheck.Never, Storage = "_CoListId", DbType = "int")]
        [IsForeignKey]
        public int? CoListId
        {
            get => _CoListId;

            set
            {
                if (_CoListId != value)
                {
                    if (_CoTaskList.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnCoListIdChanging(value);
                    SendPropertyChanging();
                    _CoListId = value;
                    SendPropertyChanged("CoListId");
                    OnCoListIdChanged();
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
                    if (_TaskStatus.HasLoadedOrAssignedValue)
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

        [Column(Name = "CreatedOn", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedOn", DbType = "datetime NOT NULL")]
        public DateTime CreatedOn
        {
            get => _CreatedOn;

            set
            {
                if (_CreatedOn != value)
                {
                    OnCreatedOnChanging(value);
                    SendPropertyChanging();
                    _CreatedOn = value;
                    SendPropertyChanged("CreatedOn");
                    OnCreatedOnChanged();
                }
            }
        }

        [Column(Name = "SourceContactId", UpdateCheck = UpdateCheck.Never, Storage = "_SourceContactId", DbType = "int")]
        [IsForeignKey]
        public int? SourceContactId
        {
            get => _SourceContactId;

            set
            {
                if (_SourceContactId != value)
                {
                    if (_SourceContact.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnSourceContactIdChanging(value);
                    SendPropertyChanging();
                    _SourceContactId = value;
                    SendPropertyChanged("SourceContactId");
                    OnSourceContactIdChanged();
                }
            }
        }

        [Column(Name = "CompletedContactId", UpdateCheck = UpdateCheck.Never, Storage = "_CompletedContactId", DbType = "int")]
        [IsForeignKey]
        public int? CompletedContactId
        {
            get => _CompletedContactId;

            set
            {
                if (_CompletedContactId != value)
                {
                    if (_CompletedContact.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnCompletedContactIdChanging(value);
                    SendPropertyChanging();
                    _CompletedContactId = value;
                    SendPropertyChanged("CompletedContactId");
                    OnCompletedContactIdChanged();
                }
            }
        }

        [Column(Name = "Notes", UpdateCheck = UpdateCheck.Never, Storage = "_Notes", DbType = "nvarchar")]
        public string Notes
        {
            get => _Notes;

            set
            {
                if (_Notes != value)
                {
                    OnNotesChanging(value);
                    SendPropertyChanging();
                    _Notes = value;
                    SendPropertyChanged("Notes");
                    OnNotesChanged();
                }
            }
        }

        [Column(Name = "ModifiedBy", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedBy", DbType = "int")]
        public int? ModifiedBy
        {
            get => _ModifiedBy;

            set
            {
                if (_ModifiedBy != value)
                {
                    OnModifiedByChanging(value);
                    SendPropertyChanging();
                    _ModifiedBy = value;
                    SendPropertyChanged("ModifiedBy");
                    OnModifiedByChanged();
                }
            }
        }

        [Column(Name = "ModifiedOn", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedOn", DbType = "datetime")]
        public DateTime? ModifiedOn
        {
            get => _ModifiedOn;

            set
            {
                if (_ModifiedOn != value)
                {
                    OnModifiedOnChanging(value);
                    SendPropertyChanging();
                    _ModifiedOn = value;
                    SendPropertyChanged("ModifiedOn");
                    OnModifiedOnChanged();
                }
            }
        }

        [Column(Name = "Project", UpdateCheck = UpdateCheck.Never, Storage = "_Project", DbType = "nvarchar(50)")]
        public string Project
        {
            get => _Project;

            set
            {
                if (_Project != value)
                {
                    OnProjectChanging(value);
                    SendPropertyChanging();
                    _Project = value;
                    SendPropertyChanged("Project");
                    OnProjectChanged();
                }
            }
        }

        [Column(Name = "Archive", UpdateCheck = UpdateCheck.Never, Storage = "_Archive", DbType = "bit NOT NULL")]
        public bool Archive
        {
            get => _Archive;

            set
            {
                if (_Archive != value)
                {
                    OnArchiveChanging(value);
                    SendPropertyChanging();
                    _Archive = value;
                    SendPropertyChanged("Archive");
                    OnArchiveChanged();
                }
            }
        }

        [Column(Name = "Priority", UpdateCheck = UpdateCheck.Never, Storage = "_Priority", DbType = "int")]
        public int? Priority
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

        [Column(Name = "WhoId", UpdateCheck = UpdateCheck.Never, Storage = "_WhoId", DbType = "int")]
        [IsForeignKey]
        public int? WhoId
        {
            get => _WhoId;

            set
            {
                if (_WhoId != value)
                {
                    if (_AboutWho.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnWhoIdChanging(value);
                    SendPropertyChanging();
                    _WhoId = value;
                    SendPropertyChanged("WhoId");
                    OnWhoIdChanged();
                }
            }
        }

        [Column(Name = "Due", UpdateCheck = UpdateCheck.Never, Storage = "_Due", DbType = "datetime")]
        public DateTime? Due
        {
            get => _Due;

            set
            {
                if (_Due != value)
                {
                    OnDueChanging(value);
                    SendPropertyChanging();
                    _Due = value;
                    SendPropertyChanged("Due");
                    OnDueChanged();
                }
            }
        }

        [Column(Name = "Location", UpdateCheck = UpdateCheck.Never, Storage = "_Location", DbType = "nvarchar(50)")]
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

        [Column(Name = "CompletedOn", UpdateCheck = UpdateCheck.Never, Storage = "_CompletedOn", DbType = "datetime")]
        public DateTime? CompletedOn
        {
            get => _CompletedOn;

            set
            {
                if (_CompletedOn != value)
                {
                    OnCompletedOnChanging(value);
                    SendPropertyChanging();
                    _CompletedOn = value;
                    SendPropertyChanged("CompletedOn");
                    OnCompletedOnChanged();
                }
            }
        }

        [Column(Name = "ForceCompleteWContact", UpdateCheck = UpdateCheck.Never, Storage = "_ForceCompleteWContact", DbType = "bit")]
        public bool? ForceCompleteWContact
        {
            get => _ForceCompleteWContact;

            set
            {
                if (_ForceCompleteWContact != value)
                {
                    OnForceCompleteWContactChanging(value);
                    SendPropertyChanging();
                    _ForceCompleteWContact = value;
                    SendPropertyChanged("ForceCompleteWContact");
                    OnForceCompleteWContactChanged();
                }
            }
        }

        [Column(Name = "OrginatorId", UpdateCheck = UpdateCheck.Never, Storage = "_OrginatorId", DbType = "int")]
        public int? OrginatorId
        {
            get => _OrginatorId;

            set
            {
                if (_OrginatorId != value)
                {
                    OnOrginatorIdChanging(value);
                    SendPropertyChanging();
                    _OrginatorId = value;
                    SendPropertyChanged("OrginatorId");
                    OnOrginatorIdChanged();
                }
            }
        }

        [Column(Name = "DeclineReason", UpdateCheck = UpdateCheck.Never, Storage = "_DeclineReason", DbType = "nvarchar")]
        public string DeclineReason
        {
            get => _DeclineReason;

            set
            {
                if (_DeclineReason != value)
                {
                    OnDeclineReasonChanging(value);
                    SendPropertyChanging();
                    _DeclineReason = value;
                    SendPropertyChanged("DeclineReason");
                    OnDeclineReasonChanged();
                }
            }
        }

        [Column(Name = "LimitToRole", UpdateCheck = UpdateCheck.Never, Storage = "_LimitToRole", DbType = "nvarchar(50)")]
        public string LimitToRole
        {
            get => _LimitToRole;

            set
            {
                if (_LimitToRole != value)
                {
                    OnLimitToRoleChanging(value);
                    SendPropertyChanging();
                    _LimitToRole = value;
                    SendPropertyChanged("LimitToRole");
                    OnLimitToRoleChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "CoTasks__CoTaskList", Storage = "_CoTaskList", ThisKey = "CoListId", IsForeignKey = true)]
        public TaskList CoTaskList
        {
            get => _CoTaskList.Entity;

            set
            {
                TaskList previousValue = _CoTaskList.Entity;
                if (((previousValue != value)
                            || (_CoTaskList.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _CoTaskList.Entity = null;
                        previousValue.CoTasks.Remove(this);
                    }

                    _CoTaskList.Entity = value;
                    if (value != null)
                    {
                        value.CoTasks.Add(this);

                        _CoListId = value.Id;

                    }

                    else
                    {
                        _CoListId = default(int?);

                    }

                    SendPropertyChanged("CoTaskList");
                }
            }
        }

        [Association(Name = "FK_Task_TaskStatus", Storage = "_TaskStatus", ThisKey = "StatusId", IsForeignKey = true)]
        public TaskStatus TaskStatus
        {
            get => _TaskStatus.Entity;

            set
            {
                TaskStatus previousValue = _TaskStatus.Entity;
                if (((previousValue != value)
                            || (_TaskStatus.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _TaskStatus.Entity = null;
                        previousValue.Tasks.Remove(this);
                    }

                    _TaskStatus.Entity = value;
                    if (value != null)
                    {
                        value.Tasks.Add(this);

                        _StatusId = value.Id;

                    }

                    else
                    {
                        _StatusId = default(int?);

                    }

                    SendPropertyChanged("TaskStatus");
                }
            }
        }

        [Association(Name = "Tasks__Owner", Storage = "_Owner", ThisKey = "OwnerId", IsForeignKey = true)]
        public Person Owner
        {
            get => _Owner.Entity;

            set
            {
                Person previousValue = _Owner.Entity;
                if (((previousValue != value)
                            || (_Owner.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Owner.Entity = null;
                        previousValue.Tasks.Remove(this);
                    }

                    _Owner.Entity = value;
                    if (value != null)
                    {
                        value.Tasks.Add(this);

                        _OwnerId = value.PeopleId;

                    }

                    else
                    {
                        _OwnerId = default(int);

                    }

                    SendPropertyChanged("Owner");
                }
            }
        }

        [Association(Name = "Tasks__TaskList", Storage = "_TaskList", ThisKey = "ListId", IsForeignKey = true)]
        public TaskList TaskList
        {
            get => _TaskList.Entity;

            set
            {
                TaskList previousValue = _TaskList.Entity;
                if (((previousValue != value)
                            || (_TaskList.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _TaskList.Entity = null;
                        previousValue.Tasks.Remove(this);
                    }

                    _TaskList.Entity = value;
                    if (value != null)
                    {
                        value.Tasks.Add(this);

                        _ListId = value.Id;

                    }

                    else
                    {
                        _ListId = default(int);

                    }

                    SendPropertyChanged("TaskList");
                }
            }
        }

        [Association(Name = "TasksAboutPerson__AboutWho", Storage = "_AboutWho", ThisKey = "WhoId", IsForeignKey = true)]
        public Person AboutWho
        {
            get => _AboutWho.Entity;

            set
            {
                Person previousValue = _AboutWho.Entity;
                if (((previousValue != value)
                            || (_AboutWho.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _AboutWho.Entity = null;
                        previousValue.TasksAboutPerson.Remove(this);
                    }

                    _AboutWho.Entity = value;
                    if (value != null)
                    {
                        value.TasksAboutPerson.Add(this);

                        _WhoId = value.PeopleId;

                    }

                    else
                    {
                        _WhoId = default(int?);

                    }

                    SendPropertyChanged("AboutWho");
                }
            }
        }

        [Association(Name = "TasksAssigned__SourceContact", Storage = "_SourceContact", ThisKey = "SourceContactId", IsForeignKey = true)]
        public Contact SourceContact
        {
            get => _SourceContact.Entity;

            set
            {
                Contact previousValue = _SourceContact.Entity;
                if (((previousValue != value)
                            || (_SourceContact.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _SourceContact.Entity = null;
                        previousValue.TasksAssigned.Remove(this);
                    }

                    _SourceContact.Entity = value;
                    if (value != null)
                    {
                        value.TasksAssigned.Add(this);

                        _SourceContactId = value.ContactId;

                    }

                    else
                    {
                        _SourceContactId = default(int?);

                    }

                    SendPropertyChanged("SourceContact");
                }
            }
        }

        [Association(Name = "TasksCompleted__CompletedContact", Storage = "_CompletedContact", ThisKey = "CompletedContactId", IsForeignKey = true)]
        public Contact CompletedContact
        {
            get => _CompletedContact.Entity;

            set
            {
                Contact previousValue = _CompletedContact.Entity;
                if (((previousValue != value)
                            || (_CompletedContact.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _CompletedContact.Entity = null;
                        previousValue.TasksCompleted.Remove(this);
                    }

                    _CompletedContact.Entity = value;
                    if (value != null)
                    {
                        value.TasksCompleted.Add(this);

                        _CompletedContactId = value.ContactId;

                    }

                    else
                    {
                        _CompletedContactId = default(int?);

                    }

                    SendPropertyChanged("CompletedContact");
                }
            }
        }

        [Association(Name = "TasksCoOwned__CoOwner", Storage = "_CoOwner", ThisKey = "CoOwnerId", IsForeignKey = true)]
        public Person CoOwner
        {
            get => _CoOwner.Entity;

            set
            {
                Person previousValue = _CoOwner.Entity;
                if (((previousValue != value)
                            || (_CoOwner.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _CoOwner.Entity = null;
                        previousValue.TasksCoOwned.Remove(this);
                    }

                    _CoOwner.Entity = value;
                    if (value != null)
                    {
                        value.TasksCoOwned.Add(this);

                        _CoOwnerId = value.PeopleId;

                    }

                    else
                    {
                        _CoOwnerId = default(int?);

                    }

                    SendPropertyChanged("CoOwner");
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
