using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Data.SqlClient;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;

namespace CmsData
{
    [DatabaseAttribute(Name="CMS")]
    public partial class CMSDataContext : DataContext
    {
        private readonly static MappingSource _mappingSource = new AttributeMappingSource();
#region Extensibility Method Definitions
        partial void OnCreated();
        
        partial void InsertActivityLog(ActivityLog instance);
        partial void UpdateActivityLog(ActivityLog instance);
        partial void DeleteActivityLog(ActivityLog instance);
        
        partial void InsertAddress(Address instance);
        partial void UpdateAddress(Address instance);
        partial void DeleteAddress(Address instance);
        
        partial void InsertAddressType(AddressType instance);
        partial void UpdateAddressType(AddressType instance);
        partial void DeleteAddressType(AddressType instance);
        
        partial void InsertAddToOrgFromTagRun(AddToOrgFromTagRun instance);
        partial void UpdateAddToOrgFromTagRun(AddToOrgFromTagRun instance);
        partial void DeleteAddToOrgFromTagRun(AddToOrgFromTagRun instance);
        
        partial void InsertApiSession(ApiSession instance);
        partial void UpdateApiSession(ApiSession instance);
        partial void DeleteApiSession(ApiSession instance);
        
        partial void InsertAttend(Attend instance);
        partial void UpdateAttend(Attend instance);
        partial void DeleteAttend(Attend instance);
        
        partial void InsertAttendCredit(AttendCredit instance);
        partial void UpdateAttendCredit(AttendCredit instance);
        partial void DeleteAttendCredit(AttendCredit instance);
        
        partial void InsertAttendType(AttendType instance);
        partial void UpdateAttendType(AttendType instance);
        partial void DeleteAttendType(AttendType instance);
        
        partial void InsertAudit(Audit instance);
        partial void UpdateAudit(Audit instance);
        partial void DeleteAudit(Audit instance);
        
        partial void InsertAuditValue(AuditValue instance);
        partial void UpdateAuditValue(AuditValue instance);
        partial void DeleteAuditValue(AuditValue instance);
        
        partial void InsertBackgroundCheckLabel(BackgroundCheckLabel instance);
        partial void UpdateBackgroundCheckLabel(BackgroundCheckLabel instance);
        partial void DeleteBackgroundCheckLabel(BackgroundCheckLabel instance);
        
        partial void InsertBackgroundCheckMVRCode(BackgroundCheckMVRCode instance);
        partial void UpdateBackgroundCheckMVRCode(BackgroundCheckMVRCode instance);
        partial void DeleteBackgroundCheckMVRCode(BackgroundCheckMVRCode instance);
        
        partial void InsertBackgroundCheck(BackgroundCheck instance);
        partial void UpdateBackgroundCheck(BackgroundCheck instance);
        partial void DeleteBackgroundCheck(BackgroundCheck instance);
        
        partial void InsertBaptismStatus(BaptismStatus instance);
        partial void UpdateBaptismStatus(BaptismStatus instance);
        partial void DeleteBaptismStatus(BaptismStatus instance);
        
        partial void InsertBaptismType(BaptismType instance);
        partial void UpdateBaptismType(BaptismType instance);
        partial void DeleteBaptismType(BaptismType instance);
        
        partial void InsertBuildingAccessType(BuildingAccessType instance);
        partial void UpdateBuildingAccessType(BuildingAccessType instance);
        partial void DeleteBuildingAccessType(BuildingAccessType instance);
        
        partial void InsertBundleDetail(BundleDetail instance);
        partial void UpdateBundleDetail(BundleDetail instance);
        partial void DeleteBundleDetail(BundleDetail instance);
        
        partial void InsertBundleHeader(BundleHeader instance);
        partial void UpdateBundleHeader(BundleHeader instance);
        partial void DeleteBundleHeader(BundleHeader instance);
        
        partial void InsertBundleHeaderType(BundleHeaderType instance);
        partial void UpdateBundleHeaderType(BundleHeaderType instance);
        partial void DeleteBundleHeaderType(BundleHeaderType instance);
        
        partial void InsertBundleStatusType(BundleStatusType instance);
        partial void UpdateBundleStatusType(BundleStatusType instance);
        partial void DeleteBundleStatusType(BundleStatusType instance);
        
        partial void InsertCampu(Campu instance);
        partial void UpdateCampu(Campu instance);
        partial void DeleteCampu(Campu instance);
        
        partial void InsertCardIdentifier(CardIdentifier instance);
        partial void UpdateCardIdentifier(CardIdentifier instance);
        partial void DeleteCardIdentifier(CardIdentifier instance);
        
        partial void InsertChangeDetail(ChangeDetail instance);
        partial void UpdateChangeDetail(ChangeDetail instance);
        partial void DeleteChangeDetail(ChangeDetail instance);
        
        partial void InsertChangeLog(ChangeLog instance);
        partial void UpdateChangeLog(ChangeLog instance);
        partial void DeleteChangeLog(ChangeLog instance);
        
        partial void InsertCheckedBatch(CheckedBatch instance);
        partial void UpdateCheckedBatch(CheckedBatch instance);
        partial void DeleteCheckedBatch(CheckedBatch instance);
        
        partial void InsertCheckInActivity(CheckInActivity instance);
        partial void UpdateCheckInActivity(CheckInActivity instance);
        partial void DeleteCheckInActivity(CheckInActivity instance);
        
        partial void InsertCheckInSetting(CheckInSetting instance);
        partial void UpdateCheckInSetting(CheckInSetting instance);
        partial void DeleteCheckInSetting(CheckInSetting instance);
        
        partial void InsertCheckInTime(CheckInTime instance);
        partial void UpdateCheckInTime(CheckInTime instance);
        partial void DeleteCheckInTime(CheckInTime instance);
        
        partial void InsertChurchAttReportId(ChurchAttReportId instance);
        partial void UpdateChurchAttReportId(ChurchAttReportId instance);
        partial void DeleteChurchAttReportId(ChurchAttReportId instance);
        
        partial void InsertContact(Contact instance);
        partial void UpdateContact(Contact instance);
        partial void DeleteContact(Contact instance);
        
        partial void InsertContactee(Contactee instance);
        partial void UpdateContactee(Contactee instance);
        partial void DeleteContactee(Contactee instance);
        
        partial void InsertContactor(Contactor instance);
        partial void UpdateContactor(Contactor instance);
        partial void DeleteContactor(Contactor instance);
        
        partial void InsertContactPreference(ContactPreference instance);
        partial void UpdateContactPreference(ContactPreference instance);
        partial void DeleteContactPreference(ContactPreference instance);
        
        partial void InsertContactReason(ContactReason instance);
        partial void UpdateContactReason(ContactReason instance);
        partial void DeleteContactReason(ContactReason instance);
        
        partial void InsertContactType(ContactType instance);
        partial void UpdateContactType(ContactType instance);
        partial void DeleteContactType(ContactType instance);
        
        partial void InsertContent(Content instance);
        partial void UpdateContent(Content instance);
        partial void DeleteContent(Content instance);
        
        partial void InsertContentKeyWord(ContentKeyWord instance);
        partial void UpdateContentKeyWord(ContentKeyWord instance);
        partial void DeleteContentKeyWord(ContentKeyWord instance);
        
        partial void InsertContribution(Contribution instance);
        partial void UpdateContribution(Contribution instance);
        partial void DeleteContribution(Contribution instance);
        
        partial void InsertContributionFund(ContributionFund instance);
        partial void UpdateContributionFund(ContributionFund instance);
        partial void DeleteContributionFund(ContributionFund instance);
        
        partial void InsertContributionsRun(ContributionsRun instance);
        partial void UpdateContributionsRun(ContributionsRun instance);
        partial void DeleteContributionsRun(ContributionsRun instance);
        
        partial void InsertContributionStatus(ContributionStatus instance);
        partial void UpdateContributionStatus(ContributionStatus instance);
        partial void DeleteContributionStatus(ContributionStatus instance);
        
        partial void InsertContributionType(ContributionType instance);
        partial void UpdateContributionType(ContributionType instance);
        partial void DeleteContributionType(ContributionType instance);
        
        partial void InsertCountry(Country instance);
        partial void UpdateCountry(Country instance);
        partial void DeleteCountry(Country instance);
        
        partial void InsertCoupon(Coupon instance);
        partial void UpdateCoupon(Coupon instance);
        partial void DeleteCoupon(Coupon instance);
        
        partial void InsertCustomColumn(CustomColumn instance);
        partial void UpdateCustomColumn(CustomColumn instance);
        partial void DeleteCustomColumn(CustomColumn instance);
        
        partial void InsertDecisionType(DecisionType instance);
        partial void UpdateDecisionType(DecisionType instance);
        partial void DeleteDecisionType(DecisionType instance);
        
        partial void InsertDeleteMeetingRun(DeleteMeetingRun instance);
        partial void UpdateDeleteMeetingRun(DeleteMeetingRun instance);
        partial void DeleteDeleteMeetingRun(DeleteMeetingRun instance);
        
        partial void InsertDivision(Division instance);
        partial void UpdateDivision(Division instance);
        partial void DeleteDivision(Division instance);
        
        partial void InsertDivOrg(DivOrg instance);
        partial void UpdateDivOrg(DivOrg instance);
        partial void DeleteDivOrg(DivOrg instance);
        
        partial void InsertDownline(Downline instance);
        partial void UpdateDownline(Downline instance);
        partial void DeleteDownline(Downline instance);
        
        partial void InsertDownlineLeader(DownlineLeader instance);
        partial void UpdateDownlineLeader(DownlineLeader instance);
        partial void DeleteDownlineLeader(DownlineLeader instance);
        
        partial void InsertDropType(DropType instance);
        partial void UpdateDropType(DropType instance);
        partial void DeleteDropType(DropType instance);
        
        partial void InsertDuplicate(Duplicate instance);
        partial void UpdateDuplicate(Duplicate instance);
        partial void DeleteDuplicate(Duplicate instance);
        
        partial void InsertDuplicatesRun(DuplicatesRun instance);
        partial void UpdateDuplicatesRun(DuplicatesRun instance);
        partial void DeleteDuplicatesRun(DuplicatesRun instance);
        
        partial void InsertEmailLink(EmailLink instance);
        partial void UpdateEmailLink(EmailLink instance);
        partial void DeleteEmailLink(EmailLink instance);
        
        partial void InsertEmailLog(EmailLog instance);
        partial void UpdateEmailLog(EmailLog instance);
        partial void DeleteEmailLog(EmailLog instance);
        
        partial void InsertEmailOptOut(EmailOptOut instance);
        partial void UpdateEmailOptOut(EmailOptOut instance);
        partial void DeleteEmailOptOut(EmailOptOut instance);
        
        partial void InsertEmailQueue(EmailQueue instance);
        partial void UpdateEmailQueue(EmailQueue instance);
        partial void DeleteEmailQueue(EmailQueue instance);
        
        partial void InsertEmailQueueTo(EmailQueueTo instance);
        partial void UpdateEmailQueueTo(EmailQueueTo instance);
        partial void DeleteEmailQueueTo(EmailQueueTo instance);
        
        partial void InsertEmailQueueToFail(EmailQueueToFail instance);
        partial void UpdateEmailQueueToFail(EmailQueueToFail instance);
        partial void DeleteEmailQueueToFail(EmailQueueToFail instance);
        
        partial void InsertEmailResponse(EmailResponse instance);
        partial void UpdateEmailResponse(EmailResponse instance);
        partial void DeleteEmailResponse(EmailResponse instance);
        
        partial void InsertEmailToText(EmailToText instance);
        partial void UpdateEmailToText(EmailToText instance);
        partial void DeleteEmailToText(EmailToText instance);
        
        partial void InsertEnrollmentTransaction(EnrollmentTransaction instance);
        partial void UpdateEnrollmentTransaction(EnrollmentTransaction instance);
        partial void DeleteEnrollmentTransaction(EnrollmentTransaction instance);
        
        partial void InsertEntryPoint(EntryPoint instance);
        partial void UpdateEntryPoint(EntryPoint instance);
        partial void DeleteEntryPoint(EntryPoint instance);
        
        partial void InsertEnvelopeOption(EnvelopeOption instance);
        partial void UpdateEnvelopeOption(EnvelopeOption instance);
        partial void DeleteEnvelopeOption(EnvelopeOption instance);
        
        partial void InsertExtraDatum(ExtraDatum instance);
        partial void UpdateExtraDatum(ExtraDatum instance);
        partial void DeleteExtraDatum(ExtraDatum instance);
        
        partial void InsertFamily(Family instance);
        partial void UpdateFamily(Family instance);
        partial void DeleteFamily(Family instance);
        
        partial void InsertFamilyCheckinLock(FamilyCheckinLock instance);
        partial void UpdateFamilyCheckinLock(FamilyCheckinLock instance);
        partial void DeleteFamilyCheckinLock(FamilyCheckinLock instance);
        
        partial void InsertFamilyExtra(FamilyExtra instance);
        partial void UpdateFamilyExtra(FamilyExtra instance);
        partial void DeleteFamilyExtra(FamilyExtra instance);
        
        partial void InsertFamilyMemberType(FamilyMemberType instance);
        partial void UpdateFamilyMemberType(FamilyMemberType instance);
        partial void DeleteFamilyMemberType(FamilyMemberType instance);
        
        partial void InsertFamilyPosition(FamilyPosition instance);
        partial void UpdateFamilyPosition(FamilyPosition instance);
        partial void DeleteFamilyPosition(FamilyPosition instance);
        
        partial void InsertFamilyRelationship(FamilyRelationship instance);
        partial void UpdateFamilyRelationship(FamilyRelationship instance);
        partial void DeleteFamilyRelationship(FamilyRelationship instance);
        
        partial void InsertGender(Gender instance);
        partial void UpdateGender(Gender instance);
        partial void DeleteGender(Gender instance);
        
        partial void InsertGeoCode(GeoCode instance);
        partial void UpdateGeoCode(GeoCode instance);
        partial void DeleteGeoCode(GeoCode instance);
        
        partial void InsertGoerSenderAmount(GoerSenderAmount instance);
        partial void UpdateGoerSenderAmount(GoerSenderAmount instance);
        partial void DeleteGoerSenderAmount(GoerSenderAmount instance);
        
        partial void InsertGoerSupporter(GoerSupporter instance);
        partial void UpdateGoerSupporter(GoerSupporter instance);
        partial void DeleteGoerSupporter(GoerSupporter instance);
        
        partial void InsertInterestPoint(InterestPoint instance);
        partial void UpdateInterestPoint(InterestPoint instance);
        partial void DeleteInterestPoint(InterestPoint instance);
        
        partial void InsertJoinType(JoinType instance);
        partial void UpdateJoinType(JoinType instance);
        partial void DeleteJoinType(JoinType instance);
        
        partial void InsertLabelFormat(LabelFormat instance);
        partial void UpdateLabelFormat(LabelFormat instance);
        partial void DeleteLabelFormat(LabelFormat instance);
        
        partial void InsertLongRunningOp(LongRunningOp instance);
        partial void UpdateLongRunningOp(LongRunningOp instance);
        partial void DeleteLongRunningOp(LongRunningOp instance);
        
        partial void InsertManagedGiving(ManagedGiving instance);
        partial void UpdateManagedGiving(ManagedGiving instance);
        partial void DeleteManagedGiving(ManagedGiving instance);
        
        partial void InsertMaritalStatus(MaritalStatus instance);
        partial void UpdateMaritalStatus(MaritalStatus instance);
        partial void DeleteMaritalStatus(MaritalStatus instance);
        
        partial void InsertMeetingExtra(MeetingExtra instance);
        partial void UpdateMeetingExtra(MeetingExtra instance);
        partial void DeleteMeetingExtra(MeetingExtra instance);
        
        partial void InsertMeeting(Meeting instance);
        partial void UpdateMeeting(Meeting instance);
        partial void DeleteMeeting(Meeting instance);
        
        partial void InsertMeetingType(MeetingType instance);
        partial void UpdateMeetingType(MeetingType instance);
        partial void DeleteMeetingType(MeetingType instance);
        
        partial void InsertMemberDocForm(MemberDocForm instance);
        partial void UpdateMemberDocForm(MemberDocForm instance);
        partial void DeleteMemberDocForm(MemberDocForm instance);
        
        partial void InsertMemberLetterStatus(MemberLetterStatus instance);
        partial void UpdateMemberLetterStatus(MemberLetterStatus instance);
        partial void DeleteMemberLetterStatus(MemberLetterStatus instance);
        
        partial void InsertMemberStatus(MemberStatus instance);
        partial void UpdateMemberStatus(MemberStatus instance);
        partial void DeleteMemberStatus(MemberStatus instance);
        
        partial void InsertMemberTag(MemberTag instance);
        partial void UpdateMemberTag(MemberTag instance);
        partial void DeleteMemberTag(MemberTag instance);
        
        partial void InsertMemberType(MemberType instance);
        partial void UpdateMemberType(MemberType instance);
        partial void DeleteMemberType(MemberType instance);
        
        partial void InsertMergeHistory(MergeHistory instance);
        partial void UpdateMergeHistory(MergeHistory instance);
        partial void DeleteMergeHistory(MergeHistory instance);
        
