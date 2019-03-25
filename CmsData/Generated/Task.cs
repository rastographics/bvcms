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
	[Table(Name="dbo.Task")]
	public partial class Task : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
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
			
			
			this._CoTaskList = default(EntityRef<TaskList>); 
			
			this._TaskStatus = default(EntityRef<TaskStatus>); 
			
			this._Owner = default(EntityRef<Person>); 
			
			this._TaskList = default(EntityRef<TaskList>); 
			
			this._AboutWho = default(EntityRef<Person>); 
			
			this._SourceContact = default(EntityRef<Contact>); 
			
			this._CompletedContact = default(EntityRef<Contact>); 
			
			this._CoOwner = default(EntityRef<Person>); 
			
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

		
		[Column(Name="OwnerId", UpdateCheck=UpdateCheck.Never, Storage="_OwnerId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int OwnerId
		{
			get { return this._OwnerId; }

			set
			{
				if (this._OwnerId != value)
				{
				
					if (this._Owner.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOwnerIdChanging(value);
					this.SendPropertyChanging();
					this._OwnerId = value;
					this.SendPropertyChanged("OwnerId");
					this.OnOwnerIdChanged();
				}

			}

		}

		
		[Column(Name="ListId", UpdateCheck=UpdateCheck.Never, Storage="_ListId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int ListId
		{
			get { return this._ListId; }

			set
			{
				if (this._ListId != value)
				{
				
					if (this._TaskList.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnListIdChanging(value);
					this.SendPropertyChanging();
					this._ListId = value;
					this.SendPropertyChanged("ListId");
					this.OnListIdChanged();
				}

			}

		}

		
		[Column(Name="CoOwnerId", UpdateCheck=UpdateCheck.Never, Storage="_CoOwnerId", DbType="int")]
		[IsForeignKey]
		public int? CoOwnerId
		{
			get { return this._CoOwnerId; }

			set
			{
				if (this._CoOwnerId != value)
				{
				
					if (this._CoOwner.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnCoOwnerIdChanging(value);
					this.SendPropertyChanging();
					this._CoOwnerId = value;
					this.SendPropertyChanged("CoOwnerId");
					this.OnCoOwnerIdChanged();
				}

			}

		}

		
		[Column(Name="CoListId", UpdateCheck=UpdateCheck.Never, Storage="_CoListId", DbType="int")]
		[IsForeignKey]
		public int? CoListId
		{
			get { return this._CoListId; }

			set
			{
				if (this._CoListId != value)
				{
				
					if (this._CoTaskList.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnCoListIdChanging(value);
					this.SendPropertyChanging();
					this._CoListId = value;
					this.SendPropertyChanged("CoListId");
					this.OnCoListIdChanged();
				}

			}

		}

		
		[Column(Name="StatusId", UpdateCheck=UpdateCheck.Never, Storage="_StatusId", DbType="int")]
		[IsForeignKey]
		public int? StatusId
		{
			get { return this._StatusId; }

			set
			{
				if (this._StatusId != value)
				{
				
					if (this._TaskStatus.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnStatusIdChanging(value);
					this.SendPropertyChanging();
					this._StatusId = value;
					this.SendPropertyChanged("StatusId");
					this.OnStatusIdChanged();
				}

			}

		}

		
		[Column(Name="CreatedOn", UpdateCheck=UpdateCheck.Never, Storage="_CreatedOn", DbType="datetime NOT NULL")]
		public DateTime CreatedOn
		{
			get { return this._CreatedOn; }

			set
			{
				if (this._CreatedOn != value)
				{
				
                    this.OnCreatedOnChanging(value);
					this.SendPropertyChanging();
					this._CreatedOn = value;
					this.SendPropertyChanged("CreatedOn");
					this.OnCreatedOnChanged();
				}

			}

		}

		
		[Column(Name="SourceContactId", UpdateCheck=UpdateCheck.Never, Storage="_SourceContactId", DbType="int")]
		[IsForeignKey]
		public int? SourceContactId
		{
			get { return this._SourceContactId; }

			set
			{
				if (this._SourceContactId != value)
				{
				
					if (this._SourceContact.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnSourceContactIdChanging(value);
					this.SendPropertyChanging();
					this._SourceContactId = value;
					this.SendPropertyChanged("SourceContactId");
					this.OnSourceContactIdChanged();
				}

			}

		}

		
		[Column(Name="CompletedContactId", UpdateCheck=UpdateCheck.Never, Storage="_CompletedContactId", DbType="int")]
		[IsForeignKey]
		public int? CompletedContactId
		{
			get { return this._CompletedContactId; }

			set
			{
				if (this._CompletedContactId != value)
				{
				
					if (this._CompletedContact.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnCompletedContactIdChanging(value);
					this.SendPropertyChanging();
					this._CompletedContactId = value;
					this.SendPropertyChanged("CompletedContactId");
					this.OnCompletedContactIdChanged();
				}

			}

		}

		
		[Column(Name="Notes", UpdateCheck=UpdateCheck.Never, Storage="_Notes", DbType="nvarchar")]
		public string Notes
		{
			get { return this._Notes; }

			set
			{
				if (this._Notes != value)
				{
				
                    this.OnNotesChanging(value);
					this.SendPropertyChanging();
					this._Notes = value;
					this.SendPropertyChanged("Notes");
					this.OnNotesChanged();
				}

			}

		}

		
		[Column(Name="ModifiedBy", UpdateCheck=UpdateCheck.Never, Storage="_ModifiedBy", DbType="int")]
		public int? ModifiedBy
		{
			get { return this._ModifiedBy; }

			set
			{
				if (this._ModifiedBy != value)
				{
				
                    this.OnModifiedByChanging(value);
					this.SendPropertyChanging();
					this._ModifiedBy = value;
					this.SendPropertyChanged("ModifiedBy");
					this.OnModifiedByChanged();
				}

			}

		}

		
		[Column(Name="ModifiedOn", UpdateCheck=UpdateCheck.Never, Storage="_ModifiedOn", DbType="datetime")]
		public DateTime? ModifiedOn
		{
			get { return this._ModifiedOn; }

			set
			{
				if (this._ModifiedOn != value)
				{
				
                    this.OnModifiedOnChanging(value);
					this.SendPropertyChanging();
					this._ModifiedOn = value;
					this.SendPropertyChanged("ModifiedOn");
					this.OnModifiedOnChanged();
				}

			}

		}

		
		[Column(Name="Project", UpdateCheck=UpdateCheck.Never, Storage="_Project", DbType="nvarchar(50)")]
		public string Project
		{
			get { return this._Project; }

			set
			{
				if (this._Project != value)
				{
				
                    this.OnProjectChanging(value);
					this.SendPropertyChanging();
					this._Project = value;
					this.SendPropertyChanged("Project");
					this.OnProjectChanged();
				}

			}

		}

		
		[Column(Name="Archive", UpdateCheck=UpdateCheck.Never, Storage="_Archive", DbType="bit NOT NULL")]
		public bool Archive
		{
			get { return this._Archive; }

			set
			{
				if (this._Archive != value)
				{
				
                    this.OnArchiveChanging(value);
					this.SendPropertyChanging();
					this._Archive = value;
					this.SendPropertyChanged("Archive");
					this.OnArchiveChanged();
				}

			}

		}

		
		[Column(Name="Priority", UpdateCheck=UpdateCheck.Never, Storage="_Priority", DbType="int")]
		public int? Priority
		{
			get { return this._Priority; }

			set
			{
				if (this._Priority != value)
				{
				
                    this.OnPriorityChanging(value);
					this.SendPropertyChanging();
					this._Priority = value;
					this.SendPropertyChanged("Priority");
					this.OnPriorityChanged();
				}

			}

		}

		
		[Column(Name="WhoId", UpdateCheck=UpdateCheck.Never, Storage="_WhoId", DbType="int")]
		[IsForeignKey]
		public int? WhoId
		{
			get { return this._WhoId; }

			set
			{
				if (this._WhoId != value)
				{
				
					if (this._AboutWho.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnWhoIdChanging(value);
					this.SendPropertyChanging();
					this._WhoId = value;
					this.SendPropertyChanged("WhoId");
					this.OnWhoIdChanged();
				}

			}

		}

		
		[Column(Name="Due", UpdateCheck=UpdateCheck.Never, Storage="_Due", DbType="datetime")]
		public DateTime? Due
		{
			get { return this._Due; }

			set
			{
				if (this._Due != value)
				{
				
                    this.OnDueChanging(value);
					this.SendPropertyChanging();
					this._Due = value;
					this.SendPropertyChanged("Due");
					this.OnDueChanged();
				}

			}

		}

		
		[Column(Name="Location", UpdateCheck=UpdateCheck.Never, Storage="_Location", DbType="nvarchar(50)")]
		public string Location
		{
			get { return this._Location; }

			set
			{
				if (this._Location != value)
				{
				
                    this.OnLocationChanging(value);
					this.SendPropertyChanging();
					this._Location = value;
					this.SendPropertyChanged("Location");
					this.OnLocationChanged();
				}

			}

		}

		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar(100)")]
		public string Description
		{
			get { return this._Description; }

			set
			{
				if (this._Description != value)
				{
				
                    this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}

			}

		}

		
		[Column(Name="CompletedOn", UpdateCheck=UpdateCheck.Never, Storage="_CompletedOn", DbType="datetime")]
		public DateTime? CompletedOn
		{
			get { return this._CompletedOn; }

			set
			{
				if (this._CompletedOn != value)
				{
				
                    this.OnCompletedOnChanging(value);
					this.SendPropertyChanging();
					this._CompletedOn = value;
					this.SendPropertyChanged("CompletedOn");
					this.OnCompletedOnChanged();
				}

			}

		}

		
		[Column(Name="ForceCompleteWContact", UpdateCheck=UpdateCheck.Never, Storage="_ForceCompleteWContact", DbType="bit")]
		public bool? ForceCompleteWContact
		{
			get { return this._ForceCompleteWContact; }

			set
			{
				if (this._ForceCompleteWContact != value)
				{
				
                    this.OnForceCompleteWContactChanging(value);
					this.SendPropertyChanging();
					this._ForceCompleteWContact = value;
					this.SendPropertyChanged("ForceCompleteWContact");
					this.OnForceCompleteWContactChanged();
				}

			}

		}

		
		[Column(Name="OrginatorId", UpdateCheck=UpdateCheck.Never, Storage="_OrginatorId", DbType="int")]
		public int? OrginatorId
		{
			get { return this._OrginatorId; }

			set
			{
				if (this._OrginatorId != value)
				{
				
                    this.OnOrginatorIdChanging(value);
					this.SendPropertyChanging();
					this._OrginatorId = value;
					this.SendPropertyChanged("OrginatorId");
					this.OnOrginatorIdChanged();
				}

			}

		}

		
		[Column(Name="DeclineReason", UpdateCheck=UpdateCheck.Never, Storage="_DeclineReason", DbType="nvarchar")]
		public string DeclineReason
		{
			get { return this._DeclineReason; }

			set
			{
				if (this._DeclineReason != value)
				{
				
                    this.OnDeclineReasonChanging(value);
					this.SendPropertyChanging();
					this._DeclineReason = value;
					this.SendPropertyChanged("DeclineReason");
					this.OnDeclineReasonChanged();
				}

			}

		}

		
		[Column(Name="LimitToRole", UpdateCheck=UpdateCheck.Never, Storage="_LimitToRole", DbType="nvarchar(50)")]
		public string LimitToRole
		{
			get { return this._LimitToRole; }

			set
			{
				if (this._LimitToRole != value)
				{
				
                    this.OnLimitToRoleChanging(value);
					this.SendPropertyChanging();
					this._LimitToRole = value;
					this.SendPropertyChanged("LimitToRole");
					this.OnLimitToRoleChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="CoTasks__CoTaskList", Storage="_CoTaskList", ThisKey="CoListId", IsForeignKey=true)]
		public TaskList CoTaskList
		{
			get { return this._CoTaskList.Entity; }

			set
			{
				TaskList previousValue = this._CoTaskList.Entity;
				if (((previousValue != value) 
							|| (this._CoTaskList.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._CoTaskList.Entity = null;
						previousValue.CoTasks.Remove(this);
					}

					this._CoTaskList.Entity = value;
					if (value != null)
					{
						value.CoTasks.Add(this);
						
						this._CoListId = value.Id;
						
					}

					else
					{
						
						this._CoListId = default(int?);
						
					}

					this.SendPropertyChanged("CoTaskList");
				}

			}

		}

		
		[Association(Name="FK_Task_TaskStatus", Storage="_TaskStatus", ThisKey="StatusId", IsForeignKey=true)]
		public TaskStatus TaskStatus
		{
			get { return this._TaskStatus.Entity; }

			set
			{
				TaskStatus previousValue = this._TaskStatus.Entity;
				if (((previousValue != value) 
							|| (this._TaskStatus.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._TaskStatus.Entity = null;
						previousValue.Tasks.Remove(this);
					}

					this._TaskStatus.Entity = value;
					if (value != null)
					{
						value.Tasks.Add(this);
						
						this._StatusId = value.Id;
						
					}

					else
					{
						
						this._StatusId = default(int?);
						
					}

					this.SendPropertyChanged("TaskStatus");
				}

			}

		}

		
		[Association(Name="Tasks__Owner", Storage="_Owner", ThisKey="OwnerId", IsForeignKey=true)]
		public Person Owner
		{
			get { return this._Owner.Entity; }

			set
			{
				Person previousValue = this._Owner.Entity;
				if (((previousValue != value) 
							|| (this._Owner.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Owner.Entity = null;
						previousValue.Tasks.Remove(this);
					}

					this._Owner.Entity = value;
					if (value != null)
					{
						value.Tasks.Add(this);
						
						this._OwnerId = value.PeopleId;
						
					}

					else
					{
						
						this._OwnerId = default(int);
						
					}

					this.SendPropertyChanged("Owner");
				}

			}

		}

		
		[Association(Name="Tasks__TaskList", Storage="_TaskList", ThisKey="ListId", IsForeignKey=true)]
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
						previousValue.Tasks.Remove(this);
					}

					this._TaskList.Entity = value;
					if (value != null)
					{
						value.Tasks.Add(this);
						
						this._ListId = value.Id;
						
					}

					else
					{
						
						this._ListId = default(int);
						
					}

					this.SendPropertyChanged("TaskList");
				}

			}

		}

		
		[Association(Name="TasksAboutPerson__AboutWho", Storage="_AboutWho", ThisKey="WhoId", IsForeignKey=true)]
		public Person AboutWho
		{
			get { return this._AboutWho.Entity; }

			set
			{
				Person previousValue = this._AboutWho.Entity;
				if (((previousValue != value) 
							|| (this._AboutWho.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._AboutWho.Entity = null;
						previousValue.TasksAboutPerson.Remove(this);
					}

					this._AboutWho.Entity = value;
					if (value != null)
					{
						value.TasksAboutPerson.Add(this);
						
						this._WhoId = value.PeopleId;
						
					}

					else
					{
						
						this._WhoId = default(int?);
						
					}

					this.SendPropertyChanged("AboutWho");
				}

			}

		}

		
		[Association(Name="TasksAssigned__SourceContact", Storage="_SourceContact", ThisKey="SourceContactId", IsForeignKey=true)]
		public Contact SourceContact
		{
			get { return this._SourceContact.Entity; }

			set
			{
				Contact previousValue = this._SourceContact.Entity;
				if (((previousValue != value) 
							|| (this._SourceContact.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._SourceContact.Entity = null;
						previousValue.TasksAssigned.Remove(this);
					}

					this._SourceContact.Entity = value;
					if (value != null)
					{
						value.TasksAssigned.Add(this);
						
						this._SourceContactId = value.ContactId;
						
					}

					else
					{
						
						this._SourceContactId = default(int?);
						
					}

					this.SendPropertyChanged("SourceContact");
				}

			}

		}

		
		[Association(Name="TasksCompleted__CompletedContact", Storage="_CompletedContact", ThisKey="CompletedContactId", IsForeignKey=true)]
		public Contact CompletedContact
		{
			get { return this._CompletedContact.Entity; }

			set
			{
				Contact previousValue = this._CompletedContact.Entity;
				if (((previousValue != value) 
							|| (this._CompletedContact.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._CompletedContact.Entity = null;
						previousValue.TasksCompleted.Remove(this);
					}

					this._CompletedContact.Entity = value;
					if (value != null)
					{
						value.TasksCompleted.Add(this);
						
						this._CompletedContactId = value.ContactId;
						
					}

					else
					{
						
						this._CompletedContactId = default(int?);
						
					}

					this.SendPropertyChanged("CompletedContact");
				}

			}

		}

		
		[Association(Name="TasksCoOwned__CoOwner", Storage="_CoOwner", ThisKey="CoOwnerId", IsForeignKey=true)]
		public Person CoOwner
		{
			get { return this._CoOwner.Entity; }

			set
			{
				Person previousValue = this._CoOwner.Entity;
				if (((previousValue != value) 
							|| (this._CoOwner.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._CoOwner.Entity = null;
						previousValue.TasksCoOwned.Remove(this);
					}

					this._CoOwner.Entity = value;
					if (value != null)
					{
						value.TasksCoOwned.Add(this);
						
						this._CoOwnerId = value.PeopleId;
						
					}

					else
					{
						
						this._CoOwnerId = default(int?);
						
					}

					this.SendPropertyChanged("CoOwner");
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

