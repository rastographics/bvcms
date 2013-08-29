using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using Dapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using UtilityExtensions;

namespace CmsWeb.Models.ContactPage
{
    public class ContactModel : IValidatableObject
    {
        private bool? canViewComments;
        private string _incomplete;

        [NoUpdate]
        public int ContactId { get; set; }
        public string ContactDate { get; set; }

        public bool NotAtHome { get; set; }
        public bool LeftDoorHanger { get; set; }
        public bool LeftMessage { get; set; }
        public bool ContactMade { get; set; }
        public bool GospelShared { get; set; }
        public bool PrayerRequest { get; set; }
        public bool GiftBagGiven { get; set; }
        public string Comments { get; set; }

        public CodeInfo ContactType { get; set; }
        public CodeInfo ContactReason { get; set; }
        public CodeInfo Ministry { get; set; }

        internal Contact contact;
        private void LoadContact(int id)
        {
            contact = DbUtil.Db.Contacts.SingleOrDefault(cc => cc.ContactId == id);
            MinisteredTo = new ContacteesModel(id);
            Ministers = new ContactorsModel(id);
            MinisteredTo.CanViewComments = CanViewComments;
            Ministers.CanViewComments = CanViewComments;
        }

        public ContactModel()
        {
        }
        public ContactModel(int id)
            : this()
        {
            LoadContact(id);
            this.CopyPropertiesFrom(contact);
        }

        public ContacteesModel MinisteredTo { get; set; }
        public ContactorsModel Ministers { get; set; }

        public void UpdateContact()
        {
            LoadContact(ContactId);
            this.CopyPropertiesTo(contact);
            DbUtil.Db.SubmitChanges();
        }
        public static void DeleteContact(int cid)
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            cn.Execute(@"
                delete contactees where ContactId = @cid;
                delete contactors where ContactId = @cid;
                update task set CompletedContactId = NULL WHERE CompletedContactId = @cid;
                update task set SourceContactId = NULL WHERE CompletedContactId = @cid;
                delete contact where ContactId = @cid;
                ", new { cid });
        }

        public int AddNewTeamContact()
        {
            var c = new Contact
            {
                ContactDate = DateTime.Now.Date,
                MinistryId = contact.MinistryId,
                CreatedBy = Util.UserId1,
                CreatedDate = DateTime.Now,
                ContactTypeId = contact.ContactTypeId,
                ContactReasonId = contact.ContactReasonId,
            };
            var q = from cor in DbUtil.Db.Contactors
                    where cor.ContactId == contact.ContactId
                    select cor;
            foreach (var p in q)
                c.contactsMakers.Add(new Contactor { PeopleId = p.PeopleId });
            DbUtil.Db.Contacts.InsertOnSubmit(c);
            DbUtil.Db.SubmitChanges();
            return c.ContactId;
        }

        public bool CanViewComments
        {
            get
            {
                if (canViewComments.HasValue)
                    return canViewComments.Value;

                if (!Util2.OrgLeadersOnly)
                {
                    canViewComments = true;
                    return true;
                }

                var q = from c in DbUtil.Db.Contactees
                        where c.ContactId == ContactId
                        select c.PeopleId;
                var q2 = from c in DbUtil.Db.Contactors
                         where c.ContactId == ContactId
                         select c.PeopleId;
                var a = q.Union(q2).ToArray();

                Tag tag = DbUtil.Db.OrgLeadersOnlyTag2();
                canViewComments = tag.People(DbUtil.Db).Any(p => a.Contains(p.PeopleId));
                return canViewComments.Value;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!Util.DateValid(ContactDate))
                results.Add(ModelError("Contact Date is required", "ContactDate"));

            if (Ministry.Value == "0")
                results.Add(ModelError("Ministry is required", "MinistryId"));

            if (ContactType.Value == "0")
                results.Add(ModelError("ContactType is required", "ContactTypeId"));

            if (ContactReason.Value == "0")
                results.Add(ModelError("ContactReason is required", "ContactReasonId"));

            return results;
        }
        private static ValidationResult ModelError(string message, string field)
        {
            return new ValidationResult(message, new[] { field });
        }

        public string Incomplete
        {
            get
            {
                if (_incomplete == null)
                    _incomplete = GetIncomplete();
                return _incomplete;
            }
        }

        private string GetIncomplete()
        {
            var sb = new StringBuilder();
            Append(Ministry.Value == "0", sb, "no ministry");
            Append(ContactType.Value == "0", sb, "no type");
            Append(ContactReason.Value == "0", sb, "no reason");
            Append(Ministers.Count() == 0, sb, "no contactors");
            Append(MinisteredTo.Count() == 0, sb, "no contactees");
            if (sb.Length > 0)
                return sb.ToString();
            return "";
        }
        private void Append(bool tf, StringBuilder sb, string text)
        {
            if (!tf)
                return;
            if (sb.Length > 0)
                sb.Append(", ");
            sb.Append(text);
        }
    }
}