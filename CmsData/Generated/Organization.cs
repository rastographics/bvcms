using CmsData.Infrastructure;
using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace CmsData
{
    [Table(Name = "dbo.Organizations")]
    public partial class Organization : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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

        private EntitySet<OrgMemberDocuments> _OrgMemberDocuments;

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
            _BFMembers = new EntitySet<Person>(new Action<Person>(attach_BFMembers), new Action<Person>(detach_BFMembers));

            _ChildOrgs = new EntitySet<Organization>(new Action<Organization>(attach_ChildOrgs), new Action<Organization>(detach_ChildOrgs));

            _contactsHad = new EntitySet<Contact>(new Action<Contact>(attach_contactsHad), new Action<Contact>(detach_contactsHad));

            _EnrollmentTransactions = new EntitySet<EnrollmentTransaction>(new Action<EnrollmentTransaction>(attach_EnrollmentTransactions), new Action<EnrollmentTransaction>(detach_EnrollmentTransactions));

            _Attends = new EntitySet<Attend>(new Action<Attend>(attach_Attends), new Action<Attend>(detach_Attends));

            _Coupons = new EntitySet<Coupon>(new Action<Coupon>(attach_Coupons), new Action<Coupon>(detach_Coupons));

            _DivOrgs = new EntitySet<DivOrg>(new Action<DivOrg>(attach_DivOrgs), new Action<DivOrg>(detach_DivOrgs));

            _GoerSenderAmounts = new EntitySet<GoerSenderAmount>(new Action<GoerSenderAmount>(attach_GoerSenderAmounts), new Action<GoerSenderAmount>(detach_GoerSenderAmounts));

            _Meetings = new EntitySet<Meeting>(new Action<Meeting>(attach_Meetings), new Action<Meeting>(detach_Meetings));

            _MemberTags = new EntitySet<MemberTag>(new Action<MemberTag>(attach_MemberTags), new Action<MemberTag>(detach_MemberTags));

            _OrganizationExtras = new EntitySet<OrganizationExtra>(new Action<OrganizationExtra>(attach_OrganizationExtras), new Action<OrganizationExtra>(detach_OrganizationExtras));

            _OrgMemberExtras = new EntitySet<OrgMemberExtra>(new Action<OrgMemberExtra>(attach_OrgMemberExtras), new Action<OrgMemberExtra>(detach_OrgMemberExtras));

            _OrgSchedules = new EntitySet<OrgSchedule>(new Action<OrgSchedule>(attach_OrgSchedules), new Action<OrgSchedule>(detach_OrgSchedules));

            _PrevOrgMemberExtras = new EntitySet<PrevOrgMemberExtra>(new Action<PrevOrgMemberExtra>(attach_PrevOrgMemberExtras), new Action<PrevOrgMemberExtra>(detach_PrevOrgMemberExtras));

            _Resources = new EntitySet<Resource>(new Action<Resource>(attach_Resources), new Action<Resource>(detach_Resources));

            _ResourceOrganizations = new EntitySet<ResourceOrganization>(new Action<ResourceOrganization>(attach_ResourceOrganizations), new Action<ResourceOrganization>(detach_ResourceOrganizations));

            _OrganizationMembers = new EntitySet<OrganizationMember>(new Action<OrganizationMember>(attach_OrganizationMembers), new Action<OrganizationMember>(detach_OrganizationMembers));

            _OrgMemberDocuments = new EntitySet<OrgMemberDocuments>(new Action<OrgMemberDocuments>(attach_OrgMemberDocuments), new Action<OrgMemberDocuments>(detach_OrgMemberDocuments));

            _ParentOrg = default(EntityRef<Organization>);

            _Campu = default(EntityRef<Campu>);

            _Division = default(EntityRef<Division>);

            _Gender = default(EntityRef<Gender>);

            _OrganizationType = default(EntityRef<OrganizationType>);

            _EntryPoint = default(EntityRef<EntryPoint>);

            _OrganizationStatus = default(EntityRef<OrganizationStatus>);

            OnCreated();
        }

        #region Columns

        [Column(Name = "OrganizationId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    OnOrganizationIdChanging(value);
                    SendPropertyChanging();
                    _OrganizationId = value;
                    SendPropertyChanged("OrganizationId");
                    OnOrganizationIdChanged();
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

        [Column(Name = "CreatedDate", UpdateCheck = UpdateCheck.Never, Storage = "_CreatedDate", DbType = "datetime NOT NULL")]
        public DateTime CreatedDate
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

        [Column(Name = "OrganizationStatusId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationStatusId", DbType = "int NOT NULL")]
        [IsForeignKey]
        public int OrganizationStatusId
        {
            get => _OrganizationStatusId;

            set
            {
                if (_OrganizationStatusId != value)
                {
                    if (_OrganizationStatus.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOrganizationStatusIdChanging(value);
                    SendPropertyChanging();
                    _OrganizationStatusId = value;
                    SendPropertyChanged("OrganizationStatusId");
                    OnOrganizationStatusIdChanged();
                }
            }
        }

        [Column(Name = "DivisionId", UpdateCheck = UpdateCheck.Never, Storage = "_DivisionId", DbType = "int")]
        [IsForeignKey]
        public int? DivisionId
        {
            get => _DivisionId;

            set
            {
                if (_DivisionId != value)
                {
                    if (_Division.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnDivisionIdChanging(value);
                    SendPropertyChanging();
                    _DivisionId = value;
                    SendPropertyChanged("DivisionId");
                    OnDivisionIdChanged();
                }
            }
        }

        [Column(Name = "LeaderMemberTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_LeaderMemberTypeId", DbType = "int")]
        public int? LeaderMemberTypeId
        {
            get => _LeaderMemberTypeId;

            set
            {
                if (_LeaderMemberTypeId != value)
                {
                    OnLeaderMemberTypeIdChanging(value);
                    SendPropertyChanging();
                    _LeaderMemberTypeId = value;
                    SendPropertyChanged("LeaderMemberTypeId");
                    OnLeaderMemberTypeIdChanged();
                }
            }
        }

        [Column(Name = "GradeAgeStart", UpdateCheck = UpdateCheck.Never, Storage = "_GradeAgeStart", DbType = "int")]
        public int? GradeAgeStart
        {
            get => _GradeAgeStart;

            set
            {
                if (_GradeAgeStart != value)
                {
                    OnGradeAgeStartChanging(value);
                    SendPropertyChanging();
                    _GradeAgeStart = value;
                    SendPropertyChanged("GradeAgeStart");
                    OnGradeAgeStartChanged();
                }
            }
        }

        [Column(Name = "GradeAgeEnd", UpdateCheck = UpdateCheck.Never, Storage = "_GradeAgeEnd", DbType = "int")]
        public int? GradeAgeEnd
        {
            get => _GradeAgeEnd;

            set
            {
                if (_GradeAgeEnd != value)
                {
                    OnGradeAgeEndChanging(value);
                    SendPropertyChanging();
                    _GradeAgeEnd = value;
                    SendPropertyChanged("GradeAgeEnd");
                    OnGradeAgeEndChanged();
                }
            }
        }

        [Column(Name = "RollSheetVisitorWks", UpdateCheck = UpdateCheck.Never, Storage = "_RollSheetVisitorWks", DbType = "int")]
        public int? RollSheetVisitorWks
        {
            get => _RollSheetVisitorWks;

            set
            {
                if (_RollSheetVisitorWks != value)
                {
                    OnRollSheetVisitorWksChanging(value);
                    SendPropertyChanging();
                    _RollSheetVisitorWks = value;
                    SendPropertyChanged("RollSheetVisitorWks");
                    OnRollSheetVisitorWksChanged();
                }
            }
        }

        [Column(Name = "SecurityTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_SecurityTypeId", DbType = "int NOT NULL")]
        public int SecurityTypeId
        {
            get => _SecurityTypeId;

            set
            {
                if (_SecurityTypeId != value)
                {
                    OnSecurityTypeIdChanging(value);
                    SendPropertyChanging();
                    _SecurityTypeId = value;
                    SendPropertyChanged("SecurityTypeId");
                    OnSecurityTypeIdChanged();
                }
            }
        }

        [Column(Name = "FirstMeetingDate", UpdateCheck = UpdateCheck.Never, Storage = "_FirstMeetingDate", DbType = "datetime")]
        public DateTime? FirstMeetingDate
        {
            get => _FirstMeetingDate;

            set
            {
                if (_FirstMeetingDate != value)
                {
                    OnFirstMeetingDateChanging(value);
                    SendPropertyChanging();
                    _FirstMeetingDate = value;
                    SendPropertyChanged("FirstMeetingDate");
                    OnFirstMeetingDateChanged();
                }
            }
        }

        [Column(Name = "LastMeetingDate", UpdateCheck = UpdateCheck.Never, Storage = "_LastMeetingDate", DbType = "datetime")]
        public DateTime? LastMeetingDate
        {
            get => _LastMeetingDate;

            set
            {
                if (_LastMeetingDate != value)
                {
                    OnLastMeetingDateChanging(value);
                    SendPropertyChanging();
                    _LastMeetingDate = value;
                    SendPropertyChanged("LastMeetingDate");
                    OnLastMeetingDateChanged();
                }
            }
        }

        [Column(Name = "OrganizationClosedDate", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationClosedDate", DbType = "datetime")]
        public DateTime? OrganizationClosedDate
        {
            get => _OrganizationClosedDate;

            set
            {
                if (_OrganizationClosedDate != value)
                {
                    OnOrganizationClosedDateChanging(value);
                    SendPropertyChanging();
                    _OrganizationClosedDate = value;
                    SendPropertyChanged("OrganizationClosedDate");
                    OnOrganizationClosedDateChanged();
                }
            }
        }

        [Column(Name = "Location", UpdateCheck = UpdateCheck.Never, Storage = "_Location", DbType = "nvarchar(200)")]
        public string Location
        {
            get => _Location;

            set
            {
                if (_Location != value)
                {
                    OnLocationChanging(value);
                    SendPropertyChanging();
                    _Location = value;
                    SendPropertyChanged("Location");
                    OnLocationChanged();
                }
            }
        }

        [Column(Name = "OrganizationName", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationName", DbType = "nvarchar(100) NOT NULL")]
        public string OrganizationName
        {
            get => _OrganizationName;

            set
            {
                if (_OrganizationName != value)
                {
                    OnOrganizationNameChanging(value);
                    SendPropertyChanging();
                    _OrganizationName = value;
                    SendPropertyChanged("OrganizationName");
                    OnOrganizationNameChanged();
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

        [Column(Name = "ParentOrgId", UpdateCheck = UpdateCheck.Never, Storage = "_ParentOrgId", DbType = "int")]
        [IsForeignKey]
        public int? ParentOrgId
        {
            get => _ParentOrgId;

            set
            {
                if (_ParentOrgId != value)
                {
                    if (_ParentOrg.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnParentOrgIdChanging(value);
                    SendPropertyChanging();
                    _ParentOrgId = value;
                    SendPropertyChanged("ParentOrgId");
                    OnParentOrgIdChanged();
                }
            }
        }

        [Column(Name = "AllowAttendOverlap", UpdateCheck = UpdateCheck.Never, Storage = "_AllowAttendOverlap", DbType = "bit NOT NULL")]
        public bool AllowAttendOverlap
        {
            get => _AllowAttendOverlap;

            set
            {
                if (_AllowAttendOverlap != value)
                {
                    OnAllowAttendOverlapChanging(value);
                    SendPropertyChanging();
                    _AllowAttendOverlap = value;
                    SendPropertyChanged("AllowAttendOverlap");
                    OnAllowAttendOverlapChanged();
                }
            }
        }

        [Column(Name = "MemberCount", UpdateCheck = UpdateCheck.Never, Storage = "_MemberCount", DbType = "int")]
        public int? MemberCount
        {
            get => _MemberCount;

            set
            {
                if (_MemberCount != value)
                {
                    OnMemberCountChanging(value);
                    SendPropertyChanging();
                    _MemberCount = value;
                    SendPropertyChanged("MemberCount");
                    OnMemberCountChanged();
                }
            }
        }

        [Column(Name = "LeaderId", UpdateCheck = UpdateCheck.Never, Storage = "_LeaderId", DbType = "int")]
        public int? LeaderId
        {
            get => _LeaderId;

            set
            {
                if (_LeaderId != value)
                {
                    OnLeaderIdChanging(value);
                    SendPropertyChanging();
                    _LeaderId = value;
                    SendPropertyChanged("LeaderId");
                    OnLeaderIdChanged();
                }
            }
        }

        [Column(Name = "LeaderName", UpdateCheck = UpdateCheck.Never, Storage = "_LeaderName", DbType = "nvarchar(50)")]
        public string LeaderName
        {
            get => _LeaderName;

            set
            {
                if (_LeaderName != value)
                {
                    OnLeaderNameChanging(value);
                    SendPropertyChanging();
                    _LeaderName = value;
                    SendPropertyChanged("LeaderName");
                    OnLeaderNameChanged();
                }
            }
        }

        [Column(Name = "ClassFilled", UpdateCheck = UpdateCheck.Never, Storage = "_ClassFilled", DbType = "bit")]
        public bool? ClassFilled
        {
            get => _ClassFilled;

            set
            {
                if (_ClassFilled != value)
                {
                    OnClassFilledChanging(value);
                    SendPropertyChanging();
                    _ClassFilled = value;
                    SendPropertyChanged("ClassFilled");
                    OnClassFilledChanged();
                }
            }
        }

        [Column(Name = "OnLineCatalogSort", UpdateCheck = UpdateCheck.Never, Storage = "_OnLineCatalogSort", DbType = "int")]
        public int? OnLineCatalogSort
        {
            get => _OnLineCatalogSort;

            set
            {
                if (_OnLineCatalogSort != value)
                {
                    OnOnLineCatalogSortChanging(value);
                    SendPropertyChanging();
                    _OnLineCatalogSort = value;
                    SendPropertyChanged("OnLineCatalogSort");
                    OnOnLineCatalogSortChanged();
                }
            }
        }

        [Column(Name = "PendingLoc", UpdateCheck = UpdateCheck.Never, Storage = "_PendingLoc", DbType = "nvarchar(40)")]
        public string PendingLoc
        {
            get => _PendingLoc;

            set
            {
                if (_PendingLoc != value)
                {
                    OnPendingLocChanging(value);
                    SendPropertyChanging();
                    _PendingLoc = value;
                    SendPropertyChanged("PendingLoc");
                    OnPendingLocChanged();
                }
            }
        }

        [Column(Name = "CanSelfCheckin", UpdateCheck = UpdateCheck.Never, Storage = "_CanSelfCheckin", DbType = "bit")]
        public bool? CanSelfCheckin
        {
            get => _CanSelfCheckin;

            set
            {
                if (_CanSelfCheckin != value)
                {
                    OnCanSelfCheckinChanging(value);
                    SendPropertyChanging();
                    _CanSelfCheckin = value;
                    SendPropertyChanged("CanSelfCheckin");
                    OnCanSelfCheckinChanged();
                }
            }
        }

        [Column(Name = "NumCheckInLabels", UpdateCheck = UpdateCheck.Never, Storage = "_NumCheckInLabels", DbType = "int")]
        public int? NumCheckInLabels
        {
            get => _NumCheckInLabels;

            set
            {
                if (_NumCheckInLabels != value)
                {
                    OnNumCheckInLabelsChanging(value);
                    SendPropertyChanging();
                    _NumCheckInLabels = value;
                    SendPropertyChanged("NumCheckInLabels");
                    OnNumCheckInLabelsChanged();
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

        [Column(Name = "AllowNonCampusCheckIn", UpdateCheck = UpdateCheck.Never, Storage = "_AllowNonCampusCheckIn", DbType = "bit")]
        public bool? AllowNonCampusCheckIn
        {
            get => _AllowNonCampusCheckIn;

            set
            {
                if (_AllowNonCampusCheckIn != value)
                {
                    OnAllowNonCampusCheckInChanging(value);
                    SendPropertyChanging();
                    _AllowNonCampusCheckIn = value;
                    SendPropertyChanged("AllowNonCampusCheckIn");
                    OnAllowNonCampusCheckInChanged();
                }
            }
        }

        [Column(Name = "NumWorkerCheckInLabels", UpdateCheck = UpdateCheck.Never, Storage = "_NumWorkerCheckInLabels", DbType = "int")]
        public int? NumWorkerCheckInLabels
        {
            get => _NumWorkerCheckInLabels;

            set
            {
                if (_NumWorkerCheckInLabels != value)
                {
                    OnNumWorkerCheckInLabelsChanging(value);
                    SendPropertyChanging();
                    _NumWorkerCheckInLabels = value;
                    SendPropertyChanged("NumWorkerCheckInLabels");
                    OnNumWorkerCheckInLabelsChanged();
                }
            }
        }

        [Column(Name = "ShowOnlyRegisteredAtCheckIn", UpdateCheck = UpdateCheck.Never, Storage = "_ShowOnlyRegisteredAtCheckIn", DbType = "bit")]
        public bool? ShowOnlyRegisteredAtCheckIn
        {
            get => _ShowOnlyRegisteredAtCheckIn;

            set
            {
                if (_ShowOnlyRegisteredAtCheckIn != value)
                {
                    OnShowOnlyRegisteredAtCheckInChanging(value);
                    SendPropertyChanging();
                    _ShowOnlyRegisteredAtCheckIn = value;
                    SendPropertyChanged("ShowOnlyRegisteredAtCheckIn");
                    OnShowOnlyRegisteredAtCheckInChanged();
                }
            }
        }

        [Column(Name = "Limit", UpdateCheck = UpdateCheck.Never, Storage = "_Limit", DbType = "int")]
        public int? Limit
        {
            get => _Limit;

            set
            {
                if (_Limit != value)
                {
                    OnLimitChanging(value);
                    SendPropertyChanging();
                    _Limit = value;
                    SendPropertyChanged("Limit");
                    OnLimitChanged();
                }
            }
        }

        [Column(Name = "GenderId", UpdateCheck = UpdateCheck.Never, Storage = "_GenderId", DbType = "int")]
        [IsForeignKey]
        public int? GenderId
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

        [Column(Name = "Description", UpdateCheck = UpdateCheck.Never, Storage = "_Description", DbType = "nvarchar")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    OnDescriptionChanging(value);
                    SendPropertyChanging();
                    _Description = value;
                    SendPropertyChanged("Description");
                    OnDescriptionChanged();
                }
            }
        }

        [Column(Name = "BirthDayStart", UpdateCheck = UpdateCheck.Never, Storage = "_BirthDayStart", DbType = "datetime")]
        public DateTime? BirthDayStart
        {
            get => _BirthDayStart;

            set
            {
                if (_BirthDayStart != value)
                {
                    OnBirthDayStartChanging(value);
                    SendPropertyChanging();
                    _BirthDayStart = value;
                    SendPropertyChanged("BirthDayStart");
                    OnBirthDayStartChanged();
                }
            }
        }

        [Column(Name = "BirthDayEnd", UpdateCheck = UpdateCheck.Never, Storage = "_BirthDayEnd", DbType = "datetime")]
        public DateTime? BirthDayEnd
        {
            get => _BirthDayEnd;

            set
            {
                if (_BirthDayEnd != value)
                {
                    OnBirthDayEndChanging(value);
                    SendPropertyChanging();
                    _BirthDayEnd = value;
                    SendPropertyChanged("BirthDayEnd");
                    OnBirthDayEndChanged();
                }
            }
        }

        [Column(Name = "LastDayBeforeExtra", UpdateCheck = UpdateCheck.Never, Storage = "_LastDayBeforeExtra", DbType = "datetime")]
        public DateTime? LastDayBeforeExtra
        {
            get => _LastDayBeforeExtra;

            set
            {
                if (_LastDayBeforeExtra != value)
                {
                    OnLastDayBeforeExtraChanging(value);
                    SendPropertyChanging();
                    _LastDayBeforeExtra = value;
                    SendPropertyChanged("LastDayBeforeExtra");
                    OnLastDayBeforeExtraChanged();
                }
            }
        }

        [Column(Name = "RegistrationTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_RegistrationTypeId", DbType = "int")]
        public int? RegistrationTypeId
        {
            get => _RegistrationTypeId;

            set
            {
                if (_RegistrationTypeId != value)
                {
                    OnRegistrationTypeIdChanging(value);
                    SendPropertyChanging();
                    _RegistrationTypeId = value;
                    SendPropertyChanged("RegistrationTypeId");
                    OnRegistrationTypeIdChanged();
                }
            }
        }

        [Column(Name = "ValidateOrgs", UpdateCheck = UpdateCheck.Never, Storage = "_ValidateOrgs", DbType = "nvarchar(60)")]
        public string ValidateOrgs
        {
            get => _ValidateOrgs;

            set
            {
                if (_ValidateOrgs != value)
                {
                    OnValidateOrgsChanging(value);
                    SendPropertyChanging();
                    _ValidateOrgs = value;
                    SendPropertyChanged("ValidateOrgs");
                    OnValidateOrgsChanged();
                }
            }
        }

        [Column(Name = "PhoneNumber", UpdateCheck = UpdateCheck.Never, Storage = "_PhoneNumber", DbType = "nvarchar(25)")]
        public string PhoneNumber
        {
            get => _PhoneNumber;

            set
            {
                if (_PhoneNumber != value)
                {
                    OnPhoneNumberChanging(value);
                    SendPropertyChanging();
                    _PhoneNumber = value;
                    SendPropertyChanged("PhoneNumber");
                    OnPhoneNumberChanged();
                }
            }
        }

        [Column(Name = "RegistrationClosed", UpdateCheck = UpdateCheck.Never, Storage = "_RegistrationClosed", DbType = "bit")]
        public bool? RegistrationClosed
        {
            get => _RegistrationClosed;

            set
            {
                if (_RegistrationClosed != value)
                {
                    OnRegistrationClosedChanging(value);
                    SendPropertyChanging();
                    _RegistrationClosed = value;
                    SendPropertyChanged("RegistrationClosed");
                    OnRegistrationClosedChanged();
                }
            }
        }

        [Column(Name = "AllowKioskRegister", UpdateCheck = UpdateCheck.Never, Storage = "_AllowKioskRegister", DbType = "bit")]
        public bool? AllowKioskRegister
        {
            get => _AllowKioskRegister;

            set
            {
                if (_AllowKioskRegister != value)
                {
                    OnAllowKioskRegisterChanging(value);
                    SendPropertyChanging();
                    _AllowKioskRegister = value;
                    SendPropertyChanged("AllowKioskRegister");
                    OnAllowKioskRegisterChanged();
                }
            }
        }

        [Column(Name = "WorshipGroupCodes", UpdateCheck = UpdateCheck.Never, Storage = "_WorshipGroupCodes", DbType = "nvarchar(100)")]
        public string WorshipGroupCodes
        {
            get => _WorshipGroupCodes;

            set
            {
                if (_WorshipGroupCodes != value)
                {
                    OnWorshipGroupCodesChanging(value);
                    SendPropertyChanging();
                    _WorshipGroupCodes = value;
                    SendPropertyChanged("WorshipGroupCodes");
                    OnWorshipGroupCodesChanged();
                }
            }
        }

        [Column(Name = "IsBibleFellowshipOrg", UpdateCheck = UpdateCheck.Never, Storage = "_IsBibleFellowshipOrg", DbType = "bit")]
        public bool? IsBibleFellowshipOrg
        {
            get => _IsBibleFellowshipOrg;

            set
            {
                if (_IsBibleFellowshipOrg != value)
                {
                    OnIsBibleFellowshipOrgChanging(value);
                    SendPropertyChanging();
                    _IsBibleFellowshipOrg = value;
                    SendPropertyChanged("IsBibleFellowshipOrg");
                    OnIsBibleFellowshipOrgChanged();
                }
            }
        }

        [Column(Name = "NoSecurityLabel", UpdateCheck = UpdateCheck.Never, Storage = "_NoSecurityLabel", DbType = "bit")]
        public bool? NoSecurityLabel
        {
            get => _NoSecurityLabel;

            set
            {
                if (_NoSecurityLabel != value)
                {
                    OnNoSecurityLabelChanging(value);
                    SendPropertyChanging();
                    _NoSecurityLabel = value;
                    SendPropertyChanged("NoSecurityLabel");
                    OnNoSecurityLabelChanged();
                }
            }
        }

        [Column(Name = "AlwaysSecurityLabel", UpdateCheck = UpdateCheck.Never, Storage = "_AlwaysSecurityLabel", DbType = "bit")]
        public bool? AlwaysSecurityLabel
        {
            get => _AlwaysSecurityLabel;

            set
            {
                if (_AlwaysSecurityLabel != value)
                {
                    OnAlwaysSecurityLabelChanging(value);
                    SendPropertyChanging();
                    _AlwaysSecurityLabel = value;
                    SendPropertyChanged("AlwaysSecurityLabel");
                    OnAlwaysSecurityLabelChanged();
                }
            }
        }

        [Column(Name = "DaysToIgnoreHistory", UpdateCheck = UpdateCheck.Never, Storage = "_DaysToIgnoreHistory", DbType = "int")]
        public int? DaysToIgnoreHistory
        {
            get => _DaysToIgnoreHistory;

            set
            {
                if (_DaysToIgnoreHistory != value)
                {
                    OnDaysToIgnoreHistoryChanging(value);
                    SendPropertyChanging();
                    _DaysToIgnoreHistory = value;
                    SendPropertyChanged("DaysToIgnoreHistory");
                    OnDaysToIgnoreHistoryChanged();
                }
            }
        }

        [Column(Name = "NotifyIds", UpdateCheck = UpdateCheck.Never, Storage = "_NotifyIds", DbType = "varchar(50)")]
        public string NotifyIds
        {
            get => _NotifyIds;

            set
            {
                if (_NotifyIds != value)
                {
                    OnNotifyIdsChanging(value);
                    SendPropertyChanging();
                    _NotifyIds = value;
                    SendPropertyChanged("NotifyIds");
                    OnNotifyIdsChanged();
                }
            }
        }

        [Column(Name = "lat", UpdateCheck = UpdateCheck.Never, Storage = "_Lat", DbType = "float")]
        public double? Lat
        {
            get => _Lat;

            set
            {
                if (_Lat != value)
                {
                    OnLatChanging(value);
                    SendPropertyChanging();
                    _Lat = value;
                    SendPropertyChanged("Lat");
                    OnLatChanged();
                }
            }
        }

        [Column(Name = "long", UpdateCheck = UpdateCheck.Never, Storage = "_LongX", DbType = "float")]
        public double? LongX
        {
            get => _LongX;

            set
            {
                if (_LongX != value)
                {
                    OnLongXChanging(value);
                    SendPropertyChanging();
                    _LongX = value;
                    SendPropertyChanged("LongX");
                    OnLongXChanged();
                }
            }
        }

        [Column(Name = "RegSetting", UpdateCheck = UpdateCheck.Never, Storage = "_RegSetting", DbType = "nvarchar")]
        public string RegSetting
        {
            get => _RegSetting;

            set
            {
                if (_RegSetting != value)
                {
                    OnRegSettingChanging(value);
                    SendPropertyChanging();
                    _RegSetting = value;
                    SendPropertyChanged("RegSetting");
                    OnRegSettingChanged();
                }
            }
        }

        [Column(Name = "OrgPickList", UpdateCheck = UpdateCheck.Never, Storage = "_OrgPickList", DbType = "varchar")]
        public string OrgPickList
        {
            get => _OrgPickList;

            set
            {
                if (_OrgPickList != value)
                {
                    OnOrgPickListChanging(value);
                    SendPropertyChanging();
                    _OrgPickList = value;
                    SendPropertyChanged("OrgPickList");
                    OnOrgPickListChanged();
                }
            }
        }

        [Column(Name = "Offsite", UpdateCheck = UpdateCheck.Never, Storage = "_Offsite", DbType = "bit")]
        public bool? Offsite
        {
            get => _Offsite;

            set
            {
                if (_Offsite != value)
                {
                    OnOffsiteChanging(value);
                    SendPropertyChanging();
                    _Offsite = value;
                    SendPropertyChanged("Offsite");
                    OnOffsiteChanged();
                }
            }
        }

        [Column(Name = "RegStart", UpdateCheck = UpdateCheck.Never, Storage = "_RegStart", DbType = "datetime")]
        public DateTime? RegStart
        {
            get => _RegStart;

            set
            {
                if (_RegStart != value)
                {
                    OnRegStartChanging(value);
                    SendPropertyChanging();
                    _RegStart = value;
                    SendPropertyChanged("RegStart");
                    OnRegStartChanged();
                }
            }
        }

        [Column(Name = "RegEnd", UpdateCheck = UpdateCheck.Never, Storage = "_RegEnd", DbType = "datetime")]
        public DateTime? RegEnd
        {
            get => _RegEnd;

            set
            {
                if (_RegEnd != value)
                {
                    OnRegEndChanging(value);
                    SendPropertyChanging();
                    _RegEnd = value;
                    SendPropertyChanged("RegEnd");
                    OnRegEndChanged();
                }
            }
        }

        [Column(Name = "LimitToRole", UpdateCheck = UpdateCheck.Never, Storage = "_LimitToRole", DbType = "nvarchar(20)")]
        public string LimitToRole
        {
            get => _LimitToRole;

            set
            {
                if (_LimitToRole != value)
                {
                    OnLimitToRoleChanging(value);
                    SendPropertyChanging();
                    _LimitToRole = value;
                    SendPropertyChanged("LimitToRole");
                    OnLimitToRoleChanged();
                }
            }
        }

        [Column(Name = "OrganizationTypeId", UpdateCheck = UpdateCheck.Never, Storage = "_OrganizationTypeId", DbType = "int")]
        [IsForeignKey]
        public int? OrganizationTypeId
        {
            get => _OrganizationTypeId;

            set
            {
                if (_OrganizationTypeId != value)
                {
                    if (_OrganizationType.HasLoadedOrAssignedValue)
                    {
                        throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
                    }

                    OnOrganizationTypeIdChanging(value);
                    SendPropertyChanging();
                    _OrganizationTypeId = value;
                    SendPropertyChanged("OrganizationTypeId");
                    OnOrganizationTypeIdChanged();
                }
            }
        }

        [Column(Name = "MemberJoinScript", UpdateCheck = UpdateCheck.Never, Storage = "_MemberJoinScript", DbType = "nvarchar(50)")]
        public string MemberJoinScript
        {
            get => _MemberJoinScript;

            set
            {
                if (_MemberJoinScript != value)
                {
                    OnMemberJoinScriptChanging(value);
                    SendPropertyChanging();
                    _MemberJoinScript = value;
                    SendPropertyChanged("MemberJoinScript");
                    OnMemberJoinScriptChanged();
                }
            }
        }

        [Column(Name = "AddToSmallGroupScript", UpdateCheck = UpdateCheck.Never, Storage = "_AddToSmallGroupScript", DbType = "nvarchar(50)")]
        public string AddToSmallGroupScript
        {
            get => _AddToSmallGroupScript;

            set
            {
                if (_AddToSmallGroupScript != value)
                {
                    OnAddToSmallGroupScriptChanging(value);
                    SendPropertyChanging();
                    _AddToSmallGroupScript = value;
                    SendPropertyChanged("AddToSmallGroupScript");
                    OnAddToSmallGroupScriptChanged();
                }
            }
        }

        [Column(Name = "RemoveFromSmallGroupScript", UpdateCheck = UpdateCheck.Never, Storage = "_RemoveFromSmallGroupScript", DbType = "nvarchar(50)")]
        public string RemoveFromSmallGroupScript
        {
            get => _RemoveFromSmallGroupScript;

            set
            {
                if (_RemoveFromSmallGroupScript != value)
                {
                    OnRemoveFromSmallGroupScriptChanging(value);
                    SendPropertyChanging();
                    _RemoveFromSmallGroupScript = value;
                    SendPropertyChanged("RemoveFromSmallGroupScript");
                    OnRemoveFromSmallGroupScriptChanged();
                }
            }
        }

        [Column(Name = "SuspendCheckin", UpdateCheck = UpdateCheck.Never, Storage = "_SuspendCheckin", DbType = "bit")]
        public bool? SuspendCheckin
        {
            get => _SuspendCheckin;

            set
            {
                if (_SuspendCheckin != value)
                {
                    OnSuspendCheckinChanging(value);
                    SendPropertyChanging();
                    _SuspendCheckin = value;
                    SendPropertyChanged("SuspendCheckin");
                    OnSuspendCheckinChanged();
                }
            }
        }

        [Column(Name = "NoAutoAbsents", UpdateCheck = UpdateCheck.Never, Storage = "_NoAutoAbsents", DbType = "bit")]
        public bool? NoAutoAbsents
        {
            get => _NoAutoAbsents;

            set
            {
                if (_NoAutoAbsents != value)
                {
                    OnNoAutoAbsentsChanging(value);
                    SendPropertyChanging();
                    _NoAutoAbsents = value;
                    SendPropertyChanged("NoAutoAbsents");
                    OnNoAutoAbsentsChanged();
                }
            }
        }

        [Column(Name = "PublishDirectory", UpdateCheck = UpdateCheck.Never, Storage = "_PublishDirectory", DbType = "int")]
        public int? PublishDirectory
        {
            get => _PublishDirectory;

            set
            {
                if (_PublishDirectory != value)
                {
                    OnPublishDirectoryChanging(value);
                    SendPropertyChanging();
                    _PublishDirectory = value;
                    SendPropertyChanged("PublishDirectory");
                    OnPublishDirectoryChanged();
                }
            }
        }

        [Column(Name = "ConsecutiveAbsentsThreshold", UpdateCheck = UpdateCheck.Never, Storage = "_ConsecutiveAbsentsThreshold", DbType = "int")]
        public int? ConsecutiveAbsentsThreshold
        {
            get => _ConsecutiveAbsentsThreshold;

            set
            {
                if (_ConsecutiveAbsentsThreshold != value)
                {
                    OnConsecutiveAbsentsThresholdChanging(value);
                    SendPropertyChanging();
                    _ConsecutiveAbsentsThreshold = value;
                    SendPropertyChanged("ConsecutiveAbsentsThreshold");
                    OnConsecutiveAbsentsThresholdChanged();
                }
            }
        }

        [Column(Name = "IsRecreationTeam", UpdateCheck = UpdateCheck.Never, Storage = "_IsRecreationTeam", DbType = "bit NOT NULL")]
        public bool IsRecreationTeam
        {
            get => _IsRecreationTeam;

            set
            {
                if (_IsRecreationTeam != value)
                {
                    OnIsRecreationTeamChanging(value);
                    SendPropertyChanging();
                    _IsRecreationTeam = value;
                    SendPropertyChanged("IsRecreationTeam");
                    OnIsRecreationTeamChanged();
                }
            }
        }

        [Column(Name = "NotWeekly", UpdateCheck = UpdateCheck.Never, Storage = "_NotWeekly", DbType = "bit")]
        public bool? NotWeekly
        {
            get => _NotWeekly;

            set
            {
                if (_NotWeekly != value)
                {
                    OnNotWeeklyChanging(value);
                    SendPropertyChanging();
                    _NotWeekly = value;
                    SendPropertyChanged("NotWeekly");
                    OnNotWeeklyChanged();
                }
            }
        }

        [Column(Name = "IsMissionTrip", UpdateCheck = UpdateCheck.Never, Storage = "_IsMissionTrip", DbType = "bit")]
        public bool? IsMissionTrip
        {
            get => _IsMissionTrip;

            set
            {
                if (_IsMissionTrip != value)
                {
                    OnIsMissionTripChanging(value);
                    SendPropertyChanging();
                    _IsMissionTrip = value;
                    SendPropertyChanged("IsMissionTrip");
                    OnIsMissionTripChanged();
                }
            }
        }

        [Column(Name = "NoCreditCards", UpdateCheck = UpdateCheck.Never, Storage = "_NoCreditCards", DbType = "bit")]
        public bool? NoCreditCards
        {
            get => _NoCreditCards;

            set
            {
                if (_NoCreditCards != value)
                {
                    OnNoCreditCardsChanging(value);
                    SendPropertyChanging();
                    _NoCreditCards = value;
                    SendPropertyChanged("NoCreditCards");
                    OnNoCreditCardsChanged();
                }
            }
        }

        [Column(Name = "GiftNotifyIds", UpdateCheck = UpdateCheck.Never, Storage = "_GiftNotifyIds", DbType = "varchar(50)")]
        public string GiftNotifyIds
        {
            get => _GiftNotifyIds;

            set
            {
                if (_GiftNotifyIds != value)
                {
                    OnGiftNotifyIdsChanging(value);
                    SendPropertyChanging();
                    _GiftNotifyIds = value;
                    SendPropertyChanged("GiftNotifyIds");
                    OnGiftNotifyIdsChanged();
                }
            }
        }

        [Column(Name = "VisitorDate", UpdateCheck = UpdateCheck.Never, Storage = "_VisitorDate", DbType = "datetime", IsDbGenerated = true)]
        public DateTime? VisitorDate
        {
            get => _VisitorDate;

            set
            {
                if (_VisitorDate != value)
                {
                    OnVisitorDateChanging(value);
                    SendPropertyChanging();
                    _VisitorDate = value;
                    SendPropertyChanged("VisitorDate");
                    OnVisitorDateChanged();
                }
            }
        }

        [Column(Name = "UseBootstrap", UpdateCheck = UpdateCheck.Never, Storage = "_UseBootstrap", DbType = "bit")]
        public bool? UseBootstrap
        {
            get => _UseBootstrap;

            set
            {
                if (_UseBootstrap != value)
                {
                    OnUseBootstrapChanging(value);
                    SendPropertyChanging();
                    _UseBootstrap = value;
                    SendPropertyChanged("UseBootstrap");
                    OnUseBootstrapChanged();
                }
            }
        }

        [Column(Name = "PublicSortOrder", UpdateCheck = UpdateCheck.Never, Storage = "_PublicSortOrder", DbType = "varchar(15)")]
        public string PublicSortOrder
        {
            get => _PublicSortOrder;

            set
            {
                if (_PublicSortOrder != value)
                {
                    OnPublicSortOrderChanging(value);
                    SendPropertyChanging();
                    _PublicSortOrder = value;
                    SendPropertyChanged("PublicSortOrder");
                    OnPublicSortOrderChanged();
                }
            }
        }

        [Column(Name = "UseRegisterLink2", UpdateCheck = UpdateCheck.Never, Storage = "_UseRegisterLink2", DbType = "bit")]
        public bool? UseRegisterLink2
        {
            get => _UseRegisterLink2;

            set
            {
                if (_UseRegisterLink2 != value)
                {
                    OnUseRegisterLink2Changing(value);
                    SendPropertyChanging();
                    _UseRegisterLink2 = value;
                    SendPropertyChanged("UseRegisterLink2");
                    OnUseRegisterLink2Changed();
                }
            }
        }

        [Column(Name = "AppCategory", UpdateCheck = UpdateCheck.Never, Storage = "_AppCategory", DbType = "varchar(15)")]
        public string AppCategory
        {
            get => _AppCategory;

            set
            {
                if (_AppCategory != value)
                {
                    OnAppCategoryChanging(value);
                    SendPropertyChanging();
                    _AppCategory = value;
                    SendPropertyChanged("AppCategory");
                    OnAppCategoryChanged();
                }
            }
        }

        [Column(Name = "RegistrationTitle", UpdateCheck = UpdateCheck.Never, Storage = "_RegistrationTitle", DbType = "nvarchar(200)")]
        public string RegistrationTitle
        {
            get => _RegistrationTitle;

            set
            {
                if (_RegistrationTitle != value)
                {
                    OnRegistrationTitleChanging(value);
                    SendPropertyChanging();
                    _RegistrationTitle = value;
                    SendPropertyChanged("RegistrationTitle");
                    OnRegistrationTitleChanged();
                }
            }
        }

        [Column(Name = "PrevMemberCount", UpdateCheck = UpdateCheck.Never, Storage = "_PrevMemberCount", DbType = "int")]
        public int? PrevMemberCount
        {
            get => _PrevMemberCount;

            set
            {
                if (_PrevMemberCount != value)
                {
                    OnPrevMemberCountChanging(value);
                    SendPropertyChanging();
                    _PrevMemberCount = value;
                    SendPropertyChanged("PrevMemberCount");
                    OnPrevMemberCountChanged();
                }
            }
        }

        [Column(Name = "ProspectCount", UpdateCheck = UpdateCheck.Never, Storage = "_ProspectCount", DbType = "int")]
        public int? ProspectCount
        {
            get => _ProspectCount;

            set
            {
                if (_ProspectCount != value)
                {
                    OnProspectCountChanging(value);
                    SendPropertyChanging();
                    _ProspectCount = value;
                    SendPropertyChanged("ProspectCount");
                    OnProspectCountChanged();
                }
            }
        }

        [Column(Name = "RegSettingXml", UpdateCheck = UpdateCheck.Never, Storage = "_RegSettingXml", DbType = "xml")]
        public string RegSettingXml
        {
            get => _RegSettingXml;

            set
            {
                if (_RegSettingXml != value)
                {
                    OnRegSettingXmlChanging(value);
                    SendPropertyChanging();
                    _RegSettingXml = value;
                    SendPropertyChanged("RegSettingXml");
                    OnRegSettingXmlChanged();
                }
            }
        }

        [Column(Name = "AttendanceBySubGroups", UpdateCheck = UpdateCheck.Never, Storage = "_AttendanceBySubGroups", DbType = "bit")]
        public bool? AttendanceBySubGroups
        {
            get => _AttendanceBySubGroups;

            set
            {
                if (_AttendanceBySubGroups != value)
                {
                    OnAttendanceBySubGroupsChanging(value);
                    SendPropertyChanging();
                    _AttendanceBySubGroups = value;
                    SendPropertyChanged("AttendanceBySubGroups");
                    OnAttendanceBySubGroupsChanged();
                }
            }
        }

        [Column(Name = "SendAttendanceLink", UpdateCheck = UpdateCheck.Never, Storage = "_SendAttendanceLink", DbType = "bit NOT NULL")]
        public bool SendAttendanceLink
        {
            get => _SendAttendanceLink;

            set
            {
                if (_SendAttendanceLink != value)
                {
                    OnSendAttendanceLinkChanging(value);
                    SendPropertyChanging();
                    _SendAttendanceLink = value;
                    SendPropertyChanged("SendAttendanceLink");
                    OnSendAttendanceLinkChanged();
                }
            }
        }

        [Column(Name = "TripFundingPagesEnable", UpdateCheck = UpdateCheck.Never, Storage = "_TripFundingPagesEnable", DbType = "bit NOT NULL")]
        public bool TripFundingPagesEnable
        {
            get => _TripFundingPagesEnable;

            set
            {
                if (_TripFundingPagesEnable != value)
                {
                    OnTripFundingPagesEnableChanging(value);
                    SendPropertyChanging();
                    _TripFundingPagesEnable = value;
                    SendPropertyChanged("TripFundingPagesEnable");
                    OnTripFundingPagesEnableChanged();
                }
            }
        }

        [Column(Name = "TripFundingPagesPublic", UpdateCheck = UpdateCheck.Never, Storage = "_TripFundingPagesPublic", DbType = "bit NOT NULL")]
        public bool TripFundingPagesPublic
        {
            get => _TripFundingPagesPublic;

            set
            {
                if (_TripFundingPagesPublic != value)
                {
                    OnTripFundingPagesPublicChanging(value);
                    SendPropertyChanging();
                    _TripFundingPagesPublic = value;
                    SendPropertyChanged("TripFundingPagesPublic");
                    OnTripFundingPagesPublicChanged();
                }
            }
        }

        [Column(Name = "TripFundingPagesShowAmounts", UpdateCheck = UpdateCheck.Never, Storage = "_TripFundingPagesShowAmounts", DbType = "bit NOT NULL")]
        public bool TripFundingPagesShowAmounts
        {
            get => _TripFundingPagesShowAmounts;

            set
            {
                if (_TripFundingPagesShowAmounts != value)
                {
                    OnTripFundingPagesShowAmountsChanging(value);
                    SendPropertyChanging();
                    _TripFundingPagesShowAmounts = value;
                    SendPropertyChanged("TripFundingPagesShowAmounts");
                    OnTripFundingPagesShowAmountsChanged();
                }
            }
        }

        #endregion

        #region Foreign Key Tables

        [Association(Name = "BFMembers__BFClass", Storage = "_BFMembers", OtherKey = "BibleFellowshipClassId")]
        public EntitySet<Person> BFMembers
           {
               get => _BFMembers;

            set => _BFMembers.Assign(value);

           }

        [Association(Name = "ChildOrgs__ParentOrg", Storage = "_ChildOrgs", OtherKey = "ParentOrgId")]
        public EntitySet<Organization> ChildOrgs
           {
               get => _ChildOrgs;

            set => _ChildOrgs.Assign(value);

           }

        [Association(Name = "contactsHad__organization", Storage = "_contactsHad", OtherKey = "OrganizationId")]
        public EntitySet<Contact> contactsHad
           {
               get => _contactsHad;

            set => _contactsHad.Assign(value);

           }

        [Association(Name = "ENROLLMENT_TRANSACTION_ORG_FK", Storage = "_EnrollmentTransactions", OtherKey = "OrganizationId")]
        public EntitySet<EnrollmentTransaction> EnrollmentTransactions
           {
               get => _EnrollmentTransactions;

            set => _EnrollmentTransactions.Assign(value);

           }

        [Association(Name = "FK_AttendWithAbsents_TBL_ORGANIZATIONS_TBL", Storage = "_Attends", OtherKey = "OrganizationId")]
        public EntitySet<Attend> Attends
           {
               get => _Attends;

            set => _Attends.Assign(value);

           }

        [Association(Name = "FK_Coupons_Organizations", Storage = "_Coupons", OtherKey = "OrgId")]
        public EntitySet<Coupon> Coupons
           {
               get => _Coupons;

            set => _Coupons.Assign(value);

           }

        [Association(Name = "FK_DivOrg_Organizations", Storage = "_DivOrgs", OtherKey = "OrgId")]
        public EntitySet<DivOrg> DivOrgs
           {
               get => _DivOrgs;

            set => _DivOrgs.Assign(value);

           }

        [Association(Name = "FK_GoerSenderAmounts_Organizations", Storage = "_GoerSenderAmounts", OtherKey = "OrgId")]
        public EntitySet<GoerSenderAmount> GoerSenderAmounts
           {
               get => _GoerSenderAmounts;

            set => _GoerSenderAmounts.Assign(value);

           }

        [Association(Name = "FK_MEETINGS_TBL_ORGANIZATIONS_TBL", Storage = "_Meetings", OtherKey = "OrganizationId")]
        public EntitySet<Meeting> Meetings
           {
               get => _Meetings;

            set => _Meetings.Assign(value);

           }

        [Association(Name = "FK_MemberTags_Organizations", Storage = "_MemberTags", OtherKey = "OrgId")]
        public EntitySet<MemberTag> MemberTags
           {
               get => _MemberTags;

            set => _MemberTags.Assign(value);

           }

        [Association(Name = "FK_OrganizationExtra_Organizations", Storage = "_OrganizationExtras", OtherKey = "OrganizationId")]
        public EntitySet<OrganizationExtra> OrganizationExtras
           {
               get => _OrganizationExtras;

            set => _OrganizationExtras.Assign(value);

           }

        [Association(Name = "FK_OrgMemberExtra_Organizations", Storage = "_OrgMemberExtras", OtherKey = "OrganizationId")]
        public EntitySet<OrgMemberExtra> OrgMemberExtras
           {
               get => _OrgMemberExtras;

            set => _OrgMemberExtras.Assign(value);

           }

        [Association(Name = "FK_OrgSchedule_Organizations", Storage = "_OrgSchedules", OtherKey = "OrganizationId")]
        public EntitySet<OrgSchedule> OrgSchedules
           {
               get => _OrgSchedules;

            set => _OrgSchedules.Assign(value);

           }

        [Association(Name = "FK_PrevOrgMemberExtra_Organization", Storage = "_PrevOrgMemberExtras", OtherKey = "OrganizationId")]
        public EntitySet<PrevOrgMemberExtra> PrevOrgMemberExtras
           {
               get => _PrevOrgMemberExtras;

            set => _PrevOrgMemberExtras.Assign(value);

           }

        [Association(Name = "FK_Resource_Organization", Storage = "_Resources", OtherKey = "OrganizationId")]
        public EntitySet<Resource> Resources
           {
               get => _Resources;

            set => _Resources.Assign(value);

           }

        [Association(Name = "FK_ResourceOrganization_Organizations", Storage = "_ResourceOrganizations", OtherKey = "OrganizationId")]
        public EntitySet<ResourceOrganization> ResourceOrganizations
           {
               get => _ResourceOrganizations;

            set => _ResourceOrganizations.Assign(value);

           }

        [Association(Name = "ORGANIZATION_MEMBERS_ORG_FK", Storage = "_OrganizationMembers", OtherKey = "OrganizationId")]
        public EntitySet<OrganizationMember> OrganizationMembers
           {
               get => _OrganizationMembers;

            set => _OrganizationMembers.Assign(value);

           }

        [Association(Name = "Org_Member_Documents_ORG_FK", Storage = "_OrgMemberDocuments", OtherKey = "OrganizationId")]
        public EntitySet<OrgMemberDocuments> OrgMemberDocuments
        {
               get => _OrgMemberDocuments;

            set => _OrgMemberDocuments.Assign(value);
           }

        #endregion

        #region Foreign Keys

        [Association(Name = "ChildOrgs__ParentOrg", Storage = "_ParentOrg", ThisKey = "ParentOrgId", IsForeignKey = true)]
        public Organization ParentOrg
        {
            get => _ParentOrg.Entity;

            set
            {
                Organization previousValue = _ParentOrg.Entity;
                if (((previousValue != value)
                            || (_ParentOrg.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _ParentOrg.Entity = null;
                        previousValue.ChildOrgs.Remove(this);
                    }

                    _ParentOrg.Entity = value;
                    if (value != null)
                    {
                        value.ChildOrgs.Add(this);

                        _ParentOrgId = value.OrganizationId;

                    }

                    else
                    {
                        _ParentOrgId = default(int?);

                    }

                    SendPropertyChanged("ParentOrg");
                }
            }
        }

        [Association(Name = "FK_Organizations_Campus", Storage = "_Campu", ThisKey = "CampusId", IsForeignKey = true)]
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
                        previousValue.Organizations.Remove(this);
                    }

                    _Campu.Entity = value;
                    if (value != null)
                    {
                        value.Organizations.Add(this);

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

        [Association(Name = "FK_Organizations_Division", Storage = "_Division", ThisKey = "DivisionId", IsForeignKey = true)]
        public Division Division
        {
            get => _Division.Entity;

            set
            {
                Division previousValue = _Division.Entity;
                if (((previousValue != value)
                            || (_Division.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _Division.Entity = null;
                        previousValue.Organizations.Remove(this);
                    }

                    _Division.Entity = value;
                    if (value != null)
                    {
                        value.Organizations.Add(this);

                        _DivisionId = value.Id;

                    }

                    else
                    {
                        _DivisionId = default(int?);

                    }

                    SendPropertyChanged("Division");
                }
            }
        }

        [Association(Name = "FK_Organizations_Gender", Storage = "_Gender", ThisKey = "GenderId", IsForeignKey = true)]
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
                        previousValue.Organizations.Remove(this);
                    }

                    _Gender.Entity = value;
                    if (value != null)
                    {
                        value.Organizations.Add(this);

                        _GenderId = value.Id;

                    }

                    else
                    {
                        _GenderId = default(int?);

                    }

                    SendPropertyChanged("Gender");
                }
            }
        }

        [Association(Name = "FK_Organizations_OrganizationType", Storage = "_OrganizationType", ThisKey = "OrganizationTypeId", IsForeignKey = true)]
        public OrganizationType OrganizationType
        {
            get => _OrganizationType.Entity;

            set
            {
                OrganizationType previousValue = _OrganizationType.Entity;
                if (((previousValue != value)
                            || (_OrganizationType.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _OrganizationType.Entity = null;
                        previousValue.Organizations.Remove(this);
                    }

                    _OrganizationType.Entity = value;
                    if (value != null)
                    {
                        value.Organizations.Add(this);

                        _OrganizationTypeId = value.Id;

                    }

                    else
                    {
                        _OrganizationTypeId = default(int?);

                    }

                    SendPropertyChanged("OrganizationType");
                }
            }
        }

        [Association(Name = "FK_ORGANIZATIONS_TBL_EntryPoint", Storage = "_EntryPoint", ThisKey = "EntryPointId", IsForeignKey = true)]
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
                        previousValue.Organizations.Remove(this);
                    }

                    _EntryPoint.Entity = value;
                    if (value != null)
                    {
                        value.Organizations.Add(this);

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

        [Association(Name = "FK_ORGANIZATIONS_TBL_OrganizationStatus", Storage = "_OrganizationStatus", ThisKey = "OrganizationStatusId", IsForeignKey = true)]
        public OrganizationStatus OrganizationStatus
        {
            get => _OrganizationStatus.Entity;

            set
            {
                OrganizationStatus previousValue = _OrganizationStatus.Entity;
                if (((previousValue != value)
                            || (_OrganizationStatus.HasLoadedOrAssignedValue == false)))
                {
                    SendPropertyChanging();
                    if (previousValue != null)
                    {
                        _OrganizationStatus.Entity = null;
                        previousValue.Organizations.Remove(this);
                    }

                    _OrganizationStatus.Entity = value;
                    if (value != null)
                    {
                        value.Organizations.Add(this);

                        _OrganizationStatusId = value.Id;

                    }

                    else
                    {
                        _OrganizationStatusId = default(int);

                    }

                    SendPropertyChanged("OrganizationStatus");
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

        private void attach_BFMembers(Person entity)
        {
            SendPropertyChanging();
            entity.BFClass = this;
        }

        private void detach_BFMembers(Person entity)
        {
            SendPropertyChanging();
            entity.BFClass = null;
        }

        private void attach_ChildOrgs(Organization entity)
        {
            SendPropertyChanging();
            entity.ParentOrg = this;
        }

        private void detach_ChildOrgs(Organization entity)
        {
            SendPropertyChanging();
            entity.ParentOrg = null;
        }

        private void attach_contactsHad(Contact entity)
        {
            SendPropertyChanging();
            entity.organization = this;
        }

        private void detach_contactsHad(Contact entity)
        {
            SendPropertyChanging();
            entity.organization = null;
        }

        private void attach_EnrollmentTransactions(EnrollmentTransaction entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_EnrollmentTransactions(EnrollmentTransaction entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_Attends(Attend entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_Attends(Attend entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_Coupons(Coupon entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_Coupons(Coupon entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_DivOrgs(DivOrg entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_DivOrgs(DivOrg entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_GoerSenderAmounts(GoerSenderAmount entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_GoerSenderAmounts(GoerSenderAmount entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_Meetings(Meeting entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_Meetings(Meeting entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_MemberTags(MemberTag entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_MemberTags(MemberTag entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_OrganizationExtras(OrganizationExtra entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_OrganizationExtras(OrganizationExtra entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_OrgMemberExtras(OrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_OrgMemberExtras(OrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_OrgSchedules(OrgSchedule entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_OrgSchedules(OrgSchedule entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_PrevOrgMemberExtras(PrevOrgMemberExtra entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_Resources(Resource entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_Resources(Resource entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_ResourceOrganizations(ResourceOrganization entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_ResourceOrganizations(ResourceOrganization entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_OrganizationMembers(OrganizationMember entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_OrganizationMembers(OrganizationMember entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }

        private void attach_OrgMemberDocuments(OrgMemberDocuments entity)
        {
            SendPropertyChanging();
            entity.Organization = this;
        }

        private void detach_OrgMemberDocuments(OrgMemberDocuments entity)
        {
            SendPropertyChanging();
            entity.Organization = null;
        }
    }
}
