using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models.Person
{
    public class ContactsMadeModel : ContactsModel
    {
        public ContactsMadeModel(int id)
            : base(id)
        {
            AddContact = "/Person2/AddContactMade/" + id;
        }
        private IQueryable<Contact> contacts;
        override public IQueryable<Contact> ModelList()
        {
            if (contacts != null)
                return contacts;
            contacts = from c in DbUtil.Db.Contacts
                       where c.contactsMakers.Any(p => p.PeopleId == person.PeopleId)
                       select c;
            return contacts;
        }
        override public IEnumerable<ContactInfo> ViewList()
        {
            var q = ApplySort().Skip(Pager.StartRow).Take(Pager.PageSize);
            return from c in q
                   let contactor = person.PreferredName
                   let contactee = c.contactees.FirstOrDefault().person.Name
                   let othercontactors = c.contactsMakers.Count() > 1 ? " and others" : ""
                   let othercontactees = c.contactees.Count() > 1 ? " and others" : ""
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