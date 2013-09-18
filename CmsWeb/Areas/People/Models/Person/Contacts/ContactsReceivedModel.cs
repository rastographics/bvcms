using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models.Person
{
    public class ContactsReceivedModel : ContactsModel
    {
        public ContactsReceivedModel(int id) : base(id)
        {
            AddContact = "/Person2/AddContactReceived/" + id;
        }

        private IQueryable<Contact> contacts;

        override public IQueryable<Contact> FetchContacts()
        {
            if (contacts != null) 
                return contacts;
            contacts = from c in DbUtil.Db.Contacts
                where c.contactees.Any(p => p.PeopleId == person.PeopleId)
                select c;
            return contacts;
        }

        override public IEnumerable<ContactInfo> Contacts()
        {
            var q = FetchContacts();
            q = ApplySort(q);
            var q2 = from c in q
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
            return q2.Skip(Pager.StartRow).Take(Pager.PageSize);
        }
    }
}