        partial void InsertMinistry(Ministry instance);
        partial void UpdateMinistry(Ministry instance);
        partial void DeleteMinistry(Ministry instance);
        
        partial void InsertMobileAppAction(MobileAppAction instance);
        partial void UpdateMobileAppAction(MobileAppAction instance);
        partial void DeleteMobileAppAction(MobileAppAction instance);
        
        partial void InsertMobileAppActionType(MobileAppActionType instance);
        partial void UpdateMobileAppActionType(MobileAppActionType instance);
        partial void DeleteMobileAppActionType(MobileAppActionType instance);
        
        partial void InsertMobileAppAudioType(MobileAppAudioType instance);
        partial void UpdateMobileAppAudioType(MobileAppAudioType instance);
        partial void DeleteMobileAppAudioType(MobileAppAudioType instance);
        
        partial void InsertMobileAppBuilding(MobileAppBuilding instance);
        partial void UpdateMobileAppBuilding(MobileAppBuilding instance);
        partial void DeleteMobileAppBuilding(MobileAppBuilding instance);
        
        partial void InsertMobileAppFloor(MobileAppFloor instance);
        partial void UpdateMobileAppFloor(MobileAppFloor instance);
        partial void DeleteMobileAppFloor(MobileAppFloor instance);
        
        partial void InsertMobileAppIcon(MobileAppIcon instance);
        partial void UpdateMobileAppIcon(MobileAppIcon instance);
        partial void DeleteMobileAppIcon(MobileAppIcon instance);
        
        partial void InsertMobileAppIconSet(MobileAppIconSet instance);
        partial void UpdateMobileAppIconSet(MobileAppIconSet instance);
        partial void DeleteMobileAppIconSet(MobileAppIconSet instance);
        
        partial void InsertMobileAppPushRegistration(MobileAppPushRegistration instance);
        partial void UpdateMobileAppPushRegistration(MobileAppPushRegistration instance);
        partial void DeleteMobileAppPushRegistration(MobileAppPushRegistration instance);
        
        partial void InsertMobileAppRoom(MobileAppRoom instance);
        partial void UpdateMobileAppRoom(MobileAppRoom instance);
        partial void DeleteMobileAppRoom(MobileAppRoom instance);
        
        partial void InsertMobileAppVideoType(MobileAppVideoType instance);
        partial void UpdateMobileAppVideoType(MobileAppVideoType instance);
        partial void DeleteMobileAppVideoType(MobileAppVideoType instance);
        
        partial void InsertNewMemberClassStatus(NewMemberClassStatus instance);
        partial void UpdateNewMemberClassStatus(NewMemberClassStatus instance);
        partial void DeleteNewMemberClassStatus(NewMemberClassStatus instance);
        
        partial void InsertNumber(Number instance);
        partial void UpdateNumber(Number instance);
        partial void DeleteNumber(Number instance);
        
        partial void InsertOneTimeLink(OneTimeLink instance);
        partial void UpdateOneTimeLink(OneTimeLink instance);
        partial void DeleteOneTimeLink(OneTimeLink instance);
        
        partial void InsertOrganizationExtra(OrganizationExtra instance);
        partial void UpdateOrganizationExtra(OrganizationExtra instance);
        partial void DeleteOrganizationExtra(OrganizationExtra instance);
        
        partial void InsertOrganizationMember(OrganizationMember instance);
        partial void UpdateOrganizationMember(OrganizationMember instance);
        partial void DeleteOrganizationMember(OrganizationMember instance);
        
        partial void InsertOrganization(Organization instance);
        partial void UpdateOrganization(Organization instance);
        partial void DeleteOrganization(Organization instance);
        
        partial void InsertOrganizationStatus(OrganizationStatus instance);
        partial void UpdateOrganizationStatus(OrganizationStatus instance);
        partial void DeleteOrganizationStatus(OrganizationStatus instance);
        
        partial void InsertOrganizationType(OrganizationType instance);
        partial void UpdateOrganizationType(OrganizationType instance);
        partial void DeleteOrganizationType(OrganizationType instance);
        
        partial void InsertOrgContent(OrgContent instance);
        partial void UpdateOrgContent(OrgContent instance);
        partial void DeleteOrgContent(OrgContent instance);
        
        partial void InsertOrgMemMemTag(OrgMemMemTag instance);
        partial void UpdateOrgMemMemTag(OrgMemMemTag instance);
        partial void DeleteOrgMemMemTag(OrgMemMemTag instance);
        
        partial void InsertOrgSchedule(OrgSchedule instance);
        partial void UpdateOrgSchedule(OrgSchedule instance);
        partial void DeleteOrgSchedule(OrgSchedule instance);
        
        partial void InsertOrigin(Origin instance);
        partial void UpdateOrigin(Origin instance);
        partial void DeleteOrigin(Origin instance);
        
        partial void InsertPaymentInfo(PaymentInfo instance);
        partial void UpdatePaymentInfo(PaymentInfo instance);
        partial void DeletePaymentInfo(PaymentInfo instance);
        
        partial void InsertPerson(Person instance);
        partial void UpdatePerson(Person instance);
        partial void DeletePerson(Person instance);
        
        partial void InsertPeopleCanEmailFor(PeopleCanEmailFor instance);
        partial void UpdatePeopleCanEmailFor(PeopleCanEmailFor instance);
        partial void DeletePeopleCanEmailFor(PeopleCanEmailFor instance);
        
        partial void InsertPeopleExtra(PeopleExtra instance);
        partial void UpdatePeopleExtra(PeopleExtra instance);
        partial void DeletePeopleExtra(PeopleExtra instance);
        
        partial void InsertPicture(Picture instance);
        partial void UpdatePicture(Picture instance);
        partial void DeletePicture(Picture instance);
        
        partial void InsertPostalLookup(PostalLookup instance);
        partial void UpdatePostalLookup(PostalLookup instance);
        partial void DeletePostalLookup(PostalLookup instance);
        
        partial void InsertPreference(Preference instance);
        partial void UpdatePreference(Preference instance);
        partial void DeletePreference(Preference instance);
        
        partial void InsertPrintJob(PrintJob instance);
        partial void UpdatePrintJob(PrintJob instance);
        partial void DeletePrintJob(PrintJob instance);
        
        partial void InsertProgDiv(ProgDiv instance);
        partial void UpdateProgDiv(ProgDiv instance);
        partial void DeleteProgDiv(ProgDiv instance);
        
        partial void InsertProgram(Program instance);
        partial void UpdateProgram(Program instance);
        partial void DeleteProgram(Program instance);
        
        partial void InsertPromotion(Promotion instance);
        partial void UpdatePromotion(Promotion instance);
        partial void DeletePromotion(Promotion instance);
        
        partial void InsertQBConnection(QBConnection instance);
        partial void UpdateQBConnection(QBConnection instance);
        partial void DeleteQBConnection(QBConnection instance);
        
        partial void InsertQuery(Query instance);
        partial void UpdateQuery(Query instance);
        partial void DeleteQuery(Query instance);
        
        partial void InsertQueryAnalysi(QueryAnalysi instance);
        partial void UpdateQueryAnalysi(QueryAnalysi instance);
        partial void DeleteQueryAnalysi(QueryAnalysi instance);
        
        partial void InsertQueryStat(QueryStat instance);
        partial void UpdateQueryStat(QueryStat instance);
        partial void DeleteQueryStat(QueryStat instance);
        
        partial void InsertRecReg(RecReg instance);
        partial void UpdateRecReg(RecReg instance);
        partial void DeleteRecReg(RecReg instance);
        
        partial void InsertRecurringAmount(RecurringAmount instance);
        partial void UpdateRecurringAmount(RecurringAmount instance);
        partial void DeleteRecurringAmount(RecurringAmount instance);
        
        partial void InsertRegistrationDatum(RegistrationDatum instance);
        partial void UpdateRegistrationDatum(RegistrationDatum instance);
        partial void DeleteRegistrationDatum(RegistrationDatum instance);
        
        partial void InsertRelatedFamily(RelatedFamily instance);
        partial void UpdateRelatedFamily(RelatedFamily instance);
        partial void DeleteRelatedFamily(RelatedFamily instance);
        
        partial void InsertRepairTransactionsRun(RepairTransactionsRun instance);
        partial void UpdateRepairTransactionsRun(RepairTransactionsRun instance);
        partial void DeleteRepairTransactionsRun(RepairTransactionsRun instance);
        
        partial void InsertResidentCode(ResidentCode instance);
        partial void UpdateResidentCode(ResidentCode instance);
        partial void DeleteResidentCode(ResidentCode instance);
        
        partial void InsertRole(Role instance);
        partial void UpdateRole(Role instance);
        partial void DeleteRole(Role instance);
        
        partial void InsertRssFeed(RssFeed instance);
        partial void UpdateRssFeed(RssFeed instance);
        partial void DeleteRssFeed(RssFeed instance);
        
        partial void InsertSecurityCode(SecurityCode instance);
        partial void UpdateSecurityCode(SecurityCode instance);
        partial void DeleteSecurityCode(SecurityCode instance);
        
        partial void InsertSetting(Setting instance);
        partial void UpdateSetting(Setting instance);
        partial void DeleteSetting(Setting instance);
        
        partial void InsertSMSGroupMember(SMSGroupMember instance);
        partial void UpdateSMSGroupMember(SMSGroupMember instance);
        partial void DeleteSMSGroupMember(SMSGroupMember instance);
        
        partial void InsertSMSGroup(SMSGroup instance);
        partial void UpdateSMSGroup(SMSGroup instance);
        partial void DeleteSMSGroup(SMSGroup instance);
        
        partial void InsertSMSItem(SMSItem instance);
        partial void UpdateSMSItem(SMSItem instance);
        partial void DeleteSMSItem(SMSItem instance);
        
        partial void InsertSMSList(SMSList instance);
        partial void UpdateSMSList(SMSList instance);
        partial void DeleteSMSList(SMSList instance);
        
        partial void InsertSMSNumber(SMSNumber instance);
        partial void UpdateSMSNumber(SMSNumber instance);
        partial void DeleteSMSNumber(SMSNumber instance);
        
        partial void InsertStateLookup(StateLookup instance);
        partial void UpdateStateLookup(StateLookup instance);
        partial void DeleteStateLookup(StateLookup instance);
        
        partial void InsertStreetType(StreetType instance);
        partial void UpdateStreetType(StreetType instance);
        partial void DeleteStreetType(StreetType instance);
        
        partial void InsertSubRequest(SubRequest instance);
        partial void UpdateSubRequest(SubRequest instance);
        partial void DeleteSubRequest(SubRequest instance);
        
        partial void InsertTag(Tag instance);
        partial void UpdateTag(Tag instance);
        partial void DeleteTag(Tag instance);
        
        partial void InsertTagPerson(TagPerson instance);
        partial void UpdateTagPerson(TagPerson instance);
        partial void DeleteTagPerson(TagPerson instance);
        
        partial void InsertTagShare(TagShare instance);
        partial void UpdateTagShare(TagShare instance);
        partial void DeleteTagShare(TagShare instance);
        
        partial void InsertTagType(TagType instance);
        partial void UpdateTagType(TagType instance);
        partial void DeleteTagType(TagType instance);
        
        partial void InsertTask(Task instance);
        partial void UpdateTask(Task instance);
        partial void DeleteTask(Task instance);
        
        partial void InsertTaskList(TaskList instance);
        partial void UpdateTaskList(TaskList instance);
        partial void DeleteTaskList(TaskList instance);
        
        partial void InsertTaskListOwner(TaskListOwner instance);
        partial void UpdateTaskListOwner(TaskListOwner instance);
        partial void DeleteTaskListOwner(TaskListOwner instance);
        
        partial void InsertTaskStatus(TaskStatus instance);
        partial void UpdateTaskStatus(TaskStatus instance);
        partial void DeleteTaskStatus(TaskStatus instance);
        
        partial void InsertTransaction(Transaction instance);
        partial void UpdateTransaction(Transaction instance);
        partial void DeleteTransaction(Transaction instance);
        
        partial void InsertTransactionPerson(TransactionPerson instance);
        partial void UpdateTransactionPerson(TransactionPerson instance);
        partial void DeleteTransactionPerson(TransactionPerson instance);
        
        partial void InsertUploadPeopleRun(UploadPeopleRun instance);
        partial void UpdateUploadPeopleRun(UploadPeopleRun instance);
        partial void DeleteUploadPeopleRun(UploadPeopleRun instance);
        
        partial void InsertUserRole(UserRole instance);
        partial void UpdateUserRole(UserRole instance);
        partial void DeleteUserRole(UserRole instance);
        
        partial void InsertUser(User instance);
        partial void UpdateUser(User instance);
        partial void DeleteUser(User instance);
        
        partial void InsertVolApplicationStatus(VolApplicationStatus instance);
        partial void UpdateVolApplicationStatus(VolApplicationStatus instance);
        partial void DeleteVolApplicationStatus(VolApplicationStatus instance);
        
        partial void InsertVolInterestCode(VolInterestCode instance);
        partial void UpdateVolInterestCode(VolInterestCode instance);
        partial void DeleteVolInterestCode(VolInterestCode instance);
        
        partial void InsertVolInterestInterestCode(VolInterestInterestCode instance);
        partial void UpdateVolInterestInterestCode(VolInterestInterestCode instance);
        partial void DeleteVolInterestInterestCode(VolInterestInterestCode instance);
        
        partial void InsertVolRequest(VolRequest instance);
        partial void UpdateVolRequest(VolRequest instance);
        partial void DeleteVolRequest(VolRequest instance);
        
        partial void InsertVolunteer(Volunteer instance);
        partial void UpdateVolunteer(Volunteer instance);
        partial void DeleteVolunteer(Volunteer instance);
        
        partial void InsertVolunteerCode(VolunteerCode instance);
        partial void UpdateVolunteerCode(VolunteerCode instance);
        partial void DeleteVolunteerCode(VolunteerCode instance);
        
        partial void InsertVolunteerForm(VolunteerForm instance);
        partial void UpdateVolunteerForm(VolunteerForm instance);
        partial void DeleteVolunteerForm(VolunteerForm instance);
        
        partial void InsertVoluteerApprovalId(VoluteerApprovalId instance);
        partial void UpdateVoluteerApprovalId(VoluteerApprovalId instance);
        partial void DeleteVoluteerApprovalId(VoluteerApprovalId instance);
        
        partial void InsertWord(Word instance);
        partial void UpdateWord(Word instance);
        partial void DeleteWord(Word instance);
        
        partial void InsertZipCode(ZipCode instance);
        partial void UpdateZipCode(ZipCode instance);
        partial void DeleteZipCode(ZipCode instance);
        
        partial void InsertZip(Zip instance);
        partial void UpdateZip(Zip instance);
        partial void DeleteZip(Zip instance);
        
#endregion
        public CMSDataContext(string connectionString) :
                base(new ProfiledDbConnection(new SqlConnection(connectionString), MiniProfiler.Current), _mappingSource)
        {
            OnCreated();
        }

#region Tables
        
        public Table< ActivityLog> ActivityLogs
        {
            get { return GetTable< ActivityLog>(); }

        }

        public Table< Address> Addresses
        {
            get { return GetTable< Address>(); }

        }

        public Table< AddressType> AddressTypes
        {
            get { return GetTable< AddressType>(); }

        }

        public Table< AddToOrgFromTagRun> AddToOrgFromTagRuns
        {
            get { return GetTable< AddToOrgFromTagRun>(); }

        }

        public Table< ApiSession> ApiSessions
        {
            get { return GetTable< ApiSession>(); }

        }

        public Table< Attend> Attends
        {
            get { return GetTable< Attend>(); }

        }

        public Table< AttendCredit> AttendCredits
        {
            get { return GetTable< AttendCredit>(); }

        }

        public Table< AttendType> AttendTypes
        {
            get { return GetTable< AttendType>(); }

        }

        public Table< Audit> Audits
        {
            get { return GetTable< Audit>(); }

        }

        public Table< AuditValue> AuditValues
        {
            get { return GetTable< AuditValue>(); }

        }

        public Table< BackgroundCheckLabel> BackgroundCheckLabels
        {
            get { return GetTable< BackgroundCheckLabel>(); }

        }

        public Table< BackgroundCheckMVRCode> BackgroundCheckMVRCodes
        {
            get { return GetTable< BackgroundCheckMVRCode>(); }

        }

        public Table< BackgroundCheck> BackgroundChecks
        {
            get { return GetTable< BackgroundCheck>(); }

        }

        public Table< BaptismStatus> BaptismStatuses
        {
            get { return GetTable< BaptismStatus>(); }

        }

        public Table< BaptismType> BaptismTypes
        {
            get { return GetTable< BaptismType>(); }

        }

        public Table< BuildingAccessType> BuildingAccessTypes
        {
            get { return GetTable< BuildingAccessType>(); }

        }

        public Table< BundleDetail> BundleDetails
        {
            get { return GetTable< BundleDetail>(); }

        }

        public Table< BundleHeader> BundleHeaders
        {
            get { return GetTable< BundleHeader>(); }

        }

        public Table< BundleHeaderType> BundleHeaderTypes
        {
            get { return GetTable< BundleHeaderType>(); }

        }

        public Table< BundleStatusType> BundleStatusTypes
        {
            get { return GetTable< BundleStatusType>(); }

        }

        public Table< Campu> Campus
        {
            get { return GetTable< Campu>(); }

        }

