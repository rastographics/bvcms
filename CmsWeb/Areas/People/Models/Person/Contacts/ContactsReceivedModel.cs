using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class ContactsReceivedModel : ContactsModel
    {
        public override string AddContact { get { return "/Person2/AddContactReceived/" + PeopleId; } }
        public override string AddContactButton { get { return "Add Contact Received By This Person"; } }

        public ContactsReceivedModel() { }
        override public IQueryable<Contact> DefineModelList()
        {
            return from c in FilteredModelList()
                   where c.contactees.Any(p => p.PeopleId == PeopleId)
                   select c;
        }

        public override IEnumerable<ContactInfo> DefineViewList(IQueryable<Contact> q)
        {
            return from c in q
                   let contactor = c.contactsMakers.FirstOrDefault().person.Name
                   let contactee = Person.PreferredName
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
