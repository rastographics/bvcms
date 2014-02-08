using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.People.Models
{
    public class ContactsReceivedModel : ContactsModel
    {
        public ContactsReceivedModel(int id)
            : base(id)
        {
            AddContact = "/Person2/AddContactReceived/" + id;
            base.AddContactButton = "Add Contact Received By This Person";
        }

        override public IQueryable<Contact> DefineModelList()
        {
            return from c in FilteredModelList()
                   where c.contactees.Any(p => p.PeopleId == person.PeopleId)
                   select c;
        }

        public override IEnumerable<ContactInfo> DefineViewList(IQueryable<Contact> q)
        {
            return from c in q
                   let contactor = c.contactsMakers.FirstOrDefault().person.Name
                   let contactee = person.PreferredName
                   let othercontactees = c.contactees.Count() > 1 ? " and others" : ""
                   let othercontactors = c.contactsMakers.Count() > 1 ? " and others" : ""
                   select new ContactInfo
                   {
                       ContactId = c.ContactId,
                       ContactDate = c.ContactDate,
                       ContactReason = c.ContactReason.Description,
                       TypeOfContact = c.ContactType.Description,
                       Contactee = contactee + othercontactees,
                       Contactor = contactor + othercontactors
                   };
        }
    }
}