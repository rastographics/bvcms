using System;
using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api
{
    public class ApiChAiPerson
    {
        [Key]
		public int PeopleId { get; set; }
        [Key]
		public int FamilyId { get; set; }
		public DateTime? CreatedDate { get; set; }
		public int? HeadOfHouseholdId { get; set; }
		public DateTime? FamilyCreatedDate { get; set; }
		public string MemberStatus { get; set; }
		public string Salutation { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string FamilyPosition { get; set; }
		public string MaritalStatus { get; set; }
		public string Gender { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string HomePhone { get; set; }
		public string EmailAddress { get; set; }
		public string BirthDate { get; set; }
		public DateTime? LastModified { get; set; }
    }
}
