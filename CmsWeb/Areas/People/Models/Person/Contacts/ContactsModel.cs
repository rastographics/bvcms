using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;
using DocumentFormat.OpenXml.Drawing;

namespace CmsWeb.Areas.People.Models
{
    public abstract class ContactsModel : PagedTableModel<CmsData.Contact, ContactInfo>
    {
        public Person person;

        public ContactsModel(int id)
            : base("Date", "desc")
        {
            person = DbUtil.Db.LoadPersonById(id);
        }
        public string AddContact { get; set; }

        public IQueryable<Contact> FilteredModelList()
        {
            var u = DbUtil.Db.CurrentUser;
            var roles = u.UserRoles.Select(uu => uu.Role.RoleName.ToLower()).ToArray();
            var ManagePrivateContacts = HttpContext.Current.User.IsInRole("ManagePrivateContacts");
            return from c in DbUtil.Db.Contacts
                   where (c.LimitToRole ?? "") == "" || roles.Contains(c.LimitToRole) || ManagePrivateContacts
                   select c;
        }

        override public IQueryable<Contact> DefineModelSort(IQueryable<Contact> q)
        {
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
                {
                    case "Date":
                        return from c in q
                                   orderby c.ContactDate
                                   select c;
                    case "Type":
                        return from c in q
                                   orderby c.ContactType.Description, c.ContactDate descending 
                                   select c;
                    case "Reason":
                        return from c in q
                                   orderby c.ContactReason.Description, c.ContactDate descending 
                                   select c;
                    case "Minister":
                        return from c in q
                                   orderby c.contactsMakers.FirstOrDefault().person.Name2, c.ContactDate descending 
                                   select c;
                    case "Contactee":
                        return from c in q
                                   orderby c.contactees.FirstOrDefault().person.Name2, c.ContactDate descending 
                                   select c;
                }
            else
                switch (Pager.Sort)
                {
                    case "Date":
                        return from c in q
                                   orderby c.ContactDate descending 
                                   select c;
                    case "Type":
                        return from c in q
                                   orderby c.ContactType.Description descending, c.ContactDate
                                   select c;
                    case "Reason":
                        return from c in q
                                   orderby c.ContactReason.Description descending, c.ContactDate
                                   select c;
                    case "Minister":
                        return from c in q
                                   orderby c.contactsMakers.FirstOrDefault().person.Name2 descending, c.ContactDate
                                   select c;
                    case "Contactee":
                        return from c in q
                                   orderby c.contactees.FirstOrDefault().person.Name2 descending, c.ContactDate
                                   select c;
                }
            return q;
        }
    }
}