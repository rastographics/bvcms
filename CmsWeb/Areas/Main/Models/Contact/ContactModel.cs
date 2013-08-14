using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using CmsWeb.Models.ContactPage;
using Dapper;
using DocumentFormat.OpenXml.Spreadsheet;
using UtilityExtensions;

namespace CmsWeb.Models.ContactPage
{
    public class ContactModel
    {
        public Contact contact { get; set; }

        public ContactModel()
        {
        }

        public ContactModel(int id)
        {
            contact = DbUtil.Db.Contacts.Single(cc => cc.ContactId == id);
            Ministry = Ministries().Single(ss => ss.Value == contact.MinistryId.ToString()).Text;
            ContactType = ContactTypes().Single(ss => ss.Value == contact.ContactTypeId.ToString()).Text;
            Reason = ContactReasons().Single(ss => ss.Value == contact.ContactReasonId.ToString()).Text;
            Contactees = new ContacteesModel(id);
            Contactors = new ContactorsModel(id);
            Contactees.CanViewComments = CanViewComments;
            Contactors.CanViewComments = CanViewComments;
        }

        public string ContactType { get; set; }
        public string Reason { get; set; }
        public string Ministry { get; set; }
        public ContacteesModel Contactees { get; set; }
        public ContactorsModel Contactors { get; set; }

        public SelectList ContactTypes()
        {
            return new SelectList(new CodeValueModel().ContactTypeCodes0(),
                "Id", "Value", contact.ContactTypeId);
        }
        public SelectList ContactReasons()
        {
            return new SelectList(new CodeValueModel().ContactReasonCodes0(),
                "Id", "Value", contact.ContactReasonId);
        }
        public SelectList Ministries()
        {
            return new SelectList(new CodeValueModel().Ministries0(),
                "Id", "Value", contact.MinistryId);
        }

        public void UpdateContact(Contact c)
        {
            contact.ContactTypeId = c.ContactTypeId;
            contact.ContactDate = c.ContactDate;
            contact.ContactReasonId = c.ContactReasonId;
            contact.MinistryId = c.MinistryId;
            contact.NotAtHome = c.NotAtHome;
            contact.LeftDoorHanger = c.LeftDoorHanger;
            contact.LeftMessage = c.LeftMessage;
            contact.GospelShared = c.GospelShared;
            contact.PrayerRequest = c.PrayerRequest;
            contact.ContactMade = c.ContactMade;
            contact.GiftBagGiven = c.GiftBagGiven;
            contact.Comments = c.Comments;
            contact.ModifiedBy = c.ModifiedBy;
            contact.ModifiedDate = c.ModifiedDate;
            DbUtil.Db.SubmitChanges();
            Ministry = Ministries().Single(ss => ss.Value == contact.MinistryId.ToString()).Text;
            ContactType = ContactTypes().Single(ss => ss.Value == contact.ContactTypeId.ToString()).Text;
            Reason = ContactReasons().Single(ss => ss.Value == contact.ContactReasonId.ToString()).Text;
        }

        internal void DeleteContact()
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            cn.Execute(@"
delete contactees where ContactId = @cid;
delete contactors where ContactId = @cid;
delete contact where ContactId = @cid;
",          new {cid = contact.ContactId });
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
    }
}