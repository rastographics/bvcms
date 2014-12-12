using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.MobileAPI
{
    public class MobileRegistrationCategory
    {
        public string Title { get; set; }
        public List<MobileRegistration> Registrations { get; set; }
    }
	public class MobileRegistration
	{
	    public string Description { get; set; }
	    public int OrgId { get; set; }
        public bool UseRegisterLink2 { get; set; }
	    public DateTime RegStart { get; set; }
	    public DateTime RegEnd { get; set; }
	}
}