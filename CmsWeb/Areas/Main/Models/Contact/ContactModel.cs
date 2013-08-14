using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
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
    }
}