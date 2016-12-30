using UtilityExtensions;

namespace CmsData
{
    public partial class Content
    {
        public string ThumbUrl => $"/Image/{ThumbID}?v={DateCreated?.Ticks ?? 0}";

        public void RemoveGrammarly()
        {
            Body = Body.RemoveGrammarly();
        }
    }
}
