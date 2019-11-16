using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.TaskListOwners")]
    public partial class TaskListOwner : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _TaskListId;

        private int _PeopleId;

        private EntityRef<Person> _Person;

        private EntityRef<TaskList> _TaskList;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnTaskListIdChanging(int value);
        partial void OnTaskListIdChanged();

        partial void OnPeopleIdChanging(int value);
        partial void OnPeopleIdChanged();

        #endregion

        public TaskListOwner()
        {
            _Person = default(EntityRef<Person>);

            _TaskList = default(EntityRef<TaskList>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "TaskListId", UpdateCheck = UpdateCheck.Never, Storage = "_TaskListId", DbType = "int NOT NULL", IsPrimaryKey = true)]
        [IsForeignKey]
        public int TaskListId
        {
            get => _TaskListId;

            set
            {
                if (_TaskListId != value)
                {
                    if (_TaskList.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnTaskListIdChanging(value);
                    SendPropertyChanging();
                    _TaskListId = value;
                    SendPropertyChanged("TaskListId");
                    OnTaskListIdChanged();
                }
            }
        }

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

        #endregion

        #region Foreign Key Tables

        #endregion

        #region Foreign Keys

        [Association(Name = "FK_TaskListOwners_PEOPLE_TBL", Storage = "_Person", ThisKey = "PeopleId", IsForeignKey = true)]
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
                        previousValue.TaskListOwners.Remove(this);
                    }

                    _Person.Entity = value;
                    if (value != null)
                    {
                        value.TaskListOwners.Add(this);

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

        [Association(Name = "FK_TaskListOwners_TaskList", Storage = "_TaskList", ThisKey = "TaskListId", IsForeignKey = true)]
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
                        previousValue.TaskListOwners.Remove(this);
                    }

                    _TaskList.Entity = value;
                    if (value != null)
                    {
                        value.TaskListOwners.Add(this);

                        _TaskListId = value.Id;

                    }

                    else
                    {
                        _TaskListId = default(int);

                    }

                    SendPropertyChanged("TaskList");
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
