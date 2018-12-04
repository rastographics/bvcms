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
	[Table(Name="dbo.TaskListOwners")]
	public partial class TaskListOwner : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
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
			
			
			this._Person = default(EntityRef<Person>); 
			
			this._TaskList = default(EntityRef<TaskList>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="TaskListId", UpdateCheck=UpdateCheck.Never, Storage="_TaskListId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int TaskListId
		{
			get { return this._TaskListId; }

			set
			{
				if (this._TaskListId != value)
				{
				
					if (this._TaskList.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnTaskListIdChanging(value);
					this.SendPropertyChanging();
					this._TaskListId = value;
					this.SendPropertyChanged("TaskListId");
					this.OnTaskListIdChanged();
				}

			}

		}

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_TaskListOwners_PEOPLE_TBL", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
		public Person Person
		{
			get { return this._Person.Entity; }

			set
			{
				Person previousValue = this._Person.Entity;
				if (((previousValue != value) 
							|| (this._Person.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Person.Entity = null;
						previousValue.TaskListOwners.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.TaskListOwners.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int);
						
					}

					this.SendPropertyChanged("Person");
				}

			}

		}

		
		[Association(Name="FK_TaskListOwners_TaskList", Storage="_TaskList", ThisKey="TaskListId", IsForeignKey=true)]
		public TaskList TaskList
		{
			get { return this._TaskList.Entity; }

			set
			{
				TaskList previousValue = this._TaskList.Entity;
				if (((previousValue != value) 
							|| (this._TaskList.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._TaskList.Entity = null;
						previousValue.TaskListOwners.Remove(this);
					}

					this._TaskList.Entity = value;
					if (value != null)
					{
						value.TaskListOwners.Add(this);
						
						this._TaskListId = value.Id;
						
					}

					else
					{
						
						this._TaskListId = default(int);
						
					}

					this.SendPropertyChanged("TaskList");
				}

			}

		}

		
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

   		
	}

}

