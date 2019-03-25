using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using CmsData.Infrastructure;

namespace CmsData
{
	[Table(Name="dbo.TaskList")]
	public partial class TaskList : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
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
			
			this._CoTasks = new EntitySet<Task>(new Action< Task>(this.attach_CoTasks), new Action< Task>(this.detach_CoTasks)); 
			
			this._TaskListOwners = new EntitySet<TaskListOwner>(new Action< TaskListOwner>(this.attach_TaskListOwners), new Action< TaskListOwner>(this.detach_TaskListOwners)); 
			
			this._Tasks = new EntitySet<Task>(new Action< Task>(this.attach_Tasks), new Action< Task>(this.detach_Tasks)); 
			
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get { return this._Id; }

			set
			{
				if (this._Id != value)
				{
				
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}

			}

		}

		
		[Column(Name="CreatedBy", UpdateCheck=UpdateCheck.Never, Storage="_CreatedBy", DbType="int")]
		public int? CreatedBy
		{
			get { return this._CreatedBy; }

			set
			{
				if (this._CreatedBy != value)
				{
				
                    this.OnCreatedByChanging(value);
					this.SendPropertyChanging();
					this._CreatedBy = value;
					this.SendPropertyChanged("CreatedBy");
					this.OnCreatedByChanged();
				}

			}

		}

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(50)")]
		public string Name
		{
			get { return this._Name; }

			set
			{
				if (this._Name != value)
				{
				
                    this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="CoTasks__CoTaskList", Storage="_CoTasks", OtherKey="CoListId")]
   		public EntitySet<Task> CoTasks
   		{
   		    get { return this._CoTasks; }

			set	{ this._CoTasks.Assign(value); }

   		}

		
   		[Association(Name="FK_TaskListOwners_TaskList", Storage="_TaskListOwners", OtherKey="TaskListId")]
   		public EntitySet<TaskListOwner> TaskListOwners
   		{
   		    get { return this._TaskListOwners; }

			set	{ this._TaskListOwners.Assign(value); }

   		}

		
   		[Association(Name="Tasks__TaskList", Storage="_Tasks", OtherKey="ListId")]
   		public EntitySet<Task> Tasks
   		{
   		    get { return this._Tasks; }

			set	{ this._Tasks.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
	#endregion
	
		public event PropertyChangingEventHandler PropertyChanging;
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
				this.PropertyChanging(this, emptyChangingEventArgs);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}

   		
		private void attach_CoTasks(Task entity)
		{
			this.SendPropertyChanging();
			entity.CoTaskList = this;
		}

		private void detach_CoTasks(Task entity)
		{
			this.SendPropertyChanging();
			entity.CoTaskList = null;
		}

		
		private void attach_TaskListOwners(TaskListOwner entity)
		{
			this.SendPropertyChanging();
			entity.TaskList = this;
		}

		private void detach_TaskListOwners(TaskListOwner entity)
		{
			this.SendPropertyChanging();
			entity.TaskList = null;
		}

		
		private void attach_Tasks(Task entity)
		{
			this.SendPropertyChanging();
			entity.TaskList = this;
		}

		private void detach_Tasks(Task entity)
		{
			this.SendPropertyChanging();
			entity.TaskList = null;
		}

		
	}

}

