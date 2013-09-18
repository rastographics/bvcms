using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CmsWeb.Areas.People.Models.Person
{
    public class ContactInfo
    {
            public int ContactId { get; set; }
            public DateTime ContactDate { get; set; }
            public string TypeOfContact { get; set; }
            public string ContactReason { get; set; }
            public string Contactor { get; set; }
            public string Contactee { get; set; }
    }
}