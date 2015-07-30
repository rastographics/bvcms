using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CmsData;
using CmsWeb.Code;

namespace CmsWeb.Areas.People.Models
{
    public class MemberNotesModel
    {
        public Person person;

        public MemberNotesModel(int id)
        {
            person = DbUtil.Db.LoadPersonById(id);
            this.CopyPropertiesFrom(person);
            PeopleId = id;
        }

        public MemberNotesModel()
        {
        }

        [NoUpdate]
        public int PeopleId { get; set; }

        public CodeInfo LetterStatus { get; set; }

        [DisplayName("Letter Requested")]
        public DateTime? LetterDateRequested { get; set; }

        [DisplayName("Letter Received")]
        public DateTime? LetterDateReceived { get; set; }

        [UIHint("Textarea"), DisplayName("Letter Notes")]
        public string LetterStatusNotes { get; set; }

        public void UpdateMemberNotes()
        {
            person = DbUtil.Db.LoadPersonById(PeopleId);
            this.CopyPropertiesTo(person);

            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity($"Updated Growth: {person.Name}");
        }
    }
}
