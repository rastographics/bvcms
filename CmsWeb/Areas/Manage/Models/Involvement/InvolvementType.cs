using System.Collections.Generic;

namespace CmsWeb.Areas.Manage.Models.Involvement
{
    public class InvolvementType
    {
        public InvolvementType()
        {
            Columns = new List<InvolvementColumn>();
        }

        public string Name { get; set; }

        public List<InvolvementColumn> Columns { get; set; }
    }
}
