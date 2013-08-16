using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Code;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Models.ContactPage
{
    public class ContactModel : IValidatableObject
    {
        private Contact contact;
        private readonly CodeValueModel cv;

        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                if (_id == 0)
                    return;
                contact = DbUtil.Db.Contacts.Single(cc => cc.ContactId == value);
            }
        }

        public ContactModel()
        {
            cv = new CodeValueModel();
        }

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

        public ContactModel(int id)
            : this()
        {
            Id = id;

            ContactDate = contact.ContactDate.ToShortDateString();

            MinistryId = contact.MinistryId ?? 0;
            ContactTypeId = contact.ContactTypeId;
            ContactReasonId = contact.ContactReasonId;

            Ministry = cv.Ministries0().ItemValue(MinistryId);
            ContactType = cv.ContactTypeCodes0().ItemValue(ContactTypeId);
            Reason = cv.ContactReasonCodes0().ItemValue(ContactReasonId);

            LeftDoorHanger = contact.LeftDoorHanger ?? false;
            LeftMessage = contact.LeftMessage ?? false;
            ContactMade = contact.ContactMade ?? false;
            GospelShared = contact.GospelShared ?? false;
            PrayerRequest = contact.PrayerRequest ?? false;
            GiftBagGiven = contact.GiftBagGiven ?? false;
            Comments = contact.Comments;

            MinisteredTo = new ContacteesModel(id);
            Ministers = new ContactorsModel(id);
            MinisteredTo.CanViewComments = CanViewComments;
            Ministers.CanViewComments = CanViewComments;
        }

        public string ContactType { get; set; }
        public string Reason { get; set; }
        public string Ministry { get; set; }
        public ContacteesModel MinisteredTo { get; set; }
        public ContactorsModel Ministers { get; set; }

        public SelectList ContactTypes()
        {
            return new SelectList(new CodeValueModel().ContactTypeCodes0(), "Id", "Value");
        }

        public SelectList ContactReasons()
        {
            return new SelectList(new CodeValueModel().ContactReasonCodes0(), "Id", "Value");
        }

        public SelectList Ministries()
        {
            return new SelectList(new CodeValueModel().Ministries0(), "Id", "Value");
        }

        public void UpdateContact()
        {
            contact.ContactDate = DateTime.Parse(ContactDate);
            contact.MinistryId = MinistryId;
            contact.ContactReasonId = ContactReasonId;
            contact.ContactTypeId = ContactTypeId;

            contact.LeftDoorHanger = LeftDoorHanger;
            contact.LeftMessage = LeftMessage;
            contact.ContactMade = ContactMade;
            contact.GospelShared = GospelShared;
            contact.PrayerRequest = PrayerRequest;
            contact.GiftBagGiven = GiftBagGiven;
            contact.Comments = Comments;

            DbUtil.Db.SubmitChanges();

            Ministry = cv.Ministries0().ItemValue(MinistryId);
            ContactType = cv.ContactTypeCodes0().ItemValue(ContactTypeId);
            Reason = cv.ContactReasonCodes0().ItemValue(ContactReasonId);
        }
        internal void DeleteContact()
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            cn.Execute(@"
delete contactees where ContactId = @cid;
delete contactors where ContactId = @cid;
delete contact where ContactId = @cid;
", new { cid = contact.ContactId });
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

        private bool? canViewComments;
        private int _id;

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
                        where c.ContactId == contact.ContactId
                        select c.PeopleId;
                var q2 = from c in DbUtil.Db.Contactors
                         where c.ContactId == contact.ContactId
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
            if (MinistryId == 0)
                results.Add(ModelError("Ministry is required", "MinistryId"));
            if (ContactTypeId == 0)
                results.Add(ModelError("ContactType is required", "ContactTypeId"));
            if (ContactReasonId == 0)
                results.Add(ModelError("ContactReason is required", "ContactReasonId"));

            return results;
        }
        private ValidationResult ModelError(string message, string field)
        {
            return new ValidationResult(message, new[] { field });
        }
    }
}