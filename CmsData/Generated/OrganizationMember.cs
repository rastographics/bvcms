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
	[Table(Name="dbo.OrganizationMembers")]
	public partial class OrganizationMember : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _OrganizationId;
		
		private int _PeopleId;
		
		private int? _CreatedBy;
		
		private DateTime? _CreatedDate;
		
		private int _MemberTypeId;
		
		private DateTime? _EnrollmentDate;
		
		private int? _ModifiedBy;
		
		private DateTime? _ModifiedDate;
		
		private DateTime? _InactiveDate;
		
		private string _AttendStr;
		
		private decimal? _AttendPct;
		
		private DateTime? _LastAttended;
		
		private bool? _Pending;
		
		private string _UserData;
		
		private decimal? _Amount;
		
		private string _Request;
		
		private string _ShirtSize;
		
		private int? _Grade;
		
		private int? _Tickets;
		
		private bool? _Moved;
		
		private string _RegisterEmail;
		
		private decimal? _AmountPaid;
		
		private string _PayLink;
		
		private int? _TranId;
		
		private int _Score;
		
		private int? _DatumId;
		
		private bool? _Hidden;
		
		private bool? _SkipInsertTriggerProcessing;
		
		private int? _RegistrationDataId;
		
		private string _OnlineRegData;
		
   		
   		private EntitySet<OrgMemberExtra> _OrgMemberExtras;
		
   		private EntitySet<OrgMemMemTag> _OrgMemMemTags;
		
    	
		private EntityRef<MemberType> _MemberType;
		
		private EntityRef<RegistrationDatum> _RegistrationDatum;
		
		private EntityRef<Transaction> _Transaction;
		
		private EntityRef<Organization> _Organization;
		
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnOrganizationIdChanging(int value);
		partial void OnOrganizationIdChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnCreatedByChanging(int? value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime? value);
		partial void OnCreatedDateChanged();
		
		partial void OnMemberTypeIdChanging(int value);
		partial void OnMemberTypeIdChanged();
		
		partial void OnEnrollmentDateChanging(DateTime? value);
		partial void OnEnrollmentDateChanged();
		
		partial void OnModifiedByChanging(int? value);
		partial void OnModifiedByChanged();
		
		partial void OnModifiedDateChanging(DateTime? value);
		partial void OnModifiedDateChanged();
		
		partial void OnInactiveDateChanging(DateTime? value);
		partial void OnInactiveDateChanged();
		
		partial void OnAttendStrChanging(string value);
		partial void OnAttendStrChanged();
		
		partial void OnAttendPctChanging(decimal? value);
		partial void OnAttendPctChanged();
		
		partial void OnLastAttendedChanging(DateTime? value);
		partial void OnLastAttendedChanged();
		
		partial void OnPendingChanging(bool? value);
		partial void OnPendingChanged();
		
		partial void OnUserDataChanging(string value);
		partial void OnUserDataChanged();
		
		partial void OnAmountChanging(decimal? value);
		partial void OnAmountChanged();
		
		partial void OnRequestChanging(string value);
		partial void OnRequestChanged();
		
		partial void OnShirtSizeChanging(string value);
		partial void OnShirtSizeChanged();
		
		partial void OnGradeChanging(int? value);
		partial void OnGradeChanged();
		
		partial void OnTicketsChanging(int? value);
		partial void OnTicketsChanged();
		
		partial void OnMovedChanging(bool? value);
		partial void OnMovedChanged();
		
		partial void OnRegisterEmailChanging(string value);
		partial void OnRegisterEmailChanged();
		
		partial void OnAmountPaidChanging(decimal? value);
		partial void OnAmountPaidChanged();
		
		partial void OnPayLinkChanging(string value);
		partial void OnPayLinkChanged();
		
		partial void OnTranIdChanging(int? value);
		partial void OnTranIdChanged();
		
		partial void OnScoreChanging(int value);
		partial void OnScoreChanged();
		
		partial void OnDatumIdChanging(int? value);
		partial void OnDatumIdChanged();
		
		partial void OnHiddenChanging(bool? value);
		partial void OnHiddenChanged();
		
		partial void OnSkipInsertTriggerProcessingChanging(bool? value);
		partial void OnSkipInsertTriggerProcessingChanged();
		
		partial void OnRegistrationDataIdChanging(int? value);
		partial void OnRegistrationDataIdChanged();
		
		partial void OnOnlineRegDataChanging(string value);
		partial void OnOnlineRegDataChanged();
		
    #endregion
		public OrganizationMember()
		{
			
			this._OrgMemberExtras = new EntitySet<OrgMemberExtra>(new Action< OrgMemberExtra>(this.attach_OrgMemberExtras), new Action< OrgMemberExtra>(this.detach_OrgMemberExtras)); 
			
			this._OrgMemMemTags = new EntitySet<OrgMemMemTag>(new Action< OrgMemMemTag>(this.attach_OrgMemMemTags), new Action< OrgMemMemTag>(this.detach_OrgMemMemTags)); 
			
			
			this._MemberType = default(EntityRef<MemberType>); 
			
			this._RegistrationDatum = default(EntityRef<RegistrationDatum>); 
			
			this._Transaction = default(EntityRef<Transaction>); 
			
			this._Organization = default(EntityRef<Organization>); 
			
			this._Person = default(EntityRef<Person>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="OrganizationId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationId", DbType="int NOT NULL", IsPrimaryKey=true)]
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

		
		[Column(Name="AttendStr", UpdateCheck=UpdateCheck.Never, Storage="_AttendStr", DbType="nvarchar(300)")]
		public string AttendStr
		{
			get { return this._AttendStr; }

			set
			{
				if (this._AttendStr != value)
				{
				
                    this.OnAttendStrChanging(value);
					this.SendPropertyChanging();
					this._AttendStr = value;
					this.SendPropertyChanged("AttendStr");
					this.OnAttendStrChanged();
				}

			}

		}

		
		[Column(Name="AttendPct", UpdateCheck=UpdateCheck.Never, Storage="_AttendPct", DbType="real")]
		public decimal? AttendPct
		{
			get { return this._AttendPct; }

			set
			{
				if (this._AttendPct != value)
				{
				
                    this.OnAttendPctChanging(value);
					this.SendPropertyChanging();
					this._AttendPct = value;
					this.SendPropertyChanged("AttendPct");
					this.OnAttendPctChanged();
				}

			}

		}

		
		[Column(Name="LastAttended", UpdateCheck=UpdateCheck.Never, Storage="_LastAttended", DbType="datetime")]
		public DateTime? LastAttended
		{
			get { return this._LastAttended; }

			set
			{
				if (this._LastAttended != value)
				{
				
                    this.OnLastAttendedChanging(value);
					this.SendPropertyChanging();
					this._LastAttended = value;
					this.SendPropertyChanged("LastAttended");
					this.OnLastAttendedChanged();
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

		
		[Column(Name="Amount", UpdateCheck=UpdateCheck.Never, Storage="_Amount", DbType="money")]
		public decimal? Amount
		{
			get { return this._Amount; }

			set
			{
				if (this._Amount != value)
				{
				
                    this.OnAmountChanging(value);
					this.SendPropertyChanging();
					this._Amount = value;
					this.SendPropertyChanged("Amount");
					this.OnAmountChanged();
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

		
		[Column(Name="Moved", UpdateCheck=UpdateCheck.Never, Storage="_Moved", DbType="bit")]
		public bool? Moved
		{
			get { return this._Moved; }

			set
			{
				if (this._Moved != value)
				{
				
                    this.OnMovedChanging(value);
					this.SendPropertyChanging();
					this._Moved = value;
					this.SendPropertyChanged("Moved");
					this.OnMovedChanged();
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

		
		[Column(Name="AmountPaid", UpdateCheck=UpdateCheck.Never, Storage="_AmountPaid", DbType="money")]
		public decimal? AmountPaid
		{
			get { return this._AmountPaid; }

			set
			{
				if (this._AmountPaid != value)
				{
				
                    this.OnAmountPaidChanging(value);
					this.SendPropertyChanging();
					this._AmountPaid = value;
					this.SendPropertyChanged("AmountPaid");
					this.OnAmountPaidChanged();
				}

			}

		}

		
		[Column(Name="PayLink", UpdateCheck=UpdateCheck.Never, Storage="_PayLink", DbType="nvarchar(100)")]
		public string PayLink
		{
			get { return this._PayLink; }

			set
			{
				if (this._PayLink != value)
				{
				
                    this.OnPayLinkChanging(value);
					this.SendPropertyChanging();
					this._PayLink = value;
					this.SendPropertyChanged("PayLink");
					this.OnPayLinkChanged();
				}

			}

		}

		
		[Column(Name="TranId", UpdateCheck=UpdateCheck.Never, Storage="_TranId", DbType="int")]
		[IsForeignKey]
		public int? TranId
		{
			get { return this._TranId; }

			set
			{
				if (this._TranId != value)
				{
				
					if (this._Transaction.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
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

		
		[Column(Name="DatumId", UpdateCheck=UpdateCheck.Never, Storage="_DatumId", DbType="int")]
		public int? DatumId
		{
			get { return this._DatumId; }

			set
			{
				if (this._DatumId != value)
				{
				
                    this.OnDatumIdChanging(value);
					this.SendPropertyChanging();
					this._DatumId = value;
					this.SendPropertyChanged("DatumId");
					this.OnDatumIdChanged();
				}

			}

		}

		
		[Column(Name="Hidden", UpdateCheck=UpdateCheck.Never, Storage="_Hidden", DbType="bit")]
		public bool? Hidden
		{
			get { return this._Hidden; }

			set
			{
				if (this._Hidden != value)
				{
				
                    this.OnHiddenChanging(value);
					this.SendPropertyChanging();
					this._Hidden = value;
					this.SendPropertyChanged("Hidden");
					this.OnHiddenChanged();
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

		
		[Column(Name="RegistrationDataId", UpdateCheck=UpdateCheck.Never, Storage="_RegistrationDataId", DbType="int")]
		[IsForeignKey]
		public int? RegistrationDataId
		{
			get { return this._RegistrationDataId; }

			set
			{
				if (this._RegistrationDataId != value)
				{
				
					if (this._RegistrationDatum.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnRegistrationDataIdChanging(value);
					this.SendPropertyChanging();
					this._RegistrationDataId = value;
					this.SendPropertyChanged("RegistrationDataId");
					this.OnRegistrationDataIdChanged();
				}

			}

		}

		
		[Column(Name="OnlineRegData", UpdateCheck=UpdateCheck.Never, Storage="_OnlineRegData", DbType="xml")]
		public string OnlineRegData
		{
			get { return this._OnlineRegData; }

			set
			{
				if (this._OnlineRegData != value)
				{
				
                    this.OnOnlineRegDataChanging(value);
					this.SendPropertyChanging();
					this._OnlineRegData = value;
					this.SendPropertyChanged("OnlineRegData");
					this.OnOnlineRegDataChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="FK_OrgMemberExtra_OrganizationMembers", Storage="_OrgMemberExtras", OtherKey="OrganizationId,PeopleId")]
   		public EntitySet<OrgMemberExtra> OrgMemberExtras
   		{
   		    get { return this._OrgMemberExtras; }

			set	{ this._OrgMemberExtras.Assign(value); }

   		}

		
   		[Association(Name="FK_OrgMemMemTags_OrganizationMembers", Storage="_OrgMemMemTags", OtherKey="OrgId,PeopleId")]
   		public EntitySet<OrgMemMemTag> OrgMemMemTags
   		{
   		    get { return this._OrgMemMemTags; }

			set	{ this._OrgMemMemTags.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_ORGANIZATION_MEMBERS_TBL_MemberType", Storage="_MemberType", ThisKey="MemberTypeId", IsForeignKey=true)]
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
						previousValue.OrganizationMembers.Remove(this);
					}

					this._MemberType.Entity = value;
					if (value != null)
					{
						value.OrganizationMembers.Add(this);
						
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

		
		[Association(Name="FK_OrganizationMembers_RegistrationData", Storage="_RegistrationDatum", ThisKey="RegistrationDataId", IsForeignKey=true)]
		public RegistrationDatum RegistrationDatum
		{
			get { return this._RegistrationDatum.Entity; }

			set
			{
				RegistrationDatum previousValue = this._RegistrationDatum.Entity;
				if (((previousValue != value) 
							|| (this._RegistrationDatum.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._RegistrationDatum.Entity = null;
						previousValue.OrganizationMembers.Remove(this);
					}

					this._RegistrationDatum.Entity = value;
					if (value != null)
					{
						value.OrganizationMembers.Add(this);
						
						this._RegistrationDataId = value.Id;
						
					}

					else
					{
						
						this._RegistrationDataId = default(int?);
						
					}

					this.SendPropertyChanged("RegistrationDatum");
				}

			}

		}

		
		[Association(Name="FK_OrganizationMembers_Transaction", Storage="_Transaction", ThisKey="TranId", IsForeignKey=true)]
		public Transaction Transaction
		{
			get { return this._Transaction.Entity; }

			set
			{
				Transaction previousValue = this._Transaction.Entity;
				if (((previousValue != value) 
							|| (this._Transaction.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Transaction.Entity = null;
						previousValue.OrganizationMembers.Remove(this);
					}

					this._Transaction.Entity = value;
					if (value != null)
					{
						value.OrganizationMembers.Add(this);
						
						this._TranId = value.Id;
						
					}

					else
					{
						
						this._TranId = default(int?);
						
					}

					this.SendPropertyChanged("Transaction");
				}

			}

		}

		
		[Association(Name="ORGANIZATION_MEMBERS_ORG_FK", Storage="_Organization", ThisKey="OrganizationId", IsForeignKey=true)]
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
						previousValue.OrganizationMembers.Remove(this);
					}

					this._Organization.Entity = value;
					if (value != null)
					{
						value.OrganizationMembers.Add(this);
						
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

		
		[Association(Name="ORGANIZATION_MEMBERS_PPL_FK", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.OrganizationMembers.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.OrganizationMembers.Add(this);
						
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

   		
		private void attach_OrgMemberExtras(OrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.OrganizationMember = this;
		}

		private void detach_OrgMemberExtras(OrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.OrganizationMember = null;
		}

		
		private void attach_OrgMemMemTags(OrgMemMemTag entity)
		{
			this.SendPropertyChanging();
			entity.OrganizationMember = this;
		}

		private void detach_OrgMemMemTags(OrgMemMemTag entity)
		{
			this.SendPropertyChanging();
			entity.OrganizationMember = null;
		}

		
	}

}

