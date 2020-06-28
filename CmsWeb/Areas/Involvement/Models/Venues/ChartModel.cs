using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SeatsioDotNet.Events;

namespace CmsWeb.Areas.Involvement.Models.Venues
{
    public class ChartModel
    {
        public long Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string VenueType { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public bool Archived { get; set; }
        public string PublishedVersionThumbnailUrl { get; set; }
        public string DraftVersionThumbnailUrl { get; set; }
        public List<Event> Events { get; set; }
        // public ChartValidationResult Validation { get; set; }
    }
}
