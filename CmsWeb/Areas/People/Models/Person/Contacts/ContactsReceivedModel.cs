using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;

namespace CmsWeb.Areas.People.Models.Person
{
    public class ContactsReceivedModel : ContactsModel
    {
        public ContactsReceivedModel(int id)
            : base(id)
        {
            AddContact = "/Person2/AddContactReceived/" + id;
        }

        private IQueryable<Contact> contacts;

        override public IQueryable<Contact> ModelList()
        {
            if (contacts != null)
                return contacts;
            contacts = from c in DbUtil.Db.Contacts
                       where c.contactees.Any(p => p.PeopleId == person.PeopleId)
                       select c;
            return contacts;
        }

        override public IEnumerable<ContactInfo> ViewList()
        {
            var q = ApplySort().Skip(Pager.StartRow).Take(Pager.PageSize);
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