        public Table< CardIdentifier> CardIdentifiers
        {
            get { return GetTable< CardIdentifier>(); }

        }

        public Table< ChangeDetail> ChangeDetails
        {
            get { return GetTable< ChangeDetail>(); }

        }

        public Table< ChangeLog> ChangeLogs
        {
            get { return GetTable< ChangeLog>(); }

        }

        public Table< CheckedBatch> CheckedBatches
        {
            get { return GetTable< CheckedBatch>(); }

        }

        public Table< CheckInActivity> CheckInActivities
        {
            get { return GetTable< CheckInActivity>(); }

        }

        public Table< CheckInSetting> CheckInSettings
        {
            get { return GetTable< CheckInSetting>(); }

        }

        public Table< CheckInTime> CheckInTimes
        {
            get { return GetTable< CheckInTime>(); }

        }

        public Table< ChurchAttReportId> ChurchAttReportIds
        {
            get { return GetTable< ChurchAttReportId>(); }

        }

        public Table< Contact> Contacts
        {
            get { return GetTable< Contact>(); }

        }

        public Table< Contactee> Contactees
        {
            get { return GetTable< Contactee>(); }

        }

        public Table< Contactor> Contactors
        {
            get { return GetTable< Contactor>(); }

        }

        public Table< ContactPreference> ContactPreferences
        {
            get { return GetTable< ContactPreference>(); }

        }

        public Table< ContactReason> ContactReasons
        {
            get { return GetTable< ContactReason>(); }

        }

        public Table< ContactType> ContactTypes
        {
            get { return GetTable< ContactType>(); }

        }

        public Table< Content> Contents
        {
            get { return GetTable< Content>(); }

        }

        public Table< ContentKeyWord> ContentKeyWords
        {
            get { return GetTable< ContentKeyWord>(); }

        }

        public Table< Contribution> Contributions
        {
            get { return GetTable< Contribution>(); }

        }

        public Table< ContributionFund> ContributionFunds
        {
            get { return GetTable< ContributionFund>(); }

        }

        public Table< ContributionsRun> ContributionsRuns
        {
            get { return GetTable< ContributionsRun>(); }

        }

        public Table< ContributionStatus> ContributionStatuses
        {
            get { return GetTable< ContributionStatus>(); }

        }

        public Table< ContributionType> ContributionTypes
        {
            get { return GetTable< ContributionType>(); }

        }

        public Table< Country> Countries
        {
            get { return GetTable< Country>(); }

        }

        public Table< Coupon> Coupons
        {
            get { return GetTable< Coupon>(); }

        }

        public Table< CustomColumn> CustomColumns
        {
            get { return GetTable< CustomColumn>(); }

        }

        public Table< DecisionType> DecisionTypes
        {
            get { return GetTable< DecisionType>(); }

        }

        public Table< DeleteMeetingRun> DeleteMeetingRuns
        {
            get { return GetTable< DeleteMeetingRun>(); }

        }

        public Table< Division> Divisions
        {
            get { return GetTable< Division>(); }

        }

        public Table< DivOrg> DivOrgs
        {
            get { return GetTable< DivOrg>(); }

        }

        public Table< Downline> Downlines
        {
            get { return GetTable< Downline>(); }

        }

        public Table< DownlineLeader> DownlineLeaders
        {
            get { return GetTable< DownlineLeader>(); }

        }

        public Table< DropType> DropTypes
        {
            get { return GetTable< DropType>(); }

        }

        public Table< Duplicate> Duplicates
        {
            get { return GetTable< Duplicate>(); }

        }

        public Table< DuplicatesRun> DuplicatesRuns
        {
            get { return GetTable< DuplicatesRun>(); }

        }

        public Table< EmailLink> EmailLinks
        {
            get { return GetTable< EmailLink>(); }

        }

        public Table< EmailLog> EmailLogs
        {
            get { return GetTable< EmailLog>(); }

        }

        public Table< EmailOptOut> EmailOptOuts
        {
            get { return GetTable< EmailOptOut>(); }

        }

        public Table< EmailQueue> EmailQueues
        {
            get { return GetTable< EmailQueue>(); }

        }

        public Table< EmailQueueTo> EmailQueueTos
        {
            get { return GetTable< EmailQueueTo>(); }

        }

        public Table< EmailQueueToFail> EmailQueueToFails
        {
            get { return GetTable< EmailQueueToFail>(); }

        }

        public Table< EmailResponse> EmailResponses
        {
            get { return GetTable< EmailResponse>(); }

        }

        public Table< EmailToText> EmailToTexts
        {
            get { return GetTable< EmailToText>(); }

        }

        public Table< EnrollmentTransaction> EnrollmentTransactions
        {
            get { return GetTable< EnrollmentTransaction>(); }

        }

        public Table< EntryPoint> EntryPoints
        {
            get { return GetTable< EntryPoint>(); }

        }

        public Table< EnvelopeOption> EnvelopeOptions
        {
            get { return GetTable< EnvelopeOption>(); }

        }

        public Table< ExtraDatum> ExtraDatas
        {
            get { return GetTable< ExtraDatum>(); }

        }

        public Table< Family> Families
        {
            get { return GetTable< Family>(); }

        }

        public Table< FamilyCheckinLock> FamilyCheckinLocks
        {
            get { return GetTable< FamilyCheckinLock>(); }

        }

        public Table< FamilyExtra> FamilyExtras
        {
            get { return GetTable< FamilyExtra>(); }

        }

        public Table< FamilyMemberType> FamilyMemberTypes
        {
            get { return GetTable< FamilyMemberType>(); }

        }

        public Table< FamilyPosition> FamilyPositions
        {
            get { return GetTable< FamilyPosition>(); }

        }

        public Table< FamilyRelationship> FamilyRelationships
        {
            get { return GetTable< FamilyRelationship>(); }

        }

        public Table< Gender> Genders
        {
            get { return GetTable< Gender>(); }

        }

        public Table< GeoCode> GeoCodes
        {
            get { return GetTable< GeoCode>(); }

        }

        public Table< GoerSenderAmount> GoerSenderAmounts
        {
            get { return GetTable< GoerSenderAmount>(); }

        }

        public Table< GoerSupporter> GoerSupporters
        {
            get { return GetTable< GoerSupporter>(); }

        }

        public Table< InterestPoint> InterestPoints
        {
            get { return GetTable< InterestPoint>(); }

        }

        public Table< JoinType> JoinTypes
        {
            get { return GetTable< JoinType>(); }

        }

        public Table< LabelFormat> LabelFormats
        {
            get { return GetTable< LabelFormat>(); }

        }

        public Table< LongRunningOp> LongRunningOps
        {
            get { return GetTable< LongRunningOp>(); }

        }

        public Table< ManagedGiving> ManagedGivings
        {
            get { return GetTable< ManagedGiving>(); }

        }

        public Table< MaritalStatus> MaritalStatuses
        {
            get { return GetTable< MaritalStatus>(); }

        }

        public Table< MeetingExtra> MeetingExtras
        {
            get { return GetTable< MeetingExtra>(); }

        }

        public Table< Meeting> Meetings
        {
            get { return GetTable< Meeting>(); }

        }

        public Table< MeetingType> MeetingTypes
        {
            get { return GetTable< MeetingType>(); }

        }

        public Table< MemberDocForm> MemberDocForms
        {
            get { return GetTable< MemberDocForm>(); }

        }

        public Table< MemberLetterStatus> MemberLetterStatuses
        {
            get { return GetTable< MemberLetterStatus>(); }

        }

        public Table< MemberStatus> MemberStatuses
        {
            get { return GetTable< MemberStatus>(); }

        }

        public Table< MemberTag> MemberTags
        {
            get { return GetTable< MemberTag>(); }

        }

        public Table< MemberType> MemberTypes
        {
            get { return GetTable< MemberType>(); }

        }

        public Table< MergeHistory> MergeHistories
        {
            get { return GetTable< MergeHistory>(); }

        }

        public Table< Ministry> Ministries
        {
            get { return GetTable< Ministry>(); }

        }

        public Table< MobileAppAction> MobileAppActions
        {
            get { return GetTable< MobileAppAction>(); }

        }

        public Table< MobileAppActionType> MobileAppActionTypes
        {
            get { return GetTable< MobileAppActionType>(); }

        }

        public Table< MobileAppAudioType> MobileAppAudioTypes
        {
            get { return GetTable< MobileAppAudioType>(); }

        }

        public Table< MobileAppBuilding> MobileAppBuildings
        {
            get { return GetTable< MobileAppBuilding>(); }

        }

        public Table< MobileAppFloor> MobileAppFloors
        {
            get { return GetTable< MobileAppFloor>(); }

        }

        public Table< MobileAppIcon> MobileAppIcons
        {
            get { return GetTable< MobileAppIcon>(); }

        }

        public Table< MobileAppIconSet> MobileAppIconSets
        {
            get { return GetTable< MobileAppIconSet>(); }

        }

        public Table< MobileAppPushRegistration> MobileAppPushRegistrations
        {
            get { return GetTable< MobileAppPushRegistration>(); }

        }

        public Table< MobileAppRoom> MobileAppRooms
        {
            get { return GetTable< MobileAppRoom>(); }

        }

        public Table< MobileAppVideoType> MobileAppVideoTypes
        {
            get { return GetTable< MobileAppVideoType>(); }

        }

        public Table< NewMemberClassStatus> NewMemberClassStatuses
        {
            get { return GetTable< NewMemberClassStatus>(); }

        }

        public Table< Number> Numbers
        {
            get { return GetTable< Number>(); }

        }

        public Table< OneTimeLink> OneTimeLinks
        {
            get { return GetTable< OneTimeLink>(); }

        }

        public Table< OrganizationExtra> OrganizationExtras
        {
            get { return GetTable< OrganizationExtra>(); }

        }

        public Table< OrganizationMember> OrganizationMembers
        {
            get { return GetTable< OrganizationMember>(); }

        }

        public Table< Organization> Organizations
        {
            get { return GetTable< Organization>(); }

        }

        public Table< OrganizationStatus> OrganizationStatuses
        {
            get { return GetTable< OrganizationStatus>(); }

        }

        public Table< OrganizationType> OrganizationTypes
        {
            get { return GetTable< OrganizationType>(); }

        }

        public Table< OrgContent> OrgContents
        {
            get { return GetTable< OrgContent>(); }

        }

        public Table< OrgMemMemTag> OrgMemMemTags
        {
            get { return GetTable< OrgMemMemTag>(); }

        }

        public Table< OrgSchedule> OrgSchedules
        {
            get { return GetTable< OrgSchedule>(); }

        }

        public Table< Origin> Origins
        {
            get { return GetTable< Origin>(); }

        }

        public Table< PaymentInfo> PaymentInfos
        {
            get { return GetTable< PaymentInfo>(); }

        }

        public Table< Person> People
        {
            get { return GetTable< Person>(); }

        }

        public Table< PeopleCanEmailFor> PeopleCanEmailFors
        {
            get { return GetTable< PeopleCanEmailFor>(); }

        }

        public Table< PeopleExtra> PeopleExtras
        {
            get { return GetTable< PeopleExtra>(); }

        }

        public Table< Picture> Pictures
        {
            get { return GetTable< Picture>(); }

        }

        public Table< PostalLookup> PostalLookups
        {
            get { return GetTable< PostalLookup>(); }

        }

        public Table< Preference> Preferences
        {
            get { return GetTable< Preference>(); }

        }

        public Table< PrintJob> PrintJobs
        {
            get { return GetTable< PrintJob>(); }

        }

        public Table< ProgDiv> ProgDivs
        {
            get { return GetTable< ProgDiv>(); }

        }

        public Table< Program> Programs
        {
            get { return GetTable< Program>(); }

        }

        public Table< Promotion> Promotions
        {
            get { return GetTable< Promotion>(); }

        }

        public Table< QBConnection> QBConnections
        {
            get { return GetTable< QBConnection>(); }

        }

        public Table< Query> Queries
        {
            get { return GetTable< Query>(); }

        }

        public Table< QueryAnalysi> QueryAnalyses
        {
            get { return GetTable< QueryAnalysi>(); }

        }

        public Table< QueryStat> QueryStats
        {
            get { return GetTable< QueryStat>(); }

        }

        public Table< RecReg> RecRegs
        {
            get { return GetTable< RecReg>(); }

        }

        public Table< RecurringAmount> RecurringAmounts
        {
            get { return GetTable< RecurringAmount>(); }

        }

        public Table< RegistrationDatum> RegistrationDatas
        {
            get { return GetTable< RegistrationDatum>(); }

        }

        public Table< RelatedFamily> RelatedFamilies
        {
            get { return GetTable< RelatedFamily>(); }

        }

        public Table< RepairTransactionsRun> RepairTransactionsRuns
        {
            get { return GetTable< RepairTransactionsRun>(); }

        }

        public Table< ResidentCode> ResidentCodes
        {
            get { return GetTable< ResidentCode>(); }

        }

        public Table< Role> Roles
        {
            get { return GetTable< Role>(); }

        }

        public Table< RssFeed> RssFeeds
        {
            get { return GetTable< RssFeed>(); }

        }

        public Table< SecurityCode> SecurityCodes
        {
            get { return GetTable< SecurityCode>(); }

        }

        public Table< Setting> Settings
        {
            get { return GetTable< Setting>(); }

        }

        public Table< SMSGroupMember> SMSGroupMembers
        {
            get { return GetTable< SMSGroupMember>(); }

        }

        public Table< SMSGroup> SMSGroups
        {
            get { return GetTable< SMSGroup>(); }

        }

        public Table< SMSItem> SMSItems
        {
            get { return GetTable< SMSItem>(); }

        }

        public Table< SMSList> SMSLists
        {
            get { return GetTable< SMSList>(); }

        }

        public Table< SMSNumber> SMSNumbers
        {
            get { return GetTable< SMSNumber>(); }

        }

        public Table< StateLookup> StateLookups
        {
            get { return GetTable< StateLookup>(); }

        }

        public Table< StreetType> StreetTypes
        {
            get { return GetTable< StreetType>(); }

        }

        public Table< SubRequest> SubRequests
        {
            get { return GetTable< SubRequest>(); }

        }

        public Table< Tag> Tags
        {
            get { return GetTable< Tag>(); }

        }

        public Table< TagPerson> TagPeople
        {
            get { return GetTable< TagPerson>(); }

        }

        public Table< TagShare> TagShares
        {
            get { return GetTable< TagShare>(); }

        }

        public Table< TagType> TagTypes
        {
            get { return GetTable< TagType>(); }

        }

        public Table< Task> Tasks
        {
            get { return GetTable< Task>(); }

        }

        public Table< TaskList> TaskLists
        {
            get { return GetTable< TaskList>(); }

        }

        public Table< TaskListOwner> TaskListOwners
        {
            get { return GetTable< TaskListOwner>(); }

        }

        public Table< TaskStatus> TaskStatuses
        {
            get { return GetTable< TaskStatus>(); }

        }

        public Table< Transaction> Transactions
        {
            get { return GetTable< Transaction>(); }

        }

        public Table< TransactionPerson> TransactionPeople
        {
            get { return GetTable< TransactionPerson>(); }

        }

        public Table< UploadPeopleRun> UploadPeopleRuns
        {
            get { return GetTable< UploadPeopleRun>(); }

        }

        public Table< UserRole> UserRoles
        {
            get { return GetTable< UserRole>(); }

        }

        public Table< User> Users
        {
            get { return GetTable< User>(); }

        }

        public Table< VolApplicationStatus> VolApplicationStatuses
        {
            get { return GetTable< VolApplicationStatus>(); }

        }

        public Table< VolInterestCode> VolInterestCodes
        {
            get { return GetTable< VolInterestCode>(); }

        }

        public Table< VolInterestInterestCode> VolInterestInterestCodes
        {
            get { return GetTable< VolInterestInterestCode>(); }

        }

        public Table< VolRequest> VolRequests
        {
            get { return GetTable< VolRequest>(); }

        }

        public Table< Volunteer> Volunteers
        {
            get { return GetTable< Volunteer>(); }

        }

        public Table< VolunteerCode> VolunteerCodes
        {
            get { return GetTable< VolunteerCode>(); }

        }

        public Table< VolunteerForm> VolunteerForms
        {
            get { return GetTable< VolunteerForm>(); }

        }

        public Table< VoluteerApprovalId> VoluteerApprovalIds
        {
            get { return GetTable< VoluteerApprovalId>(); }

        }

        public Table< Word> Words
        {
            get { return GetTable< Word>(); }

        }

        public Table< ZipCode> ZipCodes
        {
            get { return GetTable< ZipCode>(); }

        }

        public Table< Zip> Zips
        {
            get { return GetTable< Zip>(); }

        }

#endregion
#region Views
        
        public Table< View.ActiveRegistration> ViewActiveRegistrations
        {
            get { return GetTable< View.ActiveRegistration>(); }

        }

        public Table< View.ActivityAll> ViewActivityAlls
        {
            get { return GetTable< View.ActivityAll>(); }

        }

        public Table< View.AllStatusFlag> ViewAllStatusFlags
        {
            get { return GetTable< View.AllStatusFlag>(); }

        }

        public Table< View.AppRegistration> ViewAppRegistrations
        {
            get { return GetTable< View.AppRegistration>(); }

        }

