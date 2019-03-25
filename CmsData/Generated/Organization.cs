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
	[Table(Name="dbo.Organizations")]
	public partial class Organization : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _OrganizationId;
		
		private int _CreatedBy;
		
		private DateTime _CreatedDate;
		
		private int _OrganizationStatusId;
		
		private int? _DivisionId;
		
		private int? _LeaderMemberTypeId;
		
		private int? _GradeAgeStart;
		
		private int? _GradeAgeEnd;
		
		private int? _RollSheetVisitorWks;
		
		private int _SecurityTypeId;
		
		private DateTime? _FirstMeetingDate;
		
		private DateTime? _LastMeetingDate;
		
		private DateTime? _OrganizationClosedDate;
		
		private string _Location;
		
		private string _OrganizationName;
		
		private int? _ModifiedBy;
		
		private DateTime? _ModifiedDate;
		
		private int? _EntryPointId;
		
		private int? _ParentOrgId;
		
		private bool _AllowAttendOverlap;
		
		private int? _MemberCount;
		
		private int? _LeaderId;
		
		private string _LeaderName;
		
		private bool? _ClassFilled;
		
		private int? _OnLineCatalogSort;
		
		private string _PendingLoc;
		
		private bool? _CanSelfCheckin;
		
		private int? _NumCheckInLabels;
		
		private int? _CampusId;
		
		private bool? _AllowNonCampusCheckIn;
		
		private int? _NumWorkerCheckInLabels;
		
		private bool? _ShowOnlyRegisteredAtCheckIn;
		
		private int? _Limit;
		
		private int? _GenderId;
		
		private string _Description;
		
		private DateTime? _BirthDayStart;
		
		private DateTime? _BirthDayEnd;
		
		private DateTime? _LastDayBeforeExtra;
		
		private int? _RegistrationTypeId;
		
		private string _ValidateOrgs;
		
		private string _PhoneNumber;
		
		private bool? _RegistrationClosed;
		
		private bool? _AllowKioskRegister;
		
		private string _WorshipGroupCodes;
		
		private bool? _IsBibleFellowshipOrg;
		
		private bool? _NoSecurityLabel;
		
		private bool? _AlwaysSecurityLabel;
		
		private int? _DaysToIgnoreHistory;
		
		private string _NotifyIds;
		
		private double? _Lat;
		
		private double? _LongX;
		
		private string _RegSetting;
		
		private string _OrgPickList;
		
		private bool? _Offsite;
		
		private DateTime? _RegStart;
		
		private DateTime? _RegEnd;
		
		private string _LimitToRole;
		
		private int? _OrganizationTypeId;
		
		private string _MemberJoinScript;
		
		private string _AddToSmallGroupScript;
		
		private string _RemoveFromSmallGroupScript;
		
		private bool? _SuspendCheckin;
		
		private bool? _NoAutoAbsents;
		
		private int? _PublishDirectory;
		
		private int? _ConsecutiveAbsentsThreshold;
		
		private bool _IsRecreationTeam;
		
		private bool? _NotWeekly;
		
		private bool? _IsMissionTrip;
		
		private bool? _NoCreditCards;
		
		private string _GiftNotifyIds;
		
		private DateTime? _VisitorDate;
		
		private bool? _UseBootstrap;
		
		private string _PublicSortOrder;
		
		private bool? _UseRegisterLink2;
		
		private string _AppCategory;
		
		private string _RegistrationTitle;
		
		private int? _PrevMemberCount;
		
		private int? _ProspectCount;
		
		private string _RegSettingXml;
		
		private bool? _AttendanceBySubGroups;
		
		private bool _SendAttendanceLink;
		
		private bool _TripFundingPagesEnable;
		
		private bool _TripFundingPagesPublic;
		
		private bool _TripFundingPagesShowAmounts;
		
   		
   		private EntitySet<Person> _BFMembers;
		
   		private EntitySet<Organization> _ChildOrgs;
		
   		private EntitySet<Contact> _contactsHad;
		
   		private EntitySet<EnrollmentTransaction> _EnrollmentTransactions;
		
   		private EntitySet<Attend> _Attends;
		
   		private EntitySet<Coupon> _Coupons;
		
   		private EntitySet<DivOrg> _DivOrgs;
		
   		private EntitySet<GoerSenderAmount> _GoerSenderAmounts;
		
   		private EntitySet<Meeting> _Meetings;
		
   		private EntitySet<MemberTag> _MemberTags;
		
   		private EntitySet<OrganizationExtra> _OrganizationExtras;
		
   		private EntitySet<OrgMemberExtra> _OrgMemberExtras;
		
   		private EntitySet<OrgSchedule> _OrgSchedules;
		
   		private EntitySet<PrevOrgMemberExtra> _PrevOrgMemberExtras;
		
   		private EntitySet<Resource> _Resources;
		
   		private EntitySet<ResourceOrganization> _ResourceOrganizations;
		
   		private EntitySet<OrganizationMember> _OrganizationMembers;
		
    	
		private EntityRef<Organization> _ParentOrg;
		
		private EntityRef<Campu> _Campu;
		
		private EntityRef<Division> _Division;
		
		private EntityRef<Gender> _Gender;
		
		private EntityRef<OrganizationType> _OrganizationType;
		
		private EntityRef<EntryPoint> _EntryPoint;
		
		private EntityRef<OrganizationStatus> _OrganizationStatus;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnOrganizationIdChanging(int value);
		partial void OnOrganizationIdChanged();
		
		partial void OnCreatedByChanging(int value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime value);
		partial void OnCreatedDateChanged();
		
		partial void OnOrganizationStatusIdChanging(int value);
		partial void OnOrganizationStatusIdChanged();
		
		partial void OnDivisionIdChanging(int? value);
		partial void OnDivisionIdChanged();
		
		partial void OnLeaderMemberTypeIdChanging(int? value);
		partial void OnLeaderMemberTypeIdChanged();
		
		partial void OnGradeAgeStartChanging(int? value);
		partial void OnGradeAgeStartChanged();
		
		partial void OnGradeAgeEndChanging(int? value);
		partial void OnGradeAgeEndChanged();
		
		partial void OnRollSheetVisitorWksChanging(int? value);
		partial void OnRollSheetVisitorWksChanged();
		
		partial void OnSecurityTypeIdChanging(int value);
		partial void OnSecurityTypeIdChanged();
		
		partial void OnFirstMeetingDateChanging(DateTime? value);
		partial void OnFirstMeetingDateChanged();
		
		partial void OnLastMeetingDateChanging(DateTime? value);
		partial void OnLastMeetingDateChanged();
		
		partial void OnOrganizationClosedDateChanging(DateTime? value);
		partial void OnOrganizationClosedDateChanged();
		
		partial void OnLocationChanging(string value);
		partial void OnLocationChanged();
		
		partial void OnOrganizationNameChanging(string value);
		partial void OnOrganizationNameChanged();
		
		partial void OnModifiedByChanging(int? value);
		partial void OnModifiedByChanged();
		
		partial void OnModifiedDateChanging(DateTime? value);
		partial void OnModifiedDateChanged();
		
		partial void OnEntryPointIdChanging(int? value);
		partial void OnEntryPointIdChanged();
		
		partial void OnParentOrgIdChanging(int? value);
		partial void OnParentOrgIdChanged();
		
		partial void OnAllowAttendOverlapChanging(bool value);
		partial void OnAllowAttendOverlapChanged();
		
		partial void OnMemberCountChanging(int? value);
		partial void OnMemberCountChanged();
		
		partial void OnLeaderIdChanging(int? value);
		partial void OnLeaderIdChanged();
		
		partial void OnLeaderNameChanging(string value);
		partial void OnLeaderNameChanged();
		
		partial void OnClassFilledChanging(bool? value);
		partial void OnClassFilledChanged();
		
		partial void OnOnLineCatalogSortChanging(int? value);
		partial void OnOnLineCatalogSortChanged();
		
		partial void OnPendingLocChanging(string value);
		partial void OnPendingLocChanged();
		
		partial void OnCanSelfCheckinChanging(bool? value);
		partial void OnCanSelfCheckinChanged();
		
		partial void OnNumCheckInLabelsChanging(int? value);
		partial void OnNumCheckInLabelsChanged();
		
		partial void OnCampusIdChanging(int? value);
		partial void OnCampusIdChanged();
		
		partial void OnAllowNonCampusCheckInChanging(bool? value);
		partial void OnAllowNonCampusCheckInChanged();
		
		partial void OnNumWorkerCheckInLabelsChanging(int? value);
		partial void OnNumWorkerCheckInLabelsChanged();
		
		partial void OnShowOnlyRegisteredAtCheckInChanging(bool? value);
		partial void OnShowOnlyRegisteredAtCheckInChanged();
		
		partial void OnLimitChanging(int? value);
		partial void OnLimitChanged();
		
		partial void OnGenderIdChanging(int? value);
		partial void OnGenderIdChanged();
		
		partial void OnDescriptionChanging(string value);
		partial void OnDescriptionChanged();
		
		partial void OnBirthDayStartChanging(DateTime? value);
		partial void OnBirthDayStartChanged();
		
		partial void OnBirthDayEndChanging(DateTime? value);
		partial void OnBirthDayEndChanged();
		
		partial void OnLastDayBeforeExtraChanging(DateTime? value);
		partial void OnLastDayBeforeExtraChanged();
		
		partial void OnRegistrationTypeIdChanging(int? value);
		partial void OnRegistrationTypeIdChanged();
		
		partial void OnValidateOrgsChanging(string value);
		partial void OnValidateOrgsChanged();
		
		partial void OnPhoneNumberChanging(string value);
		partial void OnPhoneNumberChanged();
		
		partial void OnRegistrationClosedChanging(bool? value);
		partial void OnRegistrationClosedChanged();
		
		partial void OnAllowKioskRegisterChanging(bool? value);
		partial void OnAllowKioskRegisterChanged();
		
		partial void OnWorshipGroupCodesChanging(string value);
		partial void OnWorshipGroupCodesChanged();
		
		partial void OnIsBibleFellowshipOrgChanging(bool? value);
		partial void OnIsBibleFellowshipOrgChanged();
		
		partial void OnNoSecurityLabelChanging(bool? value);
		partial void OnNoSecurityLabelChanged();
		
		partial void OnAlwaysSecurityLabelChanging(bool? value);
		partial void OnAlwaysSecurityLabelChanged();
		
		partial void OnDaysToIgnoreHistoryChanging(int? value);
		partial void OnDaysToIgnoreHistoryChanged();
		
		partial void OnNotifyIdsChanging(string value);
		partial void OnNotifyIdsChanged();
		
		partial void OnLatChanging(double? value);
		partial void OnLatChanged();
		
		partial void OnLongXChanging(double? value);
		partial void OnLongXChanged();
		
		partial void OnRegSettingChanging(string value);
		partial void OnRegSettingChanged();
		
		partial void OnOrgPickListChanging(string value);
		partial void OnOrgPickListChanged();
		
		partial void OnOffsiteChanging(bool? value);
		partial void OnOffsiteChanged();
		
		partial void OnRegStartChanging(DateTime? value);
		partial void OnRegStartChanged();
		
		partial void OnRegEndChanging(DateTime? value);
		partial void OnRegEndChanged();
		
		partial void OnLimitToRoleChanging(string value);
		partial void OnLimitToRoleChanged();
		
		partial void OnOrganizationTypeIdChanging(int? value);
		partial void OnOrganizationTypeIdChanged();
		
		partial void OnMemberJoinScriptChanging(string value);
		partial void OnMemberJoinScriptChanged();
		
		partial void OnAddToSmallGroupScriptChanging(string value);
		partial void OnAddToSmallGroupScriptChanged();
		
		partial void OnRemoveFromSmallGroupScriptChanging(string value);
		partial void OnRemoveFromSmallGroupScriptChanged();
		
		partial void OnSuspendCheckinChanging(bool? value);
		partial void OnSuspendCheckinChanged();
		
		partial void OnNoAutoAbsentsChanging(bool? value);
		partial void OnNoAutoAbsentsChanged();
		
		partial void OnPublishDirectoryChanging(int? value);
		partial void OnPublishDirectoryChanged();
		
		partial void OnConsecutiveAbsentsThresholdChanging(int? value);
		partial void OnConsecutiveAbsentsThresholdChanged();
		
		partial void OnIsRecreationTeamChanging(bool value);
		partial void OnIsRecreationTeamChanged();
		
		partial void OnNotWeeklyChanging(bool? value);
		partial void OnNotWeeklyChanged();
		
		partial void OnIsMissionTripChanging(bool? value);
		partial void OnIsMissionTripChanged();
		
		partial void OnNoCreditCardsChanging(bool? value);
		partial void OnNoCreditCardsChanged();
		
		partial void OnGiftNotifyIdsChanging(string value);
		partial void OnGiftNotifyIdsChanged();
		
		partial void OnVisitorDateChanging(DateTime? value);
		partial void OnVisitorDateChanged();
		
		partial void OnUseBootstrapChanging(bool? value);
		partial void OnUseBootstrapChanged();
		
		partial void OnPublicSortOrderChanging(string value);
		partial void OnPublicSortOrderChanged();
		
		partial void OnUseRegisterLink2Changing(bool? value);
		partial void OnUseRegisterLink2Changed();
		
		partial void OnAppCategoryChanging(string value);
		partial void OnAppCategoryChanged();
		
		partial void OnRegistrationTitleChanging(string value);
		partial void OnRegistrationTitleChanged();
		
		partial void OnPrevMemberCountChanging(int? value);
		partial void OnPrevMemberCountChanged();
		
		partial void OnProspectCountChanging(int? value);
		partial void OnProspectCountChanged();
		
		partial void OnRegSettingXmlChanging(string value);
		partial void OnRegSettingXmlChanged();
		
		partial void OnAttendanceBySubGroupsChanging(bool? value);
		partial void OnAttendanceBySubGroupsChanged();
		
		partial void OnSendAttendanceLinkChanging(bool value);
		partial void OnSendAttendanceLinkChanged();
		
		partial void OnTripFundingPagesEnableChanging(bool value);
		partial void OnTripFundingPagesEnableChanged();
		
		partial void OnTripFundingPagesPublicChanging(bool value);
		partial void OnTripFundingPagesPublicChanged();
		
		partial void OnTripFundingPagesShowAmountsChanging(bool value);
		partial void OnTripFundingPagesShowAmountsChanged();
		
    #endregion
		public Organization()
		{
			
			this._BFMembers = new EntitySet<Person>(new Action< Person>(this.attach_BFMembers), new Action< Person>(this.detach_BFMembers)); 
			
			this._ChildOrgs = new EntitySet<Organization>(new Action< Organization>(this.attach_ChildOrgs), new Action< Organization>(this.detach_ChildOrgs)); 
			
			this._contactsHad = new EntitySet<Contact>(new Action< Contact>(this.attach_contactsHad), new Action< Contact>(this.detach_contactsHad)); 
			
			this._EnrollmentTransactions = new EntitySet<EnrollmentTransaction>(new Action< EnrollmentTransaction>(this.attach_EnrollmentTransactions), new Action< EnrollmentTransaction>(this.detach_EnrollmentTransactions)); 
			
			this._Attends = new EntitySet<Attend>(new Action< Attend>(this.attach_Attends), new Action< Attend>(this.detach_Attends)); 
			
			this._Coupons = new EntitySet<Coupon>(new Action< Coupon>(this.attach_Coupons), new Action< Coupon>(this.detach_Coupons)); 
			
			this._DivOrgs = new EntitySet<DivOrg>(new Action< DivOrg>(this.attach_DivOrgs), new Action< DivOrg>(this.detach_DivOrgs)); 
			
			this._GoerSenderAmounts = new EntitySet<GoerSenderAmount>(new Action< GoerSenderAmount>(this.attach_GoerSenderAmounts), new Action< GoerSenderAmount>(this.detach_GoerSenderAmounts)); 
			
			this._Meetings = new EntitySet<Meeting>(new Action< Meeting>(this.attach_Meetings), new Action< Meeting>(this.detach_Meetings)); 
			
			this._MemberTags = new EntitySet<MemberTag>(new Action< MemberTag>(this.attach_MemberTags), new Action< MemberTag>(this.detach_MemberTags)); 
			
			this._OrganizationExtras = new EntitySet<OrganizationExtra>(new Action< OrganizationExtra>(this.attach_OrganizationExtras), new Action< OrganizationExtra>(this.detach_OrganizationExtras)); 
			
			this._OrgMemberExtras = new EntitySet<OrgMemberExtra>(new Action< OrgMemberExtra>(this.attach_OrgMemberExtras), new Action< OrgMemberExtra>(this.detach_OrgMemberExtras)); 
			
			this._OrgSchedules = new EntitySet<OrgSchedule>(new Action< OrgSchedule>(this.attach_OrgSchedules), new Action< OrgSchedule>(this.detach_OrgSchedules)); 
			
			this._PrevOrgMemberExtras = new EntitySet<PrevOrgMemberExtra>(new Action< PrevOrgMemberExtra>(this.attach_PrevOrgMemberExtras), new Action< PrevOrgMemberExtra>(this.detach_PrevOrgMemberExtras)); 
			
			this._Resources = new EntitySet<Resource>(new Action< Resource>(this.attach_Resources), new Action< Resource>(this.detach_Resources)); 
			
			this._ResourceOrganizations = new EntitySet<ResourceOrganization>(new Action< ResourceOrganization>(this.attach_ResourceOrganizations), new Action< ResourceOrganization>(this.detach_ResourceOrganizations)); 
			
			this._OrganizationMembers = new EntitySet<OrganizationMember>(new Action< OrganizationMember>(this.attach_OrganizationMembers), new Action< OrganizationMember>(this.detach_OrganizationMembers)); 
			
			
			this._ParentOrg = default(EntityRef<Organization>); 
			
			this._Campu = default(EntityRef<Campu>); 
			
			this._Division = default(EntityRef<Division>); 
			
			this._Gender = default(EntityRef<Gender>); 
			
			this._OrganizationType = default(EntityRef<OrganizationType>); 
			
			this._EntryPoint = default(EntityRef<EntryPoint>); 
			
			this._OrganizationStatus = default(EntityRef<OrganizationStatus>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="OrganizationId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int OrganizationId
		{
			get { return this._OrganizationId; }

			set
			{
				if (this._OrganizationId != value)
				{
				
                    this.OnOrganizationIdChanging(value);
					this.SendPropertyChanging();
					this._OrganizationId = value;
					this.SendPropertyChanged("OrganizationId");
					this.OnOrganizationIdChanged();
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

		
		[Column(Name="OrganizationStatusId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationStatusId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int OrganizationStatusId
		{
			get { return this._OrganizationStatusId; }

			set
			{
				if (this._OrganizationStatusId != value)
				{
				
					if (this._OrganizationStatus.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrganizationStatusIdChanging(value);
					this.SendPropertyChanging();
					this._OrganizationStatusId = value;
					this.SendPropertyChanged("OrganizationStatusId");
					this.OnOrganizationStatusIdChanged();
				}

			}

		}

		
		[Column(Name="DivisionId", UpdateCheck=UpdateCheck.Never, Storage="_DivisionId", DbType="int")]
		[IsForeignKey]
		public int? DivisionId
		{
			get { return this._DivisionId; }

			set
			{
				if (this._DivisionId != value)
				{
				
					if (this._Division.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnDivisionIdChanging(value);
					this.SendPropertyChanging();
					this._DivisionId = value;
					this.SendPropertyChanged("DivisionId");
					this.OnDivisionIdChanged();
				}

			}

		}

		
		[Column(Name="LeaderMemberTypeId", UpdateCheck=UpdateCheck.Never, Storage="_LeaderMemberTypeId", DbType="int")]
		public int? LeaderMemberTypeId
		{
			get { return this._LeaderMemberTypeId; }

			set
			{
				if (this._LeaderMemberTypeId != value)
				{
				
                    this.OnLeaderMemberTypeIdChanging(value);
					this.SendPropertyChanging();
					this._LeaderMemberTypeId = value;
					this.SendPropertyChanged("LeaderMemberTypeId");
					this.OnLeaderMemberTypeIdChanged();
				}

			}

		}

		
		[Column(Name="GradeAgeStart", UpdateCheck=UpdateCheck.Never, Storage="_GradeAgeStart", DbType="int")]
		public int? GradeAgeStart
		{
			get { return this._GradeAgeStart; }

			set
			{
				if (this._GradeAgeStart != value)
				{
				
                    this.OnGradeAgeStartChanging(value);
					this.SendPropertyChanging();
					this._GradeAgeStart = value;
					this.SendPropertyChanged("GradeAgeStart");
					this.OnGradeAgeStartChanged();
				}

			}

		}

		
		[Column(Name="GradeAgeEnd", UpdateCheck=UpdateCheck.Never, Storage="_GradeAgeEnd", DbType="int")]
		public int? GradeAgeEnd
		{
			get { return this._GradeAgeEnd; }

			set
			{
				if (this._GradeAgeEnd != value)
				{
				
                    this.OnGradeAgeEndChanging(value);
					this.SendPropertyChanging();
					this._GradeAgeEnd = value;
					this.SendPropertyChanged("GradeAgeEnd");
					this.OnGradeAgeEndChanged();
				}

			}

		}

		
		[Column(Name="RollSheetVisitorWks", UpdateCheck=UpdateCheck.Never, Storage="_RollSheetVisitorWks", DbType="int")]
		public int? RollSheetVisitorWks
		{
			get { return this._RollSheetVisitorWks; }

			set
			{
				if (this._RollSheetVisitorWks != value)
				{
				
                    this.OnRollSheetVisitorWksChanging(value);
					this.SendPropertyChanging();
					this._RollSheetVisitorWks = value;
					this.SendPropertyChanged("RollSheetVisitorWks");
					this.OnRollSheetVisitorWksChanged();
				}

			}

		}

		
		[Column(Name="SecurityTypeId", UpdateCheck=UpdateCheck.Never, Storage="_SecurityTypeId", DbType="int NOT NULL")]
		public int SecurityTypeId
		{
			get { return this._SecurityTypeId; }

			set
			{
				if (this._SecurityTypeId != value)
				{
				
                    this.OnSecurityTypeIdChanging(value);
					this.SendPropertyChanging();
					this._SecurityTypeId = value;
					this.SendPropertyChanged("SecurityTypeId");
					this.OnSecurityTypeIdChanged();
				}

			}

		}

		
		[Column(Name="FirstMeetingDate", UpdateCheck=UpdateCheck.Never, Storage="_FirstMeetingDate", DbType="datetime")]
		public DateTime? FirstMeetingDate
		{
			get { return this._FirstMeetingDate; }

			set
			{
				if (this._FirstMeetingDate != value)
				{
				
                    this.OnFirstMeetingDateChanging(value);
					this.SendPropertyChanging();
					this._FirstMeetingDate = value;
					this.SendPropertyChanged("FirstMeetingDate");
					this.OnFirstMeetingDateChanged();
				}

			}

		}

		
		[Column(Name="LastMeetingDate", UpdateCheck=UpdateCheck.Never, Storage="_LastMeetingDate", DbType="datetime")]
		public DateTime? LastMeetingDate
		{
			get { return this._LastMeetingDate; }

			set
			{
				if (this._LastMeetingDate != value)
				{
				
                    this.OnLastMeetingDateChanging(value);
					this.SendPropertyChanging();
					this._LastMeetingDate = value;
					this.SendPropertyChanged("LastMeetingDate");
					this.OnLastMeetingDateChanged();
				}

			}

		}

		
		[Column(Name="OrganizationClosedDate", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationClosedDate", DbType="datetime")]
		public DateTime? OrganizationClosedDate
		{
			get { return this._OrganizationClosedDate; }

			set
			{
				if (this._OrganizationClosedDate != value)
				{
				
                    this.OnOrganizationClosedDateChanging(value);
					this.SendPropertyChanging();
					this._OrganizationClosedDate = value;
					this.SendPropertyChanged("OrganizationClosedDate");
					this.OnOrganizationClosedDateChanged();
				}

			}

		}

		
		[Column(Name="Location", UpdateCheck=UpdateCheck.Never, Storage="_Location", DbType="nvarchar(200)")]
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

		
		[Column(Name="EntryPointId", UpdateCheck=UpdateCheck.Never, Storage="_EntryPointId", DbType="int")]
		[IsForeignKey]
		public int? EntryPointId
		{
			get { return this._EntryPointId; }

			set
			{
				if (this._EntryPointId != value)
				{
				
					if (this._EntryPoint.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnEntryPointIdChanging(value);
					this.SendPropertyChanging();
					this._EntryPointId = value;
					this.SendPropertyChanged("EntryPointId");
					this.OnEntryPointIdChanged();
				}

			}

		}

		
		[Column(Name="ParentOrgId", UpdateCheck=UpdateCheck.Never, Storage="_ParentOrgId", DbType="int")]
		[IsForeignKey]
		public int? ParentOrgId
		{
			get { return this._ParentOrgId; }

			set
			{
				if (this._ParentOrgId != value)
				{
				
					if (this._ParentOrg.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnParentOrgIdChanging(value);
					this.SendPropertyChanging();
					this._ParentOrgId = value;
					this.SendPropertyChanged("ParentOrgId");
					this.OnParentOrgIdChanged();
				}

			}

		}

		
		[Column(Name="AllowAttendOverlap", UpdateCheck=UpdateCheck.Never, Storage="_AllowAttendOverlap", DbType="bit NOT NULL")]
		public bool AllowAttendOverlap
		{
			get { return this._AllowAttendOverlap; }

			set
			{
				if (this._AllowAttendOverlap != value)
				{
				
                    this.OnAllowAttendOverlapChanging(value);
					this.SendPropertyChanging();
					this._AllowAttendOverlap = value;
					this.SendPropertyChanged("AllowAttendOverlap");
					this.OnAllowAttendOverlapChanged();
				}

			}

		}

		
		[Column(Name="MemberCount", UpdateCheck=UpdateCheck.Never, Storage="_MemberCount", DbType="int")]
		public int? MemberCount
		{
			get { return this._MemberCount; }

			set
			{
				if (this._MemberCount != value)
				{
				
                    this.OnMemberCountChanging(value);
					this.SendPropertyChanging();
					this._MemberCount = value;
					this.SendPropertyChanged("MemberCount");
					this.OnMemberCountChanged();
				}

			}

		}

		
		[Column(Name="LeaderId", UpdateCheck=UpdateCheck.Never, Storage="_LeaderId", DbType="int")]
		public int? LeaderId
		{
			get { return this._LeaderId; }

			set
			{
				if (this._LeaderId != value)
				{
				
                    this.OnLeaderIdChanging(value);
					this.SendPropertyChanging();
					this._LeaderId = value;
					this.SendPropertyChanged("LeaderId");
					this.OnLeaderIdChanged();
				}

			}

		}

		
		[Column(Name="LeaderName", UpdateCheck=UpdateCheck.Never, Storage="_LeaderName", DbType="nvarchar(50)")]
		public string LeaderName
		{
			get { return this._LeaderName; }

			set
			{
				if (this._LeaderName != value)
				{
				
                    this.OnLeaderNameChanging(value);
					this.SendPropertyChanging();
					this._LeaderName = value;
					this.SendPropertyChanged("LeaderName");
					this.OnLeaderNameChanged();
				}

			}

		}

		
		[Column(Name="ClassFilled", UpdateCheck=UpdateCheck.Never, Storage="_ClassFilled", DbType="bit")]
		public bool? ClassFilled
		{
			get { return this._ClassFilled; }

			set
			{
				if (this._ClassFilled != value)
				{
				
                    this.OnClassFilledChanging(value);
					this.SendPropertyChanging();
					this._ClassFilled = value;
					this.SendPropertyChanged("ClassFilled");
					this.OnClassFilledChanged();
				}

			}

		}

		
		[Column(Name="OnLineCatalogSort", UpdateCheck=UpdateCheck.Never, Storage="_OnLineCatalogSort", DbType="int")]
		public int? OnLineCatalogSort
		{
			get { return this._OnLineCatalogSort; }

			set
			{
				if (this._OnLineCatalogSort != value)
				{
				
                    this.OnOnLineCatalogSortChanging(value);
					this.SendPropertyChanging();
					this._OnLineCatalogSort = value;
					this.SendPropertyChanged("OnLineCatalogSort");
					this.OnOnLineCatalogSortChanged();
				}

			}

		}

		
		[Column(Name="PendingLoc", UpdateCheck=UpdateCheck.Never, Storage="_PendingLoc", DbType="nvarchar(40)")]
		public string PendingLoc
		{
			get { return this._PendingLoc; }

			set
			{
				if (this._PendingLoc != value)
				{
				
                    this.OnPendingLocChanging(value);
					this.SendPropertyChanging();
					this._PendingLoc = value;
					this.SendPropertyChanged("PendingLoc");
					this.OnPendingLocChanged();
				}

			}

		}

		
		[Column(Name="CanSelfCheckin", UpdateCheck=UpdateCheck.Never, Storage="_CanSelfCheckin", DbType="bit")]
		public bool? CanSelfCheckin
		{
			get { return this._CanSelfCheckin; }

			set
			{
				if (this._CanSelfCheckin != value)
				{
				
                    this.OnCanSelfCheckinChanging(value);
					this.SendPropertyChanging();
					this._CanSelfCheckin = value;
					this.SendPropertyChanged("CanSelfCheckin");
					this.OnCanSelfCheckinChanged();
				}

			}

		}

		
		[Column(Name="NumCheckInLabels", UpdateCheck=UpdateCheck.Never, Storage="_NumCheckInLabels", DbType="int")]
		public int? NumCheckInLabels
		{
			get { return this._NumCheckInLabels; }

			set
			{
				if (this._NumCheckInLabels != value)
				{
				
                    this.OnNumCheckInLabelsChanging(value);
					this.SendPropertyChanging();
					this._NumCheckInLabels = value;
					this.SendPropertyChanged("NumCheckInLabels");
					this.OnNumCheckInLabelsChanged();
				}

			}

		}

		
		[Column(Name="CampusId", UpdateCheck=UpdateCheck.Never, Storage="_CampusId", DbType="int")]
		[IsForeignKey]
		public int? CampusId
		{
			get { return this._CampusId; }

			set
			{
				if (this._CampusId != value)
				{
				
					if (this._Campu.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnCampusIdChanging(value);
					this.SendPropertyChanging();
					this._CampusId = value;
					this.SendPropertyChanged("CampusId");
					this.OnCampusIdChanged();
				}

			}

		}

		
		[Column(Name="AllowNonCampusCheckIn", UpdateCheck=UpdateCheck.Never, Storage="_AllowNonCampusCheckIn", DbType="bit")]
		public bool? AllowNonCampusCheckIn
		{
			get { return this._AllowNonCampusCheckIn; }

			set
			{
				if (this._AllowNonCampusCheckIn != value)
				{
				
                    this.OnAllowNonCampusCheckInChanging(value);
					this.SendPropertyChanging();
					this._AllowNonCampusCheckIn = value;
					this.SendPropertyChanged("AllowNonCampusCheckIn");
					this.OnAllowNonCampusCheckInChanged();
				}

			}

		}

		
		[Column(Name="NumWorkerCheckInLabels", UpdateCheck=UpdateCheck.Never, Storage="_NumWorkerCheckInLabels", DbType="int")]
		public int? NumWorkerCheckInLabels
		{
			get { return this._NumWorkerCheckInLabels; }

			set
			{
				if (this._NumWorkerCheckInLabels != value)
				{
				
                    this.OnNumWorkerCheckInLabelsChanging(value);
					this.SendPropertyChanging();
					this._NumWorkerCheckInLabels = value;
					this.SendPropertyChanged("NumWorkerCheckInLabels");
					this.OnNumWorkerCheckInLabelsChanged();
				}

			}

		}

		
		[Column(Name="ShowOnlyRegisteredAtCheckIn", UpdateCheck=UpdateCheck.Never, Storage="_ShowOnlyRegisteredAtCheckIn", DbType="bit")]
		public bool? ShowOnlyRegisteredAtCheckIn
		{
			get { return this._ShowOnlyRegisteredAtCheckIn; }

			set
			{
				if (this._ShowOnlyRegisteredAtCheckIn != value)
				{
				
                    this.OnShowOnlyRegisteredAtCheckInChanging(value);
					this.SendPropertyChanging();
					this._ShowOnlyRegisteredAtCheckIn = value;
					this.SendPropertyChanged("ShowOnlyRegisteredAtCheckIn");
					this.OnShowOnlyRegisteredAtCheckInChanged();
				}

			}

		}

		
		[Column(Name="Limit", UpdateCheck=UpdateCheck.Never, Storage="_Limit", DbType="int")]
		public int? Limit
		{
			get { return this._Limit; }

			set
			{
				if (this._Limit != value)
				{
				
                    this.OnLimitChanging(value);
					this.SendPropertyChanging();
					this._Limit = value;
					this.SendPropertyChanged("Limit");
					this.OnLimitChanged();
				}

			}

		}

		
		[Column(Name="GenderId", UpdateCheck=UpdateCheck.Never, Storage="_GenderId", DbType="int")]
		[IsForeignKey]
		public int? GenderId
		{
			get { return this._GenderId; }

			set
			{
				if (this._GenderId != value)
				{
				
					if (this._Gender.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnGenderIdChanging(value);
					this.SendPropertyChanging();
					this._GenderId = value;
					this.SendPropertyChanged("GenderId");
					this.OnGenderIdChanged();
				}

			}

		}

		
		[Column(Name="Description", UpdateCheck=UpdateCheck.Never, Storage="_Description", DbType="nvarchar")]
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

		
		[Column(Name="BirthDayStart", UpdateCheck=UpdateCheck.Never, Storage="_BirthDayStart", DbType="datetime")]
		public DateTime? BirthDayStart
		{
			get { return this._BirthDayStart; }

			set
			{
				if (this._BirthDayStart != value)
				{
				
                    this.OnBirthDayStartChanging(value);
					this.SendPropertyChanging();
					this._BirthDayStart = value;
					this.SendPropertyChanged("BirthDayStart");
					this.OnBirthDayStartChanged();
				}

			}

		}

		
		[Column(Name="BirthDayEnd", UpdateCheck=UpdateCheck.Never, Storage="_BirthDayEnd", DbType="datetime")]
		public DateTime? BirthDayEnd
		{
			get { return this._BirthDayEnd; }

			set
			{
				if (this._BirthDayEnd != value)
				{
				
                    this.OnBirthDayEndChanging(value);
					this.SendPropertyChanging();
					this._BirthDayEnd = value;
					this.SendPropertyChanged("BirthDayEnd");
					this.OnBirthDayEndChanged();
				}

			}

		}

		
		[Column(Name="LastDayBeforeExtra", UpdateCheck=UpdateCheck.Never, Storage="_LastDayBeforeExtra", DbType="datetime")]
		public DateTime? LastDayBeforeExtra
		{
			get { return this._LastDayBeforeExtra; }

			set
			{
				if (this._LastDayBeforeExtra != value)
				{
				
                    this.OnLastDayBeforeExtraChanging(value);
					this.SendPropertyChanging();
					this._LastDayBeforeExtra = value;
					this.SendPropertyChanged("LastDayBeforeExtra");
					this.OnLastDayBeforeExtraChanged();
				}

			}

		}

		
		[Column(Name="RegistrationTypeId", UpdateCheck=UpdateCheck.Never, Storage="_RegistrationTypeId", DbType="int")]
		public int? RegistrationTypeId
		{
			get { return this._RegistrationTypeId; }

			set
			{
				if (this._RegistrationTypeId != value)
				{
				
                    this.OnRegistrationTypeIdChanging(value);
					this.SendPropertyChanging();
					this._RegistrationTypeId = value;
					this.SendPropertyChanged("RegistrationTypeId");
					this.OnRegistrationTypeIdChanged();
				}

			}

		}

		
		[Column(Name="ValidateOrgs", UpdateCheck=UpdateCheck.Never, Storage="_ValidateOrgs", DbType="nvarchar(60)")]
		public string ValidateOrgs
		{
			get { return this._ValidateOrgs; }

			set
			{
				if (this._ValidateOrgs != value)
				{
				
                    this.OnValidateOrgsChanging(value);
					this.SendPropertyChanging();
					this._ValidateOrgs = value;
					this.SendPropertyChanged("ValidateOrgs");
					this.OnValidateOrgsChanged();
				}

			}

		}

		
		[Column(Name="PhoneNumber", UpdateCheck=UpdateCheck.Never, Storage="_PhoneNumber", DbType="nvarchar(25)")]
		public string PhoneNumber
		{
			get { return this._PhoneNumber; }

			set
			{
				if (this._PhoneNumber != value)
				{
				
                    this.OnPhoneNumberChanging(value);
					this.SendPropertyChanging();
					this._PhoneNumber = value;
					this.SendPropertyChanged("PhoneNumber");
					this.OnPhoneNumberChanged();
				}

			}

		}

		
		[Column(Name="RegistrationClosed", UpdateCheck=UpdateCheck.Never, Storage="_RegistrationClosed", DbType="bit")]
		public bool? RegistrationClosed
		{
			get { return this._RegistrationClosed; }

			set
			{
				if (this._RegistrationClosed != value)
				{
				
                    this.OnRegistrationClosedChanging(value);
					this.SendPropertyChanging();
					this._RegistrationClosed = value;
					this.SendPropertyChanged("RegistrationClosed");
					this.OnRegistrationClosedChanged();
				}

			}

		}

		
		[Column(Name="AllowKioskRegister", UpdateCheck=UpdateCheck.Never, Storage="_AllowKioskRegister", DbType="bit")]
		public bool? AllowKioskRegister
		{
			get { return this._AllowKioskRegister; }

			set
			{
				if (this._AllowKioskRegister != value)
				{
				
                    this.OnAllowKioskRegisterChanging(value);
					this.SendPropertyChanging();
					this._AllowKioskRegister = value;
					this.SendPropertyChanged("AllowKioskRegister");
					this.OnAllowKioskRegisterChanged();
				}

			}

		}

		
		[Column(Name="WorshipGroupCodes", UpdateCheck=UpdateCheck.Never, Storage="_WorshipGroupCodes", DbType="nvarchar(100)")]
		public string WorshipGroupCodes
		{
			get { return this._WorshipGroupCodes; }

			set
			{
				if (this._WorshipGroupCodes != value)
				{
				
                    this.OnWorshipGroupCodesChanging(value);
					this.SendPropertyChanging();
					this._WorshipGroupCodes = value;
					this.SendPropertyChanged("WorshipGroupCodes");
					this.OnWorshipGroupCodesChanged();
				}

			}

		}

		
		[Column(Name="IsBibleFellowshipOrg", UpdateCheck=UpdateCheck.Never, Storage="_IsBibleFellowshipOrg", DbType="bit")]
		public bool? IsBibleFellowshipOrg
		{
			get { return this._IsBibleFellowshipOrg; }

			set
			{
				if (this._IsBibleFellowshipOrg != value)
				{
				
                    this.OnIsBibleFellowshipOrgChanging(value);
					this.SendPropertyChanging();
					this._IsBibleFellowshipOrg = value;
					this.SendPropertyChanged("IsBibleFellowshipOrg");
					this.OnIsBibleFellowshipOrgChanged();
				}

			}

		}

		
		[Column(Name="NoSecurityLabel", UpdateCheck=UpdateCheck.Never, Storage="_NoSecurityLabel", DbType="bit")]
		public bool? NoSecurityLabel
		{
			get { return this._NoSecurityLabel; }

			set
			{
				if (this._NoSecurityLabel != value)
				{
				
                    this.OnNoSecurityLabelChanging(value);
					this.SendPropertyChanging();
					this._NoSecurityLabel = value;
					this.SendPropertyChanged("NoSecurityLabel");
					this.OnNoSecurityLabelChanged();
				}

			}

		}

		
		[Column(Name="AlwaysSecurityLabel", UpdateCheck=UpdateCheck.Never, Storage="_AlwaysSecurityLabel", DbType="bit")]
		public bool? AlwaysSecurityLabel
		{
			get { return this._AlwaysSecurityLabel; }

			set
			{
				if (this._AlwaysSecurityLabel != value)
				{
				
                    this.OnAlwaysSecurityLabelChanging(value);
					this.SendPropertyChanging();
					this._AlwaysSecurityLabel = value;
					this.SendPropertyChanged("AlwaysSecurityLabel");
					this.OnAlwaysSecurityLabelChanged();
				}

			}

		}

		
		[Column(Name="DaysToIgnoreHistory", UpdateCheck=UpdateCheck.Never, Storage="_DaysToIgnoreHistory", DbType="int")]
		public int? DaysToIgnoreHistory
		{
			get { return this._DaysToIgnoreHistory; }

			set
			{
				if (this._DaysToIgnoreHistory != value)
				{
				
                    this.OnDaysToIgnoreHistoryChanging(value);
					this.SendPropertyChanging();
					this._DaysToIgnoreHistory = value;
					this.SendPropertyChanged("DaysToIgnoreHistory");
					this.OnDaysToIgnoreHistoryChanged();
				}

			}

		}

		
		[Column(Name="NotifyIds", UpdateCheck=UpdateCheck.Never, Storage="_NotifyIds", DbType="varchar(50)")]
		public string NotifyIds
		{
			get { return this._NotifyIds; }

			set
			{
				if (this._NotifyIds != value)
				{
				
                    this.OnNotifyIdsChanging(value);
					this.SendPropertyChanging();
					this._NotifyIds = value;
					this.SendPropertyChanged("NotifyIds");
					this.OnNotifyIdsChanged();
				}

			}

		}

		
		[Column(Name="lat", UpdateCheck=UpdateCheck.Never, Storage="_Lat", DbType="float")]
		public double? Lat
		{
			get { return this._Lat; }

			set
			{
				if (this._Lat != value)
				{
				
                    this.OnLatChanging(value);
					this.SendPropertyChanging();
					this._Lat = value;
					this.SendPropertyChanged("Lat");
					this.OnLatChanged();
				}

			}

		}

		
		[Column(Name="long", UpdateCheck=UpdateCheck.Never, Storage="_LongX", DbType="float")]
		public double? LongX
		{
			get { return this._LongX; }

			set
			{
				if (this._LongX != value)
				{
				
                    this.OnLongXChanging(value);
					this.SendPropertyChanging();
					this._LongX = value;
					this.SendPropertyChanged("LongX");
					this.OnLongXChanged();
				}

			}

		}

		
		[Column(Name="RegSetting", UpdateCheck=UpdateCheck.Never, Storage="_RegSetting", DbType="nvarchar")]
		public string RegSetting
		{
			get { return this._RegSetting; }

			set
			{
				if (this._RegSetting != value)
				{
				
                    this.OnRegSettingChanging(value);
					this.SendPropertyChanging();
					this._RegSetting = value;
					this.SendPropertyChanged("RegSetting");
					this.OnRegSettingChanged();
				}

			}

		}

		
		[Column(Name="OrgPickList", UpdateCheck=UpdateCheck.Never, Storage="_OrgPickList", DbType="varchar")]
		public string OrgPickList
		{
			get { return this._OrgPickList; }

			set
			{
				if (this._OrgPickList != value)
				{
				
                    this.OnOrgPickListChanging(value);
					this.SendPropertyChanging();
					this._OrgPickList = value;
					this.SendPropertyChanged("OrgPickList");
					this.OnOrgPickListChanged();
				}

			}

		}

		
		[Column(Name="Offsite", UpdateCheck=UpdateCheck.Never, Storage="_Offsite", DbType="bit")]
		public bool? Offsite
		{
			get { return this._Offsite; }

			set
			{
				if (this._Offsite != value)
				{
				
                    this.OnOffsiteChanging(value);
					this.SendPropertyChanging();
					this._Offsite = value;
					this.SendPropertyChanged("Offsite");
					this.OnOffsiteChanged();
				}

			}

		}

		
		[Column(Name="RegStart", UpdateCheck=UpdateCheck.Never, Storage="_RegStart", DbType="datetime")]
		public DateTime? RegStart
		{
			get { return this._RegStart; }

			set
			{
				if (this._RegStart != value)
				{
				
                    this.OnRegStartChanging(value);
					this.SendPropertyChanging();
					this._RegStart = value;
					this.SendPropertyChanged("RegStart");
					this.OnRegStartChanged();
				}

			}

		}

		
		[Column(Name="RegEnd", UpdateCheck=UpdateCheck.Never, Storage="_RegEnd", DbType="datetime")]
		public DateTime? RegEnd
		{
			get { return this._RegEnd; }

			set
			{
				if (this._RegEnd != value)
				{
				
                    this.OnRegEndChanging(value);
					this.SendPropertyChanging();
					this._RegEnd = value;
					this.SendPropertyChanged("RegEnd");
					this.OnRegEndChanged();
				}

			}

		}

		
		[Column(Name="LimitToRole", UpdateCheck=UpdateCheck.Never, Storage="_LimitToRole", DbType="nvarchar(20)")]
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

		
		[Column(Name="OrganizationTypeId", UpdateCheck=UpdateCheck.Never, Storage="_OrganizationTypeId", DbType="int")]
		[IsForeignKey]
		public int? OrganizationTypeId
		{
			get { return this._OrganizationTypeId; }

			set
			{
				if (this._OrganizationTypeId != value)
				{
				
					if (this._OrganizationType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrganizationTypeIdChanging(value);
					this.SendPropertyChanging();
					this._OrganizationTypeId = value;
					this.SendPropertyChanged("OrganizationTypeId");
					this.OnOrganizationTypeIdChanged();
				}

			}

		}

		
		[Column(Name="MemberJoinScript", UpdateCheck=UpdateCheck.Never, Storage="_MemberJoinScript", DbType="nvarchar(50)")]
		public string MemberJoinScript
		{
			get { return this._MemberJoinScript; }

			set
			{
				if (this._MemberJoinScript != value)
				{
				
                    this.OnMemberJoinScriptChanging(value);
					this.SendPropertyChanging();
					this._MemberJoinScript = value;
					this.SendPropertyChanged("MemberJoinScript");
					this.OnMemberJoinScriptChanged();
				}

			}

		}

		
		[Column(Name="AddToSmallGroupScript", UpdateCheck=UpdateCheck.Never, Storage="_AddToSmallGroupScript", DbType="nvarchar(50)")]
		public string AddToSmallGroupScript
		{
			get { return this._AddToSmallGroupScript; }

			set
			{
				if (this._AddToSmallGroupScript != value)
				{
				
                    this.OnAddToSmallGroupScriptChanging(value);
					this.SendPropertyChanging();
					this._AddToSmallGroupScript = value;
					this.SendPropertyChanged("AddToSmallGroupScript");
					this.OnAddToSmallGroupScriptChanged();
				}

			}

		}

		
		[Column(Name="RemoveFromSmallGroupScript", UpdateCheck=UpdateCheck.Never, Storage="_RemoveFromSmallGroupScript", DbType="nvarchar(50)")]
		public string RemoveFromSmallGroupScript
		{
			get { return this._RemoveFromSmallGroupScript; }

			set
			{
				if (this._RemoveFromSmallGroupScript != value)
				{
				
                    this.OnRemoveFromSmallGroupScriptChanging(value);
					this.SendPropertyChanging();
					this._RemoveFromSmallGroupScript = value;
					this.SendPropertyChanged("RemoveFromSmallGroupScript");
					this.OnRemoveFromSmallGroupScriptChanged();
				}

			}

		}

		
		[Column(Name="SuspendCheckin", UpdateCheck=UpdateCheck.Never, Storage="_SuspendCheckin", DbType="bit")]
		public bool? SuspendCheckin
		{
			get { return this._SuspendCheckin; }

			set
			{
				if (this._SuspendCheckin != value)
				{
				
                    this.OnSuspendCheckinChanging(value);
					this.SendPropertyChanging();
					this._SuspendCheckin = value;
					this.SendPropertyChanged("SuspendCheckin");
					this.OnSuspendCheckinChanged();
				}

			}

		}

		
		[Column(Name="NoAutoAbsents", UpdateCheck=UpdateCheck.Never, Storage="_NoAutoAbsents", DbType="bit")]
		public bool? NoAutoAbsents
		{
			get { return this._NoAutoAbsents; }

			set
			{
				if (this._NoAutoAbsents != value)
				{
				
                    this.OnNoAutoAbsentsChanging(value);
					this.SendPropertyChanging();
					this._NoAutoAbsents = value;
					this.SendPropertyChanged("NoAutoAbsents");
					this.OnNoAutoAbsentsChanged();
				}

			}

		}

		
		[Column(Name="PublishDirectory", UpdateCheck=UpdateCheck.Never, Storage="_PublishDirectory", DbType="int")]
		public int? PublishDirectory
		{
			get { return this._PublishDirectory; }

			set
			{
				if (this._PublishDirectory != value)
				{
				
                    this.OnPublishDirectoryChanging(value);
					this.SendPropertyChanging();
					this._PublishDirectory = value;
					this.SendPropertyChanged("PublishDirectory");
					this.OnPublishDirectoryChanged();
				}

			}

		}

		
		[Column(Name="ConsecutiveAbsentsThreshold", UpdateCheck=UpdateCheck.Never, Storage="_ConsecutiveAbsentsThreshold", DbType="int")]
		public int? ConsecutiveAbsentsThreshold
		{
			get { return this._ConsecutiveAbsentsThreshold; }

			set
			{
				if (this._ConsecutiveAbsentsThreshold != value)
				{
				
                    this.OnConsecutiveAbsentsThresholdChanging(value);
					this.SendPropertyChanging();
					this._ConsecutiveAbsentsThreshold = value;
					this.SendPropertyChanged("ConsecutiveAbsentsThreshold");
					this.OnConsecutiveAbsentsThresholdChanged();
				}

			}

		}

		
		[Column(Name="IsRecreationTeam", UpdateCheck=UpdateCheck.Never, Storage="_IsRecreationTeam", DbType="bit NOT NULL")]
		public bool IsRecreationTeam
		{
			get { return this._IsRecreationTeam; }

			set
			{
				if (this._IsRecreationTeam != value)
				{
				
                    this.OnIsRecreationTeamChanging(value);
					this.SendPropertyChanging();
					this._IsRecreationTeam = value;
					this.SendPropertyChanged("IsRecreationTeam");
					this.OnIsRecreationTeamChanged();
				}

			}

		}

		
		[Column(Name="NotWeekly", UpdateCheck=UpdateCheck.Never, Storage="_NotWeekly", DbType="bit")]
		public bool? NotWeekly
		{
			get { return this._NotWeekly; }

			set
			{
				if (this._NotWeekly != value)
				{
				
                    this.OnNotWeeklyChanging(value);
					this.SendPropertyChanging();
					this._NotWeekly = value;
					this.SendPropertyChanged("NotWeekly");
					this.OnNotWeeklyChanged();
				}

			}

		}

		
		[Column(Name="IsMissionTrip", UpdateCheck=UpdateCheck.Never, Storage="_IsMissionTrip", DbType="bit")]
		public bool? IsMissionTrip
		{
			get { return this._IsMissionTrip; }

			set
			{
				if (this._IsMissionTrip != value)
				{
				
                    this.OnIsMissionTripChanging(value);
					this.SendPropertyChanging();
					this._IsMissionTrip = value;
					this.SendPropertyChanged("IsMissionTrip");
					this.OnIsMissionTripChanged();
				}

			}

		}

		
		[Column(Name="NoCreditCards", UpdateCheck=UpdateCheck.Never, Storage="_NoCreditCards", DbType="bit")]
		public bool? NoCreditCards
		{
			get { return this._NoCreditCards; }

			set
			{
				if (this._NoCreditCards != value)
				{
				
                    this.OnNoCreditCardsChanging(value);
					this.SendPropertyChanging();
					this._NoCreditCards = value;
					this.SendPropertyChanged("NoCreditCards");
					this.OnNoCreditCardsChanged();
				}

			}

		}

		
		[Column(Name="GiftNotifyIds", UpdateCheck=UpdateCheck.Never, Storage="_GiftNotifyIds", DbType="varchar(50)")]
		public string GiftNotifyIds
		{
			get { return this._GiftNotifyIds; }

			set
			{
				if (this._GiftNotifyIds != value)
				{
				
                    this.OnGiftNotifyIdsChanging(value);
					this.SendPropertyChanging();
					this._GiftNotifyIds = value;
					this.SendPropertyChanged("GiftNotifyIds");
					this.OnGiftNotifyIdsChanged();
				}

			}

		}

		
		[Column(Name="VisitorDate", UpdateCheck=UpdateCheck.Never, Storage="_VisitorDate", DbType="datetime", IsDbGenerated=true)]
		public DateTime? VisitorDate
		{
			get { return this._VisitorDate; }

			set
			{
				if (this._VisitorDate != value)
				{
				
                    this.OnVisitorDateChanging(value);
					this.SendPropertyChanging();
					this._VisitorDate = value;
					this.SendPropertyChanged("VisitorDate");
					this.OnVisitorDateChanged();
				}

			}

		}

		
		[Column(Name="UseBootstrap", UpdateCheck=UpdateCheck.Never, Storage="_UseBootstrap", DbType="bit")]
		public bool? UseBootstrap
		{
			get { return this._UseBootstrap; }

			set
			{
				if (this._UseBootstrap != value)
				{
				
                    this.OnUseBootstrapChanging(value);
					this.SendPropertyChanging();
					this._UseBootstrap = value;
					this.SendPropertyChanged("UseBootstrap");
					this.OnUseBootstrapChanged();
				}

			}

		}

		
		[Column(Name="PublicSortOrder", UpdateCheck=UpdateCheck.Never, Storage="_PublicSortOrder", DbType="varchar(15)")]
		public string PublicSortOrder
		{
			get { return this._PublicSortOrder; }

			set
			{
				if (this._PublicSortOrder != value)
				{
				
                    this.OnPublicSortOrderChanging(value);
					this.SendPropertyChanging();
					this._PublicSortOrder = value;
					this.SendPropertyChanged("PublicSortOrder");
					this.OnPublicSortOrderChanged();
				}

			}

		}

		
		[Column(Name="UseRegisterLink2", UpdateCheck=UpdateCheck.Never, Storage="_UseRegisterLink2", DbType="bit")]
		public bool? UseRegisterLink2
		{
			get { return this._UseRegisterLink2; }

			set
			{
				if (this._UseRegisterLink2 != value)
				{
				
                    this.OnUseRegisterLink2Changing(value);
					this.SendPropertyChanging();
					this._UseRegisterLink2 = value;
					this.SendPropertyChanged("UseRegisterLink2");
					this.OnUseRegisterLink2Changed();
				}

			}

		}

		
		[Column(Name="AppCategory", UpdateCheck=UpdateCheck.Never, Storage="_AppCategory", DbType="varchar(15)")]
		public string AppCategory
		{
			get { return this._AppCategory; }

			set
			{
				if (this._AppCategory != value)
				{
				
                    this.OnAppCategoryChanging(value);
					this.SendPropertyChanging();
					this._AppCategory = value;
					this.SendPropertyChanged("AppCategory");
					this.OnAppCategoryChanged();
				}

			}

		}

		
		[Column(Name="RegistrationTitle", UpdateCheck=UpdateCheck.Never, Storage="_RegistrationTitle", DbType="nvarchar(200)")]
		public string RegistrationTitle
		{
			get { return this._RegistrationTitle; }

			set
			{
				if (this._RegistrationTitle != value)
				{
				
                    this.OnRegistrationTitleChanging(value);
					this.SendPropertyChanging();
					this._RegistrationTitle = value;
					this.SendPropertyChanged("RegistrationTitle");
					this.OnRegistrationTitleChanged();
				}

			}

		}

		
		[Column(Name="PrevMemberCount", UpdateCheck=UpdateCheck.Never, Storage="_PrevMemberCount", DbType="int")]
		public int? PrevMemberCount
		{
			get { return this._PrevMemberCount; }

			set
			{
				if (this._PrevMemberCount != value)
				{
				
                    this.OnPrevMemberCountChanging(value);
					this.SendPropertyChanging();
					this._PrevMemberCount = value;
					this.SendPropertyChanged("PrevMemberCount");
					this.OnPrevMemberCountChanged();
				}

			}

		}

		
		[Column(Name="ProspectCount", UpdateCheck=UpdateCheck.Never, Storage="_ProspectCount", DbType="int")]
		public int? ProspectCount
		{
			get { return this._ProspectCount; }

			set
			{
				if (this._ProspectCount != value)
				{
				
                    this.OnProspectCountChanging(value);
					this.SendPropertyChanging();
					this._ProspectCount = value;
					this.SendPropertyChanged("ProspectCount");
					this.OnProspectCountChanged();
				}

			}

		}

		
		[Column(Name="RegSettingXml", UpdateCheck=UpdateCheck.Never, Storage="_RegSettingXml", DbType="xml")]
		public string RegSettingXml
		{
			get { return this._RegSettingXml; }

			set
			{
				if (this._RegSettingXml != value)
				{
				
                    this.OnRegSettingXmlChanging(value);
					this.SendPropertyChanging();
					this._RegSettingXml = value;
					this.SendPropertyChanged("RegSettingXml");
					this.OnRegSettingXmlChanged();
				}

			}

		}

		
		[Column(Name="AttendanceBySubGroups", UpdateCheck=UpdateCheck.Never, Storage="_AttendanceBySubGroups", DbType="bit")]
		public bool? AttendanceBySubGroups
		{
			get { return this._AttendanceBySubGroups; }

			set
			{
				if (this._AttendanceBySubGroups != value)
				{
				
                    this.OnAttendanceBySubGroupsChanging(value);
					this.SendPropertyChanging();
					this._AttendanceBySubGroups = value;
					this.SendPropertyChanged("AttendanceBySubGroups");
					this.OnAttendanceBySubGroupsChanged();
				}

			}

		}

		
		[Column(Name="SendAttendanceLink", UpdateCheck=UpdateCheck.Never, Storage="_SendAttendanceLink", DbType="bit NOT NULL")]
		public bool SendAttendanceLink
		{
			get { return this._SendAttendanceLink; }

			set
			{
				if (this._SendAttendanceLink != value)
				{
				
                    this.OnSendAttendanceLinkChanging(value);
					this.SendPropertyChanging();
					this._SendAttendanceLink = value;
					this.SendPropertyChanged("SendAttendanceLink");
					this.OnSendAttendanceLinkChanged();
				}

			}

		}

		
		[Column(Name="TripFundingPagesEnable", UpdateCheck=UpdateCheck.Never, Storage="_TripFundingPagesEnable", DbType="bit NOT NULL")]
		public bool TripFundingPagesEnable
		{
			get { return this._TripFundingPagesEnable; }

			set
			{
				if (this._TripFundingPagesEnable != value)
				{
				
                    this.OnTripFundingPagesEnableChanging(value);
					this.SendPropertyChanging();
					this._TripFundingPagesEnable = value;
					this.SendPropertyChanged("TripFundingPagesEnable");
					this.OnTripFundingPagesEnableChanged();
				}

			}

		}

		
		[Column(Name="TripFundingPagesPublic", UpdateCheck=UpdateCheck.Never, Storage="_TripFundingPagesPublic", DbType="bit NOT NULL")]
		public bool TripFundingPagesPublic
		{
			get { return this._TripFundingPagesPublic; }

			set
			{
				if (this._TripFundingPagesPublic != value)
				{
				
                    this.OnTripFundingPagesPublicChanging(value);
					this.SendPropertyChanging();
					this._TripFundingPagesPublic = value;
					this.SendPropertyChanged("TripFundingPagesPublic");
					this.OnTripFundingPagesPublicChanged();
				}

			}

		}

		
		[Column(Name="TripFundingPagesShowAmounts", UpdateCheck=UpdateCheck.Never, Storage="_TripFundingPagesShowAmounts", DbType="bit NOT NULL")]
		public bool TripFundingPagesShowAmounts
		{
			get { return this._TripFundingPagesShowAmounts; }

			set
			{
				if (this._TripFundingPagesShowAmounts != value)
				{
				
                    this.OnTripFundingPagesShowAmountsChanging(value);
					this.SendPropertyChanging();
					this._TripFundingPagesShowAmounts = value;
					this.SendPropertyChanged("TripFundingPagesShowAmounts");
					this.OnTripFundingPagesShowAmountsChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="BFMembers__BFClass", Storage="_BFMembers", OtherKey="BibleFellowshipClassId")]
   		public EntitySet<Person> BFMembers
   		{
   		    get { return this._BFMembers; }

			set	{ this._BFMembers.Assign(value); }

   		}

		
   		[Association(Name="ChildOrgs__ParentOrg", Storage="_ChildOrgs", OtherKey="ParentOrgId")]
   		public EntitySet<Organization> ChildOrgs
   		{
   		    get { return this._ChildOrgs; }

			set	{ this._ChildOrgs.Assign(value); }

   		}

		
   		[Association(Name="contactsHad__organization", Storage="_contactsHad", OtherKey="OrganizationId")]
   		public EntitySet<Contact> contactsHad
   		{
   		    get { return this._contactsHad; }

			set	{ this._contactsHad.Assign(value); }

   		}

		
   		[Association(Name="ENROLLMENT_TRANSACTION_ORG_FK", Storage="_EnrollmentTransactions", OtherKey="OrganizationId")]
   		public EntitySet<EnrollmentTransaction> EnrollmentTransactions
   		{
   		    get { return this._EnrollmentTransactions; }

			set	{ this._EnrollmentTransactions.Assign(value); }

   		}

		
   		[Association(Name="FK_AttendWithAbsents_TBL_ORGANIZATIONS_TBL", Storage="_Attends", OtherKey="OrganizationId")]
   		public EntitySet<Attend> Attends
   		{
   		    get { return this._Attends; }

			set	{ this._Attends.Assign(value); }

   		}

		
   		[Association(Name="FK_Coupons_Organizations", Storage="_Coupons", OtherKey="OrgId")]
   		public EntitySet<Coupon> Coupons
   		{
   		    get { return this._Coupons; }

			set	{ this._Coupons.Assign(value); }

   		}

		
   		[Association(Name="FK_DivOrg_Organizations", Storage="_DivOrgs", OtherKey="OrgId")]
   		public EntitySet<DivOrg> DivOrgs
   		{
   		    get { return this._DivOrgs; }

			set	{ this._DivOrgs.Assign(value); }

   		}

		
   		[Association(Name="FK_GoerSenderAmounts_Organizations", Storage="_GoerSenderAmounts", OtherKey="OrgId")]
   		public EntitySet<GoerSenderAmount> GoerSenderAmounts
   		{
   		    get { return this._GoerSenderAmounts; }

			set	{ this._GoerSenderAmounts.Assign(value); }

   		}

		
   		[Association(Name="FK_MEETINGS_TBL_ORGANIZATIONS_TBL", Storage="_Meetings", OtherKey="OrganizationId")]
   		public EntitySet<Meeting> Meetings
   		{
   		    get { return this._Meetings; }

			set	{ this._Meetings.Assign(value); }

   		}

		
   		[Association(Name="FK_MemberTags_Organizations", Storage="_MemberTags", OtherKey="OrgId")]
   		public EntitySet<MemberTag> MemberTags
   		{
   		    get { return this._MemberTags; }

			set	{ this._MemberTags.Assign(value); }

   		}

		
   		[Association(Name="FK_OrganizationExtra_Organizations", Storage="_OrganizationExtras", OtherKey="OrganizationId")]
   		public EntitySet<OrganizationExtra> OrganizationExtras
   		{
   		    get { return this._OrganizationExtras; }

			set	{ this._OrganizationExtras.Assign(value); }

   		}

		
   		[Association(Name="FK_OrgMemberExtra_Organizations", Storage="_OrgMemberExtras", OtherKey="OrganizationId")]
   		public EntitySet<OrgMemberExtra> OrgMemberExtras
   		{
   		    get { return this._OrgMemberExtras; }

			set	{ this._OrgMemberExtras.Assign(value); }

   		}

		
   		[Association(Name="FK_OrgSchedule_Organizations", Storage="_OrgSchedules", OtherKey="OrganizationId")]
   		public EntitySet<OrgSchedule> OrgSchedules
   		{
   		    get { return this._OrgSchedules; }

			set	{ this._OrgSchedules.Assign(value); }

   		}

		
   		[Association(Name="FK_PrevOrgMemberExtra_Organization", Storage="_PrevOrgMemberExtras", OtherKey="OrganizationId")]
   		public EntitySet<PrevOrgMemberExtra> PrevOrgMemberExtras
   		{
   		    get { return this._PrevOrgMemberExtras; }

			set	{ this._PrevOrgMemberExtras.Assign(value); }

   		}

		
   		[Association(Name="FK_Resource_Organization", Storage="_Resources", OtherKey="OrganizationId")]
   		public EntitySet<Resource> Resources
   		{
   		    get { return this._Resources; }

			set	{ this._Resources.Assign(value); }

   		}

		
   		[Association(Name="FK_ResourceOrganization_Organizations", Storage="_ResourceOrganizations", OtherKey="OrganizationId")]
   		public EntitySet<ResourceOrganization> ResourceOrganizations
   		{
   		    get { return this._ResourceOrganizations; }

			set	{ this._ResourceOrganizations.Assign(value); }

   		}

		
   		[Association(Name="ORGANIZATION_MEMBERS_ORG_FK", Storage="_OrganizationMembers", OtherKey="OrganizationId")]
   		public EntitySet<OrganizationMember> OrganizationMembers
   		{
   		    get { return this._OrganizationMembers; }

			set	{ this._OrganizationMembers.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="ChildOrgs__ParentOrg", Storage="_ParentOrg", ThisKey="ParentOrgId", IsForeignKey=true)]
		public Organization ParentOrg
		{
			get { return this._ParentOrg.Entity; }

			set
			{
				Organization previousValue = this._ParentOrg.Entity;
				if (((previousValue != value) 
							|| (this._ParentOrg.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ParentOrg.Entity = null;
						previousValue.ChildOrgs.Remove(this);
					}

					this._ParentOrg.Entity = value;
					if (value != null)
					{
						value.ChildOrgs.Add(this);
						
						this._ParentOrgId = value.OrganizationId;
						
					}

					else
					{
						
						this._ParentOrgId = default(int?);
						
					}

					this.SendPropertyChanged("ParentOrg");
				}

			}

		}

		
		[Association(Name="FK_Organizations_Campus", Storage="_Campu", ThisKey="CampusId", IsForeignKey=true)]
		public Campu Campu
		{
			get { return this._Campu.Entity; }

			set
			{
				Campu previousValue = this._Campu.Entity;
				if (((previousValue != value) 
							|| (this._Campu.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Campu.Entity = null;
						previousValue.Organizations.Remove(this);
					}

					this._Campu.Entity = value;
					if (value != null)
					{
						value.Organizations.Add(this);
						
						this._CampusId = value.Id;
						
					}

					else
					{
						
						this._CampusId = default(int?);
						
					}

					this.SendPropertyChanged("Campu");
				}

			}

		}

		
		[Association(Name="FK_Organizations_Division", Storage="_Division", ThisKey="DivisionId", IsForeignKey=true)]
		public Division Division
		{
			get { return this._Division.Entity; }

			set
			{
				Division previousValue = this._Division.Entity;
				if (((previousValue != value) 
							|| (this._Division.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Division.Entity = null;
						previousValue.Organizations.Remove(this);
					}

					this._Division.Entity = value;
					if (value != null)
					{
						value.Organizations.Add(this);
						
						this._DivisionId = value.Id;
						
					}

					else
					{
						
						this._DivisionId = default(int?);
						
					}

					this.SendPropertyChanged("Division");
				}

			}

		}

		
		[Association(Name="FK_Organizations_Gender", Storage="_Gender", ThisKey="GenderId", IsForeignKey=true)]
		public Gender Gender
		{
			get { return this._Gender.Entity; }

			set
			{
				Gender previousValue = this._Gender.Entity;
				if (((previousValue != value) 
							|| (this._Gender.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Gender.Entity = null;
						previousValue.Organizations.Remove(this);
					}

					this._Gender.Entity = value;
					if (value != null)
					{
						value.Organizations.Add(this);
						
						this._GenderId = value.Id;
						
					}

					else
					{
						
						this._GenderId = default(int?);
						
					}

					this.SendPropertyChanged("Gender");
				}

			}

		}

		
		[Association(Name="FK_Organizations_OrganizationType", Storage="_OrganizationType", ThisKey="OrganizationTypeId", IsForeignKey=true)]
		public OrganizationType OrganizationType
		{
			get { return this._OrganizationType.Entity; }

			set
			{
				OrganizationType previousValue = this._OrganizationType.Entity;
				if (((previousValue != value) 
							|| (this._OrganizationType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._OrganizationType.Entity = null;
						previousValue.Organizations.Remove(this);
					}

					this._OrganizationType.Entity = value;
					if (value != null)
					{
						value.Organizations.Add(this);
						
						this._OrganizationTypeId = value.Id;
						
					}

					else
					{
						
						this._OrganizationTypeId = default(int?);
						
					}

					this.SendPropertyChanged("OrganizationType");
				}

			}

		}

		
		[Association(Name="FK_ORGANIZATIONS_TBL_EntryPoint", Storage="_EntryPoint", ThisKey="EntryPointId", IsForeignKey=true)]
		public EntryPoint EntryPoint
		{
			get { return this._EntryPoint.Entity; }

			set
			{
				EntryPoint previousValue = this._EntryPoint.Entity;
				if (((previousValue != value) 
							|| (this._EntryPoint.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._EntryPoint.Entity = null;
						previousValue.Organizations.Remove(this);
					}

					this._EntryPoint.Entity = value;
					if (value != null)
					{
						value.Organizations.Add(this);
						
						this._EntryPointId = value.Id;
						
					}

					else
					{
						
						this._EntryPointId = default(int?);
						
					}

					this.SendPropertyChanged("EntryPoint");
				}

			}

		}

		
		[Association(Name="FK_ORGANIZATIONS_TBL_OrganizationStatus", Storage="_OrganizationStatus", ThisKey="OrganizationStatusId", IsForeignKey=true)]
		public OrganizationStatus OrganizationStatus
		{
			get { return this._OrganizationStatus.Entity; }

			set
			{
				OrganizationStatus previousValue = this._OrganizationStatus.Entity;
				if (((previousValue != value) 
							|| (this._OrganizationStatus.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._OrganizationStatus.Entity = null;
						previousValue.Organizations.Remove(this);
					}

					this._OrganizationStatus.Entity = value;
					if (value != null)
					{
						value.Organizations.Add(this);
						
						this._OrganizationStatusId = value.Id;
						
					}

					else
					{
						
						this._OrganizationStatusId = default(int);
						
					}

					this.SendPropertyChanged("OrganizationStatus");
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

   		
		private void attach_BFMembers(Person entity)
		{
			this.SendPropertyChanging();
			entity.BFClass = this;
		}

		private void detach_BFMembers(Person entity)
		{
			this.SendPropertyChanging();
			entity.BFClass = null;
		}

		
		private void attach_ChildOrgs(Organization entity)
		{
			this.SendPropertyChanging();
			entity.ParentOrg = this;
		}

		private void detach_ChildOrgs(Organization entity)
		{
			this.SendPropertyChanging();
			entity.ParentOrg = null;
		}

		
		private void attach_contactsHad(Contact entity)
		{
			this.SendPropertyChanging();
			entity.organization = this;
		}

		private void detach_contactsHad(Contact entity)
		{
			this.SendPropertyChanging();
			entity.organization = null;
		}

		
		private void attach_EnrollmentTransactions(EnrollmentTransaction entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_EnrollmentTransactions(EnrollmentTransaction entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_Attends(Attend entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_Attends(Attend entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_Coupons(Coupon entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_Coupons(Coupon entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_DivOrgs(DivOrg entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_DivOrgs(DivOrg entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_GoerSenderAmounts(GoerSenderAmount entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_GoerSenderAmounts(GoerSenderAmount entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_Meetings(Meeting entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_Meetings(Meeting entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_MemberTags(MemberTag entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_MemberTags(MemberTag entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_OrganizationExtras(OrganizationExtra entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_OrganizationExtras(OrganizationExtra entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_OrgMemberExtras(OrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_OrgMemberExtras(OrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_OrgSchedules(OrgSchedule entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_OrgSchedules(OrgSchedule entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_Resources(Resource entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_Resources(Resource entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_ResourceOrganizations(ResourceOrganization entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_ResourceOrganizations(ResourceOrganization entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
		private void attach_OrganizationMembers(OrganizationMember entity)
		{
			this.SendPropertyChanging();
			entity.Organization = this;
		}

		private void detach_OrganizationMembers(OrganizationMember entity)
		{
			this.SendPropertyChanging();
			entity.Organization = null;
		}

		
	}

}

