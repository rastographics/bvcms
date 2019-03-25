using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Areas.People.Models;
using CmsWeb.Models;

namespace CmsWeb.Areas.Org.Models
{
    public class ContactsReceivedModel : ContactsModel
    {
        public override string AddContact { get { return "/Org/AddContactReceived/" + OrganizationId; } }
        public override string AddContactButton { get { return "Add Contact Received By This Organization"; } }
        public ContactsReceivedModel() { }
        override public IQueryable<Contact> DefineModelList()
        {
            return from c in FilteredModelList()
                   where c.OrganizationId == OrganizationId
                   select c;
        }

        public override IEnumerable<ContactInfo> DefineViewList(IQueryable<Contact> q)
        {
            return from c in q
                   let contactor = c.contactsMakers.FirstOrDefault().person.Name
                   let contactee = Organization.FullName
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
