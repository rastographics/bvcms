using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CmsData;
using System.Web.Mvc;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class MemberNotesModel
    {
        public Person person;

        [NoUpdate]
        public int PeopleId { get; set; }

        public CodeInfo LetterStatus { get; set; }

        [DisplayName("Letter Requested")]
        public DateTime? LetterDateRequested { get; set; }

        [DisplayName("Letter Received")]
        public DateTime? LetterDateReceived { get; set; }

        [UIHint("Textarea"), DisplayName("Letter Notes")]
        public string LetterStatusNotes { get; set; }

        public MemberNotesModel(int id)
        {
            person = DbUtil.Db.LoadPersonById(id);
            this.CopyPropertiesFrom(person);
            PeopleId = id;
        }
        public MemberNotesModel() { }

        public void UpdateMemberNotes()
        {
            person = DbUtil.Db.LoadPersonById(PeopleId);
            this.CopyPropertiesTo(person);

            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Updated Growth: {0}".Fmt(person.Name));
        }
    }
}