        public Table< View.AttendCredit> ViewAttendCredits
        {
            get { return GetTable< View.AttendCredit>(); }

        }

        public Table< View.AttendCredits2> ViewAttendCredits2s
        {
            get { return GetTable< View.AttendCredits2>(); }

        }

        public Table< View.BundleList> ViewBundleLists
        {
            get { return GetTable< View.BundleList>(); }

        }

        public Table< View.ChangeLogDetail> ViewChangeLogDetails
        {
            get { return GetTable< View.ChangeLogDetail>(); }

        }

        public Table< View.Church> ViewChurches
        {
            get { return GetTable< View.Church>(); }

        }

        public Table< View.City> ViewCities
        {
            get { return GetTable< View.City>(); }

        }

        public Table< View.ContributionsBasic> ViewContributionsBasics
        {
            get { return GetTable< View.ContributionsBasic>(); }

        }

        public Table< View.ContributionsView> ViewContributionsViews
        {
            get { return GetTable< View.ContributionsView>(); }

        }

        public Table< View.DonorProfileList> ViewDonorProfileLists
        {
            get { return GetTable< View.DonorProfileList>(); }

        }

        public Table< View.FailedEmail> ViewFailedEmails
        {
            get { return GetTable< View.FailedEmail>(); }

        }

        public Table< View.FamilyFirstTime> ViewFamilyFirstTimes
        {
            get { return GetTable< View.FamilyFirstTime>(); }

        }

        public Table< View.FirstName> ViewFirstNames
        {
            get { return GetTable< View.FirstName>(); }

        }

        public Table< View.FirstName2> ViewFirstName2s
        {
            get { return GetTable< View.FirstName2>(); }

        }

        public Table< View.FirstNick> ViewFirstNicks
        {
            get { return GetTable< View.FirstNick>(); }

        }

        public Table< View.HeadOrSpouseWithEmail> ViewHeadOrSpouseWithEmails
        {
            get { return GetTable< View.HeadOrSpouseWithEmail>(); }

        }

        public Table< View.IncompleteTask> ViewIncompleteTasks
        {
            get { return GetTable< View.IncompleteTask>(); }

        }

        public Table< View.LastAttend> ViewLastAttends
        {
            get { return GetTable< View.LastAttend>(); }

        }

        public Table< View.LastName> ViewLastNames
        {
            get { return GetTable< View.LastName>(); }

        }

        public Table< View.MasterOrg> ViewMasterOrgs
        {
            get { return GetTable< View.MasterOrg>(); }

        }

        public Table< View.MeetingConflict> ViewMeetingConflicts
        {
            get { return GetTable< View.MeetingConflict>(); }

        }

        public Table< View.MemberDatum> ViewMemberDatas
        {
            get { return GetTable< View.MemberDatum>(); }

        }

        public Table< View.MinistryInfo> ViewMinistryInfos
        {
            get { return GetTable< View.MinistryInfo>(); }

        }

        public Table< View.MissionTripTotal> ViewMissionTripTotals
        {
            get { return GetTable< View.MissionTripTotal>(); }

        }

        public Table< View.Nick> ViewNicks
        {
            get { return GetTable< View.Nick>(); }

        }

        public Table< View.OnlineRegQA> ViewOnlineRegQAs
        {
            get { return GetTable< View.OnlineRegQA>(); }

        }

        public Table< View.OrganizationLeader> ViewOrganizationLeaders
        {
            get { return GetTable< View.OrganizationLeader>(); }

        }

        public Table< View.OrganizationStructure> ViewOrganizationStructures
        {
            get { return GetTable< View.OrganizationStructure>(); }

        }

        public Table< View.OrgSchedules2> ViewOrgSchedules2s
        {
            get { return GetTable< View.OrgSchedules2>(); }

        }

        public Table< View.OrgsWithFee> ViewOrgsWithFees
        {
            get { return GetTable< View.OrgsWithFee>(); }

        }

        public Table< View.OrgsWithoutFee> ViewOrgsWithoutFees
        {
            get { return GetTable< View.OrgsWithoutFee>(); }

        }

        public Table< View.PeopleBasicModifed> ViewPeopleBasicModifeds
        {
            get { return GetTable< View.PeopleBasicModifed>(); }

        }

        public Table< View.PickListOrg> ViewPickListOrgs
        {
            get { return GetTable< View.PickListOrg>(); }

        }

        public Table< View.PickListOrgs2> ViewPickListOrgs2s
        {
            get { return GetTable< View.PickListOrgs2>(); }

        }

        public Table< View.PreviousMemberCount> ViewPreviousMemberCounts
        {
            get { return GetTable< View.PreviousMemberCount>(); }

        }

        public Table< View.ProspectCount> ViewProspectCounts
        {
            get { return GetTable< View.ProspectCount>(); }

        }

        public Table< View.RandNumber> ViewRandNumbers
        {
            get { return GetTable< View.RandNumber>(); }

        }

        public Table< View.RegistrationList> ViewRegistrationLists
        {
            get { return GetTable< View.RegistrationList>(); }

        }

        public Table< View.RegsettingCount> ViewRegsettingCounts
        {
            get { return GetTable< View.RegsettingCount>(); }

        }

        public Table< View.RegsettingMessage> ViewRegsettingMessages
        {
            get { return GetTable< View.RegsettingMessage>(); }

        }

        public Table< View.RegsettingOption> ViewRegsettingOptions
        {
            get { return GetTable< View.RegsettingOption>(); }

        }

        public Table< View.RegsettingUsage> ViewRegsettingUsages
        {
            get { return GetTable< View.RegsettingUsage>(); }

        }

        public Table< View.SpouseOrHeadWithEmail> ViewSpouseOrHeadWithEmails
        {
            get { return GetTable< View.SpouseOrHeadWithEmail>(); }

        }

        public Table< View.Sproc> ViewSprocs
        {
            get { return GetTable< View.Sproc>(); }

        }

        public Table< View.StatusFlagColumn> ViewStatusFlagColumns
        {
            get { return GetTable< View.StatusFlagColumn>(); }

        }

        public Table< View.StatusFlagList> ViewStatusFlagLists
        {
            get { return GetTable< View.StatusFlagList>(); }

        }

        public Table< View.StatusFlagNamesRole> ViewStatusFlagNamesRoles
        {
            get { return GetTable< View.StatusFlagNamesRole>(); }

        }

        public Table< View.TransactionBalance> ViewTransactionBalances
        {
            get { return GetTable< View.TransactionBalance>(); }

        }

        public Table< View.TransactionList> ViewTransactionLists
        {
            get { return GetTable< View.TransactionList>(); }

        }

        public Table< View.TransactionSummary> ViewTransactionSummaries
        {
            get { return GetTable< View.TransactionSummary>(); }

        }

        public Table< View.Trigger> ViewTriggers
        {
            get { return GetTable< View.Trigger>(); }

        }

        public Table< View.UserLeader> ViewUserLeaders
        {
            get { return GetTable< View.UserLeader>(); }

        }

        public Table< View.UserList> ViewUserLists
        {
            get { return GetTable< View.UserList>(); }

        }

        public Table< View.UserRole> ViewUserRoles
        {
            get { return GetTable< View.UserRole>(); }

        }

        public Table< View.VolunteerTime> ViewVolunteerTimes
        {
            get { return GetTable< View.VolunteerTime>(); }

        }

#endregion
#region Table Functions
        
