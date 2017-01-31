using System;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api
{
    public class ApiPerson
    {
        [Key]
        public int PeopleId { get; set; }
        public int FamilyId { get; set; }
        public int PositionInFamilyId { get; set; }
        public int MemberStatusId { get; set; }
        public int? CampusId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GenderId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string EmailAddress { get; set; }
        public int MaritalStatusId { get; set; }
        public string PrimaryAddress { get; set; }
        public string PrimaryAddress2 { get; set; }
        public string PrimaryCity { get; set; }
        public string PrimaryState { get; set; }
        public string PrimaryZip { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string CellPhone { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeceased { get; set; }
    }
}
