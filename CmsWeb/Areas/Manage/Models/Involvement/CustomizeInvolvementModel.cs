using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Areas.Manage.Models.Involvement
{
    public class CustomizeInvolvementModel
    {
        public CustomizeInvolvementModel(IEnumerable<InvolvementTabModel.OrgType> involvementTypes)
        {
            var involvementTypeList = involvementTypes.ToList();

            Current = new InvolvementTabModel("InvolvementTableCurrent", involvementTypeList);
            Pending = new InvolvementTabModel("InvolvementTablePending", involvementTypeList);
            Previous = new InvolvementTabModel("InvolvementTablePrevious", involvementTypeList);
        }

        public InvolvementTabModel Current { get; set; }

        public InvolvementTabModel Pending { get; set; }

        public InvolvementTabModel Previous { get; set; }
    }
}