        [Function(Name="dbo.ActivityLogSearch", IsComposable = true)]
        public IQueryable< View.ActivityLogSearch > ActivityLogSearch(
            [Parameter(DbType="varchar")] string machine,
            [Parameter(DbType="varchar")] string activity,
            [Parameter(DbType="int")] int? userid,
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="int")] int? peopleid,
            [Parameter(DbType="datetime")] DateTime? enddate,
            [Parameter(DbType="int")] int? lookback,
            [Parameter(DbType="int")] int? pagesize,
            [Parameter(DbType="int")] int? pagenum
            )
        {
            return CreateMethodCallQuery< View.ActivityLogSearch>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    machine,
                    activity,
                    userid,
                    orgid,
                    peopleid,
                    enddate,
                    lookback,
                    pagesize,
                    pagenum
                );
        }

        [Function(Name="dbo.AttendanceCredits", IsComposable = true)]
        public IQueryable< View.AttendanceCredit > AttendanceCredits(
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="int")] int? pid
            )
        {
            return CreateMethodCallQuery< View.AttendanceCredit>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    pid
                );
        }

        [Function(Name="dbo.AttendanceTypeAsOf", IsComposable = true)]
        public IQueryable< View.AttendanceTypeAsOf > AttendanceTypeAsOf(
            [Parameter(DbType="datetime")] DateTime? from,
            [Parameter(DbType="datetime")] DateTime? to,
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="int")] int? orgtype,
            [Parameter(DbType="nvarchar")] string ids
            )
        {
            return CreateMethodCallQuery< View.AttendanceTypeAsOf>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    from,
                    to,
                    progid,
                    divid,
                    orgid,
                    orgtype,
                    ids
                );
        }

        [Function(Name="dbo.AttendCntHistory", IsComposable = true)]
        public IQueryable< View.AttendCntHistory > AttendCntHistory(
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? org,
            [Parameter(DbType="int")] int? sched,
            [Parameter(DbType="datetime")] DateTime? start,
            [Parameter(DbType="datetime")] DateTime? end
            )
        {
            return CreateMethodCallQuery< View.AttendCntHistory>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    progid,
                    divid,
                    org,
                    sched,
                    start,
                    end
                );
        }

        [Function(Name="dbo.AttendCommitments", IsComposable = true)]
        public IQueryable< View.AttendCommitment > AttendCommitments(
            [Parameter(DbType="int")] int? oid
            )
        {
            return CreateMethodCallQuery< View.AttendCommitment>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                );
        }

        [Function(Name="dbo.AttendDaysAfterNthVisitAsOf", IsComposable = true)]
        public IQueryable< View.AttendDaysAfterNthVisitAsOf > AttendDaysAfterNthVisitAsOf(
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? org,
            [Parameter(DbType="datetime")] DateTime? d1,
            [Parameter(DbType="datetime")] DateTime? d2,
            [Parameter(DbType="int")] int? n,
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.AttendDaysAfterNthVisitAsOf>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    progid,
                    divid,
                    org,
                    d1,
                    d2,
                    n,
                    days
                );
        }

        [Function(Name="dbo.AttendedAsOf", IsComposable = true)]
        public IQueryable< View.AttendedAsOf > AttendedAsOf(
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? org,
            [Parameter(DbType="datetime")] DateTime? dt1,
            [Parameter(DbType="datetime")] DateTime? dt2,
            [Parameter(DbType="bit")] bool? guestonly
            )
        {
            return CreateMethodCallQuery< View.AttendedAsOf>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    progid,
                    divid,
                    org,
                    dt1,
                    dt2,
                    guestonly
                );
        }

        [Function(Name="dbo.AttendMemberTypeAsOf", IsComposable = true)]
        public IQueryable< View.AttendMemberTypeAsOf > AttendMemberTypeAsOf(
            [Parameter(DbType="datetime")] DateTime? from,
            [Parameter(DbType="datetime")] DateTime? to,
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="nvarchar")] string ids,
            [Parameter(DbType="nvarchar")] string notids
            )
        {
            return CreateMethodCallQuery< View.AttendMemberTypeAsOf>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    from,
                    to,
                    progid,
                    divid,
                    orgid,
                    ids,
                    notids
                );
        }

        [Function(Name="dbo.CheckinByDate", IsComposable = true)]
        public IQueryable< View.CheckinByDate > CheckinByDate(
            [Parameter(DbType="datetime")] DateTime? dt
            )
        {
            return CreateMethodCallQuery< View.CheckinByDate>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    dt
                );
        }

        [Function(Name="dbo.CheckinFamilyMembers", IsComposable = true)]
        public IQueryable< View.CheckinFamilyMember > CheckinFamilyMembers(
            [Parameter(DbType="int")] int? familyid,
            [Parameter(DbType="int")] int? campus,
            [Parameter(DbType="int")] int? thisday
            )
        {
            return CreateMethodCallQuery< View.CheckinFamilyMember>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    familyid,
                    campus,
                    thisday
                );
        }

        [Function(Name="dbo.CheckinMatch", IsComposable = true)]
        public IQueryable< View.CheckinMatch > CheckinMatch(
            [Parameter(DbType="nvarchar")] string id
            )
        {
            return CreateMethodCallQuery< View.CheckinMatch>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    id
                );
        }

        [Function(Name="dbo.ConsecutiveAbsents", IsComposable = true)]
        public IQueryable< View.ConsecutiveAbsent > ConsecutiveAbsents(
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.ConsecutiveAbsent>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    divid,
                    days
                );
        }

        [Function(Name="dbo.ContactSummary", IsComposable = true)]
        public IQueryable< View.ContactSummary > ContactSummary(
            [Parameter(DbType="datetime")] DateTime? dt1,
            [Parameter(DbType="datetime")] DateTime? dt2,
            [Parameter(DbType="int")] int? min,
            [Parameter(DbType="int")] int? type,
            [Parameter(DbType="int")] int? reas
            )
        {
            return CreateMethodCallQuery< View.ContactSummary>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    dt1,
                    dt2,
                    min,
                    type,
                    reas
                );
        }

        [Function(Name="dbo.ContactTypeTotals", IsComposable = true)]
        public IQueryable< View.ContactTypeTotal > ContactTypeTotals(
            [Parameter(DbType="datetime")] DateTime? dt1,
            [Parameter(DbType="datetime")] DateTime? dt2,
            [Parameter(DbType="int")] int? min
            )
        {
            return CreateMethodCallQuery< View.ContactTypeTotal>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    dt1,
                    dt2,
                    min
                );
        }

        [Function(Name="dbo.ContributionCountTable", IsComposable = true)]
        public IQueryable< View.ContributionCountTable > ContributionCountTable(
            [Parameter(DbType="int")] int? days,
            [Parameter(DbType="int")] int? cnt,
            [Parameter(DbType="int")] int? fundid,
            [Parameter(DbType="nvarchar")] string op
            )
        {
            return CreateMethodCallQuery< View.ContributionCountTable>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    days,
                    cnt,
                    fundid,
                    op
                );
        }

        [Function(Name="dbo.Contributions0", IsComposable = true)]
        public IQueryable< View.Contributions0 > Contributions0(
            [Parameter(DbType="datetime")] DateTime? fd,
            [Parameter(DbType="datetime")] DateTime? td,
            [Parameter(DbType="int")] int? fundid,
            [Parameter(DbType="int")] int? campusid,
            [Parameter(DbType="bit")] bool? pledges,
            [Parameter(DbType="bit")] bool? nontaxded,
            [Parameter(DbType="bit")] bool? includeUnclosed
            )
        {
            return CreateMethodCallQuery< View.Contributions0>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fd,
                    td,
                    fundid,
                    campusid,
                    pledges,
                    nontaxded,
                    includeUnclosed
                );
        }

        [Function(Name="dbo.Contributions2", IsComposable = true)]
        public IQueryable< View.Contributions2 > Contributions2(
            [Parameter(DbType="datetime")] DateTime? fd,
            [Parameter(DbType="datetime")] DateTime? td,
            [Parameter(DbType="int")] int? campusid,
            [Parameter(DbType="bit")] bool? pledges,
            [Parameter(DbType="bit")] bool? nontaxded,
            [Parameter(DbType="bit")] bool? includeUnclosed
            )
        {
            return CreateMethodCallQuery< View.Contributions2>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fd,
                    td,
                    campusid,
                    pledges,
                    nontaxded,
                    includeUnclosed
                );
        }

        [Function(Name="dbo.ContributionSearch", IsComposable = true)]
        public IQueryable< View.ContributionSearch > ContributionSearch(
            [Parameter(DbType="nvarchar")] string name,
            [Parameter(DbType="nvarchar")] string comments,
            [Parameter(DbType="int")] int? bundletype,
            [Parameter(DbType="int")] int? type,
            [Parameter(DbType="int")] int? status,
            [Parameter(DbType="money")] decimal? minamt,
            [Parameter(DbType="money")] decimal? maxamt,
            [Parameter(DbType="datetime")] DateTime? startdate,
            [Parameter(DbType="datetime")] DateTime? enddate,
            [Parameter(DbType="varchar")] string taxnontax,
            [Parameter(DbType="int")] int? fundid,
            [Parameter(DbType="int")] int? campusid,
            [Parameter(DbType="int")] int? year,
            [Parameter(DbType="bit")] bool? includeunclosedbundles,
            [Parameter(DbType="bit")] bool? mobile,
            [Parameter(DbType="int")] int? online
            )
        {
            return CreateMethodCallQuery< View.ContributionSearch>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    name,
                    comments,
                    bundletype,
                    type,
                    status,
                    minamt,
                    maxamt,
                    startdate,
                    enddate,
                    taxnontax,
                    fundid,
                    campusid,
                    year,
                    includeunclosedbundles,
                    mobile,
                    online
                );
        }

        [Function(Name="dbo.Contributors", IsComposable = true)]
        public IQueryable< View.Contributor > Contributors(
            [Parameter(DbType="datetime")] DateTime? fd,
            [Parameter(DbType="datetime")] DateTime? td,
            [Parameter(DbType="int")] int? pid,
            [Parameter(DbType="int")] int? spid,
            [Parameter(DbType="int")] int? fid,
            [Parameter(DbType="bit")] bool? noaddrok,
            [Parameter(DbType="int")] int? tagid
            )
        {
            return CreateMethodCallQuery< View.Contributor>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fd,
                    td,
                    pid,
                    spid,
                    fid,
                    noaddrok,
                    tagid
                );
        }

        [Function(Name="dbo.CsvTable", IsComposable = true)]
        public IQueryable< View.CsvTable > CsvTable(
            [Parameter(DbType="nvarchar")] string csv
            )
        {
            return CreateMethodCallQuery< View.CsvTable>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    csv
                );
        }

        [Function(Name="dbo.CurrOrgMembers", IsComposable = true)]
        public IQueryable< View.CurrOrgMember > CurrOrgMembers(
            [Parameter(DbType="varchar")] string orgs
            )
        {
            return CreateMethodCallQuery< View.CurrOrgMember>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgs
                );
        }

        [Function(Name="dbo.CurrOrgMembers2", IsComposable = true)]
        public IQueryable< View.CurrOrgMembers2 > CurrOrgMembers2(
            [Parameter(DbType="varchar")] string orgs,
            [Parameter(DbType="varchar")] string pids
            )
        {
            return CreateMethodCallQuery< View.CurrOrgMembers2>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgs,
                    pids
                );
        }

        [Function(Name="dbo.DownlineCategories", IsComposable = true)]
        public IQueryable< View.DownlineCategory > DownlineCategories(
            [Parameter(DbType="int")] int? categoryId
            )
        {
            return CreateMethodCallQuery< View.DownlineCategory>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    categoryId
                );
        }

        [Function(Name="dbo.DownlineDetails", IsComposable = true)]
        public IQueryable< View.DownlineDetail > DownlineDetails(
            [Parameter(DbType="int")] int? categoryid,
            [Parameter(DbType="int")] int? leaderid,
            [Parameter(DbType="int")] int? level,
            [Parameter(DbType="int")] int? pagenum,
            [Parameter(DbType="int")] int? pagesize
            )
        {
            return CreateMethodCallQuery< View.DownlineDetail>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    categoryid,
                    leaderid,
                    level,
                    pagenum,
                    pagesize
                );
        }

        [Function(Name="dbo.DownlineLevels", IsComposable = true)]
        public IQueryable< View.DownlineLevel > DownlineLevels(
            [Parameter(DbType="int")] int? categoryid,
            [Parameter(DbType="int")] int? leaderid,
            [Parameter(DbType="int")] int? pagenum,
            [Parameter(DbType="int")] int? pagesize
            )
        {
            return CreateMethodCallQuery< View.DownlineLevel>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    categoryid,
                    leaderid,
                    pagenum,
                    pagesize
                );
        }

        [Function(Name="dbo.DownlineSingleTrace", IsComposable = true)]
        public IQueryable< View.DownlineSingleTrace > DownlineSingleTrace(
            [Parameter(DbType="int")] int? categoryid,
            [Parameter(DbType="int")] int? leaderid,
            [Parameter(DbType="varchar")] string trace
            )
        {
            return CreateMethodCallQuery< View.DownlineSingleTrace>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    categoryid,
                    leaderid,
                    trace
                );
        }

        [Function(Name="dbo.DownlineSummary", IsComposable = true)]
        public IQueryable< View.DownlineSummary > DownlineSummary(
            [Parameter(DbType="int")] int? categoryid,
            [Parameter(DbType="int")] int? pagenum,
            [Parameter(DbType="int")] int? pagesize
            )
        {
            return CreateMethodCallQuery< View.DownlineSummary>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    categoryid,
                    pagenum,
                    pagesize
                );
        }

        [Function(Name="dbo.FamilyMembers", IsComposable = true)]
        public IQueryable< View.FamilyMember > FamilyMembers(
            [Parameter(DbType="int")] int? pid
            )
        {
            return CreateMethodCallQuery< View.FamilyMember>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                );
        }

        [Function(Name="dbo.FilterOnlineReg", IsComposable = true)]
        public IQueryable< View.FilterOnlineReg > FilterOnlineReg(
            [Parameter(DbType="int")] int? onlinereg
            )
        {
            return CreateMethodCallQuery< View.FilterOnlineReg>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    onlinereg
                );
        }

        [Function(Name="dbo.FilterOrgSearchName", IsComposable = true)]
        public IQueryable< View.FilterOrgSearchName > FilterOrgSearchName(
            [Parameter(DbType="varchar")] string name
            )
        {
            return CreateMethodCallQuery< View.FilterOrgSearchName>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    name
                );
        }

        [Function(Name="dbo.FindPerson", IsComposable = true)]
        public IQueryable< View.FindPerson > FindPerson(
            [Parameter(DbType="nvarchar")] string first,
            [Parameter(DbType="nvarchar")] string last,
            [Parameter(DbType="datetime")] DateTime? dob,
            [Parameter(DbType="nvarchar")] string email,
            [Parameter(DbType="nvarchar")] string phone
            )
        {
            return CreateMethodCallQuery< View.FindPerson>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    first,
                    last,
                    dob,
                    email,
                    phone
                );
        }

        [Function(Name="dbo.FindPerson2", IsComposable = true)]
        public IQueryable< View.FindPerson2 > FindPerson2(
            [Parameter(DbType="nvarchar")] string first,
            [Parameter(DbType="nvarchar")] string goesby,
            [Parameter(DbType="nvarchar")] string last,
            [Parameter(DbType="int")] int? m,
            [Parameter(DbType="int")] int? d,
            [Parameter(DbType="int")] int? y,
            [Parameter(DbType="nvarchar")] string email,
            [Parameter(DbType="nvarchar")] string email2,
            [Parameter(DbType="nvarchar")] string phone1,
            [Parameter(DbType="nvarchar")] string phone2,
            [Parameter(DbType="nvarchar")] string phone3
            )
        {
            return CreateMethodCallQuery< View.FindPerson2>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    first,
                    goesby,
                    last,
                    m,
                    d,
                    y,
                    email,
                    email2,
                    phone1,
                    phone2,
                    phone3
                );
        }

        [Function(Name="dbo.FindPerson3", IsComposable = true)]
        public IQueryable< View.FindPerson3 > FindPerson3(
            [Parameter(DbType="nvarchar")] string first,
            [Parameter(DbType="nvarchar")] string last,
            [Parameter(DbType="datetime")] DateTime? dob,
            [Parameter(DbType="nvarchar")] string email,
            [Parameter(DbType="nvarchar")] string phone1,
            [Parameter(DbType="nvarchar")] string phone2,
            [Parameter(DbType="nvarchar")] string phone3
            )
        {
            return CreateMethodCallQuery< View.FindPerson3>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    first,
                    last,
                    dob,
                    email,
                    phone1,
                    phone2,
                    phone3
                );
        }

        [Function(Name="dbo.FindPerson4", IsComposable = true)]
        public IQueryable< View.FindPerson4 > FindPerson4(
            [Parameter(DbType="int")] int? PeopleId1
            )
        {
            return CreateMethodCallQuery< View.FindPerson4>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    PeopleId1
                );
        }

        [Function(Name="dbo.FirstTimeGivers", IsComposable = true)]
        public IQueryable< View.FirstTimeGiver > FirstTimeGivers(
            [Parameter(DbType="int")] int? days,
            [Parameter(DbType="int")] int? fundid
            )
        {
            return CreateMethodCallQuery< View.FirstTimeGiver>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    days,
                    fundid
                );
        }

        [Function(Name="dbo.GenRanges", IsComposable = true)]
        public IQueryable< View.GenRange > GenRanges(
            [Parameter(DbType="varchar")] string amts
            )
        {
            return CreateMethodCallQuery< View.GenRange>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    amts
                );
        }

        [Function(Name="dbo.GetContributions", IsComposable = true)]
        public IQueryable< View.GetContribution > GetContributions(
            [Parameter(DbType="int")] int? fid,
            [Parameter(DbType="bit")] bool? pledge
            )
        {
            return CreateMethodCallQuery< View.GetContribution>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fid,
                    pledge
                );
        }

        [Function(Name="dbo.GetContributionTotalsBothIfJoint", IsComposable = true)]
        public IQueryable< View.GetContributionTotalsBothIfJoint > GetContributionTotalsBothIfJoint(
            [Parameter(DbType="datetime")] DateTime? startdt,
            [Parameter(DbType="datetime")] DateTime? enddt
            )
        {
            return CreateMethodCallQuery< View.GetContributionTotalsBothIfJoint>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    startdt,
                    enddt
                );
        }

        [Function(Name="dbo.GetTodaysMeetingHours", IsComposable = true)]
        public IQueryable< View.GetTodaysMeetingHour > GetTodaysMeetingHours(
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="int")] int? thisday
            )
        {
            return CreateMethodCallQuery< View.GetTodaysMeetingHour>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    thisday
                );
        }

        [Function(Name="dbo.GetTodaysMeetingHours2", IsComposable = true)]
        public IQueryable< View.GetTodaysMeetingHours2 > GetTodaysMeetingHours2(
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="int")] int? thisday,
            [Parameter(DbType="bit")] bool? kioskmode
            )
        {
            return CreateMethodCallQuery< View.GetTodaysMeetingHours2>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    thisday,
                    kioskmode
                );
        }

        [Function(Name="dbo.GetTodaysMeetingHours3", IsComposable = true)]
        public IQueryable< View.GetTodaysMeetingHours3 > GetTodaysMeetingHours3(
            [Parameter(DbType="int")] int? thisday
            )
        {
            return CreateMethodCallQuery< View.GetTodaysMeetingHours3>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    thisday
                );
        }

        [Function(Name="dbo.GetTotalContributions", IsComposable = true)]
        public IQueryable< View.GetTotalContribution > GetTotalContributions(
            [Parameter(DbType="datetime")] DateTime? startdt,
            [Parameter(DbType="datetime")] DateTime? enddt
            )
        {
            return CreateMethodCallQuery< View.GetTotalContribution>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    startdt,
                    enddt
                );
        }

        [Function(Name="dbo.GetTotalContributions2", IsComposable = true)]
        public IQueryable< View.GetTotalContributions2 > GetTotalContributions2(
            [Parameter(DbType="datetime")] DateTime? fd,
            [Parameter(DbType="datetime")] DateTime? td,
            [Parameter(DbType="int")] int? campusid,
            [Parameter(DbType="bit")] bool? nontaxded,
            [Parameter(DbType="bit")] bool? includeUnclosed
            )
        {
            return CreateMethodCallQuery< View.GetTotalContributions2>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fd,
                    td,
                    campusid,
                    nontaxded,
                    includeUnclosed
                );
        }

        [Function(Name="dbo.GetTotalContributions3", IsComposable = true)]
        public IQueryable< View.GetTotalContributions3 > GetTotalContributions3(
            [Parameter(DbType="datetime")] DateTime? fd,
            [Parameter(DbType="datetime")] DateTime? td,
            [Parameter(DbType="int")] int? campusid,
            [Parameter(DbType="bit")] bool? nontaxded,
            [Parameter(DbType="bit")] bool? includeUnclosed
            )
        {
            return CreateMethodCallQuery< View.GetTotalContributions3>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fd,
                    td,
                    campusid,
                    nontaxded,
                    includeUnclosed
                );
        }

        [Function(Name="dbo.GetTotalContributionsAgeRange", IsComposable = true)]
        public IQueryable< View.GetTotalContributionsAgeRange > GetTotalContributionsAgeRange(
            [Parameter(DbType="datetime")] DateTime? fd,
            [Parameter(DbType="datetime")] DateTime? td,
            [Parameter(DbType="int")] int? campusid,
            [Parameter(DbType="bit")] bool? nontaxded,
            [Parameter(DbType="bit")] bool? includeUnclosed
            )
        {
            return CreateMethodCallQuery< View.GetTotalContributionsAgeRange>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fd,
                    td,
                    campusid,
                    nontaxded,
                    includeUnclosed
                );
        }

        [Function(Name="dbo.GetTotalContributionsDonor2", IsComposable = true)]
        public IQueryable< View.GetTotalContributionsDonor2 > GetTotalContributionsDonor2(
            [Parameter(DbType="datetime")] DateTime? fd,
            [Parameter(DbType="datetime")] DateTime? td,
            [Parameter(DbType="int")] int? campusid,
            [Parameter(DbType="bit")] bool? nontaxded,
            [Parameter(DbType="bit")] bool? includeUnclosed
            )
        {
            return CreateMethodCallQuery< View.GetTotalContributionsDonor2>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fd,
                    td,
                    campusid,
                    nontaxded,
                    includeUnclosed
                );
        }

        [Function(Name="dbo.GetTotalContributionsRange", IsComposable = true)]
        public IQueryable< View.GetTotalContributionsRange > GetTotalContributionsRange(
            [Parameter(DbType="datetime")] DateTime? fd,
            [Parameter(DbType="datetime")] DateTime? td,
            [Parameter(DbType="int")] int? campusid,
            [Parameter(DbType="bit")] bool? nontaxded,
            [Parameter(DbType="bit")] bool? includeUnclosed
            )
        {
            return CreateMethodCallQuery< View.GetTotalContributionsRange>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fd,
                    td,
                    campusid,
                    nontaxded,
                    includeUnclosed
                );
        }

        [Function(Name="dbo.GivingChange", IsComposable = true)]
        public IQueryable< View.GivingChange > GivingChange(
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.GivingChange>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    days
                );
        }

        [Function(Name="dbo.GivingCurrentPercentOfFormer", IsComposable = true)]
        public IQueryable< View.GivingCurrentPercentOfFormer > GivingCurrentPercentOfFormer(
            [Parameter(DbType="datetime")] DateTime? dt1,
            [Parameter(DbType="datetime")] DateTime? dt2,
            [Parameter(DbType="nvarchar")] string comp,
            [Parameter(DbType="float")] double? pct
            )
        {
            return CreateMethodCallQuery< View.GivingCurrentPercentOfFormer>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    dt1,
                    dt2,
                    comp,
                    pct
                );
        }

        [Function(Name="dbo.GuestList", IsComposable = true)]
        public IQueryable< View.GuestList > GuestList(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="datetime")] DateTime? since,
            [Parameter(DbType="bit")] bool? showHidden,
            [Parameter(DbType="varchar")] string first,
            [Parameter(DbType="varchar")] string last
            )
        {
            return CreateMethodCallQuery< View.GuestList>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    since,
                    showHidden,
                    first,
                    last
                );
        }

        [Function(Name="dbo.GuestList2", IsComposable = true)]
        public IQueryable< View.GuestList2 > GuestList2(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="datetime")] DateTime? since,
            [Parameter(DbType="bit")] bool? showHidden
            )
        {
            return CreateMethodCallQuery< View.GuestList2>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    since,
                    showHidden
                );
        }

        [Function(Name="dbo.HasIncompleteRegistrations", IsComposable = true)]
        public IQueryable< View.HasIncompleteRegistration > HasIncompleteRegistrations(
            [Parameter(DbType="int")] int? prog,
            [Parameter(DbType="int")] int? div,
            [Parameter(DbType="int")] int? org,
            [Parameter(DbType="datetime")] DateTime? begdt,
            [Parameter(DbType="datetime")] DateTime? enddt
            )
        {
            return CreateMethodCallQuery< View.HasIncompleteRegistration>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    prog,
                    div,
                    org,
                    begdt,
                    enddt
                );
        }

        [Function(Name="dbo.LastAttendOrg", IsComposable = true)]
        public IQueryable< View.LastAttendOrg > LastAttendOrg(
            [Parameter(DbType="int")] int? oid
            )
        {
            return CreateMethodCallQuery< View.LastAttendOrg>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                );
        }

        [Function(Name="dbo.LastMeetings", IsComposable = true)]
        public IQueryable< View.LastMeeting > LastMeetings(
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.LastMeeting>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    divid,
                    days
                );
        }

        [Function(Name="dbo.MeetingsDataForDateRange", IsComposable = true)]
        public IQueryable< View.MeetingsDataForDateRange > MeetingsDataForDateRange(
            [Parameter(DbType="varchar")] string orgs,
            [Parameter(DbType="datetime")] DateTime? startdate,
            [Parameter(DbType="datetime")] DateTime? enddate
            )
        {
            return CreateMethodCallQuery< View.MeetingsDataForDateRange>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgs,
                    startdate,
                    enddate
                );
        }

        [Function(Name="dbo.MembersAsOf", IsComposable = true)]
        public IQueryable< View.MembersAsOf > MembersAsOf(
            [Parameter(DbType="datetime")] DateTime? from,
            [Parameter(DbType="datetime")] DateTime? to,
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? orgid
            )
        {
            return CreateMethodCallQuery< View.MembersAsOf>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    from,
                    to,
                    progid,
                    divid,
                    orgid
                );
        }

        [Function(Name="dbo.MembersWhoAttendedOrgs", IsComposable = true)]
        public IQueryable< View.MembersWhoAttendedOrg > MembersWhoAttendedOrgs(
            [Parameter(DbType="varchar")] string orgs,
            [Parameter(DbType="datetime")] DateTime? firstdate
            )
        {
            return CreateMethodCallQuery< View.MembersWhoAttendedOrg>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgs,
                    firstdate
                );
        }

        [Function(Name="dbo.MostRecentItems", IsComposable = true)]
        public IQueryable< View.MostRecentItem > MostRecentItems(
            [Parameter(DbType="int")] int? uid
            )
        {
            return CreateMethodCallQuery< View.MostRecentItem>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    uid
                );
        }

        [Function(Name="dbo.NotAttendedAsOf", IsComposable = true)]
        public IQueryable< View.NotAttendedAsOf > NotAttendedAsOf(
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? org,
            [Parameter(DbType="datetime")] DateTime? dt1,
            [Parameter(DbType="datetime")] DateTime? dt2,
            [Parameter(DbType="bit")] bool? guestonly
            )
        {
            return CreateMethodCallQuery< View.NotAttendedAsOf>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    progid,
                    divid,
                    org,
                    dt1,
                    dt2,
                    guestonly
                );
        }

        [Function(Name="dbo.OrgMember", IsComposable = true)]
        public IQueryable< View.OrgMember > OrgMember(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="varchar")] string grouptype,
            [Parameter(DbType="varchar")] string first,
            [Parameter(DbType="varchar")] string last,
            [Parameter(DbType="varchar")] string sgfilter,
            [Parameter(DbType="bit")] bool? showhidden
            )
        {
            return CreateMethodCallQuery< View.OrgMember>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    grouptype,
                    first,
                    last,
                    sgfilter,
                    showhidden
                );
        }

        [Function(Name="dbo.OrgMemberInfo", IsComposable = true)]
        public IQueryable< View.OrgMemberInfo > OrgMemberInfo(
            [Parameter(DbType="int")] int? oid
            )
        {
            return CreateMethodCallQuery< View.OrgMemberInfo>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                );
        }

        [Function(Name="dbo.OrgMemberQuestions", IsComposable = true)]
        public IQueryable< View.OrgMemberQuestion > OrgMemberQuestions(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="int")] int? pid
            )
        {
            return CreateMethodCallQuery< View.OrgMemberQuestion>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    pid
                );
        }

        [Function(Name="dbo.OrgMembersAsOfDate", IsComposable = true)]
        public IQueryable< View.OrgMembersAsOfDate > OrgMembersAsOfDate(
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="datetime")] DateTime? meetingdt
            )
        {
            return CreateMethodCallQuery< View.OrgMembersAsOfDate>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    meetingdt
                );
        }

        [Function(Name="dbo.OrgMinistryInfo", IsComposable = true)]
        public IQueryable< View.OrgMinistryInfo > OrgMinistryInfo(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="varchar")] string grouptype,
            [Parameter(DbType="varchar")] string first,
            [Parameter(DbType="varchar")] string last,
            [Parameter(DbType="varchar")] string sgfilter,
            [Parameter(DbType="bit")] bool? showhidden
            )
        {
            return CreateMethodCallQuery< View.OrgMinistryInfo>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    grouptype,
                    first,
                    last,
                    sgfilter,
                    showhidden
                );
        }

        [Function(Name="dbo.OrgPeople", IsComposable = true)]
        public IQueryable< View.OrgPerson > OrgPeople(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="varchar")] string grouptype,
            [Parameter(DbType="varchar")] string first,
            [Parameter(DbType="varchar")] string last,
            [Parameter(DbType="varchar")] string sgfilter,
            [Parameter(DbType="bit")] bool? showhidden,
            [Parameter(DbType="nvarchar")] string currtag,
            [Parameter(DbType="int")] int? currtagowner,
            [Parameter(DbType="bit")] bool? filterchecked,
            [Parameter(DbType="bit")] bool? filtertag,
            [Parameter(DbType="bit")] bool? ministryinfo,
            [Parameter(DbType="int")] int? userpeopleid
            )
        {
            return CreateMethodCallQuery< View.OrgPerson>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    grouptype,
                    first,
                    last,
                    sgfilter,
                    showhidden,
                    currtag,
                    currtagowner,
                    filterchecked,
                    filtertag,
                    ministryinfo,
                    userpeopleid
                );
        }

        [Function(Name="dbo.OrgPeople2", IsComposable = true)]
        public IQueryable< View.OrgPeople2 > OrgPeople2(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="varchar")] string grouptype,
            [Parameter(DbType="varchar")] string first,
            [Parameter(DbType="varchar")] string last,
            [Parameter(DbType="varchar")] string sgfilter,
            [Parameter(DbType="bit")] bool? showhidden,
            [Parameter(DbType="nvarchar")] string currtag,
            [Parameter(DbType="int")] int? currtagowner,
            [Parameter(DbType="bit")] bool? filterchecked,
            [Parameter(DbType="bit")] bool? filtertag,
            [Parameter(DbType="bit")] bool? ministryinfo,
            [Parameter(DbType="int")] int? userpeopleid
            )
        {
            return CreateMethodCallQuery< View.OrgPeople2>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    grouptype,
                    first,
                    last,
                    sgfilter,
                    showhidden,
                    currtag,
                    currtagowner,
                    filterchecked,
                    filtertag,
                    ministryinfo,
                    userpeopleid
                );
        }

        [Function(Name="dbo.OrgPeopleCurrent", IsComposable = true)]
        public IQueryable< View.OrgPeopleCurrent > OrgPeopleCurrent(
            [Parameter(DbType="int")] int? oid
            )
        {
            return CreateMethodCallQuery< View.OrgPeopleCurrent>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                );
        }

        [Function(Name="dbo.OrgPeopleGuests", IsComposable = true)]
        public IQueryable< View.OrgPeopleGuest > OrgPeopleGuests(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="bit")] bool? showhidden
            )
        {
            return CreateMethodCallQuery< View.OrgPeopleGuest>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    showhidden
                );
        }

        [Function(Name="dbo.OrgPeopleIds", IsComposable = true)]
        public IQueryable< View.OrgPeopleId > OrgPeopleIds(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="varchar")] string grouptype,
            [Parameter(DbType="varchar")] string first,
            [Parameter(DbType="varchar")] string last,
            [Parameter(DbType="varchar")] string sgfilter,
            [Parameter(DbType="bit")] bool? showhidden,
            [Parameter(DbType="nvarchar")] string currtag,
            [Parameter(DbType="int")] int? currtagowner,
            [Parameter(DbType="bit")] bool? filterchecked,
            [Parameter(DbType="bit")] bool? filtertag,
            [Parameter(DbType="bit")] bool? ministryinfo,
            [Parameter(DbType="int")] int? userpeopleid
            )
        {
            return CreateMethodCallQuery< View.OrgPeopleId>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    grouptype,
                    first,
                    last,
                    sgfilter,
                    showhidden,
                    currtag,
                    currtagowner,
                    filterchecked,
                    filtertag,
                    ministryinfo,
                    userpeopleid
                );
        }

        [Function(Name="dbo.OrgPeopleInactive", IsComposable = true)]
        public IQueryable< View.OrgPeopleInactive > OrgPeopleInactive(
            [Parameter(DbType="int")] int? oid
            )
        {
            return CreateMethodCallQuery< View.OrgPeopleInactive>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                );
        }

        [Function(Name="dbo.OrgPeoplePending", IsComposable = true)]
        public IQueryable< View.OrgPeoplePending > OrgPeoplePending(
            [Parameter(DbType="int")] int? oid
            )
        {
            return CreateMethodCallQuery< View.OrgPeoplePending>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                );
        }

        [Function(Name="dbo.OrgPeoplePrevious", IsComposable = true)]
        public IQueryable< View.OrgPeoplePreviou > OrgPeoplePrevious(
            [Parameter(DbType="int")] int? oid
            )
        {
            return CreateMethodCallQuery< View.OrgPeoplePreviou>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                );
        }

        [Function(Name="dbo.OrgPeopleProspects", IsComposable = true)]
        public IQueryable< View.OrgPeopleProspect > OrgPeopleProspects(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="bit")] bool? showhidden
            )
        {
            return CreateMethodCallQuery< View.OrgPeopleProspect>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    showhidden
                );
        }

        [Function(Name="dbo.OrgSearch", IsComposable = true)]
        public IQueryable< View.OrgSearch > OrgSearch(
            [Parameter(DbType="nvarchar")] string name,
            [Parameter(DbType="int")] int? prog,
            [Parameter(DbType="int")] int? div,
            [Parameter(DbType="int")] int? type,
            [Parameter(DbType="int")] int? campus,
            [Parameter(DbType="int")] int? sched,
            [Parameter(DbType="int")] int? status,
            [Parameter(DbType="int")] int? onlinereg,
            [Parameter(DbType="int")] int? UserId,
            [Parameter(DbType="int")] int? targetDiv
            )
        {
            return CreateMethodCallQuery< View.OrgSearch>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    name,
                    prog,
                    div,
                    type,
                    campus,
                    sched,
                    status,
                    onlinereg,
                    UserId,
                    targetDiv
                );
        }

        [Function(Name="dbo.OrgVisitorsAsOfDate", IsComposable = true)]
        public IQueryable< View.OrgVisitorsAsOfDate > OrgVisitorsAsOfDate(
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="datetime")] DateTime? meetingdt,
            [Parameter(DbType="bit")] bool? NoCurrentMembers
            )
        {
            return CreateMethodCallQuery< View.OrgVisitorsAsOfDate>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    meetingdt,
                    NoCurrentMembers
                );
        }

        [Function(Name="dbo.PeopleIdsFromOrgSearch", IsComposable = true)]
        public IQueryable< View.PeopleIdsFromOrgSearch > PeopleIdsFromOrgSearch(
            [Parameter(DbType="nvarchar")] string name,
            [Parameter(DbType="int")] int? prog,
            [Parameter(DbType="int")] int? div,
            [Parameter(DbType="int")] int? type,
            [Parameter(DbType="int")] int? campus,
            [Parameter(DbType="int")] int? sched,
            [Parameter(DbType="int")] int? status,
            [Parameter(DbType="int")] int? onlinereg,
            [Parameter(DbType="bit")] bool? mainfellowship,
            [Parameter(DbType="bit")] bool? parentorg
            )
        {
            return CreateMethodCallQuery< View.PeopleIdsFromOrgSearch>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    name,
                    prog,
                    div,
                    type,
                    campus,
                    sched,
                    status,
                    onlinereg,
                    mainfellowship,
                    parentorg
                );
        }

        [Function(Name="dbo.PersonStatusFlags", IsComposable = true)]
        public IQueryable< View.PersonStatusFlag > PersonStatusFlags(
            [Parameter(DbType="int")] int? tagid
            )
        {
            return CreateMethodCallQuery< View.PersonStatusFlag>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    tagid
                );
        }

        [Function(Name="dbo.PledgeFulfillment", IsComposable = true)]
        public IQueryable< View.PledgeFulfillment > PledgeFulfillment(
            [Parameter(DbType="int")] int? fundid
            )
        {
            return CreateMethodCallQuery< View.PledgeFulfillment>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fundid
                );
        }

        [Function(Name="dbo.PledgeReport", IsComposable = true)]
        public IQueryable< View.PledgeReport > PledgeReport(
            [Parameter(DbType="datetime")] DateTime? fd,
            [Parameter(DbType="datetime")] DateTime? td,
            [Parameter(DbType="int")] int? campusid
            )
        {
            return CreateMethodCallQuery< View.PledgeReport>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fd,
                    td,
                    campusid
                );
        }

        [Function(Name="dbo.Pledges0", IsComposable = true)]
        public IQueryable< View.Pledges0 > Pledges0(
            [Parameter(DbType="datetime")] DateTime? fd,
            [Parameter(DbType="datetime")] DateTime? td,
            [Parameter(DbType="int")] int? fundid,
            [Parameter(DbType="int")] int? campusid
            )
        {
            return CreateMethodCallQuery< View.Pledges0>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fd,
                    td,
                    fundid,
                    campusid
                );
        }

        [Function(Name="dbo.PotentialSubstitutes", IsComposable = true)]
        public IQueryable< View.PotentialSubstitute > PotentialSubstitutes(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="int")] int? mid
            )
        {
            return CreateMethodCallQuery< View.PotentialSubstitute>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    mid
                );
        }

        [Function(Name="dbo.RecentAbsents", IsComposable = true)]
        public IQueryable< View.RecentAbsent > RecentAbsents(
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.RecentAbsent>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    divid,
                    days
                );
        }

        [Function(Name="dbo.RecentAbsents2", IsComposable = true)]
        public IQueryable< View.RecentAbsents2 > RecentAbsents2(
            [Parameter(DbType="int")] int? orgid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.RecentAbsents2>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    divid,
                    days
                );
        }

        [Function(Name="dbo.RecentAttendance", IsComposable = true)]
        public IQueryable< View.RecentAttendance > RecentAttendance(
            [Parameter(DbType="int")] int? oid
            )
        {
            return CreateMethodCallQuery< View.RecentAttendance>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                );
        }

        [Function(Name="dbo.RecentAttendInDaysByCount", IsComposable = true)]
        public IQueryable< View.RecentAttendInDaysByCount > RecentAttendInDaysByCount(
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? org,
            [Parameter(DbType="int")] int? orgtype,
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.RecentAttendInDaysByCount>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    progid,
                    divid,
                    org,
                    orgtype,
                    days
                );
        }

        [Function(Name="dbo.RecentAttendInDaysByCountDesc", IsComposable = true)]
        public IQueryable< View.RecentAttendInDaysByCountDesc > RecentAttendInDaysByCountDesc(
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? org,
            [Parameter(DbType="int")] int? orgtype,
            [Parameter(DbType="int")] int? days,
            [Parameter(DbType="varchar")] string desc
            )
        {
            return CreateMethodCallQuery< View.RecentAttendInDaysByCountDesc>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    progid,
                    divid,
                    org,
                    orgtype,
                    days,
                    desc
                );
        }

        [Function(Name="dbo.RecentAttendMemberType", IsComposable = true)]
        public IQueryable< View.RecentAttendMemberType > RecentAttendMemberType(
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? org,
            [Parameter(DbType="int")] int? days,
            [Parameter(DbType="varchar")] string idstring
            )
        {
            return CreateMethodCallQuery< View.RecentAttendMemberType>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    progid,
                    divid,
                    org,
                    days,
                    idstring
                );
        }

        [Function(Name="dbo.RecentAttendType", IsComposable = true)]
        public IQueryable< View.RecentAttendType > RecentAttendType(
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? org,
            [Parameter(DbType="int")] int? days,
            [Parameter(DbType="varchar")] string idstring
            )
        {
            return CreateMethodCallQuery< View.RecentAttendType>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    progid,
                    divid,
                    org,
                    days,
                    idstring
                );
        }

        [Function(Name="dbo.RecentGiver", IsComposable = true)]
        public IQueryable< View.RecentGiver > RecentGiver(
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.RecentGiver>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    days
                );
        }

        [Function(Name="dbo.RecentIncompleteRegistrations", IsComposable = true)]
        public IQueryable< View.RecentIncompleteRegistration > RecentIncompleteRegistrations(
            [Parameter(DbType="int")] int? prog,
            [Parameter(DbType="int")] int? div,
            [Parameter(DbType="int")] int? org,
            [Parameter(DbType="datetime")] DateTime? begdt,
            [Parameter(DbType="datetime")] DateTime? enddt
            )
        {
            return CreateMethodCallQuery< View.RecentIncompleteRegistration>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    prog,
                    div,
                    org,
                    begdt,
                    enddt
                );
        }

        [Function(Name="dbo.RecentIncompleteRegistrations2", IsComposable = true)]
        public IQueryable< View.RecentIncompleteRegistrations2 > RecentIncompleteRegistrations2(
            [Parameter(DbType="varchar")] string orgs,
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.RecentIncompleteRegistrations2>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgs,
                    days
                );
        }

        [Function(Name="dbo.RecentNewVisitCount", IsComposable = true)]
        public IQueryable< View.RecentNewVisitCount > RecentNewVisitCount(
            [Parameter(DbType="int")] int? progid,
            [Parameter(DbType="int")] int? divid,
            [Parameter(DbType="int")] int? org,
            [Parameter(DbType="int")] int? orgtype,
            [Parameter(DbType="int")] int? days0,
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.RecentNewVisitCount>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    progid,
                    divid,
                    org,
                    orgtype,
                    days0,
                    days
                );
        }

        [Function(Name="dbo.RecentRegistrations", IsComposable = true)]
        public IQueryable< View.RecentRegistration > RecentRegistrations(
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.RecentRegistration>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    days
                );
        }

        [Function(Name="dbo.RegisterLinksFromMaster", IsComposable = true)]
        public IQueryable< View.RegisterLinksFromMaster > RegisterLinksFromMaster(
            [Parameter(DbType="int")] int? master
            )
        {
            return CreateMethodCallQuery< View.RegisterLinksFromMaster>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    master
                );
        }

        [Function(Name="dbo.Registrations", IsComposable = true)]
        public IQueryable< View.Registration > Registrations(
            [Parameter(DbType="int")] int? days
            )
        {
            return CreateMethodCallQuery< View.Registration>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    days
                );
        }

        [Function(Name="dbo.RollList", IsComposable = true)]
        public IQueryable< View.RollList > RollList(
            [Parameter(DbType="int")] int? mid,
            [Parameter(DbType="datetime")] DateTime? meetingdt,
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="bit")] bool? current,
            [Parameter(DbType="bit")] bool? FromMobile
            )
        {
            return CreateMethodCallQuery< View.RollList>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    mid,
                    meetingdt,
                    oid,
                    current,
                    FromMobile
                );
        }

        [Function(Name="dbo.RollListHighlight", IsComposable = true)]
        public IQueryable< View.RollListHighlight > RollListHighlight(
            [Parameter(DbType="int")] int? mid,
            [Parameter(DbType="datetime")] DateTime? meetingdt,
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="bit")] bool? current,
            [Parameter(DbType="varchar")] string highlight
            )
        {
            return CreateMethodCallQuery< View.RollListHighlight>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    mid,
                    meetingdt,
                    oid,
                    current,
                    highlight
                );
        }

        [Function(Name="dbo.SearchDivisions", IsComposable = true)]
        public IQueryable< View.SearchDivision > SearchDivisions(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="varchar")] string name
            )
        {
            return CreateMethodCallQuery< View.SearchDivision>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    name
                );
        }

        [Function(Name="dbo.SenderGifts", IsComposable = true)]
        public IQueryable< View.SenderGift > SenderGifts(
            [Parameter(DbType="varchar")] string oids
            )
        {
            return CreateMethodCallQuery< View.SenderGift>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oids
                );
        }

        [Function(Name="dbo.Split", IsComposable = true)]
        public IQueryable< View.Split > Split(
            [Parameter(DbType="nvarchar")] string InputText,
            [Parameter(DbType="nvarchar")] string Delimiter
            )
        {
            return CreateMethodCallQuery< View.Split>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    InputText,
                    Delimiter
                );
        }

        [Function(Name="dbo.SplitInts", IsComposable = true)]
        public IQueryable< View.SplitInt > SplitInts(
            [Parameter(DbType="varchar")] string List
            )
        {
            return CreateMethodCallQuery< View.SplitInt>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    List
                );
        }

        [Function(Name="dbo.StatusFlags", IsComposable = true)]
        public IQueryable< View.StatusFlag > StatusFlags(
            [Parameter(DbType="nvarchar")] string flags
            )
        {
            return CreateMethodCallQuery< View.StatusFlag>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    flags
                );
        }

        [Function(Name="dbo.StatusFlagsPerson", IsComposable = true)]
        public IQueryable< View.StatusFlagsPerson > StatusFlagsPerson(
            [Parameter(DbType="int")] int? pid
            )
        {
            return CreateMethodCallQuery< View.StatusFlagsPerson>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                );
        }

        [Function(Name="dbo.SundayDates", IsComposable = true)]
        public IQueryable< View.SundayDate > SundayDates(
            [Parameter(DbType="datetime")] DateTime? dt1,
            [Parameter(DbType="datetime")] DateTime? dt2
            )
        {
            return CreateMethodCallQuery< View.SundayDate>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    dt1,
                    dt2
                );
        }

        [Function(Name="dbo.TaggedPeople", IsComposable = true)]
        public IQueryable< View.TaggedPerson > TaggedPeople(
            [Parameter(DbType="int")] int? tagid
            )
        {
            return CreateMethodCallQuery< View.TaggedPerson>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    tagid
                );
        }

        [Function(Name="dbo.TPStats", IsComposable = true)]
        public IQueryable< View.TPStat > TPStats(
            [Parameter(DbType="varchar")] string emails
            )
        {
            return CreateMethodCallQuery< View.TPStat>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    emails
                );
        }

        [Function(Name="dbo.VisitNumberSinceDate", IsComposable = true)]
        public IQueryable< View.VisitNumberSinceDate > VisitNumberSinceDate(
            [Parameter(DbType="datetime")] DateTime? dt,
            [Parameter(DbType="int")] int? n
            )
        {
            return CreateMethodCallQuery< View.VisitNumberSinceDate>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    dt,
                    n
                );
        }

        [Function(Name="dbo.VisitsAbsents", IsComposable = true)]
        public IQueryable< View.VisitsAbsent > VisitsAbsents(
            [Parameter(DbType="int")] int? meetingid
            )
        {
            return CreateMethodCallQuery< View.VisitsAbsent>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    meetingid
                );
        }

        [Function(Name="dbo.VolunteerCalendar", IsComposable = true)]
        public IQueryable< View.VolunteerCalendar > VolunteerCalendar(
            [Parameter(DbType="int")] int? oid,
            [Parameter(DbType="nvarchar")] string sg1,
            [Parameter(DbType="nvarchar")] string sg2
            )
        {
            return CreateMethodCallQuery< View.VolunteerCalendar>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    sg1,
                    sg2
                );
        }

        [Function(Name="dbo.WeeklyAttendsForOrgs", IsComposable = true)]
        public IQueryable< View.WeeklyAttendsForOrg > WeeklyAttendsForOrgs(
            [Parameter(DbType="varchar")] string orgs,
            [Parameter(DbType="datetime")] DateTime? firstdate
            )
        {
            return CreateMethodCallQuery< View.WeeklyAttendsForOrg>(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgs,
                    firstdate
                );
        }

