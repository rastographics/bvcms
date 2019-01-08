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
	[Table(Name="dbo.Contact")]
	public partial class Contact : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _ContactId;
		
		private int _CreatedBy;
		
		private DateTime _CreatedDate;
		
		private int? _ContactTypeId;
		
		private DateTime _ContactDate;
		
		private int? _ContactReasonId;
		
		private int? _MinistryId;
		
		private bool? _NotAtHome;
		
		private bool? _LeftDoorHanger;
		
		private bool? _LeftMessage;
		
		private bool? _GospelShared;
		
		private bool? _PrayerRequest;
		
		private bool? _ContactMade;
		
		private bool? _GiftBagGiven;
		
		private string _Comments;
		
		private int? _ModifiedBy;
		
		private DateTime? _ModifiedDate;
		
		private string _LimitToRole;
		
		private int? _OrganizationId;
		
   		
   		private EntitySet<Contactee> _contactees;
		
   		private EntitySet<Contactor> _contactsMakers;
		
   		private EntitySet<ContactExtra> _ContactExtras;
		
   		private EntitySet<Task> _TasksAssigned;
		
   		private EntitySet<Task> _TasksCompleted;
		
    	
		private EntityRef<Organization> _organization;
		
		private EntityRef<ContactType> _ContactType;
		
		private EntityRef<Ministry> _Ministry;
		
		private EntityRef<ContactReason> _ContactReason;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnContactIdChanging(int value);
		partial void OnContactIdChanged();
		
		partial void OnCreatedByChanging(int value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime value);
		partial void OnCreatedDateChanged();
		
		partial void OnContactTypeIdChanging(int? value);
		partial void OnContactTypeIdChanged();
		
		partial void OnContactDateChanging(DateTime value);
		partial void OnContactDateChanged();
		
		partial void OnContactReasonIdChanging(int? value);
		partial void OnContactReasonIdChanged();
		
		partial void OnMinistryIdChanging(int? value);
		partial void OnMinistryIdChanged();
		
		partial void OnNotAtHomeChanging(bool? value);
		partial void OnNotAtHomeChanged();
		
		partial void OnLeftDoorHangerChanging(bool? value);
		partial void OnLeftDoorHangerChanged();
		
		partial void OnLeftMessageChanging(bool? value);
		partial void OnLeftMessageChanged();
		
		partial void OnGospelSharedChanging(bool? value);
		partial void OnGospelSharedChanged();
		
		partial void OnPrayerRequestChanging(bool? value);
		partial void OnPrayerRequestChanged();
		
		partial void OnContactMadeChanging(bool? value);
		partial void OnContactMadeChanged();
		
		partial void OnGiftBagGivenChanging(bool? value);
		partial void OnGiftBagGivenChanged();
		
		partial void OnCommentsChanging(string value);
		partial void OnCommentsChanged();
		
		partial void OnModifiedByChanging(int? value);
		partial void OnModifiedByChanged();
		
		partial void OnModifiedDateChanging(DateTime? value);
		partial void OnModifiedDateChanged();
		
		partial void OnLimitToRoleChanging(string value);
		partial void OnLimitToRoleChanged();
		
		partial void OnOrganizationIdChanging(int? value);
		partial void OnOrganizationIdChanged();
		
    #endregion
		public Contact()
		{
			
			this._contactees = new EntitySet<Contactee>(new Action< Contactee>(this.attach_contactees), new Action< Contactee>(this.detach_contactees)); 
			
			this._contactsMakers = new EntitySet<Contactor>(new Action< Contactor>(this.attach_contactsMakers), new Action< Contactor>(this.detach_contactsMakers)); 
			
			this._ContactExtras = new EntitySet<ContactExtra>(new Action< ContactExtra>(this.attach_ContactExtras), new Action< ContactExtra>(this.detach_ContactExtras)); 
			
			this._TasksAssigned = new EntitySet<Task>(new Action< Task>(this.attach_TasksAssigned), new Action< Task>(this.detach_TasksAssigned)); 
			
			this._TasksCompleted = new EntitySet<Task>(new Action< Task>(this.attach_TasksCompleted), new Action< Task>(this.detach_TasksCompleted)); 
			
			
			this._organization = default(EntityRef<Organization>); 
			
			this._ContactType = default(EntityRef<ContactType>); 
			
			this._Ministry = default(EntityRef<Ministry>); 
			
			this._ContactReason = default(EntityRef<ContactReason>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="ContactId", UpdateCheck=UpdateCheck.Never, Storage="_ContactId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ContactId
		{
			get { return this._ContactId; }

			set
			{
				if (this._ContactId != value)
				{
				
                    this.OnContactIdChanging(value);
					this.SendPropertyChanging();
					this._ContactId = value;
					this.SendPropertyChanged("ContactId");
					this.OnContactIdChanged();
				}

			}

		}

		
		[Column(Name="CreatedBy", UpdateCheck=UpdateCheck.Never, Storage="_CreatedBy", DbType="int NOT NULL")]
		public int CreatedBy
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

		
		[Column(Name="CreatedDate", UpdateCheck=UpdateCheck.Never, Storage="_CreatedDate", DbType="datetime NOT NULL")]
		public DateTime CreatedDate
		{
			get { return this._CreatedDate; }

			set
			{
				if (this._CreatedDate != value)
				{
				
                    this.OnCreatedDateChanging(value);
					this.SendPropertyChanging();
					this._CreatedDate = value;
					this.SendPropertyChanged("CreatedDate");
					this.OnCreatedDateChanged();
				}

			}

		}

		
		[Column(Name="ContactTypeId", UpdateCheck=UpdateCheck.Never, Storage="_ContactTypeId", DbType="int")]
		[IsForeignKey]
		public int? ContactTypeId
		{
			get { return this._ContactTypeId; }

			set
			{
				if (this._ContactTypeId != value)
				{
				
					if (this._ContactType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnContactTypeIdChanging(value);
					this.SendPropertyChanging();
					this._ContactTypeId = value;
					this.SendPropertyChanged("ContactTypeId");
					this.OnContactTypeIdChanged();
				}

			}

		}

		
		[Column(Name="ContactDate", UpdateCheck=UpdateCheck.Never, Storage="_ContactDate", DbType="datetime NOT NULL")]
		public DateTime ContactDate
		{
			get { return this._ContactDate; }

			set
			{
				if (this._ContactDate != value)
				{
				
                    this.OnContactDateChanging(value);
					this.SendPropertyChanging();
					this._ContactDate = value;
					this.SendPropertyChanged("ContactDate");
					this.OnContactDateChanged();
				}

			}

		}

		
		[Column(Name="ContactReasonId", UpdateCheck=UpdateCheck.Never, Storage="_ContactReasonId", DbType="int")]
		[IsForeignKey]
		public int? ContactReasonId
		{
			get { return this._ContactReasonId; }

			set
			{
				if (this._ContactReasonId != value)
				{
				
					if (this._ContactReason.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnContactReasonIdChanging(value);
					this.SendPropertyChanging();
					this._ContactReasonId = value;
					this.SendPropertyChanged("ContactReasonId");
					this.OnContactReasonIdChanged();
				}

			}

		}

		
		[Column(Name="MinistryId", UpdateCheck=UpdateCheck.Never, Storage="_MinistryId", DbType="int")]
		[IsForeignKey]
		public int? MinistryId
		{
			get { return this._MinistryId; }

			set
			{
				if (this._MinistryId != value)
				{
				
					if (this._Ministry.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnMinistryIdChanging(value);
					this.SendPropertyChanging();
					this._MinistryId = value;
					this.SendPropertyChanged("MinistryId");
					this.OnMinistryIdChanged();
				}

			}

		}

		
		[Column(Name="NotAtHome", UpdateCheck=UpdateCheck.Never, Storage="_NotAtHome", DbType="bit")]
		public bool? NotAtHome
		{
			get { return this._NotAtHome; }

			set
			{
				if (this._NotAtHome != value)
				{
				
                    this.OnNotAtHomeChanging(value);
					this.SendPropertyChanging();
					this._NotAtHome = value;
					this.SendPropertyChanged("NotAtHome");
					this.OnNotAtHomeChanged();
				}

			}

		}

		
		[Column(Name="LeftDoorHanger", UpdateCheck=UpdateCheck.Never, Storage="_LeftDoorHanger", DbType="bit")]
		public bool? LeftDoorHanger
		{
			get { return this._LeftDoorHanger; }

			set
			{
				if (this._LeftDoorHanger != value)
				{
				
                    this.OnLeftDoorHangerChanging(value);
					this.SendPropertyChanging();
					this._LeftDoorHanger = value;
					this.SendPropertyChanged("LeftDoorHanger");
					this.OnLeftDoorHangerChanged();
				}

			}

		}

		
		[Column(Name="LeftMessage", UpdateCheck=UpdateCheck.Never, Storage="_LeftMessage", DbType="bit")]
		public bool? LeftMessage
		{
			get { return this._LeftMessage; }

			set
			{
				if (this._LeftMessage != value)
				{
				
                    this.OnLeftMessageChanging(value);
					this.SendPropertyChanging();
					this._LeftMessage = value;
					this.SendPropertyChanged("LeftMessage");
					this.OnLeftMessageChanged();
				}

			}

		}

		
		[Column(Name="GospelShared", UpdateCheck=UpdateCheck.Never, Storage="_GospelShared", DbType="bit")]
		public bool? GospelShared
		{
			get { return this._GospelShared; }

			set
			{
				if (this._GospelShared != value)
				{
				
                    this.OnGospelSharedChanging(value);
					this.SendPropertyChanging();
					this._GospelShared = value;
					this.SendPropertyChanged("GospelShared");
					this.OnGospelSharedChanged();
				}

			}

		}

		
		[Column(Name="PrayerRequest", UpdateCheck=UpdateCheck.Never, Storage="_PrayerRequest", DbType="bit")]
		public bool? PrayerRequest
		{
			get { return this._PrayerRequest; }

			set
			{
				if (this._PrayerRequest != value)
				{
				
                    this.OnPrayerRequestChanging(value);
					this.SendPropertyChanging();
					this._PrayerRequest = value;
					this.SendPropertyChanged("PrayerRequest");
					this.OnPrayerRequestChanged();
				}

			}

		}

		
		[Column(Name="ContactMade", UpdateCheck=UpdateCheck.Never, Storage="_ContactMade", DbType="bit")]
		public bool? ContactMade
		{
			get { return this._ContactMade; }

			set
			{
				if (this._ContactMade != value)
				{
				
                    this.OnContactMadeChanging(value);
					this.SendPropertyChanging();
					this._ContactMade = value;
					this.SendPropertyChanged("ContactMade");
					this.OnContactMadeChanged();
				}

			}

		}

		
		[Column(Name="GiftBagGiven", UpdateCheck=UpdateCheck.Never, Storage="_GiftBagGiven", DbType="bit")]
		public bool? GiftBagGiven
		{
			get { return this._GiftBagGiven; }

			set
			{
				if (this._GiftBagGiven != value)
				{
				
                    this.OnGiftBagGivenChanging(value);
					this.SendPropertyChanging();
					this._GiftBagGiven = value;
					this.SendPropertyChanged("GiftBagGiven");
					this.OnGiftBagGivenChanged();
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

		
		[Column(Name="ModifiedDate", UpdateCheck=UpdateCheck.Never, Storage="_ModifiedDate", DbType="datetime")]
		public DateTime? ModifiedDate
		{
			get { return this._ModifiedDate; }

			set
			{
				if (this._ModifiedDate != value)
				{
				
                    this.OnModifiedDateChanging(value);
					this.SendPropertyChanging();
					this._ModifiedDate = value;
					this.SendPropertyChanged("ModifiedDate");
					this.OnModifiedDateChanged();
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

		
		[Column(Name="OrganizationId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationId", DbType="int")]
		[IsForeignKey]
		public int? OrganizationId
		{
			get { return this._OrganizationId; }

			set
			{
				if (this._OrganizationId != value)
				{
				
					if (this._organization.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrganizationIdChanging(value);
					this.SendPropertyChanging();
					this._OrganizationId = value;
					this.SendPropertyChanged("OrganizationId");
					this.OnOrganizationIdChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="contactees__contact", Storage="_contactees", OtherKey="ContactId")]
   		public EntitySet<Contactee> contactees
   		{
   		    get { return this._contactees; }

			set	{ this._contactees.Assign(value); }

   		}

		
   		[Association(Name="contactsMakers__contact", Storage="_contactsMakers", OtherKey="ContactId")]
   		public EntitySet<Contactor> contactsMakers
   		{
   		    get { return this._contactsMakers; }

			set	{ this._contactsMakers.Assign(value); }

   		}

		
   		[Association(Name="FK_ContactExtra_Contact", Storage="_ContactExtras", OtherKey="ContactId")]
   		public EntitySet<ContactExtra> ContactExtras
   		{
   		    get { return this._ContactExtras; }

			set	{ this._ContactExtras.Assign(value); }

   		}

		
   		[Association(Name="TasksAssigned__SourceContact", Storage="_TasksAssigned", OtherKey="SourceContactId")]
   		public EntitySet<Task> TasksAssigned
   		{
   		    get { return this._TasksAssigned; }

			set	{ this._TasksAssigned.Assign(value); }

   		}

		
   		[Association(Name="TasksCompleted__CompletedContact", Storage="_TasksCompleted", OtherKey="CompletedContactId")]
   		public EntitySet<Task> TasksCompleted
   		{
   		    get { return this._TasksCompleted; }

			set	{ this._TasksCompleted.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="contactsHad__organization", Storage="_organization", ThisKey="OrganizationId", IsForeignKey=true)]
		public Organization organization
		{
			get { return this._organization.Entity; }

			set
			{
				Organization previousValue = this._organization.Entity;
				if (((previousValue != value) 
							|| (this._organization.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._organization.Entity = null;
						previousValue.contactsHad.Remove(this);
					}

					this._organization.Entity = value;
					if (value != null)
					{
						value.contactsHad.Add(this);
						
						this._OrganizationId = value.OrganizationId;
						
					}

					else
					{
						
						this._OrganizationId = default(int?);
						
					}

					this.SendPropertyChanged("organization");
				}

			}

		}

		
		[Association(Name="FK_Contacts_ContactTypes", Storage="_ContactType", ThisKey="ContactTypeId", IsForeignKey=true)]
		public ContactType ContactType
		{
			get { return this._ContactType.Entity; }

			set
			{
				ContactType previousValue = this._ContactType.Entity;
				if (((previousValue != value) 
							|| (this._ContactType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ContactType.Entity = null;
						previousValue.Contacts.Remove(this);
					}

					this._ContactType.Entity = value;
					if (value != null)
					{
						value.Contacts.Add(this);
						
						this._ContactTypeId = value.Id;
						
					}

					else
					{
						
						this._ContactTypeId = default(int?);
						
					}

					this.SendPropertyChanged("ContactType");
				}

			}

		}

		
		[Association(Name="FK_Contacts_Ministries", Storage="_Ministry", ThisKey="MinistryId", IsForeignKey=true)]
		public Ministry Ministry
		{
			get { return this._Ministry.Entity; }

			set
			{
				Ministry previousValue = this._Ministry.Entity;
				if (((previousValue != value) 
							|| (this._Ministry.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Ministry.Entity = null;
						previousValue.Contacts.Remove(this);
					}

					this._Ministry.Entity = value;
					if (value != null)
					{
						value.Contacts.Add(this);
						
						this._MinistryId = value.MinistryId;
						
					}

					else
					{
						
						this._MinistryId = default(int?);
						
					}

					this.SendPropertyChanged("Ministry");
				}

			}

		}

		
		[Association(Name="FK_NewContacts_ContactReasons", Storage="_ContactReason", ThisKey="ContactReasonId", IsForeignKey=true)]
		public ContactReason ContactReason
		{
			get { return this._ContactReason.Entity; }

			set
			{
				ContactReason previousValue = this._ContactReason.Entity;
				if (((previousValue != value) 
							|| (this._ContactReason.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ContactReason.Entity = null;
						previousValue.Contacts.Remove(this);
					}

					this._ContactReason.Entity = value;
					if (value != null)
					{
						value.Contacts.Add(this);
						
						this._ContactReasonId = value.Id;
						
					}

					else
					{
						
						this._ContactReasonId = default(int?);
						
					}

					this.SendPropertyChanged("ContactReason");
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

   		
		private void attach_contactees(Contactee entity)
		{
			this.SendPropertyChanging();
			entity.contact = this;
		}

		private void detach_contactees(Contactee entity)
		{
			this.SendPropertyChanging();
			entity.contact = null;
		}

		
		private void attach_contactsMakers(Contactor entity)
		{
			this.SendPropertyChanging();
			entity.contact = this;
		}

		private void detach_contactsMakers(Contactor entity)
		{
			this.SendPropertyChanging();
			entity.contact = null;
		}

		
		private void attach_ContactExtras(ContactExtra entity)
		{
			this.SendPropertyChanging();
			entity.Contact = this;
		}

		private void detach_ContactExtras(ContactExtra entity)
		{
			this.SendPropertyChanging();
			entity.Contact = null;
		}

		
		private void attach_TasksAssigned(Task entity)
		{
			this.SendPropertyChanging();
			entity.SourceContact = this;
		}

		private void detach_TasksAssigned(Task entity)
		{
			this.SendPropertyChanging();
			entity.SourceContact = null;
		}

		
		private void attach_TasksCompleted(Task entity)
		{
			this.SendPropertyChanging();
			entity.CompletedContact = this;
		}

		private void detach_TasksCompleted(Task entity)
		{
			this.SendPropertyChanging();
			entity.CompletedContact = null;
		}

		
	}

}

