using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityExtensions;

namespace CmsData
{
    public partial class OrgFilter
    {
        public string NameFilter
        {
            get
            { 
                return $"{FirstName} {LastName}".Trim();
            }
            set
            {
                if (value == null) FirstName = LastName = "";
                string first, last;
                Util.NameSplit(value, out first, out last);
                if (first.HasValue() || last.HasValue())
                {
                    FirstName = first;
                    LastName = last;
                }
                else
                {
                    FirstName = null;
                    LastName = value; // treat FirstName as a fuzzy search
                }
            }
        }
    }
}
