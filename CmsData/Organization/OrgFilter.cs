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
            get { return $"{FirstName} {LastName}"; }
            set
            {
                string first, last;
                Util.NameSplit(value, out first, out last);
                FirstName = first;
                LastName = last;
            }
        }
    }
}
