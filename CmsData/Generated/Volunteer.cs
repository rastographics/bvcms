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
	[Table(Name="dbo.Volunteer")]
	public partial class Volunteer : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
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
			
			this._VolunteerForms = new EntitySet<VolunteerForm>(new Action< VolunteerForm>(this.attach_VolunteerForms), new Action< VolunteerForm>(this.detach_VolunteerForms)); 
			
			this._VoluteerApprovalIds = new EntitySet<VoluteerApprovalId>(new Action< VoluteerApprovalId>(this.attach_VoluteerApprovalIds), new Action< VoluteerApprovalId>(this.detach_VoluteerApprovalIds)); 
			
			
			this._Person = default(EntityRef<Person>); 
			
			this._VolApplicationStatus = default(EntityRef<VolApplicationStatus>); 
			
			this._StatusMvr = default(EntityRef<VolApplicationStatus>); 
			
			OnCreated();
		}

		
    #region Columns
		
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

		
		[Column(Name="StatusId", UpdateCheck=UpdateCheck.Never, Storage="_StatusId", DbType="int")]
		[IsForeignKey]
		public int? StatusId
		{
			get { return this._StatusId; }

			set
			{
				if (this._StatusId != value)
				{
				
					if (this._VolApplicationStatus.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnStatusIdChanging(value);
					this.SendPropertyChanging();
					this._StatusId = value;
					this.SendPropertyChanged("StatusId");
					this.OnStatusIdChanged();
				}

			}

		}

		
		[Column(Name="ProcessedDate", UpdateCheck=UpdateCheck.Never, Storage="_ProcessedDate", DbType="datetime")]
		public DateTime? ProcessedDate
		{
			get { return this._ProcessedDate; }

			set
			{
				if (this._ProcessedDate != value)
				{
				
                    this.OnProcessedDateChanging(value);
					this.SendPropertyChanging();
					this._ProcessedDate = value;
					this.SendPropertyChanged("ProcessedDate");
					this.OnProcessedDateChanged();
				}

			}

		}

		
		[Column(Name="Standard", UpdateCheck=UpdateCheck.Never, Storage="_Standard", DbType="bit NOT NULL")]
		public bool Standard
		{
			get { return this._Standard; }

			set
			{
				if (this._Standard != value)
				{
				
                    this.OnStandardChanging(value);
					this.SendPropertyChanging();
					this._Standard = value;
					this.SendPropertyChanged("Standard");
					this.OnStandardChanged();
				}

			}

		}

		
		[Column(Name="Children", UpdateCheck=UpdateCheck.Never, Storage="_Children", DbType="bit NOT NULL")]
		public bool Children
		{
			get { return this._Children; }

			set
			{
				if (this._Children != value)
				{
				
                    this.OnChildrenChanging(value);
					this.SendPropertyChanging();
					this._Children = value;
					this.SendPropertyChanged("Children");
					this.OnChildrenChanged();
				}

			}

		}

		
		[Column(Name="Leader", UpdateCheck=UpdateCheck.Never, Storage="_Leader", DbType="bit NOT NULL")]
		public bool Leader
		{
			get { return this._Leader; }

			set
			{
				if (this._Leader != value)
				{
				
                    this.OnLeaderChanging(value);
					this.SendPropertyChanging();
					this._Leader = value;
					this.SendPropertyChanged("Leader");
					this.OnLeaderChanged();
				}

			}

		}

		
		[Column(Name="Comments", UpdateCheck=UpdateCheck.Never, Storage="_Comments", DbType="nvarchar")]
		public string Comments
		{
			get { return this._Comments; }

			set
			{
				if (this._Comments != value)
				{
				
                    this.OnCommentsChanging(value);
					this.SendPropertyChanging();
					this._Comments = value;
					this.SendPropertyChanged("Comments");
					this.OnCommentsChanged();
				}

			}

		}

		
		[Column(Name="MVRStatusId", UpdateCheck=UpdateCheck.Never, Storage="_MVRStatusId", DbType="int")]
		[IsForeignKey]
		public int? MVRStatusId
		{
			get { return this._MVRStatusId; }

			set
			{
				if (this._MVRStatusId != value)
				{
				
					if (this._StatusMvr.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnMVRStatusIdChanging(value);
					this.SendPropertyChanging();
					this._MVRStatusId = value;
					this.SendPropertyChanged("MVRStatusId");
					this.OnMVRStatusIdChanged();
				}

			}

		}

		
		[Column(Name="MVRProcessedDate", UpdateCheck=UpdateCheck.Never, Storage="_MVRProcessedDate", DbType="datetime")]
		public DateTime? MVRProcessedDate
		{
			get { return this._MVRProcessedDate; }

			set
			{
				if (this._MVRProcessedDate != value)
				{
				
                    this.OnMVRProcessedDateChanging(value);
					this.SendPropertyChanging();
					this._MVRProcessedDate = value;
					this.SendPropertyChanged("MVRProcessedDate");
					this.OnMVRProcessedDateChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_VolunteerForm_Volunteer1", Storage="_VolunteerForms", OtherKey="PeopleId")]
   		public EntitySet<VolunteerForm> VolunteerForms
   		{
   		    get { return this._VolunteerForms; }

			set	{ this._VolunteerForms.Assign(value); }

   		}

		
   		[Association(Name="FK_VoluteerApprovalIds_Volunteer", Storage="_VoluteerApprovalIds", OtherKey="PeopleId")]
   		public EntitySet<VoluteerApprovalId> VoluteerApprovalIds
   		{
   		    get { return this._VoluteerApprovalIds; }

			set	{ this._VoluteerApprovalIds.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_Volunteer_PEOPLE_TBL", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.Volunteers.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.Volunteers.Add(this);
						
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

		
		[Association(Name="FK_Volunteer_VolApplicationStatus", Storage="_VolApplicationStatus", ThisKey="StatusId", IsForeignKey=true)]
		public VolApplicationStatus VolApplicationStatus
		{
			get { return this._VolApplicationStatus.Entity; }

			set
			{
				VolApplicationStatus previousValue = this._VolApplicationStatus.Entity;
				if (((previousValue != value) 
							|| (this._VolApplicationStatus.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._VolApplicationStatus.Entity = null;
						previousValue.Volunteers.Remove(this);
					}

					this._VolApplicationStatus.Entity = value;
					if (value != null)
					{
						value.Volunteers.Add(this);
						
						this._StatusId = value.Id;
						
					}

					else
					{
						
						this._StatusId = default(int?);
						
					}

					this.SendPropertyChanged("VolApplicationStatus");
				}

			}

		}

		
		[Association(Name="StatusMvrId__StatusMvr", Storage="_StatusMvr", ThisKey="MVRStatusId", IsForeignKey=true)]
		public VolApplicationStatus StatusMvr
		{
			get { return this._StatusMvr.Entity; }

			set
			{
				VolApplicationStatus previousValue = this._StatusMvr.Entity;
				if (((previousValue != value) 
							|| (this._StatusMvr.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._StatusMvr.Entity = null;
						previousValue.StatusMvrId.Remove(this);
					}

					this._StatusMvr.Entity = value;
					if (value != null)
					{
						value.StatusMvrId.Add(this);
						
						this._MVRStatusId = value.Id;
						
					}

					else
					{
						
						this._MVRStatusId = default(int?);
						
					}

					this.SendPropertyChanged("StatusMvr");
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

   		
		private void attach_VolunteerForms(VolunteerForm entity)
		{
			this.SendPropertyChanging();
			entity.Volunteer = this;
		}

		private void detach_VolunteerForms(VolunteerForm entity)
		{
			this.SendPropertyChanging();
			entity.Volunteer = null;
		}

		
		private void attach_VoluteerApprovalIds(VoluteerApprovalId entity)
		{
			this.SendPropertyChanging();
			entity.Volunteer = this;
		}

		private void detach_VoluteerApprovalIds(VoluteerApprovalId entity)
		{
			this.SendPropertyChanging();
			entity.Volunteer = null;
		}

		
	}

}

