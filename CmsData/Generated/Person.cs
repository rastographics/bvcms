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
	[Table(Name="dbo.People")]
	public partial class Person : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _PeopleId;
		
		private int _CreatedBy;
		
		private DateTime? _CreatedDate;
		
		private int _DropCodeId;
		
		private int _GenderId;
		
		private bool _DoNotMailFlag;
		
		private bool _DoNotCallFlag;
		
		private bool _DoNotVisitFlag;
		
		private int _AddressTypeId;
		
		private int _PhonePrefId;
		
		private int _MaritalStatusId;
		
		private int _PositionInFamilyId;
		
		private int _MemberStatusId;
		
		private int _FamilyId;
		
		private int? _BirthMonth;
		
		private int? _BirthDay;
		
		private int? _BirthYear;
		
		private int? _OriginId;
		
		private int? _EntryPointId;
		
		private int? _InterestPointId;
		
		private int? _BaptismTypeId;
		
		private int? _BaptismStatusId;
		
		private int? _DecisionTypeId;
		
		private int? _NewMemberClassStatusId;
		
		private int? _LetterStatusId;
		
		private int _JoinCodeId;
		
		private int? _EnvelopeOptionsId;
		
		private bool? _BadAddressFlag;
		
		private int? _ResCodeId;
		
		private DateTime? _AddressFromDate;
		
		private DateTime? _AddressToDate;
		
		private DateTime? _WeddingDate;
		
		private DateTime? _OriginDate;
		
		private DateTime? _BaptismSchedDate;
		
		private DateTime? _BaptismDate;
		
		private DateTime? _DecisionDate;
		
		private DateTime? _LetterDateRequested;
		
		private DateTime? _LetterDateReceived;
		
		private DateTime? _JoinDate;
		
		private DateTime? _DropDate;
		
		private DateTime? _DeceasedDate;
		
		private string _TitleCode;
		
		private string _FirstName;
		
		private string _MiddleName;
		
		private string _MaidenName;
		
		private string _LastName;
		
		private string _SuffixCode;
		
		private string _NickName;
		
		private string _AddressLineOne;
		
		private string _AddressLineTwo;
		
		private string _CityName;
		
		private string _StateCode;
		
		private string _ZipCode;
		
		private string _CountryName;
		
		private string _StreetName;
		
		private string _CellPhone;
		
		private string _WorkPhone;
		
		private string _EmailAddress;
		
		private string _OtherPreviousChurch;
		
		private string _OtherNewChurch;
		
		private string _SchoolOther;
		
		private string _EmployerOther;
		
		private string _OccupationOther;
		
		private string _HobbyOther;
		
		private string _SkillOther;
		
		private string _InterestOther;
		
		private string _LetterStatusNotes;
		
		private string _Comments;
		
		private bool _ChristAsSavior;
		
		private bool? _MemberAnyChurch;
		
		private bool _InterestedInJoining;
		
		private bool _PleaseVisit;
		
		private bool _InfoBecomeAChristian;
		
		private bool _ContributionsStatement;
		
		private int? _ModifiedBy;
		
		private DateTime? _ModifiedDate;
		
		private int? _PictureId;
		
		private int? _ContributionOptionsId;
		
		private string _PrimaryCity;
		
		private string _PrimaryZip;
		
		private string _PrimaryAddress;
		
		private string _PrimaryState;
		
		private string _HomePhone;
		
		private int? _SpouseId;
		
		private string _PrimaryAddress2;
		
		private int? _PrimaryResCode;
		
		private int? _PrimaryBadAddrFlag;
		
		private DateTime? _LastContact;
		
		private int? _Grade;
		
		private string _CellPhoneLU;
		
		private string _WorkPhoneLU;
		
		private int? _BibleFellowshipClassId;
		
		private int? _CampusId;
		
		private string _CellPhoneAC;
		
		private string _CheckInNotes;
		
		private int? _Age;
		
		private string _AltName;
		
		private bool? _CustodyIssue;
		
		private bool? _OkTransport;
		
		private DateTime? _BDate;
		
		private bool? _HasDuplicates;
		
		private string _FirstName2;
		
		private string _EmailAddress2;
		
		private bool? _SendEmailAddress1;
		
		private bool? _SendEmailAddress2;
		
		private DateTime? _NewMemberClassDate;
		
		private string _PrimaryCountry;
		
		private bool _ReceiveSMS;
		
		private bool? _DoNotPublishPhones;
		
		private bool? _IsDeceased;
		
		private string _Ssn;
		
		private string _Dln;
		
		private int? _DLStateID;
		
		private int? _HashNum;
		
		private string _PreferredName;
		
		private string _Name2;
		
		private string _Name;
		
		private bool? _ElectronicStatement;
		
   		
   		private EntitySet<Contactee> _contactsHad;
		
   		private EntitySet<Contactor> _contactsMade;
		
   		private EntitySet<EnrollmentTransaction> _EnrollmentTransactions;
		
   		private EntitySet<Family> _FamiliesHeaded;
		
   		private EntitySet<Family> _FamiliesHeaded2;
		
   		private EntitySet<Attend> _Attends;
		
   		private EntitySet<BackgroundCheck> _BackgroundChecks;
		
   		private EntitySet<CardIdentifier> _CardIdentifiers;
		
   		private EntitySet<CheckInTime> _CheckInTimes;
		
   		private EntitySet<Contribution> _Contributions;
		
   		private EntitySet<Coupon> _Coupons;
		
   		private EntitySet<EmailOptOut> _EmailOptOuts;
		
   		private EntitySet<EmailQueue> _EmailQueues;
		
   		private EntitySet<EmailQueueTo> _EmailQueueTos;
		
   		private EntitySet<EmailResponse> _EmailResponses;
		
   		private EntitySet<GoerSupporter> _FK_Goers;
		
   		private EntitySet<ManagedGiving> _ManagedGivings;
		
   		private EntitySet<MemberDocForm> _MemberDocForms;
		
   		private EntitySet<MobileAppDevice> _MobileAppDevices;
		
   		private EntitySet<MobileAppPushRegistration> _MobileAppPushRegistrations;
		
   		private EntitySet<OrgMemberExtra> _OrgMemberExtras;
		
   		private EntitySet<PaymentInfo> _PaymentInfos;
		
   		private EntitySet<PeopleExtra> _PeopleExtras;
		
   		private EntitySet<PrevOrgMemberExtra> _PrevOrgMemberExtras;
		
   		private EntitySet<RecReg> _RecRegs;
		
   		private EntitySet<RecurringAmount> _RecurringAmounts;
		
   		private EntitySet<SMSItem> _SMSItems;
		
   		private EntitySet<SMSList> _SMSLists;
		
   		private EntitySet<GoerSupporter> _FK_Supporters;
		
   		private EntitySet<TagShare> _TagShares;
		
   		private EntitySet<TaskListOwner> _TaskListOwners;
		
   		private EntitySet<Transaction> _Transactions;
		
   		private EntitySet<TransactionPerson> _TransactionPeople;
		
   		private EntitySet<User> _Users;
		
   		private EntitySet<VolInterestInterestCode> _VolInterestInterestCodes;
		
   		private EntitySet<Volunteer> _Volunteers;
		
   		private EntitySet<VolunteerForm> _VolunteerForms;
		
   		private EntitySet<VoluteerApprovalId> _VoluteerApprovalIds;
		
   		private EntitySet<GoerSenderAmount> _GoerAmounts;
		
   		private EntitySet<PeopleCanEmailFor> _OnBehalfOfPeople;
		
   		private EntitySet<OrganizationMember> _OrganizationMembers;
		
   		private EntitySet<BackgroundCheck> _People;
		
   		private EntitySet<PeopleCanEmailFor> _PersonsCanEmail;
		
   		private EntitySet<GoerSenderAmount> _SenderAmounts;
		
   		private EntitySet<SubRequest> _SubRequests;
		
   		private EntitySet<SubRequest> _SubResponses;
		
   		private EntitySet<TagPerson> _Tags;
		
   		private EntitySet<Tag> _TagsOwned;
		
   		private EntitySet<Task> _Tasks;
		
   		private EntitySet<Task> _TasksAboutPerson;
		
   		private EntitySet<Task> _TasksCoOwned;
		
   		private EntitySet<VolRequest> _VolRequests;
		
   		private EntitySet<VolRequest> _VolResponses;
		
    	
		private EntityRef<Organization> _BFClass;
		
		private EntityRef<EnvelopeOption> _EnvelopeOption;
		
		private EntityRef<BaptismStatus> _BaptismStatus;
		
		private EntityRef<BaptismType> _BaptismType;
		
		private EntityRef<Campu> _Campu;
		
		private EntityRef<DecisionType> _DecisionType;
		
		private EntityRef<NewMemberClassStatus> _NewMemberClassStatus;
		
		private EntityRef<DropType> _DropType;
		
		private EntityRef<EntryPoint> _EntryPoint;
		
		private EntityRef<Family> _Family;
		
		private EntityRef<FamilyPosition> _FamilyPosition;
		
		private EntityRef<Gender> _Gender;
		
		private EntityRef<InterestPoint> _InterestPoint;
		
		private EntityRef<JoinType> _JoinType;
		
		private EntityRef<MaritalStatus> _MaritalStatus;
		
		private EntityRef<MemberLetterStatus> _MemberLetterStatus;
		
		private EntityRef<MemberStatus> _MemberStatus;
		
		private EntityRef<Origin> _Origin;
		
		private EntityRef<Picture> _Picture;
		
		private EntityRef<ResidentCode> _ResidentCode;
		
		private EntityRef<EnvelopeOption> _ContributionStatementOption;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnCreatedByChanging(int value);
		partial void OnCreatedByChanged();
		
		partial void OnCreatedDateChanging(DateTime? value);
		partial void OnCreatedDateChanged();
		
		partial void OnDropCodeIdChanging(int value);
		partial void OnDropCodeIdChanged();
		
		partial void OnGenderIdChanging(int value);
		partial void OnGenderIdChanged();
		
		partial void OnDoNotMailFlagChanging(bool value);
		partial void OnDoNotMailFlagChanged();
		
		partial void OnDoNotCallFlagChanging(bool value);
		partial void OnDoNotCallFlagChanged();
		
		partial void OnDoNotVisitFlagChanging(bool value);
		partial void OnDoNotVisitFlagChanged();
		
		partial void OnAddressTypeIdChanging(int value);
		partial void OnAddressTypeIdChanged();
		
		partial void OnPhonePrefIdChanging(int value);
		partial void OnPhonePrefIdChanged();
		
		partial void OnMaritalStatusIdChanging(int value);
		partial void OnMaritalStatusIdChanged();
		
		partial void OnPositionInFamilyIdChanging(int value);
		partial void OnPositionInFamilyIdChanged();
		
		partial void OnMemberStatusIdChanging(int value);
		partial void OnMemberStatusIdChanged();
		
		partial void OnFamilyIdChanging(int value);
		partial void OnFamilyIdChanged();
		
		partial void OnBirthMonthChanging(int? value);
		partial void OnBirthMonthChanged();
		
		partial void OnBirthDayChanging(int? value);
		partial void OnBirthDayChanged();
		
		partial void OnBirthYearChanging(int? value);
		partial void OnBirthYearChanged();
		
		partial void OnOriginIdChanging(int? value);
		partial void OnOriginIdChanged();
		
		partial void OnEntryPointIdChanging(int? value);
		partial void OnEntryPointIdChanged();
		
		partial void OnInterestPointIdChanging(int? value);
		partial void OnInterestPointIdChanged();
		
		partial void OnBaptismTypeIdChanging(int? value);
		partial void OnBaptismTypeIdChanged();
		
		partial void OnBaptismStatusIdChanging(int? value);
		partial void OnBaptismStatusIdChanged();
		
		partial void OnDecisionTypeIdChanging(int? value);
		partial void OnDecisionTypeIdChanged();
		
		partial void OnNewMemberClassStatusIdChanging(int? value);
		partial void OnNewMemberClassStatusIdChanged();
		
		partial void OnLetterStatusIdChanging(int? value);
		partial void OnLetterStatusIdChanged();
		
		partial void OnJoinCodeIdChanging(int value);
		partial void OnJoinCodeIdChanged();
		
		partial void OnEnvelopeOptionsIdChanging(int? value);
		partial void OnEnvelopeOptionsIdChanged();
		
		partial void OnBadAddressFlagChanging(bool? value);
		partial void OnBadAddressFlagChanged();
		
		partial void OnResCodeIdChanging(int? value);
		partial void OnResCodeIdChanged();
		
		partial void OnAddressFromDateChanging(DateTime? value);
		partial void OnAddressFromDateChanged();
		
		partial void OnAddressToDateChanging(DateTime? value);
		partial void OnAddressToDateChanged();
		
		partial void OnWeddingDateChanging(DateTime? value);
		partial void OnWeddingDateChanged();
		
		partial void OnOriginDateChanging(DateTime? value);
		partial void OnOriginDateChanged();
		
		partial void OnBaptismSchedDateChanging(DateTime? value);
		partial void OnBaptismSchedDateChanged();
		
		partial void OnBaptismDateChanging(DateTime? value);
		partial void OnBaptismDateChanged();
		
		partial void OnDecisionDateChanging(DateTime? value);
		partial void OnDecisionDateChanged();
		
		partial void OnLetterDateRequestedChanging(DateTime? value);
		partial void OnLetterDateRequestedChanged();
		
		partial void OnLetterDateReceivedChanging(DateTime? value);
		partial void OnLetterDateReceivedChanged();
		
		partial void OnJoinDateChanging(DateTime? value);
		partial void OnJoinDateChanged();
		
		partial void OnDropDateChanging(DateTime? value);
		partial void OnDropDateChanged();
		
		partial void OnDeceasedDateChanging(DateTime? value);
		partial void OnDeceasedDateChanged();
		
		partial void OnTitleCodeChanging(string value);
		partial void OnTitleCodeChanged();
		
		partial void OnFirstNameChanging(string value);
		partial void OnFirstNameChanged();
		
		partial void OnMiddleNameChanging(string value);
		partial void OnMiddleNameChanged();
		
		partial void OnMaidenNameChanging(string value);
		partial void OnMaidenNameChanged();
		
		partial void OnLastNameChanging(string value);
		partial void OnLastNameChanged();
		
		partial void OnSuffixCodeChanging(string value);
		partial void OnSuffixCodeChanged();
		
		partial void OnNickNameChanging(string value);
		partial void OnNickNameChanged();
		
		partial void OnAddressLineOneChanging(string value);
		partial void OnAddressLineOneChanged();
		
		partial void OnAddressLineTwoChanging(string value);
		partial void OnAddressLineTwoChanged();
		
		partial void OnCityNameChanging(string value);
		partial void OnCityNameChanged();
		
		partial void OnStateCodeChanging(string value);
		partial void OnStateCodeChanged();
		
		partial void OnZipCodeChanging(string value);
		partial void OnZipCodeChanged();
		
		partial void OnCountryNameChanging(string value);
		partial void OnCountryNameChanged();
		
		partial void OnStreetNameChanging(string value);
		partial void OnStreetNameChanged();
		
		partial void OnCellPhoneChanging(string value);
		partial void OnCellPhoneChanged();
		
		partial void OnWorkPhoneChanging(string value);
		partial void OnWorkPhoneChanged();
		
		partial void OnEmailAddressChanging(string value);
		partial void OnEmailAddressChanged();
		
		partial void OnOtherPreviousChurchChanging(string value);
		partial void OnOtherPreviousChurchChanged();
		
		partial void OnOtherNewChurchChanging(string value);
		partial void OnOtherNewChurchChanged();
		
		partial void OnSchoolOtherChanging(string value);
		partial void OnSchoolOtherChanged();
		
		partial void OnEmployerOtherChanging(string value);
		partial void OnEmployerOtherChanged();
		
		partial void OnOccupationOtherChanging(string value);
		partial void OnOccupationOtherChanged();
		
		partial void OnHobbyOtherChanging(string value);
		partial void OnHobbyOtherChanged();
		
		partial void OnSkillOtherChanging(string value);
		partial void OnSkillOtherChanged();
		
		partial void OnInterestOtherChanging(string value);
		partial void OnInterestOtherChanged();
		
		partial void OnLetterStatusNotesChanging(string value);
		partial void OnLetterStatusNotesChanged();
		
		partial void OnCommentsChanging(string value);
		partial void OnCommentsChanged();
		
		partial void OnChristAsSaviorChanging(bool value);
		partial void OnChristAsSaviorChanged();
		
		partial void OnMemberAnyChurchChanging(bool? value);
		partial void OnMemberAnyChurchChanged();
		
		partial void OnInterestedInJoiningChanging(bool value);
		partial void OnInterestedInJoiningChanged();
		
		partial void OnPleaseVisitChanging(bool value);
		partial void OnPleaseVisitChanged();
		
		partial void OnInfoBecomeAChristianChanging(bool value);
		partial void OnInfoBecomeAChristianChanged();
		
		partial void OnContributionsStatementChanging(bool value);
		partial void OnContributionsStatementChanged();
		
		partial void OnModifiedByChanging(int? value);
		partial void OnModifiedByChanged();
		
		partial void OnModifiedDateChanging(DateTime? value);
		partial void OnModifiedDateChanged();
		
		partial void OnPictureIdChanging(int? value);
		partial void OnPictureIdChanged();
		
		partial void OnContributionOptionsIdChanging(int? value);
		partial void OnContributionOptionsIdChanged();
		
		partial void OnPrimaryCityChanging(string value);
		partial void OnPrimaryCityChanged();
		
		partial void OnPrimaryZipChanging(string value);
		partial void OnPrimaryZipChanged();
		
		partial void OnPrimaryAddressChanging(string value);
		partial void OnPrimaryAddressChanged();
		
		partial void OnPrimaryStateChanging(string value);
		partial void OnPrimaryStateChanged();
		
		partial void OnHomePhoneChanging(string value);
		partial void OnHomePhoneChanged();
		
		partial void OnSpouseIdChanging(int? value);
		partial void OnSpouseIdChanged();
		
		partial void OnPrimaryAddress2Changing(string value);
		partial void OnPrimaryAddress2Changed();
		
		partial void OnPrimaryResCodeChanging(int? value);
		partial void OnPrimaryResCodeChanged();
		
		partial void OnPrimaryBadAddrFlagChanging(int? value);
		partial void OnPrimaryBadAddrFlagChanged();
		
		partial void OnLastContactChanging(DateTime? value);
		partial void OnLastContactChanged();
		
		partial void OnGradeChanging(int? value);
		partial void OnGradeChanged();
		
		partial void OnCellPhoneLUChanging(string value);
		partial void OnCellPhoneLUChanged();
		
		partial void OnWorkPhoneLUChanging(string value);
		partial void OnWorkPhoneLUChanged();
		
		partial void OnBibleFellowshipClassIdChanging(int? value);
		partial void OnBibleFellowshipClassIdChanged();
		
		partial void OnCampusIdChanging(int? value);
		partial void OnCampusIdChanged();
		
		partial void OnCellPhoneACChanging(string value);
		partial void OnCellPhoneACChanged();
		
		partial void OnCheckInNotesChanging(string value);
		partial void OnCheckInNotesChanged();
		
		partial void OnAgeChanging(int? value);
		partial void OnAgeChanged();
		
		partial void OnAltNameChanging(string value);
		partial void OnAltNameChanged();
		
		partial void OnCustodyIssueChanging(bool? value);
		partial void OnCustodyIssueChanged();
		
		partial void OnOkTransportChanging(bool? value);
		partial void OnOkTransportChanged();
		
		partial void OnBDateChanging(DateTime? value);
		partial void OnBDateChanged();
		
		partial void OnHasDuplicatesChanging(bool? value);
		partial void OnHasDuplicatesChanged();
		
		partial void OnFirstName2Changing(string value);
		partial void OnFirstName2Changed();
		
		partial void OnEmailAddress2Changing(string value);
		partial void OnEmailAddress2Changed();
		
		partial void OnSendEmailAddress1Changing(bool? value);
		partial void OnSendEmailAddress1Changed();
		
		partial void OnSendEmailAddress2Changing(bool? value);
		partial void OnSendEmailAddress2Changed();
		
		partial void OnNewMemberClassDateChanging(DateTime? value);
		partial void OnNewMemberClassDateChanged();
		
		partial void OnPrimaryCountryChanging(string value);
		partial void OnPrimaryCountryChanged();
		
		partial void OnReceiveSMSChanging(bool value);
		partial void OnReceiveSMSChanged();
		
		partial void OnDoNotPublishPhonesChanging(bool? value);
		partial void OnDoNotPublishPhonesChanged();
		
		partial void OnIsDeceasedChanging(bool? value);
		partial void OnIsDeceasedChanged();
		
		partial void OnSsnChanging(string value);
		partial void OnSsnChanged();
		
		partial void OnDlnChanging(string value);
		partial void OnDlnChanged();
		
		partial void OnDLStateIDChanging(int? value);
		partial void OnDLStateIDChanged();
		
		partial void OnHashNumChanging(int? value);
		partial void OnHashNumChanged();
		
		partial void OnPreferredNameChanging(string value);
		partial void OnPreferredNameChanged();
		
		partial void OnName2Changing(string value);
		partial void OnName2Changed();
		
		partial void OnNameChanging(string value);
		partial void OnNameChanged();
		
		partial void OnElectronicStatementChanging(bool? value);
		partial void OnElectronicStatementChanged();
		
    #endregion
		public Person()
		{
			
			this._contactsHad = new EntitySet<Contactee>(new Action< Contactee>(this.attach_contactsHad), new Action< Contactee>(this.detach_contactsHad)); 
			
			this._contactsMade = new EntitySet<Contactor>(new Action< Contactor>(this.attach_contactsMade), new Action< Contactor>(this.detach_contactsMade)); 
			
			this._EnrollmentTransactions = new EntitySet<EnrollmentTransaction>(new Action< EnrollmentTransaction>(this.attach_EnrollmentTransactions), new Action< EnrollmentTransaction>(this.detach_EnrollmentTransactions)); 
			
			this._FamiliesHeaded = new EntitySet<Family>(new Action< Family>(this.attach_FamiliesHeaded), new Action< Family>(this.detach_FamiliesHeaded)); 
			
			this._FamiliesHeaded2 = new EntitySet<Family>(new Action< Family>(this.attach_FamiliesHeaded2), new Action< Family>(this.detach_FamiliesHeaded2)); 
			
			this._Attends = new EntitySet<Attend>(new Action< Attend>(this.attach_Attends), new Action< Attend>(this.detach_Attends)); 
			
			this._BackgroundChecks = new EntitySet<BackgroundCheck>(new Action< BackgroundCheck>(this.attach_BackgroundChecks), new Action< BackgroundCheck>(this.detach_BackgroundChecks)); 
			
			this._CardIdentifiers = new EntitySet<CardIdentifier>(new Action< CardIdentifier>(this.attach_CardIdentifiers), new Action< CardIdentifier>(this.detach_CardIdentifiers)); 
			
			this._CheckInTimes = new EntitySet<CheckInTime>(new Action< CheckInTime>(this.attach_CheckInTimes), new Action< CheckInTime>(this.detach_CheckInTimes)); 
			
			this._Contributions = new EntitySet<Contribution>(new Action< Contribution>(this.attach_Contributions), new Action< Contribution>(this.detach_Contributions)); 
			
			this._Coupons = new EntitySet<Coupon>(new Action< Coupon>(this.attach_Coupons), new Action< Coupon>(this.detach_Coupons)); 
			
			this._EmailOptOuts = new EntitySet<EmailOptOut>(new Action< EmailOptOut>(this.attach_EmailOptOuts), new Action< EmailOptOut>(this.detach_EmailOptOuts)); 
			
			this._EmailQueues = new EntitySet<EmailQueue>(new Action< EmailQueue>(this.attach_EmailQueues), new Action< EmailQueue>(this.detach_EmailQueues)); 
			
			this._EmailQueueTos = new EntitySet<EmailQueueTo>(new Action< EmailQueueTo>(this.attach_EmailQueueTos), new Action< EmailQueueTo>(this.detach_EmailQueueTos)); 
			
			this._EmailResponses = new EntitySet<EmailResponse>(new Action< EmailResponse>(this.attach_EmailResponses), new Action< EmailResponse>(this.detach_EmailResponses)); 
			
			this._FK_Goers = new EntitySet<GoerSupporter>(new Action< GoerSupporter>(this.attach_FK_Goers), new Action< GoerSupporter>(this.detach_FK_Goers)); 
			
			this._ManagedGivings = new EntitySet<ManagedGiving>(new Action< ManagedGiving>(this.attach_ManagedGivings), new Action< ManagedGiving>(this.detach_ManagedGivings)); 
			
			this._MemberDocForms = new EntitySet<MemberDocForm>(new Action< MemberDocForm>(this.attach_MemberDocForms), new Action< MemberDocForm>(this.detach_MemberDocForms)); 
			
			this._MobileAppDevices = new EntitySet<MobileAppDevice>(new Action< MobileAppDevice>(this.attach_MobileAppDevices), new Action< MobileAppDevice>(this.detach_MobileAppDevices)); 
			
			this._MobileAppPushRegistrations = new EntitySet<MobileAppPushRegistration>(new Action< MobileAppPushRegistration>(this.attach_MobileAppPushRegistrations), new Action< MobileAppPushRegistration>(this.detach_MobileAppPushRegistrations)); 
			
			this._OrgMemberExtras = new EntitySet<OrgMemberExtra>(new Action< OrgMemberExtra>(this.attach_OrgMemberExtras), new Action< OrgMemberExtra>(this.detach_OrgMemberExtras)); 
			
			this._PaymentInfos = new EntitySet<PaymentInfo>(new Action< PaymentInfo>(this.attach_PaymentInfos), new Action< PaymentInfo>(this.detach_PaymentInfos)); 
			
			this._PeopleExtras = new EntitySet<PeopleExtra>(new Action< PeopleExtra>(this.attach_PeopleExtras), new Action< PeopleExtra>(this.detach_PeopleExtras)); 
			
			this._PrevOrgMemberExtras = new EntitySet<PrevOrgMemberExtra>(new Action< PrevOrgMemberExtra>(this.attach_PrevOrgMemberExtras), new Action< PrevOrgMemberExtra>(this.detach_PrevOrgMemberExtras)); 
			
			this._RecRegs = new EntitySet<RecReg>(new Action< RecReg>(this.attach_RecRegs), new Action< RecReg>(this.detach_RecRegs)); 
			
			this._RecurringAmounts = new EntitySet<RecurringAmount>(new Action< RecurringAmount>(this.attach_RecurringAmounts), new Action< RecurringAmount>(this.detach_RecurringAmounts)); 
			
			this._SMSItems = new EntitySet<SMSItem>(new Action< SMSItem>(this.attach_SMSItems), new Action< SMSItem>(this.detach_SMSItems)); 
			
			this._SMSLists = new EntitySet<SMSList>(new Action< SMSList>(this.attach_SMSLists), new Action< SMSList>(this.detach_SMSLists)); 
			
			this._FK_Supporters = new EntitySet<GoerSupporter>(new Action< GoerSupporter>(this.attach_FK_Supporters), new Action< GoerSupporter>(this.detach_FK_Supporters)); 
			
			this._TagShares = new EntitySet<TagShare>(new Action< TagShare>(this.attach_TagShares), new Action< TagShare>(this.detach_TagShares)); 
			
			this._TaskListOwners = new EntitySet<TaskListOwner>(new Action< TaskListOwner>(this.attach_TaskListOwners), new Action< TaskListOwner>(this.detach_TaskListOwners)); 
			
			this._Transactions = new EntitySet<Transaction>(new Action< Transaction>(this.attach_Transactions), new Action< Transaction>(this.detach_Transactions)); 
			
			this._TransactionPeople = new EntitySet<TransactionPerson>(new Action< TransactionPerson>(this.attach_TransactionPeople), new Action< TransactionPerson>(this.detach_TransactionPeople)); 
			
			this._Users = new EntitySet<User>(new Action< User>(this.attach_Users), new Action< User>(this.detach_Users)); 
			
			this._VolInterestInterestCodes = new EntitySet<VolInterestInterestCode>(new Action< VolInterestInterestCode>(this.attach_VolInterestInterestCodes), new Action< VolInterestInterestCode>(this.detach_VolInterestInterestCodes)); 
			
			this._Volunteers = new EntitySet<Volunteer>(new Action< Volunteer>(this.attach_Volunteers), new Action< Volunteer>(this.detach_Volunteers)); 
			
			this._VolunteerForms = new EntitySet<VolunteerForm>(new Action< VolunteerForm>(this.attach_VolunteerForms), new Action< VolunteerForm>(this.detach_VolunteerForms)); 
			
			this._VoluteerApprovalIds = new EntitySet<VoluteerApprovalId>(new Action< VoluteerApprovalId>(this.attach_VoluteerApprovalIds), new Action< VoluteerApprovalId>(this.detach_VoluteerApprovalIds)); 
			
			this._GoerAmounts = new EntitySet<GoerSenderAmount>(new Action< GoerSenderAmount>(this.attach_GoerAmounts), new Action< GoerSenderAmount>(this.detach_GoerAmounts)); 
			
			this._OnBehalfOfPeople = new EntitySet<PeopleCanEmailFor>(new Action< PeopleCanEmailFor>(this.attach_OnBehalfOfPeople), new Action< PeopleCanEmailFor>(this.detach_OnBehalfOfPeople)); 
			
			this._OrganizationMembers = new EntitySet<OrganizationMember>(new Action< OrganizationMember>(this.attach_OrganizationMembers), new Action< OrganizationMember>(this.detach_OrganizationMembers)); 
			
			this._People = new EntitySet<BackgroundCheck>(new Action< BackgroundCheck>(this.attach_People), new Action< BackgroundCheck>(this.detach_People)); 
			
			this._PersonsCanEmail = new EntitySet<PeopleCanEmailFor>(new Action< PeopleCanEmailFor>(this.attach_PersonsCanEmail), new Action< PeopleCanEmailFor>(this.detach_PersonsCanEmail)); 
			
			this._SenderAmounts = new EntitySet<GoerSenderAmount>(new Action< GoerSenderAmount>(this.attach_SenderAmounts), new Action< GoerSenderAmount>(this.detach_SenderAmounts)); 
			
			this._SubRequests = new EntitySet<SubRequest>(new Action< SubRequest>(this.attach_SubRequests), new Action< SubRequest>(this.detach_SubRequests)); 
			
			this._SubResponses = new EntitySet<SubRequest>(new Action< SubRequest>(this.attach_SubResponses), new Action< SubRequest>(this.detach_SubResponses)); 
			
			this._Tags = new EntitySet<TagPerson>(new Action< TagPerson>(this.attach_Tags), new Action< TagPerson>(this.detach_Tags)); 
			
			this._TagsOwned = new EntitySet<Tag>(new Action< Tag>(this.attach_TagsOwned), new Action< Tag>(this.detach_TagsOwned)); 
			
			this._Tasks = new EntitySet<Task>(new Action< Task>(this.attach_Tasks), new Action< Task>(this.detach_Tasks)); 
			
			this._TasksAboutPerson = new EntitySet<Task>(new Action< Task>(this.attach_TasksAboutPerson), new Action< Task>(this.detach_TasksAboutPerson)); 
			
			this._TasksCoOwned = new EntitySet<Task>(new Action< Task>(this.attach_TasksCoOwned), new Action< Task>(this.detach_TasksCoOwned)); 
			
			this._VolRequests = new EntitySet<VolRequest>(new Action< VolRequest>(this.attach_VolRequests), new Action< VolRequest>(this.detach_VolRequests)); 
			
			this._VolResponses = new EntitySet<VolRequest>(new Action< VolRequest>(this.attach_VolResponses), new Action< VolRequest>(this.detach_VolResponses)); 
			
			
			this._BFClass = default(EntityRef<Organization>); 
			
			this._EnvelopeOption = default(EntityRef<EnvelopeOption>); 
			
			this._BaptismStatus = default(EntityRef<BaptismStatus>); 
			
			this._BaptismType = default(EntityRef<BaptismType>); 
			
			this._Campu = default(EntityRef<Campu>); 
			
			this._DecisionType = default(EntityRef<DecisionType>); 
			
			this._NewMemberClassStatus = default(EntityRef<NewMemberClassStatus>); 
			
			this._DropType = default(EntityRef<DropType>); 
			
			this._EntryPoint = default(EntityRef<EntryPoint>); 
			
			this._Family = default(EntityRef<Family>); 
			
			this._FamilyPosition = default(EntityRef<FamilyPosition>); 
			
			this._Gender = default(EntityRef<Gender>); 
			
			this._InterestPoint = default(EntityRef<InterestPoint>); 
			
			this._JoinType = default(EntityRef<JoinType>); 
			
			this._MaritalStatus = default(EntityRef<MaritalStatus>); 
			
			this._MemberLetterStatus = default(EntityRef<MemberLetterStatus>); 
			
			this._MemberStatus = default(EntityRef<MemberStatus>); 
			
			this._Origin = default(EntityRef<Origin>); 
			
			this._Picture = default(EntityRef<Picture>); 
			
			this._ResidentCode = default(EntityRef<ResidentCode>); 
			
			this._ContributionStatementOption = default(EntityRef<EnvelopeOption>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
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

		
		[Column(Name="DropCodeId", UpdateCheck=UpdateCheck.Never, Storage="_DropCodeId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int DropCodeId
		{
			get { return this._DropCodeId; }

			set
			{
				if (this._DropCodeId != value)
				{
				
					if (this._DropType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnDropCodeIdChanging(value);
					this.SendPropertyChanging();
					this._DropCodeId = value;
					this.SendPropertyChanged("DropCodeId");
					this.OnDropCodeIdChanged();
				}

			}

		}

		
		[Column(Name="GenderId", UpdateCheck=UpdateCheck.Never, Storage="_GenderId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int GenderId
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

		
		[Column(Name="DoNotMailFlag", UpdateCheck=UpdateCheck.Never, Storage="_DoNotMailFlag", DbType="bit NOT NULL")]
		public bool DoNotMailFlag
		{
			get { return this._DoNotMailFlag; }

			set
			{
				if (this._DoNotMailFlag != value)
				{
				
                    this.OnDoNotMailFlagChanging(value);
					this.SendPropertyChanging();
					this._DoNotMailFlag = value;
					this.SendPropertyChanged("DoNotMailFlag");
					this.OnDoNotMailFlagChanged();
				}

			}

		}

		
		[Column(Name="DoNotCallFlag", UpdateCheck=UpdateCheck.Never, Storage="_DoNotCallFlag", DbType="bit NOT NULL")]
		public bool DoNotCallFlag
		{
			get { return this._DoNotCallFlag; }

			set
			{
				if (this._DoNotCallFlag != value)
				{
				
                    this.OnDoNotCallFlagChanging(value);
					this.SendPropertyChanging();
					this._DoNotCallFlag = value;
					this.SendPropertyChanged("DoNotCallFlag");
					this.OnDoNotCallFlagChanged();
				}

			}

		}

		
		[Column(Name="DoNotVisitFlag", UpdateCheck=UpdateCheck.Never, Storage="_DoNotVisitFlag", DbType="bit NOT NULL")]
		public bool DoNotVisitFlag
		{
			get { return this._DoNotVisitFlag; }

			set
			{
				if (this._DoNotVisitFlag != value)
				{
				
                    this.OnDoNotVisitFlagChanging(value);
					this.SendPropertyChanging();
					this._DoNotVisitFlag = value;
					this.SendPropertyChanged("DoNotVisitFlag");
					this.OnDoNotVisitFlagChanged();
				}

			}

		}

		
		[Column(Name="AddressTypeId", UpdateCheck=UpdateCheck.Never, Storage="_AddressTypeId", DbType="int NOT NULL")]
		public int AddressTypeId
		{
			get { return this._AddressTypeId; }

			set
			{
				if (this._AddressTypeId != value)
				{
				
                    this.OnAddressTypeIdChanging(value);
					this.SendPropertyChanging();
					this._AddressTypeId = value;
					this.SendPropertyChanged("AddressTypeId");
					this.OnAddressTypeIdChanged();
				}

			}

		}

		
		[Column(Name="PhonePrefId", UpdateCheck=UpdateCheck.Never, Storage="_PhonePrefId", DbType="int NOT NULL")]
		public int PhonePrefId
		{
			get { return this._PhonePrefId; }

			set
			{
				if (this._PhonePrefId != value)
				{
				
                    this.OnPhonePrefIdChanging(value);
					this.SendPropertyChanging();
					this._PhonePrefId = value;
					this.SendPropertyChanged("PhonePrefId");
					this.OnPhonePrefIdChanged();
				}

			}

		}

		
		[Column(Name="MaritalStatusId", UpdateCheck=UpdateCheck.Never, Storage="_MaritalStatusId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int MaritalStatusId
		{
			get { return this._MaritalStatusId; }

			set
			{
				if (this._MaritalStatusId != value)
				{
				
					if (this._MaritalStatus.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnMaritalStatusIdChanging(value);
					this.SendPropertyChanging();
					this._MaritalStatusId = value;
					this.SendPropertyChanged("MaritalStatusId");
					this.OnMaritalStatusIdChanged();
				}

			}

		}

		
		[Column(Name="PositionInFamilyId", UpdateCheck=UpdateCheck.Never, Storage="_PositionInFamilyId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int PositionInFamilyId
		{
			get { return this._PositionInFamilyId; }

			set
			{
				if (this._PositionInFamilyId != value)
				{
				
					if (this._FamilyPosition.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPositionInFamilyIdChanging(value);
					this.SendPropertyChanging();
					this._PositionInFamilyId = value;
					this.SendPropertyChanged("PositionInFamilyId");
					this.OnPositionInFamilyIdChanged();
				}

			}

		}

		
		[Column(Name="MemberStatusId", UpdateCheck=UpdateCheck.Never, Storage="_MemberStatusId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int MemberStatusId
		{
			get { return this._MemberStatusId; }

			set
			{
				if (this._MemberStatusId != value)
				{
				
					if (this._MemberStatus.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnMemberStatusIdChanging(value);
					this.SendPropertyChanging();
					this._MemberStatusId = value;
					this.SendPropertyChanged("MemberStatusId");
					this.OnMemberStatusIdChanged();
				}

			}

		}

		
		[Column(Name="FamilyId", UpdateCheck=UpdateCheck.Never, Storage="_FamilyId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int FamilyId
		{
			get { return this._FamilyId; }

			set
			{
				if (this._FamilyId != value)
				{
				
					if (this._Family.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnFamilyIdChanging(value);
					this.SendPropertyChanging();
					this._FamilyId = value;
					this.SendPropertyChanged("FamilyId");
					this.OnFamilyIdChanged();
				}

			}

		}

		
		[Column(Name="BirthMonth", UpdateCheck=UpdateCheck.Never, Storage="_BirthMonth", DbType="int")]
		public int? BirthMonth
		{
			get { return this._BirthMonth; }

			set
			{
				if (this._BirthMonth != value)
				{
				
                    this.OnBirthMonthChanging(value);
					this.SendPropertyChanging();
					this._BirthMonth = value;
					this.SendPropertyChanged("BirthMonth");
					this.OnBirthMonthChanged();
				}

			}

		}

		
		[Column(Name="BirthDay", UpdateCheck=UpdateCheck.Never, Storage="_BirthDay", DbType="int")]
		public int? BirthDay
		{
			get { return this._BirthDay; }

			set
			{
				if (this._BirthDay != value)
				{
				
                    this.OnBirthDayChanging(value);
					this.SendPropertyChanging();
					this._BirthDay = value;
					this.SendPropertyChanged("BirthDay");
					this.OnBirthDayChanged();
				}

			}

		}

		
		[Column(Name="BirthYear", UpdateCheck=UpdateCheck.Never, Storage="_BirthYear", DbType="int")]
		public int? BirthYear
		{
			get { return this._BirthYear; }

			set
			{
				if (this._BirthYear != value)
				{
				
                    this.OnBirthYearChanging(value);
					this.SendPropertyChanging();
					this._BirthYear = value;
					this.SendPropertyChanged("BirthYear");
					this.OnBirthYearChanged();
				}

			}

		}

		
		[Column(Name="OriginId", UpdateCheck=UpdateCheck.Never, Storage="_OriginId", DbType="int")]
		[IsForeignKey]
		public int? OriginId
		{
			get { return this._OriginId; }

			set
			{
				if (this._OriginId != value)
				{
				
					if (this._Origin.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOriginIdChanging(value);
					this.SendPropertyChanging();
					this._OriginId = value;
					this.SendPropertyChanged("OriginId");
					this.OnOriginIdChanged();
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

		
		[Column(Name="InterestPointId", UpdateCheck=UpdateCheck.Never, Storage="_InterestPointId", DbType="int")]
		[IsForeignKey]
		public int? InterestPointId
		{
			get { return this._InterestPointId; }

			set
			{
				if (this._InterestPointId != value)
				{
				
					if (this._InterestPoint.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnInterestPointIdChanging(value);
					this.SendPropertyChanging();
					this._InterestPointId = value;
					this.SendPropertyChanged("InterestPointId");
					this.OnInterestPointIdChanged();
				}

			}

		}

		
		[Column(Name="BaptismTypeId", UpdateCheck=UpdateCheck.Never, Storage="_BaptismTypeId", DbType="int")]
		[IsForeignKey]
		public int? BaptismTypeId
		{
			get { return this._BaptismTypeId; }

			set
			{
				if (this._BaptismTypeId != value)
				{
				
					if (this._BaptismType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnBaptismTypeIdChanging(value);
					this.SendPropertyChanging();
					this._BaptismTypeId = value;
					this.SendPropertyChanged("BaptismTypeId");
					this.OnBaptismTypeIdChanged();
				}

			}

		}

		
		[Column(Name="BaptismStatusId", UpdateCheck=UpdateCheck.Never, Storage="_BaptismStatusId", DbType="int")]
		[IsForeignKey]
		public int? BaptismStatusId
		{
			get { return this._BaptismStatusId; }

			set
			{
				if (this._BaptismStatusId != value)
				{
				
					if (this._BaptismStatus.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnBaptismStatusIdChanging(value);
					this.SendPropertyChanging();
					this._BaptismStatusId = value;
					this.SendPropertyChanged("BaptismStatusId");
					this.OnBaptismStatusIdChanged();
				}

			}

		}

		
		[Column(Name="DecisionTypeId", UpdateCheck=UpdateCheck.Never, Storage="_DecisionTypeId", DbType="int")]
		[IsForeignKey]
		public int? DecisionTypeId
		{
			get { return this._DecisionTypeId; }

			set
			{
				if (this._DecisionTypeId != value)
				{
				
					if (this._DecisionType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnDecisionTypeIdChanging(value);
					this.SendPropertyChanging();
					this._DecisionTypeId = value;
					this.SendPropertyChanged("DecisionTypeId");
					this.OnDecisionTypeIdChanged();
				}

			}

		}

		
		[Column(Name="NewMemberClassStatusId", UpdateCheck=UpdateCheck.Never, Storage="_NewMemberClassStatusId", DbType="int")]
		[IsForeignKey]
		public int? NewMemberClassStatusId
		{
			get { return this._NewMemberClassStatusId; }

			set
			{
				if (this._NewMemberClassStatusId != value)
				{
				
					if (this._NewMemberClassStatus.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnNewMemberClassStatusIdChanging(value);
					this.SendPropertyChanging();
					this._NewMemberClassStatusId = value;
					this.SendPropertyChanged("NewMemberClassStatusId");
					this.OnNewMemberClassStatusIdChanged();
				}

			}

		}

		
		[Column(Name="LetterStatusId", UpdateCheck=UpdateCheck.Never, Storage="_LetterStatusId", DbType="int")]
		[IsForeignKey]
		public int? LetterStatusId
		{
			get { return this._LetterStatusId; }

			set
			{
				if (this._LetterStatusId != value)
				{
				
					if (this._MemberLetterStatus.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnLetterStatusIdChanging(value);
					this.SendPropertyChanging();
					this._LetterStatusId = value;
					this.SendPropertyChanged("LetterStatusId");
					this.OnLetterStatusIdChanged();
				}

			}

		}

		
		[Column(Name="JoinCodeId", UpdateCheck=UpdateCheck.Never, Storage="_JoinCodeId", DbType="int NOT NULL")]
		[IsForeignKey]
		public int JoinCodeId
		{
			get { return this._JoinCodeId; }

			set
			{
				if (this._JoinCodeId != value)
				{
				
					if (this._JoinType.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnJoinCodeIdChanging(value);
					this.SendPropertyChanging();
					this._JoinCodeId = value;
					this.SendPropertyChanged("JoinCodeId");
					this.OnJoinCodeIdChanged();
				}

			}

		}

		
		[Column(Name="EnvelopeOptionsId", UpdateCheck=UpdateCheck.Never, Storage="_EnvelopeOptionsId", DbType="int")]
		[IsForeignKey]
		public int? EnvelopeOptionsId
		{
			get { return this._EnvelopeOptionsId; }

			set
			{
				if (this._EnvelopeOptionsId != value)
				{
				
					if (this._EnvelopeOption.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnEnvelopeOptionsIdChanging(value);
					this.SendPropertyChanging();
					this._EnvelopeOptionsId = value;
					this.SendPropertyChanged("EnvelopeOptionsId");
					this.OnEnvelopeOptionsIdChanged();
				}

			}

		}

		
		[Column(Name="BadAddressFlag", UpdateCheck=UpdateCheck.Never, Storage="_BadAddressFlag", DbType="bit")]
		public bool? BadAddressFlag
		{
			get { return this._BadAddressFlag; }

			set
			{
				if (this._BadAddressFlag != value)
				{
				
                    this.OnBadAddressFlagChanging(value);
					this.SendPropertyChanging();
					this._BadAddressFlag = value;
					this.SendPropertyChanged("BadAddressFlag");
					this.OnBadAddressFlagChanged();
				}

			}

		}

		
		[Column(Name="ResCodeId", UpdateCheck=UpdateCheck.Never, Storage="_ResCodeId", DbType="int")]
		[IsForeignKey]
		public int? ResCodeId
		{
			get { return this._ResCodeId; }

			set
			{
				if (this._ResCodeId != value)
				{
				
					if (this._ResidentCode.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnResCodeIdChanging(value);
					this.SendPropertyChanging();
					this._ResCodeId = value;
					this.SendPropertyChanged("ResCodeId");
					this.OnResCodeIdChanged();
				}

			}

		}

		
		[Column(Name="AddressFromDate", UpdateCheck=UpdateCheck.Never, Storage="_AddressFromDate", DbType="datetime")]
		public DateTime? AddressFromDate
		{
			get { return this._AddressFromDate; }

			set
			{
				if (this._AddressFromDate != value)
				{
				
                    this.OnAddressFromDateChanging(value);
					this.SendPropertyChanging();
					this._AddressFromDate = value;
					this.SendPropertyChanged("AddressFromDate");
					this.OnAddressFromDateChanged();
				}

			}

		}

		
		[Column(Name="AddressToDate", UpdateCheck=UpdateCheck.Never, Storage="_AddressToDate", DbType="datetime")]
		public DateTime? AddressToDate
		{
			get { return this._AddressToDate; }

			set
			{
				if (this._AddressToDate != value)
				{
				
                    this.OnAddressToDateChanging(value);
					this.SendPropertyChanging();
					this._AddressToDate = value;
					this.SendPropertyChanged("AddressToDate");
					this.OnAddressToDateChanged();
				}

			}

		}

		
		[Column(Name="WeddingDate", UpdateCheck=UpdateCheck.Never, Storage="_WeddingDate", DbType="datetime")]
		public DateTime? WeddingDate
		{
			get { return this._WeddingDate; }

			set
			{
				if (this._WeddingDate != value)
				{
				
                    this.OnWeddingDateChanging(value);
					this.SendPropertyChanging();
					this._WeddingDate = value;
					this.SendPropertyChanged("WeddingDate");
					this.OnWeddingDateChanged();
				}

			}

		}

		
		[Column(Name="OriginDate", UpdateCheck=UpdateCheck.Never, Storage="_OriginDate", DbType="datetime")]
		public DateTime? OriginDate
		{
			get { return this._OriginDate; }

			set
			{
				if (this._OriginDate != value)
				{
				
                    this.OnOriginDateChanging(value);
					this.SendPropertyChanging();
					this._OriginDate = value;
					this.SendPropertyChanged("OriginDate");
					this.OnOriginDateChanged();
				}

			}

		}

		
		[Column(Name="BaptismSchedDate", UpdateCheck=UpdateCheck.Never, Storage="_BaptismSchedDate", DbType="datetime")]
		public DateTime? BaptismSchedDate
		{
			get { return this._BaptismSchedDate; }

			set
			{
				if (this._BaptismSchedDate != value)
				{
				
                    this.OnBaptismSchedDateChanging(value);
					this.SendPropertyChanging();
					this._BaptismSchedDate = value;
					this.SendPropertyChanged("BaptismSchedDate");
					this.OnBaptismSchedDateChanged();
				}

			}

		}

		
		[Column(Name="BaptismDate", UpdateCheck=UpdateCheck.Never, Storage="_BaptismDate", DbType="datetime")]
		public DateTime? BaptismDate
		{
			get { return this._BaptismDate; }

			set
			{
				if (this._BaptismDate != value)
				{
				
                    this.OnBaptismDateChanging(value);
					this.SendPropertyChanging();
					this._BaptismDate = value;
					this.SendPropertyChanged("BaptismDate");
					this.OnBaptismDateChanged();
				}

			}

		}

		
		[Column(Name="DecisionDate", UpdateCheck=UpdateCheck.Never, Storage="_DecisionDate", DbType="datetime")]
		public DateTime? DecisionDate
		{
			get { return this._DecisionDate; }

			set
			{
				if (this._DecisionDate != value)
				{
				
                    this.OnDecisionDateChanging(value);
					this.SendPropertyChanging();
					this._DecisionDate = value;
					this.SendPropertyChanged("DecisionDate");
					this.OnDecisionDateChanged();
				}

			}

		}

		
		[Column(Name="LetterDateRequested", UpdateCheck=UpdateCheck.Never, Storage="_LetterDateRequested", DbType="datetime")]
		public DateTime? LetterDateRequested
		{
			get { return this._LetterDateRequested; }

			set
			{
				if (this._LetterDateRequested != value)
				{
				
                    this.OnLetterDateRequestedChanging(value);
					this.SendPropertyChanging();
					this._LetterDateRequested = value;
					this.SendPropertyChanged("LetterDateRequested");
					this.OnLetterDateRequestedChanged();
				}

			}

		}

		
		[Column(Name="LetterDateReceived", UpdateCheck=UpdateCheck.Never, Storage="_LetterDateReceived", DbType="datetime")]
		public DateTime? LetterDateReceived
		{
			get { return this._LetterDateReceived; }

			set
			{
				if (this._LetterDateReceived != value)
				{
				
                    this.OnLetterDateReceivedChanging(value);
					this.SendPropertyChanging();
					this._LetterDateReceived = value;
					this.SendPropertyChanged("LetterDateReceived");
					this.OnLetterDateReceivedChanged();
				}

			}

		}

		
		[Column(Name="JoinDate", UpdateCheck=UpdateCheck.Never, Storage="_JoinDate", DbType="datetime")]
		public DateTime? JoinDate
		{
			get { return this._JoinDate; }

			set
			{
				if (this._JoinDate != value)
				{
				
                    this.OnJoinDateChanging(value);
					this.SendPropertyChanging();
					this._JoinDate = value;
					this.SendPropertyChanged("JoinDate");
					this.OnJoinDateChanged();
				}

			}

		}

		
		[Column(Name="DropDate", UpdateCheck=UpdateCheck.Never, Storage="_DropDate", DbType="datetime")]
		public DateTime? DropDate
		{
			get { return this._DropDate; }

			set
			{
				if (this._DropDate != value)
				{
				
                    this.OnDropDateChanging(value);
					this.SendPropertyChanging();
					this._DropDate = value;
					this.SendPropertyChanged("DropDate");
					this.OnDropDateChanged();
				}

			}

		}

		
		[Column(Name="DeceasedDate", UpdateCheck=UpdateCheck.Never, Storage="_DeceasedDate", DbType="datetime")]
		public DateTime? DeceasedDate
		{
			get { return this._DeceasedDate; }

			set
			{
				if (this._DeceasedDate != value)
				{
				
                    this.OnDeceasedDateChanging(value);
					this.SendPropertyChanging();
					this._DeceasedDate = value;
					this.SendPropertyChanged("DeceasedDate");
					this.OnDeceasedDateChanged();
				}

			}

		}

		
		[Column(Name="TitleCode", UpdateCheck=UpdateCheck.Never, Storage="_TitleCode", DbType="nvarchar(10)")]
		public string TitleCode
		{
			get { return this._TitleCode; }

			set
			{
				if (this._TitleCode != value)
				{
				
                    this.OnTitleCodeChanging(value);
					this.SendPropertyChanging();
					this._TitleCode = value;
					this.SendPropertyChanged("TitleCode");
					this.OnTitleCodeChanged();
				}

			}

		}

		
		[Column(Name="FirstName", UpdateCheck=UpdateCheck.Never, Storage="_FirstName", DbType="nvarchar(25)")]
		public string FirstName
		{
			get { return this._FirstName; }

			set
			{
				if (this._FirstName != value)
				{
				
                    this.OnFirstNameChanging(value);
					this.SendPropertyChanging();
					this._FirstName = value;
					this.SendPropertyChanged("FirstName");
					this.OnFirstNameChanged();
				}

			}

		}

		
		[Column(Name="MiddleName", UpdateCheck=UpdateCheck.Never, Storage="_MiddleName", DbType="nvarchar(30)")]
		public string MiddleName
		{
			get { return this._MiddleName; }

			set
			{
				if (this._MiddleName != value)
				{
				
                    this.OnMiddleNameChanging(value);
					this.SendPropertyChanging();
					this._MiddleName = value;
					this.SendPropertyChanged("MiddleName");
					this.OnMiddleNameChanged();
				}

			}

		}

		
		[Column(Name="MaidenName", UpdateCheck=UpdateCheck.Never, Storage="_MaidenName", DbType="nvarchar(20)")]
		public string MaidenName
		{
			get { return this._MaidenName; }

			set
			{
				if (this._MaidenName != value)
				{
				
                    this.OnMaidenNameChanging(value);
					this.SendPropertyChanging();
					this._MaidenName = value;
					this.SendPropertyChanged("MaidenName");
					this.OnMaidenNameChanged();
				}

			}

		}

		
		[Column(Name="LastName", UpdateCheck=UpdateCheck.Never, Storage="_LastName", DbType="nvarchar(100) NOT NULL")]
		public string LastName
		{
			get { return this._LastName; }

			set
			{
				if (this._LastName != value)
				{
				
                    this.OnLastNameChanging(value);
					this.SendPropertyChanging();
					this._LastName = value;
					this.SendPropertyChanged("LastName");
					this.OnLastNameChanged();
				}

			}

		}

		
		[Column(Name="SuffixCode", UpdateCheck=UpdateCheck.Never, Storage="_SuffixCode", DbType="nvarchar(10)")]
		public string SuffixCode
		{
			get { return this._SuffixCode; }

			set
			{
				if (this._SuffixCode != value)
				{
				
                    this.OnSuffixCodeChanging(value);
					this.SendPropertyChanging();
					this._SuffixCode = value;
					this.SendPropertyChanged("SuffixCode");
					this.OnSuffixCodeChanged();
				}

			}

		}

		
		[Column(Name="NickName", UpdateCheck=UpdateCheck.Never, Storage="_NickName", DbType="nvarchar(25)")]
		public string NickName
		{
			get { return this._NickName; }

			set
			{
				if (this._NickName != value)
				{
				
                    this.OnNickNameChanging(value);
					this.SendPropertyChanging();
					this._NickName = value;
					this.SendPropertyChanged("NickName");
					this.OnNickNameChanged();
				}

			}

		}

		
		[Column(Name="AddressLineOne", UpdateCheck=UpdateCheck.Never, Storage="_AddressLineOne", DbType="nvarchar(100)")]
		public string AddressLineOne
		{
			get { return this._AddressLineOne; }

			set
			{
				if (this._AddressLineOne != value)
				{
				
                    this.OnAddressLineOneChanging(value);
					this.SendPropertyChanging();
					this._AddressLineOne = value;
					this.SendPropertyChanged("AddressLineOne");
					this.OnAddressLineOneChanged();
				}

			}

		}

		
		[Column(Name="AddressLineTwo", UpdateCheck=UpdateCheck.Never, Storage="_AddressLineTwo", DbType="nvarchar(100)")]
		public string AddressLineTwo
		{
			get { return this._AddressLineTwo; }

			set
			{
				if (this._AddressLineTwo != value)
				{
				
                    this.OnAddressLineTwoChanging(value);
					this.SendPropertyChanging();
					this._AddressLineTwo = value;
					this.SendPropertyChanged("AddressLineTwo");
					this.OnAddressLineTwoChanged();
				}

			}

		}

		
		[Column(Name="CityName", UpdateCheck=UpdateCheck.Never, Storage="_CityName", DbType="nvarchar(30)")]
		public string CityName
		{
			get { return this._CityName; }

			set
			{
				if (this._CityName != value)
				{
				
                    this.OnCityNameChanging(value);
					this.SendPropertyChanging();
					this._CityName = value;
					this.SendPropertyChanged("CityName");
					this.OnCityNameChanged();
				}

			}

		}

		
		[Column(Name="StateCode", UpdateCheck=UpdateCheck.Never, Storage="_StateCode", DbType="nvarchar(30)")]
		public string StateCode
		{
			get { return this._StateCode; }

			set
			{
				if (this._StateCode != value)
				{
				
                    this.OnStateCodeChanging(value);
					this.SendPropertyChanging();
					this._StateCode = value;
					this.SendPropertyChanged("StateCode");
					this.OnStateCodeChanged();
				}

			}

		}

		
		[Column(Name="ZipCode", UpdateCheck=UpdateCheck.Never, Storage="_ZipCode", DbType="nvarchar(15)")]
		public string ZipCode
		{
			get { return this._ZipCode; }

			set
			{
				if (this._ZipCode != value)
				{
				
                    this.OnZipCodeChanging(value);
					this.SendPropertyChanging();
					this._ZipCode = value;
					this.SendPropertyChanged("ZipCode");
					this.OnZipCodeChanged();
				}

			}

		}

		
		[Column(Name="CountryName", UpdateCheck=UpdateCheck.Never, Storage="_CountryName", DbType="nvarchar(40)")]
		public string CountryName
		{
			get { return this._CountryName; }

			set
			{
				if (this._CountryName != value)
				{
				
                    this.OnCountryNameChanging(value);
					this.SendPropertyChanging();
					this._CountryName = value;
					this.SendPropertyChanged("CountryName");
					this.OnCountryNameChanged();
				}

			}

		}

		
		[Column(Name="StreetName", UpdateCheck=UpdateCheck.Never, Storage="_StreetName", DbType="nvarchar(40)")]
		public string StreetName
		{
			get { return this._StreetName; }

			set
			{
				if (this._StreetName != value)
				{
				
                    this.OnStreetNameChanging(value);
					this.SendPropertyChanging();
					this._StreetName = value;
					this.SendPropertyChanged("StreetName");
					this.OnStreetNameChanged();
				}

			}

		}

		
		[Column(Name="CellPhone", UpdateCheck=UpdateCheck.Never, Storage="_CellPhone", DbType="nvarchar(20)")]
		public string CellPhone
		{
			get { return this._CellPhone; }

			set
			{
				if (this._CellPhone != value)
				{
				
                    this.OnCellPhoneChanging(value);
					this.SendPropertyChanging();
					this._CellPhone = value;
					this.SendPropertyChanged("CellPhone");
					this.OnCellPhoneChanged();
				}

			}

		}

		
		[Column(Name="WorkPhone", UpdateCheck=UpdateCheck.Never, Storage="_WorkPhone", DbType="nvarchar(20)")]
		public string WorkPhone
		{
			get { return this._WorkPhone; }

			set
			{
				if (this._WorkPhone != value)
				{
				
                    this.OnWorkPhoneChanging(value);
					this.SendPropertyChanging();
					this._WorkPhone = value;
					this.SendPropertyChanged("WorkPhone");
					this.OnWorkPhoneChanged();
				}

			}

		}

		
		[Column(Name="EmailAddress", UpdateCheck=UpdateCheck.Never, Storage="_EmailAddress", DbType="nvarchar(150)")]
		public string EmailAddress
		{
			get { return this._EmailAddress; }

			set
			{
				if (this._EmailAddress != value)
				{
				
                    this.OnEmailAddressChanging(value);
					this.SendPropertyChanging();
					this._EmailAddress = value;
					this.SendPropertyChanged("EmailAddress");
					this.OnEmailAddressChanged();
				}

			}

		}

		
		[Column(Name="OtherPreviousChurch", UpdateCheck=UpdateCheck.Never, Storage="_OtherPreviousChurch", DbType="nvarchar(120)")]
		public string OtherPreviousChurch
		{
			get { return this._OtherPreviousChurch; }

			set
			{
				if (this._OtherPreviousChurch != value)
				{
				
                    this.OnOtherPreviousChurchChanging(value);
					this.SendPropertyChanging();
					this._OtherPreviousChurch = value;
					this.SendPropertyChanged("OtherPreviousChurch");
					this.OnOtherPreviousChurchChanged();
				}

			}

		}

		
		[Column(Name="OtherNewChurch", UpdateCheck=UpdateCheck.Never, Storage="_OtherNewChurch", DbType="nvarchar(60)")]
		public string OtherNewChurch
		{
			get { return this._OtherNewChurch; }

			set
			{
				if (this._OtherNewChurch != value)
				{
				
                    this.OnOtherNewChurchChanging(value);
					this.SendPropertyChanging();
					this._OtherNewChurch = value;
					this.SendPropertyChanged("OtherNewChurch");
					this.OnOtherNewChurchChanged();
				}

			}

		}

		
		[Column(Name="SchoolOther", UpdateCheck=UpdateCheck.Never, Storage="_SchoolOther", DbType="nvarchar(100)")]
		public string SchoolOther
		{
			get { return this._SchoolOther; }

			set
			{
				if (this._SchoolOther != value)
				{
				
                    this.OnSchoolOtherChanging(value);
					this.SendPropertyChanging();
					this._SchoolOther = value;
					this.SendPropertyChanged("SchoolOther");
					this.OnSchoolOtherChanged();
				}

			}

		}

		
		[Column(Name="EmployerOther", UpdateCheck=UpdateCheck.Never, Storage="_EmployerOther", DbType="nvarchar(120)")]
		public string EmployerOther
		{
			get { return this._EmployerOther; }

			set
			{
				if (this._EmployerOther != value)
				{
				
                    this.OnEmployerOtherChanging(value);
					this.SendPropertyChanging();
					this._EmployerOther = value;
					this.SendPropertyChanged("EmployerOther");
					this.OnEmployerOtherChanged();
				}

			}

		}

		
		[Column(Name="OccupationOther", UpdateCheck=UpdateCheck.Never, Storage="_OccupationOther", DbType="nvarchar(120)")]
		public string OccupationOther
		{
			get { return this._OccupationOther; }

			set
			{
				if (this._OccupationOther != value)
				{
				
                    this.OnOccupationOtherChanging(value);
					this.SendPropertyChanging();
					this._OccupationOther = value;
					this.SendPropertyChanged("OccupationOther");
					this.OnOccupationOtherChanged();
				}

			}

		}

		
		[Column(Name="HobbyOther", UpdateCheck=UpdateCheck.Never, Storage="_HobbyOther", DbType="nvarchar(40)")]
		public string HobbyOther
		{
			get { return this._HobbyOther; }

			set
			{
				if (this._HobbyOther != value)
				{
				
                    this.OnHobbyOtherChanging(value);
					this.SendPropertyChanging();
					this._HobbyOther = value;
					this.SendPropertyChanged("HobbyOther");
					this.OnHobbyOtherChanged();
				}

			}

		}

		
		[Column(Name="SkillOther", UpdateCheck=UpdateCheck.Never, Storage="_SkillOther", DbType="nvarchar(40)")]
		public string SkillOther
		{
			get { return this._SkillOther; }

			set
			{
				if (this._SkillOther != value)
				{
				
                    this.OnSkillOtherChanging(value);
					this.SendPropertyChanging();
					this._SkillOther = value;
					this.SendPropertyChanged("SkillOther");
					this.OnSkillOtherChanged();
				}

			}

		}

		
		[Column(Name="InterestOther", UpdateCheck=UpdateCheck.Never, Storage="_InterestOther", DbType="nvarchar(40)")]
		public string InterestOther
		{
			get { return this._InterestOther; }

			set
			{
				if (this._InterestOther != value)
				{
				
                    this.OnInterestOtherChanging(value);
					this.SendPropertyChanging();
					this._InterestOther = value;
					this.SendPropertyChanged("InterestOther");
					this.OnInterestOtherChanged();
				}

			}

		}

		
		[Column(Name="LetterStatusNotes", UpdateCheck=UpdateCheck.Never, Storage="_LetterStatusNotes", DbType="nvarchar(3000)")]
		public string LetterStatusNotes
		{
			get { return this._LetterStatusNotes; }

			set
			{
				if (this._LetterStatusNotes != value)
				{
				
                    this.OnLetterStatusNotesChanging(value);
					this.SendPropertyChanging();
					this._LetterStatusNotes = value;
					this.SendPropertyChanged("LetterStatusNotes");
					this.OnLetterStatusNotesChanged();
				}

			}

		}

		
		[Column(Name="Comments", UpdateCheck=UpdateCheck.Never, Storage="_Comments", DbType="nvarchar(3000)")]
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

		
		[Column(Name="ChristAsSavior", UpdateCheck=UpdateCheck.Never, Storage="_ChristAsSavior", DbType="bit NOT NULL")]
		public bool ChristAsSavior
		{
			get { return this._ChristAsSavior; }

			set
			{
				if (this._ChristAsSavior != value)
				{
				
                    this.OnChristAsSaviorChanging(value);
					this.SendPropertyChanging();
					this._ChristAsSavior = value;
					this.SendPropertyChanged("ChristAsSavior");
					this.OnChristAsSaviorChanged();
				}

			}

		}

		
		[Column(Name="MemberAnyChurch", UpdateCheck=UpdateCheck.Never, Storage="_MemberAnyChurch", DbType="bit")]
		public bool? MemberAnyChurch
		{
			get { return this._MemberAnyChurch; }

			set
			{
				if (this._MemberAnyChurch != value)
				{
				
                    this.OnMemberAnyChurchChanging(value);
					this.SendPropertyChanging();
					this._MemberAnyChurch = value;
					this.SendPropertyChanged("MemberAnyChurch");
					this.OnMemberAnyChurchChanged();
				}

			}

		}

		
		[Column(Name="InterestedInJoining", UpdateCheck=UpdateCheck.Never, Storage="_InterestedInJoining", DbType="bit NOT NULL")]
		public bool InterestedInJoining
		{
			get { return this._InterestedInJoining; }

			set
			{
				if (this._InterestedInJoining != value)
				{
				
                    this.OnInterestedInJoiningChanging(value);
					this.SendPropertyChanging();
					this._InterestedInJoining = value;
					this.SendPropertyChanged("InterestedInJoining");
					this.OnInterestedInJoiningChanged();
				}

			}

		}

		
		[Column(Name="PleaseVisit", UpdateCheck=UpdateCheck.Never, Storage="_PleaseVisit", DbType="bit NOT NULL")]
		public bool PleaseVisit
		{
			get { return this._PleaseVisit; }

			set
			{
				if (this._PleaseVisit != value)
				{
				
                    this.OnPleaseVisitChanging(value);
					this.SendPropertyChanging();
					this._PleaseVisit = value;
					this.SendPropertyChanged("PleaseVisit");
					this.OnPleaseVisitChanged();
				}

			}

		}

		
		[Column(Name="InfoBecomeAChristian", UpdateCheck=UpdateCheck.Never, Storage="_InfoBecomeAChristian", DbType="bit NOT NULL")]
		public bool InfoBecomeAChristian
		{
			get { return this._InfoBecomeAChristian; }

			set
			{
				if (this._InfoBecomeAChristian != value)
				{
				
                    this.OnInfoBecomeAChristianChanging(value);
					this.SendPropertyChanging();
					this._InfoBecomeAChristian = value;
					this.SendPropertyChanged("InfoBecomeAChristian");
					this.OnInfoBecomeAChristianChanged();
				}

			}

		}

		
		[Column(Name="ContributionsStatement", UpdateCheck=UpdateCheck.Never, Storage="_ContributionsStatement", DbType="bit NOT NULL")]
		public bool ContributionsStatement
		{
			get { return this._ContributionsStatement; }

			set
			{
				if (this._ContributionsStatement != value)
				{
				
                    this.OnContributionsStatementChanging(value);
					this.SendPropertyChanging();
					this._ContributionsStatement = value;
					this.SendPropertyChanged("ContributionsStatement");
					this.OnContributionsStatementChanged();
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

		
		[Column(Name="PictureId", UpdateCheck=UpdateCheck.Never, Storage="_PictureId", DbType="int")]
		[IsForeignKey]
		public int? PictureId
		{
			get { return this._PictureId; }

			set
			{
				if (this._PictureId != value)
				{
				
					if (this._Picture.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPictureIdChanging(value);
					this.SendPropertyChanging();
					this._PictureId = value;
					this.SendPropertyChanged("PictureId");
					this.OnPictureIdChanged();
				}

			}

		}

		
		[Column(Name="ContributionOptionsId", UpdateCheck=UpdateCheck.Never, Storage="_ContributionOptionsId", DbType="int")]
		[IsForeignKey]
		public int? ContributionOptionsId
		{
			get { return this._ContributionOptionsId; }

			set
			{
				if (this._ContributionOptionsId != value)
				{
				
					if (this._ContributionStatementOption.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnContributionOptionsIdChanging(value);
					this.SendPropertyChanging();
					this._ContributionOptionsId = value;
					this.SendPropertyChanged("ContributionOptionsId");
					this.OnContributionOptionsIdChanged();
				}

			}

		}

		
		[Column(Name="PrimaryCity", UpdateCheck=UpdateCheck.Never, Storage="_PrimaryCity", DbType="nvarchar(30)")]
		public string PrimaryCity
		{
			get { return this._PrimaryCity; }

			set
			{
				if (this._PrimaryCity != value)
				{
				
                    this.OnPrimaryCityChanging(value);
					this.SendPropertyChanging();
					this._PrimaryCity = value;
					this.SendPropertyChanged("PrimaryCity");
					this.OnPrimaryCityChanged();
				}

			}

		}

		
		[Column(Name="PrimaryZip", UpdateCheck=UpdateCheck.Never, Storage="_PrimaryZip", DbType="nvarchar(15)")]
		public string PrimaryZip
		{
			get { return this._PrimaryZip; }

			set
			{
				if (this._PrimaryZip != value)
				{
				
                    this.OnPrimaryZipChanging(value);
					this.SendPropertyChanging();
					this._PrimaryZip = value;
					this.SendPropertyChanged("PrimaryZip");
					this.OnPrimaryZipChanged();
				}

			}

		}

		
		[Column(Name="PrimaryAddress", UpdateCheck=UpdateCheck.Never, Storage="_PrimaryAddress", DbType="nvarchar(100)")]
		public string PrimaryAddress
		{
			get { return this._PrimaryAddress; }

			set
			{
				if (this._PrimaryAddress != value)
				{
				
                    this.OnPrimaryAddressChanging(value);
					this.SendPropertyChanging();
					this._PrimaryAddress = value;
					this.SendPropertyChanged("PrimaryAddress");
					this.OnPrimaryAddressChanged();
				}

			}

		}

		
		[Column(Name="PrimaryState", UpdateCheck=UpdateCheck.Never, Storage="_PrimaryState", DbType="nvarchar(20)")]
		public string PrimaryState
		{
			get { return this._PrimaryState; }

			set
			{
				if (this._PrimaryState != value)
				{
				
                    this.OnPrimaryStateChanging(value);
					this.SendPropertyChanging();
					this._PrimaryState = value;
					this.SendPropertyChanged("PrimaryState");
					this.OnPrimaryStateChanged();
				}

			}

		}

		
		[Column(Name="HomePhone", UpdateCheck=UpdateCheck.Never, Storage="_HomePhone", DbType="nvarchar(20)")]
		public string HomePhone
		{
			get { return this._HomePhone; }

			set
			{
				if (this._HomePhone != value)
				{
				
                    this.OnHomePhoneChanging(value);
					this.SendPropertyChanging();
					this._HomePhone = value;
					this.SendPropertyChanged("HomePhone");
					this.OnHomePhoneChanged();
				}

			}

		}

		
		[Column(Name="SpouseId", UpdateCheck=UpdateCheck.Never, Storage="_SpouseId", DbType="int")]
		public int? SpouseId
		{
			get { return this._SpouseId; }

			set
			{
				if (this._SpouseId != value)
				{
				
                    this.OnSpouseIdChanging(value);
					this.SendPropertyChanging();
					this._SpouseId = value;
					this.SendPropertyChanged("SpouseId");
					this.OnSpouseIdChanged();
				}

			}

		}

		
		[Column(Name="PrimaryAddress2", UpdateCheck=UpdateCheck.Never, Storage="_PrimaryAddress2", DbType="nvarchar(100)")]
		public string PrimaryAddress2
		{
			get { return this._PrimaryAddress2; }

			set
			{
				if (this._PrimaryAddress2 != value)
				{
				
                    this.OnPrimaryAddress2Changing(value);
					this.SendPropertyChanging();
					this._PrimaryAddress2 = value;
					this.SendPropertyChanged("PrimaryAddress2");
					this.OnPrimaryAddress2Changed();
				}

			}

		}

		
		[Column(Name="PrimaryResCode", UpdateCheck=UpdateCheck.Never, Storage="_PrimaryResCode", DbType="int")]
		public int? PrimaryResCode
		{
			get { return this._PrimaryResCode; }

			set
			{
				if (this._PrimaryResCode != value)
				{
				
                    this.OnPrimaryResCodeChanging(value);
					this.SendPropertyChanging();
					this._PrimaryResCode = value;
					this.SendPropertyChanged("PrimaryResCode");
					this.OnPrimaryResCodeChanged();
				}

			}

		}

		
		[Column(Name="PrimaryBadAddrFlag", UpdateCheck=UpdateCheck.Never, Storage="_PrimaryBadAddrFlag", DbType="int")]
		public int? PrimaryBadAddrFlag
		{
			get { return this._PrimaryBadAddrFlag; }

			set
			{
				if (this._PrimaryBadAddrFlag != value)
				{
				
                    this.OnPrimaryBadAddrFlagChanging(value);
					this.SendPropertyChanging();
					this._PrimaryBadAddrFlag = value;
					this.SendPropertyChanged("PrimaryBadAddrFlag");
					this.OnPrimaryBadAddrFlagChanged();
				}

			}

		}

		
		[Column(Name="LastContact", UpdateCheck=UpdateCheck.Never, Storage="_LastContact", DbType="datetime")]
		public DateTime? LastContact
		{
			get { return this._LastContact; }

			set
			{
				if (this._LastContact != value)
				{
				
                    this.OnLastContactChanging(value);
					this.SendPropertyChanging();
					this._LastContact = value;
					this.SendPropertyChanged("LastContact");
					this.OnLastContactChanged();
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

		
		[Column(Name="CellPhoneLU", UpdateCheck=UpdateCheck.Never, Storage="_CellPhoneLU", DbType="char(7)")]
		public string CellPhoneLU
		{
			get { return this._CellPhoneLU; }

			set
			{
				if (this._CellPhoneLU != value)
				{
				
                    this.OnCellPhoneLUChanging(value);
					this.SendPropertyChanging();
					this._CellPhoneLU = value;
					this.SendPropertyChanged("CellPhoneLU");
					this.OnCellPhoneLUChanged();
				}

			}

		}

		
		[Column(Name="WorkPhoneLU", UpdateCheck=UpdateCheck.Never, Storage="_WorkPhoneLU", DbType="char(7)")]
		public string WorkPhoneLU
		{
			get { return this._WorkPhoneLU; }

			set
			{
				if (this._WorkPhoneLU != value)
				{
				
                    this.OnWorkPhoneLUChanging(value);
					this.SendPropertyChanging();
					this._WorkPhoneLU = value;
					this.SendPropertyChanged("WorkPhoneLU");
					this.OnWorkPhoneLUChanged();
				}

			}

		}

		
		[Column(Name="BibleFellowshipClassId", UpdateCheck=UpdateCheck.Never, Storage="_BibleFellowshipClassId", DbType="int")]
		[IsForeignKey]
		public int? BibleFellowshipClassId
		{
			get { return this._BibleFellowshipClassId; }

			set
			{
				if (this._BibleFellowshipClassId != value)
				{
				
					if (this._BFClass.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnBibleFellowshipClassIdChanging(value);
					this.SendPropertyChanging();
					this._BibleFellowshipClassId = value;
					this.SendPropertyChanged("BibleFellowshipClassId");
					this.OnBibleFellowshipClassIdChanged();
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

		
		[Column(Name="CellPhoneAC", UpdateCheck=UpdateCheck.Never, Storage="_CellPhoneAC", DbType="char(3)")]
		public string CellPhoneAC
		{
			get { return this._CellPhoneAC; }

			set
			{
				if (this._CellPhoneAC != value)
				{
				
                    this.OnCellPhoneACChanging(value);
					this.SendPropertyChanging();
					this._CellPhoneAC = value;
					this.SendPropertyChanged("CellPhoneAC");
					this.OnCellPhoneACChanged();
				}

			}

		}

		
		[Column(Name="CheckInNotes", UpdateCheck=UpdateCheck.Never, Storage="_CheckInNotes", DbType="nvarchar(1000)")]
		public string CheckInNotes
		{
			get { return this._CheckInNotes; }

			set
			{
				if (this._CheckInNotes != value)
				{
				
                    this.OnCheckInNotesChanging(value);
					this.SendPropertyChanging();
					this._CheckInNotes = value;
					this.SendPropertyChanged("CheckInNotes");
					this.OnCheckInNotesChanged();
				}

			}

		}

		
		[Column(Name="Age", UpdateCheck=UpdateCheck.Never, Storage="_Age", DbType="int", IsDbGenerated=true)]
		public int? Age
		{
			get { return this._Age; }

			set
			{
				if (this._Age != value)
				{
				
                    this.OnAgeChanging(value);
					this.SendPropertyChanging();
					this._Age = value;
					this.SendPropertyChanged("Age");
					this.OnAgeChanged();
				}

			}

		}

		
		[Column(Name="AltName", UpdateCheck=UpdateCheck.Never, Storage="_AltName", DbType="nvarchar(100)")]
		public string AltName
		{
			get { return this._AltName; }

			set
			{
				if (this._AltName != value)
				{
				
                    this.OnAltNameChanging(value);
					this.SendPropertyChanging();
					this._AltName = value;
					this.SendPropertyChanged("AltName");
					this.OnAltNameChanged();
				}

			}

		}

		
		[Column(Name="CustodyIssue", UpdateCheck=UpdateCheck.Never, Storage="_CustodyIssue", DbType="bit")]
		public bool? CustodyIssue
		{
			get { return this._CustodyIssue; }

			set
			{
				if (this._CustodyIssue != value)
				{
				
                    this.OnCustodyIssueChanging(value);
					this.SendPropertyChanging();
					this._CustodyIssue = value;
					this.SendPropertyChanged("CustodyIssue");
					this.OnCustodyIssueChanged();
				}

			}

		}

		
		[Column(Name="OkTransport", UpdateCheck=UpdateCheck.Never, Storage="_OkTransport", DbType="bit")]
		public bool? OkTransport
		{
			get { return this._OkTransport; }

			set
			{
				if (this._OkTransport != value)
				{
				
                    this.OnOkTransportChanging(value);
					this.SendPropertyChanging();
					this._OkTransport = value;
					this.SendPropertyChanged("OkTransport");
					this.OnOkTransportChanged();
				}

			}

		}

		
		[Column(Name="BDate", UpdateCheck=UpdateCheck.Never, Storage="_BDate", DbType="datetime", IsDbGenerated=true)]
		public DateTime? BDate
		{
			get { return this._BDate; }

			set
			{
				if (this._BDate != value)
				{
				
                    this.OnBDateChanging(value);
					this.SendPropertyChanging();
					this._BDate = value;
					this.SendPropertyChanged("BDate");
					this.OnBDateChanged();
				}

			}

		}

		
		[Column(Name="HasDuplicates", UpdateCheck=UpdateCheck.Never, Storage="_HasDuplicates", DbType="bit")]
		public bool? HasDuplicates
		{
			get { return this._HasDuplicates; }

			set
			{
				if (this._HasDuplicates != value)
				{
				
                    this.OnHasDuplicatesChanging(value);
					this.SendPropertyChanging();
					this._HasDuplicates = value;
					this.SendPropertyChanged("HasDuplicates");
					this.OnHasDuplicatesChanged();
				}

			}

		}

		
		[Column(Name="FirstName2", UpdateCheck=UpdateCheck.Never, Storage="_FirstName2", DbType="nvarchar(50)")]
		public string FirstName2
		{
			get { return this._FirstName2; }

			set
			{
				if (this._FirstName2 != value)
				{
				
                    this.OnFirstName2Changing(value);
					this.SendPropertyChanging();
					this._FirstName2 = value;
					this.SendPropertyChanged("FirstName2");
					this.OnFirstName2Changed();
				}

			}

		}

		
		[Column(Name="EmailAddress2", UpdateCheck=UpdateCheck.Never, Storage="_EmailAddress2", DbType="nvarchar(60)")]
		public string EmailAddress2
		{
			get { return this._EmailAddress2; }

			set
			{
				if (this._EmailAddress2 != value)
				{
				
                    this.OnEmailAddress2Changing(value);
					this.SendPropertyChanging();
					this._EmailAddress2 = value;
					this.SendPropertyChanged("EmailAddress2");
					this.OnEmailAddress2Changed();
				}

			}

		}

		
		[Column(Name="SendEmailAddress1", UpdateCheck=UpdateCheck.Never, Storage="_SendEmailAddress1", DbType="bit")]
		public bool? SendEmailAddress1
		{
			get { return this._SendEmailAddress1; }

			set
			{
				if (this._SendEmailAddress1 != value)
				{
				
                    this.OnSendEmailAddress1Changing(value);
					this.SendPropertyChanging();
					this._SendEmailAddress1 = value;
					this.SendPropertyChanged("SendEmailAddress1");
					this.OnSendEmailAddress1Changed();
				}

			}

		}

		
		[Column(Name="SendEmailAddress2", UpdateCheck=UpdateCheck.Never, Storage="_SendEmailAddress2", DbType="bit")]
		public bool? SendEmailAddress2
		{
			get { return this._SendEmailAddress2; }

			set
			{
				if (this._SendEmailAddress2 != value)
				{
				
                    this.OnSendEmailAddress2Changing(value);
					this.SendPropertyChanging();
					this._SendEmailAddress2 = value;
					this.SendPropertyChanged("SendEmailAddress2");
					this.OnSendEmailAddress2Changed();
				}

			}

		}

		
		[Column(Name="NewMemberClassDate", UpdateCheck=UpdateCheck.Never, Storage="_NewMemberClassDate", DbType="datetime")]
		public DateTime? NewMemberClassDate
		{
			get { return this._NewMemberClassDate; }

			set
			{
				if (this._NewMemberClassDate != value)
				{
				
                    this.OnNewMemberClassDateChanging(value);
					this.SendPropertyChanging();
					this._NewMemberClassDate = value;
					this.SendPropertyChanged("NewMemberClassDate");
					this.OnNewMemberClassDateChanged();
				}

			}

		}

		
		[Column(Name="PrimaryCountry", UpdateCheck=UpdateCheck.Never, Storage="_PrimaryCountry", DbType="nvarchar(40)")]
		public string PrimaryCountry
		{
			get { return this._PrimaryCountry; }

			set
			{
				if (this._PrimaryCountry != value)
				{
				
                    this.OnPrimaryCountryChanging(value);
					this.SendPropertyChanging();
					this._PrimaryCountry = value;
					this.SendPropertyChanged("PrimaryCountry");
					this.OnPrimaryCountryChanged();
				}

			}

		}

		
		[Column(Name="ReceiveSMS", UpdateCheck=UpdateCheck.Never, Storage="_ReceiveSMS", DbType="bit NOT NULL")]
		public bool ReceiveSMS
		{
			get { return this._ReceiveSMS; }

			set
			{
				if (this._ReceiveSMS != value)
				{
				
                    this.OnReceiveSMSChanging(value);
					this.SendPropertyChanging();
					this._ReceiveSMS = value;
					this.SendPropertyChanged("ReceiveSMS");
					this.OnReceiveSMSChanged();
				}

			}

		}

		
		[Column(Name="DoNotPublishPhones", UpdateCheck=UpdateCheck.Never, Storage="_DoNotPublishPhones", DbType="bit")]
		public bool? DoNotPublishPhones
		{
			get { return this._DoNotPublishPhones; }

			set
			{
				if (this._DoNotPublishPhones != value)
				{
				
                    this.OnDoNotPublishPhonesChanging(value);
					this.SendPropertyChanging();
					this._DoNotPublishPhones = value;
					this.SendPropertyChanged("DoNotPublishPhones");
					this.OnDoNotPublishPhonesChanged();
				}

			}

		}

		
		[Column(Name="IsDeceased", UpdateCheck=UpdateCheck.Never, Storage="_IsDeceased", DbType="bit", IsDbGenerated=true)]
		public bool? IsDeceased
		{
			get { return this._IsDeceased; }

			set
			{
				if (this._IsDeceased != value)
				{
				
                    this.OnIsDeceasedChanging(value);
					this.SendPropertyChanging();
					this._IsDeceased = value;
					this.SendPropertyChanged("IsDeceased");
					this.OnIsDeceasedChanged();
				}

			}

		}

		
		[Column(Name="SSN", UpdateCheck=UpdateCheck.Never, Storage="_Ssn", DbType="nvarchar(50)")]
		public string Ssn
		{
			get { return this._Ssn; }

			set
			{
				if (this._Ssn != value)
				{
				
                    this.OnSsnChanging(value);
					this.SendPropertyChanging();
					this._Ssn = value;
					this.SendPropertyChanged("Ssn");
					this.OnSsnChanged();
				}

			}

		}

		
		[Column(Name="DLN", UpdateCheck=UpdateCheck.Never, Storage="_Dln", DbType="nvarchar(75)")]
		public string Dln
		{
			get { return this._Dln; }

			set
			{
				if (this._Dln != value)
				{
				
                    this.OnDlnChanging(value);
					this.SendPropertyChanging();
					this._Dln = value;
					this.SendPropertyChanged("Dln");
					this.OnDlnChanged();
				}

			}

		}

		
		[Column(Name="DLStateID", UpdateCheck=UpdateCheck.Never, Storage="_DLStateID", DbType="int")]
		public int? DLStateID
		{
			get { return this._DLStateID; }

			set
			{
				if (this._DLStateID != value)
				{
				
                    this.OnDLStateIDChanging(value);
					this.SendPropertyChanging();
					this._DLStateID = value;
					this.SendPropertyChanged("DLStateID");
					this.OnDLStateIDChanged();
				}

			}

		}

		
		[Column(Name="HashNum", UpdateCheck=UpdateCheck.Never, Storage="_HashNum", DbType="int", IsDbGenerated=true)]
		public int? HashNum
		{
			get { return this._HashNum; }

			set
			{
				if (this._HashNum != value)
				{
				
                    this.OnHashNumChanging(value);
					this.SendPropertyChanging();
					this._HashNum = value;
					this.SendPropertyChanged("HashNum");
					this.OnHashNumChanged();
				}

			}

		}

		
		[Column(Name="PreferredName", UpdateCheck=UpdateCheck.Never, Storage="_PreferredName", DbType="nvarchar(25)", IsDbGenerated=true)]
		public string PreferredName
		{
			get { return this._PreferredName; }

			set
			{
				if (this._PreferredName != value)
				{
				
                    this.OnPreferredNameChanging(value);
					this.SendPropertyChanging();
					this._PreferredName = value;
					this.SendPropertyChanged("PreferredName");
					this.OnPreferredNameChanged();
				}

			}

		}

		
		[Column(Name="Name2", UpdateCheck=UpdateCheck.Never, Storage="_Name2", DbType="nvarchar(139)", IsDbGenerated=true)]
		public string Name2
		{
			get { return this._Name2; }

			set
			{
				if (this._Name2 != value)
				{
				
                    this.OnName2Changing(value);
					this.SendPropertyChanging();
					this._Name2 = value;
					this.SendPropertyChanged("Name2");
					this.OnName2Changed();
				}

			}

		}

		
		[Column(Name="Name", UpdateCheck=UpdateCheck.Never, Storage="_Name", DbType="nvarchar(138)", IsDbGenerated=true)]
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

		
		[Column(Name="ElectronicStatement", UpdateCheck=UpdateCheck.Never, Storage="_ElectronicStatement", DbType="bit")]
		public bool? ElectronicStatement
		{
			get { return this._ElectronicStatement; }

			set
			{
				if (this._ElectronicStatement != value)
				{
				
                    this.OnElectronicStatementChanging(value);
					this.SendPropertyChanging();
					this._ElectronicStatement = value;
					this.SendPropertyChanged("ElectronicStatement");
					this.OnElectronicStatementChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
   		[Association(Name="contactsHad__person", Storage="_contactsHad", OtherKey="PeopleId")]
   		public EntitySet<Contactee> contactsHad
   		{
   		    get { return this._contactsHad; }

			set	{ this._contactsHad.Assign(value); }

   		}

		
   		[Association(Name="contactsMade__person", Storage="_contactsMade", OtherKey="PeopleId")]
   		public EntitySet<Contactor> contactsMade
   		{
   		    get { return this._contactsMade; }

			set	{ this._contactsMade.Assign(value); }

   		}

		
   		[Association(Name="ENROLLMENT_TRANSACTION_PPL_FK", Storage="_EnrollmentTransactions", OtherKey="PeopleId")]
   		public EntitySet<EnrollmentTransaction> EnrollmentTransactions
   		{
   		    get { return this._EnrollmentTransactions; }

			set	{ this._EnrollmentTransactions.Assign(value); }

   		}

		
   		[Association(Name="FamiliesHeaded__HeadOfHousehold", Storage="_FamiliesHeaded", OtherKey="HeadOfHouseholdId")]
   		public EntitySet<Family> FamiliesHeaded
   		{
   		    get { return this._FamiliesHeaded; }

			set	{ this._FamiliesHeaded.Assign(value); }

   		}

		
   		[Association(Name="FamiliesHeaded2__HeadOfHouseholdSpouse", Storage="_FamiliesHeaded2", OtherKey="HeadOfHouseholdSpouseId")]
   		public EntitySet<Family> FamiliesHeaded2
   		{
   		    get { return this._FamiliesHeaded2; }

			set	{ this._FamiliesHeaded2.Assign(value); }

   		}

		
   		[Association(Name="FK_AttendWithAbsents_TBL_PEOPLE_TBL", Storage="_Attends", OtherKey="PeopleId")]
   		public EntitySet<Attend> Attends
   		{
   		    get { return this._Attends; }

			set	{ this._Attends.Assign(value); }

   		}

		
   		[Association(Name="FK_BackgroundChecks_People", Storage="_BackgroundChecks", OtherKey="PeopleID")]
   		public EntitySet<BackgroundCheck> BackgroundChecks
   		{
   		    get { return this._BackgroundChecks; }

			set	{ this._BackgroundChecks.Assign(value); }

   		}

		
   		[Association(Name="FK_CardIdentifiers_People", Storage="_CardIdentifiers", OtherKey="PeopleId")]
   		public EntitySet<CardIdentifier> CardIdentifiers
   		{
   		    get { return this._CardIdentifiers; }

			set	{ this._CardIdentifiers.Assign(value); }

   		}

		
   		[Association(Name="FK_CheckInTimes_People", Storage="_CheckInTimes", OtherKey="PeopleId")]
   		public EntitySet<CheckInTime> CheckInTimes
   		{
   		    get { return this._CheckInTimes; }

			set	{ this._CheckInTimes.Assign(value); }

   		}

		
   		[Association(Name="FK_Contribution_People", Storage="_Contributions", OtherKey="PeopleId")]
   		public EntitySet<Contribution> Contributions
   		{
   		    get { return this._Contributions; }

			set	{ this._Contributions.Assign(value); }

   		}

		
   		[Association(Name="FK_Coupons_People", Storage="_Coupons", OtherKey="PeopleId")]
   		public EntitySet<Coupon> Coupons
   		{
   		    get { return this._Coupons; }

			set	{ this._Coupons.Assign(value); }

   		}

		
   		[Association(Name="FK_EmailOptOut_People", Storage="_EmailOptOuts", OtherKey="ToPeopleId")]
   		public EntitySet<EmailOptOut> EmailOptOuts
   		{
   		    get { return this._EmailOptOuts; }

			set	{ this._EmailOptOuts.Assign(value); }

   		}

		
   		[Association(Name="FK_EmailQueue_People", Storage="_EmailQueues", OtherKey="QueuedBy")]
   		public EntitySet<EmailQueue> EmailQueues
   		{
   		    get { return this._EmailQueues; }

			set	{ this._EmailQueues.Assign(value); }

   		}

		
   		[Association(Name="FK_EmailQueueTo_People", Storage="_EmailQueueTos", OtherKey="PeopleId")]
   		public EntitySet<EmailQueueTo> EmailQueueTos
   		{
   		    get { return this._EmailQueueTos; }

			set	{ this._EmailQueueTos.Assign(value); }

   		}

		
   		[Association(Name="FK_EmailResponses_People", Storage="_EmailResponses", OtherKey="PeopleId")]
   		public EntitySet<EmailResponse> EmailResponses
   		{
   		    get { return this._EmailResponses; }

			set	{ this._EmailResponses.Assign(value); }

   		}

		
   		[Association(Name="FK_Goers__Supporter", Storage="_FK_Goers", OtherKey="SupporterId")]
   		public EntitySet<GoerSupporter> FK_Goers
   		{
   		    get { return this._FK_Goers; }

			set	{ this._FK_Goers.Assign(value); }

   		}

		
   		[Association(Name="FK_ManagedGiving_People", Storage="_ManagedGivings", OtherKey="PeopleId")]
   		public EntitySet<ManagedGiving> ManagedGivings
   		{
   		    get { return this._ManagedGivings; }

			set	{ this._ManagedGivings.Assign(value); }

   		}

		
   		[Association(Name="FK_MemberDocForm_PEOPLE_TBL", Storage="_MemberDocForms", OtherKey="PeopleId")]
   		public EntitySet<MemberDocForm> MemberDocForms
   		{
   		    get { return this._MemberDocForms; }

			set	{ this._MemberDocForms.Assign(value); }

   		}

		
   		[Association(Name="FK_MobileAppDevices_People", Storage="_MobileAppDevices", OtherKey="PeopleID")]
   		public EntitySet<MobileAppDevice> MobileAppDevices
   		{
   		    get { return this._MobileAppDevices; }

			set	{ this._MobileAppDevices.Assign(value); }

   		}

		
   		[Association(Name="FK_MobileAppPushRegistrations_People", Storage="_MobileAppPushRegistrations", OtherKey="PeopleId")]
   		public EntitySet<MobileAppPushRegistration> MobileAppPushRegistrations
   		{
   		    get { return this._MobileAppPushRegistrations; }

			set	{ this._MobileAppPushRegistrations.Assign(value); }

   		}

		
   		[Association(Name="FK_OrgMemberExtra_People", Storage="_OrgMemberExtras", OtherKey="PeopleId")]
   		public EntitySet<OrgMemberExtra> OrgMemberExtras
   		{
   		    get { return this._OrgMemberExtras; }

			set	{ this._OrgMemberExtras.Assign(value); }

   		}

		
   		[Association(Name="FK_PaymentInfo_People", Storage="_PaymentInfos", OtherKey="PeopleId")]
   		public EntitySet<PaymentInfo> PaymentInfos
   		{
   		    get { return this._PaymentInfos; }

			set	{ this._PaymentInfos.Assign(value); }

   		}

		
   		[Association(Name="FK_PeopleExtra_People", Storage="_PeopleExtras", OtherKey="PeopleId")]
   		public EntitySet<PeopleExtra> PeopleExtras
   		{
   		    get { return this._PeopleExtras; }

			set	{ this._PeopleExtras.Assign(value); }

   		}

		
   		[Association(Name="FK_PrevOrgMemberExtra_People]", Storage="_PrevOrgMemberExtras", OtherKey="PeopleId")]
   		public EntitySet<PrevOrgMemberExtra> PrevOrgMemberExtras
   		{
   		    get { return this._PrevOrgMemberExtras; }

			set	{ this._PrevOrgMemberExtras.Assign(value); }

   		}

		
   		[Association(Name="FK_RecReg_People", Storage="_RecRegs", OtherKey="PeopleId")]
   		public EntitySet<RecReg> RecRegs
   		{
   		    get { return this._RecRegs; }

			set	{ this._RecRegs.Assign(value); }

   		}

		
   		[Association(Name="FK_RecurringAmounts_People", Storage="_RecurringAmounts", OtherKey="PeopleId")]
   		public EntitySet<RecurringAmount> RecurringAmounts
   		{
   		    get { return this._RecurringAmounts; }

			set	{ this._RecurringAmounts.Assign(value); }

   		}

		
   		[Association(Name="FK_SMSItems_People", Storage="_SMSItems", OtherKey="PeopleID")]
   		public EntitySet<SMSItem> SMSItems
   		{
   		    get { return this._SMSItems; }

			set	{ this._SMSItems.Assign(value); }

   		}

		
   		[Association(Name="FK_SMSList_People", Storage="_SMSLists", OtherKey="SenderID")]
   		public EntitySet<SMSList> SMSLists
   		{
   		    get { return this._SMSLists; }

			set	{ this._SMSLists.Assign(value); }

   		}

		
   		[Association(Name="FK_Supporters__Goer", Storage="_FK_Supporters", OtherKey="GoerId")]
   		public EntitySet<GoerSupporter> FK_Supporters
   		{
   		    get { return this._FK_Supporters; }

			set	{ this._FK_Supporters.Assign(value); }

   		}

		
   		[Association(Name="FK_TagShare_People", Storage="_TagShares", OtherKey="PeopleId")]
   		public EntitySet<TagShare> TagShares
   		{
   		    get { return this._TagShares; }

			set	{ this._TagShares.Assign(value); }

   		}

		
   		[Association(Name="FK_TaskListOwners_PEOPLE_TBL", Storage="_TaskListOwners", OtherKey="PeopleId")]
   		public EntitySet<TaskListOwner> TaskListOwners
   		{
   		    get { return this._TaskListOwners; }

			set	{ this._TaskListOwners.Assign(value); }

   		}

		
   		[Association(Name="FK_Transaction_People", Storage="_Transactions", OtherKey="LoginPeopleId")]
   		public EntitySet<Transaction> Transactions
   		{
   		    get { return this._Transactions; }

			set	{ this._Transactions.Assign(value); }

   		}

		
   		[Association(Name="FK_TransactionPeople_Person", Storage="_TransactionPeople", OtherKey="PeopleId")]
   		public EntitySet<TransactionPerson> TransactionPeople
   		{
   		    get { return this._TransactionPeople; }

			set	{ this._TransactionPeople.Assign(value); }

   		}

		
   		[Association(Name="FK_Users_People", Storage="_Users", OtherKey="PeopleId")]
   		public EntitySet<User> Users
   		{
   		    get { return this._Users; }

			set	{ this._Users.Assign(value); }

   		}

		
   		[Association(Name="FK_VolInterestInterestCodes_People", Storage="_VolInterestInterestCodes", OtherKey="PeopleId")]
   		public EntitySet<VolInterestInterestCode> VolInterestInterestCodes
   		{
   		    get { return this._VolInterestInterestCodes; }

			set	{ this._VolInterestInterestCodes.Assign(value); }

   		}

		
   		[Association(Name="FK_Volunteer_PEOPLE_TBL", Storage="_Volunteers", OtherKey="PeopleId")]
   		public EntitySet<Volunteer> Volunteers
   		{
   		    get { return this._Volunteers; }

			set	{ this._Volunteers.Assign(value); }

   		}

		
   		[Association(Name="FK_VolunteerForm_PEOPLE_TBL", Storage="_VolunteerForms", OtherKey="PeopleId")]
   		public EntitySet<VolunteerForm> VolunteerForms
   		{
   		    get { return this._VolunteerForms; }

			set	{ this._VolunteerForms.Assign(value); }

   		}

		
   		[Association(Name="FK_VoluteerApprovalIds_People", Storage="_VoluteerApprovalIds", OtherKey="PeopleId")]
   		public EntitySet<VoluteerApprovalId> VoluteerApprovalIds
   		{
   		    get { return this._VoluteerApprovalIds; }

			set	{ this._VoluteerApprovalIds.Assign(value); }

   		}

		
   		[Association(Name="GoerAmounts__Goer", Storage="_GoerAmounts", OtherKey="SupporterId")]
   		public EntitySet<GoerSenderAmount> GoerAmounts
   		{
   		    get { return this._GoerAmounts; }

			set	{ this._GoerAmounts.Assign(value); }

   		}

		
   		[Association(Name="OnBehalfOfPeople__PersonCanEmail", Storage="_OnBehalfOfPeople", OtherKey="CanEmail")]
   		public EntitySet<PeopleCanEmailFor> OnBehalfOfPeople
   		{
   		    get { return this._OnBehalfOfPeople; }

			set	{ this._OnBehalfOfPeople.Assign(value); }

   		}

		
   		[Association(Name="ORGANIZATION_MEMBERS_PPL_FK", Storage="_OrganizationMembers", OtherKey="PeopleId")]
   		public EntitySet<OrganizationMember> OrganizationMembers
   		{
   		    get { return this._OrganizationMembers; }

			set	{ this._OrganizationMembers.Assign(value); }

   		}

		
   		[Association(Name="People__User", Storage="_People", OtherKey="UserID")]
   		public EntitySet<BackgroundCheck> People
   		{
   		    get { return this._People; }

			set	{ this._People.Assign(value); }

   		}

		
   		[Association(Name="PersonsCanEmail__OnBehalfOfPerson", Storage="_PersonsCanEmail", OtherKey="OnBehalfOf")]
   		public EntitySet<PeopleCanEmailFor> PersonsCanEmail
   		{
   		    get { return this._PersonsCanEmail; }

			set	{ this._PersonsCanEmail.Assign(value); }

   		}

		
   		[Association(Name="SenderAmounts__Sender", Storage="_SenderAmounts", OtherKey="GoerId")]
   		public EntitySet<GoerSenderAmount> SenderAmounts
   		{
   		    get { return this._SenderAmounts; }

			set	{ this._SenderAmounts.Assign(value); }

   		}

		
   		[Association(Name="SubRequests__Requestor", Storage="_SubRequests", OtherKey="RequestorId")]
   		public EntitySet<SubRequest> SubRequests
   		{
   		    get { return this._SubRequests; }

			set	{ this._SubRequests.Assign(value); }

   		}

		
   		[Association(Name="SubResponses__Substitute", Storage="_SubResponses", OtherKey="SubstituteId")]
   		public EntitySet<SubRequest> SubResponses
   		{
   		    get { return this._SubResponses; }

			set	{ this._SubResponses.Assign(value); }

   		}

		
   		[Association(Name="Tags__Person", Storage="_Tags", OtherKey="PeopleId")]
   		public EntitySet<TagPerson> Tags
   		{
   		    get { return this._Tags; }

			set	{ this._Tags.Assign(value); }

   		}

		
   		[Association(Name="TagsOwned__PersonOwner", Storage="_TagsOwned", OtherKey="PeopleId")]
   		public EntitySet<Tag> TagsOwned
   		{
   		    get { return this._TagsOwned; }

			set	{ this._TagsOwned.Assign(value); }

   		}

		
   		[Association(Name="Tasks__Owner", Storage="_Tasks", OtherKey="OwnerId")]
   		public EntitySet<Task> Tasks
   		{
   		    get { return this._Tasks; }

			set	{ this._Tasks.Assign(value); }

   		}

		
   		[Association(Name="TasksAboutPerson__AboutWho", Storage="_TasksAboutPerson", OtherKey="WhoId")]
   		public EntitySet<Task> TasksAboutPerson
   		{
   		    get { return this._TasksAboutPerson; }

			set	{ this._TasksAboutPerson.Assign(value); }

   		}

		
   		[Association(Name="TasksCoOwned__CoOwner", Storage="_TasksCoOwned", OtherKey="CoOwnerId")]
   		public EntitySet<Task> TasksCoOwned
   		{
   		    get { return this._TasksCoOwned; }

			set	{ this._TasksCoOwned.Assign(value); }

   		}

		
   		[Association(Name="VolRequests__Requestor", Storage="_VolRequests", OtherKey="RequestorId")]
   		public EntitySet<VolRequest> VolRequests
   		{
   		    get { return this._VolRequests; }

			set	{ this._VolRequests.Assign(value); }

   		}

		
   		[Association(Name="VolResponses__Volunteer", Storage="_VolResponses", OtherKey="VolunteerId")]
   		public EntitySet<VolRequest> VolResponses
   		{
   		    get { return this._VolResponses; }

			set	{ this._VolResponses.Assign(value); }

   		}

		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="BFMembers__BFClass", Storage="_BFClass", ThisKey="BibleFellowshipClassId", IsForeignKey=true)]
		public Organization BFClass
		{
			get { return this._BFClass.Entity; }

			set
			{
				Organization previousValue = this._BFClass.Entity;
				if (((previousValue != value) 
							|| (this._BFClass.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._BFClass.Entity = null;
						previousValue.BFMembers.Remove(this);
					}

					this._BFClass.Entity = value;
					if (value != null)
					{
						value.BFMembers.Add(this);
						
						this._BibleFellowshipClassId = value.OrganizationId;
						
					}

					else
					{
						
						this._BibleFellowshipClassId = default(int?);
						
					}

					this.SendPropertyChanged("BFClass");
				}

			}

		}

		
		[Association(Name="EnvPeople__EnvelopeOption", Storage="_EnvelopeOption", ThisKey="EnvelopeOptionsId", IsForeignKey=true)]
		public EnvelopeOption EnvelopeOption
		{
			get { return this._EnvelopeOption.Entity; }

			set
			{
				EnvelopeOption previousValue = this._EnvelopeOption.Entity;
				if (((previousValue != value) 
							|| (this._EnvelopeOption.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._EnvelopeOption.Entity = null;
						previousValue.EnvPeople.Remove(this);
					}

					this._EnvelopeOption.Entity = value;
					if (value != null)
					{
						value.EnvPeople.Add(this);
						
						this._EnvelopeOptionsId = value.Id;
						
					}

					else
					{
						
						this._EnvelopeOptionsId = default(int?);
						
					}

					this.SendPropertyChanged("EnvelopeOption");
				}

			}

		}

		
		[Association(Name="FK_People_BaptismStatus", Storage="_BaptismStatus", ThisKey="BaptismStatusId", IsForeignKey=true)]
		public BaptismStatus BaptismStatus
		{
			get { return this._BaptismStatus.Entity; }

			set
			{
				BaptismStatus previousValue = this._BaptismStatus.Entity;
				if (((previousValue != value) 
							|| (this._BaptismStatus.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._BaptismStatus.Entity = null;
						previousValue.People.Remove(this);
					}

					this._BaptismStatus.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._BaptismStatusId = value.Id;
						
					}

					else
					{
						
						this._BaptismStatusId = default(int?);
						
					}

					this.SendPropertyChanged("BaptismStatus");
				}

			}

		}

		
		[Association(Name="FK_People_BaptismType", Storage="_BaptismType", ThisKey="BaptismTypeId", IsForeignKey=true)]
		public BaptismType BaptismType
		{
			get { return this._BaptismType.Entity; }

			set
			{
				BaptismType previousValue = this._BaptismType.Entity;
				if (((previousValue != value) 
							|| (this._BaptismType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._BaptismType.Entity = null;
						previousValue.People.Remove(this);
					}

					this._BaptismType.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._BaptismTypeId = value.Id;
						
					}

					else
					{
						
						this._BaptismTypeId = default(int?);
						
					}

					this.SendPropertyChanged("BaptismType");
				}

			}

		}

		
		[Association(Name="FK_People_Campus", Storage="_Campu", ThisKey="CampusId", IsForeignKey=true)]
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
						previousValue.People.Remove(this);
					}

					this._Campu.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
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

		
		[Association(Name="FK_People_DecisionType", Storage="_DecisionType", ThisKey="DecisionTypeId", IsForeignKey=true)]
		public DecisionType DecisionType
		{
			get { return this._DecisionType.Entity; }

			set
			{
				DecisionType previousValue = this._DecisionType.Entity;
				if (((previousValue != value) 
							|| (this._DecisionType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._DecisionType.Entity = null;
						previousValue.People.Remove(this);
					}

					this._DecisionType.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._DecisionTypeId = value.Id;
						
					}

					else
					{
						
						this._DecisionTypeId = default(int?);
						
					}

					this.SendPropertyChanged("DecisionType");
				}

			}

		}

		
		[Association(Name="FK_People_DiscoveryClassStatus", Storage="_NewMemberClassStatus", ThisKey="NewMemberClassStatusId", IsForeignKey=true)]
		public NewMemberClassStatus NewMemberClassStatus
		{
			get { return this._NewMemberClassStatus.Entity; }

			set
			{
				NewMemberClassStatus previousValue = this._NewMemberClassStatus.Entity;
				if (((previousValue != value) 
							|| (this._NewMemberClassStatus.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._NewMemberClassStatus.Entity = null;
						previousValue.People.Remove(this);
					}

					this._NewMemberClassStatus.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._NewMemberClassStatusId = value.Id;
						
					}

					else
					{
						
						this._NewMemberClassStatusId = default(int?);
						
					}

					this.SendPropertyChanged("NewMemberClassStatus");
				}

			}

		}

		
		[Association(Name="FK_People_DropType", Storage="_DropType", ThisKey="DropCodeId", IsForeignKey=true)]
		public DropType DropType
		{
			get { return this._DropType.Entity; }

			set
			{
				DropType previousValue = this._DropType.Entity;
				if (((previousValue != value) 
							|| (this._DropType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._DropType.Entity = null;
						previousValue.People.Remove(this);
					}

					this._DropType.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._DropCodeId = value.Id;
						
					}

					else
					{
						
						this._DropCodeId = default(int);
						
					}

					this.SendPropertyChanged("DropType");
				}

			}

		}

		
		[Association(Name="FK_People_EntryPoint", Storage="_EntryPoint", ThisKey="EntryPointId", IsForeignKey=true)]
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
						previousValue.People.Remove(this);
					}

					this._EntryPoint.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
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

		
		[Association(Name="FK_People_Families", Storage="_Family", ThisKey="FamilyId", IsForeignKey=true)]
		public Family Family
		{
			get { return this._Family.Entity; }

			set
			{
				Family previousValue = this._Family.Entity;
				if (((previousValue != value) 
							|| (this._Family.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Family.Entity = null;
						previousValue.People.Remove(this);
					}

					this._Family.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._FamilyId = value.FamilyId;
						
					}

					else
					{
						
						this._FamilyId = default(int);
						
					}

					this.SendPropertyChanged("Family");
				}

			}

		}

		
		[Association(Name="FK_People_FamilyPosition", Storage="_FamilyPosition", ThisKey="PositionInFamilyId", IsForeignKey=true)]
		public FamilyPosition FamilyPosition
		{
			get { return this._FamilyPosition.Entity; }

			set
			{
				FamilyPosition previousValue = this._FamilyPosition.Entity;
				if (((previousValue != value) 
							|| (this._FamilyPosition.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._FamilyPosition.Entity = null;
						previousValue.People.Remove(this);
					}

					this._FamilyPosition.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._PositionInFamilyId = value.Id;
						
					}

					else
					{
						
						this._PositionInFamilyId = default(int);
						
					}

					this.SendPropertyChanged("FamilyPosition");
				}

			}

		}

		
		[Association(Name="FK_People_Gender", Storage="_Gender", ThisKey="GenderId", IsForeignKey=true)]
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
						previousValue.People.Remove(this);
					}

					this._Gender.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._GenderId = value.Id;
						
					}

					else
					{
						
						this._GenderId = default(int);
						
					}

					this.SendPropertyChanged("Gender");
				}

			}

		}

		
		[Association(Name="FK_People_InterestPoint", Storage="_InterestPoint", ThisKey="InterestPointId", IsForeignKey=true)]
		public InterestPoint InterestPoint
		{
			get { return this._InterestPoint.Entity; }

			set
			{
				InterestPoint previousValue = this._InterestPoint.Entity;
				if (((previousValue != value) 
							|| (this._InterestPoint.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._InterestPoint.Entity = null;
						previousValue.People.Remove(this);
					}

					this._InterestPoint.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._InterestPointId = value.Id;
						
					}

					else
					{
						
						this._InterestPointId = default(int?);
						
					}

					this.SendPropertyChanged("InterestPoint");
				}

			}

		}

		
		[Association(Name="FK_People_JoinType", Storage="_JoinType", ThisKey="JoinCodeId", IsForeignKey=true)]
		public JoinType JoinType
		{
			get { return this._JoinType.Entity; }

			set
			{
				JoinType previousValue = this._JoinType.Entity;
				if (((previousValue != value) 
							|| (this._JoinType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._JoinType.Entity = null;
						previousValue.People.Remove(this);
					}

					this._JoinType.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._JoinCodeId = value.Id;
						
					}

					else
					{
						
						this._JoinCodeId = default(int);
						
					}

					this.SendPropertyChanged("JoinType");
				}

			}

		}

		
		[Association(Name="FK_People_MaritalStatus", Storage="_MaritalStatus", ThisKey="MaritalStatusId", IsForeignKey=true)]
		public MaritalStatus MaritalStatus
		{
			get { return this._MaritalStatus.Entity; }

			set
			{
				MaritalStatus previousValue = this._MaritalStatus.Entity;
				if (((previousValue != value) 
							|| (this._MaritalStatus.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._MaritalStatus.Entity = null;
						previousValue.People.Remove(this);
					}

					this._MaritalStatus.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._MaritalStatusId = value.Id;
						
					}

					else
					{
						
						this._MaritalStatusId = default(int);
						
					}

					this.SendPropertyChanged("MaritalStatus");
				}

			}

		}

		
		[Association(Name="FK_People_MemberLetterStatus", Storage="_MemberLetterStatus", ThisKey="LetterStatusId", IsForeignKey=true)]
		public MemberLetterStatus MemberLetterStatus
		{
			get { return this._MemberLetterStatus.Entity; }

			set
			{
				MemberLetterStatus previousValue = this._MemberLetterStatus.Entity;
				if (((previousValue != value) 
							|| (this._MemberLetterStatus.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._MemberLetterStatus.Entity = null;
						previousValue.People.Remove(this);
					}

					this._MemberLetterStatus.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._LetterStatusId = value.Id;
						
					}

					else
					{
						
						this._LetterStatusId = default(int?);
						
					}

					this.SendPropertyChanged("MemberLetterStatus");
				}

			}

		}

		
		[Association(Name="FK_People_MemberStatus", Storage="_MemberStatus", ThisKey="MemberStatusId", IsForeignKey=true)]
		public MemberStatus MemberStatus
		{
			get { return this._MemberStatus.Entity; }

			set
			{
				MemberStatus previousValue = this._MemberStatus.Entity;
				if (((previousValue != value) 
							|| (this._MemberStatus.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._MemberStatus.Entity = null;
						previousValue.People.Remove(this);
					}

					this._MemberStatus.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._MemberStatusId = value.Id;
						
					}

					else
					{
						
						this._MemberStatusId = default(int);
						
					}

					this.SendPropertyChanged("MemberStatus");
				}

			}

		}

		
		[Association(Name="FK_People_Origin", Storage="_Origin", ThisKey="OriginId", IsForeignKey=true)]
		public Origin Origin
		{
			get { return this._Origin.Entity; }

			set
			{
				Origin previousValue = this._Origin.Entity;
				if (((previousValue != value) 
							|| (this._Origin.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Origin.Entity = null;
						previousValue.People.Remove(this);
					}

					this._Origin.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._OriginId = value.Id;
						
					}

					else
					{
						
						this._OriginId = default(int?);
						
					}

					this.SendPropertyChanged("Origin");
				}

			}

		}

		
		[Association(Name="FK_PEOPLE_TBL_Picture", Storage="_Picture", ThisKey="PictureId", IsForeignKey=true)]
		public Picture Picture
		{
			get { return this._Picture.Entity; }

			set
			{
				Picture previousValue = this._Picture.Entity;
				if (((previousValue != value) 
							|| (this._Picture.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Picture.Entity = null;
						previousValue.People.Remove(this);
					}

					this._Picture.Entity = value;
					if (value != null)
					{
						value.People.Add(this);
						
						this._PictureId = value.PictureId;
						
					}

					else
					{
						
						this._PictureId = default(int?);
						
					}

					this.SendPropertyChanged("Picture");
				}

			}

		}

		
		[Association(Name="ResCodePeople__ResidentCode", Storage="_ResidentCode", ThisKey="ResCodeId", IsForeignKey=true)]
		public ResidentCode ResidentCode
		{
			get { return this._ResidentCode.Entity; }

			set
			{
				ResidentCode previousValue = this._ResidentCode.Entity;
				if (((previousValue != value) 
							|| (this._ResidentCode.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ResidentCode.Entity = null;
						previousValue.ResCodePeople.Remove(this);
					}

					this._ResidentCode.Entity = value;
					if (value != null)
					{
						value.ResCodePeople.Add(this);
						
						this._ResCodeId = value.Id;
						
					}

					else
					{
						
						this._ResCodeId = default(int?);
						
					}

					this.SendPropertyChanged("ResidentCode");
				}

			}

		}

		
		[Association(Name="StmtPeople__ContributionStatementOption", Storage="_ContributionStatementOption", ThisKey="ContributionOptionsId", IsForeignKey=true)]
		public EnvelopeOption ContributionStatementOption
		{
			get { return this._ContributionStatementOption.Entity; }

			set
			{
				EnvelopeOption previousValue = this._ContributionStatementOption.Entity;
				if (((previousValue != value) 
							|| (this._ContributionStatementOption.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._ContributionStatementOption.Entity = null;
						previousValue.StmtPeople.Remove(this);
					}

					this._ContributionStatementOption.Entity = value;
					if (value != null)
					{
						value.StmtPeople.Add(this);
						
						this._ContributionOptionsId = value.Id;
						
					}

					else
					{
						
						this._ContributionOptionsId = default(int?);
						
					}

					this.SendPropertyChanged("ContributionStatementOption");
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

   		
		private void attach_contactsHad(Contactee entity)
		{
			this.SendPropertyChanging();
			entity.person = this;
		}

		private void detach_contactsHad(Contactee entity)
		{
			this.SendPropertyChanging();
			entity.person = null;
		}

		
		private void attach_contactsMade(Contactor entity)
		{
			this.SendPropertyChanging();
			entity.person = this;
		}

		private void detach_contactsMade(Contactor entity)
		{
			this.SendPropertyChanging();
			entity.person = null;
		}

		
		private void attach_EnrollmentTransactions(EnrollmentTransaction entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_EnrollmentTransactions(EnrollmentTransaction entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_FamiliesHeaded(Family entity)
		{
			this.SendPropertyChanging();
			entity.HeadOfHousehold = this;
		}

		private void detach_FamiliesHeaded(Family entity)
		{
			this.SendPropertyChanging();
			entity.HeadOfHousehold = null;
		}

		
		private void attach_FamiliesHeaded2(Family entity)
		{
			this.SendPropertyChanging();
			entity.HeadOfHouseholdSpouse = this;
		}

		private void detach_FamiliesHeaded2(Family entity)
		{
			this.SendPropertyChanging();
			entity.HeadOfHouseholdSpouse = null;
		}

		
		private void attach_Attends(Attend entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_Attends(Attend entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_BackgroundChecks(BackgroundCheck entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_BackgroundChecks(BackgroundCheck entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_CardIdentifiers(CardIdentifier entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_CardIdentifiers(CardIdentifier entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_CheckInTimes(CheckInTime entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_CheckInTimes(CheckInTime entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_Contributions(Contribution entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_Contributions(Contribution entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_Coupons(Coupon entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_Coupons(Coupon entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_EmailOptOuts(EmailOptOut entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_EmailOptOuts(EmailOptOut entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_EmailQueues(EmailQueue entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_EmailQueues(EmailQueue entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_EmailQueueTos(EmailQueueTo entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_EmailQueueTos(EmailQueueTo entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_EmailResponses(EmailResponse entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_EmailResponses(EmailResponse entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_FK_Goers(GoerSupporter entity)
		{
			this.SendPropertyChanging();
			entity.Supporter = this;
		}

		private void detach_FK_Goers(GoerSupporter entity)
		{
			this.SendPropertyChanging();
			entity.Supporter = null;
		}

		
		private void attach_ManagedGivings(ManagedGiving entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_ManagedGivings(ManagedGiving entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_MemberDocForms(MemberDocForm entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_MemberDocForms(MemberDocForm entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_MobileAppDevices(MobileAppDevice entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_MobileAppDevices(MobileAppDevice entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_MobileAppPushRegistrations(MobileAppPushRegistration entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_MobileAppPushRegistrations(MobileAppPushRegistration entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_OrgMemberExtras(OrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_OrgMemberExtras(OrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_PaymentInfos(PaymentInfo entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_PaymentInfos(PaymentInfo entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_PeopleExtras(PeopleExtra entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_PeopleExtras(PeopleExtra entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_RecRegs(RecReg entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_RecRegs(RecReg entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_RecurringAmounts(RecurringAmount entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_RecurringAmounts(RecurringAmount entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_SMSItems(SMSItem entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_SMSItems(SMSItem entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_SMSLists(SMSList entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_SMSLists(SMSList entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_FK_Supporters(GoerSupporter entity)
		{
			this.SendPropertyChanging();
			entity.Goer = this;
		}

		private void detach_FK_Supporters(GoerSupporter entity)
		{
			this.SendPropertyChanging();
			entity.Goer = null;
		}

		
		private void attach_TagShares(TagShare entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_TagShares(TagShare entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_TaskListOwners(TaskListOwner entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_TaskListOwners(TaskListOwner entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_Transactions(Transaction entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_Transactions(Transaction entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_TransactionPeople(TransactionPerson entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_TransactionPeople(TransactionPerson entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_Users(User entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_Users(User entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_VolInterestInterestCodes(VolInterestInterestCode entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_VolInterestInterestCodes(VolInterestInterestCode entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_Volunteers(Volunteer entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_Volunteers(Volunteer entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_VolunteerForms(VolunteerForm entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_VolunteerForms(VolunteerForm entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_VoluteerApprovalIds(VoluteerApprovalId entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_VoluteerApprovalIds(VoluteerApprovalId entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_GoerAmounts(GoerSenderAmount entity)
		{
			this.SendPropertyChanging();
			entity.Goer = this;
		}

		private void detach_GoerAmounts(GoerSenderAmount entity)
		{
			this.SendPropertyChanging();
			entity.Goer = null;
		}

		
		private void attach_OnBehalfOfPeople(PeopleCanEmailFor entity)
		{
			this.SendPropertyChanging();
			entity.PersonCanEmail = this;
		}

		private void detach_OnBehalfOfPeople(PeopleCanEmailFor entity)
		{
			this.SendPropertyChanging();
			entity.PersonCanEmail = null;
		}

		
		private void attach_OrganizationMembers(OrganizationMember entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_OrganizationMembers(OrganizationMember entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_People(BackgroundCheck entity)
		{
			this.SendPropertyChanging();
			entity.User = this;
		}

		private void detach_People(BackgroundCheck entity)
		{
			this.SendPropertyChanging();
			entity.User = null;
		}

		
		private void attach_PersonsCanEmail(PeopleCanEmailFor entity)
		{
			this.SendPropertyChanging();
			entity.OnBehalfOfPerson = this;
		}

		private void detach_PersonsCanEmail(PeopleCanEmailFor entity)
		{
			this.SendPropertyChanging();
			entity.OnBehalfOfPerson = null;
		}

		
		private void attach_SenderAmounts(GoerSenderAmount entity)
		{
			this.SendPropertyChanging();
			entity.Sender = this;
		}

		private void detach_SenderAmounts(GoerSenderAmount entity)
		{
			this.SendPropertyChanging();
			entity.Sender = null;
		}

		
		private void attach_SubRequests(SubRequest entity)
		{
			this.SendPropertyChanging();
			entity.Requestor = this;
		}

		private void detach_SubRequests(SubRequest entity)
		{
			this.SendPropertyChanging();
			entity.Requestor = null;
		}

		
		private void attach_SubResponses(SubRequest entity)
		{
			this.SendPropertyChanging();
			entity.Substitute = this;
		}

		private void detach_SubResponses(SubRequest entity)
		{
			this.SendPropertyChanging();
			entity.Substitute = null;
		}

		
		private void attach_Tags(TagPerson entity)
		{
			this.SendPropertyChanging();
			entity.Person = this;
		}

		private void detach_Tags(TagPerson entity)
		{
			this.SendPropertyChanging();
			entity.Person = null;
		}

		
		private void attach_TagsOwned(Tag entity)
		{
			this.SendPropertyChanging();
			entity.PersonOwner = this;
		}

		private void detach_TagsOwned(Tag entity)
		{
			this.SendPropertyChanging();
			entity.PersonOwner = null;
		}

		
		private void attach_Tasks(Task entity)
		{
			this.SendPropertyChanging();
			entity.Owner = this;
		}

		private void detach_Tasks(Task entity)
		{
			this.SendPropertyChanging();
			entity.Owner = null;
		}

		
		private void attach_TasksAboutPerson(Task entity)
		{
			this.SendPropertyChanging();
			entity.AboutWho = this;
		}

		private void detach_TasksAboutPerson(Task entity)
		{
			this.SendPropertyChanging();
			entity.AboutWho = null;
		}

		
		private void attach_TasksCoOwned(Task entity)
		{
			this.SendPropertyChanging();
			entity.CoOwner = this;
		}

		private void detach_TasksCoOwned(Task entity)
		{
			this.SendPropertyChanging();
			entity.CoOwner = null;
		}

		
		private void attach_VolRequests(VolRequest entity)
		{
			this.SendPropertyChanging();
			entity.Requestor = this;
		}

		private void detach_VolRequests(VolRequest entity)
		{
			this.SendPropertyChanging();
			entity.Requestor = null;
		}

		
		private void attach_VolResponses(VolRequest entity)
		{
			this.SendPropertyChanging();
			entity.Volunteer = this;
		}

		private void detach_VolResponses(VolRequest entity)
		{
			this.SendPropertyChanging();
			entity.Volunteer = null;
		}

		
	}

}

