using CmsData;
using CmsWeb.Models;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public abstract class ContactsModel : PagedTableModel<CmsData.Contact, ContactInfo>
    {
        public Person Person { get; set; }
        public int? PeopleId
        {
            get { return Person == null ? (int?)null : Person.PeopleId; }
            set { Person = DbUtil.Db.LoadPersonById(value ?? 0); }
        }

        protected ContactsModel()
            : base("Date", "desc", true)
        { }

        public abstract string AddContact { get; }
        public abstract string AddContactButton { get; }

        public IQueryable<Contact> FilteredModelList()
        {
            var u = DbUtil.Db.CurrentUser;
            var roles = u.UserRoles.Select(uu => uu.Role.RoleName.ToLower()).ToArray();
            var ManagePrivateContacts = HttpContextFactory.Current.User.IsInRole("ManagePrivateContacts");
            return from c in DbUtil.Db.Contacts
                   where (c.LimitToRole ?? "") == "" || roles.Contains(c.LimitToRole) || ManagePrivateContacts
                   select c;
        }

        public override IQueryable<Contact> DefineModelSort(IQueryable<Contact> q)
        {
            if (Direction == "asc")
            {
                switch (Sort)
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
            }
            else
            {
                switch (Sort)
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
            }

            return q;
        }
    }
}
