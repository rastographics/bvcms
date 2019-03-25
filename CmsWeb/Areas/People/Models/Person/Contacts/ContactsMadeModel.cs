using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class ContactsMadeModel : ContactsModel
    {
        public override string AddContact { get { return "/Person2/AddContactMade/" + PeopleId; } }
        public override string AddContactButton { get { return "Add Contact Made By This Person"; } }

        public ContactsMadeModel() { }
        override public IQueryable<Contact> DefineModelList()
        {
            return from c in FilteredModelList()
                   where c.contactsMakers.Any(p => p.PeopleId == Person.PeopleId)
                   select c;
        }

        public override IEnumerable<ContactInfo> DefineViewList(IQueryable<Contact> q)
        {
            return from c in q
                   let contactor = Person.PreferredName
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
