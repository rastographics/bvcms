using System;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api
{
    [ApiMapName("OrganizationMembers")]
    public class ApiOrganizationMember
    {
        [Key]
        public int OrganizationId { get; set; }
        [Key]
        public int PeopleId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int MemberTypeId { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? InactiveDate { get; set; }
        public string AttendStr { get; set; }
        public decimal? AttendPct { get; set; }
        public DateTime? LastAttended { get; set; }
        public bool? Pending { get; set; }
        public string UserData { get; set; }
        public decimal? Amount { get; set; }
        public string Request { get; set; }
        public string ShirtSize { get; set; }
        public int? Grade { get; set; }
        public int? Tickets { get; set; }
        public bool? Moved { get; set; }
        public string RegisterEmail { get; set; }
        public decimal? AmountPaid { get; set; }
        public string PayLink { get; set; }
        public int? TranId { get; set; }
        public string Score { get; set; }
        public string DatumId { get; set; }
        public bool? Hidden { get; set; }
        public bool? SkipInsertTriggerProcessing { get; set; }
        public int? RegistrationDataId { get; set; }
        public string OnlineRegData { get; set; }
    }
}
