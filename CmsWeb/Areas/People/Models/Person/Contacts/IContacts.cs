using System.Collections.Generic;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models.Person
{
    public interface IContacts
    {
        PagerModel2 Pager { get; set; }
        IEnumerable<ContactInfo> Contacts();
        string AddContact { get; set; }
    }
}