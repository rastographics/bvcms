using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.TaskList")]
    public partial class TaskList : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        #region Private Fields

        private int _Id;

        private int? _CreatedBy;

        private string _Name;

        private EntitySet<Task> _CoTasks;

        private EntitySet<TaskListOwner> _TaskListOwners;

        private EntitySet<Task> _Tasks;

        #endregion

        #region Extensibility Method Definitions

        partial void OnLoaded();
        partial void OnValidate(System.Data.Linq.ChangeAction action);
        partial void OnCreated();

        partial void OnIdChanging(int value);
        partial void OnIdChanged();

        partial void OnCreatedByChanging(int? value);
        partial void OnCreatedByChanged();

        partial void OnNameChanging(string value);
        partial void OnNameChanged();

        #endregion

        public TaskList()
        {
            _CoTasks = new EntitySet<Task>(new Action<Task>(attach_CoTasks), new Action<Task>(detach_CoTasks));

            _TaskListOwners = new EntitySet<TaskListOwner>(new Action<TaskListOwner>(attach_TaskListOwners), new Action<TaskListOwner>(detach_TaskListOwners));

            _Tasks = new EntitySet<Task>(new Action<Task>(attach_Tasks), new Action<Task>(detach_Tasks));

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

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(50)")]
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

        #endregion

        #region Foreign Key Tables

        [Association(Name = "CoTasks__CoTaskList", Storage = "_CoTasks", OtherKey = "CoListId")]
        public EntitySet<Task> CoTasks
           {
               get => _CoTasks;

            set => _CoTasks.Assign(value);

           }

        [Association(Name = "FK_TaskListOwners_TaskList", Storage = "_TaskListOwners", OtherKey = "TaskListId")]
        public EntitySet<TaskListOwner> TaskListOwners
           {
               get => _TaskListOwners;

            set => _TaskListOwners.Assign(value);

           }

        [Association(Name = "Tasks__TaskList", Storage = "_Tasks", OtherKey = "ListId")]
        public EntitySet<Task> Tasks
           {
               get => _Tasks;

            set => _Tasks.Assign(value);

           }

        #endregion

        #region Foreign Keys

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

        private void attach_CoTasks(Task entity)
        {
            SendPropertyChanging();
            entity.CoTaskList = this;
        }

        private void detach_CoTasks(Task entity)
        {
            SendPropertyChanging();
            entity.CoTaskList = null;
        }

        private void attach_TaskListOwners(TaskListOwner entity)
        {
            SendPropertyChanging();
            entity.TaskList = this;
        }

        private void detach_TaskListOwners(TaskListOwner entity)
        {
            SendPropertyChanging();
            entity.TaskList = null;
        }

        private void attach_Tasks(Task entity)
        {
            SendPropertyChanging();
            entity.TaskList = this;
        }

        private void detach_Tasks(Task entity)
        {
            SendPropertyChanging();
            entity.TaskList = null;
        }
    }
}
