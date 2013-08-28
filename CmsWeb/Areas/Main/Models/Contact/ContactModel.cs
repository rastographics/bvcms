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
using UtilityExtensions;

namespace CmsWeb.Models.ContactPage
{
    public class ContactModel : IValidatableObject
    {
        internal Contact contact;
        public readonly CodeValueModel cv;
        private readonly CMSDataContext Db;
        private bool? canViewComments;
        private int _id;
        private string _incomplete;

        public string ContactDate { get; set; }

        public int MinistryId { get; set; }
        public int ContactTypeId { get; set; }
        public int ContactReasonId { get; set; }

        public bool NotAtHome { get; set; }
        public bool LeftDoorHanger { get; set; }
        public bool LeftMessage { get; set; }
        public bool ContactMade { get; set; }
        public bool GospelShared { get; set; }
        public bool PrayerRequest { get; set; }
        public bool GiftBagGiven { get; set; }
        public string Comments { get; set; }

        [CodeValue]
        public string ContactType { get; set; }
        [CodeValue]
        public string ContactReason{ get; set; }
        [CodeValue]
        public string Ministry{ get; set; }
        
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                if (_id == 0)
                    return;
                contact = DbUtil.Db.Contacts.SingleOrDefault(cc => cc.ContactId == value);
                if (contact == null)
                    return;
                MinisteredTo = new ContacteesModel(value);
                Ministers = new ContactorsModel(value);
                MinisteredTo.CanViewComments = CanViewComments;
                Ministers.CanViewComments = CanViewComments;
            }
        }

        public ContactModel()
        {
            Db = DbUtil.Db;
            cv = new CodeValueModel();
        }
        public ContactModel(int id)
            : this()
        {
            Id = id;
            if (contact == null)
                return;
            this.CopyPropertiesFrom(contact);
        }

        public ContacteesModel MinisteredTo { get; set; }
        public ContactorsModel Ministers { get; set; }

        public void UpdateContact()
        {
            this.CopyPropertiesTo(contact);
            this.CopyPropertiesFrom(contact, onlyfields:"ContactType,ContactReason,Ministry");
            Db.SubmitChanges();
        }
        internal void DeleteContact()
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            cn.Execute(@"
                delete contactees where ContactId = @cid;
                delete contactors where ContactId = @cid;
                update task set CompletedContactId = NULL WHERE CompletedContactId = @cid;
                update task set SourceContactId = NULL WHERE CompletedContactId = @cid;
                delete contact where ContactId = @cid;
                ", new { cid = Id });
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
            var q = from cor in Db.Contactors
                    where cor.ContactId == contact.ContactId
                    select cor;
            foreach (var p in q)
                c.contactsMakers.Add(new Contactor { PeopleId = p.PeopleId });
            Db.Contacts.InsertOnSubmit(c);
            Db.SubmitChanges();
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

                var q = from c in Db.Contactees
                        where c.ContactId == Id
                        select c.PeopleId;
                var q2 = from c in Db.Contactors
                         where c.ContactId == Id
                         select c.PeopleId;
                var a = q.Union(q2).ToArray();

                Tag tag = Db.OrgLeadersOnlyTag2();
                canViewComments = tag.People(Db).Any(p => a.Contains(p.PeopleId));
                return canViewComments.Value;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!Util.DateValid(ContactDate))
                results.Add(ModelError("Contact Date is required", "ContactDate"));

            if (MinistryId == 0)
                results.Add(ModelError("Ministry is required", "MinistryId"));

            if (ContactTypeId == 0)
                results.Add(ModelError("ContactType is required", "ContactTypeId"));

            if (ContactReasonId == 0)
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
            Append(MinistryId == 0, sb, "no ministry");
            Append(ContactTypeId == 0, sb, "no type");
            Append(ContactReasonId == 0, sb, "no reason");
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