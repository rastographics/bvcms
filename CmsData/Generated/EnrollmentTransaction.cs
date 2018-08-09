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
	[Table(Name="dbo.EnrollmentTransaction")]
	public partial class EnrollmentTransaction : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _TransactionId;
		
		private bool _TransactionStatus;
		
		private int? _CreatedBy;
		
		private DateTime? _CreatedDate;
		
		private DateTime _TransactionDate;
		
		private int _TransactionTypeId;
		
		private int _OrganizationId;
		
		private string _OrganizationName;
		
		private int _PeopleId;
		
		private int _MemberTypeId;
		
		private DateTime? _EnrollmentDate;
		
		private decimal? _AttendancePercentage;
		
		private DateTime? _NextTranChangeDate;
		
		private int? _EnrollmentTransactionId;
		
		private bool? _Pending;
		
		private DateTime? _InactiveDate;
		
		private string _UserData;
		
		private string _Request;
		
		private string _ShirtSize;
		
		private int? _Grade;
		
		private int? _Tickets;
		
		private string _RegisterEmail;
		
		private int? _TranId;
		
		private int _Score;
		
		private string _SmallGroups;
		
		private bool? _SkipInsertTriggerProcessing;
		
   		
   		private EntitySet<EnrollmentTransaction> _DescTransactions;
		
   		private EntitySet<PrevOrgMemberExtra> _PrevOrgMemberExtras;
		
    	
		private EntityRef<EnrollmentTransaction> _FirstTransaction;
		
		private EntityRef<Organization> _Organization;
		
		private EntityRef<Person> _Person;
		
		private EntityRef<MemberType> _MemberType;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnTransactionIdChanging(int value);
		partial void OnTransactionIdChanged();
		
		partial void OnTransactionStatusChanging(bool value);
		partial void OnTransactionStatusChanged();
		
		partial void OnCreatedByChanging(int? value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime? value);
		partial void OnCreatedDateChanged();
		
		partial void OnTransactionDateChanging(DateTime value);
		partial void OnTransactionDateChanged();
		
		partial void OnTransactionTypeIdChanging(int value);
		partial void OnTransactionTypeIdChanged();
		
		partial void OnOrganizationIdChanging(int value);
		partial void OnOrganizationIdChanged();
		
		partial void OnOrganizationNameChanging(string value);
		partial void OnOrganizationNameChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnMemberTypeIdChanging(int value);
		partial void OnMemberTypeIdChanged();
		
		partial void OnEnrollmentDateChanging(DateTime? value);
		partial void OnEnrollmentDateChanged();
		
		partial void OnAttendancePercentageChanging(decimal? value);
		partial void OnAttendancePercentageChanged();
		
		partial void OnNextTranChangeDateChanging(DateTime? value);
		partial void OnNextTranChangeDateChanged();
		
		partial void OnEnrollmentTransactionIdChanging(int? value);
		partial void OnEnrollmentTransactionIdChanged();
		
		partial void OnPendingChanging(bool? value);
		partial void OnPendingChanged();
		
		partial void OnInactiveDateChanging(DateTime? value);
		partial void OnInactiveDateChanged();
		
		partial void OnUserDataChanging(string value);
		partial void OnUserDataChanged();
		
		partial void OnRequestChanging(string value);
		partial void OnRequestChanged();
		
		partial void OnShirtSizeChanging(string value);
		partial void OnShirtSizeChanged();
		
		partial void OnGradeChanging(int? value);
		partial void OnGradeChanged();
		
		partial void OnTicketsChanging(int? value);
		partial void OnTicketsChanged();
		
		partial void OnRegisterEmailChanging(string value);
		partial void OnRegisterEmailChanged();
		
		partial void OnTranIdChanging(int? value);
		partial void OnTranIdChanged();
		
		partial void OnScoreChanging(int value);
		partial void OnScoreChanged();
		
		partial void OnSmallGroupsChanging(string value);
		partial void OnSmallGroupsChanged();
		
		partial void OnSkipInsertTriggerProcessingChanging(bool? value);
		partial void OnSkipInsertTriggerProcessingChanged();
		
    #endregion
		public EnrollmentTransaction()
		{
			
			this._DescTransactions = new EntitySet<EnrollmentTransaction>(new Action< EnrollmentTransaction>(this.attach_DescTransactions), new Action< EnrollmentTransaction>(this.detach_DescTransactions)); 
			
			this._PrevOrgMemberExtras = new EntitySet<PrevOrgMemberExtra>(new Action< PrevOrgMemberExtra>(this.attach_PrevOrgMemberExtras), new Action< PrevOrgMemberExtra>(this.detach_PrevOrgMemberExtras)); 
			
			
			this._FirstTransaction = default(EntityRef<EnrollmentTransaction>); 
			
			this._Organization = default(EntityRef<Organization>); 
			
			this._Person = default(EntityRef<Person>); 
			
			this._MemberType = default(EntityRef<MemberType>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="TransactionId", UpdateCheck=UpdateCheck.Never, Storage="_TransactionId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int TransactionId
		{
			get { return this._TransactionId; }

			set
			{
				if (this._TransactionId != value)
				{
				
                    this.OnTransactionIdChanging(value);
					this.SendPropertyChanging();
					this._TransactionId = value;
					this.SendPropertyChanged("TransactionId");
					this.OnTransactionIdChanged();
				}

			}

		}

		
		[Column(Name="TransactionStatus", UpdateCheck=UpdateCheck.Never, Storage="_TransactionStatus", DbType="bit NOT NULL")]
		public bool TransactionStatus
		{
			get { return this._TransactionStatus; }

			set
			{
				if (this._TransactionStatus != value)
				{
				
                    this.OnTransactionStatusChanging(value);
					this.SendPropertyChanging();
					this._TransactionStatus = value;
					this.SendPropertyChanged("TransactionStatus");
					this.OnTransactionStatusChanged();
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

		
		[Column(Name="CreatedDate", UpdateCheck=UpdateCheck.Never, Storage="_CreatedDate", DbType="datetime")]
		public DateTime? CreatedDate
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

		
		[Column(Name="TransactionDate", UpdateCheck=UpdateCheck.Never, Storage="_TransactionDate", DbType="datetime NOT NULL")]
		public DateTime TransactionDate
		{
			get { return this._TransactionDate; }

			set
			{
				if (this._TransactionDate != value)
				{
				
                    this.OnTransactionDateChanging(value);
					this.SendPropertyChanging();
					this._TransactionDate = value;
					this.SendPropertyChanged("TransactionDate");
					this.OnTransactionDateChanged();
				}

			}

		}

		
		[Column(Name="TransactionTypeId", UpdateCheck=UpdateCheck.Never, Storage="_TransactionTypeId", DbType="int NOT NULL")]
		public int TransactionTypeId
		{
			get { return this._TransactionTypeId; }

			set
			{
				if (this._TransactionTypeId != value)
				{
				
                    this.OnTransactionTypeIdChanging(value);
					this.SendPropertyChanging();
					this._TransactionTypeId = value;
					this.SendPropertyChanged("TransactionTypeId");
					this.OnTransactionTypeIdChanged();
				}

			}

		}

		
		[Column(Name="OrganizationId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int OrganizationId
		{
			get { return this._OrganizationId; }

			set
			{
				if (this._OrganizationId != value)
				{
				
					if (this._Organization.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrganizationIdChanging(value);
					this.SendPropertyChanging();
					this._OrganizationId = value;
					this.SendPropertyChanged("OrganizationId");
					this.OnOrganizationIdChanged();
				}

			}

		}

		
		[Column(Name="OrganizationName", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationName", DbType="nvarchar(100) NOT NULL")]
		public string OrganizationName
		{
			get { return this._OrganizationName; }

			set
			{
				if (this._OrganizationName != value)
				{
				
                    this.OnOrganizationNameChanging(value);
					this.SendPropertyChanging();
					this._OrganizationName = value;
					this.SendPropertyChanged("OrganizationName");
					this.OnOrganizationNameChanged();
				}

			}

		}

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL")]
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

		
		[Column(Name="MemberTypeId", UpdateCheck=UpdateCheck.Never, Storage="_MemberTypeId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int MemberTypeId
		{
			get { return this._MemberTypeId; }

			set
			{
				if (this._MemberTypeId != value)
				{
				
					if (this._MemberType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnMemberTypeIdChanging(value);
					this.SendPropertyChanging();
					this._MemberTypeId = value;
					this.SendPropertyChanged("MemberTypeId");
					this.OnMemberTypeIdChanged();
				}

			}

		}

		
		[Column(Name="EnrollmentDate", UpdateCheck=UpdateCheck.Never, Storage="_EnrollmentDate", DbType="datetime")]
		public DateTime? EnrollmentDate
		{
			get { return this._EnrollmentDate; }

			set
			{
				if (this._EnrollmentDate != value)
				{
				
                    this.OnEnrollmentDateChanging(value);
					this.SendPropertyChanging();
					this._EnrollmentDate = value;
					this.SendPropertyChanged("EnrollmentDate");
					this.OnEnrollmentDateChanged();
				}

			}

		}

		
		[Column(Name="AttendancePercentage", UpdateCheck=UpdateCheck.Never, Storage="_AttendancePercentage", DbType="real")]
		public decimal? AttendancePercentage
		{
			get { return this._AttendancePercentage; }

			set
			{
				if (this._AttendancePercentage != value)
				{
				
                    this.OnAttendancePercentageChanging(value);
					this.SendPropertyChanging();
					this._AttendancePercentage = value;
					this.SendPropertyChanged("AttendancePercentage");
					this.OnAttendancePercentageChanged();
				}

			}

		}

		
		[Column(Name="NextTranChangeDate", UpdateCheck=UpdateCheck.Never, Storage="_NextTranChangeDate", DbType="datetime")]
		public DateTime? NextTranChangeDate
		{
			get { return this._NextTranChangeDate; }

			set
			{
				if (this._NextTranChangeDate != value)
				{
				
                    this.OnNextTranChangeDateChanging(value);
					this.SendPropertyChanging();
					this._NextTranChangeDate = value;
					this.SendPropertyChanged("NextTranChangeDate");
					this.OnNextTranChangeDateChanged();
				}

			}

		}

		
		[Column(Name="EnrollmentTransactionId", UpdateCheck=UpdateCheck.Never, Storage="_EnrollmentTransactionId", DbType="int")]
		[IsForeignKey]
		public int? EnrollmentTransactionId
		{
			get { return this._EnrollmentTransactionId; }

			set
			{
				if (this._EnrollmentTransactionId != value)
				{
				
					if (this._FirstTransaction.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnEnrollmentTransactionIdChanging(value);
					this.SendPropertyChanging();
					this._EnrollmentTransactionId = value;
					this.SendPropertyChanged("EnrollmentTransactionId");
					this.OnEnrollmentTransactionIdChanged();
				}

			}

		}

		
		[Column(Name="Pending", UpdateCheck=UpdateCheck.Never, Storage="_Pending", DbType="bit")]
		public bool? Pending
		{
			get { return this._Pending; }

			set
			{
				if (this._Pending != value)
				{
				
                    this.OnPendingChanging(value);
					this.SendPropertyChanging();
					this._Pending = value;
					this.SendPropertyChanged("Pending");
					this.OnPendingChanged();
				}

			}

		}

		
		[Column(Name="InactiveDate", UpdateCheck=UpdateCheck.Never, Storage="_InactiveDate", DbType="datetime")]
		public DateTime? InactiveDate
		{
			get { return this._InactiveDate; }

			set
			{
				if (this._InactiveDate != value)
				{
				
                    this.OnInactiveDateChanging(value);
					this.SendPropertyChanging();
					this._InactiveDate = value;
					this.SendPropertyChanged("InactiveDate");
					this.OnInactiveDateChanged();
				}

			}

		}

		
		[Column(Name="UserData", UpdateCheck=UpdateCheck.Never, Storage="_UserData", DbType="nvarchar")]
		public string UserData
		{
			get { return this._UserData; }

			set
			{
				if (this._UserData != value)
				{
				
                    this.OnUserDataChanging(value);
					this.SendPropertyChanging();
					this._UserData = value;
					this.SendPropertyChanged("UserData");
					this.OnUserDataChanged();
				}

			}

		}

		
		[Column(Name="Request", UpdateCheck=UpdateCheck.Never, Storage="_Request", DbType="nvarchar(140)")]
		public string Request
		{
			get { return this._Request; }

			set
			{
				if (this._Request != value)
				{
				
                    this.OnRequestChanging(value);
					this.SendPropertyChanging();
					this._Request = value;
					this.SendPropertyChanged("Request");
					this.OnRequestChanged();
				}

			}

		}

		
		[Column(Name="ShirtSize", UpdateCheck=UpdateCheck.Never, Storage="_ShirtSize", DbType="nvarchar(50)")]
		public string ShirtSize
		{
			get { return this._ShirtSize; }

			set
			{
				if (this._ShirtSize != value)
				{
				
                    this.OnShirtSizeChanging(value);
					this.SendPropertyChanging();
					this._ShirtSize = value;
					this.SendPropertyChanged("ShirtSize");
					this.OnShirtSizeChanged();
				}

			}

		}

		
		[Column(Name="Grade", UpdateCheck=UpdateCheck.Never, Storage="_Grade", DbType="int")]
		public int? Grade
		{
			get { return this._Grade; }

			set
			{
				if (this._Grade != value)
				{
				
                    this.OnGradeChanging(value);
					this.SendPropertyChanging();
					this._Grade = value;
					this.SendPropertyChanged("Grade");
					this.OnGradeChanged();
				}

			}

		}

		
		[Column(Name="Tickets", UpdateCheck=UpdateCheck.Never, Storage="_Tickets", DbType="int")]
		public int? Tickets
		{
			get { return this._Tickets; }

			set
			{
				if (this._Tickets != value)
				{
				
                    this.OnTicketsChanging(value);
					this.SendPropertyChanging();
					this._Tickets = value;
					this.SendPropertyChanged("Tickets");
					this.OnTicketsChanged();
				}

			}

		}

		
		[Column(Name="RegisterEmail", UpdateCheck=UpdateCheck.Never, Storage="_RegisterEmail", DbType="nvarchar(80)")]
		public string RegisterEmail
		{
			get { return this._RegisterEmail; }

			set
			{
				if (this._RegisterEmail != value)
				{
				
                    this.OnRegisterEmailChanging(value);
					this.SendPropertyChanging();
					this._RegisterEmail = value;
					this.SendPropertyChanged("RegisterEmail");
					this.OnRegisterEmailChanged();
				}

			}

		}

		
		[Column(Name="TranId", UpdateCheck=UpdateCheck.Never, Storage="_TranId", DbType="int")]
		public int? TranId
		{
			get { return this._TranId; }

			set
			{
				if (this._TranId != value)
				{
				
                    this.OnTranIdChanging(value);
					this.SendPropertyChanging();
					this._TranId = value;
					this.SendPropertyChanged("TranId");
					this.OnTranIdChanged();
				}

			}

		}

		
		[Column(Name="Score", UpdateCheck=UpdateCheck.Never, Storage="_Score", DbType="int NOT NULL")]
		public int Score
		{
			get { return this._Score; }

			set
			{
				if (this._Score != value)
				{
				
                    this.OnScoreChanging(value);
					this.SendPropertyChanging();
					this._Score = value;
					this.SendPropertyChanged("Score");
					this.OnScoreChanged();
				}

			}

		}

		
		[Column(Name="SmallGroups", UpdateCheck=UpdateCheck.Never, Storage="_SmallGroups", DbType="nvarchar(2000)")]
		public string SmallGroups
		{
			get { return this._SmallGroups; }

			set
			{
				if (this._SmallGroups != value)
				{
				
                    this.OnSmallGroupsChanging(value);
					this.SendPropertyChanging();
					this._SmallGroups = value;
					this.SendPropertyChanged("SmallGroups");
					this.OnSmallGroupsChanged();
				}

			}

		}

		
		[Column(Name="SkipInsertTriggerProcessing", UpdateCheck=UpdateCheck.Never, Storage="_SkipInsertTriggerProcessing", DbType="bit")]
		public bool? SkipInsertTriggerProcessing
		{
			get { return this._SkipInsertTriggerProcessing; }

			set
			{
				if (this._SkipInsertTriggerProcessing != value)
				{
				
                    this.OnSkipInsertTriggerProcessingChanging(value);
					this.SendPropertyChanging();
					this._SkipInsertTriggerProcessing = value;
					this.SendPropertyChanged("SkipInsertTriggerProcessing");
					this.OnSkipInsertTriggerProcessingChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="DescTransactions__FirstTransaction", Storage="_DescTransactions", OtherKey="EnrollmentTransactionId")]
   		public EntitySet<EnrollmentTransaction> DescTransactions
   		{
   		    get { return this._DescTransactions; }

			set	{ this._DescTransactions.Assign(value); }

   		}

		
   		[Association(Name="FK_PrevOrgMemberExtra_EnrollmentTransaction", Storage="_PrevOrgMemberExtras", OtherKey="EnrollmentTranId")]
   		public EntitySet<PrevOrgMemberExtra> PrevOrgMemberExtras
   		{
   		    get { return this._PrevOrgMemberExtras; }

			set	{ this._PrevOrgMemberExtras.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="DescTransactions__FirstTransaction", Storage="_FirstTransaction", ThisKey="EnrollmentTransactionId", IsForeignKey=true)]
		public EnrollmentTransaction FirstTransaction
		{
			get { return this._FirstTransaction.Entity; }

			set
			{
				EnrollmentTransaction previousValue = this._FirstTransaction.Entity;
				if (((previousValue != value) 
							|| (this._FirstTransaction.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._FirstTransaction.Entity = null;
						previousValue.DescTransactions.Remove(this);
					}

					this._FirstTransaction.Entity = value;
					if (value != null)
					{
						value.DescTransactions.Add(this);
						
						this._EnrollmentTransactionId = value.TransactionId;
						
					}

					else
					{
						
						this._EnrollmentTransactionId = default(int?);
						
					}

					this.SendPropertyChanged("FirstTransaction");
				}

			}

		}

		
		[Association(Name="ENROLLMENT_TRANSACTION_ORG_FK", Storage="_Organization", ThisKey="OrganizationId", IsForeignKey=true)]
		public Organization Organization
		{
			get { return this._Organization.Entity; }

			set
			{
				Organization previousValue = this._Organization.Entity;
				if (((previousValue != value) 
							|| (this._Organization.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Organization.Entity = null;
						previousValue.EnrollmentTransactions.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.EnrollmentTransactions.Add(this);
						
						this._OrganizationId = value.OrganizationId;
						
					}

					else
					{
						
						this._OrganizationId = default(int);
						
					}

					this.SendPropertyChanged("Organization");
				}

			}

		}

		
		[Association(Name="ENROLLMENT_TRANSACTION_PPL_FK", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.EnrollmentTransactions.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.EnrollmentTransactions.Add(this);
						
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

		
		[Association(Name="FK_ENROLLMENT_TRANSACTION_TBL_MemberType", Storage="_MemberType", ThisKey="MemberTypeId", IsForeignKey=true)]
		public MemberType MemberType
		{
			get { return this._MemberType.Entity; }

			set
			{
				MemberType previousValue = this._MemberType.Entity;
				if (((previousValue != value) 
							|| (this._MemberType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._MemberType.Entity = null;
						previousValue.EnrollmentTransactions.Remove(this);
					}

					this._MemberType.Entity = value;
					if (value != null)
					{
						value.EnrollmentTransactions.Add(this);
						
						this._MemberTypeId = value.Id;
						
					}

					else
					{
						
						this._MemberTypeId = default(int);
						
					}

					this.SendPropertyChanged("MemberType");
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

   		
		private void attach_DescTransactions(EnrollmentTransaction entity)
		{
			this.SendPropertyChanging();
			entity.FirstTransaction = this;
		}

		private void detach_DescTransactions(EnrollmentTransaction entity)
		{
			this.SendPropertyChanging();
			entity.FirstTransaction = null;
		}

		
		private void attach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.EnrollmentTransaction = this;
		}

		private void detach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.EnrollmentTransaction = null;
		}

		
	}

}

