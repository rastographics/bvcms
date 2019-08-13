namespace CmsWeb.Areas.Manage.Models.Involvement
{
    public class CustomizeInvolvementModel
    {
        public CustomizeInvolvementModel()
        {
            Current = new InvolvementTabModel("InvolvementTableCurrent");
            Pending = new InvolvementTabModel("InvolvementTablePending");
            Previous = new InvolvementTabModel("InvolvementTablePrevious");
        }

        public InvolvementTabModel Current { get; set; }

        public InvolvementTabModel Pending { get; set; }

        public InvolvementTabModel Previous { get; set; }
    }
}
