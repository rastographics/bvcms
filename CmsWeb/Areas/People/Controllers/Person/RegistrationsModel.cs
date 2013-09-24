using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class RegistrationsModel
    {
        public Person person;

        public RegistrationsModel(int id)
        {
            person = DbUtil.Db.LoadPersonById(id);
        }

        public string ShirtSize { get; set; }

        public bool CustodyIssue { get; set; }

        public bool OkTransport { get; set; }

        [DisplayName("Emergency Contact")]
        public string Emcontact { get; set; }

        [DisplayName("Emergency Phone")]
        public string Emphone { get; set; }

        [DisplayName("Health Insurance Carrier")]
        public string Insurance { get; set; }

        [DisplayName("Policy Number")]
        public string Policy { get; set; }

        public string Doctor { get; set; }

        [DisplayName("Doctor's Phone")]
        public string Docphone { get; set; }

        [UIHint("textarea")]
        public string MedicalDescription { get; set; }

        [UIHint("textarea")]
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

        [DisplayName("Interested in Coaching")]
        public bool Coaching { get; set; }

        public static RegistrationsModel GetRegistrations(int id)
        {
            var m = new RegistrationsModel(id);
            var rr = m.person.GetRecReg();
            m.CopyPropertiesFrom(rr);
            m.OkTransport = m.person.OkTransport ?? false;
            m.CustodyIssue = m.person.CustodyIssue ?? false;
            return m;
        }
        public void UpdateModel()
        {
            var rr = person.SetRecReg();
            this.CopyPropertiesTo(rr);
            person.CustodyIssue = CustodyIssue;
            person.OkTransport = OkTransport;

            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Updated RecReg: {0}".Fmt(person.Name));
        }
    }
}
