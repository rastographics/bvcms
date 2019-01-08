namespace CmsWeb.Areas.Coordinator.Models
{
    public class CheckinActionDto
    {
        public const string IncrementCapacity = "IncrementCapacity";
        public const string DecrementCapacity = "DecrementCapacity";
        public const string ToggleCheckinOpen = "ToggleCheckinOpen";
        public const string SetDefaults = "SetDefaults";
        public const string SetAllDefaults = "SetAllDefaults";

        public string SelectedTimeslot { get; set; }
        public int OrganizationId { get; set; }
        public int SubgroupId { get; set; }
        public string SubgroupName { get; set; }
        public string Service { get; set; }
    }
}
