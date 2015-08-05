using System.ComponentModel.DataAnnotations;

namespace CmsWeb.Models.Api.Lookup
{
    [ApiMapName("FamilyPositions")]
    public class ApiFamilyPosition
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
