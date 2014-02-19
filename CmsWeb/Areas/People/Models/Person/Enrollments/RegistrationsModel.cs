using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using CmsData;
using System.Linq;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class RegistrationsModel
    {
        public Person person;
        private int peopleId;

        [NoUpdate]
        public int PeopleId
        {
            get { return peopleId; }
            set
            {
                peopleId = value;
                person = DbUtil.Db.LoadPersonById(peopleId);
            }
        }

        public RegistrationsModel(int id)
        {
            person = DbUtil.Db.LoadPersonById(id);
            var rr = person.SetRecReg();
            this.CopyPropertiesFrom(rr);
        }
        public RegistrationsModel() { }

        public string ShirtSize { get; set; }

        public bool CustodyIssue { get; set; }

        public bool OkTransport { get; set; }

        [DisplayName("Emergency Contact")]
        public string Emcontact { get; set; }

        [DisplayName("Emergency Phone")]
        public string Emphone { get; set; }

        [DisplayName("Health Insurance")]
        public string Insurance { get; set; }

        [DisplayName("Policy Number")]
        public string Policy { get; set; }

        public string Doctor { get; set; }

        [DisplayName("Doctor's Phone")]
        public string Docphone { get; set; }

        [UIHint("Textarea")]
        public string MedicalDescription { get; set; }

        [UIHint("Textarea"), DisplayName("Registration Log")]
        public string Comments { get; set; }

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
        public bool ActiveInOtherChurch { get; set; }

        [DisplayName("Coaching Interest")]
        public bool Coaching { get; set; }

        public void UpdateModel()
        {
            var rr = person.SetRecReg();
            this.CopyPropertiesTo(rr);
            person.CustodyIssue = CustodyIssue;
            person.OkTransport = OkTransport;

            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Updated RecReg: {0}".Fmt(person.Name));
        }

        public class GoerItem
        {
            public int Id { get; set; }
            public string Trip { get; set; }
            public decimal Cost { get; set; }
            public decimal Paid { get; set; }
        }

        public List<GoerItem> GoerList()
        {
            if(!HttpContext.Current.User.IsInRole("Developer"))
                return new List<GoerItem>();
            return (from m in person.OrganizationMembers
                    where m.Organization.IsMissionTrip == true
                    where m.Organization.OrganizationStatusId == CmsData.Codes.OrgStatusCode.Active
                    where m.OrgMemMemTags.Any(mm => mm.MemberTag.Name == "Goer")
                    select new GoerItem
                    {
                        Id = m.OrganizationId,
                        Trip = m.Organization.OrganizationName,
                        Cost = m.Amount ?? 0,
                        Paid = m.TotalPaid(DbUtil.Db) ?? 0,
                    }).ToList();
        }
    }
}
