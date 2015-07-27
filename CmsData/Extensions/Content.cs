namespace CmsData
{
    public partial class Content
    {
        public string ThumbUrl => $"/Image/{ThumbID}?v={DateCreated?.Ticks ?? 0}";
    }
}
