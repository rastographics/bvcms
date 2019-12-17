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
        public int? GradeAgeStart { get; set; }
        public int? GradeAgeEnd { get; set; }
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
        public int? CampusId { get; set; }
        public int? Limit { get; set; }
        public int? GenderId { get; set; }
        public string Description { get; set; }
        public DateTime? BirthDayStart { get; set; }
        public DateTime? BirthDayEnd { get; set; }
        public DateTime? LastDayBeforeExtra { get; set; }
        public int? RegistrationTypeId { get; set; }
        public string PhoneNumber { get; set; }
        public bool? RegistrationClosed { get; set; }
        public string NotifyIds { get; set; }
        public string RegSetting { get; set; }
        public string OrgPickList { get; set; }
        public bool? Offsite { get; set; }
        public DateTime? RegStart { get; set; }
        public DateTime? RegEnd { get; set; }
        public string LimitToRole { get; set; }
        public int? OrganizationTypeId { get; set; }
        public bool? SuspendCheckin { get; set; }
        public int? PublishDirectory { get; set; }
        public bool? IsMissionTrip { get; set; }
        public DateTime? VisitorDate { get; set; }
        public string PublicSortOrder { get; set; }
        public bool? UseRegisterLink2 { get; set; }
        public string AppCategory { get; set; }
        public string RegistrationTitle { get; set; }
        public int? PrevMemberCount { get; set; }
        public int? ProspectCount { get; set; }
        public string RegSettingXml { get; set; }
    }
}
