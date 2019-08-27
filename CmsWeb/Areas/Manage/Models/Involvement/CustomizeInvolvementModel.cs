using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Areas.Manage.Models.Involvement
{
    public class CustomizeInvolvementModel
    {
        public CustomizeInvolvementModel(IEnumerable<InvolvementTabModel.OrgType> involvementTypes)
        {
            InvolvementTypeList = involvementTypes.ToList();

            Current = new InvolvementTabModel("InvolvementTableCurrent", InvolvementTypeList);
            Pending = new InvolvementTabModel("InvolvementTablePending", InvolvementTypeList);
            Previous = new InvolvementTabModel("InvolvementTablePrevious", InvolvementTypeList);
        }

        public InvolvementTabModel Current { get; set; }

        public InvolvementTabModel Pending { get; set; }

        public InvolvementTabModel Previous { get; set; }

        public List<InvolvementTabModel.OrgType> InvolvementTypeList { get; set; }
    }
}