#endregion
#region Scalar Functions
        
        [Function(Name="dbo.DonorTotalUnitsSize", IsComposable = true)]
        [return: Parameter(DbType = "money")]
        public decimal? DonorTotalUnitsSize(
            [Parameter(Name = "t", DbType="table type")] string t,
            [Parameter(Name = "min", DbType="int")] int? min,
            [Parameter(Name = "max", DbType="int")] int? max
            )
        {
            return ((decimal?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    t,
                    min,
                    max
                ).ReturnValue));
        }

        [Function(Name="dbo.AttendItem", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? AttendItem(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "n", DbType="int")] int? n
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    n
                ).ReturnValue));
        }

        [Function(Name="dbo.ContributionChange", IsComposable = true)]
        [return: Parameter(DbType = "float")]
        public double? ContributionChange(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "dt1", DbType="datetime")] DateTime? dt1,
            [Parameter(Name = "dt2", DbType="datetime")] DateTime? dt2
            )
        {
            return ((double?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    dt1,
                    dt2
                ).ReturnValue));
        }

        [Function(Name="dbo.DonorTotalGiftsAttrRange", IsComposable = true)]
        [return: Parameter(DbType = "money")]
        public decimal? DonorTotalGiftsAttrRange(
            [Parameter(Name = "t", DbType="table type")] string t,
            [Parameter(Name = "min", DbType="int")] int? min,
            [Parameter(Name = "max", DbType="int")] int? max
            )
        {
            return ((decimal?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    t,
                    min,
                    max
                ).ReturnValue));
        }

        [Function(Name="dbo.LastActive", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? LastActive(
            [Parameter(Name = "uid", DbType="int")] int? uid
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    uid
                ).ReturnValue));
        }

        [Function(Name="dbo.ContributionAmount2", IsComposable = true)]
        [return: Parameter(DbType = "money")]
        public decimal? ContributionAmount2(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "dt1", DbType="datetime")] DateTime? dt1,
            [Parameter(Name = "dt2", DbType="datetime")] DateTime? dt2,
            [Parameter(Name = "fundid", DbType="int")] int? fundid
            )
        {
            return ((decimal?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    dt1,
                    dt2,
                    fundid
                ).ReturnValue));
        }

        [Function(Name="dbo.DonorTotalUnitsAttrRange", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? DonorTotalUnitsAttrRange(
            [Parameter(Name = "t", DbType="table type")] string t,
            [Parameter(Name = "min", DbType="int")] int? min,
            [Parameter(Name = "max", DbType="int")] int? max
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    t,
                    min,
                    max
                ).ReturnValue));
        }

        [Function(Name="dbo.FamilyMakeup", IsComposable = true)]
        [return: Parameter(DbType = "varchar")]
        public string FamilyMakeup(
            [Parameter(Name = "fid", DbType="int")] int? fid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fid
                ).ReturnValue));
        }

        [Function(Name="dbo.AddressMatch", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? AddressMatch(
            [Parameter(Name = "var1", DbType="nvarchar")] string var1,
            [Parameter(Name = "var2", DbType="nvarchar")] string var2
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    var1,
                    var2
                ).ReturnValue));
        }

        [Function(Name="dbo.AvgSunAttendance", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? AvgSunAttendance(
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod()))
                ).ReturnValue));
        }

        [Function(Name="dbo.GetAttendedTodaysMeeting", IsComposable = true)]
        [return: Parameter(DbType = "bit")]
        public bool? GetAttendedTodaysMeeting(
            [Parameter(Name = "orgid", DbType="int")] int? orgid,
            [Parameter(Name = "thisday", DbType="int")] int? thisday,
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((bool?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    thisday,
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.ComputePositionInFamily", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? ComputePositionInFamily(
            [Parameter(Name = "age", DbType="int")] int? age,
            [Parameter(Name = "married", DbType="bit")] bool? married,
            [Parameter(Name = "fid", DbType="int")] int? fid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    age,
                    married,
                    fid
                ).ReturnValue));
        }

        [Function(Name="dbo.GetTodaysMeetingHour", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? GetTodaysMeetingHour(
            [Parameter(Name = "thisday", DbType="int")] int? thisday,
            [Parameter(Name = "MeetingTime", DbType="datetime")] DateTime? MeetingTime,
            [Parameter(Name = "SchedDay", DbType="int")] int? SchedDay
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    thisday,
                    MeetingTime,
                    SchedDay
                ).ReturnValue));
        }

        [Function(Name="dbo.GetTodaysMeetingId", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? GetTodaysMeetingId(
            [Parameter(Name = "orgid", DbType="int")] int? orgid,
            [Parameter(Name = "thisday", DbType="int")] int? thisday
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    thisday
                ).ReturnValue));
        }

        [Function(Name="dbo.SpouseIdJoint", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? SpouseIdJoint(
            [Parameter(Name = "peopleid", DbType="int")] int? peopleid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    peopleid
                ).ReturnValue));
        }

        [Function(Name="dbo.AttendDesc", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string AttendDesc(
            [Parameter(Name = "id", DbType="int")] int? id
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    id
                ).ReturnValue));
        }

        [Function(Name="dbo.MemberDesc", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string MemberDesc(
            [Parameter(Name = "id", DbType="int")] int? id
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    id
                ).ReturnValue));
        }

        [Function(Name="dbo.fn_diagramobjects", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? FnDiagramobjects(
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod()))
                ).ReturnValue));
        }

        [Function(Name="dbo.PrimaryCountry", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string PrimaryCountry(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.WasDeaconActive2008", IsComposable = true)]
        [return: Parameter(DbType = "bit")]
        public bool? WasDeaconActive2008(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "dt", DbType="datetime")] DateTime? dt
            )
        {
            return ((bool?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    dt
                ).ReturnValue));
        }

        [Function(Name="dbo.LastAttend", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? LastAttend(
            [Parameter(Name = "orgid", DbType="int")] int? orgid,
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.LastDrop", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? LastDrop(
            [Parameter(Name = "orgid", DbType="int")] int? orgid,
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.LastAttended", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? LastAttended(
            [Parameter(Name = "orgid", DbType="int")] int? orgid,
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid,
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.BaptismAgeRange", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string BaptismAgeRange(
            [Parameter(Name = "age", DbType="int")] int? age
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    age
                ).ReturnValue));
        }

        [Function(Name="dbo.DaysSinceAttend", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? DaysSinceAttend(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "oid", DbType="int")] int? oid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    oid
                ).ReturnValue));
        }

        [Function(Name="dbo.SundayForWeek", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? SundayForWeek(
            [Parameter(Name = "year", DbType="int")] int? year,
            [Parameter(Name = "week", DbType="int")] int? week
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    year,
                    week
                ).ReturnValue));
        }

        [Function(Name="dbo.ScheduleId", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? ScheduleId(
            [Parameter(Name = "day", DbType="int")] int? day,
            [Parameter(Name = "time", DbType="datetime")] DateTime? time
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    day,
                    time
                ).ReturnValue));
        }

        [Function(Name="dbo.GetScheduleTime", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? GetScheduleTime(
            [Parameter(Name = "day", DbType="int")] int? day,
            [Parameter(Name = "time", DbType="datetime")] DateTime? time
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    day,
                    time
                ).ReturnValue));
        }

        [Function(Name="dbo.OrganizationMemberCount", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? OrganizationMemberCount(
            [Parameter(Name = "oid", DbType="int")] int? oid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                ).ReturnValue));
        }

        [Function(Name="dbo.PersonAttendCountOrg", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? PersonAttendCountOrg(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "oid", DbType="int")] int? oid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    oid
                ).ReturnValue));
        }

        [Function(Name="dbo.PrimaryAddress2", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string PrimaryAddress2(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.GetEldestFamilyMember", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? GetEldestFamilyMember(
            [Parameter(Name = "fid", DbType="int")] int? fid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fid
                ).ReturnValue));
        }

        [Function(Name="dbo.Birthday", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? Birthday(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.HeadOfHouseholdId", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? HeadOfHouseholdId(
            [Parameter(Name = "familyid", DbType="int")] int? familyid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    familyid
                ).ReturnValue));
        }

        [Function(Name="dbo.DOB", IsComposable = true)]
        [return: Parameter(DbType = "varchar")]
        public string Dob(
            [Parameter(Name = "m", DbType="int")] int? m,
            [Parameter(Name = "d", DbType="int")] int? d,
            [Parameter(Name = "y", DbType="int")] int? y
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    m,
                    d,
                    y
                ).ReturnValue));
        }

        [Function(Name="dbo.HeadOfHouseHoldSpouseId", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? HeadOfHouseHoldSpouseId(
            [Parameter(Name = "familyid", DbType="int")] int? familyid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    familyid
                ).ReturnValue));
        }

        [Function(Name="dbo.CoupleFlag", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? CoupleFlag(
            [Parameter(Name = "familyid", DbType="int")] int? familyid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    familyid
                ).ReturnValue));
        }

        [Function(Name="dbo.NextChangeTransactionId2", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? NextChangeTransactionId2(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "tid", DbType="int")] int? tid,
            [Parameter(Name = "typeid", DbType="int")] int? typeid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    oid,
                    tid,
                    typeid
                ).ReturnValue));
        }

        [Function(Name="dbo.PrimaryCity", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string PrimaryCity(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.PrimaryZip", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string PrimaryZip(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.InSmallGroup", IsComposable = true)]
        [return: Parameter(DbType = "varchar")]
        public string InSmallGroup(
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "sg", DbType="varchar")] string sg
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    pid,
                    sg
                ).ReturnValue));
        }

        [Function(Name="dbo.BibleFellowshipClassId", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? BibleFellowshipClassId(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.SpouseId", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? SpouseId(
            [Parameter(Name = "peopleid", DbType="int")] int? peopleid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    peopleid
                ).ReturnValue));
        }

        [Function(Name="dbo.OrgCheckedCount", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? OrgCheckedCount(
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "groupselect", DbType="varchar")] string groupselect,
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    groupselect,
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.Age", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? Age(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.PrimaryResCode", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? PrimaryResCode(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.GetCurrentOnlineBundle", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? GetCurrentOnlineBundle(
            [Parameter(Name = "next", DbType="datetime")] DateTime? next,
            [Parameter(Name = "prev", DbType="datetime")] DateTime? prev
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    next,
                    prev
                ).ReturnValue));
        }

        [Function(Name="dbo.EntryPointId", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? EntryPointId(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.Tool_VarbinaryToVarcharHex", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string ToolVarbinaryToVarcharHex(
            [Parameter(Name = "VarbinaryValue", DbType="varbinary")] byte[] VarbinaryValue
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    VarbinaryValue
                ).ReturnValue));
        }

        [Function(Name="dbo.PrimaryBadAddressFlag", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? PrimaryBadAddressFlag(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.RegexMatch", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string RegexMatch(
            [Parameter(Name = "subject", DbType="nvarchar")] string subject,
            [Parameter(Name = "pattern", DbType="nvarchar")] string pattern
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    subject,
                    pattern
                ).ReturnValue));
        }

        [Function(Name="dbo.NextTranChangeDate", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? NextTranChangeDate(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "tdt", DbType="datetime")] DateTime? tdt,
            [Parameter(Name = "typeid", DbType="int")] int? typeid
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    oid,
                    tdt,
                    typeid
                ).ReturnValue));
        }

        [Function(Name="dbo.AllRegexMatchs", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string AllRegexMatchs(
            [Parameter(Name = "subject", DbType="nvarchar")] string subject,
            [Parameter(Name = "pattern", DbType="nvarchar")] string pattern
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    subject,
                    pattern
                ).ReturnValue));
        }

        [Function(Name="dbo.NextChangeTransactionId", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? NextChangeTransactionId(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "tid", DbType="int")] int? tid,
            [Parameter(Name = "typeid", DbType="int")] int? typeid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    oid,
                    tid,
                    typeid
                ).ReturnValue));
        }

        [Function(Name="dbo.IsValidEmail", IsComposable = true)]
        [return: Parameter(DbType = "bit")]
        public bool? IsValidEmail(
            [Parameter(Name = "addr", DbType="nvarchar")] string addr
            )
        {
            return ((bool?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    addr
                ).ReturnValue));
        }

        [Function(Name="dbo.PrimaryAddress", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string PrimaryAddress(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.LastNameCount", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? LastNameCount(
            [Parameter(Name = "last", DbType="nvarchar")] string last
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    last
                ).ReturnValue));
        }

        [Function(Name="dbo.PrimaryState", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string PrimaryState(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.HomePhone", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string HomePhone(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.MemberStatusDescription", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string MemberStatusDescription(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.OrganizationLeaderId", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? OrganizationLeaderId(
            [Parameter(Name = "orgid", DbType="int")] int? orgid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid
                ).ReturnValue));
        }

        [Function(Name="dbo.UserName", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string UserName(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.EnrollmentTransactionId", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? EnrollmentTransactionId(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "tdt", DbType="datetime")] DateTime? tdt,
            [Parameter(Name = "ttid", DbType="int")] int? ttid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    oid,
                    tdt,
                    ttid
                ).ReturnValue));
        }

        [Function(Name="dbo.NextBirthday", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? NextBirthday(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.GetPeopleIdFromIndividualNumber", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? GetPeopleIdFromIndividualNumber(
            [Parameter(Name = "indnum", DbType="int")] int? indnum
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    indnum
                ).ReturnValue));
        }

        [Function(Name="dbo.OrganizationLeaderName", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string OrganizationLeaderName(
            [Parameter(Name = "orgid", DbType="int")] int? orgid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    orgid
                ).ReturnValue));
        }

        [Function(Name="dbo.UserPeopleIdFromEmail", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? UserPeopleIdFromEmail(
            [Parameter(Name = "email", DbType="nvarchar")] string email
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    email
                ).ReturnValue));
        }

        [Function(Name="dbo.SmallGroupLeader", IsComposable = true)]
        [return: Parameter(DbType = "varchar")]
        public string SmallGroupLeader(
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.DayAndTime", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string DayAndTime(
            [Parameter(Name = "dt", DbType="datetime")] DateTime? dt
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    dt
                ).ReturnValue));
        }

        [Function(Name="dbo.WeekNumber", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? WeekNumber(
            [Parameter(Name = "dt", DbType="datetime")] DateTime? dt
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    dt
                ).ReturnValue));
        }

        [Function(Name="dbo.DollarRange", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? DollarRange(
            [Parameter(Name = "amt", DbType="decimal")] decimal? amt
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    amt
                ).ReturnValue));
        }

        [Function(Name="dbo.SundayForWeekNumber", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? SundayForWeekNumber(
            [Parameter(Name = "wkn", DbType="int")] int? wkn
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    wkn
                ).ReturnValue));
        }

        [Function(Name="dbo.FirstMondayOfMonth", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? FirstMondayOfMonth(
            [Parameter(Name = "inputDate", DbType="datetime")] DateTime? inputDate
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    inputDate
                ).ReturnValue));
        }

        [Function(Name="dbo.StartsLower", IsComposable = true)]
        [return: Parameter(DbType = "bit")]
        public bool? StartsLower(
            [Parameter(Name = "s", DbType="nvarchar")] string s
            )
        {
            return ((bool?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    s
                ).ReturnValue));
        }

        [Function(Name="dbo.LastContact", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? LastContact(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.MaxPastMeeting", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? MaxPastMeeting(
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod()))
                ).ReturnValue));
        }

        [Function(Name="dbo.DaysSinceContact", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? DaysSinceContact(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.DaysBetween12Attend", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? DaysBetween12Attend(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "progid", DbType="int")] int? progid,
            [Parameter(Name = "divid", DbType="int")] int? divid,
            [Parameter(Name = "orgid", DbType="int")] int? orgid,
            [Parameter(Name = "lookback", DbType="int")] int? lookback
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    progid,
                    divid,
                    orgid,
                    lookback
                ).ReturnValue));
        }

        [Function(Name="dbo.MaxMeetingDate", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? MaxMeetingDate(
            [Parameter(Name = "oid", DbType="int")] int? oid
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                ).ReturnValue));
        }

        [Function(Name="dbo.GetCurrentMissionTripBundle", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? GetCurrentMissionTripBundle(
            [Parameter(Name = "next", DbType="datetime")] DateTime? next,
            [Parameter(Name = "prev", DbType="datetime")] DateTime? prev
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    next,
                    prev
                ).ReturnValue));
        }

        [Function(Name="dbo.UserRoleList", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string UserRoleList(
            [Parameter(Name = "uid", DbType="int")] int? uid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    uid
                ).ReturnValue));
        }

        [Function(Name="dbo.LastMemberTypeInTrans", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? LastMemberTypeInTrans(
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.MemberTypeAtLastDrop", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? MemberTypeAtLastDrop(
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.OrganizationMemberCount2", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? OrganizationMemberCount2(
            [Parameter(Name = "oid", DbType="int")] int? oid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                ).ReturnValue));
        }

        [Function(Name="dbo.LastIdInTrans", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? LastIdInTrans(
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.ComputeAge", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? ComputeAge(
            [Parameter(Name = "m", DbType="int")] int? m,
            [Parameter(Name = "d", DbType="int")] int? d,
            [Parameter(Name = "y", DbType="int")] int? y
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    m,
                    d,
                    y
                ).ReturnValue));
        }

        [Function(Name="dbo.GetCurrentOnlinePledgeBundle", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? GetCurrentOnlinePledgeBundle(
            [Parameter(Name = "next", DbType="datetime")] DateTime? next,
            [Parameter(Name = "prev", DbType="datetime")] DateTime? prev
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    next,
                    prev
                ).ReturnValue));
        }

        [Function(Name="dbo.GetWeekDayNameOfDate", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string GetWeekDayNameOfDate(
            [Parameter(Name = "DateX", DbType="datetime")] DateTime? DateX
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    DateX
                ).ReturnValue));
        }

        [Function(Name="dbo.GetScheduleDesc", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string GetScheduleDesc(
            [Parameter(Name = "meetingtime", DbType="datetime")] DateTime? meetingtime
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    meetingtime
                ).ReturnValue));
        }

        [Function(Name="dbo.NextAnniversary", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? NextAnniversary(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.FindResCode", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? FindResCode(
            [Parameter(Name = "zipcode", DbType="nvarchar")] string zipcode,
            [Parameter(Name = "country", DbType="nvarchar")] string country
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    zipcode,
                    country
                ).ReturnValue));
        }

        [Function(Name="dbo.SchoolGrade", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? SchoolGrade(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.WidowedDate", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? WidowedDate(
            [Parameter(Name = "peopleid", DbType="int")] int? peopleid
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    peopleid
                ).ReturnValue));
        }

        [Function(Name="dbo.FirstActivity", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? FirstActivity(
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod()))
                ).ReturnValue));
        }

        [Function(Name="dbo.GetPeopleIdFromACS", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? GetPeopleIdFromACS(
            [Parameter(Name = "famnum", DbType="int")] int? famnum,
            [Parameter(Name = "indnum", DbType="int")] int? indnum
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    famnum,
                    indnum
                ).ReturnValue));
        }

        [Function(Name="dbo.SundayForDate", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? SundayForDate(
            [Parameter(Name = "dt", DbType="datetime")] DateTime? dt
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    dt
                ).ReturnValue));
        }

        [Function(Name="dbo.CreateForeignKeys", IsComposable = true)]
        [return: Parameter(DbType = "varchar")]
        public string CreateForeignKeys(
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod()))
                ).ReturnValue));
        }

        [Function(Name="dbo.DropForeignKeys", IsComposable = true)]
        [return: Parameter(DbType = "varchar")]
        public string DropForeignKeys(
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod()))
                ).ReturnValue));
        }

        [Function(Name="dbo.ElapsedTime", IsComposable = true)]
        [return: Parameter(DbType = "varchar")]
        public string ElapsedTime(
            [Parameter(Name = "start", DbType="datetime")] DateTime? start,
            [Parameter(Name = "end", DbType="datetime")] DateTime? end
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    start,
                    end
                ).ReturnValue));
        }

        [Function(Name="dbo.UEmail", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string UEmail(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.DecToBase", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string DecToBase(
            [Parameter(Name = "val", DbType="bigint")] long? val,
            [Parameter(Name = "baseX", DbType="int")] int? baseX
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    val,
                    baseX
                ).ReturnValue));
        }

        [Function(Name="dbo.FindPerson0", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? FindPerson0(
            [Parameter(Name = "first", DbType="nvarchar")] string first,
            [Parameter(Name = "last", DbType="nvarchar")] string last,
            [Parameter(Name = "dob", DbType="datetime")] DateTime? dob,
            [Parameter(Name = "email", DbType="nvarchar")] string email,
            [Parameter(Name = "phone", DbType="nvarchar")] string phone
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    first,
                    last,
                    dob,
                    email,
                    phone
                ).ReturnValue));
        }

        [Function(Name="dbo.FirstMeetingDateLastLear", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string FirstMeetingDateLastLear(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.ParseDate", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? ParseDate(
            [Parameter(Name = "dtin", DbType="varchar")] string dtin
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    dtin
                ).ReturnValue));
        }

        [Function(Name="dbo.MemberTypeAsOf", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? MemberTypeAsOf(
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "dt", DbType="datetime")] DateTime? dt
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    pid,
                    dt
                ).ReturnValue));
        }

        [Function(Name="dbo.GetSecurityCode", IsComposable = true)]
        [return: Parameter(DbType = "char")]
        public string GetSecurityCode(
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod()))
                ).ReturnValue));
        }

        [Function(Name="dbo.PledgeCount", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? PledgeCount(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "days", DbType="int")] int? days,
            [Parameter(Name = "fundid", DbType="int")] int? fundid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    days,
                    fundid
                ).ReturnValue));
        }

        [Function(Name="dbo.PledgeAmount", IsComposable = true)]
        [return: Parameter(DbType = "money")]
        public decimal? PledgeAmount(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "days", DbType="int")] int? days,
            [Parameter(Name = "fundid", DbType="int")] int? fundid
            )
        {
            return ((decimal?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    days,
                    fundid
                ).ReturnValue));
        }

        [Function(Name="dbo.OrgFee", IsComposable = true)]
        [return: Parameter(DbType = "money")]
        public decimal? OrgFee(
            [Parameter(Name = "oid", DbType="int")] int? oid
            )
        {
            return ((decimal?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                ).ReturnValue));
        }

        [Function(Name="dbo.FmtPhone", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string FmtPhone(
            [Parameter(Name = "PhoneNumber", DbType="nvarchar")] string PhoneNumber
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    PhoneNumber
                ).ReturnValue));
        }

        [Function(Name="dbo.OrganizationProspectCount", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? OrganizationProspectCount(
            [Parameter(Name = "oid", DbType="int")] int? oid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                ).ReturnValue));
        }

        [Function(Name="dbo.IsSmallGroupLeaderOnly", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? IsSmallGroupLeaderOnly(
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.OrganizationPrevCount", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? OrganizationPrevCount(
            [Parameter(Name = "oid", DbType="int")] int? oid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                ).ReturnValue));
        }

        [Function(Name="dbo.GetAttendType", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? GetAttendType(
            [Parameter(Name = "attended", DbType="bit")] bool? attended,
            [Parameter(Name = "membertypeid", DbType="int")] int? membertypeid,
            [Parameter(Name = "group", DbType="bit")] bool? group
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    attended,
                    membertypeid,
                    group
                ).ReturnValue));
        }

        [Function(Name="dbo.LastChanged", IsComposable = true)]
        [return: Parameter(DbType = "datetime")]
        public DateTime? LastChanged(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "field", DbType="nvarchar")] string field
            )
        {
            return ((DateTime?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    field
                ).ReturnValue));
        }

        [Function(Name="dbo.GetDigits", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string GetDigits(
            [Parameter(Name = "Str", DbType="nvarchar")] string Str
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    Str
                ).ReturnValue));
        }

        [Function(Name="dbo.ParentNamesAndCells", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string ParentNamesAndCells(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.StatusFlagsAll", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string StatusFlagsAll(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.TotalPaid", IsComposable = true)]
        [return: Parameter(DbType = "money")]
        public decimal? TotalPaid(
            [Parameter(Name = "oid", DbType="int")] int? oid,
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((decimal?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid,
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.StatusFlag", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string StatusFlag(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.OrganizationPrevMemberCount", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? OrganizationPrevMemberCount(
            [Parameter(Name = "oid", DbType="int")] int? oid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    oid
                ).ReturnValue));
        }

        [Function(Name="dbo.DonorTotalUnits", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? DonorTotalUnits(
            [Parameter(Name = "t", DbType="table type")] string t,
            [Parameter(Name = "attr", DbType="int")] int? attr
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    t,
                    attr
                ).ReturnValue));
        }

        [Function(Name="dbo.DonorTotalGifts", IsComposable = true)]
        [return: Parameter(DbType = "money")]
        public decimal? DonorTotalGifts(
            [Parameter(Name = "t", DbType="table type")] string t,
            [Parameter(Name = "attr", DbType="int")] int? attr
            )
        {
            return ((decimal?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    t,
                    attr
                ).ReturnValue));
        }

        [Function(Name="dbo.DonorTotalMean", IsComposable = true)]
        [return: Parameter(DbType = "money")]
        public decimal? DonorTotalMean(
            [Parameter(Name = "t", DbType="table type")] string t,
            [Parameter(Name = "attr", DbType="int")] int? attr
            )
        {
            return ((decimal?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    t,
                    attr
                ).ReturnValue));
        }

        [Function(Name="dbo.SpaceToNull", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string SpaceToNull(
            [Parameter(Name = "s", DbType="nvarchar")] string s
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    s
                ).ReturnValue));
        }

        [Function(Name="dbo.ContributionCount", IsComposable = true)]
        [return: Parameter(DbType = "int")]
        public int? ContributionCount(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "days", DbType="int")] int? days,
            [Parameter(Name = "fundid", DbType="int")] int? fundid
            )
        {
            return ((int?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    days,
                    fundid
                ).ReturnValue));
        }

        [Function(Name="dbo.DonorTotalMedian", IsComposable = true)]
        [return: Parameter(DbType = "money")]
        public decimal? DonorTotalMedian(
            [Parameter(Name = "t", DbType="table type")] string t,
            [Parameter(Name = "attr", DbType="int")] int? attr,
            [Parameter(Name = "threshold", DbType="money")] decimal? threshold
            )
        {
            return ((decimal?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    t,
                    attr,
                    threshold
                ).ReturnValue));
        }

        [Function(Name="dbo.ContributionAmount", IsComposable = true)]
        [return: Parameter(DbType = "money")]
        public decimal? ContributionAmount(
            [Parameter(Name = "pid", DbType="int")] int? pid,
            [Parameter(Name = "days", DbType="int")] int? days,
            [Parameter(Name = "fundid", DbType="int")] int? fundid
            )
        {
            return ((decimal?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid,
                    days,
                    fundid
                ).ReturnValue));
        }

        [Function(Name="dbo.UName2", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string UName2(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.OneHeadOfHouseholdIsMember", IsComposable = true)]
        [return: Parameter(DbType = "bit")]
        public bool? OneHeadOfHouseholdIsMember(
            [Parameter(Name = "fid", DbType="int")] int? fid
            )
        {
            return ((bool?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    fid
                ).ReturnValue));
        }

        [Function(Name="dbo.UName", IsComposable = true)]
        [return: Parameter(DbType = "nvarchar")]
        public string UName(
            [Parameter(Name = "pid", DbType="int")] int? pid
            )
        {
            return ((string)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    pid
                ).ReturnValue));
        }

        [Function(Name="dbo.DonorTotalGiftsSize", IsComposable = true)]
        [return: Parameter(DbType = "money")]
        public decimal? DonorTotalGiftsSize(
            [Parameter(Name = "t", DbType="table type")] string t,
            [Parameter(Name = "min", DbType="int")] int? min,
            [Parameter(Name = "max", DbType="int")] int? max
            )
        {
            return ((decimal?)(ExecuteMethodCall(this,
                ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    t,
                    min,
                    max
                ).ReturnValue));
        }

