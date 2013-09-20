using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models.Person
{
    public abstract class ContactsModel : PagedTableModel<Contact, ContactInfo>
    {
        public CmsData.Person person;

        public ContactsModel(int id)
            : base("Date", "desc")
        {
            person = DbUtil.Db.LoadPersonById(id);
        }
        public string AddContact { get; set; }

        override public IQueryable<Contact> ApplySort()
        {
            var q = ModelList();
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
                {
                    case "Date":
                        q = from c in q
                                   orderby c.ContactDate
                                   select c;
                        break;
                    case "Type":
                        q = from c in q
                                   orderby c.ContactType.Description, c.ContactDate descending 
                                   select c;
                        break;
                    case "Reason":
                        q = from c in q
                                   orderby c.ContactReason.Description, c.ContactDate descending 
                                   select c;
                        break;
                    case "Minister":
                        q = from c in q
                                   orderby c.contactsMakers.FirstOrDefault().person.Name2, c.ContactDate descending 
                                   select c;
                        break;
                    case "Contactee":
                        q = from c in q
                                   orderby c.contactees.FirstOrDefault().person.Name2, c.ContactDate descending 
                                   select c;
                        break;
                }
            else
                switch (Pager.Sort)
                {
                    case "Date":
                        q = from c in q
                                   orderby c.ContactDate descending 
                                   select c;
                        break;
                    case "Type":
                        q = from c in q
                                   orderby c.ContactType.Description descending, c.ContactDate
                                   select c;
                        break;
                    case "Reason":
                        q = from c in q
                                   orderby c.ContactReason.Description descending, c.ContactDate
                                   select c;
                        break;
                    case "Minister":
                        q = from c in q
                                   orderby c.contactsMakers.FirstOrDefault().person.Name2 descending, c.ContactDate
                                   select c;
                        break;
                    case "Contactee":
                        q = from c in q
                                   orderby c.contactees.FirstOrDefault().person.Name2 descending, c.ContactDate
                                   select c;
                        break;
                }
            return q;
        }
    }
}