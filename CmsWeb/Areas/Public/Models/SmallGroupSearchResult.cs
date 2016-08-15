using System.Collections.Generic;
using CmsData;

namespace CmsWeb.Models
{
    public class SmallGroupSearchResult
    {
        public List<Organization> Organizations { get; set; }
        public bool IsInitialSearch { get; set; }
    }
}
