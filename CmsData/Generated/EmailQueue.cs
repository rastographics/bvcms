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
	[Table(Name="dbo.EmailQueue")]
	public partial class EmailQueue : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private DateTime? _SendWhen;
		
		private string _Subject;
		
		private string _Body;
		
		private string _FromAddr;
		
		private DateTime? _Sent;
		
		private DateTime? _Started;
		
		private DateTime _Queued;
		
		private string _FromName;
		
		private int? _QueuedBy;
		
		private bool? _Redacted;
		
		private bool? _Transactional;
		
		private bool? _PublicX;
		
		private string _Error;
		
		private bool? _CCParents;
		
		private bool? _NoReplacements;
		
		private int? _SendFromOrgId;
		
		private bool? _FinanceOnly;
		
		private string _CClist;
		
		private bool? _Testing;
		
		private bool? _ReadyToSend;
		
   		
   		private EntitySet<EmailLink> _EmailLinks;
		
   		private EntitySet<EmailQueueTo> _EmailQueueTos;
		
   		private EntitySet<EmailResponse> _EmailResponses;
		
    	
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnSendWhenChanging(DateTime? value);
		partial void OnSendWhenChanged();
		
		partial void OnSubjectChanging(string value);
		partial void OnSubjectChanged();
		
		partial void OnBodyChanging(string value);
		partial void OnBodyChanged();
		
		partial void OnFromAddrChanging(string value);
		partial void OnFromAddrChanged();
		
		partial void OnSentChanging(DateTime? value);
		partial void OnSentChanged();
		
		partial void OnStartedChanging(DateTime? value);
		partial void OnStartedChanged();
		
		partial void OnQueuedChanging(DateTime value);
		partial void OnQueuedChanged();
		
		partial void OnFromNameChanging(string value);
		partial void OnFromNameChanged();
		
		partial void OnQueuedByChanging(int? value);
		partial void OnQueuedByChanged();
		
		partial void OnRedactedChanging(bool? value);
		partial void OnRedactedChanged();
		
		partial void OnTransactionalChanging(bool? value);
		partial void OnTransactionalChanged();
		
		partial void OnPublicXChanging(bool? value);
		partial void OnPublicXChanged();
		
		partial void OnErrorChanging(string value);
		partial void OnErrorChanged();
		
		partial void OnCCParentsChanging(bool? value);
		partial void OnCCParentsChanged();
		
		partial void OnNoReplacementsChanging(bool? value);
		partial void OnNoReplacementsChanged();
		
		partial void OnSendFromOrgIdChanging(int? value);
		partial void OnSendFromOrgIdChanged();
		
		partial void OnFinanceOnlyChanging(bool? value);
		partial void OnFinanceOnlyChanged();
		
		partial void OnCClistChanging(string value);
		partial void OnCClistChanged();
		
		partial void OnTestingChanging(bool? value);
		partial void OnTestingChanged();
		
		partial void OnReadyToSendChanging(bool? value);
		partial void OnReadyToSendChanged();
		
    #endregion
		public EmailQueue()
		{
			
			this._EmailLinks = new EntitySet<EmailLink>(new Action< EmailLink>(this.attach_EmailLinks), new Action< EmailLink>(this.detach_EmailLinks)); 
			
			this._EmailQueueTos = new EntitySet<EmailQueueTo>(new Action< EmailQueueTo>(this.attach_EmailQueueTos), new Action< EmailQueueTo>(this.detach_EmailQueueTos)); 
			
			this._EmailResponses = new EntitySet<EmailResponse>(new Action< EmailResponse>(this.attach_EmailResponses), new Action< EmailResponse>(this.detach_EmailResponses)); 
			
			
			this._Person = default(EntityRef<Person>); 
			
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

		
		[Column(Name="SendWhen", UpdateCheck=UpdateCheck.Never, Storage="_SendWhen", DbType="datetime")]
		public DateTime? SendWhen
		{
			get { return this._SendWhen; }

			set
			{
				if (this._SendWhen != value)
				{
				
                    this.OnSendWhenChanging(value);
					this.SendPropertyChanging();
					this._SendWhen = value;
					this.SendPropertyChanged("SendWhen");
					this.OnSendWhenChanged();
				}

			}

		}

		
		[Column(Name="Subject", UpdateCheck=UpdateCheck.Never, Storage="_Subject", DbType="nvarchar(200)")]
		public string Subject
		{
			get { return this._Subject; }

			set
			{
				if (this._Subject != value)
				{
				
                    this.OnSubjectChanging(value);
					this.SendPropertyChanging();
					this._Subject = value;
					this.SendPropertyChanged("Subject");
					this.OnSubjectChanged();
				}

			}

		}

		
		[Column(Name="Body", UpdateCheck=UpdateCheck.Never, Storage="_Body", DbType="nvarchar")]
		public string Body
		{
			get { return this._Body; }

			set
			{
				if (this._Body != value)
				{
				
                    this.OnBodyChanging(value);
					this.SendPropertyChanging();
					this._Body = value;
					this.SendPropertyChanged("Body");
					this.OnBodyChanged();
				}

			}

		}

		
		[Column(Name="FromAddr", UpdateCheck=UpdateCheck.Never, Storage="_FromAddr", DbType="nvarchar(100)")]
		public string FromAddr
		{
			get { return this._FromAddr; }

			set
			{
				if (this._FromAddr != value)
				{
				
                    this.OnFromAddrChanging(value);
					this.SendPropertyChanging();
					this._FromAddr = value;
					this.SendPropertyChanged("FromAddr");
					this.OnFromAddrChanged();
				}

			}

		}

		
		[Column(Name="Sent", UpdateCheck=UpdateCheck.Never, Storage="_Sent", DbType="datetime")]
		public DateTime? Sent
		{
			get { return this._Sent; }

			set
			{
				if (this._Sent != value)
				{
				
                    this.OnSentChanging(value);
					this.SendPropertyChanging();
					this._Sent = value;
					this.SendPropertyChanged("Sent");
					this.OnSentChanged();
				}

			}

		}

		
		[Column(Name="Started", UpdateCheck=UpdateCheck.Never, Storage="_Started", DbType="datetime")]
		public DateTime? Started
		{
			get { return this._Started; }

			set
			{
				if (this._Started != value)
				{
				
                    this.OnStartedChanging(value);
					this.SendPropertyChanging();
					this._Started = value;
					this.SendPropertyChanged("Started");
					this.OnStartedChanged();
				}

			}

		}

		
		[Column(Name="Queued", UpdateCheck=UpdateCheck.Never, Storage="_Queued", DbType="datetime NOT NULL")]
		public DateTime Queued
		{
			get { return this._Queued; }

			set
			{
				if (this._Queued != value)
				{
				
                    this.OnQueuedChanging(value);
					this.SendPropertyChanging();
					this._Queued = value;
					this.SendPropertyChanged("Queued");
					this.OnQueuedChanged();
				}

			}

		}

		
		[Column(Name="FromName", UpdateCheck=UpdateCheck.Never, Storage="_FromName", DbType="nvarchar(60)")]
		public string FromName
		{
			get { return this._FromName; }

			set
			{
				if (this._FromName != value)
				{
				
                    this.OnFromNameChanging(value);
					this.SendPropertyChanging();
					this._FromName = value;
					this.SendPropertyChanged("FromName");
					this.OnFromNameChanged();
				}

			}

		}

		
		[Column(Name="QueuedBy", UpdateCheck=UpdateCheck.Never, Storage="_QueuedBy", DbType="int")]
		[IsForeignKey]
		public int? QueuedBy
		{
			get { return this._QueuedBy; }

			set
			{
				if (this._QueuedBy != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnQueuedByChanging(value);
					this.SendPropertyChanging();
					this._QueuedBy = value;
					this.SendPropertyChanged("QueuedBy");
					this.OnQueuedByChanged();
				}

			}

		}

		
		[Column(Name="Redacted", UpdateCheck=UpdateCheck.Never, Storage="_Redacted", DbType="bit")]
		public bool? Redacted
		{
			get { return this._Redacted; }

			set
			{
				if (this._Redacted != value)
				{
				
                    this.OnRedactedChanging(value);
					this.SendPropertyChanging();
					this._Redacted = value;
					this.SendPropertyChanged("Redacted");
					this.OnRedactedChanged();
				}

			}

		}

		
		[Column(Name="Transactional", UpdateCheck=UpdateCheck.Never, Storage="_Transactional", DbType="bit")]
		public bool? Transactional
		{
			get { return this._Transactional; }

			set
			{
				if (this._Transactional != value)
				{
				
                    this.OnTransactionalChanging(value);
					this.SendPropertyChanging();
					this._Transactional = value;
					this.SendPropertyChanged("Transactional");
					this.OnTransactionalChanged();
				}

			}

		}

		
		[Column(Name="Public", UpdateCheck=UpdateCheck.Never, Storage="_PublicX", DbType="bit")]
		public bool? PublicX
		{
			get { return this._PublicX; }

			set
			{
				if (this._PublicX != value)
				{
				
                    this.OnPublicXChanging(value);
					this.SendPropertyChanging();
					this._PublicX = value;
					this.SendPropertyChanged("PublicX");
					this.OnPublicXChanged();
				}

			}

		}

		
		[Column(Name="Error", UpdateCheck=UpdateCheck.Never, Storage="_Error", DbType="nvarchar(200)")]
		public string Error
		{
			get { return this._Error; }

			set
			{
				if (this._Error != value)
				{
				
                    this.OnErrorChanging(value);
					this.SendPropertyChanging();
					this._Error = value;
					this.SendPropertyChanged("Error");
					this.OnErrorChanged();
				}

			}

		}

		
		[Column(Name="CCParents", UpdateCheck=UpdateCheck.Never, Storage="_CCParents", DbType="bit")]
		public bool? CCParents
		{
			get { return this._CCParents; }

			set
			{
				if (this._CCParents != value)
				{
				
                    this.OnCCParentsChanging(value);
					this.SendPropertyChanging();
					this._CCParents = value;
					this.SendPropertyChanged("CCParents");
					this.OnCCParentsChanged();
				}

			}

		}

		
		[Column(Name="NoReplacements", UpdateCheck=UpdateCheck.Never, Storage="_NoReplacements", DbType="bit")]
		public bool? NoReplacements
		{
			get { return this._NoReplacements; }

			set
			{
				if (this._NoReplacements != value)
				{
				
                    this.OnNoReplacementsChanging(value);
					this.SendPropertyChanging();
					this._NoReplacements = value;
					this.SendPropertyChanged("NoReplacements");
					this.OnNoReplacementsChanged();
				}

			}

		}

		
		[Column(Name="SendFromOrgId", UpdateCheck=UpdateCheck.Never, Storage="_SendFromOrgId", DbType="int")]
		public int? SendFromOrgId
		{
			get { return this._SendFromOrgId; }

			set
			{
				if (this._SendFromOrgId != value)
				{
				
                    this.OnSendFromOrgIdChanging(value);
					this.SendPropertyChanging();
					this._SendFromOrgId = value;
					this.SendPropertyChanged("SendFromOrgId");
					this.OnSendFromOrgIdChanged();
				}

			}

		}

		
		[Column(Name="FinanceOnly", UpdateCheck=UpdateCheck.Never, Storage="_FinanceOnly", DbType="bit")]
		public bool? FinanceOnly
		{
			get { return this._FinanceOnly; }

			set
			{
				if (this._FinanceOnly != value)
				{
				
                    this.OnFinanceOnlyChanging(value);
					this.SendPropertyChanging();
					this._FinanceOnly = value;
					this.SendPropertyChanged("FinanceOnly");
					this.OnFinanceOnlyChanged();
				}

			}

		}

		
		[Column(Name="CClist", UpdateCheck=UpdateCheck.Never, Storage="_CClist", DbType="nvarchar")]
		public string CClist
		{
			get { return this._CClist; }

			set
			{
				if (this._CClist != value)
				{
				
                    this.OnCClistChanging(value);
					this.SendPropertyChanging();
					this._CClist = value;
					this.SendPropertyChanged("CClist");
					this.OnCClistChanged();
				}

			}

		}

		
		[Column(Name="Testing", UpdateCheck=UpdateCheck.Never, Storage="_Testing", DbType="bit")]
		public bool? Testing
		{
			get { return this._Testing; }

			set
			{
				if (this._Testing != value)
				{
				
                    this.OnTestingChanging(value);
					this.SendPropertyChanging();
					this._Testing = value;
					this.SendPropertyChanged("Testing");
					this.OnTestingChanged();
				}

			}

		}

		
		[Column(Name="ReadyToSend", UpdateCheck=UpdateCheck.Never, Storage="_ReadyToSend", DbType="bit")]
		public bool? ReadyToSend
		{
			get { return this._ReadyToSend; }

			set
			{
				if (this._ReadyToSend != value)
				{
				
                    this.OnReadyToSendChanging(value);
					this.SendPropertyChanging();
					this._ReadyToSend = value;
					this.SendPropertyChanged("ReadyToSend");
					this.OnReadyToSendChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_EmailLinks_EmailQueue", Storage="_EmailLinks", OtherKey="EmailID")]
   		public EntitySet<EmailLink> EmailLinks
   		{
   		    get { return this._EmailLinks; }

			set	{ this._EmailLinks.Assign(value); }

   		}

		
   		[Association(Name="FK_EmailQueueTo_EmailQueue", Storage="_EmailQueueTos", OtherKey="Id")]
   		public EntitySet<EmailQueueTo> EmailQueueTos
   		{
   		    get { return this._EmailQueueTos; }

			set	{ this._EmailQueueTos.Assign(value); }

   		}

		
   		[Association(Name="FK_EmailResponses_EmailQueue", Storage="_EmailResponses", OtherKey="EmailQueueId")]
   		public EntitySet<EmailResponse> EmailResponses
   		{
   		    get { return this._EmailResponses; }

			set	{ this._EmailResponses.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_EmailQueue_People", Storage="_Person", ThisKey="QueuedBy", IsForeignKey=true)]
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
						previousValue.EmailQueues.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.EmailQueues.Add(this);
						
						this._QueuedBy = value.PeopleId;
						
					}

					else
					{
						
						this._QueuedBy = default(int?);
						
					}

					this.SendPropertyChanged("Person");
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

   		
		private void attach_EmailLinks(EmailLink entity)
		{
			this.SendPropertyChanging();
			entity.EmailQueue = this;
		}

		private void detach_EmailLinks(EmailLink entity)
		{
			this.SendPropertyChanging();
			entity.EmailQueue = null;
		}

		
		private void attach_EmailQueueTos(EmailQueueTo entity)
		{
			this.SendPropertyChanging();
			entity.EmailQueue = this;
		}

		private void detach_EmailQueueTos(EmailQueueTo entity)
		{
			this.SendPropertyChanging();
			entity.EmailQueue = null;
		}

		
		private void attach_EmailResponses(EmailResponse entity)
		{
			this.SendPropertyChanging();
			entity.EmailQueue = this;
		}

		private void detach_EmailResponses(EmailResponse entity)
		{
			this.SendPropertyChanging();
			entity.EmailQueue = null;
		}

		
	}

}

