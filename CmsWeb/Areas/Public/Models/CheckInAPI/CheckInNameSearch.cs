using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Models.CheckInAPI
{
    public class CheckInNameSearch
    {
        public string name = "";

        public string first = "";
        public string last = "";

        public void splitName()
        {
            Util.NameSplit(name, out first, out last);
        }
    }
}