using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class RegistrationsModel
    {
        private Person person;

        public RegistrationsModel(int id)
        {
            PeopleId = id;
            var rr = Person.SetRecReg();
            this.CopyPropertiesFrom(rr);
            CustodyIssue = Person.CustodyIssue ?? false;
            OkTransport = Person.OkTransport ?? false;
        }

        public RegistrationsModel()
        {
        }

        public int? PeopleId { get; set; }

        public Person Person
        {
            get
            {
                if (person == null && PeopleId.HasValue)
                {
                    person = DbUtil.Db.LoadPersonById(PeopleId.Value);
                }

                return person;
            }
        }

        public string ShirtSize { get; set; }
        public bool CustodyIssue { get; set; }
        public bool OkTransport { get; set; }

        [DisplayName("Emergency Contact"), StringLength(100)]
        public string Emcontact { get; set; }

        [DisplayName("Emergency Phone"), StringLength(50)]
        public string Emphone { get; set; }

        [DisplayName("Health Insurance")]
        public string Insurance { get; set; }

        [DisplayName("Policy Number")]
        public string Policy { get; set; }

        public string Doctor { get; set; }

        [DisplayName("Doctor's Phone")]
        public string Docphone { get; set; }

        [UIHint("Textarea"), DisplayName("Allergies")]
        public string MedicalDescription { get; set; }

        [UIHint("Textarea"), DisplayName("Registration Log")]
        public string Comments { get; set; }
        public bool ShowComments { get; set; }

        public bool Tylenol { get; set; }
        public bool Advil { get; set; }
        public bool Robitussin { get; set; }
        public bool Maalox { get; set; }

        [DisplayName("Mother's Name")]
        public string Mname { get; set; }

        [DisplayName("Father's Name")]
        public string Fname { get; set; }

        [DisplayName("Member Here")]
        public bool Member { get; set; }

        public bool ActiveInAnotherChurch { get; set; }

        [DisplayName("Coaching Interest")]
        public bool Coaching { get; set; }

        public void UpdateModel(bool ExcludeComments)
        {
            var rr = Person.SetRecReg();
            if (ExcludeComments)
            {
                this.CopyPropertiesTo(rr, null, "", "Comments");
            }
            else
            {
                this.CopyPropertiesTo(rr);
            }
            Person.CustodyIssue = CustodyIssue;
            Person.OkTransport = OkTransport;

            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity($"Updated RecReg: {Person.Name}");
        }

        public List<GoerItem> GoerList()
        {
            return (from m in Person.OrganizationMembers
                    where m.Organization.IsMissionTrip == true
                    where m.Organization.OrganizationStatusId == OrgStatusCode.Active
                    where m.OrgMemMemTags.Any(mm => mm.MemberTag.Name == "Goer")
                    let ts = DbUtil.Db.ViewMissionTripTotals.SingleOrDefault(tt => tt.OrganizationId == m.OrganizationId && tt.PeopleId == m.PeopleId)
                    select new GoerItem
                    {
                        Id = m.OrganizationId,
                        Trip = m.Organization?.OrganizationName,
                        Cost = ts.TripCost ?? 0,
                        Paid = ts.Raised ?? 0,
                        ShowFundingLink = m.Organization?.TripFundingPagesEnable ?? false
                    }).ToList();
        }

        public class GoerItem
        {
            public int Id { get; set; }
            public string Trip { get; set; }
            public decimal Cost { get; set; }
            public decimal Paid { get; set; }
            public bool ShowFundingLink { get; set; }
        }
    }
}
