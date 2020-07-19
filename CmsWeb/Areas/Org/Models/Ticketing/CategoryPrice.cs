using System.Diagnostics.CodeAnalysis;

namespace CmsWeb.Areas.Org.Models.Ticketing
{
    // ReSharper disable once ClassNeverInstantiated.Local
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CatPrice
    {
        public int category { get; set; }
        public decimal price { get; set; }
    }
}
