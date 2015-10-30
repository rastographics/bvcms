using System;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api
{
    public class ApiOrganization
    {
        [Key]
        public int OrganizationId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int OrganizationStatusId { get; set; }
        public int? DivisionId { get; set; }
        public int? LeaderMemberTypeId { get; set; }
        public int? GradeAgeStart { get; set; }
        public int? GradeAgeEnd { get; set; }
        public int? RollSheetVisitorWks { get; set; }
        public int SecurityTypeId { get; set; }
        public DateTime? FirstMeetingDate { get; set; }
        public DateTime? LastMeetingDate { get; set; }
        public DateTime? OrganizationClosedDate { get; set; }
        public string Location { get; set; }
        public string OrganizationName { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? EntryPointId { get; set; }
        public int? ParentOrgId { get; set; }
        public bool AllowAttendOverlap { get; set; }
        public int? MemberCount { get; set; }
        public int? LeaderId { get; set; }
        public string LeaderName { get; set; }
        public bool? ClassFilled { get; set; }
        public int? OnLineCatalogSort { get; set; }
        public string PendingLoc { get; set; }
        public bool? CanSelfCheckin { get; set; }
        public int? NumCheckInLabels { get; set; }
        public int? CampusId { get; set; }
        public bool? AllowNonCampusCheckIn { get; set; }
        public int? NumWorkerCheckInLabels { get; set; }
        public bool? ShowOnlyRegisteredAtCheckIn { get; set; }
        public int? Limit { get; set; }
        public int? GenderId { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDayStart { get; set; }
        public DateTime? BirthDayEnd { get; set; }
        public DateTime? LastDayBeforeExtra { get; set; }
        public int? RegistrationTypeId { get; set; }
        public string ValidateOrgs { get; set; }
        public string PhoneNumber { get; set; }
        public bool? RegistrationClosed { get; set; }
        public bool? AllowKioskRegister { get; set; }
        public string WorshipGroupCodes { get; set; }
        public bool? IsBibleFellowshipOrg { get; set; }
        public bool? NoSecurityLabel { get; set; }
        public bool? AlwaysSecurityLabel { get; set; }
        public int? DaysToIgnoreHistory { get; set; }
        public string NotifyIds { get; set; }
        public double? Lat { get; set; }
        public double? LongX { get; set; }
        public string RegSetting { get; set; }
        public string OrgPickList { get; set; }
        public bool? Offsite { get; set; }
        public DateTime? RegStart { get; set; }
        public DateTime? RegEnd { get; set; }
        public string LimitToRole { get; set; }
        public int? OrganizationTypeId { get; set; }
        public string MemberJoinScript { get; set; }
        public string AddToSmallGroupScript { get; set; }
        public string RemoveFromSmallGroupScript { get; set; }
        public bool? SuspendCheckin { get; set; }
        public bool? NoAutoAbsents { get; set; }
        public int? PublishDirectory { get; set; }
        public int? ConsecutiveAbsentsThreshold { get; set; }
        public bool IsRecreationTeam { get; set; }
        public bool? NotWeekly { get; set; }
        public bool? IsMissionTrip { get; set; }
        public bool? NoCreditCards { get; set; }
        public string GiftNotifyIds { get; set; }
        public DateTime? VisitorDate { get; set; }
        public bool? UseBootstrap { get; set; }
        public string PublicSortOrder { get; set; }
        public bool? UseRegisterLink2 { get; set; }
        public string AppCategory { get; set; }
        public string RegistrationTitle { get; set; }
        public int? PrevMemberCount { get; set; }
        public int? ProspectCount { get; set; }
        public string RegSettingXml { get; set; }
    }
}
