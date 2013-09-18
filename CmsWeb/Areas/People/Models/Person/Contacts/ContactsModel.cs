using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace CmsWeb.Areas.People.Models.Person
{
    public abstract class ContactsModel : IContacts
    {
        public CmsData.Person person;
        public PagerModel2 Pager { get; set; }

        public ContactsModel(int id)
        {
            person = DbUtil.Db.LoadPersonById(id);
            Pager = new PagerModel2(Count) {Sort = "Date", Direction = "desc" };
        }
        int? count;
        public int Count()
        {
            if (!count.HasValue)
                count = FetchContacts().Count();
            return count.Value;
        }
        public abstract IQueryable<Contact> FetchContacts();
        public abstract IEnumerable<ContactInfo> Contacts();
        public string AddContact { get; set; }

        public IQueryable<Contact> ApplySort(IQueryable<Contact> contacts)
        {
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
                {
                    case "Date":
                        contacts = from c in contacts
                                   orderby c.ContactDate
                                   select c;
                        break;
                    case "Type":
                        contacts = from c in contacts
                                   orderby c.ContactType.Description, c.ContactDate descending 
                                   select c;
                        break;
                    case "Reason":
                        contacts = from c in contacts
                                   orderby c.ContactReason.Description, c.ContactDate descending 
                                   select c;
                        break;
                    case "Minister":
                        contacts = from c in contacts
                                   orderby c.contactsMakers.FirstOrDefault().person.Name2, c.ContactDate descending 
                                   select c;
                        break;
                    case "Contactee":
                        contacts = from c in contacts
                                   orderby c.contactees.FirstOrDefault().person.Name2, c.ContactDate descending 
                                   select c;
                        break;
                }
            else
                switch (Pager.Sort)
                {
                    case "Date":
                        contacts = from c in contacts
                                   orderby c.ContactDate descending 
                                   select c;
                        break;
                    case "Type":
                        contacts = from c in contacts
                                   orderby c.ContactType.Description descending, c.ContactDate
                                   select c;
                        break;
                    case "Reason":
                        contacts = from c in contacts
                                   orderby c.ContactReason.Description descending, c.ContactDate
                                   select c;
                        break;
                    case "Minister":
                        contacts = from c in contacts
                                   orderby c.contactsMakers.FirstOrDefault().person.Name2 descending, c.ContactDate
                                   select c;
                        break;
                    case "Contactee":
                        contacts = from c in contacts
                                   orderby c.contactees.FirstOrDefault().person.Name2 descending, c.ContactDate
                                   select c;
                        break;
                }
            return contacts;
        }
    }
}