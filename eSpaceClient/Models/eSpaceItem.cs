namespace eSpace.Models
{
    public class eSpaceItem
    {
        public long ItemId { get; set; }
        /// <summary>
        /// e.g. Space, Resource, Service
        /// </summary>
        public string ItemType { get; set; }
        public string Name { get; set; }
    }
}
