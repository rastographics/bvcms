using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api.Lookup
{
    public class ApiLookup
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
