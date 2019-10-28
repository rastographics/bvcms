using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.People")]
    public partial class Person : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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

        private EntitySet<OrgMemberDocuments> _OrgMemberDocuments;

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
            _contactsHad = new EntitySet<Contactee>(new Action<Contactee>(attach_contactsHad), new Action<Contactee>(detach_contactsHad));

            _contactsMade = new EntitySet<Contactor>(new Action<Contactor>(attach_contactsMade), new Action<Contactor>(detach_contactsMade));

            _EnrollmentTransactions = new EntitySet<EnrollmentTransaction>(new Action<EnrollmentTransaction>(attach_EnrollmentTransactions), new Action<EnrollmentTransaction>(detach_EnrollmentTransactions));

            _FamiliesHeaded = new EntitySet<Family>(new Action<Family>(attach_FamiliesHeaded), new Action<Family>(detach_FamiliesHeaded));

            _FamiliesHeaded2 = new EntitySet<Family>(new Action<Family>(attach_FamiliesHeaded2), new Action<Family>(detach_FamiliesHeaded2));

            _Attends = new EntitySet<Attend>(new Action<Attend>(attach_Attends), new Action<Attend>(detach_Attends));

            _BackgroundChecks = new EntitySet<BackgroundCheck>(new Action<BackgroundCheck>(attach_BackgroundChecks), new Action<BackgroundCheck>(detach_BackgroundChecks));

            _CardIdentifiers = new EntitySet<CardIdentifier>(new Action<CardIdentifier>(attach_CardIdentifiers), new Action<CardIdentifier>(detach_CardIdentifiers));

            _CheckInTimes = new EntitySet<CheckInTime>(new Action<CheckInTime>(attach_CheckInTimes), new Action<CheckInTime>(detach_CheckInTimes));

            _Contributions = new EntitySet<Contribution>(new Action<Contribution>(attach_Contributions), new Action<Contribution>(detach_Contributions));

            _Coupons = new EntitySet<Coupon>(new Action<Coupon>(attach_Coupons), new Action<Coupon>(detach_Coupons));

            _EmailOptOuts = new EntitySet<EmailOptOut>(new Action<EmailOptOut>(attach_EmailOptOuts), new Action<EmailOptOut>(detach_EmailOptOuts));

            _EmailQueues = new EntitySet<EmailQueue>(new Action<EmailQueue>(attach_EmailQueues), new Action<EmailQueue>(detach_EmailQueues));

            _EmailQueueTos = new EntitySet<EmailQueueTo>(new Action<EmailQueueTo>(attach_EmailQueueTos), new Action<EmailQueueTo>(detach_EmailQueueTos));

            _EmailResponses = new EntitySet<EmailResponse>(new Action<EmailResponse>(attach_EmailResponses), new Action<EmailResponse>(detach_EmailResponses));

            _FK_Goers = new EntitySet<GoerSupporter>(new Action<GoerSupporter>(attach_FK_Goers), new Action<GoerSupporter>(detach_FK_Goers));

            _ManagedGivings = new EntitySet<ManagedGiving>(new Action<ManagedGiving>(attach_ManagedGivings), new Action<ManagedGiving>(detach_ManagedGivings));

            _MemberDocForms = new EntitySet<MemberDocForm>(new Action<MemberDocForm>(attach_MemberDocForms), new Action<MemberDocForm>(detach_MemberDocForms));

            _MobileAppDevices = new EntitySet<MobileAppDevice>(new Action<MobileAppDevice>(attach_MobileAppDevices), new Action<MobileAppDevice>(detach_MobileAppDevices));

            _MobileAppPushRegistrations = new EntitySet<MobileAppPushRegistration>(new Action<MobileAppPushRegistration>(attach_MobileAppPushRegistrations), new Action<MobileAppPushRegistration>(detach_MobileAppPushRegistrations));

            _OrgMemberExtras = new EntitySet<OrgMemberExtra>(new Action<OrgMemberExtra>(attach_OrgMemberExtras), new Action<OrgMemberExtra>(detach_OrgMemberExtras));

            _PaymentInfos = new EntitySet<PaymentInfo>(new Action<PaymentInfo>(attach_PaymentInfos), new Action<PaymentInfo>(detach_PaymentInfos));

            _PeopleExtras = new EntitySet<PeopleExtra>(new Action<PeopleExtra>(attach_PeopleExtras), new Action<PeopleExtra>(detach_PeopleExtras));

            _PrevOrgMemberExtras = new EntitySet<PrevOrgMemberExtra>(new Action<PrevOrgMemberExtra>(attach_PrevOrgMemberExtras), new Action<PrevOrgMemberExtra>(detach_PrevOrgMemberExtras));

            _RecRegs = new EntitySet<RecReg>(new Action<RecReg>(attach_RecRegs), new Action<RecReg>(detach_RecRegs));

            _RecurringAmounts = new EntitySet<RecurringAmount>(new Action<RecurringAmount>(attach_RecurringAmounts), new Action<RecurringAmount>(detach_RecurringAmounts));

            _SMSItems = new EntitySet<SMSItem>(new Action<SMSItem>(attach_SMSItems), new Action<SMSItem>(detach_SMSItems));

            _SMSLists = new EntitySet<SMSList>(new Action<SMSList>(attach_SMSLists), new Action<SMSList>(detach_SMSLists));

            _FK_Supporters = new EntitySet<GoerSupporter>(new Action<GoerSupporter>(attach_FK_Supporters), new Action<GoerSupporter>(detach_FK_Supporters));

            _TagShares = new EntitySet<TagShare>(new Action<TagShare>(attach_TagShares), new Action<TagShare>(detach_TagShares));

            _TaskListOwners = new EntitySet<TaskListOwner>(new Action<TaskListOwner>(attach_TaskListOwners), new Action<TaskListOwner>(detach_TaskListOwners));

            _Transactions = new EntitySet<Transaction>(new Action<Transaction>(attach_Transactions), new Action<Transaction>(detach_Transactions));

            _TransactionPeople = new EntitySet<TransactionPerson>(new Action<TransactionPerson>(attach_TransactionPeople), new Action<TransactionPerson>(detach_TransactionPeople));

            _Users = new EntitySet<User>(new Action<User>(attach_Users), new Action<User>(detach_Users));

            _VolInterestInterestCodes = new EntitySet<VolInterestInterestCode>(new Action<VolInterestInterestCode>(attach_VolInterestInterestCodes), new Action<VolInterestInterestCode>(detach_VolInterestInterestCodes));

            _Volunteers = new EntitySet<Volunteer>(new Action<Volunteer>(attach_Volunteers), new Action<Volunteer>(detach_Volunteers));

            _VolunteerForms = new EntitySet<VolunteerForm>(new Action<VolunteerForm>(attach_VolunteerForms), new Action<VolunteerForm>(detach_VolunteerForms));

            _VoluteerApprovalIds = new EntitySet<VoluteerApprovalId>(new Action<VoluteerApprovalId>(attach_VoluteerApprovalIds), new Action<VoluteerApprovalId>(detach_VoluteerApprovalIds));

            _GoerAmounts = new EntitySet<GoerSenderAmount>(new Action<GoerSenderAmount>(attach_GoerAmounts), new Action<GoerSenderAmount>(detach_GoerAmounts));

            _OnBehalfOfPeople = new EntitySet<PeopleCanEmailFor>(new Action<PeopleCanEmailFor>(attach_OnBehalfOfPeople), new Action<PeopleCanEmailFor>(detach_OnBehalfOfPeople));

            _OrganizationMembers = new EntitySet<OrganizationMember>(new Action<OrganizationMember>(attach_OrganizationMembers), new Action<OrganizationMember>(detach_OrganizationMembers));

            _People = new EntitySet<BackgroundCheck>(new Action<BackgroundCheck>(attach_People), new Action<BackgroundCheck>(detach_People));

            _PersonsCanEmail = new EntitySet<PeopleCanEmailFor>(new Action<PeopleCanEmailFor>(attach_PersonsCanEmail), new Action<PeopleCanEmailFor>(detach_PersonsCanEmail));

            _SenderAmounts = new EntitySet<GoerSenderAmount>(new Action<GoerSenderAmount>(attach_SenderAmounts), new Action<GoerSenderAmount>(detach_SenderAmounts));

            _SubRequests = new EntitySet<SubRequest>(new Action<SubRequest>(attach_SubRequests), new Action<SubRequest>(detach_SubRequests));

            _SubResponses = new EntitySet<SubRequest>(new Action<SubRequest>(attach_SubResponses), new Action<SubRequest>(detach_SubResponses));

            _Tags = new EntitySet<TagPerson>(new Action<TagPerson>(attach_Tags), new Action<TagPerson>(detach_Tags));

            _TagsOwned = new EntitySet<Tag>(new Action<Tag>(attach_TagsOwned), new Action<Tag>(detach_TagsOwned));

            _Tasks = new EntitySet<Task>(new Action<Task>(attach_Tasks), new Action<Task>(detach_Tasks));

            _TasksAboutPerson = new EntitySet<Task>(new Action<Task>(attach_TasksAboutPerson), new Action<Task>(detach_TasksAboutPerson));

            _TasksCoOwned = new EntitySet<Task>(new Action<Task>(attach_TasksCoOwned), new Action<Task>(detach_TasksCoOwned));

            _VolRequests = new EntitySet<VolRequest>(new Action<VolRequest>(attach_VolRequests), new Action<VolRequest>(detach_VolRequests));

            _VolResponses = new EntitySet<VolRequest>(new Action<VolRequest>(attach_VolResponses), new Action<VolRequest>(detach_VolResponses));

            _OrgMemberDocuments = new EntitySet<OrgMemberDocuments>(new Action<OrgMemberDocuments>(attach_OrgMemberDocuments), new Action<OrgMemberDocuments>(detach_OrgMemberDocuments));

            _BFClass = default(EntityRef<Organization>);

            _EnvelopeOption = default(EntityRef<EnvelopeOption>);

            _BaptismStatus = default(EntityRef<BaptismStatus>);

            _BaptismType = default(EntityRef<BaptismType>);

            _Campu = default(EntityRef<Campu>);

            _DecisionType = default(EntityRef<DecisionType>);

            _NewMemberClassStatus = default(EntityRef<NewMemberClassStatus>);

            _DropType = default(EntityRef<DropType>);

            _EntryPoint = default(EntityRef<EntryPoint>);

            _Family = default(EntityRef<Family>);

            _FamilyPosition = default(EntityRef<FamilyPosition>);

            _Gender = default(EntityRef<Gender>);

            _InterestPoint = default(EntityRef<InterestPoint>);

            _JoinType = default(EntityRef<JoinType>);

            _MaritalStatus = default(EntityRef<MaritalStatus>);

            _MemberLetterStatus = default(EntityRef<MemberLetterStatus>);

            _MemberStatus = default(EntityRef<MemberStatus>);

            _Origin = default(EntityRef<Origin>);

            _Picture = default(EntityRef<Picture>);

            _ResidentCode = default(EntityRef<ResidentCode>);

            _ContributionStatementOption = default(EntityRef<EnvelopeOption>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "PeopleId", UpdateCheck = UpdateCheck.Never, Storage = "_PeopleId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int PeopleId
        {
            get => _PeopleId;

            set
            {
                if (_PeopleId != value)
                {
                    OnPeopleIdChanging(value);
                    SendPropertyChanging();
                    _PeopleId = value;
                    SendPropertyChanged("PeopleId");
                    OnPeopleIdChanged();
                }
            }
        }

        [Column(Name = "CreatedBy", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedBy", DbType = "int NOT NULL")]
        public int CreatedBy
        {
            get => _CreatedBy;

            set
            {
                if (_CreatedBy != value)
                {
                    OnCreatedByChanging(value);
                    SendPropertyChanging();
                    _CreatedBy = value;
                    SendPropertyChanged("CreatedBy");
                    OnCreatedByChanged();
                }
            }
        }

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime")]
        public DateTime? CreatedDate
        {
            get => _CreatedDate;

            set
            {
                if (_CreatedDate != value)
                {
                    OnCreatedDateChanging(value);
                    SendPropertyChanging();
                    _CreatedDate = value;
                    SendPropertyChanged("CreatedDate");
                    OnCreatedDateChanged();
                }
            }
        }

        [Column(Name = "DropCodeId", UpdateCheck = UpdateCheck.Never, Storage = "_DropCodeId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int DropCodeId
        {
            get => _DropCodeId;

            set
            {
                if (_DropCodeId != value)
                {
                    if (_DropType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnDropCodeIdChanging(value);
                    SendPropertyChanging();
                    _DropCodeId = value;
                    SendPropertyChanged("DropCodeId");
                    OnDropCodeIdChanged();
                }
            }
        }

        [Column(Name = "GenderId", UpdateCheck = UpdateCheck.Never, Storage = "_GenderId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int GenderId
        {
            get => _GenderId;

            set
            {
                if (_GenderId != value)
                {
                    if (_Gender.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnGenderIdChanging(value);
                    SendPropertyChanging();
                    _GenderId = value;
                    SendPropertyChanged("GenderId");
                    OnGenderIdChanged();
                }
            }
        }

        [Column(Name = "DoNotMailFlag", UpdateCheck = UpdateCheck.Never, Storage = "_DoNotMailFlag", DbType = "bit NOT NULL")]
        public bool DoNotMailFlag
        {
            get => _DoNotMailFlag;

            set
            {
                if (_DoNotMailFlag != value)
                {
                    OnDoNotMailFlagChanging(value);
                    SendPropertyChanging();
                    _DoNotMailFlag = value;
                    SendPropertyChanged("DoNotMailFlag");
                    OnDoNotMailFlagChanged();
                }
            }
        }

        [Column(Name = "DoNotCallFlag", UpdateCheck = UpdateCheck.Never, Storage = "_DoNotCallFlag", DbType = "bit NOT NULL")]
        public bool DoNotCallFlag
        {
            get => _DoNotCallFlag;

            set
            {
                if (_DoNotCallFlag != value)
                {
                    OnDoNotCallFlagChanging(value);
                    SendPropertyChanging();
                    _DoNotCallFlag = value;
                    SendPropertyChanged("DoNotCallFlag");
                    OnDoNotCallFlagChanged();
                }
            }
        }

        [Column(Name = "DoNotVisitFlag", UpdateCheck = UpdateCheck.Never, Storage = "_DoNotVisitFlag", DbType = "bit NOT NULL")]
        public bool DoNotVisitFlag
        {
            get => _DoNotVisitFlag;

            set
            {
                if (_DoNotVisitFlag != value)
                {
                    OnDoNotVisitFlagChanging(value);
                    SendPropertyChanging();
                    _DoNotVisitFlag = value;
                    SendPropertyChanged("DoNotVisitFlag");
                    OnDoNotVisitFlagChanged();
                }
            }
        }

        [Column(Name = "AddressTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_AddressTypeId", DbType = "int NOT NULL")]
        public int AddressTypeId
        {
            get => _AddressTypeId;

            set
            {
                if (_AddressTypeId != value)
                {
                    OnAddressTypeIdChanging(value);
                    SendPropertyChanging();
                    _AddressTypeId = value;
                    SendPropertyChanged("AddressTypeId");
                    OnAddressTypeIdChanged();
                }
            }
        }

        [Column(Name = "PhonePrefId", UpdateCheck = UpdateCheck.Never, Storage = "_PhonePrefId", DbType = "int NOT NULL")]
        public int PhonePrefId
        {
            get => _PhonePrefId;

            set
            {
                if (_PhonePrefId != value)
                {
                    OnPhonePrefIdChanging(value);
                    SendPropertyChanging();
                    _PhonePrefId = value;
                    SendPropertyChanged("PhonePrefId");
                    OnPhonePrefIdChanged();
                }
            }
        }

        [Column(Name = "MaritalStatusId", UpdateCheck = UpdateCheck.Never, Storage = "_MaritalStatusId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int MaritalStatusId
        {
            get => _MaritalStatusId;

            set
            {
                if (_MaritalStatusId != value)
                {
                    if (_MaritalStatus.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnMaritalStatusIdChanging(value);
                    SendPropertyChanging();
                    _MaritalStatusId = value;
                    SendPropertyChanged("MaritalStatusId");
                    OnMaritalStatusIdChanged();
                }
            }
        }

        [Column(Name = "PositionInFamilyId", UpdateCheck = UpdateCheck.Never, Storage = "_PositionInFamilyId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int PositionInFamilyId
        {
            get => _PositionInFamilyId;

            set
            {
                if (_PositionInFamilyId != value)
                {
                    if (_FamilyPosition.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPositionInFamilyIdChanging(value);
                    SendPropertyChanging();
                    _PositionInFamilyId = value;
                    SendPropertyChanged("PositionInFamilyId");
                    OnPositionInFamilyIdChanged();
                }
            }
        }

        [Column(Name = "MemberStatusId", UpdateCheck = UpdateCheck.Never, Storage = "_MemberStatusId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int MemberStatusId
        {
            get => _MemberStatusId;

            set
            {
                if (_MemberStatusId != value)
                {
                    if (_MemberStatus.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnMemberStatusIdChanging(value);
                    SendPropertyChanging();
                    _MemberStatusId = value;
                    SendPropertyChanged("MemberStatusId");
                    OnMemberStatusIdChanged();
                }
            }
        }

        [Column(Name = "FamilyId", UpdateCheck = UpdateCheck.Never, Storage = "_FamilyId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int FamilyId
        {
            get => _FamilyId;

            set
            {
                if (_FamilyId != value)
                {
                    if (_Family.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnFamilyIdChanging(value);
                    SendPropertyChanging();
                    _FamilyId = value;
                    SendPropertyChanged("FamilyId");
                    OnFamilyIdChanged();
                }
            }
        }

        [Column(Name = "BirthMonth", UpdateCheck = UpdateCheck.Never, Storage = "_BirthMonth", DbType = "int")]
        public int? BirthMonth
        {
            get => _BirthMonth;

            set
            {
                if (_BirthMonth != value)
                {
                    OnBirthMonthChanging(value);
                    SendPropertyChanging();
                    _BirthMonth = value;
                    SendPropertyChanged("BirthMonth");
                    OnBirthMonthChanged();
                }
            }
        }

        [Column(Name = "BirthDay", UpdateCheck = UpdateCheck.Never, Storage = "_BirthDay", DbType = "int")]
        public int? BirthDay
        {
            get => _BirthDay;

            set
            {
                if (_BirthDay != value)
                {
                    OnBirthDayChanging(value);
                    SendPropertyChanging();
                    _BirthDay = value;
                    SendPropertyChanged("BirthDay");
                    OnBirthDayChanged();
                }
            }
        }

        [Column(Name = "BirthYear", UpdateCheck = UpdateCheck.Never, Storage = "_BirthYear", DbType = "int")]
        public int? BirthYear
        {
            get => _BirthYear;

            set
            {
                if (_BirthYear != value)
                {
                    OnBirthYearChanging(value);
                    SendPropertyChanging();
                    _BirthYear = value;
                    SendPropertyChanged("BirthYear");
                    OnBirthYearChanged();
                }
            }
        }

        [Column(Name = "OriginId", UpdateCheck = UpdateCheck.Never, Storage = "_OriginId", DbType = "int")]
        [IsForeignKey]
        public int? OriginId
        {
            get => _OriginId;

            set
            {
                if (_OriginId != value)
                {
                    if (_Origin.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOriginIdChanging(value);
                    SendPropertyChanging();
                    _OriginId = value;
                    SendPropertyChanged("OriginId");
                    OnOriginIdChanged();
                }
            }
        }

        [Column(Name = "EntryPointId", UpdateCheck = UpdateCheck.Never, Storage = "_EntryPointId", DbType = "int")]
        [IsForeignKey]
        public int? EntryPointId
        {
            get => _EntryPointId;

            set
            {
                if (_EntryPointId != value)
                {
                    if (_EntryPoint.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnEntryPointIdChanging(value);
                    SendPropertyChanging();
                    _EntryPointId = value;
                    SendPropertyChanged("EntryPointId");
                    OnEntryPointIdChanged();
                }
            }
        }

        [Column(Name = "InterestPointId", UpdateCheck = UpdateCheck.Never, Storage = "_InterestPointId", DbType = "int")]
        [IsForeignKey]
        public int? InterestPointId
        {
            get => _InterestPointId;

            set
            {
                if (_InterestPointId != value)
                {
                    if (_InterestPoint.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnInterestPointIdChanging(value);
                    SendPropertyChanging();
                    _InterestPointId = value;
                    SendPropertyChanged("InterestPointId");
                    OnInterestPointIdChanged();
                }
            }
        }

        [Column(Name = "BaptismTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_BaptismTypeId", DbType = "int")]
        [IsForeignKey]
        public int? BaptismTypeId
        {
            get => _BaptismTypeId;

            set
            {
                if (_BaptismTypeId != value)
                {
                    if (_BaptismType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnBaptismTypeIdChanging(value);
                    SendPropertyChanging();
                    _BaptismTypeId = value;
                    SendPropertyChanged("BaptismTypeId");
                    OnBaptismTypeIdChanged();
                }
            }
        }

        [Column(Name = "BaptismStatusId", UpdateCheck = UpdateCheck.Never, Storage = "_BaptismStatusId", DbType = "int")]
        [IsForeignKey]
        public int? BaptismStatusId
        {
            get => _BaptismStatusId;

            set
            {
                if (_BaptismStatusId != value)
                {
                    if (_BaptismStatus.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnBaptismStatusIdChanging(value);
                    SendPropertyChanging();
                    _BaptismStatusId = value;
                    SendPropertyChanged("BaptismStatusId");
                    OnBaptismStatusIdChanged();
                }
            }
        }

        [Column(Name = "DecisionTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_DecisionTypeId", DbType = "int")]
        [IsForeignKey]
        public int? DecisionTypeId
        {
            get => _DecisionTypeId;

            set
            {
                if (_DecisionTypeId != value)
                {
                    if (_DecisionType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnDecisionTypeIdChanging(value);
                    SendPropertyChanging();
                    _DecisionTypeId = value;
                    SendPropertyChanged("DecisionTypeId");
                    OnDecisionTypeIdChanged();
                }
            }
        }

        [Column(Name = "NewMemberClassStatusId", UpdateCheck = UpdateCheck.Never, Storage = "_NewMemberClassStatusId", DbType = "int")]
        [IsForeignKey]
        public int? NewMemberClassStatusId
        {
            get => _NewMemberClassStatusId;

            set
            {
                if (_NewMemberClassStatusId != value)
                {
                    if (_NewMemberClassStatus.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnNewMemberClassStatusIdChanging(value);
                    SendPropertyChanging();
                    _NewMemberClassStatusId = value;
                    SendPropertyChanged("NewMemberClassStatusId");
                    OnNewMemberClassStatusIdChanged();
                }
            }
        }

        [Column(Name = "LetterStatusId", UpdateCheck = UpdateCheck.Never, Storage = "_LetterStatusId", DbType = "int")]
        [IsForeignKey]
        public int? LetterStatusId
        {
            get => _LetterStatusId;

            set
            {
                if (_LetterStatusId != value)
                {
                    if (_MemberLetterStatus.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnLetterStatusIdChanging(value);
                    SendPropertyChanging();
                    _LetterStatusId = value;
                    SendPropertyChanged("LetterStatusId");
                    OnLetterStatusIdChanged();
                }
            }
        }

        [Column(Name = "JoinCodeId", UpdateCheck = UpdateCheck.Never, Storage = "_JoinCodeId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int JoinCodeId
        {
            get => _JoinCodeId;

            set
            {
                if (_JoinCodeId != value)
                {
                    if (_JoinType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnJoinCodeIdChanging(value);
                    SendPropertyChanging();
                    _JoinCodeId = value;
                    SendPropertyChanged("JoinCodeId");
                    OnJoinCodeIdChanged();
                }
            }
        }

        [Column(Name = "EnvelopeOptionsId", UpdateCheck = UpdateCheck.Never, Storage = "_EnvelopeOptionsId", DbType = "int")]
        [IsForeignKey]
        public int? EnvelopeOptionsId
        {
            get => _EnvelopeOptionsId;

            set
            {
                if (_EnvelopeOptionsId != value)
                {
                    if (_EnvelopeOption.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnEnvelopeOptionsIdChanging(value);
                    SendPropertyChanging();
                    _EnvelopeOptionsId = value;
                    SendPropertyChanged("EnvelopeOptionsId");
                    OnEnvelopeOptionsIdChanged();
                }
            }
        }

        [Column(Name = "BadAddressFlag", UpdateCheck = UpdateCheck.Never, Storage = "_BadAddressFlag", DbType = "bit")]
        public bool? BadAddressFlag
        {
            get => _BadAddressFlag;

            set
            {
                if (_BadAddressFlag != value)
                {
                    OnBadAddressFlagChanging(value);
                    SendPropertyChanging();
                    _BadAddressFlag = value;
                    SendPropertyChanged("BadAddressFlag");
                    OnBadAddressFlagChanged();
                }
            }
        }

        [Column(Name = "ResCodeId", UpdateCheck = UpdateCheck.Never, Storage = "_ResCodeId", DbType = "int")]
        [IsForeignKey]
        public int? ResCodeId
        {
            get => _ResCodeId;

            set
            {
                if (_ResCodeId != value)
                {
                    if (_ResidentCode.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnResCodeIdChanging(value);
                    SendPropertyChanging();
                    _ResCodeId = value;
                    SendPropertyChanged("ResCodeId");
                    OnResCodeIdChanged();
                }
            }
        }

        [Column(Name = "AddressFromDate", UpdateCheck = UpdateCheck.Never, Storage = "_AddressFromDate", DbType = "datetime")]
        public DateTime? AddressFromDate
        {
            get => _AddressFromDate;

            set
            {
                if (_AddressFromDate != value)
                {
                    OnAddressFromDateChanging(value);
                    SendPropertyChanging();
                    _AddressFromDate = value;
                    SendPropertyChanged("AddressFromDate");
                    OnAddressFromDateChanged();
                }
            }
        }

        [Column(Name = "AddressToDate", UpdateCheck = UpdateCheck.Never, Storage = "_AddressToDate", DbType = "datetime")]
        public DateTime? AddressToDate
        {
            get => _AddressToDate;

            set
            {
                if (_AddressToDate != value)
                {
                    OnAddressToDateChanging(value);
                    SendPropertyChanging();
                    _AddressToDate = value;
                    SendPropertyChanged("AddressToDate");
                    OnAddressToDateChanged();
                }
            }
        }

        [Column(Name = "WeddingDate", UpdateCheck = UpdateCheck.Never, Storage = "_WeddingDate", DbType = "datetime")]
        public DateTime? WeddingDate
        {
            get => _WeddingDate;

            set
            {
                if (_WeddingDate != value)
                {
                    OnWeddingDateChanging(value);
                    SendPropertyChanging();
                    _WeddingDate = value;
                    SendPropertyChanged("WeddingDate");
                    OnWeddingDateChanged();
                }
            }
        }

        [Column(Name = "OriginDate", UpdateCheck = UpdateCheck.Never, Storage = "_OriginDate", DbType = "datetime")]
        public DateTime? OriginDate
        {
            get => _OriginDate;

            set
            {
                if (_OriginDate != value)
                {
                    OnOriginDateChanging(value);
                    SendPropertyChanging();
                    _OriginDate = value;
                    SendPropertyChanged("OriginDate");
                    OnOriginDateChanged();
                }
            }
        }

        [Column(Name = "BaptismSchedDate", UpdateCheck = UpdateCheck.Never, Storage = "_BaptismSchedDate", DbType = "datetime")]
        public DateTime? BaptismSchedDate
        {
            get => _BaptismSchedDate;

            set
            {
                if (_BaptismSchedDate != value)
                {
                    OnBaptismSchedDateChanging(value);
                    SendPropertyChanging();
                    _BaptismSchedDate = value;
                    SendPropertyChanged("BaptismSchedDate");
                    OnBaptismSchedDateChanged();
                }
            }
        }

        [Column(Name = "BaptismDate", UpdateCheck = UpdateCheck.Never, Storage = "_BaptismDate", DbType = "datetime")]
        public DateTime? BaptismDate
        {
            get => _BaptismDate;

            set
            {
                if (_BaptismDate != value)
                {
                    OnBaptismDateChanging(value);
                    SendPropertyChanging();
                    _BaptismDate = value;
                    SendPropertyChanged("BaptismDate");
                    OnBaptismDateChanged();
                }
            }
        }

        [Column(Name = "DecisionDate", UpdateCheck = UpdateCheck.Never, Storage = "_DecisionDate", DbType = "datetime")]
        public DateTime? DecisionDate
        {
            get => _DecisionDate;

            set
            {
                if (_DecisionDate != value)
                {
                    OnDecisionDateChanging(value);
                    SendPropertyChanging();
                    _DecisionDate = value;
                    SendPropertyChanged("DecisionDate");
                    OnDecisionDateChanged();
                }
            }
        }

        [Column(Name = "LetterDateRequested", UpdateCheck = UpdateCheck.Never, Storage = "_LetterDateRequested", DbType = "datetime")]
        public DateTime? LetterDateRequested
        {
            get => _LetterDateRequested;

            set
            {
                if (_LetterDateRequested != value)
                {
                    OnLetterDateRequestedChanging(value);
                    SendPropertyChanging();
                    _LetterDateRequested = value;
                    SendPropertyChanged("LetterDateRequested");
                    OnLetterDateRequestedChanged();
                }
            }
        }

        [Column(Name = "LetterDateReceived", UpdateCheck = UpdateCheck.Never, Storage = "_LetterDateReceived", DbType = "datetime")]
        public DateTime? LetterDateReceived
        {
            get => _LetterDateReceived;

            set
            {
                if (_LetterDateReceived != value)
                {
                    OnLetterDateReceivedChanging(value);
                    SendPropertyChanging();
                    _LetterDateReceived = value;
                    SendPropertyChanged("LetterDateReceived");
                    OnLetterDateReceivedChanged();
                }
            }
        }

        [Column(Name = "JoinDate", UpdateCheck = UpdateCheck.Never, Storage = "_JoinDate", DbType = "datetime")]
        public DateTime? JoinDate
        {
            get => _JoinDate;

            set
            {
                if (_JoinDate != value)
                {
                    OnJoinDateChanging(value);
                    SendPropertyChanging();
                    _JoinDate = value;
                    SendPropertyChanged("JoinDate");
                    OnJoinDateChanged();
                }
            }
        }

        [Column(Name = "DropDate", UpdateCheck = UpdateCheck.Never, Storage = "_DropDate", DbType = "datetime")]
        public DateTime? DropDate
        {
            get => _DropDate;

            set
            {
                if (_DropDate != value)
                {
                    OnDropDateChanging(value);
                    SendPropertyChanging();
                    _DropDate = value;
                    SendPropertyChanged("DropDate");
                    OnDropDateChanged();
                }
            }
        }

        [Column(Name = "DeceasedDate", UpdateCheck = UpdateCheck.Never, Storage = "_DeceasedDate", DbType = "datetime")]
        public DateTime? DeceasedDate
        {
            get => _DeceasedDate;

            set
            {
                if (_DeceasedDate != value)
                {
                    OnDeceasedDateChanging(value);
                    SendPropertyChanging();
                    _DeceasedDate = value;
                    SendPropertyChanged("DeceasedDate");
                    OnDeceasedDateChanged();
                }
            }
        }

        [Column(Name = "TitleCode", UpdateCheck = UpdateCheck.Never, Storage = "_TitleCode", DbType = "nvarchar(10)")]
        public string TitleCode
        {
            get => _TitleCode;

            set
            {
                if (_TitleCode != value)
                {
                    OnTitleCodeChanging(value);
                    SendPropertyChanging();
                    _TitleCode = value;
                    SendPropertyChanged("TitleCode");
                    OnTitleCodeChanged();
                }
            }
        }

        [Column(Name = "FirstName", UpdateCheck = UpdateCheck.Never, Storage = "_FirstName", DbType = "nvarchar(25)")]
        public string FirstName
        {
            get => _FirstName;

            set
            {
                if (_FirstName != value)
                {
                    OnFirstNameChanging(value);
                    SendPropertyChanging();
                    _FirstName = value;
                    SendPropertyChanged("FirstName");
                    OnFirstNameChanged();
                }
            }
        }

        [Column(Name = "MiddleName", UpdateCheck = UpdateCheck.Never, Storage = "_MiddleName", DbType = "nvarchar(30)")]
        public string MiddleName
        {
            get => _MiddleName;

            set
            {
                if (_MiddleName != value)
                {
                    OnMiddleNameChanging(value);
                    SendPropertyChanging();
                    _MiddleName = value;
                    SendPropertyChanged("MiddleName");
                    OnMiddleNameChanged();
                }
            }
        }

        [Column(Name = "MaidenName", UpdateCheck = UpdateCheck.Never, Storage = "_MaidenName", DbType = "nvarchar(20)")]
        public string MaidenName
        {
            get => _MaidenName;

            set
            {
                if (_MaidenName != value)
                {
                    OnMaidenNameChanging(value);
                    SendPropertyChanging();
                    _MaidenName = value;
                    SendPropertyChanged("MaidenName");
                    OnMaidenNameChanged();
                }
            }
        }

        [Column(Name = "LastName", UpdateCheck = UpdateCheck.Never, Storage = "_LastName", DbType = "nvarchar(100) NOT NULL")]
        public string LastName
        {
            get => _LastName;

            set
            {
                if (_LastName != value)
                {
                    OnLastNameChanging(value);
                    SendPropertyChanging();
                    _LastName = value;
                    SendPropertyChanged("LastName");
                    OnLastNameChanged();
                }
            }
        }

        [Column(Name = "SuffixCode", UpdateCheck = UpdateCheck.Never, Storage = "_SuffixCode", DbType = "nvarchar(10)")]
        public string SuffixCode
        {
            get => _SuffixCode;

            set
            {
                if (_SuffixCode != value)
                {
                    OnSuffixCodeChanging(value);
                    SendPropertyChanging();
                    _SuffixCode = value;
                    SendPropertyChanged("SuffixCode");
                    OnSuffixCodeChanged();
                }
            }
        }

        [Column(Name = "NickName", UpdateCheck = UpdateCheck.Never, Storage = "_NickName", DbType = "nvarchar(25)")]
        public string NickName
        {
            get => _NickName;

            set
            {
                if (_NickName != value)
                {
                    OnNickNameChanging(value);
                    SendPropertyChanging();
                    _NickName = value;
                    SendPropertyChanged("NickName");
                    OnNickNameChanged();
                }
            }
        }

        [Column(Name = "AddressLineOne", UpdateCheck = UpdateCheck.Never, Storage = "_AddressLineOne", DbType = "nvarchar(100)")]
        public string AddressLineOne
        {
            get => _AddressLineOne;

            set
            {
                if (_AddressLineOne != value)
                {
                    OnAddressLineOneChanging(value);
                    SendPropertyChanging();
                    _AddressLineOne = value;
                    SendPropertyChanged("AddressLineOne");
                    OnAddressLineOneChanged();
                }
            }
        }

        [Column(Name = "AddressLineTwo", UpdateCheck = UpdateCheck.Never, Storage = "_AddressLineTwo", DbType = "nvarchar(100)")]
        public string AddressLineTwo
        {
            get => _AddressLineTwo;

            set
            {
                if (_AddressLineTwo != value)
                {
                    OnAddressLineTwoChanging(value);
                    SendPropertyChanging();
                    _AddressLineTwo = value;
                    SendPropertyChanged("AddressLineTwo");
                    OnAddressLineTwoChanged();
                }
            }
        }

        [Column(Name = "CityName", UpdateCheck = UpdateCheck.Never, Storage = "_CityName", DbType = "nvarchar(30)")]
        public string CityName
        {
            get => _CityName;

            set
            {
                if (_CityName != value)
                {
                    OnCityNameChanging(value);
                    SendPropertyChanging();
                    _CityName = value;
                    SendPropertyChanged("CityName");
                    OnCityNameChanged();
                }
            }
        }

        [Column(Name = "StateCode", UpdateCheck = UpdateCheck.Never, Storage = "_StateCode", DbType = "nvarchar(30)")]
        public string StateCode
        {
            get => _StateCode;

            set
            {
                if (_StateCode != value)
                {
                    OnStateCodeChanging(value);
                    SendPropertyChanging();
                    _StateCode = value;
                    SendPropertyChanged("StateCode");
                    OnStateCodeChanged();
                }
            }
        }

        [Column(Name = "ZipCode", UpdateCheck = UpdateCheck.Never, Storage = "_ZipCode", DbType = "nvarchar(15)")]
        public string ZipCode
        {
            get => _ZipCode;

            set
            {
                if (_ZipCode != value)
                {
                    OnZipCodeChanging(value);
                    SendPropertyChanging();
                    _ZipCode = value;
                    SendPropertyChanged("ZipCode");
                    OnZipCodeChanged();
                }
            }
        }

        [Column(Name = "CountryName", UpdateCheck = UpdateCheck.Never, Storage = "_CountryName", DbType = "nvarchar(40)")]
        public string CountryName
        {
            get => _CountryName;

            set
            {
                if (_CountryName != value)
                {
                    OnCountryNameChanging(value);
                    SendPropertyChanging();
                    _CountryName = value;
                    SendPropertyChanged("CountryName");
                    OnCountryNameChanged();
                }
            }
        }

        [Column(Name = "StreetName", UpdateCheck = UpdateCheck.Never, Storage = "_StreetName", DbType = "nvarchar(40)")]
        public string StreetName
        {
            get => _StreetName;

            set
            {
                if (_StreetName != value)
                {
                    OnStreetNameChanging(value);
                    SendPropertyChanging();
                    _StreetName = value;
                    SendPropertyChanged("StreetName");
                    OnStreetNameChanged();
                }
            }
        }

        [Column(Name = "CellPhone", UpdateCheck = UpdateCheck.Never, Storage = "_CellPhone", DbType = "nvarchar(20)")]
        public string CellPhone
        {
            get => _CellPhone;

            set
            {
                if (_CellPhone != value)
                {
                    OnCellPhoneChanging(value);
                    SendPropertyChanging();
                    _CellPhone = value;
                    SendPropertyChanged("CellPhone");
                    OnCellPhoneChanged();
                }
            }
        }

        [Column(Name = "WorkPhone", UpdateCheck = UpdateCheck.Never, Storage = "_WorkPhone", DbType = "nvarchar(20)")]
        public string WorkPhone
        {
            get => _WorkPhone;

            set
            {
                if (_WorkPhone != value)
                {
                    OnWorkPhoneChanging(value);
                    SendPropertyChanging();
                    _WorkPhone = value;
                    SendPropertyChanged("WorkPhone");
                    OnWorkPhoneChanged();
                }
            }
        }

        [Column(Name = "EmailAddress", UpdateCheck = UpdateCheck.Never, Storage = "_EmailAddress", DbType = "nvarchar(150)")]
        public string EmailAddress
        {
            get => _EmailAddress;

            set
            {
                if (_EmailAddress != value)
                {
                    OnEmailAddressChanging(value);
                    SendPropertyChanging();
                    _EmailAddress = value;
                    SendPropertyChanged("EmailAddress");
                    OnEmailAddressChanged();
                }
            }
        }

        [Column(Name = "OtherPreviousChurch", UpdateCheck = UpdateCheck.Never, Storage = "_OtherPreviousChurch", DbType = "nvarchar(120)")]
        public string OtherPreviousChurch
        {
            get => _OtherPreviousChurch;

            set
            {
                if (_OtherPreviousChurch != value)
                {
                    OnOtherPreviousChurchChanging(value);
                    SendPropertyChanging();
                    _OtherPreviousChurch = value;
                    SendPropertyChanged("OtherPreviousChurch");
                    OnOtherPreviousChurchChanged();
                }
            }
        }

        [Column(Name = "OtherNewChurch", UpdateCheck = UpdateCheck.Never, Storage = "_OtherNewChurch", DbType = "nvarchar(60)")]
        public string OtherNewChurch
        {
            get => _OtherNewChurch;

            set
            {
                if (_OtherNewChurch != value)
                {
                    OnOtherNewChurchChanging(value);
                    SendPropertyChanging();
                    _OtherNewChurch = value;
                    SendPropertyChanged("OtherNewChurch");
                    OnOtherNewChurchChanged();
                }
            }
        }

        [Column(Name = "SchoolOther", UpdateCheck = UpdateCheck.Never, Storage = "_SchoolOther", DbType = "nvarchar(100)")]
        public string SchoolOther
        {
            get => _SchoolOther;

            set
            {
                if (_SchoolOther != value)
                {
                    OnSchoolOtherChanging(value);
                    SendPropertyChanging();
                    _SchoolOther = value;
                    SendPropertyChanged("SchoolOther");
                    OnSchoolOtherChanged();
                }
            }
        }

        [Column(Name = "EmployerOther", UpdateCheck = UpdateCheck.Never, Storage = "_EmployerOther", DbType = "nvarchar(120)")]
        public string EmployerOther
        {
            get => _EmployerOther;

            set
            {
                if (_EmployerOther != value)
                {
                    OnEmployerOtherChanging(value);
                    SendPropertyChanging();
                    _EmployerOther = value;
                    SendPropertyChanged("EmployerOther");
                    OnEmployerOtherChanged();
                }
            }
        }

        [Column(Name = "OccupationOther", UpdateCheck = UpdateCheck.Never, Storage = "_OccupationOther", DbType = "nvarchar(120)")]
        public string OccupationOther
        {
            get => _OccupationOther;

            set
            {
                if (_OccupationOther != value)
                {
                    OnOccupationOtherChanging(value);
                    SendPropertyChanging();
                    _OccupationOther = value;
                    SendPropertyChanged("OccupationOther");
                    OnOccupationOtherChanged();
                }
            }
        }

        [Column(Name = "HobbyOther", UpdateCheck = UpdateCheck.Never, Storage = "_HobbyOther", DbType = "nvarchar(40)")]
        public string HobbyOther
        {
            get => _HobbyOther;

            set
            {
                if (_HobbyOther != value)
                {
                    OnHobbyOtherChanging(value);
                    SendPropertyChanging();
                    _HobbyOther = value;
                    SendPropertyChanged("HobbyOther");
                    OnHobbyOtherChanged();
                }
            }
        }

        [Column(Name = "SkillOther", UpdateCheck = UpdateCheck.Never, Storage = "_SkillOther", DbType = "nvarchar(40)")]
        public string SkillOther
        {
            get => _SkillOther;

            set
            {
                if (_SkillOther != value)
                {
                    OnSkillOtherChanging(value);
                    SendPropertyChanging();
                    _SkillOther = value;
                    SendPropertyChanged("SkillOther");
                    OnSkillOtherChanged();
                }
            }
        }

        [Column(Name = "InterestOther", UpdateCheck = UpdateCheck.Never, Storage = "_InterestOther", DbType = "nvarchar(40)")]
        public string InterestOther
        {
            get => _InterestOther;

            set
            {
                if (_InterestOther != value)
                {
                    OnInterestOtherChanging(value);
                    SendPropertyChanging();
                    _InterestOther = value;
                    SendPropertyChanged("InterestOther");
                    OnInterestOtherChanged();
                }
            }
        }

        [Column(Name = "LetterStatusNotes", UpdateCheck = UpdateCheck.Never, Storage = "_LetterStatusNotes", DbType = "nvarchar(3000)")]
        public string LetterStatusNotes
        {
            get => _LetterStatusNotes;

            set
            {
                if (_LetterStatusNotes != value)
                {
                    OnLetterStatusNotesChanging(value);
                    SendPropertyChanging();
                    _LetterStatusNotes = value;
                    SendPropertyChanged("LetterStatusNotes");
                    OnLetterStatusNotesChanged();
                }
            }
        }

        [Column(Name = "Comments", UpdateCheck = UpdateCheck.Never, Storage = "_Comments", DbType = "nvarchar(3000)")]
        public string Comments
        {
            get => _Comments;

            set
            {
                if (_Comments != value)
                {
                    OnCommentsChanging(value);
                    SendPropertyChanging();
                    _Comments = value;
                    SendPropertyChanged("Comments");
                    OnCommentsChanged();
                }
            }
        }

        [Column(Name = "ChristAsSavior", UpdateCheck = UpdateCheck.Never, Storage = "_ChristAsSavior", DbType = "bit NOT NULL")]
        public bool ChristAsSavior
        {
            get => _ChristAsSavior;

            set
            {
                if (_ChristAsSavior != value)
                {
                    OnChristAsSaviorChanging(value);
                    SendPropertyChanging();
                    _ChristAsSavior = value;
                    SendPropertyChanged("ChristAsSavior");
                    OnChristAsSaviorChanged();
                }
            }
        }

        [Column(Name = "MemberAnyChurch", UpdateCheck = UpdateCheck.Never, Storage = "_MemberAnyChurch", DbType = "bit")]
        public bool? MemberAnyChurch
        {
            get => _MemberAnyChurch;

            set
            {
                if (_MemberAnyChurch != value)
                {
                    OnMemberAnyChurchChanging(value);
                    SendPropertyChanging();
                    _MemberAnyChurch = value;
                    SendPropertyChanged("MemberAnyChurch");
                    OnMemberAnyChurchChanged();
                }
            }
        }

        [Column(Name = "InterestedInJoining", UpdateCheck = UpdateCheck.Never, Storage = "_InterestedInJoining", DbType = "bit NOT NULL")]
        public bool InterestedInJoining
        {
            get => _InterestedInJoining;

            set
            {
                if (_InterestedInJoining != value)
                {
                    OnInterestedInJoiningChanging(value);
                    SendPropertyChanging();
                    _InterestedInJoining = value;
                    SendPropertyChanged("InterestedInJoining");
                    OnInterestedInJoiningChanged();
                }
            }
        }

        [Column(Name = "PleaseVisit", UpdateCheck = UpdateCheck.Never, Storage = "_PleaseVisit", DbType = "bit NOT NULL")]
        public bool PleaseVisit
        {
            get => _PleaseVisit;

            set
            {
                if (_PleaseVisit != value)
                {
                    OnPleaseVisitChanging(value);
                    SendPropertyChanging();
                    _PleaseVisit = value;
                    SendPropertyChanged("PleaseVisit");
                    OnPleaseVisitChanged();
                }
            }
        }

        [Column(Name = "InfoBecomeAChristian", UpdateCheck = UpdateCheck.Never, Storage = "_InfoBecomeAChristian", DbType = "bit NOT NULL")]
        public bool InfoBecomeAChristian
        {
            get => _InfoBecomeAChristian;

            set
            {
                if (_InfoBecomeAChristian != value)
                {
                    OnInfoBecomeAChristianChanging(value);
                    SendPropertyChanging();
                    _InfoBecomeAChristian = value;
                    SendPropertyChanged("InfoBecomeAChristian");
                    OnInfoBecomeAChristianChanged();
                }
            }
        }

        [Column(Name = "ContributionsStatement", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionsStatement", DbType = "bit NOT NULL")]
        public bool ContributionsStatement
        {
            get => _ContributionsStatement;

            set
            {
                if (_ContributionsStatement != value)
                {
                    OnContributionsStatementChanging(value);
                    SendPropertyChanging();
                    _ContributionsStatement = value;
                    SendPropertyChanged("ContributionsStatement");
                    OnContributionsStatementChanged();
                }
            }
        }

        [Column(Name = "ModifiedBy", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedBy", DbType = "int")]
        public int? ModifiedBy
        {
            get => _ModifiedBy;

            set
            {
                if (_ModifiedBy != value)
                {
                    OnModifiedByChanging(value);
                    SendPropertyChanging();
                    _ModifiedBy = value;
                    SendPropertyChanged("ModifiedBy");
                    OnModifiedByChanged();
                }
            }
        }

        [Column(Name = "ModifiedDate", UpdateCheck = UpdateCheck.Never, Storage = "_ModifiedDate", DbType = "datetime")]
        public DateTime? ModifiedDate
        {
            get => _ModifiedDate;

            set
            {
                if (_ModifiedDate != value)
                {
                    OnModifiedDateChanging(value);
                    SendPropertyChanging();
                    _ModifiedDate = value;
                    SendPropertyChanged("ModifiedDate");
                    OnModifiedDateChanged();
                }
            }
        }

        [Column(Name = "PictureId", UpdateCheck = UpdateCheck.Never, Storage = "_PictureId", DbType = "int")]
        [IsForeignKey]
        public int? PictureId
        {
            get => _PictureId;

            set
            {
                if (_PictureId != value)
                {
                    if (_Picture.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnPictureIdChanging(value);
                    SendPropertyChanging();
                    _PictureId = value;
                    SendPropertyChanged("PictureId");
                    OnPictureIdChanged();
                }
            }
        }

        [Column(Name = "ContributionOptionsId", UpdateCheck = UpdateCheck.Never, Storage = "_ContributionOptionsId", DbType = "int")]
        [IsForeignKey]
        public int? ContributionOptionsId
        {
            get => _ContributionOptionsId;

            set
            {
                if (_ContributionOptionsId != value)
                {
                    if (_ContributionStatementOption.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnContributionOptionsIdChanging(value);
                    SendPropertyChanging();
                    _ContributionOptionsId = value;
                    SendPropertyChanged("ContributionOptionsId");
                    OnContributionOptionsIdChanged();
                }
            }
        }

        [Column(Name = "PrimaryCity", UpdateCheck = UpdateCheck.Never, Storage = "_PrimaryCity", DbType = "nvarchar(30)")]
        public string PrimaryCity
        {
            get => _PrimaryCity;

            set
            {
                if (_PrimaryCity != value)
                {
                    OnPrimaryCityChanging(value);
                    SendPropertyChanging();
                    _PrimaryCity = value;
                    SendPropertyChanged("PrimaryCity");
                    OnPrimaryCityChanged();
                }
            }
        }

        [Column(Name = "PrimaryZip", UpdateCheck = UpdateCheck.Never, Storage = "_PrimaryZip", DbType = "nvarchar(15)")]
        public string PrimaryZip
        {
            get => _PrimaryZip;

            set
            {
                if (_PrimaryZip != value)
                {
                    OnPrimaryZipChanging(value);
                    SendPropertyChanging();
                    _PrimaryZip = value;
                    SendPropertyChanged("PrimaryZip");
                    OnPrimaryZipChanged();
                }
            }
        }

        [Column(Name = "PrimaryAddress", UpdateCheck = UpdateCheck.Never, Storage = "_PrimaryAddress", DbType = "nvarchar(100)")]
        public string PrimaryAddress
        {
            get => _PrimaryAddress;

            set
            {
                if (_PrimaryAddress != value)
                {
                    OnPrimaryAddressChanging(value);
                    SendPropertyChanging();
                    _PrimaryAddress = value;
                    SendPropertyChanged("PrimaryAddress");
                    OnPrimaryAddressChanged();
                }
            }
        }

        [Column(Name = "PrimaryState", UpdateCheck = UpdateCheck.Never, Storage = "_PrimaryState", DbType = "nvarchar(20)")]
        public string PrimaryState
        {
            get => _PrimaryState;

            set
            {
                if (_PrimaryState != value)
                {
                    OnPrimaryStateChanging(value);
                    SendPropertyChanging();
                    _PrimaryState = value;
                    SendPropertyChanged("PrimaryState");
                    OnPrimaryStateChanged();
                }
            }
        }

        [Column(Name = "HomePhone", UpdateCheck = UpdateCheck.Never, Storage = "_HomePhone", DbType = "nvarchar(20)")]
        public string HomePhone
        {
            get => _HomePhone;

            set
            {
                if (_HomePhone != value)
                {
                    OnHomePhoneChanging(value);
                    SendPropertyChanging();
                    _HomePhone = value;
                    SendPropertyChanged("HomePhone");
                    OnHomePhoneChanged();
                }
            }
        }

        [Column(Name = "SpouseId", UpdateCheck = UpdateCheck.Never, Storage = "_SpouseId", DbType = "int")]
        public int? SpouseId
        {
            get => _SpouseId;

            set
            {
                if (_SpouseId != value)
                {
                    OnSpouseIdChanging(value);
                    SendPropertyChanging();
                    _SpouseId = value;
                    SendPropertyChanged("SpouseId");
                    OnSpouseIdChanged();
                }
            }
        }

        [Column(Name = "PrimaryAddress2", UpdateCheck = UpdateCheck.Never, Storage = "_PrimaryAddress2", DbType = "nvarchar(100)")]
        public string PrimaryAddress2
        {
            get => _PrimaryAddress2;

            set
            {
                if (_PrimaryAddress2 != value)
                {
                    OnPrimaryAddress2Changing(value);
                    SendPropertyChanging();
                    _PrimaryAddress2 = value;
                    SendPropertyChanged("PrimaryAddress2");
                    OnPrimaryAddress2Changed();
                }
            }
        }

        [Column(Name = "PrimaryResCode", UpdateCheck = UpdateCheck.Never, Storage = "_PrimaryResCode", DbType = "int")]
        public int? PrimaryResCode
        {
            get => _PrimaryResCode;

            set
            {
                if (_PrimaryResCode != value)
                {
                    OnPrimaryResCodeChanging(value);
                    SendPropertyChanging();
                    _PrimaryResCode = value;
                    SendPropertyChanged("PrimaryResCode");
                    OnPrimaryResCodeChanged();
                }
            }
        }

        [Column(Name = "PrimaryBadAddrFlag", UpdateCheck = UpdateCheck.Never, Storage = "_PrimaryBadAddrFlag", DbType = "int")]
        public int? PrimaryBadAddrFlag
        {
            get => _PrimaryBadAddrFlag;

            set
            {
                if (_PrimaryBadAddrFlag != value)
                {
                    OnPrimaryBadAddrFlagChanging(value);
                    SendPropertyChanging();
                    _PrimaryBadAddrFlag = value;
                    SendPropertyChanged("PrimaryBadAddrFlag");
                    OnPrimaryBadAddrFlagChanged();
                }
            }
        }

        [Column(Name = "LastContact", UpdateCheck = UpdateCheck.Never, Storage = "_LastContact", DbType = "datetime")]
        public DateTime? LastContact
        {
            get => _LastContact;

            set
            {
                if (_LastContact != value)
                {
                    OnLastContactChanging(value);
                    SendPropertyChanging();
                    _LastContact = value;
                    SendPropertyChanged("LastContact");
                    OnLastContactChanged();
                }
            }
        }

        [Column(Name = "Grade", UpdateCheck = UpdateCheck.Never, Storage = "_Grade", DbType = "int")]
        public int? Grade
        {
            get => _Grade;

            set
            {
                if (_Grade != value)
                {
                    OnGradeChanging(value);
                    SendPropertyChanging();
                    _Grade = value;
                    SendPropertyChanged("Grade");
                    OnGradeChanged();
                }
            }
        }

        [Column(Name = "CellPhoneLU", UpdateCheck = UpdateCheck.Never, Storage = "_CellPhoneLU", DbType = "char(7)")]
        public string CellPhoneLU
        {
            get => _CellPhoneLU;

            set
            {
                if (_CellPhoneLU != value)
                {
                    OnCellPhoneLUChanging(value);
                    SendPropertyChanging();
                    _CellPhoneLU = value;
                    SendPropertyChanged("CellPhoneLU");
                    OnCellPhoneLUChanged();
                }
            }
        }

        [Column(Name = "WorkPhoneLU", UpdateCheck = UpdateCheck.Never, Storage = "_WorkPhoneLU", DbType = "char(7)")]
        public string WorkPhoneLU
        {
            get => _WorkPhoneLU;

            set
            {
                if (_WorkPhoneLU != value)
                {
                    OnWorkPhoneLUChanging(value);
                    SendPropertyChanging();
                    _WorkPhoneLU = value;
                    SendPropertyChanged("WorkPhoneLU");
                    OnWorkPhoneLUChanged();
                }
            }
        }

        [Column(Name = "BibleFellowshipClassId", UpdateCheck = UpdateCheck.Never, Storage = "_BibleFellowshipClassId", DbType = "int")]
        [IsForeignKey]
        public int? BibleFellowshipClassId
        {
            get => _BibleFellowshipClassId;

            set
            {
                if (_BibleFellowshipClassId != value)
                {
                    if (_BFClass.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnBibleFellowshipClassIdChanging(value);
                    SendPropertyChanging();
                    _BibleFellowshipClassId = value;
                    SendPropertyChanged("BibleFellowshipClassId");
                    OnBibleFellowshipClassIdChanged();
                }
            }
        }

        [Column(Name = "CampusId", UpdateCheck = UpdateCheck.Never, Storage = "_CampusId", DbType = "int")]
        [IsForeignKey]
        public int? CampusId
        {
            get => _CampusId;

            set
            {
                if (_CampusId != value)
                {
                    if (_Campu.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnCampusIdChanging(value);
                    SendPropertyChanging();
                    _CampusId = value;
                    SendPropertyChanged("CampusId");
                    OnCampusIdChanged();
                }
            }
        }

        [Column(Name = "CellPhoneAC", UpdateCheck = UpdateCheck.Never, Storage = "_CellPhoneAC", DbType = "char(3)")]
        public string CellPhoneAC
        {
            get => _CellPhoneAC;

            set
            {
                if (_CellPhoneAC != value)
                {
                    OnCellPhoneACChanging(value);
                    SendPropertyChanging();
                    _CellPhoneAC = value;
                    SendPropertyChanged("CellPhoneAC");
                    OnCellPhoneACChanged();
                }
            }
        }

        [Column(Name = "CheckInNotes", UpdateCheck = UpdateCheck.Never, Storage = "_CheckInNotes", DbType = "nvarchar(1000)")]
        public string CheckInNotes
        {
            get => _CheckInNotes;

            set
            {
                if (_CheckInNotes != value)
                {
                    OnCheckInNotesChanging(value);
                    SendPropertyChanging();
                    _CheckInNotes = value;
                    SendPropertyChanged("CheckInNotes");
                    OnCheckInNotesChanged();
                }
            }
        }

        [Column(Name = "Age", UpdateCheck = UpdateCheck.Never, Storage = "_Age", DbType = "int", IsDbGenerated = true)]
        public int? Age
        {
            get => _Age;

            set
            {
                if (_Age != value)
                {
                    OnAgeChanging(value);
                    SendPropertyChanging();
                    _Age = value;
                    SendPropertyChanged("Age");
                    OnAgeChanged();
                }
            }
        }

        [Column(Name = "AltName", UpdateCheck = UpdateCheck.Never, Storage = "_AltName", DbType = "nvarchar(100)")]
        public string AltName
        {
            get => _AltName;

            set
            {
                if (_AltName != value)
                {
                    OnAltNameChanging(value);
                    SendPropertyChanging();
                    _AltName = value;
                    SendPropertyChanged("AltName");
                    OnAltNameChanged();
                }
            }
        }

        [Column(Name = "CustodyIssue", UpdateCheck = UpdateCheck.Never, Storage = "_CustodyIssue", DbType = "bit")]
        public bool? CustodyIssue
        {
            get => _CustodyIssue;

            set
            {
                if (_CustodyIssue != value)
                {
                    OnCustodyIssueChanging(value);
                    SendPropertyChanging();
                    _CustodyIssue = value;
                    SendPropertyChanged("CustodyIssue");
                    OnCustodyIssueChanged();
                }
            }
        }

        [Column(Name = "OkTransport", UpdateCheck = UpdateCheck.Never, Storage = "_OkTransport", DbType = "bit")]
        public bool? OkTransport
        {
            get => _OkTransport;

            set
            {
                if (_OkTransport != value)
                {
                    OnOkTransportChanging(value);
                    SendPropertyChanging();
                    _OkTransport = value;
                    SendPropertyChanged("OkTransport");
                    OnOkTransportChanged();
                }
            }
        }

        [Column(Name = "BDate", UpdateCheck = UpdateCheck.Never, Storage = "_BDate", DbType = "datetime", IsDbGenerated = true)]
        public DateTime? BDate
        {
            get => _BDate;

            set
            {
                if (_BDate != value)
                {
                    OnBDateChanging(value);
                    SendPropertyChanging();
                    _BDate = value;
                    SendPropertyChanged("BDate");
                    OnBDateChanged();
                }
            }
        }

        [Column(Name = "HasDuplicates", UpdateCheck = UpdateCheck.Never, Storage = "_HasDuplicates", DbType = "bit")]
        public bool? HasDuplicates
        {
            get => _HasDuplicates;

            set
            {
                if (_HasDuplicates != value)
                {
                    OnHasDuplicatesChanging(value);
                    SendPropertyChanging();
                    _HasDuplicates = value;
                    SendPropertyChanged("HasDuplicates");
                    OnHasDuplicatesChanged();
                }
            }
        }

        [Column(Name = "FirstName2", UpdateCheck = UpdateCheck.Never, Storage = "_FirstName2", DbType = "nvarchar(50)")]
        public string FirstName2
        {
            get => _FirstName2;

            set
            {
                if (_FirstName2 != value)
                {
                    OnFirstName2Changing(value);
                    SendPropertyChanging();
                    _FirstName2 = value;
                    SendPropertyChanged("FirstName2");
                    OnFirstName2Changed();
                }
            }
        }

        [Column(Name = "EmailAddress2", UpdateCheck = UpdateCheck.Never, Storage = "_EmailAddress2", DbType = "nvarchar(60)")]
        public string EmailAddress2
        {
            get => _EmailAddress2;

            set
            {
                if (_EmailAddress2 != value)
                {
                    OnEmailAddress2Changing(value);
                    SendPropertyChanging();
                    _EmailAddress2 = value;
                    SendPropertyChanged("EmailAddress2");
                    OnEmailAddress2Changed();
                }
            }
        }

        [Column(Name = "SendEmailAddress1", UpdateCheck = UpdateCheck.Never, Storage = "_SendEmailAddress1", DbType = "bit")]
        public bool? SendEmailAddress1
        {
            get => _SendEmailAddress1;

            set
            {
                if (_SendEmailAddress1 != value)
                {
                    OnSendEmailAddress1Changing(value);
                    SendPropertyChanging();
                    _SendEmailAddress1 = value;
                    SendPropertyChanged("SendEmailAddress1");
                    OnSendEmailAddress1Changed();
                }
            }
        }

        [Column(Name = "SendEmailAddress2", UpdateCheck = UpdateCheck.Never, Storage = "_SendEmailAddress2", DbType = "bit")]
        public bool? SendEmailAddress2
        {
            get => _SendEmailAddress2;

            set
            {
                if (_SendEmailAddress2 != value)
                {
                    OnSendEmailAddress2Changing(value);
                    SendPropertyChanging();
                    _SendEmailAddress2 = value;
                    SendPropertyChanged("SendEmailAddress2");
                    OnSendEmailAddress2Changed();
                }
            }
        }

        [Column(Name = "NewMemberClassDate", UpdateCheck = UpdateCheck.Never, Storage = "_NewMemberClassDate", DbType = "datetime")]
        public DateTime? NewMemberClassDate
        {
            get => _NewMemberClassDate;

            set
            {
                if (_NewMemberClassDate != value)
                {
                    OnNewMemberClassDateChanging(value);
                    SendPropertyChanging();
                    _NewMemberClassDate = value;
                    SendPropertyChanged("NewMemberClassDate");
                    OnNewMemberClassDateChanged();
                }
            }
        }

        [Column(Name = "PrimaryCountry", UpdateCheck = UpdateCheck.Never, Storage = "_PrimaryCountry", DbType = "nvarchar(40)")]
        public string PrimaryCountry
        {
            get => _PrimaryCountry;

            set
            {
                if (_PrimaryCountry != value)
                {
                    OnPrimaryCountryChanging(value);
                    SendPropertyChanging();
                    _PrimaryCountry = value;
                    SendPropertyChanged("PrimaryCountry");
                    OnPrimaryCountryChanged();
                }
            }
        }

        [Column(Name = "ReceiveSMS", UpdateCheck = UpdateCheck.Never, Storage = "_ReceiveSMS", DbType = "bit NOT NULL")]
        public bool ReceiveSMS
        {
            get => _ReceiveSMS;

            set
            {
                if (_ReceiveSMS != value)
                {
                    OnReceiveSMSChanging(value);
                    SendPropertyChanging();
                    _ReceiveSMS = value;
                    SendPropertyChanged("ReceiveSMS");
                    OnReceiveSMSChanged();
                }
            }
        }

        [Column(Name = "DoNotPublishPhones", UpdateCheck = UpdateCheck.Never, Storage = "_DoNotPublishPhones", DbType = "bit")]
        public bool? DoNotPublishPhones
        {
            get => _DoNotPublishPhones;

            set
            {
                if (_DoNotPublishPhones != value)
                {
                    OnDoNotPublishPhonesChanging(value);
                    SendPropertyChanging();
                    _DoNotPublishPhones = value;
                    SendPropertyChanged("DoNotPublishPhones");
                    OnDoNotPublishPhonesChanged();
                }
            }
        }

        [Column(Name = "IsDeceased", UpdateCheck = UpdateCheck.Never, Storage = "_IsDeceased", DbType = "bit", IsDbGenerated = true)]
        public bool? IsDeceased
        {
            get => _IsDeceased;

            set
            {
                if (_IsDeceased != value)
                {
                    OnIsDeceasedChanging(value);
                    SendPropertyChanging();
                    _IsDeceased = value;
                    SendPropertyChanged("IsDeceased");
                    OnIsDeceasedChanged();
                }
            }
        }

        [Column(Name = "SSN", UpdateCheck = UpdateCheck.Never, Storage = "_Ssn", DbType = "nvarchar(50)")]
        public string Ssn
        {
            get => _Ssn;

            set
            {
                if (_Ssn != value)
                {
                    OnSsnChanging(value);
                    SendPropertyChanging();
                    _Ssn = value;
                    SendPropertyChanged("Ssn");
                    OnSsnChanged();
                }
            }
        }

        [Column(Name = "DLN", UpdateCheck = UpdateCheck.Never, Storage = "_Dln", DbType = "nvarchar(75)")]
        public string Dln
        {
            get => _Dln;

            set
            {
                if (_Dln != value)
                {
                    OnDlnChanging(value);
                    SendPropertyChanging();
                    _Dln = value;
                    SendPropertyChanged("Dln");
                    OnDlnChanged();
                }
            }
        }

        [Column(Name = "DLStateID", UpdateCheck = UpdateCheck.Never, Storage = "_DLStateID", DbType = "int")]
        public int? DLStateID
        {
            get => _DLStateID;

            set
            {
                if (_DLStateID != value)
                {
                    OnDLStateIDChanging(value);
                    SendPropertyChanging();
                    _DLStateID = value;
                    SendPropertyChanged("DLStateID");
                    OnDLStateIDChanged();
                }
            }
        }

        [Column(Name = "HashNum", UpdateCheck = UpdateCheck.Never, Storage = "_HashNum", DbType = "int", IsDbGenerated = true)]
        public int? HashNum
        {
            get => _HashNum;

            set
            {
                if (_HashNum != value)
                {
                    OnHashNumChanging(value);
                    SendPropertyChanging();
                    _HashNum = value;
                    SendPropertyChanged("HashNum");
                    OnHashNumChanged();
                }
            }
        }

        [Column(Name = "PreferredName", UpdateCheck = UpdateCheck.Never, Storage = "_PreferredName", DbType = "nvarchar(25)", IsDbGenerated = true)]
        public string PreferredName
        {
            get => _PreferredName;

            set
            {
                if (_PreferredName != value)
                {
                    OnPreferredNameChanging(value);
                    SendPropertyChanging();
                    _PreferredName = value;
                    SendPropertyChanged("PreferredName");
                    OnPreferredNameChanged();
                }
            }
        }

        [Column(Name = "Name2", UpdateCheck = UpdateCheck.Never, Storage = "_Name2", DbType = "nvarchar(139)", IsDbGenerated = true)]
        public string Name2
        {
            get => _Name2;

            set
            {
                if (_Name2 != value)
                {
                    OnName2Changing(value);
                    SendPropertyChanging();
                    _Name2 = value;
                    SendPropertyChanged("Name2");
                    OnName2Changed();
                }
            }
        }

        [Column(Name = "Name", UpdateCheck = UpdateCheck.Never, Storage = "_Name", DbType = "nvarchar(138)", IsDbGenerated = true)]
        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    OnNameChanging(value);
                    SendPropertyChanging();
                    _Name = value;
                    SendPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }

        [Column(Name = "ElectronicStatement", UpdateCheck = UpdateCheck.Never, Storage = "_ElectronicStatement", DbType = "bit")]
        public bool? ElectronicStatement
        {
            get => _ElectronicStatement;

            set
            {
                if (_ElectronicStatement != value)
                {
                    OnElectronicStatementChanging(value);
                    SendPropertyChanging();
                    _ElectronicStatement = value;
                    SendPropertyChanged("ElectronicStatement");
                    OnElectronicStatementChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "contactsHad__person", Storage = "_contactsHad", OtherKey = "PeopleId")]
        public EntitySet<Contactee> contactsHad
           {
               get => _contactsHad;

            set => _contactsHad.Assign(value);

           }

        [Association(Name = "contactsMade__person", Storage = "_contactsMade", OtherKey = "PeopleId")]
        public EntitySet<Contactor> contactsMade
           {
               get => _contactsMade;

            set => _contactsMade.Assign(value);

           }

        [Association(Name = "ENROLLMENT_TRANSACTION_PPL_FK", Storage = "_EnrollmentTransactions", OtherKey = "PeopleId")]
        public EntitySet<EnrollmentTransaction> EnrollmentTransactions
           {
               get => _EnrollmentTransactions;

            set => _EnrollmentTransactions.Assign(value);

           }

        [Association(Name = "FamiliesHeaded__HeadOfHousehold", Storage = "_FamiliesHeaded", OtherKey = "HeadOfHouseholdId")]
        public EntitySet<Family> FamiliesHeaded
           {
               get => _FamiliesHeaded;

            set => _FamiliesHeaded.Assign(value);

           }

        [Association(Name = "FamiliesHeaded2__HeadOfHouseholdSpouse", Storage = "_FamiliesHeaded2", OtherKey = "HeadOfHouseholdSpouseId")]
        public EntitySet<Family> FamiliesHeaded2
           {
               get => _FamiliesHeaded2;

            set => _FamiliesHeaded2.Assign(value);

           }

        [Association(Name = "FK_AttendWithAbsents_TBL_PEOPLE_TBL", Storage = "_Attends", OtherKey = "PeopleId")]
        public EntitySet<Attend> Attends
           {
               get => _Attends;

            set => _Attends.Assign(value);

           }

        [Association(Name = "FK_BackgroundChecks_People", Storage = "_BackgroundChecks", OtherKey = "PeopleID")]
        public EntitySet<BackgroundCheck> BackgroundChecks
           {
               get => _BackgroundChecks;

            set => _BackgroundChecks.Assign(value);

           }

        [Association(Name = "FK_CardIdentifiers_People", Storage = "_CardIdentifiers", OtherKey = "PeopleId")]
        public EntitySet<CardIdentifier> CardIdentifiers
           {
               get => _CardIdentifiers;

            set => _CardIdentifiers.Assign(value);

           }

        [Association(Name = "FK_CheckInTimes_People", Storage = "_CheckInTimes", OtherKey = "PeopleId")]
        public EntitySet<CheckInTime> CheckInTimes
           {
               get => _CheckInTimes;

            set => _CheckInTimes.Assign(value);

           }

        [Association(Name = "FK_Contribution_People", Storage = "_Contributions", OtherKey = "PeopleId")]
        public EntitySet<Contribution> Contributions
           {
               get => _Contributions;

            set => _Contributions.Assign(value);

           }

        [Association(Name = "FK_Coupons_People", Storage = "_Coupons", OtherKey = "PeopleId")]
        public EntitySet<Coupon> Coupons
           {
               get => _Coupons;

            set => _Coupons.Assign(value);

           }

        [Association(Name = "FK_EmailOptOut_People", Storage = "_EmailOptOuts", OtherKey = "ToPeopleId")]
        public EntitySet<EmailOptOut> EmailOptOuts
           {
               get => _EmailOptOuts;

            set => _EmailOptOuts.Assign(value);

           }

        [Association(Name = "FK_EmailQueue_People", Storage = "_EmailQueues", OtherKey = "QueuedBy")]
        public EntitySet<EmailQueue> EmailQueues
           {
               get => _EmailQueues;

            set => _EmailQueues.Assign(value);

           }

        [Association(Name = "FK_EmailQueueTo_People", Storage = "_EmailQueueTos", OtherKey = "PeopleId")]
        public EntitySet<EmailQueueTo> EmailQueueTos
           {
               get => _EmailQueueTos;

            set => _EmailQueueTos.Assign(value);

           }

        [Association(Name = "FK_EmailResponses_People", Storage = "_EmailResponses", OtherKey = "PeopleId")]
        public EntitySet<EmailResponse> EmailResponses
           {
               get => _EmailResponses;

            set => _EmailResponses.Assign(value);

           }

        [Association(Name = "FK_Goers__Supporter", Storage = "_FK_Goers", OtherKey = "SupporterId")]
        public EntitySet<GoerSupporter> FK_Goers
           {
               get => _FK_Goers;

            set => _FK_Goers.Assign(value);

           }

        [Association(Name = "FK_ManagedGiving_People", Storage = "_ManagedGivings", OtherKey = "PeopleId")]
        public EntitySet<ManagedGiving> ManagedGivings
           {
               get => _ManagedGivings;

            set => _ManagedGivings.Assign(value);

           }

        [Association(Name = "FK_MemberDocForm_PEOPLE_TBL", Storage = "_MemberDocForms", OtherKey = "PeopleId")]
        public EntitySet<MemberDocForm> MemberDocForms
           {
               get => _MemberDocForms;

            set => _MemberDocForms.Assign(value);

           }

        [Association(Name = "FK_MobileAppDevices_People", Storage = "_MobileAppDevices", OtherKey = "PeopleID")]
        public EntitySet<MobileAppDevice> MobileAppDevices
           {
               get => _MobileAppDevices;

            set => _MobileAppDevices.Assign(value);

           }

        [Association(Name = "FK_MobileAppPushRegistrations_People", Storage = "_MobileAppPushRegistrations", OtherKey = "PeopleId")]
        public EntitySet<MobileAppPushRegistration> MobileAppPushRegistrations
           {
               get => _MobileAppPushRegistrations;

            set => _MobileAppPushRegistrations.Assign(value);

           }

        [Association(Name = "FK_OrgMemberExtra_People", Storage = "_OrgMemberExtras", OtherKey = "PeopleId")]
        public EntitySet<OrgMemberExtra> OrgMemberExtras
           {
               get => _OrgMemberExtras;

            set => _OrgMemberExtras.Assign(value);

           }

        [Association(Name = "FK_PaymentInfo_People", Storage = "_PaymentInfos", OtherKey = "PeopleId")]
        public EntitySet<PaymentInfo> PaymentInfos
           {
               get => _PaymentInfos;

            set => _PaymentInfos.Assign(value);

           }

        [Association(Name = "FK_PeopleExtra_People", Storage = "_PeopleExtras", OtherKey = "PeopleId")]
        public EntitySet<PeopleExtra> PeopleExtras
           {
               get => _PeopleExtras;

            set => _PeopleExtras.Assign(value);

           }

        [Association(Name = "FK_PrevOrgMemberExtra_People]", Storage = "_PrevOrgMemberExtras", OtherKey = "PeopleId")]
        public EntitySet<PrevOrgMemberExtra> PrevOrgMemberExtras
           {
               get => _PrevOrgMemberExtras;

            set => _PrevOrgMemberExtras.Assign(value);

           }

        [Association(Name = "FK_RecReg_People", Storage = "_RecRegs", OtherKey = "PeopleId")]
        public EntitySet<RecReg> RecRegs
           {
               get => _RecRegs;

            set => _RecRegs.Assign(value);

           }

        [Association(Name = "FK_RecurringAmounts_People", Storage = "_RecurringAmounts", OtherKey = "PeopleId")]
        public EntitySet<RecurringAmount> RecurringAmounts
           {
               get => _RecurringAmounts;

            set => _RecurringAmounts.Assign(value);

           }

        [Association(Name = "FK_SMSItems_People", Storage = "_SMSItems", OtherKey = "PeopleID")]
        public EntitySet<SMSItem> SMSItems
           {
               get => _SMSItems;

            set => _SMSItems.Assign(value);

           }

        [Association(Name = "FK_SMSList_People", Storage = "_SMSLists", OtherKey = "SenderID")]
        public EntitySet<SMSList> SMSLists
           {
               get => _SMSLists;

            set => _SMSLists.Assign(value);

           }

        [Association(Name = "FK_Supporters__Goer", Storage = "_FK_Supporters", OtherKey = "GoerId")]
        public EntitySet<GoerSupporter> FK_Supporters
           {
               get => _FK_Supporters;

            set => _FK_Supporters.Assign(value);

           }

        [Association(Name = "FK_TagShare_People", Storage = "_TagShares", OtherKey = "PeopleId")]
        public EntitySet<TagShare> TagShares
           {
               get => _TagShares;

            set => _TagShares.Assign(value);

           }

        [Association(Name = "FK_TaskListOwners_PEOPLE_TBL", Storage = "_TaskListOwners", OtherKey = "PeopleId")]
        public EntitySet<TaskListOwner> TaskListOwners
           {
               get => _TaskListOwners;

            set => _TaskListOwners.Assign(value);

           }

        [Association(Name = "FK_Transaction_People", Storage = "_Transactions", OtherKey = "LoginPeopleId")]
        public EntitySet<Transaction> Transactions
           {
               get => _Transactions;

            set => _Transactions.Assign(value);

           }

        [Association(Name = "FK_TransactionPeople_Person", Storage = "_TransactionPeople", OtherKey = "PeopleId")]
        public EntitySet<TransactionPerson> TransactionPeople
           {
               get => _TransactionPeople;

            set => _TransactionPeople.Assign(value);

           }

        [Association(Name = "FK_Users_People", Storage = "_Users", OtherKey = "PeopleId")]
        public EntitySet<User> Users
           {
               get => _Users;

            set => _Users.Assign(value);

           }

        [Association(Name = "FK_VolInterestInterestCodes_People", Storage = "_VolInterestInterestCodes", OtherKey = "PeopleId")]
        public EntitySet<VolInterestInterestCode> VolInterestInterestCodes
           {
               get => _VolInterestInterestCodes;

            set => _VolInterestInterestCodes.Assign(value);

           }

        [Association(Name = "FK_Volunteer_PEOPLE_TBL", Storage = "_Volunteers", OtherKey = "PeopleId")]
        public EntitySet<Volunteer> Volunteers
           {
               get => _Volunteers;

            set => _Volunteers.Assign(value);

           }

        [Association(Name = "FK_VolunteerForm_PEOPLE_TBL", Storage = "_VolunteerForms", OtherKey = "PeopleId")]
        public EntitySet<VolunteerForm> VolunteerForms
           {
               get => _VolunteerForms;

            set => _VolunteerForms.Assign(value);

           }

        [Association(Name = "FK_VoluteerApprovalIds_People", Storage = "_VoluteerApprovalIds", OtherKey = "PeopleId")]
        public EntitySet<VoluteerApprovalId> VoluteerApprovalIds
           {
               get => _VoluteerApprovalIds;

            set => _VoluteerApprovalIds.Assign(value);

           }

        [Association(Name = "GoerAmounts__Goer", Storage = "_GoerAmounts", OtherKey = "SupporterId")]
        public EntitySet<GoerSenderAmount> GoerAmounts
           {
               get => _GoerAmounts;

            set => _GoerAmounts.Assign(value);

           }

        [Association(Name = "OnBehalfOfPeople__PersonCanEmail", Storage = "_OnBehalfOfPeople", OtherKey = "CanEmail")]
        public EntitySet<PeopleCanEmailFor> OnBehalfOfPeople
           {
               get => _OnBehalfOfPeople;

            set => _OnBehalfOfPeople.Assign(value);

           }

        [Association(Name = "ORGANIZATION_MEMBERS_PPL_FK", Storage = "_OrganizationMembers", OtherKey = "PeopleId")]
        public EntitySet<OrganizationMember> OrganizationMembers
           {
               get => _OrganizationMembers;

            set => _OrganizationMembers.Assign(value);

           }

        [Association(Name = "Org_Member_Documents_PPL_FK", Storage = "_OrgMemberDocuments", OtherKey = "PeopleId")]
        public EntitySet<OrgMemberDocuments> OrgMemberDocuments
        {
            get => _OrgMemberDocuments;

            set => _OrgMemberDocuments.Assign(value);
        }

        [Association(Name = "People__User", Storage = "_People", OtherKey = "UserID")]
        public EntitySet<BackgroundCheck> People
           {
               get => _People;

            set => _People.Assign(value);

           }

        [Association(Name = "PersonsCanEmail__OnBehalfOfPerson", Storage = "_PersonsCanEmail", OtherKey = "OnBehalfOf")]
        public EntitySet<PeopleCanEmailFor> PersonsCanEmail
           {
               get => _PersonsCanEmail;

            set => _PersonsCanEmail.Assign(value);

           }

        [Association(Name = "SenderAmounts__Sender", Storage = "_SenderAmounts", OtherKey = "GoerId")]
        public EntitySet<GoerSenderAmount> SenderAmounts
           {
               get => _SenderAmounts;

            set => _SenderAmounts.Assign(value);

           }

        [Association(Name = "SubRequests__Requestor", Storage = "_SubRequests", OtherKey = "RequestorId")]
        public EntitySet<SubRequest> SubRequests
           {
               get => _SubRequests;

            set => _SubRequests.Assign(value);

           }

        [Association(Name = "SubResponses__Substitute", Storage = "_SubResponses", OtherKey = "SubstituteId")]
        public EntitySet<SubRequest> SubResponses
           {
               get => _SubResponses;

            set => _SubResponses.Assign(value);

           }

        [Association(Name = "Tags__Person", Storage = "_Tags", OtherKey = "PeopleId")]
        public EntitySet<TagPerson> Tags
           {
               get => _Tags;

            set => _Tags.Assign(value);

           }

        [Association(Name = "TagsOwned__PersonOwner", Storage = "_TagsOwned", OtherKey = "PeopleId")]
        public EntitySet<Tag> TagsOwned
           {
               get => _TagsOwned;

            set => _TagsOwned.Assign(value);

           }

        [Association(Name = "Tasks__Owner", Storage = "_Tasks", OtherKey = "OwnerId")]
        public EntitySet<Task> Tasks
           {
               get => _Tasks;

            set => _Tasks.Assign(value);

           }

        [Association(Name = "TasksAboutPerson__AboutWho", Storage = "_TasksAboutPerson", OtherKey = "WhoId")]
        public EntitySet<Task> TasksAboutPerson
           {
               get => _TasksAboutPerson;

            set => _TasksAboutPerson.Assign(value);

           }

        [Association(Name = "TasksCoOwned__CoOwner", Storage = "_TasksCoOwned", OtherKey = "CoOwnerId")]
        public EntitySet<Task> TasksCoOwned
           {
               get => _TasksCoOwned;

            set => _TasksCoOwned.Assign(value);

           }

        [Association(Name = "VolRequests__Requestor", Storage = "_VolRequests", OtherKey = "RequestorId")]
        public EntitySet<VolRequest> VolRequests
           {
               get => _VolRequests;

            set => _VolRequests.Assign(value);

           }

        [Association(Name = "VolResponses__Volunteer", Storage = "_VolResponses", OtherKey = "VolunteerId")]
        public EntitySet<VolRequest> VolResponses
           {
               get => _VolResponses;

            set => _VolResponses.Assign(value);

           }

        #endregion

        #region Foreign Keys

        [Association(Name = "BFMembers__BFClass", Storage = "_BFClass", ThisKey = "BibleFellowshipClassId", IsForeignKey = true)]
        public Organization BFClass
        {
            get => _BFClass.Entity;

            set
            {
                Organization previousValue = _BFClass.Entity;
                if (((previousValue != value)
                            || (_BFClass.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _BFClass.Entity = null;
                        previousValue.BFMembers.Remove(this);
                    }

                    _BFClass.Entity = value;
                    if (value != null)
                    {
                        value.BFMembers.Add(this);

                        _BibleFellowshipClassId = value.OrganizationId;

                    }

                    else
                    {
                        _BibleFellowshipClassId = default(int?);

                    }

                    SendPropertyChanged("BFClass");
                }
            }
        }

        [Association(Name = "EnvPeople__EnvelopeOption", Storage = "_EnvelopeOption", ThisKey = "EnvelopeOptionsId", IsForeignKey = true)]
        public EnvelopeOption EnvelopeOption
        {
            get => _EnvelopeOption.Entity;

            set
            {
                EnvelopeOption previousValue = _EnvelopeOption.Entity;
                if (((previousValue != value)
                            || (_EnvelopeOption.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _EnvelopeOption.Entity = null;
                        previousValue.EnvPeople.Remove(this);
                    }

                    _EnvelopeOption.Entity = value;
                    if (value != null)
                    {
                        value.EnvPeople.Add(this);

                        _EnvelopeOptionsId = value.Id;

                    }

                    else
                    {
                        _EnvelopeOptionsId = default(int?);

                    }

                    SendPropertyChanged("EnvelopeOption");
                }
            }
        }

        [Association(Name = "FK_People_BaptismStatus", Storage = "_BaptismStatus", ThisKey = "BaptismStatusId", IsForeignKey = true)]
        public BaptismStatus BaptismStatus
        {
            get => _BaptismStatus.Entity;

            set
            {
                BaptismStatus previousValue = _BaptismStatus.Entity;
                if (((previousValue != value)
                            || (_BaptismStatus.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _BaptismStatus.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _BaptismStatus.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _BaptismStatusId = value.Id;

                    }

                    else
                    {
                        _BaptismStatusId = default(int?);

                    }

                    SendPropertyChanged("BaptismStatus");
                }
            }
        }

        [Association(Name = "FK_People_BaptismType", Storage = "_BaptismType", ThisKey = "BaptismTypeId", IsForeignKey = true)]
        public BaptismType BaptismType
        {
            get => _BaptismType.Entity;

            set
            {
                BaptismType previousValue = _BaptismType.Entity;
                if (((previousValue != value)
                            || (_BaptismType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _BaptismType.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _BaptismType.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _BaptismTypeId = value.Id;

                    }

                    else
                    {
                        _BaptismTypeId = default(int?);

                    }

                    SendPropertyChanged("BaptismType");
                }
            }
        }

        [Association(Name = "FK_People_Campus", Storage = "_Campu", ThisKey = "CampusId", IsForeignKey = true)]
        public Campu Campu
        {
            get => _Campu.Entity;

            set
            {
                Campu previousValue = _Campu.Entity;
                if (((previousValue != value)
                            || (_Campu.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Campu.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _Campu.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _CampusId = value.Id;

                    }

                    else
                    {
                        _CampusId = default(int?);

                    }

                    SendPropertyChanged("Campu");
                }
            }
        }

        [Association(Name = "FK_People_DecisionType", Storage = "_DecisionType", ThisKey = "DecisionTypeId", IsForeignKey = true)]
        public DecisionType DecisionType
        {
            get => _DecisionType.Entity;

            set
            {
                DecisionType previousValue = _DecisionType.Entity;
                if (((previousValue != value)
                            || (_DecisionType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _DecisionType.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _DecisionType.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _DecisionTypeId = value.Id;

                    }

                    else
                    {
                        _DecisionTypeId = default(int?);

                    }

                    SendPropertyChanged("DecisionType");
                }
            }
        }

        [Association(Name = "FK_People_DiscoveryClassStatus", Storage = "_NewMemberClassStatus", ThisKey = "NewMemberClassStatusId", IsForeignKey = true)]
        public NewMemberClassStatus NewMemberClassStatus
        {
            get => _NewMemberClassStatus.Entity;

            set
            {
                NewMemberClassStatus previousValue = _NewMemberClassStatus.Entity;
                if (((previousValue != value)
                            || (_NewMemberClassStatus.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _NewMemberClassStatus.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _NewMemberClassStatus.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _NewMemberClassStatusId = value.Id;

                    }

                    else
                    {
                        _NewMemberClassStatusId = default(int?);

                    }

                    SendPropertyChanged("NewMemberClassStatus");
                }
            }
        }

        [Association(Name = "FK_People_DropType", Storage = "_DropType", ThisKey = "DropCodeId", IsForeignKey = true)]
        public DropType DropType
        {
            get => _DropType.Entity;

            set
            {
                DropType previousValue = _DropType.Entity;
                if (((previousValue != value)
                            || (_DropType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _DropType.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _DropType.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _DropCodeId = value.Id;

                    }

                    else
                    {
                        _DropCodeId = default(int);

                    }

                    SendPropertyChanged("DropType");
                }
            }
        }

        [Association(Name = "FK_People_EntryPoint", Storage = "_EntryPoint", ThisKey = "EntryPointId", IsForeignKey = true)]
        public EntryPoint EntryPoint
        {
            get => _EntryPoint.Entity;

            set
            {
                EntryPoint previousValue = _EntryPoint.Entity;
                if (((previousValue != value)
                            || (_EntryPoint.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _EntryPoint.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _EntryPoint.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _EntryPointId = value.Id;

                    }

                    else
                    {
                        _EntryPointId = default(int?);

                    }

                    SendPropertyChanged("EntryPoint");
                }
            }
        }

        [Association(Name = "FK_People_Families", Storage = "_Family", ThisKey = "FamilyId", IsForeignKey = true)]
        public Family Family
        {
            get => _Family.Entity;

            set
            {
                Family previousValue = _Family.Entity;
                if (((previousValue != value)
                            || (_Family.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Family.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _Family.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _FamilyId = value.FamilyId;

                    }

                    else
                    {
                        _FamilyId = default(int);

                    }

                    SendPropertyChanged("Family");
                }
            }
        }

        [Association(Name = "FK_People_FamilyPosition", Storage = "_FamilyPosition", ThisKey = "PositionInFamilyId", IsForeignKey = true)]
        public FamilyPosition FamilyPosition
        {
            get => _FamilyPosition.Entity;

            set
            {
                FamilyPosition previousValue = _FamilyPosition.Entity;
                if (((previousValue != value)
                            || (_FamilyPosition.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _FamilyPosition.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _FamilyPosition.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _PositionInFamilyId = value.Id;

                    }

                    else
                    {
                        _PositionInFamilyId = default(int);

                    }

                    SendPropertyChanged("FamilyPosition");
                }
            }
        }

        [Association(Name = "FK_People_Gender", Storage = "_Gender", ThisKey = "GenderId", IsForeignKey = true)]
        public Gender Gender
        {
            get => _Gender.Entity;

            set
            {
                Gender previousValue = _Gender.Entity;
                if (((previousValue != value)
                            || (_Gender.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Gender.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _Gender.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _GenderId = value.Id;

                    }

                    else
                    {
                        _GenderId = default(int);

                    }

                    SendPropertyChanged("Gender");
                }
            }
        }

        [Association(Name = "FK_People_InterestPoint", Storage = "_InterestPoint", ThisKey = "InterestPointId", IsForeignKey = true)]
        public InterestPoint InterestPoint
        {
            get => _InterestPoint.Entity;

            set
            {
                InterestPoint previousValue = _InterestPoint.Entity;
                if (((previousValue != value)
                            || (_InterestPoint.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _InterestPoint.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _InterestPoint.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _InterestPointId = value.Id;

                    }

                    else
                    {
                        _InterestPointId = default(int?);

                    }

                    SendPropertyChanged("InterestPoint");
                }
            }
        }

        [Association(Name = "FK_People_JoinType", Storage = "_JoinType", ThisKey = "JoinCodeId", IsForeignKey = true)]
        public JoinType JoinType
        {
            get => _JoinType.Entity;

            set
            {
                JoinType previousValue = _JoinType.Entity;
                if (((previousValue != value)
                            || (_JoinType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _JoinType.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _JoinType.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _JoinCodeId = value.Id;

                    }

                    else
                    {
                        _JoinCodeId = default(int);

                    }

                    SendPropertyChanged("JoinType");
                }
            }
        }

        [Association(Name = "FK_People_MaritalStatus", Storage = "_MaritalStatus", ThisKey = "MaritalStatusId", IsForeignKey = true)]
        public MaritalStatus MaritalStatus
        {
            get => _MaritalStatus.Entity;

            set
            {
                MaritalStatus previousValue = _MaritalStatus.Entity;
                if (((previousValue != value)
                            || (_MaritalStatus.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _MaritalStatus.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _MaritalStatus.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _MaritalStatusId = value.Id;

                    }

                    else
                    {
                        _MaritalStatusId = default(int);

                    }

                    SendPropertyChanged("MaritalStatus");
                }
            }
        }

        [Association(Name = "FK_People_MemberLetterStatus", Storage = "_MemberLetterStatus", ThisKey = "LetterStatusId", IsForeignKey = true)]
        public MemberLetterStatus MemberLetterStatus
        {
            get => _MemberLetterStatus.Entity;

            set
            {
                MemberLetterStatus previousValue = _MemberLetterStatus.Entity;
                if (((previousValue != value)
                            || (_MemberLetterStatus.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _MemberLetterStatus.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _MemberLetterStatus.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _LetterStatusId = value.Id;

                    }

                    else
                    {
                        _LetterStatusId = default(int?);

                    }

                    SendPropertyChanged("MemberLetterStatus");
                }
            }
        }

        [Association(Name = "FK_People_MemberStatus", Storage = "_MemberStatus", ThisKey = "MemberStatusId", IsForeignKey = true)]
        public MemberStatus MemberStatus
        {
            get => _MemberStatus.Entity;

            set
            {
                MemberStatus previousValue = _MemberStatus.Entity;
                if (((previousValue != value)
                            || (_MemberStatus.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _MemberStatus.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _MemberStatus.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _MemberStatusId = value.Id;

                    }

                    else
                    {
                        _MemberStatusId = default(int);

                    }

                    SendPropertyChanged("MemberStatus");
                }
            }
        }

        [Association(Name = "FK_People_Origin", Storage = "_Origin", ThisKey = "OriginId", IsForeignKey = true)]
        public Origin Origin
        {
            get => _Origin.Entity;

            set
            {
                Origin previousValue = _Origin.Entity;
                if (((previousValue != value)
                            || (_Origin.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Origin.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _Origin.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _OriginId = value.Id;

                    }

                    else
                    {
                        _OriginId = default(int?);

                    }

                    SendPropertyChanged("Origin");
                }
            }
        }

        [Association(Name = "FK_PEOPLE_TBL_Picture", Storage = "_Picture", ThisKey = "PictureId", IsForeignKey = true)]
        public Picture Picture
        {
            get => _Picture.Entity;

            set
            {
                Picture previousValue = _Picture.Entity;
                if (((previousValue != value)
                            || (_Picture.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Picture.Entity = null;
                        previousValue.People.Remove(this);
                    }

                    _Picture.Entity = value;
                    if (value != null)
                    {
                        value.People.Add(this);

                        _PictureId = value.PictureId;

                    }

                    else
                    {
                        _PictureId = default(int?);

                    }

                    SendPropertyChanged("Picture");
                }
            }
        }

        [Association(Name = "ResCodePeople__ResidentCode", Storage = "_ResidentCode", ThisKey = "ResCodeId", IsForeignKey = true)]
        public ResidentCode ResidentCode
        {
            get => _ResidentCode.Entity;

            set
            {
                ResidentCode previousValue = _ResidentCode.Entity;
                if (((previousValue != value)
                            || (_ResidentCode.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ResidentCode.Entity = null;
                        previousValue.ResCodePeople.Remove(this);
                    }

                    _ResidentCode.Entity = value;
                    if (value != null)
                    {
                        value.ResCodePeople.Add(this);

                        _ResCodeId = value.Id;

                    }

                    else
                    {
                        _ResCodeId = default(int?);

                    }

                    SendPropertyChanged("ResidentCode");
                }
            }
        }

        [Association(Name = "StmtPeople__ContributionStatementOption", Storage = "_ContributionStatementOption", ThisKey = "ContributionOptionsId", IsForeignKey = true)]
        public EnvelopeOption ContributionStatementOption
        {
            get => _ContributionStatementOption.Entity;

            set
            {
                EnvelopeOption previousValue = _ContributionStatementOption.Entity;
                if (((previousValue != value)
                            || (_ContributionStatementOption.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ContributionStatementOption.Entity = null;
                        previousValue.StmtPeople.Remove(this);
                    }

                    _ContributionStatementOption.Entity = value;
                    if (value != null)
                    {
                        value.StmtPeople.Add(this);

                        _ContributionOptionsId = value.Id;

                    }

                    else
                    {
                        _ContributionOptionsId = default(int?);

                    }

                    SendPropertyChanged("ContributionStatementOption");
                }
            }
        }

        #endregion

        public event PropertyChangingEventHandler PropertyChanging;
        protected virtual void SendPropertyChanging()
        {
            if ((PropertyChanging != null))
            {
                PropertyChanging(this, emptyChangingEventArgs);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((PropertyChanged != null))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void attach_contactsHad(Contactee entity)
        {
            SendPropertyChanging();
            entity.person = this;
        }

        private void detach_contactsHad(Contactee entity)
        {
            SendPropertyChanging();
            entity.person = null;
        }

        private void attach_contactsMade(Contactor entity)
        {
            SendPropertyChanging();
            entity.person = this;
        }

        private void detach_contactsMade(Contactor entity)
        {
            SendPropertyChanging();
            entity.person = null;
        }

        private void attach_EnrollmentTransactions(EnrollmentTransaction entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_EnrollmentTransactions(EnrollmentTransaction entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_FamiliesHeaded(Family entity)
        {
            SendPropertyChanging();
            entity.HeadOfHousehold = this;
        }

        private void detach_FamiliesHeaded(Family entity)
        {
            SendPropertyChanging();
            entity.HeadOfHousehold = null;
        }

        private void attach_FamiliesHeaded2(Family entity)
        {
            SendPropertyChanging();
            entity.HeadOfHouseholdSpouse = this;
        }

        private void detach_FamiliesHeaded2(Family entity)
        {
            SendPropertyChanging();
            entity.HeadOfHouseholdSpouse = null;
        }

        private void attach_Attends(Attend entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_Attends(Attend entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_BackgroundChecks(BackgroundCheck entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_BackgroundChecks(BackgroundCheck entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_CardIdentifiers(CardIdentifier entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_CardIdentifiers(CardIdentifier entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_CheckInTimes(CheckInTime entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_CheckInTimes(CheckInTime entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_Contributions(Contribution entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_Contributions(Contribution entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_Coupons(Coupon entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_Coupons(Coupon entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_EmailOptOuts(EmailOptOut entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_EmailOptOuts(EmailOptOut entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_EmailQueues(EmailQueue entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_EmailQueues(EmailQueue entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_EmailQueueTos(EmailQueueTo entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_EmailQueueTos(EmailQueueTo entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_EmailResponses(EmailResponse entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_EmailResponses(EmailResponse entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_FK_Goers(GoerSupporter entity)
        {
            SendPropertyChanging();
            entity.Supporter = this;
        }

        private void detach_FK_Goers(GoerSupporter entity)
        {
            SendPropertyChanging();
            entity.Supporter = null;
        }

        private void attach_ManagedGivings(ManagedGiving entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_ManagedGivings(ManagedGiving entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_MemberDocForms(MemberDocForm entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_MemberDocForms(MemberDocForm entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_MobileAppDevices(MobileAppDevice entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_MobileAppDevices(MobileAppDevice entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_MobileAppPushRegistrations(MobileAppPushRegistration entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_MobileAppPushRegistrations(MobileAppPushRegistration entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_OrgMemberExtras(OrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_OrgMemberExtras(OrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_PaymentInfos(PaymentInfo entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_PaymentInfos(PaymentInfo entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_PeopleExtras(PeopleExtra entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_PeopleExtras(PeopleExtra entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_RecRegs(RecReg entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_RecRegs(RecReg entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_RecurringAmounts(RecurringAmount entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_RecurringAmounts(RecurringAmount entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_SMSItems(SMSItem entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_SMSItems(SMSItem entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_SMSLists(SMSList entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_SMSLists(SMSList entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_FK_Supporters(GoerSupporter entity)
        {
            SendPropertyChanging();
            entity.Goer = this;
        }

        private void detach_FK_Supporters(GoerSupporter entity)
        {
            SendPropertyChanging();
            entity.Goer = null;
        }

        private void attach_TagShares(TagShare entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_TagShares(TagShare entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_TaskListOwners(TaskListOwner entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_TaskListOwners(TaskListOwner entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_Transactions(Transaction entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_Transactions(Transaction entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_TransactionPeople(TransactionPerson entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_TransactionPeople(TransactionPerson entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_Users(User entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_Users(User entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_VolInterestInterestCodes(VolInterestInterestCode entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_VolInterestInterestCodes(VolInterestInterestCode entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_Volunteers(Volunteer entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_Volunteers(Volunteer entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_VolunteerForms(VolunteerForm entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_VolunteerForms(VolunteerForm entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_VoluteerApprovalIds(VoluteerApprovalId entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_VoluteerApprovalIds(VoluteerApprovalId entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_GoerAmounts(GoerSenderAmount entity)
        {
            SendPropertyChanging();
            entity.Goer = this;
        }

        private void detach_GoerAmounts(GoerSenderAmount entity)
        {
            SendPropertyChanging();
            entity.Goer = null;
        }

        private void attach_OnBehalfOfPeople(PeopleCanEmailFor entity)
        {
            SendPropertyChanging();
            entity.PersonCanEmail = this;
        }

        private void detach_OnBehalfOfPeople(PeopleCanEmailFor entity)
        {
            SendPropertyChanging();
            entity.PersonCanEmail = null;
        }

        private void attach_OrganizationMembers(OrganizationMember entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_OrganizationMembers(OrganizationMember entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_OrgMemberDocuments(OrgMemberDocuments entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_OrgMemberDocuments(OrgMemberDocuments entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_People(BackgroundCheck entity)
        {
            SendPropertyChanging();
            entity.User = this;
        }

        private void detach_People(BackgroundCheck entity)
        {
            SendPropertyChanging();
            entity.User = null;
        }

        private void attach_PersonsCanEmail(PeopleCanEmailFor entity)
        {
            SendPropertyChanging();
            entity.OnBehalfOfPerson = this;
        }

        private void detach_PersonsCanEmail(PeopleCanEmailFor entity)
        {
            SendPropertyChanging();
            entity.OnBehalfOfPerson = null;
        }

        private void attach_SenderAmounts(GoerSenderAmount entity)
        {
            SendPropertyChanging();
            entity.Sender = this;
        }

        private void detach_SenderAmounts(GoerSenderAmount entity)
        {
            SendPropertyChanging();
            entity.Sender = null;
        }

        private void attach_SubRequests(SubRequest entity)
        {
            SendPropertyChanging();
            entity.Requestor = this;
        }

        private void detach_SubRequests(SubRequest entity)
        {
            SendPropertyChanging();
            entity.Requestor = null;
        }

        private void attach_SubResponses(SubRequest entity)
        {
            SendPropertyChanging();
            entity.Substitute = this;
        }

        private void detach_SubResponses(SubRequest entity)
        {
            SendPropertyChanging();
            entity.Substitute = null;
        }

        private void attach_Tags(TagPerson entity)
        {
            SendPropertyChanging();
            entity.Person = this;
        }

        private void detach_Tags(TagPerson entity)
        {
            SendPropertyChanging();
            entity.Person = null;
        }

        private void attach_TagsOwned(Tag entity)
        {
            SendPropertyChanging();
            entity.PersonOwner = this;
        }

        private void detach_TagsOwned(Tag entity)
        {
            SendPropertyChanging();
            entity.PersonOwner = null;
        }

        private void attach_Tasks(Task entity)
        {
            SendPropertyChanging();
            entity.Owner = this;
        }

        private void detach_Tasks(Task entity)
        {
            SendPropertyChanging();
            entity.Owner = null;
        }

        private void attach_TasksAboutPerson(Task entity)
        {
            SendPropertyChanging();
            entity.AboutWho = this;
        }

        private void detach_TasksAboutPerson(Task entity)
        {
            SendPropertyChanging();
            entity.AboutWho = null;
        }

        private void attach_TasksCoOwned(Task entity)
        {
            SendPropertyChanging();
            entity.CoOwner = this;
        }

        private void detach_TasksCoOwned(Task entity)
        {
            SendPropertyChanging();
            entity.CoOwner = null;
        }

        private void attach_VolRequests(VolRequest entity)
        {
            SendPropertyChanging();
            entity.Requestor = this;
        }

        private void detach_VolRequests(VolRequest entity)
        {
            SendPropertyChanging();
            entity.Requestor = null;
        }

        private void attach_VolResponses(VolRequest entity)
        {
            SendPropertyChanging();
            entity.Volunteer = this;
        }

        private void detach_VolResponses(VolRequest entity)
        {
            SendPropertyChanging();
            entity.Volunteer = null;
        }
    }
}