#endregion
#region Stored Procedures
        
        [Function(Name="dbo.NextSecurityCode")]
        public ISingleResult< SecurityCode> NextSecurityCode(
            [Parameter(Name = "dt", DbType="datetime")] DateTime? dt
            )
        {
            IExecuteResult result = ExecuteMethodCall(this, ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    dt
            );
            return ((ISingleResult< SecurityCode>)(result.ReturnValue));
        }

        [Function(Name="dbo.TopPledgers")]
        public ISingleResult< TopGiver> TopPledgers(
            [Parameter(Name = "top", DbType="int")] int? top,
            [Parameter(Name = "sdate", DbType="datetime")] DateTime? sdate,
            [Parameter(Name = "edate", DbType="datetime")] DateTime? edate
            )
        {
            IExecuteResult result = ExecuteMethodCall(this, ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    top,
                    sdate,
                    edate
            );
            return ((ISingleResult< TopGiver>)(result.ReturnValue));
        }

        [Function(Name="dbo.TopGivers")]
        public ISingleResult< TopGiver> TopGivers(
            [Parameter(Name = "top", DbType="int")] int? top,
            [Parameter(Name = "sdate", DbType="datetime")] DateTime? sdate,
            [Parameter(Name = "edate", DbType="datetime")] DateTime? edate
            )
        {
            IExecuteResult result = ExecuteMethodCall(this, ((MethodInfo)(MethodBase.GetCurrentMethod())),
                    top,
                    sdate,
                    edate
            );
            return ((ISingleResult< TopGiver>)(result.ReturnValue));
        }

#endregion
   }

}
